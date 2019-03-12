using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ChatWeb.Tool
{
    public static class JsonHelper
    {
        public static string JsonSerialize(this object obj)
        {
            string input = JsonConvert.SerializeObject(obj);
            return Regex.Replace(input, "\\\\/Date\\((-?(\\d+))\\)\\\\/", delegate (Match match)
            {
                DateTime dateTime = new DateTime(1970, 1, 1);
                dateTime = dateTime.AddMilliseconds((double)long.Parse(match.Groups[1].Value));
                dateTime = dateTime.ToLocalTime();
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            });
        }

        public static T JsonDeserialize<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
