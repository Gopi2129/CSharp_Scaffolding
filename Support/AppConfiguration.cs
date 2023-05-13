using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Utils
{
    public class AppConfiguration : IAppConfiguration
    {
        public string TestingEnvironment { get; set; }

        public string LoginUrl { get; set; }

        public string ApplicationName { get; set; }

        public string BrowserName { get; set; }

        public string IncognitoMode { get; set; }

        public string HeadlessBrowser { get; set; }

        public string killDriverInstance { get; set; }

        public string AutoOpenReport { get; set; }
        public string? TestingLevel { get; set; }



        public AppConfiguration() { 
              var Configuration = new ConfigurationBuilder()
                .AddJsonFile(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent + @"\\Scaffolding\\appconfig.json", optional:true, reloadOnChange:true)
                .AddEnvironmentVariables("ASPNETCORE_")
                .Build();

            TestingEnvironment = Configuration["TestingEnvironment"];
            IncognitoMode = Configuration["ApplicationSettings:IncognitoMode"];
            HeadlessBrowser = Configuration["ApplicationSettings:HeadlessBrowser"];
            BrowserName = Configuration["ApplicationSettings:BrowserName"];
            ApplicationName = Configuration["ApplicationSettings:ApplicationName"];
            LoginUrl = (!string.IsNullOrEmpty(TestingEnvironment) && TestingEnvironment.ToLower().Equals("QA", StringComparison.OrdinalIgnoreCase)) ? Configuration["ApplicationUrl:QAUrl"] : Configuration["ApplicationUrl:StageUrl"];
            killDriverInstance = Configuration["ApplicationSettings:killDriverInstance"];
            AutoOpenReport= Configuration["ApplicationSettings:AutoOpenReport"];
            TestingLevel = Configuration["ApplicationSettings:TestingLevel"];

        }

    }

    public interface IAppConfiguration
    {
        public string TestingEnvironment { get; set; }

        public string LoginUrl { get; set; }

        public string ApplicationName { get; set; }

        public string BrowserName { get; set; }
        public string IncognitoMode { get; set; }

        public string HeadlessBrowser { get; set; }
        public string killDriverInstance { get; set; }

        public string AutoOpenReport { get; set; }
        public string? TestingLevel { get; set; }

    }

}
