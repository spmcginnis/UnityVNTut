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

    // Initialize Character Object Instance -------------------------------------------------------------------------------------
    public Character (string _name, bool enabledOnStart = true)
    {
        CharacterManager CM = CharacterManager.instance;

        // locate the prefab
        GameObject prefab = Resources.Load($"CharacterPrefabs/Character[{_name}]") as GameObject;
        
        //instantiate the prefab into the scene, directly on the character panel
        GameObject ob = GameObject.Instantiate(prefab, CM.characterPanel);

        root = ob.GetComponent<RectTransform>();
        characterName = _name;

        // Set the renderers 
        // if(isMultiLayerCharacter) ... // needed if mixing single- and multiple-image types

        renderers.bodyRenderer = ob.transform.Find("RenderBody").GetComponentInChildren<Image>();
        renderers.faceRenderer = ob.transform.Find("RenderFace").GetComponentInChildren<Image>();
        
        // Add the renderers to the lists of renderers
        renderers.allBodyRenderers.Add(renderers.bodyRenderer);
        renderers.allFaceRenderers.Add(renderers.faceRenderer);


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


    // SECTION: Image Transitions ---------------------------------------------------
    public Sprite GetSprite(int i = 0)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>($"Images/CharacterSprites/{characterName}/{characterName}");

        Debug.Log(sprites.Length);
        
        return sprites[i];
    }

    public Sprite GetSprite(string charName, int i = 0)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>($"Images/CharacterSprites/{characterName}");

        Debug.Log(sprites.Length);
        
        return sprites[i];
    }


    // Setting manually (by index).  Depricated in favor of transitions
    public void SetBody(int i)
    {
        renderers.bodyRenderer.sprite = GetSprite(i);
    }
    public void SetBody(Sprite body)
    {
        renderers.bodyRenderer.sprite = body;
    }
    public void SetFace(int i)
    {
        renderers.faceRenderer.sprite = GetSprite(i);
    }
    public void SetFace(Sprite face)
    {
        renderers.faceRenderer.sprite = face;
    }

    // Body Transition Coroutine Handling:
        // A) bool to register coroutine status.
        // B) initialized coroutine set to null.
        // C) Function to start coroutine.
        // D) Function to stop coroutine.
        // E) IEnumerator to...
        // Note: Because the Character class does not extend MonoBehavior, it cannot itself run or stop a coroutine, so we use a class that can, in this case, CharacterManager.
    bool isBodyTransitioning { get { return bodyTransition != null; } } //returns true if the coroutine is not null, that is, if the coroutine is running.
    Coroutine bodyTransition = null; // initialize coroutine and set to null

    public void StartBodyTransition(Sprite sprite, float speed, bool smooth)
    {
        if (renderers.bodyRenderer.sprite == sprite) { return; } // Escape the function if the new sprite is the same as the current one.
        StopBodyTransition(); // stop any current transition before starting a new one
        bodyTransition = CharacterManager.instance.StartCoroutine(BodyTransition(sprite, speed, smooth)); // Calls the IEnumerator and passes the parameter set.
    }

    void StopBodyTransition()
    {
        if (isBodyTransitioning)
        {
            CharacterManager.instance.StopCoroutine(bodyTransition);
        }
    }

    public IEnumerator BodyTransition (Sprite sprite, float speed, bool smooth)
    {
        foreach (Image renderer in renderers.allBodyRenderers) // Check the list of renderers for the one we are looking for
        {
            if (renderer.sprite == sprite) // If the current renderer has the same sprite as our target sprite, then we set it active and break the loop.
            {
                renderers.bodyRenderer = renderer;
                break;
            }
        }

        if (renderers.bodyRenderer.sprite != sprite) // if the bodyRenderer does not reference our sprite, then we know a suitable target was not found in the list and we need to create one.
        {
            Image image = GameObject
                .Instantiate (renderers.bodyRenderer.gameObject, renderers.bodyRenderer.transform.parent)
                .GetComponent<Image>();
            renderers.allBodyRenderers.Add(image); // adds the new image to the list
            renderers.bodyRenderer = image; // makes the image active
            image.color = GlobalFunctions.SetAlpha(image.color, 0f); //Sets alpha to zero to start.
            image.sprite = sprite; // Sets the image to use the new sprite


        }

        while (GlobalFunctions.TransitionImage(ref renderers.bodyRenderer, ref renderers.allBodyRenderers, speed, smooth))
        {
            yield return new WaitForEndOfFrame();
        }

        StopBodyTransition();

    }

    // Face Transition Coroutine Handling
    bool isFaceTransitioning { get { return faceTransition != null; } } //returns true if the coroutine is not null, that is, if the coroutine is running.
    Coroutine faceTransition = null; // initialize coroutine and set to null

    public void StartFaceTransition(Sprite sprite, float speed, bool smooth)
    {
        if (renderers.faceRenderer.sprite == sprite) { return; } // Escape the function if the new sprite is the same as the current one.
        StopFaceTransition(); // stop any current transition before starting a new one
        faceTransition = CharacterManager.instance.StartCoroutine(FaceTransition(sprite, speed, smooth)); // Calls the IEnumerator and passes the parameter set.
    }

    void StopFaceTransition()
    {
        if (isFaceTransitioning)
        {
            CharacterManager.instance.StopCoroutine(faceTransition);
        }
    }

    public IEnumerator FaceTransition(Sprite sprite, float speed, bool smooth)
    {
        foreach (Image renderer in renderers.allFaceRenderers) // Check the list of renderers for the one we are looking for
        {
            if (renderer.sprite == sprite) // If the current renderer has the same sprite as our target sprite, then we set it active and break the loop.
            {
                renderers.faceRenderer = renderer;
                break;
            }
        }

        if (renderers.faceRenderer.sprite != sprite) // if the bodyRenderer does not reference our sprite, then we know a suitable target was not found in the list and we need to create one.
        {
            Image image = GameObject
                .Instantiate(renderers.faceRenderer.gameObject, renderers.faceRenderer.transform.parent)
                .GetComponent<Image>();
            renderers.allFaceRenderers.Add(image); // adds the new image to the list
            renderers.faceRenderer = image; // makes the image active
            image.color = GlobalFunctions.SetAlpha(image.color, 0f); //Sets alpha to zero to start.
            image.sprite = sprite; // Sets the image to use the new sprite


        }

        while (GlobalFunctions.TransitionImage(ref renderers.faceRenderer, ref renderers.allFaceRenderers, speed, smooth))
        {
            yield return new WaitForEndOfFrame();
        }

        StopFaceTransition();

    }



    [System.Serializable]
    public class Renderers
    {
        // RawImage used for a single-layer raw image character.  Can be flipped and stuff.
        // public RawImage rendererSingle; 

        //Image used for sprites and multi-layer sprites.
        public Image bodyRenderer;
        public Image faceRenderer;

        // More renderers for image transitions
        public List<Image> allBodyRenderers = new List<Image>();
        public List<Image> allFaceRenderers = new List<Image>();
    }

    public Renderers renderers = new Renderers(); // create an instance of the Renderers class


}
