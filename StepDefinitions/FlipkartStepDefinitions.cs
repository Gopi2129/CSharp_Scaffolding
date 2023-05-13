using Automation.Utils;
using Automation.Web.Tests.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Automation.Web.Tests.StepDefinitions
{
    [Binding]
    public class FlipkartStepDefinitions
    {

        FlipkartPage loginPage;

            public FlipkartStepDefinitions(FlipkartPage loginPage, AppConfiguration appConfiguration)
            {
                this.loginPage = loginPage;
            }

        [Given(@"Launch flipkart application")]
        public void GivenLaunchFlipkartApplication()
        {
            loginPage.LaunchFlipkartApplication();
        }
        [Then(@"Click cancel on popup")]
        public void ThenClickCancelOnPopup()
        {
            loginPage.ClickCancelOnPopUp();
        }

        [Then(@"Click banner")]
        public void ThenClickBanner()
        {
            loginPage.ClickBannerLink();
        }

    }

}
