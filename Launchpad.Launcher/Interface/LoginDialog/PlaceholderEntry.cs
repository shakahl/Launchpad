using System.Linq;
using Gdk;
using Gtk;

namespace Launchpad.Launcher.Interface.LoginDialog
{
	public class PlaceholderEntry : Entry
	{
		public string PlaceholderText { get; set; }

		public bool Default { get; set; }

		private readonly Color DefaultTextColor;
		private readonly Color PlaceholderColor;

		public PlaceholderEntry()
		{
			this.DefaultTextColor = this.Style.Background(StateType.Normal);
			Color.Parse("gray", ref this.PlaceholderColor);
		}

		protected override void OnShown()
		{
			this.Text = this.PlaceholderText;
			this.ModifyText(StateType.Normal, this.PlaceholderColor);

			this.Default = true;

			base.OnShown();
		}

		protected override bool OnFocusInEvent(EventFocus evnt)
		{
			if (this.Default)
			{
				this.Text = string.Empty;

				this.ModifyText(StateType.Normal, this.DefaultTextColor);
			}

			return base.OnFocusInEvent(evnt);
		}

		protected override bool OnFocusOutEvent(EventFocus evnt)
		{
			if (string.IsNullOrEmpty(this.Text))
			{
				this.Text = this.PlaceholderText;
				this.ModifyText(StateType.Normal, this.PlaceholderColor);

				this.Default = true;
			}
			else
			{
				this.Default = false;
			}

			return base.OnFocusOutEvent(evnt);
		}
	}
}