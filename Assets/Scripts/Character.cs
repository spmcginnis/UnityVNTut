using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Represents any and all characters that appear in the VN.

[System.Serializable] //Enables viewing them in the editor
public class Character
{
    public string characterName;

    /// <summary>
    /// Reference the dialogue system, to be assigned at character creation.
    /// </summary>
    DialogueSystem dialogue;

    public void Say(string speech)
    {
        dialogue.Say(speech, characterName);
    }

    /// <summary>
    /// A root container for all images related to a given character in the current scene.
    /// </summary>
    [HideInInspector] public RectTransform root;

    /// <summary>
    /// Only necessary if using both kinds of character images
    /// </summary>
    /// public bool isMultiLayerCharacter { get { return renderers.rendererSingle == null;  } }

    /// <summary>
    /// An initialization call to create a new character and pass in some params to customize it at creation.
    /// This is also where we will load the prefab and spawn it in the scene.
    /// </summary>
    /// <param name="_name"></param>
    public Character (string _name)
    {
        CharacterManager CM = CharacterManager.instance;

        // locate the prefab
        GameObject prefab = Resources.Load($"CharacterPrefabs/Character[{_name}]") as GameObject;
        
        //instantiate the prefab into the scene, directly on the character panel
        GameObject ob = GameObject.Instantiate(prefab, CM.characterPanel);

        root = ob.GetComponent<RectTransform>();
        characterName = _name;

        //get the renderers 
        // if(isMultiLayerCharacter) ...

        renderers.bodyRenderer = ob.transform.Find("RenderBody").GetComponent<Image>();
        renderers.faceRenderer = ob.transform.Find("RenderFace").GetComponent<Image>();

        dialogue = DialogueSystem.instance;


    }

    [System.Serializable]
    public class Renderers
    {
        // RawImage used for a single-layer raw image character.  Can be flipped and stuff.
        // public RawImage rendererSingle; 

        //Image used for sprites and multi-layer sprites.
        public Image bodyRenderer;
        public Image faceRenderer;
    }

    public Renderers renderers = new Renderers(); // create an instance of the Renderers class


}
