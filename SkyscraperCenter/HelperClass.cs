using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SkyscraperCenter
{
    public static class HelperClass
    {

        public static string GetHtmlFromUrl(string url)
        {
            var myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";
            var myResponse = myRequest.GetResponse();
            var sr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            var result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();

            return result;
        }
    }
}
