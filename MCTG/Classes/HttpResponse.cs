using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Classes
{
    public class HttpResponse (int statusCode, string content, Dictionary<string, string>? headers = null)
    {

        public int statusCode { get; set; } = statusCode;
        public string ContentType { get;} = "application/json";
        public string Content { get; set; } = content;
        public Dictionary<string, string>? Headers { get; set; } = headers;

        public int getStatusCode()
        {
            return this.statusCode;
        }

        public string GetContentType()
        {
            return this.ContentType;
        }

        public string GetContent()
        {
            return this.Content;
        }

        public Dictionary<string,string> GetHeader()
        {
            return this.Headers;
        }
    }
}
