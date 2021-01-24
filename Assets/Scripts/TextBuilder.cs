using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBuilder
{
    // aka TextArchitect
    // Not a monobehavior because we don't attach it to any gameobject.


    private string _currentText = "";
    public string currentText { get { return _currentText; } }

    private string previousText;
    private string targetText;

    private int charactersPerFrame = 1;

    [Range(1f,60f)]
    private float renderSpeed = 1f; // This is actually a delay (?) That is, higher value means slower text rendering.

    private bool hasMarkup = true; // useEncapsulation

    public bool skip = false; // For handling fast forwarding


    // Initialize and instance

    public TextBuilder(string targetText, string previousText = "", int charactersPerFrame = 1, float speed = 1f, bool hasMarkup = true)
    {
        this.targetText = targetText;
        this.previousText = previousText;
        this.charactersPerFrame = charactersPerFrame;
        this.renderSpeed = speed;
        this.hasMarkup = hasMarkup;

        buildProcess = DialogueSystem.instance.StartCoroutine(BuildText());

    }

    // Build Process Coroutine
    public bool isBuilding { get { return buildProcess != null; } } // A signal available outside of the class to indicate whether the coroutine is busy.
    Coroutine buildProcess = null;

    IEnumerator BuildText()
    {
        int runsThisFrame = 0;
        string[] textArray = hasMarkup ? MarkupManager.SplitByTag(targetText) : new string[1] { targetText };

        // If this is additive text, make sure we include the previous text.

        _currentText = previousText;

        // We need a storage variable to separate out the nested text.
        string nestedText = ""; // aka curText

        // Build the text by moving through each part
        for (int i = 0; i < textArray.Length; i++)
        {
            string section = textArray[i];

            // Tags will always be odd-indexed.  Even if there are two in a row, there will be an empty array between them.
            bool isOdd = i % 2 == 1;

            if (isOdd && hasMarkup)
            {
                // store the current text into something that can be referenced as a restart point as tagged sections of text get added or removed.

                nestedText = _currentText;

                NESTED_TEXT childNode = new NESTED_TEXT($"<{section}>", textArray, i);
                while (!childNode.isDone)
                {
                    //during each loop we need to call the next step in the markup process
                    // First set a boolean to indicate if a step was taken
                    bool stepped = childNode.Step();

                    _currentText = nestedText + childNode.displayText;

                    // Only yield if a step was taken in building the string
                    if (stepped)
                    {
                        runsThisFrame++;
                        int maxRunsPerFrame = skip ? 5 : charactersPerFrame; // Handles fast forwarding // Move the variable declaration up?

                        if (runsThisFrame == maxRunsPerFrame)
                        {
                            runsThisFrame = 0;
                            yield return new WaitForSeconds(skip ? 0.01f : 0.01f * renderSpeed);
                        }
                    }

                }
                i = childNode.arrayProgress + 1;
            }
            else // Not processing markup
            {
                for (int j = 0; j < section.Length; j++)
                {
                    _currentText += section[j];
                    runsThisFrame++;
                    int maxRunsPerFrame = skip ? 5 : charactersPerFrame; // Handles fast forwarding // Move the variable declaration up?

                    if (runsThisFrame == maxRunsPerFrame)
                    {
                        runsThisFrame = 0;
                        yield return new WaitForSeconds(skip ? 0.01f : 0.01f * renderSpeed);
                    }

                }
            }


        }

        // Signal the end of the build process
        buildProcess = null;

    }

    public void Stop()
    {
        if (isBuilding)
        {
            DialogueSystem.instance.StopCoroutine(buildProcess);
        }
        buildProcess = null;
    }


    private class NESTED_TEXT
    {
        private string startTag = "";
        private string endTag = "";
        private string currentText = "";
        private string targetText = "";

        public string displayText { get { return _displayText; } }
        private string _displayText = "";

        // begin looking for nested tags
        private string[] allSpeechAndTagsArray;
        private int _arrayProgress = 0;
        public int arrayProgress { get { return _arrayProgress; } }


        // Signal for process completion
        public bool isDone { get { return _isDone; } }
        private bool _isDone = false;

        public NESTED_TEXT parentMarkupNode = null;
        public NESTED_TEXT childMarkupNode = null;

        // Initialization Call
        public NESTED_TEXT(string tagName, string[] allSpeechAndTagsArray, int _arrayProgress)
        {
            this.startTag = tagName;
            this.allSpeechAndTagsArray = allSpeechAndTagsArray;
            this._arrayProgress = _arrayProgress;

            if (allSpeechAndTagsArray.Length -1 > _arrayProgress)
            {
                string nextPart = allSpeechAndTagsArray[_arrayProgress + 1];

                targetText = nextPart;

                this._arrayProgress++;
            }

            GenerateEndTag();
            
        }

        void GenerateEndTag()
        {
            endTag = startTag.Replace("<", "").Replace(">", "");

            if (endTag.Contains("="))
            {
                endTag = $"</{endTag.Split('=')[0]}>"; // This doesn't seem like it will work for <name attr=value> format, but I'm not sure unity rich text uses any elements like that.
            }
            else
            {
                endTag = $"</{endTag}>";
            }
        }

        // Function to step through the array during the build process
        // Returns true if a step was taken, false when a step must be made from a lower lever of the nesting.
        public bool Step()
        {
            if (isDone) // The current enclosure is done
            {
                return true;
            }

            if (childMarkupNode != null && !childMarkupNode.isDone) // There is a subEnclosure still being processed.
            {
                return childMarkupNode.Step();
            }
            else // Any children have already been processed, to this point.
            { 
                if (currentText == targetText) // we've finished the current text node and need to check for more children.
                {
                    if (allSpeechAndTagsArray.Length > _arrayProgress + 1) // There is another entry in the array.
                    {
                        string nextPart = allSpeechAndTagsArray[_arrayProgress + 1];
                        bool isATag = ((_arrayProgress + 1) % 2 != 0);

                        if (isATag) // the next part is a tag
                        {
                            if ($"<{nextPart}>" == endTag) // We have reached the closing tag and can end the step at this level.
                            {
                                _isDone = true;

                                // Check for parents
                                if (parentMarkupNode != null)
                                {
                                    string taggedText = startTag + currentText + endTag;
                                    parentMarkupNode.currentText += taggedText;
                                    parentMarkupNode.targetText += taggedText;

                                    // Bypass the next value of the array
                                    UpdateArrayProgress(2);
                                }
                            }
                            else // we have hit a tag within a tag
                            {
                                childMarkupNode = new NESTED_TEXT($"<{nextPart}>", allSpeechAndTagsArray, _arrayProgress + 1);
                                childMarkupNode.parentMarkupNode = this;

                                UpdateArrayProgress();

                            }

                        }
                        else // The next part is text to be added
                        {
                            targetText += nextPart;
                            UpdateArrayProgress();
                        }

                    }
                    else // This is the end of the text.
                    {
                        _isDone = true;
                    }
                }
                else // There is still text to build
                {
                    currentText += targetText[currentText.Length];

                    // Update the display text, which also means updating any parents if this is a child node.
                    UpdateDisplayText("");

                    return true; // a step was taken
                }
            }
            
            return false; // is this necessary?
        }

        void UpdateArrayProgress(int increment = 1)
        {
            _arrayProgress += increment;

            if (parentMarkupNode != null)
            {
                parentMarkupNode.UpdateArrayProgress(increment);
            }
        }

        void UpdateDisplayText(string childText)
        {
            _displayText = $"{startTag}{currentText}{childText}{endTag}";
        
            if (parentMarkupNode != null)
            {
                parentMarkupNode.UpdateDisplayText(displayText);
            }
        }


    }
}
