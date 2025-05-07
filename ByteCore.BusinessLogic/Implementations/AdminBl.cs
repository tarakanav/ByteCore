using System;
using System.Net;
using System.Web.Configuration;
using ByteCore.BusinessLogic.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ByteCore.BusinessLogic.Implementations
{
    public class AdminBl : IAdminBl
    {
        public int GetCurrentTemperature()
        {
            var city = WebConfigurationManager.AppSettings["WeatherCity"];
            if (string.IsNullOrWhiteSpace(city))
                throw new InvalidOperationException("WeatherCity not configured.");

            var url = $"https://wttr.in/{Uri.EscapeDataString(city)}?format=j1";

            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", "curl");
                var json = client.DownloadString(url);

                var jObj = JObject.Parse(json);
                if (!(jObj["current_condition"] is JArray conds) || conds.Count == 0)
                    throw new InvalidOperationException("No current_condition found in response.");

                var tempToken = conds[0]["temp_C"];
                if (tempToken == null)
                    throw new InvalidOperationException("temp_C field missing in response.");

                var tempStr = tempToken.Value<string>();
                if (int.TryParse(tempStr, out int temp))
                {
                    return temp;
                }

                throw new InvalidOperationException("Unable to parse temperature from wttr.in response.");
            }
        }
    }
}