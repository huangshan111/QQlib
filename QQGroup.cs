using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QQLib.Core;
using QQLib.Model;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using QQLib.Interface;

namespace QQLib
{
    public class QQGroup
    {
        public QQGroup(string qq, string password, IVCodeRecognizer recognizer)
        {
            this.qq = qq;
            this.password = password;
            this.recognizer = recognizer;
        }

        private string Cookie;
        private string qq;
        private string password;
        private IVCodeRecognizer recognizer;

        /// <summary>
        /// 从cookie中获取pt_login_sig
        /// </summary>
        /// <returns></returns>
        private string GetLoginSig()
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = "https://xui.ptlogin2.qq.com/cgi-bin/xlogin?appid=715030901&daid=73&pt_no_auth=1&s_url=http%3A%2F%2Fqun.qq.com%2F",
                // URL = "http://xui.ptlogin2.qq.com/cgi-bin/xlogin?proxy_url=http%3A//qzs.qq.com/qzone/v6/portal/proxy.html&daid=5&pt_qzone_sig=1&hide_title_bar=1&low_login=0&qlogin_auto_login=1&no_verifyimg=1&link_target=blank&appid=549000912&style=22&target=self&s_url=http%3A//qzs.qq.com/qzone/v5/loginsucc.html?para=izone&pt_qr_app=%E6%89%8B%E6%9C%BAQQ%E7%A9%BA%E9%97%B4&pt_qr_link=http%3A//z.qzone.com/download.html&self_regurl=http%3A//qzs.qq.com/qzone/v6/reg/index.html&pt_qr_help_link=http%3A//z.qzone.com/download.html",//URL     必需项    
                Method = "get",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = "",//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   
                Postdata = "",//Post数据     可选项GET时不需要写   
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "text/html",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                //ProxyPwd = "123456",//代理服务器密码     可选项    
                //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                ResultType = ResultType.String
            };
            HttpResult result = http.GetHtml(item);
            Cookie = result.Cookie;
            return CookieHelper.GetCookie(Cookie, "pt_login_sig");
        }

        /// <summary>
        /// 判断是否需要验证码
        /// </summary>
        /// <param name="pt_login_sig"></param>
        /// <returns></returns>
        private Verifier CheckIfNeedVCode(string pt_login_sig)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = "https://ssl.ptlogin2.qq.com/check?regmaster=&pt_tea=2&pt_vcode=1&uin=" + qq + "&appid=715030901&js_ver=10188&js_type=1&login_sig=" + pt_login_sig + "&u1=http%3A%2F%2Fqun.qq.com%2F&pt_uistyle=40",
                // URL = "http://check.ptlogin2.qq.com/check?regmaster=&pt_tea=1&uin=" + comboBox1.Text + "&appid=549000912&js_ver=10112&js_type=1&login_sig=" + Login_sig + "&u1=http%3A%2F%2Fqzs.qq.com%2Fqzone%2Fv5%2Floginsucc.html%3Fpara%3Dizone&r=0." + spacehelp.GetRandomString(17),//URL     必需项    
                Method = "get",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = "",//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   
                Postdata = "",//Post数据     可选项GET时不需要写   
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "text/html",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                //ProxyPwd = "123456",//代理服务器密码     可选项    
                //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                ResultType = ResultType.String
            };
            HttpResult result = http.GetHtml(item);
            string html = result.Html;

            Regex regex = new Regex("\'(.*?)\'");
            MatchCollection mc = regex.Matches(html);

            //ptui_checkVC('0','!SKN','\x00\x00\x00\x00\x29\x63\x72\x39','e4bd42ddf2a8812b702ed30755e12453b7d2eab1570e6831d67fd2c67258925ad6e05e4a695c6a0f888c8e3085132b600003803c9e8f64f8','2');
            //  ptui_checkVC('1','fRd3R27tWtAMJmM6RbX18mCBU-zYr85-OWMu3taOVxLH4od11ohQoA**','\x00\x00\x00\x00\x29\x63\x72\x39','','2');
            Verifier verifier = new Verifier(this.qq, this.recognizer);
            verifier.pt_vcode_v1 = mc[0].Groups[1].Value;
            verifier.cap_cd = mc[1].Groups[1].Value;
            verifier.uin = mc[2].Groups[1].Value;
            verifier.ptvfsession = mc[3].Groups[1].Value;
            return verifier;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="verifier"></param>
        /// <param name="pt_login_sig"></param>
        /// <returns></returns>
        private string Login(Verifier verifier, string pt_login_sig)
        {
            string p = QQJSEncrypt.GetEncryptPassword(qq, password, verifier.cap_cd);
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                //https://ssl.ptlogin2.qq.com/login?u=694383161&verifycode=!KQT&pt_vcode_v1=0&pt_verifysession_v1=3cd6f312f164d81dd59383e798a57b4d97a8626c8&p=M1rqIDUXZ&pt_randsalt=2&u1=http%3A%2F%2Fqun.qq.com%2F&ptredirect=1&h=1&t=1&g=1&from_ui=1&ptlang=2052&action=2-4-1482459085672&js_ver=10188&js_type=1&login_sig=fsdcodBCzEFhcIiag&pt_uistyle=40&aid=715030901&daid=73&
                URL = "https://ssl.ptlogin2.qq.com/login?u=" + qq + "&verifycode=" + verifier.cap_cd + "&pt_vcode_v1=" + verifier.pt_vcode_v1 + "&pt_verifysession_v1=" + verifier.ptvfsession + "&p=" + p + "&pt_randsalt=2&u1=http%3A%2F%2Fqun.qq.com%2F&ptredirect=1&h=1&t=1&g=1&from_ui=1&ptlang=2052&action=2-4-1482459085672&js_ver=10188&js_type=1&login_sig=" + pt_login_sig + "&pt_uistyle=40&aid=715030901&daid=73&",//URL     必需项    
                Method = "get",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = "",//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   
                Postdata = "",//Post数据     可选项GET时不需要写   
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "text/html",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                //ProxyPwd = "123456",//代理服务器密码     可选项    
                //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                ResultType = ResultType.String,
                Encoding = Encoding.UTF8
            };
            HttpResult result = http.GetHtml(item);
            Cookie = result.Cookie;

            Regex regex = new Regex("\'(.*?)\'");
            MatchCollection mc = regex.Matches(result.Html);
            
            return mc[2].Groups[1].Value;

            // string ss = "http://ptlogin2.qq.com/login?u=" + comboBox1.Text + "&verifycode=" + Code + "&pt_vcode_v1=0&pt_verifysession_v1=" + verifysession + "&p=" + p + "&pt_randsalt=0&u1=http%3A%2F%2Fqzs.qq.com%2Fqzone%2Fv5%2Floginsucc.html%3Fpara%3Dizone&ptredirect=0&h=1&t=1&g=1&from_ui=1&ptlang=2052&action=2-39-1422557661032&js_ver=10112&js_type=1&login_sig=" + Login_sig + "&pt_uistyle=32&aid=549000912&daid=5&pt_qzone_sig=1&";
            //MessageBox.Show(ss+"\r\n"+ss.Length);
            // MessageBox.Show(Login_sig+"\r\n"+Code+"\r\n"+Salt+"\r\n"+p);
            //MessageBox.Show(verifysession);
            //MessageBox.Show(Login_sig + Login_sig.Length + "\r\n" + Code + Code.Length + "\r\n" + comboBox1.Text + comboBox1.Text.Length + "\r\n" + verifysession + verifysession.Length + "\r\n" + p + p.Length + "\r\n");


            // MessageBox.Show(spacehelp.GetStringMid(html, "0','", "', '").Contains("登录成功！") ? spacehelp.GetStringMid(html, "_','0','", "', '") + spacehelp.GetStringMid(html, "', '", "')") : spacehelp.GetStringMid(html, ",'','0','", "', ''"));
        }

        /// <summary>
        /// 登录成功后跳转
        /// </summary>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        private string QQGroupIndex(string redirectUrl)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = redirectUrl,
                Method = "get",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = Cookie,//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   
                Postdata = "",//Post数据     可选项GET时不需要写   
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "text/html",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                //ProxyPwd = "123456",//代理服务器密码     可选项    
                //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                ResultType = ResultType.String
            };
            HttpResult result = http.GetHtml(item);
            Cookie = result.Cookie;
            return CookieHelper.GetCookie(Cookie, "skey");
        }

        /// <summary>
        /// 获取加入的群数量
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        private int GetGroupCount(string skey)
        {
            string uin = "o0" + qq;
            string bkn = QQJSEncrypt.getBkn(skey);
            HttpHelper http = new HttpHelper();
            HttpItem getGroupInfoItem = new HttpItem()
            {
                URL = "http://qun.qq.com/cgi-bin/qun_mgr/get_group_list",
                // URL = "http://qun.qq.com/cgi-bin/qunwelcome/myinfo?callback=?&bkn=" + bkn,
                Method = "post",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = "pt2gguin=" + uin + "; uin=" + uin + "; skey=" + skey + "; p_uin=" + uin + ";",//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   
                Postdata = ("bkn=" + bkn),//Post数据     可选项GET时不需要写   
                Timeout = 100000,//连接超时时间     可选项默认为100000    
                ReadWriteTimeout = 30000,//写入Post数据超时时间     可选项默认为30000   
                UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",//用户的浏览器类型，版本，操作系统     可选项有默认值   
                ContentType = "text/html",//返回类型    可选项有默认值   
                Allowautoredirect = false,//是否根据301跳转     可选项   
                //CerPath = "d:\123.cer",//证书绝对路径     可选项不需要证书时可以不写这个参数   
                //Connectionlimit = 1024,//最大连接数     可选项 默认为1024    
                ProxyIp = "",//代理服务器ID     可选项 不需要代理 时可以不设置这三个参数    
                //ProxyPwd = "123456",//代理服务器密码     可选项    
                //ProxyUserName = "administrator",//代理服务器账户名     可选项   
                ResultType = ResultType.String
            };
            HttpResult result = http.GetHtml(getGroupInfoItem);
            string groupJsonHtml = result.Html;
            GroupInfo gi = JsonConvert.DeserializeObject<GroupInfo>(groupJsonHtml);
            if (gi.join != null)
            {
                return gi.join.Count();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 抓取的入口
        /// </summary>
        /// <returns></returns>
        public int Fetch()
        {
            string pt_login_sig = GetLoginSig();

            Verifier verifier = CheckIfNeedVCode(pt_login_sig);

            //需要验证码 
            if (verifier.pt_vcode_v1 == "1")
            {
                verifier.Verify();
            }

            string redirectUrl = Login(verifier, pt_login_sig);
            string skey = QQGroupIndex(redirectUrl);
            return GetGroupCount(skey);
        }

    }
}
