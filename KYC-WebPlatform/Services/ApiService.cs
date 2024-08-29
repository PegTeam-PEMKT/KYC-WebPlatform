using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;


namespace KYC_WebPlatform.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> SendRequestAsync(string Name)
        {
            string url = "https://test.pegasus.co.ug:9108/testpegasusaggregation/sanctionLV1/"; // Replace with your actual endpoint URL

            // Create the request object
            var request = new SanctionRequest
            {
                Schema = "Person",
                Name = Name,
                RequestType = "Sanction",
                Threshold = "0.7"
            };

            // Serialize the request object to JSON
            string jsonString = JsonConvert.SerializeObject(request);

            // Prepare the request content
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            try
            {
                // Send the HTTP POST request
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                string jsonResponse = await response.Content.ReadAsStringAsync();


                /*                // Deserialize the JSON response into an ApiResponse object
                                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);*/

                // Deserialize the JSON response into a list of ApiResponse objects

                
  /*              List<SanctionResponse> responseList = JsonConvert.DeserializeObject<List<SanctionResponse>>(jsonResponse);*/

                return jsonResponse;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }


    }
}