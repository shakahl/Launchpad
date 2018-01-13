//
//  GalaxiesAuthenticatorClientTests.cs
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
using System.Threading.Tasks;
using Galaxies.Auth.Tests.TestBases;
using Xunit;
using static Galaxies.Auth.AuthenticationResponse;

namespace Galaxies.Auth.Tests
{
	public class GalaxiesAuthenticatorClientTests
	{
		public class LoginAsync : GalaxiesAuthenticatorClientTestBase
		{
			[Fact]
			public async Task InvalidUsernameReturnsInvalidUsername()
			{
				var actual = await this.Client.LoginAsync("lorem", this.ValidPassword);

				Assert.Equal(InvalidUsername, actual);
			}

			[Fact]
			public async Task NullUsernameReturnsInvalidUsername()
			{
				var actual = await this.Client.LoginAsync(null, this.ValidPassword);

				Assert.Equal(InvalidUsername, actual);
			}

			[Fact]
			public async Task EmptyUsernameReturnsInvalidUsername()
			{
				var actual = await this.Client.LoginAsync(string.Empty, this.ValidPassword);

				Assert.Equal(InvalidUsername, actual);
			}

			[Fact]
			public async Task InvalidPasswordReturnsInvalidPassword()
			{
				var actual = await this.Client.LoginAsync(this.ValidUsername, "ipsum");

				Assert.Equal(InvalidPassword, actual);
			}

			[Fact]
			public async Task NullPasswordReturnsInvalidPassword()
			{
				var actual = await this.Client.LoginAsync(this.ValidUsername, null);

				Assert.Equal(InvalidPassword, actual);
			}

			[Fact]
			public async Task EmptyPasswordReturnsInvalidPassword()
			{
				var actual = await this.Client.LoginAsync(this.ValidUsername, string.Empty);

				Assert.Equal(InvalidPassword, actual);
			}

			[Fact]
			public async Task ValidCredentialsReturnsOK()
			{
				var actual = await this.Client.LoginAsync(this.ValidUsername, this.ValidPassword);

				Assert.Equal(OK, actual);
			}
		}
	}
}