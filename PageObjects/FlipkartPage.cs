using Automation.Utils;
using Automation.Web.Tests.Support;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Web.Tests.PageObjects
{
    public class FlipkartPage:CommonActionClass
    {

        private By link_Banner = By.XPath("(//div[@class='_37M3Pb']/div/a)[2]");
        private By button_cancel = By.XPath("//button[text()='✕']");


        readonly string url;
        public FlipkartPage(AppConfiguration config)
        {
            this.url = config.LoginUrl;
        }

        //Healthy Benefits Methods

        //Method to Launch application
        public void LaunchFlipkartApplication(bool isLocationEnabled = false)
        {
            LaunchApplication(url, isLocationEnabled);
            ReporterClass.AddStepLog("Launching Application Url - > " + url);
        }

        /// <summary>
        /// Method to click on any banner
        /// </summary>
        public void ClickBannerLink()
        {
            if (WaitForElement(link_Banner, 70) != null)
            {
                ClickElement(link_Banner);
            }
            else
            {
                ReporterClass.AddFailedStepLog($"Wait for Element {link_Banner} Failed.");
            }
        }
        public void ClickCancelOnPopUp()
        {
            if (WaitForElement(button_cancel, 70) != null)
            {
                ClickElement(button_cancel);
            }
            else
            {
                ReporterClass.AddFailedStepLog($"Wait for Element {button_cancel} Failed.");
            }
        }

    }
}
