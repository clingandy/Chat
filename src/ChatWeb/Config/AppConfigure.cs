using System.Threading;
using ChatWeb.Tool;

namespace ChatWeb.Config
{
    public class AppConfigure
    {
        public AppConfigure()
        {
            Smp = new SemaphoreSlim(AppSettingsHelper.GetInt32("MaxDegreeOfParallelism", 100));
        }

        public static SemaphoreSlim Smp = new SemaphoreSlim(100);
    }
}
