using Funq;
using ServiceStack;
using BAPT.ServiceInterface;
using ServiceStack.Text;
using System;
namespace BAPT
{
    public class AppHost : AppHostBase
    {
        /// <summary>
        /// Default constructor.
        /// Base constructor requires a name and assembly to locate web service classes. 
        /// </summary>
        public AppHost()
            : base("BAPT", typeof(BAPTServices).Assembly)
        {

        }

        /// <summary>
        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        /// </summary>
        /// <param name="container"></param>
        public override void Configure(Container container)
        {
            //Config examples
            //this.Plugins.Add(new PostmanFeature());
            //this.Plugins.Add(new CorsFeature());
            //SetConfig(new HostConfig
            //{
            //    WebHostUrl = "http://localhost/BAPT/api/",
            //});
          
            JsConfig<DateTime>.SerializeFn = time => new DateTime(time.Ticks, DateTimeKind.Local).ToString("o");
            JsConfig<DateTime?>.SerializeFn =
                time => time != null ? new DateTime(time.Value.Ticks, DateTimeKind.Local).ToString("o") : null;
            JsConfig.DateHandler = DateHandler.ISO8601;
            JsConfig.IncludeNullValues = true;
        }
    }
}