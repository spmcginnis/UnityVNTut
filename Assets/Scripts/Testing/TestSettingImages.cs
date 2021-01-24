using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSettingImages : MonoBehaviour
{

    ImageController controller;

    public Texture texture;

    // Start is called before the first frame update
    void Start()
    {
        controller = ImageController.instance;
        
    }

    // Update is called once per frame
    void Update()
    {
        ImageController.LAYER layer = null;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            layer = controller.background;
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            layer = controller.cinematic;
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            layer = controller.foreground;
        }
        else
        {
            layer = controller.background;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {

            //layer.SetTexture(texture);

            layer.StartImageTransition(texture);

        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            layer.StartImageTransition();
            //layer.SetTexture();
        }
    }
}
