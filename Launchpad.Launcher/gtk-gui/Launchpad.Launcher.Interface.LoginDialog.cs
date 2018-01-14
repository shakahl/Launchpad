using Gdk;
using Gtk;

namespace Launchpad.Launcher.Interface.LoginDialog
{
	public sealed partial class LoginDialog
	{
		private VBox MainBox;
		private Alignment UsernameAlignment;
		private Alignment PasswordAlignment;

		private PlaceholderEntry UsernameEntry;
		private PlaceholderEntry PasswordEntry;

		private Alignment ButtonBoxAlignment;

		private Alignment ThrobberAlignment;
		private Label Throbber;

		private Alignment RememberUsernameAlignment;
		private CheckButton RememberUsernameCheckButton;

		private HBox ButtonBox;

		private Alignment LoginButtonAlignment;
		private Alignment CancelButtonAlignment;

		private Button LoginButton;
		private Button CancelButton;


		private void Build()
		{
			this.Title = "Login - SWG TC3";

			this.WidthRequest = 250;

			this.Resizable = false;

			CreateWidgets();

			BuildHierarchy();

			BindEvents();

			Setup();

			ShowAll();
		}

		private void Setup()
		{
			this.LoginButton.GrabDefault();

			this.RememberUsernameCheckButton.Active = this.Config.GetRememberMe();
			if (this.Config.GetRememberMe())
			{
				this.UsernameEntry.Text = this.Config.GetGalaxiesLoginUsername();
			}
		}

		private void BindEvents()
		{
			this.RememberUsernameCheckButton.Toggled += OnRememberMeToggled;
			this.LoginButton.Clicked += OnLoginClicked;
			this.CancelButton.Clicked += OnCancelClicked;
		}

		private void BuildHierarchy()
		{
			this.MainBox.PackStart(this.UsernameAlignment);
			this.MainBox.PackStart(this.PasswordAlignment);
			this.MainBox.PackStart(this.ThrobberAlignment);
			this.MainBox.PackStart(this.RememberUsernameAlignment);
			this.MainBox.PackEnd(this.ButtonBoxAlignment);

			this.UsernameAlignment.Add(this.UsernameEntry);
			this.PasswordAlignment.Add(this.PasswordEntry);

			this.ThrobberAlignment.Add(this.Throbber);

			this.RememberUsernameAlignment.Add(this.RememberUsernameCheckButton);

			this.ButtonBoxAlignment.Add(this.ButtonBox);

			this.ButtonBox.PackStart(this.CancelButtonAlignment);
			this.ButtonBox.PackEnd(this.LoginButtonAlignment);

			this.LoginButtonAlignment.Add(this.LoginButton);
			this.CancelButtonAlignment.Add(this.CancelButton);
		}

		private void CreateWidgets()
		{
			this.MainBox = this.VBox;
			this.MainBox.Spacing = 6;

			this.UsernameAlignment = new Alignment(0.5f, 0.5f, 1.0f, 1.0f)
			{
				LeftPadding = 6,
				RightPadding = 6
			};

			this.PasswordAlignment = new Alignment(0.5f, 0.5f, 1.0f, 1.0f)
			{
				LeftPadding = 6,
				RightPadding = 6
			};

			this.UsernameEntry = new PlaceholderEntry
			{
				TooltipText = "Username",
				PlaceholderText = "Username",
				ActivatesDefault = true
			};

			this.PasswordEntry = new PlaceholderEntry
			{
				TooltipText = "Password",
				PlaceholderText = "Password",
				Visibility = false,
				ActivatesDefault = true
			};

			this.ThrobberAlignment = new Alignment(1.0f, 0.5f, 1.0f, 1.0f)
			{
				LeftPadding = 6,
				RightPadding = 6
			};

			this.Throbber = new Label();

			this.RememberUsernameAlignment = new Alignment(0.5f, 0.5f, 1.0f, 1.0f)
			{
				LeftPadding = 6,
				RightPadding = 6
			};

			this.RememberUsernameCheckButton = new CheckButton
			{
				Label = "Remember me"
			};

			this.ButtonBoxAlignment = new Alignment(1.0f, 0.5f, 1.0f, 1.0f)
			{
				LeftPadding = 6,
				RightPadding = 6
			};

			this.ButtonBox = new HBox();

			this.LoginButtonAlignment = new Alignment(1.0f, 0.5f, 1.0f, 1.0f)
			{
				RightPadding = 6
			};

			this.CancelButtonAlignment = new Alignment(1.0f, 0.5f, 1.0f, 1.0f)
			{
				LeftPadding = 6
			};

			this.LoginButton = new Button
			{
				Label = "Login",
				ReceivesDefault = true,
				CanDefault = true
			};

			this.CancelButton = new Button
			{
				Label = "Cancel"
			};
		}
	}
}