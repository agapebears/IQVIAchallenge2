using System;
using System.Web.Mvc;
using IQVIAbadAPIMVCApp;
using IQVIAbadAPIMVCApp.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IQVIAbadAPIMVCApp.Tests.Controllers
{
    [TestClass]
    public class TweetControllerTest
    {
        [TestMethod]
        public void TweetsResultsNotNull()
        {
            // Arrange
            TweetController controller = new TweetController();

            // Act
            ViewResult result = controller.Tweets() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TweetsAsyncEndDateGreaterThanEqualStartDate()
        {
            // Arrange
            TweetController controller = new TweetController();

            // Act
            ViewResult result = controller.TweetsAsync() as ViewResult;

            // Assert
            //Assert.AreEqual("TweetsAsync", result.ViewBag.Title);
            Assert.IsTrue(result.ViewBag.endDate >= result.ViewBag.startDate);

        }        
    }
}
