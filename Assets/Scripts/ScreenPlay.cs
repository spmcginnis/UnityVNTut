using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPlay
{
    // This should handle all of the screenplay elements, such as a list of speakers, as well as the text for a given section of screenplay.
    // The text itself should be generated in another class using the properties and methods of this class.
    // There should, perhaps, be a third class to manage the two.

    // string castOfCharacters;
    string speaker; // TODO refactor into a speaker object or a list of them.
    string storyText;
    string stageDirection; // A command line to be interpreted with a method.  
    // string specialConditions; // Will need a way to handle things like whether a scene is accesible given the game state.

    public ScreenPlay Line(string speaker = "", string storyText = "", string stageDirection = "")
    {
        this.speaker = speaker;
        this.storyText = storyText;
        this.stageDirection=stageDirection;
        return this;
    }
}
