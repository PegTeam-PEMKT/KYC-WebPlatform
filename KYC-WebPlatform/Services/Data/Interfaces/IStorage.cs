using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KYC_WebPlatform.Services.Data.Interfaces
{
    public interface IStorage
    {
        List<object> ExecuteSelectQuery(string query);
    }
}