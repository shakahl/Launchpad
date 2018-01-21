//
//  AsyncProgressReport.cs
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

namespace Launchpad.Launcher.Handlers.Protocols.Progress
{
	/// <summary>
	/// A record class for reporting progress from an asynchronous request.
	/// </summary>
	public class AsyncProgressReport
	{
		/// <summary>
		/// Gets or sets the progress fraction of the progress bar (0 to 1)
		/// </summary>
		public double Fraction { get; set; }

		/// <summary>
		/// Gets or sets the progress bar message.
		/// </summary>
		public string ProgressBarMessage { get; set; }

		/// <summary>
		/// Gets or sets the indicator message.
		/// </summary>
		public string IndicatorMessage { get; set; }
	}
}
