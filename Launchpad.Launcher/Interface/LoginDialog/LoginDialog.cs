using System;
using System.Configuration;
using System.Threading;
using Galaxies.Auth;
using GLib;
using Gtk;
using Launchpad.Launcher.Handlers;

namespace Launchpad.Launcher.Interface.LoginDialog
{
	public sealed partial class LoginDialog : Gtk.Dialog
	{
		private readonly ConfigHandler Config = ConfigHandler.Instance;

		private readonly GalaxiesAuthenticatorClient Client;
		private readonly CancellationTokenSource TokenSource;

		public AuthenticationResponse AuthResponse { get; set; }

		public bool WasCancelled { get; private set; }

		public LoginDialog()
		{
			this.Client = new GalaxiesAuthenticatorClient();
			this.TokenSource = new CancellationTokenSource();

			Build();
		}

		private async void OnLoginClicked(object sender, EventArgs e)
		{
			LockUI();

			this.Throbber.Text = "Logging in...";

			this.AuthResponse = await this.Client.LoginAsync(this.UsernameEntry.GetText(), this.PasswordEntry.GetText(), this.TokenSource.Token);

			switch (this.AuthResponse)
			{
				case AuthenticationResponse.Timeout:
				{
					this.Throbber.Text = "The login request timed out.";
					break;
				}
				case AuthenticationResponse.InvalidServerResponse:
				{
					this.Throbber.Text = "The server responded with an invalid code.";
					break;
				}
				case AuthenticationResponse.OK:
				{
					this.Throbber.Text = "Login successful.";

					this.WasCancelled = false;

					// Save the username
					if (this.Config.GetRememberMe())
					{
						this.Config.SetGalaxiesLoginUsername(this.UsernameEntry.GetText());
					}

					Respond(ResponseType.Ok);

					break;
				}
				case AuthenticationResponse.InvalidUsername:
				{
					this.Throbber.Text = "Invalid username.";
					break;
				}
				case AuthenticationResponse.InvalidPassword:
				{
					this.Throbber.Text = "Invalid password.";
					break;
				}
				case AuthenticationResponse.Cancelled:
				{
					this.Throbber.Text = string.Empty;
					this.WasCancelled = true;
					break;
				}
			}

			UnlockUI();
		}

		private void LockUI()
		{
			this.LoginButton.Sensitive = false;

			this.RememberUsernameCheckButton.Sensitive = false;

			this.UsernameEntry.Sensitive = false;
			this.PasswordEntry.Sensitive = false;
		}

		private void UnlockUI()
		{
			this.LoginButton.Sensitive = true;

			this.RememberUsernameCheckButton.Sensitive = true;

			this.UsernameEntry.Sensitive = true;
			this.PasswordEntry.Sensitive = true;
		}

		private void OnCancelClicked(object sender, EventArgs e)
		{
			this.WasCancelled = true;

			this.TokenSource.Cancel();

			Respond(ResponseType.Cancel);
		}

		private void OnRememberMeToggled(object sender, EventArgs e)
		{
			this.Config.SetRememberMe(this.RememberUsernameCheckButton.Active);
		}
	}
}