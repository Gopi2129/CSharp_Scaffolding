using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using AventStack.ExtentReports.Gherkin.Model;
using System.IO.Compression;
using Automation.Web.Tests.Support;

namespace Automation.Utils
{
    [Binding]
    public class ReporterClass
    {
        private static ExtentHtmlReporter? htmlReporter;
        public static ExtentReports? extentReports;
        static string? reportCompleteFilePath;
        static string? screenshotPath;
        static string? reporterNameTimeStamp;
        private static ExtentTest featureName;
        private static ExtentTest scenarioName;
        static ExtentTest stepName;
        public static ThreadLocal<ExtentTest> scenarioThreadLocal = new ThreadLocal<ExtentTest>();
        private const string scenarioUserNameKey = "Username";
        private static IServiceProvider serviceProvider;
        private static bool isAutoOpenReport ;
        private static bool iskillDriverInstance;

        [BeforeTestRun]
        public static void CreateExtentHtmlReporter()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();

            reporterNameTimeStamp = DateTime.Now.ToString("dd_MMM_yyy_HH_mm_ss");
            reportCompleteFilePath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent + @"\Reports\TestReport\Report_" + reporterNameTimeStamp + @"\";
            Console.WriteLine("Path of the Report File - >" + reportCompleteFilePath);
            screenshotPath = Directory.CreateDirectory(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent + @"\Reports\TestReport\Report_" + reporterNameTimeStamp + @"\Screenshots\").FullName;
            htmlReporter = new ExtentHtmlReporter(reportCompleteFilePath);
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
            var config = serviceProvider.GetService<IAppConfiguration>();
            string reportName = config.ApplicationName + "_" + config.TestingLevel;
            isAutoOpenReport=config.AutoOpenReport.Equals("Yes", StringComparison.OrdinalIgnoreCase) ? true : false;
            iskillDriverInstance = config.killDriverInstance.Equals("NO", StringComparison.OrdinalIgnoreCase) ? true : false;

            htmlReporter.Config.ReportName = reportName;
            extentReports = new ExtentReports();
            extentReports.AttachReporter(htmlReporter);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAppConfiguration, AppConfiguration>();
        }

        [BeforeFeature]
        public static void CreateFeature(FeatureContext featureContext)
        {
            featureName = extentReports.CreateTest<Feature>(featureContext.FeatureInfo.Title, featureContext.FeatureInfo.Description);
        }

        [BeforeScenario]
        public static void CreateScenario(ScenarioContext scenarioContext)
        {
            string scenarioTitle = scenarioContext.ScenarioInfo.Title;
            string scenarioTag = scenarioContext.ScenarioInfo.Tags.ToList().FirstOrDefault(s => s.Contains("Scenario", StringComparison.OrdinalIgnoreCase));
            scenarioTitle = scenarioTag + "_" + scenarioTitle;
            if (scenarioContext.ScenarioInfo.Arguments.Contains(scenarioUserNameKey))
            {
                scenarioTitle += "_" + scenarioContext.ScenarioInfo.Tags.ToList().FirstOrDefault(s => s.Contains("Scenario", StringComparison.OrdinalIgnoreCase));
            }
            scenarioName = featureName.CreateNode<Scenario>(scenarioTitle, scenarioContext.ScenarioInfo.Tags[0].Trim());

        }

        [BeforeStep]
        public static void createStep(ScenarioContext scenarioContext)
        {
            var stepType = scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
            switch (stepType)
            {
                case "Given":
                    stepName = scenarioName.CreateNode<Given>(scenarioContext.StepContext.StepInfo.Text);
                    scenarioThreadLocal.Value = stepName;
                    break;
                case "When":
                    stepName = scenarioName.CreateNode<When>(scenarioContext.StepContext.StepInfo.Text);
                    scenarioThreadLocal.Value = stepName;
                    break;
                case "Then":
                    stepName = scenarioName.CreateNode<Then>(scenarioContext.StepContext.StepInfo.Text);
                    scenarioThreadLocal.Value = stepName;
                    break;
                case "And":
                    stepName = scenarioName.CreateNode<And>(scenarioContext.StepContext.StepInfo.Text);
                    scenarioThreadLocal.Value = stepName;
                    break;
            }
        }




        [AfterStep]
        public static void CreateStepWithScenario(ScenarioContext scenarioContext)
        {
            var stepType = scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
            if (scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.OK)
            {
                switch (stepType)
                {
                    case "Given":
                        stepName.Pass("Step Passed Successfully");
                        break;
                    case "When":
                        stepName.Pass("Step Passed Successfully");
                        break;
                    case "Then":
                        stepName.Pass("Step Passed Successfully");
                        break;
                    case "And":
                        stepName.Pass("Step Passed Successfully");
                        break;
                }
            }
            else if (scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError)
            {
                string filePathToSaveScreenshots = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent + @"\Reports\Screenshots\ScreenshotImage" + DateTime.Now.ToString("ddMMHHmmss") + ".png";
                switch (stepType)
                {
                    case "Given":
                        stepName.Pass("Step Passed Successfully");
                        break;
                    case "When":
                        stepName.Pass("Step Passed Successfully");
                        break;
                    case "Then":
                        stepName.Pass("Step Passed Successfully");
                        break;
                    case "And":
                        stepName.Pass("Step Passed Successfully");
                        break;
                }

            }
        }

        public static void AddStepLog(string passedDescription)
        {
            stepName.Pass(passedDescription);
        }

