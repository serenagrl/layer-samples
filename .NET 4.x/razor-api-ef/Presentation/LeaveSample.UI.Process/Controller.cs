using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LeaveSample.UI.Process
{
    /// <summary>
    /// Base class for UI Controllers (not the ASP.NET WEB API Controllers).
    /// </summary>
    public abstract class Controller
    {
        /// <summary>
        /// Sends a Http Get request to a URL with querystring style parameters.
        /// </summary>
        /// <typeparam name="T">The returned type from the call.</typeparam>
        /// <param name="path">The path to the service.</param>
        /// <param name="parameters">A dictionary containing the parameters and values to form the query.</param>
        /// <param name="mediaType">The media type to use i.e. application/xml or application/json.</param>
        /// <returns>An object specified in the generic type.</returns>
        protected static T HttpGet<T>(string path, Dictionary<string, object> parameters, string mediaType)
        {
            UriBuilder builder = new UriBuilder();
            builder.Path = path;
            builder.Query = string.Join("&", parameters.Where(p => p.Value != null)
                            .Select(p => string.Format("{0}={1}",
                                 HttpUtility.UrlEncode(p.Key),
                                 HttpUtility.UrlEncode(p.Value.ToString()))));

            return HttpGet<T>(builder.Uri.PathAndQuery, mediaType);
        }

        /// <summary>
        /// Sends a Http Get request to a URL with parameters separated by /.
        /// </summary>
        /// <typeparam name="T">The returned type from the call.</typeparam>
        /// <param name="path">The path to the service.</param>
        /// <param name="values">A list of parameter values to form the query.</param>
        /// <param name="mediaType">The media type to use i.e. application/xml or application/json.</param>
        /// <return
        protected static T HttpGet<T>(string path, List<object> values, string mediaType)
        {
            string query = string.Empty;
            string pathAndQuery = path.EndsWith("/") ? path : path += "/";

            if (values != null && values.Count > 0)
                query = string.Join("/", values.ToArray());

            if (!string.IsNullOrWhiteSpace(query))
                pathAndQuery += query;

            return HttpGet<T>(pathAndQuery, mediaType);
        }

        /// <summary>
        /// Sends a Http Get request to a URL.
        /// </summary>
        /// <typeparam name="T">The returned type from the call.</typeparam>
        /// <param name="pathAndQuery">The path and query to call.</param>
        /// <param name="mediaType">The media type to use i.e. application/xml or application/json.</param>
        /// <returns>An object specified in the generic type.</returns>
        private static T HttpGet<T>(string pathAndQuery, string mediaType)
        {
            T result = default(T);

            // Execute the Http call.
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["serviceUrl"]);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

                var response = client.GetAsync(pathAndQuery).Result;
                response.EnsureSuccessStatusCode();

                result = response.Content.ReadAsAsync<T>().Result;
            }

            return result;
        }
    }
}
