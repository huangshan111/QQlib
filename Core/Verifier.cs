using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using QQLib.Model;
using QQLib.Interface;
using System.Text.RegularExpressions;
using System.Net;

namespace QQLib.Core
{
    public class Verifier
    {
        public string pt_vcode_v1 { get; set; }
        public string cap_cd { get; set; }
        public string uin { get; set; }
        public string ptvfsession { get; set; }

        private int times;
        private string qq;
        private IVCodeRecognizer vcodeRecognizer;

        public Verifier(string qq, IVCodeRecognizer vcodeRecognizer)
        {
            this.qq = qq;
            this.vcodeRecognizer = vcodeRecognizer;
            this.times = 0;
        }

        /// <summary>
        /// 验证的入口方法
        /// </summary>
        public bool Verify()
        {
            bool b = false;
            string sess = Getsess();
            string vsig = GetVsigFromPage(sess);

            while (times < 3)
            {
                if (times != 0)
                {
                    vsig = GetVsig(sess);
                }

                byte[] vcodeImageBuffer = get_verify_image(vsig, sess);
                string vcode = this.vcodeRecognizer.Recognize(vcodeImageBuffer);
                if (VerifyVcode(vsig, vcode, sess))
                {
                    b = true;
                    break;
                }
                else
                {
                    this.times++;
                }
            }
            return b;
        }

        //新增逻辑 获取sess
        private string Getsess()
        {
            string sess = "";
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = "https://ssl.captcha.qq.com/cap_union_new_gettype?aid=715030901&asig=&captype=&protocol=https&clientype=2&disturblevel=&apptype=2&curenv=inner&uid=" + this.qq + "&cap_cd=" + this.cap_cd + "&lang=2052&callback=_aq_863628",
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
            string html = result.Html;
            if (!string.IsNullOrEmpty(html))
            {
                Regex regex = new Regex("\"sess\":\"(.*)\"");
                Match match = regex.Match(html);
                sess = match.Groups[1].Value;
            }
            return sess;
        }

        /// <summary>
        /// 1.获取Vsig 从一个页面里面
        /// </summary>
        /// <returns></returns>
        private string GetVsigFromPage(string sess)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = "https://ssl.captcha.qq.com/cap_union_new_show?aid=715030901&asig=&captype=&protocol=https&clientype=2&disturblevel=&apptype=2&curenv=inner&sess=" + sess + "&noBorder=noborder&showtype=embed&uid=" + this.qq + "&cap_cd=" + this.cap_cd + "&lang=2052&rnd=514856",
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
            Regex regex = new Regex("v=\"(.*?)\"");
            Match match = regex.Match(result.Html);
            return match.Groups[1].Value;
        }

        /// <summary>
        /// 1.获取Vsig
        /// </summary>
        /// <returns></returns>
        private string GetVsig(string sess)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = "https://ssl.captcha.qq.com/cap_union_new_getsig?aid=715030901&apptype=2&uid=" + this.qq + "&cap_cd=" + this.cap_cd + "&sess=" + sess,
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
            VisgModel visgModel = JsonConvert.DeserializeObject<VisgModel>(result.Html);
            return visgModel == null ? "" : visgModel.vsig;
        }

        /// <summary>
        /// 获取验证码图片 返回字节
        /// </summary>
        /// <param name="vsig"></param>
        /// <returns></returns>
        private byte[] get_verify_image(string vsig, string sess)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                //                aid:715030901
                //asig:
                //captype:
                //protocol:https
                //clientype:2
                //disturblevel:
                //apptype:2
                //curenv:inner
                //noBorder:noborder
                //showtype:embed
                //uid:406400859
                //cap_cd:e-U_GqgokFChJQrF9rDxC5U-auwL0MfOsIuktS6ecEkPV2lHKsARqQ**
                //lang:2052
                //rnd:737480
                //rand:0.5590966601520853
                //vsig:gwuIYeNI57lzzoBifP0YihOjXTj7Jk8FPFe1FU_2dO4DdxN-8lt7JP8c099jZQcj3HGkcMkRuXO0IDHlBDkG5b5yQtgyVblA7joFcZ7v1A-lynM0LNcGCIg**
                //ischartype:1
                URL = "https://ssl.captcha.qq.com/cap_union_new_getcapbysig?ischartype=1&protocol=https&curenv=inner&noBorder:noborder&showtype=embed&lang=2052&rnd=737480&aid=715030901&clientype=2&apptype=2&uid=" + this.qq + "&cap_cd=" + this.cap_cd + "&vsig=" + vsig + "&sess=" + sess,
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
                ResultType = ResultType.Byte,
                Encoding = Encoding.UTF8
            };
            HttpResult result = http.GetHtml(item);
            return result.ResultByte;
        }

        /// <summary>
        /// 验证验证码是否正确
        /// </summary>
        /// <param name="vsig"></param>
        /// <param name="vcode"></param>
        /// <returns></returns>
        private bool VerifyVcode(string vsig, string vcode, string sess)
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = "https://ssl.captcha.qq.com/cap_union_new_verify?random=1482473943560",
                Method = "post",//URL     可选项 默认为Get   
                IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写   
                Cookie = "",//字符串Cookie     可选项   
                Referer = "",//来源URL     可选项   
                Postdata = "sess=" + sess + "&ans=" + vcode + "&vsig=" + vsig + "&cdata=0&subcapclass=0&lang=2052&rnd=512924&aid=715030901&protocol=https&clientype=2&apptype=2&curenv=inner&noBorder=noborder&showtype=embed&uid=" + this.qq + "&cap_cd=" + this.cap_cd,//Post数据     可选项GET时不需要写   
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
            string html = result.Html;
            VCodeModel vcodeModel = JsonConvert.DeserializeObject<VCodeModel>(html);
            if (vcodeModel.errorCode.Equals("0"))
            {
                this.cap_cd = vcodeModel.randstr;
                this.ptvfsession = vcodeModel.ticket;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
