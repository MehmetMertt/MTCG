using System;
using System.Collections.Generic;

namespace MCTG.Classes
{
    public class HttpResponse
    {
        // Constructor parameters
        public HttpResponse(int statusCode, string content, Dictionary<string, string>? headers = null)
        {
            // Set the default headers
            this.Headers = new Dictionary<string, string>()
            {
                { "Content-Type", "application/json" }
            };

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    this.Headers[header.Key] = header.Value; // Overwrite default or add new headers
                }
            }

            this.StatusCode = statusCode;
            this.Content = content;
        }

        // Properties
        public int StatusCode { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        // Methods to retrieve response data
        public int GetStatusCode()
        {
            return this.StatusCode;
        }

        public string GetContentType()
        {
            return this.Headers.ContainsKey("Content-Type") ? this.Headers["Content-Type"] : "application/json";
        }

        public string GetContent()
        {
            return this.Content;
        }

        public Dictionary<string, string> GetHeaders()
        {
            return this.Headers;
        }

        public void AddHeader(string key, string value)
        {
            this.Headers[key] = value; // Adds or updates a header
        }
    }
}