﻿using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;

public class TextToSpeech : MonoBehaviour
{
    // Hook up the three properties below with a Text, InputField and Button object in your UI.
    public Text outputText;
    public InputField inputField;
    public Button speakButton;
    public AudioSource audioSource;

    //uses the following to track if it is playing or not and the message.
    private object threadLocker = new object();
    private bool waitingForSpeak;
    private string message;

    //uses the following to play the message and decode it
    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;

    //tracks to play
    public void ButtonClick()
    {
        lock (threadLocker)
        {
            waitingForSpeak = true;
        }

        string newMessage = string.Empty;

        // Starts speech synthesis, and returns after a single utterance is synthesized.
        using (var result = synthesizer.SpeakTextAsync(inputField.text).Result)
        {
            // Checks result.
            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
                // Use the Unity API to play audio here as a short term solution.
                // Native playback support will be added in the future release.
                var sampleCount = result.AudioData.Length / 2;
                var audioData = new float[sampleCount];
                for (var i = 0; i < sampleCount; ++i)
                {
                    audioData[i] = (short)(result.AudioData[i * 2 + 1] << 8 | result.AudioData[i * 2]) / 32768.0F;
                }

                // The output audio format is 16K 16bit mono
                var audioClip = AudioClip.Create("SynthesizedAudio", sampleCount, 1, 16000, false);
                audioClip.SetData(audioData, 0);
                audioSource.clip = audioClip;
                audioSource.Play();

                newMessage = "Speech synthesis succeeded!";
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                newMessage = $"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?";
            }
        }

        lock (threadLocker)
        {
            message = newMessage;
            waitingForSpeak = false;
        }
    }

    //run at start.
    void Start()
    {
        //if it is missing its output
        if (outputText == null)
        {
            UnityEngine.Debug.LogError("outputText property is null! Assign a UI Text element to it.");
        }
        // if it is missing its input text value
        else if (inputField == null)
        {
            message = "inputField property is null! Assign a UI InputField element to it.";
            UnityEngine.Debug.LogError(message);
        }
        //missing speak button
        else if (speakButton == null)
        {
            message = "speakButton property is null! Assign a UI Button to it.";
            UnityEngine.Debug.LogError(message);
        }
        else
        {
            // Continue with normal initialization, Text, InputField and Button objects are present.
            inputField.text = "Enter text you wish spoken here.";
            message = "Click button to synthesize speech";
            speakButton.onClick.AddListener(ButtonClick);

            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            speechConfig = SpeechConfig.FromSubscription("879c496fe1ca49b7b44104e656ea5451", "eastus");

            // The default format is Riff16Khz16BitMonoPcm.
            // We are playing the audio in memory as audio clip, which doesn't require riff header.
            // So we need to set the format to Raw16Khz16BitMonoPcm.
            speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw16Khz16BitMonoPcm);

            // Creates a speech synthesizer.
            // Make sure to dispose the synthesizer after use!
            synthesizer = new SpeechSynthesizer(speechConfig, null);
        }
    }

    void Update()
    {
        lock (threadLocker)
        {
            //if recording does not equal null it records, gets permission, and sets recording started to true
            if (speakButton != null)
            {
                speakButton.interactable = !waitingForSpeak;
            }
            //when no more text is being input it stops.
            if (outputText != null)
            {
                outputText.text = message;
            }
        }
    }
    //deletes the talking voice so it doesnt loop.
    void OnDestroy()
    {
        synthesizer.Dispose();
    }
}

