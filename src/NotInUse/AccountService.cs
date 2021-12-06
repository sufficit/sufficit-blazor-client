using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Services
{
	public class AccountService : IAccountService
	{
		private readonly AuthenticationStateProvider _customAuthenticationProvider;
		private readonly ILocalStorageService _localStorageService;
		public AccountService(ILocalStorageService localStorageService,
		  AuthenticationStateProvider customAuthenticationProvider)
		{
			_localStorageService = localStorageService;
			_customAuthenticationProvider = customAuthenticationProvider;
		}
		public async Task<bool> LoginAsync()
		{
			string token = "copy_Past_Test_Token_in_the_article_above";
			await _localStorageService.SetItemAsync("token", token);
			(_customAuthenticationProvider as CustomAuthenticationProvider).Notify();
			return true;
		}

		public async Task<bool> LogoutAsync()
		{
			await _localStorageService.RemoveItemAsync("token");
			(_customAuthenticationProvider as CustomAuthenticationProvider).Notify();
			return true;
		}
	}
}
