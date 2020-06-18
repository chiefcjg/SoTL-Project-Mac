using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using Valve.Newtonsoft.Json.Linq;
using System.Web;
using Valve.VR;
using UnityEngine.SceneManagement;

public class SpeechToTextNewest : MonoBehaviour
{
    // Hook up the  properties below with a Text and Button object in your UI.
    public String outputText = "test";
    public String outputText2 = "test";

    // a reference to the action
    public SteamVR_Action_Boolean RecordMic;

    // a reference to the hand
    public SteamVR_Input_Sources handType;

    //uses the following to track if it is recording or not and the message.
    private object threadLocker = new object();
    private bool waitingForReco;
    private string message;
    private string answer = "";


    //tracks Topintent and score
    public ArrayList FeedbackAnswers = new ArrayList();

    //tracks good and bad scene
    public string GoodSceneName;
    public string BadSceneName;

    //tracks if recording started or not.
    public bool recordingStarted = false;


    //tracks permission and arguments
    private bool micPermissionGranted = false;
    public string[] args;

    //tracks answer
    private string feedbackAnswer;


    public async void ButtonClick(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        // Creates an instance of a speech config with specified subscription key and service region.
        // Replace with your own subscription key and service region (e.g., "westus").
        var config = SpeechConfig.FromSubscription("879c496fe1ca49b7b44104e656ea5451", "eastus");

        // YOUR-APP-ID: The App ID GUID found on the www.luis.ai Application Settings page.
        var appId = "0d4309a9-a5ff-43c1-b89a-831714645afe";

        // YOUR-PREDICTION-KEY: 32 character key.
        var predictionKey = "f028c5f86a0e47f8ac31de166d0b4de9";

        // YOUR-PREDICTION-ENDPOINT: Example is "https://westus.api.cognitive.microsoft.com/"
        var predictionEndpoint = "https://westus.api.cognitive.microsoft.com/";

        var utterance = "";

        // Make sure to dispose the recognizer after use!
        using (var recognizer = new SpeechRecognizer(config))
        {
            lock (threadLocker)
            {
                waitingForReco = true;
            }

            // Starts speech recognition, and returns after a single utterance is recognized. The end of a
            // single utterance is determined by listening for silence at the end or until a maximum of 15
            // seconds of audio is processed.  The task returns the recognition text as result.
            // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
            // shot recognition like command or query.
            // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
            var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

            // Checks result.
            string newMessage = string.Empty;
            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                newMessage = result.Text;
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                newMessage = "NOMATCH: Speech could not be recognized.";
            }
            //if you stop it it stops. //if there are errors
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                newMessage = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}";
            }

            lock (threadLocker)
            {
                message = newMessage;
                waitingForReco = false;
            }
            utterance = message;
            Task<string> strPrediction = MakeRequest(predictionKey, predictionEndpoint, appId, utterance);

            var predictionResult = JObject.Parse(strPrediction.Result);
            var topIntent = predictionResult["prediction"]["topIntent"];
            var score = predictionResult["prediction"]["intents"][topIntent.ToString()]["score"];

            // provides topintent and score.
            FeedbackAnswers.Add(topIntent.ToString());
            FeedbackAnswers.Add(score);

            Debug.Log(FeedbackAnswers[0]);
            Debug.Log(FeedbackAnswers[1]);
            Debug.Log(strPrediction.Result);
        }
    }

    void Start()
    {
        //it work!
        // Continue with normal initialization, Text and Button objects are present.
        micPermissionGranted = true;
        message = "Click button to recognize speech ";
        RecordMic.AddOnStateDownListener(ButtonClick, handType);
    }


    //updated every frame.
    void Update()
    {
        lock (threadLocker)
        {

            //when no more text is being input it stops.
            if (outputText != null)
            {
                outputText = answer;
                CheckSumScore();
            }
        }
    }

    //used to check the score/check and compare two answers.
    public void CheckSumScore()
    {
        try
        {
            //checks the answer against the message.
            if (FeedbackAnswers[0].ToString() == "p3v.ScenarioStart" && FeedbackAnswers[1] > .1)
            {
                SceneManager.LoadScene(GoodSceneName);
                //VRPlayer needs to be deleted as it cannot transfer over to the next scene.
                Destroy(this.gameObject);

            }
            //incase it fails.
            else
            {
                SceneManager.LoadScene(BadSceneName);
                //VRPlayer needs to be deleted as it cannot transfer over to the next scene.
                Destroy(this.gameObject);
            }
            //catches out of range exception
        }
        //catches the argument for feedback answers not being fullfilled yet.
        catch (ArgumentOutOfRangeException)
        { }

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

        // used to test parameteres if it gets them.
        Debug.Log("endpoint: " + predictionEndpoint);
        Debug.Log("appId: " + appId);
        Debug.Log("queryString: " + queryString);
        Debug.Log("endpointUri: " + predictionEndpointUri);

        var response = await client.GetAsync(predictionEndpointUri);

        var strResponseContent = await response.Content.ReadAsStringAsync();

        // Display the JSON result from LUIS.
        // Console.WriteLine(strResponseContent.ToString());
        // return the JSON
        return strResponseContent.ToString();
    }
}