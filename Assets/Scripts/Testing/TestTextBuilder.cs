using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTextBuilder : MonoBehaviour
{
    public Text text;
    TextBuilder builder;

    [TextArea(5,10)]
    public string say;

    public int charactersPerFrame = 1;
    public float renderDelay = 1f;
    public bool hasMarkup = true;

    // Start is called before the first frame update
    void Start()
    {
        builder = new TextBuilder(say);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            builder = new TextBuilder(say, "", charactersPerFrame, renderDelay, hasMarkup);
        }

        text.text = builder.currentText;
    }
}
