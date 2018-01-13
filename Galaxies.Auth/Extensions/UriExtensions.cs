//
//  UriExtensions.cs
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
using System.Text;
using System.Web;

namespace Galaxies.Auth.Extensions
{
	public static class UriExtensions
	{
		public static Uri AddQuery(this Uri uri, string name, string value)
		{
			var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

			httpValueCollection.Remove(name);
			httpValueCollection.Add(name, value);

			var ub = new UriBuilder(uri);

			// this code block is taken from httpValueCollection.ToString() method
			// and modified so it encodes strings with HttpUtility.UrlEncode
			if (httpValueCollection.Count == 0)
			{
				ub.Query = String.Empty;
			}
			else
			{
				var sb = new StringBuilder();

				for (int i = 0; i < httpValueCollection.Count; i++)
				{
					string text = httpValueCollection.GetKey(i);
					{
						text = HttpUtility.UrlEncode(text);

						string val = (text != null) ? (text + "=") : string.Empty;
						string[] vals = httpValueCollection.GetValues(i);

						if (sb.Length > 0)
						{
							sb.Append('&');
						}

						if (vals == null || vals.Length == 0)
						{
							sb.Append(val);
						}
						else
						{
							if (vals.Length == 1)
							{
								sb.Append(val);
								sb.Append(HttpUtility.UrlEncode(vals[0]));
							}
							else
							{
								for (int j = 0; j < vals.Length; j++)
								{
									if (j > 0)
									{
										sb.Append('&');
									}

									sb.Append(val);
									sb.Append(HttpUtility.UrlEncode(vals[j]));
								}
							}
						}
					}
				}

				ub.Query = sb.ToString();
			}

			return ub.Uri;
		}
	}
}