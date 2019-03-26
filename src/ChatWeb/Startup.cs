using System.Threading.Tasks;
using ChatWeb.Redis;
using ChatWeb.Tool;
using ChatWeb.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RedisAccessor;

namespace ChatWeb
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton(redisHelper =>
            {
                var redisAddr = AppSettingsHelper.GetString("Redis:RedisAddr");
                var redisDb = AppSettingsHelper.GetInt32("Redis:RedisDb");
                var prefixKey = AppSettingsHelper.GetString("Redis:PrefixKey") ?? "prefix_";
                return DependencyExtensions.UseRedis(redisAddr, redisDb, prefixKey);
            });

            services.AddSingleton<ChatService>();
            services.AddSingleton<IChannelManage, ChannelManage>();
            services.AddSingleton<RedisMessageManage>();
            services.AddHostedService<AppBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //允许全部跨域
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
