using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScreenPlayLines : MonoBehaviour
{
    DialogueSystem dialogueSystem;
    int lineIndex = 0;
    List<ScreenPlay> testChapter;

    void Start()
    {
        dialogueSystem = DialogueSystem.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
