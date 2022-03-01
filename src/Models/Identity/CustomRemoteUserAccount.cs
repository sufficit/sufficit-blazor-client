using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Sufficit.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SufficitBlazorClient.Models.Identity
{
    public class CustomRemoteUserAccount : RemoteUserAccount
    {
        [JsonPropertyName("type")]
        public string Type => this.GetType().Name;

        [JsonPropertyName("amr")]
        public string[] AuthenticationMethod { get; set; }

        [JsonPropertyName("sub")]
        public string Id { get; set; }

        [JsonIgnore]
        public Guid ID { get { if (Guid.TryParse(Id, out Guid _id)) return _id; return Guid.Empty; } }

        [JsonPropertyName("name")] 
        public string Name { get; set; }

        [JsonPropertyName("preferred_username")] 
        public string Username { get; set; }

        [JsonPropertyName("role")]
        public string[] Roles { get; set; }

        [JsonPropertyName("token")]
        public AccessToken Token { get; set; }

        [JsonPropertyName("policies")]
        public IEnumerable<UserPolicy> Policies { get; set; }
    }
}
