using System.Collections.Generic;

namespace KYC_WebPlatform.Services.Data.Interfaces
{
    public interface IStorage
    {
        Dictionary<string, List<object>> ExecuteSelectQuery(string query);
    }
}