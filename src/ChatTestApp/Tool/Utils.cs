using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ChatTestApp.Tool
{
    public static class Utils
    {
        #region GET、POST方法

        /// <summary>
        /// get方法
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetData(string url)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);

            myRequest.Method = "GET";                      //确定GET模式
            myRequest.ContentType = "application/json";                     //确定获取的数据格式
            myRequest.Headers.Add("Authorization", "fwadmin=98CA2FFD2ACB56B612B3641341D74A5A");   //信息头参数添加

            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();

            StreamReader reader = new StreamReader(myResponse.GetResponseStream() ?? throw new InvalidOperationException(), Encoding.UTF8);
            string content = reader.ReadToEnd();
            reader.Close();
            myResponse.Close();
            return content;

        }

        /// <summary>
        /// POST数据 带参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string PostData(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "POST";
            myRequest.ContentType = "dataType/application/x-www-form-urlencoded";
            myRequest.Headers.Add("Authorization", "fwadmin=98CA2FFD2ACB56B612B3641341D74A5A");   //确定信息头参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0) builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            myRequest.ContentLength = data.Length;
            using (Stream reqStream = myRequest.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            HttpWebResponse resp = (HttpWebResponse)myRequest.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream ?? throw new InvalidOperationException(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        /// <summary>
        /// POST数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string PostData(string url, string postData)
        {
            byte[] data = Encoding.UTF8.GetBytes(postData);     //postData可以为空
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);

            myRequest.Method = "POST";                     //确定post模式
            myRequest.ContentType = "dataType/json";
            myRequest.Headers.Add("Authorization", "fwadmin=98CA2FFD2ACB56B612B3641341D74A5A");   //确定信息头参数
            myRequest.ContentLength = data.Length;
            Stream newStream = myRequest.GetRequestStream();

            newStream.Write(data, 0, data.Length);
            newStream.Close();

            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();

            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();
            reader.Close();
            myResponse.Close();
            return content;

        }

        #endregion

    }
}
