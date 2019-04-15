using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatWeb.Tool
{
    public static class Utils
    {
        /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式 秒
        /// </summary>  
        /// <param name="time">时间</param>
        /// <returns>long</returns>  
        public static int ConvertDateTimeToInt(this DateTime time)
        {
            var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            var t = (time.Ticks - startTime.Ticks) / 10000000;   //除10000调整为10位      
            return (int)t;
        }
    }
}
