using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using IQVIAbadAPIMVCApp.Models;

namespace IQVIAbadAPIMVCApp.Controllers
{
    public class TweetController : Controller
    {
        const int MAX_RECORDS_RETURNED = 100;
        string apiUrl = "https://badapi.iqvia.io/api/v1/Tweets";
        DateTime startDate = new DateTime(2016, 1, 1, 0, 0, 0, 000, DateTimeKind.Utc);
        DateTime endDate = new DateTime(2017, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
        //DateTime endDate = new DateTime(2016, 1, 7, 23, 59, 59, 999, DateTimeKind.Utc);

        // GET: Tweet
        public ActionResult Index()
        {
            return View();
        }

        // GET: Tweet
        public ActionResult Tweets()
        {
            var webClient = new System.Net.WebClient();
            webClient.QueryString.Add("startDate", startDate.ToString());
            webClient.QueryString.Add("endDate", endDate.ToString());
            string results = webClient.DownloadString(apiUrl);

            List<Tweet> tweets = new List<Tweet>();

            var jsonParse = new JavaScriptSerializer().Deserialize<List<Tweet>>(results);

            tweets = jsonParse;

            while (jsonParse.Count() == MAX_RECORDS_RETURNED)
            {
                //This new date is identical to the last value in the previously parsed list.
                //This is incase there are more tweets with the same datetime stamp.
                var newStartDate = DateTime.Parse(jsonParse[jsonParse.Count() - 1].stamp);

                webClient = new System.Net.WebClient();
                webClient.QueryString.Add("startDate", newStartDate.ToString());
                webClient.QueryString.Add("endDate", endDate.ToString());
                results = webClient.DownloadString(apiUrl);

                jsonParse = new JavaScriptSerializer().Deserialize<List<Tweet>>(results);

                tweets.AddRange(jsonParse);
            }

            List<Tweet> updatedListOfTweets = RemoveDuplicates(tweets);

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(updatedListOfTweets);
        }

        //This function will take a List of Tweet objects and uses extensions methods
        //to group and return the first item in each group.  This will produce a list of unique records
        private List<Tweet> RemoveDuplicates(List<Tweet> originalRecords)
        {
            List<Tweet> newListOfTweets = originalRecords.GroupBy(x => x.id).Select(x => x.First()).ToList();

            return newListOfTweets;
        }

        // GET: Tweets using async/await with HttpClient object
        public ActionResult TweetsAsync()
        {
            try
            {
                UriBuilder uriString = BuildQueryString(startDate.ToString(), endDate.ToString(), apiUrl);

                var t = Task.Run(() => GetURI(new Uri(uriString.ToString()), endDate, apiUrl));
                t.Wait();

                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;

                List<Tweet> updatedTweets = RemoveDuplicates(t.Result);

                return View(updatedTweets);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                return View("Error");
            }
        }

        private async Task<List<Tweet>> GetURI(Uri u, DateTime end, string url)
        {
            try
            {
                var response = string.Empty;
                List<Tweet> tweets = new List<Tweet>();

                using (var client = new HttpClient())
                {
                    HttpResponseMessage result = await client.GetAsync(u);
                    if (result.IsSuccessStatusCode)
                    {
                        response = await result.Content.ReadAsStringAsync();

                        var jsonParse = new JavaScriptSerializer().Deserialize<List<Tweet>>(response);
                        tweets = jsonParse;

                        while (jsonParse.Count == MAX_RECORDS_RETURNED)
                        {
                            //change querystring with updated daterange
                            //This new date is identical to the last value in the previously parsed list.
                            //This is incase there are more tweets with the same datetime stamp.
                            var newStartDate = DateTime.Parse(jsonParse[jsonParse.Count() - 1].stamp);

                            UriBuilder newUri = BuildQueryString(newStartDate.ToString(), end.ToString(), url);

                            result = await client.GetAsync(newUri.ToString());
                            response = await result.Content.ReadAsStringAsync();

                            jsonParse = new JavaScriptSerializer().Deserialize<List<Tweet>>(response);

                            tweets.AddRange(jsonParse);
                        }
                    }
                    else
                    {
                        //some error resulted here.  The API call doesn't exaclty return a code.
                        throw new Exception(result.ReasonPhrase);
                    }
                }

                return tweets;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Build the query string for the HttpClient object.
        //This allows for the querystring parameters to be added.
        private UriBuilder BuildQueryString(string start, string end, string url)
        {
            UriBuilder uriBuilder = new UriBuilder(url);

            //build query parameters
            var queryParams = HttpUtility.ParseQueryString(uriBuilder.Query);
            queryParams["startDate"] = start;
            queryParams["endDate"] = end;

            uriBuilder.Query = queryParams.ToString();

            return uriBuilder;
        }
    }
}