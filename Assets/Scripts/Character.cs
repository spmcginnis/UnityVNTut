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

    public bool isEnabled
    { 
        get { return root.gameObject.activeInHierarchy; }
        set { root.gameObject.SetActive(value); }
    }


    public Character (string _name, bool enabledOnStart = true)
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

        isEnabled = enabledOnStart;
    }

    public void Say(string speech, bool isAdditive = false)
    {
        if(!isEnabled)
        {
            isEnabled = true;
        }
        if (isAdditive)
        {
            dialogue.SayAdd(speech, characterName);
        } else
        {
            dialogue.Say(speech, characterName);
        }
    }


    // SECTION: Character Sprite Movement ---------------------------------------------------------------------------
    public Vector2 anchorPadding { get { return root.anchorMax - root.anchorMin; } }
    Vector2 targetPosition;
    Coroutine moving;
    bool isMoving { get { return moving != null; } }

    public void MoveTo(Vector2 target, float speed, bool smooth = true)
    {
        StopMoving();

        moving = CharacterManager.instance.StartCoroutine(Moving(target, speed, smooth));
    }

    IEnumerator Moving(Vector2 target, float speed, bool smooth)
    {
        targetPosition = target;
        // We need to establish padding for the movement since the position is calculated from the lower right corner of the sprite's bounding box.  The padding keeps it from going off screen.
        Vector2 padding = anchorPadding;
        float maxX = 1f - padding.x;
        float maxY = 1f - padding.y;
        // maxX and maxY are just percentages to apply to our target position.
        // now get the actual position target for the minimum anchors (left/bottom bounds) of the character sprite bounding.
        Vector2 minAnchorTarget = new Vector2(maxX * targetPosition.x, maxY * targetPosition.y);

        speed *= Time.deltaTime;

        // Move until we reach the target position, either linear or smooth(lerp)
        while (root.anchorMin != minAnchorTarget)
        {
            root.anchorMin = (!smooth) ?
                Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed) : Vector2.Lerp(root.anchorMin, minAnchorTarget, speed);
            root.anchorMax = root.anchorMin + padding;
            yield return new WaitForEndOfFrame();
        }

        StopMoving();

    }

    public void StopMoving()
    {
        if(isMoving)
        {
            CharacterManager.instance.StopCoroutine(moving);
        }
        moving = null;
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
