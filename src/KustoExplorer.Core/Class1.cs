using System;
using System.Security.Cryptography;
using System.Threading;
using Kusto.Data.Net.Client;

namespace KustoExplorer.Core
{
    public class Class1
    {

        public string GetTest(string query)
        {
            // todo: we need to be able to define a list of these
            var client = KustoClientFactory.CreateCslQueryProvider("https://ddtelinsights.kusto.windows.net/DDTelInsights;Fed=true");

            //var reader = client.ExecuteQuery(".show function VSMTI_VSMac_PublicVersionsWithFirstReleaseDate");
            var reader = client.ExecuteQuery(query);
            if (reader.RecordsAffected == 0)
            {

            }


            return "test";
        }
    }
}

