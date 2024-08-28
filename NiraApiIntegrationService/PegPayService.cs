using System;
using NiraApiIntegrationService.ug.co.pegasus.test;

namespace NiraApiIntegrationService
{
    public class PegPayService
    {
        public string QueryCustomerDetails(string dateOfBirth, string documentId, string givenName,
                                           string utility, string vendorCode, string password,
                                           string nationalId, string surname)
        {
            try
            {
                var client = new PegPay();
                var request = new QueryRequest
                {
                    QueryField1 = dateOfBirth,
                    QueryField2 = documentId,
                    QueryField3 = givenName,
                    QueryField4 = utility,
                    QueryField5 = vendorCode,
                    QueryField6 = password,
                    QueryField7 = nationalId,
                    QueryField10 = surname
                };

                var response = client.QueryCustomerDetails(request);

                return $"Status Code: {response.ResponseField6}\n" +
                       $"Status Description: {response.ResponseField7}\n" +
                       $"Boolean Field: {response.ResponseField27}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
