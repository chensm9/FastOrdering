using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;

namespace FastOrdering.Services
{
    public class MessageHelper
    {
        private string url = @"http://utf8.api.smschinese.cn/";
        private string userid = "";
        private string key = "";//秘钥
        private string smsMob;
        private string smsText;

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="isMD5">是否需要MD5加密</param>
        /// <param name="userid">申请的用户名</param>
        /// <param name="key">申请的秘钥</param>
        /// <param name="mob">发送的手机（多个需要以','连接）</param>
        /// <param name="text">信息内容（注意签名格式【XX网】）</param>
        public MessageHelper(bool isMD5, string userid, string key, string mob, string text)
        {
            this.userid = "/?Uid=" + userid;
            this.key = isMD5 ? "&KeyMD5=" + GetMD5Str(key) : "&Key=" + key;
            this.smsMob = "&smsMob=" + mob;
            this.smsText = "&smsText=" + text;
        }

        //MD5加密
        private string GetMD5Str(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            StringBuilder sbStr = new StringBuilder();
            byte[] buffer = Encoding.Default.GetBytes(str);
            byte[] dataBuff = md5.ComputeHash(buffer);//计算指定字符数组的哈希值

            foreach (byte d in dataBuff)
                sbStr.Append(d.ToString("x2"));

            return (sbStr + "").ToUpper();
        }


        private string GetResponse()
        {
            var targetUrl = url + userid + key + smsMob + smsText;
            var res = "";
            try
            {
                HttpWebRequest hr = (HttpWebRequest)WebRequest.Create(targetUrl);
                hr.Method = "GET";
                hr.Timeout = 30 * 60 * 1000;//30分钟超时
                WebResponse hs = hr.GetResponse();
                Stream sr = hs.GetResponseStream();
                StreamReader ser = new StreamReader(sr, Encoding.Default);
                res = ser.ReadToEnd();
            }
            catch (Exception ex)
            {
                res = ex.Message;
                throw ex;
            }
            return res;
        }

        private enum Result
        {
            没有该用户账户 = -1,
            接口密钥不正确 = -2,
            MD5接口密钥加密不正确 = -21,
            短信数量不足 = -3,
            该用户被禁用 = -11,
            短信内容出现非法字符 = -14,
            手机号格式不正确 = -4,
            手机号码为空 = -41,
            短信内容为空 = -42,
            短信签名格式不正确 = -51,
            IP限制 = -6
        }

        public string GetSendStr()
        {
            var res = "";
            var response = GetResponse();
            if (response != "")
            {
                int temp;
                if (Int32.TryParse(response, out temp))
                {
                    if (temp > 0)
                        res = string.Format("{0}条信息发送成功", temp);
                    else
                        res = Enum.GetName(typeof(Result), temp);
                }
                else res = "发送失败：" + response;
            }
            else res = "发送失败";
            return res;
        }

    }

}
