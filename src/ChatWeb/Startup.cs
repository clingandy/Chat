using ChatWeb.Config;
using ChatWeb.Redis;
using ChatWeb.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedisAccessor;

namespace ChatWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddOptions();

            // 加载配置
            services.Configure<MessageConfigure>(Configuration.GetSection("Message"));

            // 练手项目，建议修改
            services.AddSingleton(redisHelper =>
            {
                var redisAddr = Configuration.GetValue<string>("Redis:RedisAddr");
                var redisDb = Configuration.GetValue<int>("Redis:RedisDb");
                var prefixKey = Configuration.GetValue<string>("Redis:PrefixKey") ?? "prefix_";
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

            // 加载配置
            var cnfigBuilder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)//增加环境配置文件，新建项目默认有
                .AddEnvironmentVariables();
            Configuration = cnfigBuilder.Build();

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
