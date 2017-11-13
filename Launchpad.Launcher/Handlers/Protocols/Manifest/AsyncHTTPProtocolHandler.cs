//
//  HTTPProtocolHandler.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2016 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See themanifest
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Launchpad.Common;
using Launchpad.Common.Enums;

namespace Launchpad.Launcher.Handlers.Protocols.Manifest
{
	/// <summary>
	/// HTTP protocol handler. Patches the launcher and game using the
	/// HTTP/HTTPS protocol.
	/// </summary>
	internal sealed class AsyncHTTPProtocolHandler : AsyncManifestBasedProtocolHandler
	{
		/// <summary>
		/// Logger instance for this class.
		/// </summary>
		private static readonly ILog Log = LogManager.GetLogger(typeof(HTTPProtocolHandler));

		private readonly HttpClient Client;

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncHTTPProtocolHandler"/>.
		/// </summary>
		public AsyncHTTPProtocolHandler()
		{
			this.Client = new HttpClient
			(
				new HttpClientHandler
				{
					Credentials = new NetworkCredential
					(
						this.Config.GetRemoteUsername(),
						this.Config.GetRemotePassword()
					),
					ClientCertificateOptions = ClientCertificateOption.Automatic
				}
			);
		}

		/// <summary>
		/// Determines whether this instance can provide patches. Checks for an active connection to the
		/// patch provider (file server, distributed hash tables, hyperspace compression waves etc.)
		/// </summary>
		/// <returns><c>true</c> if this instance can provide patches; otherwise, <c>false</c>.</returns>
		public override async Task<bool> CanPatchAsync(CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();
			Log.Info("Pinging remote patching server to determine if we can connect to it.");

			this.Client.Timeout = TimeSpan.FromSeconds(4);
			try
			{
				using (var response = await this.Client.GetAsync(this.Config.GetBaseHTTPUrl(), ct))
				{
					if (!response.IsSuccessStatusCode)
					{
						Log.Warn($"Could not successfully connect to the patch server: {response.ReasonPhrase}");
						return false;
					}
				}

				return true;
			}
			catch (OperationCanceledException)
			{
				Log.Warn("Unable to connect to remote patch server.");
				return false;
			}
		}

		/// <summary>
		/// Determines whether the protocol can provide patches and updates for the provided platform.
		/// </summary>
		/// <returns><c>true</c> if the platform is available; otherwise, <c>false</c>.</returns>
		public override async Task<bool> IsPlatformAvailableAsync(CancellationToken ct, ESystemTarget platform)
		{
			string remote = $"{this.Config.GetBaseHTTPUrl()}/game/{platform}/.provides";

			return await DoesRemoteDirectoryOrFileExistAsync(remote);
		}

		/// <summary>
		/// Determines whether this protocol can provide access to a changelog.
		/// </summary>
		/// <returns><c>true</c> if this protocol can provide a changelog; otherwise, <c>false</c>.</returns>
		public override async Task<bool> CanProvideChangelogAsync(CancellationToken ct)
		{
			return await Task.FromResult(false);
		}

		/// <summary>
		/// Gets the changelog.
		/// </summary>
		/// <returns>The changelog.</returns>
		public override async Task<string> GetChangelogSourceAsync(CancellationToken ct)
		{
			return await Task.FromResult(string.Empty);
		}

		/// <summary>
		/// Determines whether this protocol can provide access to a banner for the game.
		/// </summary>
		/// <returns><c>true</c> if this instance can provide banner; otherwise, <c>false</c>.</returns>
		public override async Task<bool> CanProvideBannerAsync(CancellationToken ct)
		{
			string bannerURL = $"{this.Config.GetBaseHTTPUrl()}/launcher/banner.png";

			return await DoesRemoteDirectoryOrFileExistAsync(bannerURL);
		}

		/// <summary>
		/// Gets the banner.
		/// </summary>
		/// <returns>The banner.</returns>
		public override async Task<Bitmap> GetBannerAsync(CancellationToken ct)
		{
			string bannerURL = $"{this.Config.GetBaseHTTPUrl()}/launcher/banner.png";
			string localBannerPath = $"{Path.GetTempPath()}/banner.png";

			await DownloadRemoteFileAsync(bannerURL, localBannerPath, ct);
			return new Bitmap(localBannerPath);
		}

		/// <summary>
		/// Downloads a remote file to a local file path.
		/// </summary>
		/// <param name="url">The remote url of the file..</param>
		/// <param name="localPath">Local path where the file is to be stored.</param>
		/// <param name="ct"></param>
		/// <param name="totalSize">Total size of the file as stated in the manifest.</param>
		/// <param name="contentOffset">Content offset. If nonzero, appends data to an existing file.</param>
		protected override async Task DownloadRemoteFileAsync(string url, string localPath, CancellationToken ct, long totalSize = 0, long contentOffset = 0)
		{
			// Early bail out
			ct.ThrowIfCancellationRequested();

			// Clean the url string
			string remoteURL = url.Replace(Path.DirectorySeparatorChar, '/');

			try
			{
				this.Client.DefaultRequestHeaders.Range.Ranges.Clear();
				if (contentOffset > 0)
				{
					this.Client.DefaultRequestHeaders.Range.Unit = "bytes";
					this.Client.DefaultRequestHeaders.Range.Ranges.Add(new RangeItemHeaderValue(contentOffset, totalSize));
				}

				using (var rs = await this.Client.GetStreamAsync(remoteURL))
				{
					using (var fs = File.Open(localPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
					{
						if (contentOffset > 0)
						{
							fs.Seek(contentOffset, SeekOrigin.Begin);
						}

						await rs.CopyToAsync(fs);
					}
				}
			}
			catch (OperationCanceledException ocex)
			{
				Log.Error($"Request failed: {ocex}");
				throw;
			}
		}

		/// <summary>
		/// Reads the string content of a remote file. The output is scrubbed
		/// of all \r, \n and \0 characters before it is returned.
		/// </summary>
		/// <returns>The contents of the remote file.</returns>
		/// <param name="url">The remote url of the file.</param>
		/// <param name="useAnonymousLogin">If set to <c>true</c> use anonymous login.</param>
		protected override async Task<string> ReadRemoteFileAsync(CancellationToken ct, string url, bool useAnonymousLogin = false)
		{
			// Early bail out
			ct.ThrowIfCancellationRequested();

			string remoteURL = url.Replace(Path.DirectorySeparatorChar, '/');

			try
			{
				return await this.Client.GetStringAsync(remoteURL);
			}
			catch (OperationCanceledException ocex)
			{
				Log.Error($"Request failed: {ocex}");
				throw;
			}
		}

		/// <summary>
		/// Checks if the provided path points to a valid directory or file.
		/// </summary>
		/// <returns><c>true</c>, if the directory or file exists, <c>false</c> otherwise.</returns>
		/// <param name="url">The remote url of the directory or file.</param>
		private async Task<bool> DoesRemoteDirectoryOrFileExistAsync(string url)
		{
			string cleanURL = url.Replace(Path.DirectorySeparatorChar, '/');

			try
			{
				using (var response = await this.Client.GetAsync(cleanURL, HttpCompletionOption.ResponseHeadersRead))
				{
					if (response.StatusCode != HttpStatusCode.OK)
					{
						return false;
					}
				}
			}
			catch (OperationCanceledException ocex)
			{
				Log.Error($"Request failed: {ocex}");
				throw;
			}

			return true;
		}
	}
}

