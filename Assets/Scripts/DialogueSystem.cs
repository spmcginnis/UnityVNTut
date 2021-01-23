using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;
    public ELEMENTS elements;

    public bool isSpeaking { get { return speaking != null; } }
    [HideInInspector] public bool isWaitingForUserInput = false;


    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Say something and print it on the screen.
    /// </summary>
    public void Say(string speech, string speaker = "")
    {
        // First ensure no one is speaking
        StopSpeaking();
        speechText.text = targetSpeech;
        // Start new coroutine for speech.
        speaking = StartCoroutine(Speaking(speech, false, speaker));
    }

    public void SayAdd(string speech, string speaker = "")
    {
        StopSpeaking();
        speechText.text = targetSpeech; // keeps text from jarbling if the key is pressed rapidly during an additive text segment.
        speaking = StartCoroutine(Speaking(speech, true, speaker));
    }

    public void StopSpeaking()
    {
        // Make sure coroutine is not null before stopping it, otherwise we might get a null reference error.
        if (isSpeaking)
        {
            StopCoroutine(speaking);
            
        }
        speaking = null;
    }

    /// <summary>
    /// Coroutine for speaking, as long as this is running, we know someone is speaking.
    /// Responsible for showing the text and the speaker's name.
    /// Also checks for current speech and for user input.
    /// </summary>

    string targetSpeech = "";
    Coroutine speaking = null;
    IEnumerator Speaking(string speech, bool additive = false, string speaker = "")
    {
        speechPanel.SetActive(true);
        targetSpeech = speech;

        if (!additive)
        {
            speechText.text = "";
        } else {
            targetSpeech = speechText.text + targetSpeech;
        }

        
        speakerNameText.text = DetermineSpeaker(speaker);
        isWaitingForUserInput = false;

        // Output one character at a time
        while(speechText.text != targetSpeech)
        {
            speechText.text += targetSpeech[speechText.text.Length];
            yield return new WaitForEndOfFrame();
        }

        isWaitingForUserInput = true;
        while(isWaitingForUserInput) // Tut omitted the curly braces here, but that seems off to me.  
        {
            yield return new WaitForEndOfFrame();
        }
        StopSpeaking();

    }

    string DetermineSpeaker(string s)
    {
        string retVal = speakerNameText.text; // default return is the current name
        if (s != speakerNameText.text && s != "")
        {
            retVal = (s.ToLower().Contains("narrator")) ? "" : s;
        }

        return retVal;
    }

    /// <summary>
    /// Close the speech panel and stop all dialogue.
    /// </summary>
    public void Close()
    {
        StopSpeaking();
        speechPanel.SetActive(false);
    }



    [System.Serializable]
    public class ELEMENTS
    {
        /// <summary>
        /// The main panel containing all dialogue-related elements on the UI
        /// </summary>
        public GameObject speechPanel;
        public Text speakerNameText;
        public Text speechText;
    }
    public GameObject speechPanel { get { return elements.speechPanel; } }
    public Text speakerNameText { get { return elements.speakerNameText; } }
    public Text speechText { get { return elements.speechText; } }


}
