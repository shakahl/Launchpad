//
//  IAsyncBannerProvider.cs
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

using System.Drawing;
using System.Threading.Tasks;

namespace Launchpad.Launcher.Handlers.Protocols
{
	/// <summary>
	/// Interface for something providing banners.
	/// </summary>
	public interface IAsyncBannerProvider
	{
		/// <summary>
		/// Determines whether or not the provider can provide a banner.
		/// </summary>
		/// <returns>true if a banner is available; otherwise, false.</returns>
		Task<bool> CanProvideBannerAsync();

		/// <summary>
		/// Retrieves the banner from the server.
		/// </summary>
		/// <returns>The banner.</returns>
		Task<Bitmap> GetBannerAsync();
	}
}
