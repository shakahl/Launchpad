using System;
using System.Threading;
using Galaxies.Auth;
using GLib;
using Gtk;

namespace Launchpad.Launcher.Interface.LoginDialog
{
	public sealed partial class LoginDialog : Gtk.Dialog
	{
		private readonly GalaxiesAuthenticatorClient Client;
		private readonly CancellationTokenSource TokenSource;

		public AuthenticationResponse AuthResponse { get; set; }

		public bool WasSuccessful { get; private set; }
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

			this.AuthResponse = await this.Client.LoginAsync(this.UsernameEntry.Text, this.PasswordEntry.Text, this.TokenSource.Token);

			switch (this.AuthResponse)
			{
				case AuthenticationResponse.Timeout:
				{
					this.Throbber.Text = "The login request timed out.";
					this.WasSuccessful = false;
					break;
				}
				case AuthenticationResponse.InvalidServerResponse:
				{
					this.Throbber.Text = "The server responded with an invalid code.";
					this.WasSuccessful = false;
					break;
				}
				case AuthenticationResponse.OK:
				{
					this.Throbber.Text = "Login successful.";

					this.WasCancelled = false;
					this.WasSuccessful = true;

					Respond(ResponseType.Ok);

					break;
				}
				case AuthenticationResponse.InvalidUsername:
				{
					this.Throbber.Text = "Invalid username.";
					this.WasSuccessful = false;
					break;
				}
				case AuthenticationResponse.InvalidPassword:
				{
					this.Throbber.Text = "Invalid password.";
					this.WasSuccessful = false;
					break;
				}
				case AuthenticationResponse.Cancelled:
				{
					this.Throbber.Text = string.Empty;
					this.WasCancelled = true;
					this.WasSuccessful = false;
					break;
				}
			}

			UnlockUI();
		}

		private void LockUI()
		{
			this.LoginButton.Sensitive = false;

			this.UsernameEntry.Sensitive = false;
			this.PasswordEntry.Sensitive = false;
		}

		private void UnlockUI()
		{
			this.LoginButton.Sensitive = true;

			this.UsernameEntry.Sensitive = true;
			this.PasswordEntry.Sensitive = true;
		}

		private void OnCancelClicked(object sender, EventArgs e)
		{
			this.WasCancelled = true;
			this.WasSuccessful = false;

			this.TokenSource.Cancel();

			Respond(ResponseType.Cancel);
		}
	}
}