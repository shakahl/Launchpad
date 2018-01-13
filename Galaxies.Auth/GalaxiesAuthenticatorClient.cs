//
//  GalaxiesAuthenticatorClient.cs
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

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Galaxies.Auth.Extensions;
using static Galaxies.Auth.AuthenticationResponse;

namespace Galaxies.Auth
{
	public class GalaxiesAuthenticatorClient
	{
		private static readonly HttpClient Client = new HttpClient();

		private readonly Uri BaseAuthenticationURL = new Uri("http://swgtc3.net/auth.php");

		static GalaxiesAuthenticatorClient()
		{
			Client.Timeout = TimeSpan.FromSeconds(5);
		}

		/// <summary>
		/// Attempts to log the user in using the given credentials.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <returns>The response returned by the server.</returns>
		public Task<AuthenticationResponse> LoginAsync(string username, string password) => LoginAsync(username, password, CancellationToken.None);

		/// <summary>
		/// Attempts to log the user in using the given credentials.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <param name="ct">A cancellation token, which can be used to cancel the request.</param>
		/// <returns>The response returned by the server.</returns>
		public async Task<AuthenticationResponse> LoginAsync(string username, string password, CancellationToken ct)
		{
			// Build query string
			var query = this.BaseAuthenticationURL
				.AddQuery("un", username)
				.AddQuery("pw", password);

			string response;
			try
			{
				var httpResponse = await Client.GetAsync(query, ct);
				response = await httpResponse.Content.ReadAsStringAsync();
			}
			catch (TaskCanceledException)
			{
				if (ct.IsCancellationRequested)
				{
					return Cancelled;
				}

				return AuthenticationResponse.Timeout;
			}

			if (!int.TryParse(response, out int resultValue))
			{
				return InvalidServerResponse;
			}

			var result = (AuthenticationResponse) resultValue;
			if (!Enum.IsDefined(typeof(AuthenticationResponse), result))
			{
				return InvalidServerResponse;
			}

			return result;
		}
	}
}
