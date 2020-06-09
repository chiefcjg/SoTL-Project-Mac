using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using System.Linq;
using System;

public class SpeechToText : MonoBehaviour
{
    // Hook up the  properties below with a Text and Button object in your UI.
    public Text outputText;
    public Text outputText2;
    public Button startRecoButton;

    //users the following to track if it is recording or not and the message.
    private object threadLocker = new object();
    private bool waitingForReco;
    private string message;
    private string answer = "";

    public bool recordingStarted = false;

    private bool micPermissionGranted = false;

    public async void ButtonClick()
    {
        // Creates an instance of a speech config with specified subscription key and service region.
        // Replace with your own subscription key and service region (e.g., "westus").
        var config = SpeechConfig.FromSubscription("879c496fe1ca49b7b44104e656ea5451", "eastus");

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
        }
    }

    //
    void Start()
    {
        //if it is missing its output text
        if (outputText == null)
        {
            UnityEngine.Debug.LogError("outputText property is null! Assign a UI Text element to it. ");
        }

        // if it is missing the UI button
        else if (startRecoButton == null)
        {
            message = "startRecoButton property is null! Assign a UI Button to it. ";
            UnityEngine.Debug.LogError(message);
        }
        else
        {
            //it work!
            // Continue with normal initialization, Text and Button objects are present.
            micPermissionGranted = true;
            message = "Click button to recognize speech ";
            startRecoButton.onClick.AddListener(ButtonClick);
        }
    }


    //updated every frame.
    void Update()
    {
        lock (threadLocker)
        {
            //if recording does not equal null it records, gets permission, and sets recording started to true
            if (startRecoButton != null)
            {
                startRecoButton.interactable = !waitingForReco && micPermissionGranted;
                recordingStarted = true;
            }
            //when no more text is being input it stops.
            if (outputText != null)
            {
                checkSumScore();
                outputText.text = message;
            }
        }
    }

    //used to check the score/check and compare two answers.
    public void checkSumScore()
    {
        answer = "Does this work?";
        //checks the answer against the message.
        if (message == answer)
        {
            outputText2.text = "it worked";
        }
        //incase it fails.
        else
        {
            outputText2.text = "failed";
        }

    }
}