using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RebelSoftware.HttpService;
using RebelSoftware.Logging;
using RebelSoftware.Serialization;
using RebelSoftware.MessageQueue;
using RebelSoftware.EmployeeLogic;

namespace backend_crud_CSharp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //var logger = RebelSoftware.LoggingService.Logging.createLogger();
            var logger = LoggingServiceFactory.CreateLoggingService();
            var messageQueuer = MessageQueueServiceFactory.CreateMessageQueueService();
            var serializationService = SerializationServiceFactory.CreateJsonSerializationService();
            var employeeLogicService = EmployeeLogicServiceFactory.CreateEmployeeLogicService();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //if you uncomment these lines, you will get an error message containing  "No 'Access-Control-Allow-Origin' header is present on the requested resource."  on the front end. (VSCode debug console for node app)
            //app.UseHttpsRedirection();
            //app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(builder => builder.AllowAnyMethod()
                              .AllowAnyHeader()
                              .SetIsOriginAllowed(origin => true)
                              .AllowCredentials());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapPost("/employee/create", async context => 
                {
                    var httpService = HttpServiceFactory.CreateHttpService(context);
                    var controller = new EmployeeController(logger, messageQueuer, serializationService, httpService, employeeLogicService);

                    controller.Create();
                });
            });
        }
    }
}
