//
//  IAsyncPatchProvider.cs
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Launchpad.Common.Enums;
using Launchpad.Common.Handlers.Manifest;

namespace Launchpad.Launcher.Handlers.Protocols
{
	/// <summary>
	/// Interface for asynchronous patching protocols.
	/// </summary>
	public interface IAsyncPatchProvider
	{
		/// <summary>
		/// Determines whether or not the patch server can be connected to.
		/// </summary>
		/// <returns>true if a connection can be established; otherwise, false.</returns>
		Task<bool> CanConnectAsync();

		/// <summary>
		/// Determines whether or not files are provided by the server for the given platform.
		/// </summary>
		/// <param name="systemTarget">The platform.</param>
		/// <returns>true if files are provided; otherwise, false.</returns>
		Task<bool> IsPlatformAvailableAsync(ESystemTarget systemTarget);

		/// <summary>
		/// Determines whether or not the given module is outdated and requires patching.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <returns>true if the module is outdated; otherwise, false.</returns>
		Task<bool> IsModuleOutdatedAsync(EModule module);

		/// <summary>
		/// Downloads the latest version of the specified module.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <returns>A task that should be awaited.</returns>
		Task DownloadModuleAsync(EModule module);

		/// <summary>
		/// Updates the given module to the latest version.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <returns>A task that should be awaited.</returns>
		Task UpdateModuleAsync(EModule module);

		/// <summary>
		/// Verifies and repairs the local files of the given module.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <returns>A task that should be awaited.</returns>
		Task VerifyModuleAsync(EModule module);

		/// <summary>
		/// Gets the remote version of the given module.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <returns>The version.</returns>
		Task<Version> GetRemoteModuleVersionAsync(EModule module);

		/// <summary>
		/// Gets the local version of the given module.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <returns>The version.</returns>
		Task<Version> GetLocalModuleVersionAsync(EModule module);
	}
}
