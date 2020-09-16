using System;
using System.Collections.Generic;
using System.Text;

namespace CommerceApp.Entity
{
    public static class Helper
    {
        public static string UrlCreater(string url)
        {
            url = url.Trim();
            url = url.ToLower();
            string newUrl = string.Empty;
            for (int i = 0; i < url.Length; i++)
            {
                if (url[i] == ' ') newUrl += "-";
                else if (url[i] == 'ı') newUrl += "i";
                else if (url[i] == 'ö') newUrl += "o";
                else if (url[i] == 'ü') newUrl += "u";
                else if (url[i] == 'ş') newUrl += "s";
                else if (url[i] == 'ç') newUrl += "c";
                else if (url[i] == 'ğ') newUrl += "g";
                else newUrl += url[i];
            }
            return newUrl;
        }
    }
}
