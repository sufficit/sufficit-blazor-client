using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SufficitBlazorClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Services
{
	public class CustomAuthenticationProvider : AuthenticationStateProvider
	{
		private readonly ILocalStorageService _localStorageService;

		public CustomAuthenticationProvider(ILocalStorageService localStorageService)
		{
			_localStorageService = localStorageService;
		}
		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			AuthenticationState loginUser = null;
			try
			{
				string token = await _localStorageService.GetItemAsync<string>("token");
				if (string.IsNullOrEmpty(token))
				{
					var anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity() { }));
					return anonymous;
				}
				var userClaimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "Fake Authentication"));
				loginUser = new AuthenticationState(userClaimPrincipal);
				
			}
			catch { }
			return loginUser;
		}

		public void Notify()
		{
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
		}
	}

}