        public static void AddStepLog(string passedDescription, bool needUIScreenScreenshot = false, bool needElementScreenshot = false, By element = null)
        {
            string filePathToSaveScreenshots;
            if (needUIScreenScreenshot == true)
            {
                filePathToSaveScreenshots = screenshotPath + "ScreenshotImage " + DateTime.Now.ToString("ddMMHHmmss") + ".png";
                stepName.Pass(passedDescription).AddScreenCaptureFromPath(CommonActionClass.TakeScreenshotImage(filePathToSaveScreenshots));
            }
            else if (needElementScreenshot == true)
            {
                if (element == null)
                {
                    throw new NullReferenceException("Element is null to Take Element Screenshot");
                }
                else
                {
                    filePathToSaveScreenshots = screenshotPath + "ScreenshotImage " + DateTime.Now.ToString("ddMMHHmmss") + ".png";
                    stepName.Pass(passedDescription).AddScreenCaptureFromPath(CommonActionClass.TakeElementScreenshot(element, filePathToSaveScreenshots));
                }
            }
            else
            {
                stepName.Pass(passedDescription);
            }
        }

        public static void AddPassedStepMessage(string passedDescription, bool needUIScreenScreenshot = false, bool needElementScreenshot = false, By element = null)
        {
            string filePathToSaveScreenshots;
            if (needUIScreenScreenshot == true)
            {
                filePathToSaveScreenshots = screenshotPath + "ScreenshotImage " + DateTime.Now.ToString("ddMMHHmmss") + ".png";
                stepName.Pass($"<font color=green size=2><strong> {passedDescription}  </strong></font>").AddScreenCaptureFromPath(CommonActionClass.TakeScreenshotImage(filePathToSaveScreenshots));
            }
            else if (needElementScreenshot == true)
            {
                if (element == null)
                {
                    throw new NullReferenceException("Element is null to Take Element Screenshot");
                }
                else
                {
                    filePathToSaveScreenshots = screenshotPath + "ScreenshotImage " + DateTime.Now.ToString("ddMMHHmmss") + ".png";
                    stepName.Pass($"<font color=green size=2><strong> {passedDescription}  </strong></font>").AddScreenCaptureFromPath(CommonActionClass.TakeElementScreenshot(element, filePathToSaveScreenshots));
                }
            }
            else
            {
                stepName.Pass($"<font color=green size=2><strong> {passedDescription} </strong></font>");
            }
        }

        public static void AddFailedStepLog(string failedDescription)
        {
            string filePathToSaveScreenshots;
            if (CommonActionClass.driver != null)
            {
                filePathToSaveScreenshots = screenshotPath + "ScreenshotImage " + DateTime.Now.ToString("ddMMHHmmss") + ".png";
                stepName.Fail($"<font color=red size =2>{failedDescription}</font>").AddScreenCaptureFromPath(CommonActionClass.TakeScreenshotImage(filePathToSaveScreenshots));
                Assert.Fail();
            }
            else
            {
                stepName.Fail($"<font color=maroon size =2><strong>{failedDescription}</strong></font>");
                Assert.Fail();
            }
        }

        public static void AddFailedStepLog(string failedDescription, bool isApiStep)
        {
            if (isApiStep == true)
            {
                stepName.Fail(failedDescription);
                Assert.Fail();
            }
            else
            {
                string filePathToSaveScreenshots = screenshotPath + "ScreenshotImage " + DateTime.Now.ToString("ddMMHHmmss") + ".png";
                stepName.Fail(failedDescription).AddScreenCaptureFromPath(CommonActionClass.TakeScreenshotImage(filePathToSaveScreenshots));
                Assert.Fail();
            }
        }

        public static void AddWarningStepMessage(string warningDescription, bool needUIScreenScreenshot)
        {
            string filePathToSaveScreenshots;
            if (needUIScreenScreenshot == true)
            {
                filePathToSaveScreenshots = screenshotPath + "ScreenshotImage " + DateTime.Now.ToString("ddMMHHmmss") + ".png";
                stepName.Pass($"<font color=yellow size=2><strong> {warningDescription}  </strong></font>").AddScreenCaptureFromPath(CommonActionClass.TakeScreenshotImage(filePathToSaveScreenshots));
            }
            else
            {
                stepName.Pass($"<font color=yellow size=2><strong> {warningDescription} </strong></font>");
            }
        }
        [AfterTestRun]
        public static void AfterTestReporterFlush()
        {
            try
            {
                if (iskillDriverInstance)
                {
                    CommonActionClass.QuitDriverInstance();
                }
                extentReports.Flush();
                string newReportPath = reportCompleteFilePath + "Automation_Report_" + reporterNameTimeStamp + ".html";
                if (File.Exists(reportCompleteFilePath + "index.html"))
                {
                    File.Move(reportCompleteFilePath + "index.html", newReportPath);
                    Console.WriteLine($"Report File generated after execution. Path - [{reportCompleteFilePath}]");
                }
                else
                {
                    throw new FileNotFoundException("Report File is not available.");
                }
                if (isAutoOpenReport)
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.FileName = newReportPath;
                    process.Start();
                }
                reportCompleteFilePath = reportCompleteFilePath.Substring(0, reportCompleteFilePath.Length - 1);
                string zipFilePath = reportCompleteFilePath + ".zip";
                ZipFile.CreateFromDirectory(reportCompleteFilePath, zipFilePath);
                Console.WriteLine($"Zipped Report File generated after execution. Path - [{zipFilePath}]");
                /*if(SendReportAfterExecution.Equals("Yes",StringComparison.OrdinalIgnoreCase))
                {
                    mail.SendEmailWithAttachment(zipFilePath);
                }*/
                System.Diagnostics.Process[] allChromeProccess = System.Diagnostics.Process.GetProcessesByName("chromedriver");
                string s = allChromeProccess[0].ProcessName;
                foreach (System.Diagnostics.Process chromeprocess in allChromeProccess)
                {
                    chromeprocess.Kill();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }


    }

}
        
