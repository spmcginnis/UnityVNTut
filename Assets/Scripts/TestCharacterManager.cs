using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterManager : MonoBehaviour
{
    public Character Monk;

    // Start is called before the first frame update
    void Start()
    {
        Monk = CharacterManager.instance.GetCharacter("Monk");
    }

    public string[] speech;
    int i = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (i<speech.Length)
            {
                Monk.Say(speech[i]);
                i++;
            } else {
                DialogueSystem.instance.Close(); //close and hide itself.
            }
            
        }
    }
}
