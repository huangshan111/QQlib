using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Noesis.Javascript;

namespace QQLib.Core
{
    public class QQJSEncrypt
    {
        /// <summary>
        ///  qq 群管理密码登录加密
        /// </summary>
        /// <param name="qq">QQ号码</param>
        /// <param name="password">QQ密码</param>
        /// <param name="code">验证码 也就是cap_cd</param>
        /// <returns></returns>
        public static string GetEncryptPassword(string qq, string password, string code)
        {
            JavascriptContext context = new JavascriptContext();

            // Setting external parameters for the context
            context.SetParameter("uid", qq);
            context.SetParameter("pwd", password);
            context.SetParameter("code", code);

            // Script
            string script = File.ReadAllText("QQRSA.txt");

            // Running the script
            context.Run(script);

            // Getting a parameter
            return Convert.ToString(context.GetParameter("result"));
        }

        /// <summary>
        /// 获取查询群组信息的bkn
        /// </summary>
        /// <param name="skey">从cookie里得到的skey</param>
        /// <returns></returns>
        public static string getBkn(string skey)
        {
            JavascriptContext context = new JavascriptContext();

            // Setting external parameters for the context

            context.SetParameter("skey", skey);

            // Script
            string script = File.ReadAllText("bkn.txt");

            // Running the script
            context.Run(script);

            // Getting a parameter
            return Convert.ToString(context.GetParameter("bkn"));
        }
    }
}