using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SufficitBlazorClient
{
    public class CustomUserAccount : RemoteUserAccount
    {
        [JsonPropertyName("amr")]
        public string[] AuthenticationMethod { get; set; }

        [JsonPropertyName("sub")]
        public Guid ID { get; set; }
    }   
}
