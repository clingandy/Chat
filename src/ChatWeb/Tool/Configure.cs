
using System.Threading;

namespace ChatWeb.Tool
{
    public class Configure
    {
        public Configure()
        {
            Smp = new SemaphoreSlim(AppSettingsHelper.GetInt32("MaxDegreeOfParallelism", 100));
        }

        public static SemaphoreSlim Smp = new SemaphoreSlim(100);
    }
}
