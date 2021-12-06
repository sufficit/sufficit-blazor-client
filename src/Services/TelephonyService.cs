using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sufficit.EndPoints.Configuration;
using Sufficit.Telephony;
using SufficitBlazorClient.Models.Telephony;
using SufficitBlazorClient.Pages.Telephony;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SufficitBlazorClient.Services
{
    public class TelephonyService
    {
        private readonly ILogger _logger; 
        private readonly EndPointsAPIOptions _endPointsAPIOptions;
        private readonly HttpClient _httpClient;

        public TelephonyService(ILogger<TelephonyService> logger, IOptions<EndPointsAPIOptions> endPointsAPIOptions, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _endPointsAPIOptions = endPointsAPIOptions.Value;
            _httpClient = clientFactory.CreateClient(_endPointsAPIOptions.ClientId);
        }

        public async Task<IEnumerable<ICallRecordBasic>> CallSearchAsync(CallSearchParameters parameters, CancellationToken cancellationToken = default)
        {
            string requestEndpoint = "/telephony/calls";
            string requestParams = $"idcontext={parameters.IDContext}&start={parameters.Start}";

            string requestUri = $"{requestEndpoint}?{requestParams}";
            return await _httpClient.GetFromJsonAsync<CallRecord[]>(requestUri, cancellationToken);
        }
    }
}
