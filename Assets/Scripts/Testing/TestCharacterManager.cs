using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterManager : MonoBehaviour
{
    public Character Monk;

    // Start is called before the first frame update
    void Start()
    {
        // Create a character instance
        Monk = CharacterManager.instance.GetCharacter("Monk", enableNewCharacterOnStart: false);
        Monk.GetSprite(4);

    }

    public string[] speech;
    int i = 0;


    // SECTION: Testing the character movements
    public Vector2 positionA = new Vector2(-2, 0);
    public Vector2 positionB = new Vector2(.5f, 0);
    public Vector2 positionC = new Vector2(3, 0);
    public float moveSpeed;
    public bool smooth;

    public bool endScene = false;

    // SECTION: Testing the sprite transitions
    public int bodySpriteIndex;
    public int faceSpriteIndex;
    public float transitionSpeed = 2f;
    public bool smoothTransition = true;



    // Update is called once per frame
    void Update()
    {
        // Speech Handler
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (i < speech.Length)
            {
                if (!Monk.isEnabled)
                {
                    Monk.MoveTo(positionA, 100);
                    Monk.isEnabled = true;
                    Monk.MoveTo(positionB, moveSpeed, smooth);
                }
                faceSpriteIndex = bodySpriteIndex == 4 ? 3 : (i % 3)%2;
                Monk.StartFaceTransition(Monk.GetSprite(faceSpriteIndex), transitionSpeed, smoothTransition);
                Monk.Say(speech[i]);
                i++;
            }
            else if (!endScene)
            {
                Monk.Say("Um... see you!");
                Monk.MoveTo(positionC, moveSpeed, smooth);
                endScene = true;
            } else
            {
                DialogueSystem.instance.Close(); //close and hide itself.
            }
        }

        // Movement Handler
        if (Input.GetKey(KeyCode.A))
        {
            Monk.MoveTo(positionB, moveSpeed, smooth);
        }

        // Expression Handler
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (endScene) { return; }

            if (bodySpriteIndex!=4)
            {
                bodySpriteIndex = 4;
                faceSpriteIndex = 3;
                //Monk.SetBody(bodySpriteIndex);
                Monk.StartBodyTransition(Monk.GetSprite(bodySpriteIndex), transitionSpeed, smoothTransition);
                Monk.StartFaceTransition(Monk.GetSprite(faceSpriteIndex), transitionSpeed, smoothTransition);
            } else
            {
                Monk.Say("Waaaaaa!");
                Monk.MoveTo(positionC, moveSpeed/2, smooth);
                speech = new string[0];
            }

        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Monk.SetFace(0);
        }

        // Testing Image Transitions
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (endScene) { return; }

            bodySpriteIndex = bodySpriteIndex == 5 ? 4 : 5;
            faceSpriteIndex = bodySpriteIndex == 5 ? 0 : 3;
            Monk.StartBodyTransition(Monk.GetSprite(bodySpriteIndex), transitionSpeed, smoothTransition);
            Monk.StartFaceTransition(Monk.GetSprite(faceSpriteIndex), transitionSpeed, smoothTransition);
        }


    }
}
