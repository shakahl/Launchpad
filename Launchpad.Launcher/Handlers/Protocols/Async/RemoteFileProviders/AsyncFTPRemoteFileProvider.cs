//
//  AsyncFTPRemoteFileProvider.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using Launchpad.Launcher.Handlers.Protocols.Progress;

namespace Launchpad.Launcher.Handlers.Protocols.RemoteFileProviders
{
	/// <summary>
	/// Asynchronous file provider for the FTP protocol.
	/// </summary>
	public sealed class AsyncFTPRemoteFileProvider : IAsyncRemoteFileProvider, IDisposable
	{
		private readonly IProgress<AsyncProgressReport> ProgressReporter;

		private readonly FtpClient Client;
		private readonly NetworkCredential Credentials;
		private readonly Uri BaseUri;

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncFTPRemoteFileProvider"/> class.
		/// </summary>
		/// <param name="baseUri">The base URI for all requests.</param>
		/// <param name="credentials">The credentials to use for authorizing with the remote.</param>
		/// <param name="progressReporter">The progress reporter to use.</param>
		public AsyncFTPRemoteFileProvider(Uri baseUri, NetworkCredential credentials, IProgress<AsyncProgressReport> progressReporter)
			: this(progressReporter)
		{
			this.BaseUri = baseUri;
			this.Credentials = credentials;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncFTPRemoteFileProvider"/> class.
		/// </summary>
		/// <param name="baseAddress">The base address for all requests.</param>
		/// <param name="credentials">The credentials to use for authorizing with the remote.</param>
		/// <param name="progressReporter">The progress reporter to use.</param>
		public AsyncFTPRemoteFileProvider(string baseAddress, NetworkCredential credentials, IProgress<AsyncProgressReport> progressReporter)
			: this(progressReporter)
		{
			this.Credentials = credentials;
			if (!baseAddress.StartsWith("ftp://"))
			{
				baseAddress = $"ftp://{baseAddress}";
			}

			this.BaseUri = new Uri(baseAddress);
		}

		private AsyncFTPRemoteFileProvider(IProgress<AsyncProgressReport> progressReporter)
		{
			this.ProgressReporter = progressReporter;
			this.Client = new FtpClient(this.BaseUri.LocalPath, this.Credentials);
		}

		/// <inheritdoc />
		public async Task<bool> Connect()
		{
			try
			{
				await this.Client.ConnectAsync();
			}
			catch (TaskCanceledException)
			{
				// Timeout
				return false;
			}

			return true;
		}

		/// <inheritdoc />
		public async Task<bool> DoesRemoteFileExistAsync(string path)
		{
			await EnsureConnectedAsync();

			return await this.Client.FileExistsAsync(path);
		}

		/// <inheritdoc />
		public async Task<string> ReadRemoteFileAsync(string path)
		{
			await EnsureConnectedAsync();
			using (var ftpfs = await this.Client.OpenReadAsync(path))
			{
				var buf = await ReadChunkedAsync(ftpfs, 4096, CancellationToken.None);

				return Encoding.UTF8.GetString(buf);
			}
		}

		/// <inheritdoc />
		public async Task<Stream> OpenRemoteFileAsync(string path)
		{
			await EnsureConnectedAsync();

			return await this.Client.OpenReadAsync(path);
		}

		/// <inheritdoc />
		public async Task<Stream> OpenRemoteFileAsync(string path, long contentOffset)
		{
			await EnsureConnectedAsync();

			return await this.Client.OpenReadAsync(path, contentOffset);
		}

		/// <inheritdoc />
		public async Task DownloadRemoteFile(string path, string localPath)
		{
			await EnsureConnectedAsync();

			await this.Client.DownloadFileAsync(localPath, path);
		}

		/// <inheritdoc />
		public async Task DownloadRemoteFile(string path, string localPath, long contentOffset)
		{
			await EnsureConnectedAsync();

			using (var ftps = await OpenRemoteFileAsync(path, contentOffset))
			{
				using (var fs = File.OpenWrite(localPath))
				{
					fs.Seek(contentOffset, SeekOrigin.Begin);
					await CopyChunkedAsync(ftps, fs, 4096, CancellationToken.None);
				}
			}
		}

		/// <summary>
		/// Reads the source stream into the target stream, using the specified chunk size.
		/// </summary>
		/// <param name="source">The stream to read from.</param>
		/// <param name="targetStream">The stream to write to.</param>
		/// <param name="chunkSize">The size of the chunk.</param>
		/// <param name="ct">A cancellation token.</param>
		/// <param name="progressReport">The progress reporter.</param>
		/// <returns>A byte array containing the data in the stream.</returns>
		private async Task CopyChunkedAsync
		(
			Stream source,
			Stream targetStream,
			int chunkSize,
			CancellationToken ct,
			IProgress<double> progressReport = null
		)
		{
			var offset = 0;
			while (source.Position < source.Length)
			{
				var chunk = new byte[chunkSize];
				var readBytes = await source.ReadAsync(chunk, offset, chunkSize, ct);

				await targetStream.WriteAsync(chunk, 0, chunkSize, ct);

				offset += readBytes;

				progressReport?.Report((double)offset / source.Length);
			}
		}

		/// <summary>
		/// Reads the source stream into a byte array, using the specified chunk size.
		/// </summary>
		/// <param name="source">The stream to read from.</param>
		/// <param name="chunkSize">The size of the chunk.</param>
		/// <param name="ct">A cancellation token.</param>
		/// <param name="progressReport">The progress reporter.</param>
		/// <returns>A byte array containing the data in the stream.</returns>
		private async Task<byte[]> ReadChunkedAsync
		(
			Stream source,
			int chunkSize,
			CancellationToken ct,
			IProgress<double> progressReport = null
		)
		{
			var buf = new byte[source.Length];

			var offset = 0;
			while (source.Position < source.Length)
			{
				var chunk = new byte[chunkSize];
				var readBytes = await source.ReadAsync(chunk, offset, chunkSize, ct);

				Buffer.BlockCopy(chunk, 0, buf, offset, chunkSize);

				offset += readBytes;

				progressReport?.Report((double)offset / source.Length);
			}

			return buf;
		}

		/// <summary>
		/// Ensures that the client is connected.
		/// </summary>
		/// <returns>A task that should be awaited.</returns>
		private async Task EnsureConnectedAsync()
		{
			if (this.Client.IsConnected)
			{
				return;
			}

			await this.Client.ConnectAsync();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			if (this.Client.IsConnected)
			{
				this.Client?.Disconnect();
			}

			this.Client?.Dispose();
		}
	}
}
