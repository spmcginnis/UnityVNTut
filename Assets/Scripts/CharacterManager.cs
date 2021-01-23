using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Will spawn characters as we need them.

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance; // A static instance of the class that can be accessed from elsewhere easily.

    /// <summary>
    /// All chars must be spawned attached to the character panel.
    /// </summary>
    public RectTransform characterPanel;

    /// <summary>
    /// A list of all chars currently in the scene.
    /// </summary>
    public List<Character> characters = new List<Character>();

    /// <summary>
    /// A dictionary for indexing characters for quick lookup.
    /// </summary>
    public Dictionary<string, int> characterDictionary = new Dictionary<string, int>();

    /// <summary>
    /// Return an instance of a desired char
    /// </summary>
    public Character GetCharacter(string characterName, bool shouldCreateNewCharacter = true, bool enableNewCharacterOnStart = true)
    {
        int index = -1; // a temporary storage integer

        // Search the diciontary to find the character quickly if it is already in our scene
        if (characterDictionary.TryGetValue(characterName, out index))
        {
            return characters[index];
        } else if (shouldCreateNewCharacter) {
            return CreateNewCharacter(characterName, enableNewCharacterOnStart);
        }

        return null;
    }

    /// <summary>
    /// Creates a new character
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public Character CreateNewCharacter(string characterName, bool enabledOnStart = true)
    {
        Character newCharacter = new Character(characterName, enabledOnStart);

        characterDictionary.Add(characterName, characters.Count);
        characters.Add(newCharacter);

        return newCharacter;
    }


    void Awake()
    {
        instance = this;
    }


}
