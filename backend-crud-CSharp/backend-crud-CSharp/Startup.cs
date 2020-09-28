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

using RebelSoftware.LoggingService;
using RebelSoftware.MessageQueueService;
using RebelSoftware.SerializationService;
using RebelSoftware.HttpService;

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
            var logger = Logging.createLogger();
            var messageQueuer = MessageQueueing.createMessageQueuer();
            var serializationService = Serialization.createSerializationService<Model.Employee>();
            var employeeValidator = Validation.getEmployeeValidator();


            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

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
                    var controller = new EmployeeController(logger, messageQueuer, serializationService, httpService, employeeValidator);

                    controller.Create();
                    //await context.Response.WriteAsync("hello");
                });
            });
        }
    }
}
