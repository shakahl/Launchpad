//
//  AsyncManifestPatchService.cs
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
using System.Drawing;
using System.Threading.Tasks;
using Launchpad.Common.Enums;
using Launchpad.Common.Handlers.Manifest;
using Launchpad.Launcher.Utility;

namespace Launchpad.Launcher.Handlers.Protocols
{
	/// <summary>
	/// A TPL-based asynchronous implementation of a manifest-based patching service.
	/// </summary>
	public abstract class AsyncManifestPatchService : IAsyncPatchProvider, IAsyncBannerProvider, IAsyncChangelogProvider, IDisposable
	{
		/// <summary>
		/// Gets the configuration instance.
		/// </summary>
		protected ConfigHandler Config { get; }

		/// <summary>
		/// Gets an instance of the manifest service.
		/// </summary>
		protected ManifestHandler Manifests { get; }

		/// <summary>
		/// Gets an instance of a remote file provider.
		/// </summary>
		protected IAsyncRemoteFileProvider RemoteFileProvider { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncManifestPatchService"/> class.
		/// </summary>
		protected AsyncManifestPatchService()
		{
			this.Config = ConfigHandler.Instance;
			this.Manifests = new ManifestHandler
			(
				DirectoryHelpers.GetLocalLauncherDirectory(),
				new Uri(this.Config.Configuration.RemoteAddress.AbsoluteUri),
				this.Config.Configuration.SystemTarget
			);
		}

		/// <inheritdoc />
		public Task<bool> CanConnectAsync()
		{
			return this.RemoteFileProvider.Connect();
		}

		/// <inheritdoc />
		public Task<bool> IsPlatformAvailableAsync(ESystemTarget systemTarget)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public Task<bool> IsModuleOutdatedAsync(EModule module)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public Task DownloadModuleAsync(EModule module)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public Task UpdateModuleAsync(EModule module)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public Task VerifyModuleAsync(EModule module)
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public Task<Version> GetRemoteModuleVersionAsync(EModule module)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public Task<Version> GetLocalModuleVersionAsync(EModule module)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public Task<bool> CanProvideBannerAsync()
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public Task<Bitmap> GetBannerAsync()
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public Task<bool> CanProvideChangelogAsync()
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public Task<string> GetChangelogHTML()
		{
			throw new System.NotImplementedException();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			this.RemoteFileProvider?.Dispose();
		}
	}
}
