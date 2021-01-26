using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLines : MonoBehaviour
{
    LineRenderer line;
    public Color lineColor = Color.white;
    public Material material;


    void Awake()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.widthMultiplier = 10f;
        line.positionCount = 2;
        line.material = material;
    }

    // Update is called once per frame
    void Update()
    {
        // PosX 519
        // PosY 0 to 517
        // line.SetPosition = 519f;


        line.SetPosition(0, new Vector3( 519f, 0f, 1f));
        line.SetPosition(1, new Vector3(519f, 1200f, 1f));

        line.startColor = lineColor;
        line.endColor = line.startColor;


    }
}
