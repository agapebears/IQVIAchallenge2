using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IQVIAchallengeWebApp1;
using IQVIAchallengeWebApp1.Controllers;
using System.Web.Mvc;

namespace IQVIAchallengeWebApp1.Tests.Controllers
{
    /// <summary>
    /// Summary description for TweetControllerTest
    /// </summary>
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
