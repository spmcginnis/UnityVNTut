using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterManager : MonoBehaviour
{
    public Character Monk;

    // Start is called before the first frame update
    void Start()
    {
        Monk = CharacterManager.instance.GetCharacter("Monk", enableNewCharacterOnStart: false);
    }

    public string[] speech;
    int i = 0;

    public Vector2 positionA = new Vector2(-2, 0);
    public Vector2 positionB = new Vector2(.5f, 0);
    public Vector2 positionC = new Vector2(3, 0);
    public float moveSpeed;
    public bool smooth;
    
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

                Monk.Say(speech[i]);
                i++;
            }
            else if (Monk.isEnabled)
            {
                Monk.Say("Um... see you!");
                Monk.MoveTo(positionC, moveSpeed, smooth);
            }
            else
            {
                Monk.isEnabled = false;
                DialogueSystem.instance.Close(); //close and hide itself.
            }
        }

        // Movement Handler
        if (Input.GetKey(KeyCode.A))
        {
            Monk.MoveTo(positionB, moveSpeed, smooth);
        }

    }
}
