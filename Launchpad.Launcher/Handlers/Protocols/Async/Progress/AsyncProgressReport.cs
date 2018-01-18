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
