using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQLib.Core
{
    public static class CookieHelper
    {
        public static string GetCookie(string cookieStr, string key)
        {
            string value = string.Empty;
            string[] cookies = cookieStr.Split(';');
            foreach (string cookie in cookies)
            {
                if (cookie.Contains(key))
                {
                    string[] keyValue = cookie.Split('=');
                    if (keyValue.Length == 2)
                    {
                        value = keyValue[1];
                        break;
                    }
                }
            }
            return value;
        }
    }
}
