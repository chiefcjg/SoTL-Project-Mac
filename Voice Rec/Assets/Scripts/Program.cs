using System;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;
//using Newtonsoft.Json.Linq;

namespace csharp_predict_with_rest
{
     class Program
    {
        static void Main(string[] args)
        {
            //////////
            // Values to modify.

            // YOUR-APP-ID: The App ID GUID found on the www.luis.ai Application Settings page.
            var appId = "0d4309a9-a5ff-43c1-b89a-831714645afe";

            // YOUR-PREDICTION-KEY: 32 character key.
            var predictionKey = "f028c5f86a0e47f8ac31de166d0b4de9";

            // YOUR-PREDICTION-ENDPOINT: Example is "https://westus.api.cognitive.microsoft.com/"
            var predictionEndpoint = "https://westus.api.cognitive.microsoft.com/";

            // An utterance to test the LUIS quick-start app 
			//(https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-get-started-create-app).
			var utterance = "";
            if (args.Length == 0)
			{
				utterance = "I stopped you for going 60 kilometres per hour in a 30 zone";
			}
			else
			{
				utterance = args[0];
			}
            //////////

			Task<string> strPrediction = MakeRequest(predictionKey, predictionEndpoint, appId, utterance);
			//var x = JObject.Parse(strPrediction.Result);
			//var score = x["prediction"]["intents"];
			
			Console.WriteLine(strPrediction.Result);
			
            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }

        static async Task<string> MakeRequest(string predictionKey, string predictionEndpoint, string appId, string utterance)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", predictionKey);

            // The "q" parameter contains the utterance to send to LUIS
            queryString["query"] = utterance;

            // These optional request parameters are set to their default values
            queryString["verbose"] = "true";
            queryString["show-all-intents"] = "false";
            queryString["staging"] = "false";
            queryString["timezoneOffset"] = "0";

            var predictionEndpointUri = String.Format("{0}luis/prediction/v3.0/apps/{1}/slots/production/predict?{2}", predictionEndpoint, appId, queryString);

            // Remove these before updating the article.
            Console.WriteLine("endpoint: " + predictionEndpoint);
            Console.WriteLine("appId: " + appId);
            Console.WriteLine("queryString: " + queryString);
            Console.WriteLine("endpointUri: " + predictionEndpointUri);

            var response = await client.GetAsync(predictionEndpointUri);

            var strResponseContent = await response.Content.ReadAsStringAsync();

            // Display the JSON result from LUIS.
            // Console.WriteLine(strResponseContent.ToString());
			// return the JSON
			return strResponseContent.ToString();
        }
    }
}