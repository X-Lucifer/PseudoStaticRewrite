using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PseudoStaticRewrite.Models;

namespace PseudoStaticRewrite
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddControllersWithViews();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
            services.AddRouting(x =>
            {
                //url�Զ�Сд
                x.LowercaseUrls = true;
            });
            services.AddControllersWithViews(x =>
            {
                x.EnableEndpointRouting = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                //ֻ��ȡController���͵Ŀ�����
                var assembly = typeof(Startup).Assembly.GetTypes().AsEnumerable()
                    .Where(x => typeof(Controller).IsAssignableFrom(x) &&
                                x.GetCustomAttributes(typeof(AutoPseudoAttribute), true).Any()).ToList();
                foreach (var item in assembly)
                {
                    var xname = item.Name.Replace("controller", "", StringComparison.OrdinalIgnoreCase);
                    foreach (var xitem in item.GetMethods()
                        .Where(x => x.IsPublic && (typeof(IActionResult).IsAssignableFrom(x.ReturnType) || typeof(Task<IActionResult>).IsAssignableFrom(x.ReturnType))))
                    {
                        var xparam = xitem.GetParameters().Where(x =>
                            !x.IsOut && !x.IsIn).ToList();
                        if (!xparam.Any())
                        {
                            //�޲�actionĬ������
                            continue;
                        }

                        int zi = 1;
                        //·�ɲ����ɶൽ�ٵĹ���ƥ��, ���ݿ�ѡ����
                        for (int i = xparam.Count; i > 0; i--)
                        {
                            var zlist = xparam;
                            zlist.RemoveRange(i, xparam.Count - i);
                            StringBuilder sb = new StringBuilder();
                            foreach (var xroute in zlist)
                            {
                                sb.Append("{" + xroute.Name + "}/");
                            }

                            var route = sb.ToString().Remove(sb.Length - 1, 1);
                            var pattern = xname + "/" + xitem.Name + "/" + route;
                            var zname = xname + "_" + xitem.Name + "_" + zi;
                            //Ĭ����չ��, ��֧��������չ��. ����ʱҲ��֧��������չ��
                            endpoints.MapControllerRoute(name: zname, pattern: pattern + ".html",
                                new { controller = xname, action = xitem.Name });
                            //������չ��֧��
                            endpoints.MapControllerRoute(name: zname + "_x", pattern: pattern,
                                new { controller = xname, action = xitem.Name });
                            zi++;
                        }
                    }
                }
                //Ĭ��·�ɹ���
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
