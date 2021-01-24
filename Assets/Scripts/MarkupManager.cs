using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkupManager : MonoBehaviour
{
    // Primary job is to split strings into arrays based on the presence of markup.  Later it will also handle string interpolations.

    public static string[] SplitByTag(string targetText)
    {
        return targetText.Split(new char[2] { '<', '>' }); // Splits on either of the characters.
    }



}
