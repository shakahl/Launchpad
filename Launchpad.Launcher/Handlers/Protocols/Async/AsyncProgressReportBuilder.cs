//
//  AsyncProgressReportBuilder.cs
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
using System.Text;
using Launchpad.Launcher.Handlers.Protocols.Progress;

namespace Launchpad.Launcher.Handlers.Protocols
{
	/// <summary>
	/// Builder class for asynchronous progress reports.
	/// </summary>
	public sealed class AsyncProgressReportBuilder
	{
		/// <summary>
		/// Gets the filename of the builder. This is prioritized over the path.
		/// </summary>
		public string Filename { get; private set; }

		/// <summary>
		/// Gets the path of the builder.
		/// </summary>
		public string Path { get; private set; }

		/// <summary>
		/// Gets the current value of the builder.
		/// </summary>
		public double? CurrentValue { get; private set; }

		/// <summary>
		/// Gets the target value of the builder.
		/// </summary>
		public double? TargetValue { get; private set; }

		/// <summary>
		/// Gets the fraction of the builder.
		/// </summary>
		public double? Fraction { get; private set; }

		/// <summary>
		/// Gets the indicator message of the builder.
		/// </summary>
		public string IndicatorMessage { get; private set; }

		/// <summary>
		/// Sets the filename of the builder.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <returns>The builder, with the filename.</returns>
		public AsyncProgressReportBuilder WithFilename(string filename)
		{
			this.Filename = filename;
			return this;
		}

		/// <summary>
		/// Sets the path of the builder.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>The builder, with the path.</returns>
		public AsyncProgressReportBuilder WithPath(string path)
		{
			this.Path = path;
			return this;
		}

		/// <summary>
		/// Sets the current value of the builder.
		/// </summary>
		/// <param name="value">The current value.</param>
		/// <returns>The builder, with the current value.</returns>
		public AsyncProgressReportBuilder WithCurrentValue(double value)
		{
			this.CurrentValue = value;
			return this;
		}

		/// <summary>
		/// Sets the indicator target value of the builder.
		/// </summary>
		/// <param name="target">The target value.</param>
		/// <returns>The builder, with the target value.</returns>
		public AsyncProgressReportBuilder WithTargetValue(double target)
		{
			this.TargetValue = target;
			return this;
		}

		/// <summary>
		/// Sets the fraction of the builder.
		/// </summary>
		/// <param name="fraction">The fraction.</param>
		/// <returns>The builder, with the fraction.</returns>
		public AsyncProgressReportBuilder WithFraction(double fraction)
		{
			this.Fraction = fraction;
			return this;
		}

		/// <summary>
		/// Sets the indicator message of the builder.
		/// </summary>
		/// <param name="indicatorMessage">The message.</param>
		/// <returns>The builder, with the message.</returns>
		public AsyncProgressReportBuilder WithIndicatorMessage(string indicatorMessage)
		{
			this.IndicatorMessage = indicatorMessage;
			return this;
		}

		/// <summary>
		/// Builds the final progress report.
		/// </summary>
		/// <returns>The report.</returns>
		public AsyncProgressReport Build()
		{
			// Special cases
			if (!this.Fraction.HasValue)
			{
				if (this.CurrentValue.HasValue && this.TargetValue.HasValue)
				{
					this.Fraction = this.CurrentValue / this.TargetValue;
				}
			}

			var progressBarMessage = new StringBuilder();
			if (!(this.Filename is null))
			{
				progressBarMessage.Append(this.Filename);
			}
			else if (!(this.Path is null))
			{
				progressBarMessage.Append(this.Path);
			}

			if (this.CurrentValue.HasValue && this.TargetValue.HasValue)
			{
				progressBarMessage.Append($" - ({this.CurrentValue}/{this.TargetValue})");
			}

			return new AsyncProgressReport
			{
				Fraction = this.Fraction ?? 0,
				IndicatorMessage = this.IndicatorMessage,
				ProgressBarMessage = progressBarMessage.ToString()
			};
		}
	}
}
