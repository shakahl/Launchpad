//
//  IAsyncRemoteFileProvider.cs
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
using System.Threading.Tasks;

namespace Launchpad.Launcher.Handlers.Protocols
{
	/// <summary>
	/// Interface for remote file providers.
	/// </summary>
	public interface IAsyncRemoteFileProvider : IDisposable
	{
		/// <summary>
		/// Connects to the remote server, initializing the file provider.
		/// </summary>
		/// <returns>true if the connection was successful; otherwise, false.</returns>
		Task<bool> Connect();

		/// <summary>
		/// Determines whether or not a file exists at the given path.
		/// </summary>
		/// <param name="path">The path of the file on the remote.</param>
		/// <returns>true if the file exists; otherwise, false.</returns>
		Task<bool> DoesRemoteFileExistAsync(string path);

		/// <summary>
		/// Reads the contents of the remote file as a string.
		/// </summary>
		/// <param name="path">The path of the file on the remote.</param>
		/// <returns>The contents of the file.</returns>
		Task<string> ReadRemoteFileAsync(string path);

		/// <summary>
		/// Opens a stream to the remote file.
		/// </summary>
		/// <param name="path">The path of the file on the remote.</param>
		/// <returns>A stream with the contents of the file.</returns>
		Task<Stream> OpenRemoteFileAsync(string path);

		/// <summary>
		/// Opens a stream to the remote file.
		/// </summary>
		/// <param name="path">The path of the file on the remote.</param>
		/// <param name="contentOffset">The offset at which to start reading and writing.</param>
		/// <returns>A stream with the contents of the file.</returns>
		Task<Stream> OpenRemoteFileAsync(string path, long contentOffset);

		/// <summary>
		/// Downloads the file at the given remote path to given local path, overwriting any existing file.
		/// </summary>
		/// <param name="path">The path of the file on the remote.</param>
		/// <param name="localPath">The local path where the file should be downloaded.</param>
		/// <returns>A task that should be awaited.</returns>
		Task DownloadRemoteFile(string path, string localPath);

		/// <summary>
		/// Downloads the file at the given remote path to given local path, starting at the offset and overwriting
		/// data from that point onward.
		/// </summary>
		/// <param name="path">The path of the file on the remote.</param>
		/// <param name="localPath">The local path where the file should be downloaded.</param>
		/// <param name="contentOffset">The offset at which to start reading and writing.</param>
		/// <returns>A task that should be awaited.</returns>
		Task DownloadRemoteFile(string path, string localPath, long contentOffset);
	}
}
