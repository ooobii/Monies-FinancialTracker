using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.Ajax.Utilities;

namespace FinancialTracker_Svc.Helpers
{
    public static class Util
    {
        private static HttpResponseException _errNoKey() {
            return new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) {
                ReasonPhrase = "Missing Secret",
                Content = new StringContent("An API key is required for this method, but none was provided.")
            });
        }

        public static string GetApiKeyFromRequest(HttpRequestMessage message) {
            var queryString = new System.Uri(message.RequestUri.AbsoluteUri).Query;
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(queryString);
            var key = queryDictionary.Get("api_key");
            if( string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key) ) {
                throw _errNoKey();
            }
            return key;
        }

    }
}