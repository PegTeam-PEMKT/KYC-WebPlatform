using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;


namespace KYC_WebPlatform.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        /*public async Task<string> SendRequestAsync(string Name)
        {
            string url = "https://test.pegasus.co.ug:9108/testpegasusaggregation/sanctionLV1/"; // Replace with your actual endpoint URL

            // Create the request object
            var request = new SanctionRequest
            {
                Schema = "Person",
                Name = Name,
                RequestType = "Sanction",
                Threshold = "0.9"
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


                *//*                // Deserialize the JSON response into an ApiResponse object
                                ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);*//*

                // Deserialize the JSON response into a list of ApiResponse objects

                
  *//*              List<SanctionResponse> responseList = JsonConvert.DeserializeObject<List<SanctionResponse>>(jsonResponse);*//*

                return jsonResponse;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }*/


        public SanctionResponse SendRequestAsync(string Name)
        {
            string url = "https://test.pegasus.co.ug:9108/testpegasusaggregation/sanctionLV1/"; // Replace with your actual endpoint URL



            try
            {

                // Create the request object
                SanctionRequest request = new SanctionRequest()
                {
                    Schema = "Person",
                    Name = Name,
                    RequestType = "Sanction",
                    Threshold = "0.9"
                };



                // Serialize the request object to JSON
                string jsonString = JsonConvert.SerializeObject(request, Formatting.Indented);

                // Prepare the request content
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                using (HttpClient httpClient = new HttpClient())
                {
                    SanctionResponse[] results = null;

                    // Send the HTTP POST request
                    HttpResponseMessage response = httpClient.PostAsync(url, content).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return new SanctionResponse
                        {
                            StatusCode = "404"
                        };
                    }

                    Debug.WriteLine("\n\n\n******Response befor ReadAsStringAsync\n" + response + "\n\n\n\n");

                    try
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;


                        Debug.WriteLine("\n\n\n******Response after ReadAsStringAsync before Deserialisation" + jsonResponse + "\n\n\n\n");
                        /*   if (jsonResponse == null)
                           {
                               return new SanctionResponse
                               {
                                   StatusCode = "23",
                                   StatusDescription = "NOT EXISTS",
                                   Sanctioned = false,
                                   Score = 0
                               };
                           }

       */

                        results = JsonConvert.DeserializeObject<SanctionResponse[]>(jsonResponse);

                        Debug.WriteLine("\n\n\n******Response after Deserialisation  before returning" + results[0].ToString() + "\n\n\n\n");
                        return results[0];
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("From SendRequestAsync: " + ex.Message);
                        return new SanctionResponse
                        {

                            StatusCode = "23",
                            StatusDescription = "NOT EXISTS",
                            Sanctioned = false,
                            Score = 0
                        };

                    }

                    /*                    var results = JsonConvert.DeserializeObject<SanctionResponse[]>(jsonResponse);

                                        Debug.WriteLine("\n\n\n******Response after Deserialisation  before returning" + results[0].ToString() + "\n\n\n\n");
                    */

                    //  return results[0];
                }

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }


    }
}