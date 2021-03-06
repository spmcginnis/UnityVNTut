﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalFunctions : MonoBehaviour
{
    // Image transition function
    public static bool TransitionImage(ref Image activeImage, ref List<Image> allImages, float speed, bool smooth )
    {
        bool didAnyValueChange = false;
        speed *= Time.deltaTime;

        //Increment down so we dont get an out-of-range exception when destroying the images we have already used.
        for (int i = allImages.Count -1; i >=0; i--)
        {
            Image image = allImages[i];

            if (image == activeImage) // active image fades in
            {
                if (image.color.a < 1f)
                {
                    image.color = SetAlpha(image.color, smooth ? Mathf.Lerp(image.color.a, 1f, speed) : Mathf.MoveTowards(image.color.a, 1f, speed));
                    didAnyValueChange = true;
                }

            }
            else // inactive image fades out and gets removed from the list
            {
                if (image.color.a > 0)
                {
                    image.color = SetAlpha(image.color, smooth ? Mathf.Lerp(image.color.a, 0f, speed) : Mathf.MoveTowards(image.color.a, 0f, speed));
                    didAnyValueChange = true;
                }
                else
                {
                    allImages.RemoveAt(i);
                    DestroyImmediate(image.gameObject);
                    continue;
                }
            }

        }


        return didAnyValueChange; // returning a bool to signal the while loop in Character.BodyTransition

    }

    // RawImage transition function for backgrounds etc
    public static bool TransitionRawImage(ref RawImage activeImage, ref List<RawImage> allImages, float speed, bool smooth)
    {
        bool didAnyValueChange = false;
        speed *= Time.deltaTime;

        //Increment down so we dont get an out-of-range exception when destroying the images we have already used.
        for (int i = allImages.Count - 1; i >= 0; i--)
        {
            RawImage image = allImages[i];

            if (image == activeImage) // active image fades in
            {
                if (image.color.a < 1f)
                {
                    image.color = SetAlpha(image.color, smooth ? Mathf.Lerp(image.color.a, 1f, speed) : Mathf.MoveTowards(image.color.a, 1f, speed));
                    didAnyValueChange = true;
                }

            }
            else // inactive image fades out and gets removed from the list
            {
                if (image.color.a > 0)
                {
                    image.color = SetAlpha(image.color, smooth ? Mathf.Lerp(image.color.a, 0f, speed) : Mathf.MoveTowards(image.color.a, 0f, speed));
                    didAnyValueChange = true;
                }
                else
                {
                    allImages.RemoveAt(i);
                    DestroyImmediate(image.gameObject);
                    continue;
                }
            }

        }


        return didAnyValueChange; // returning a bool to signal the while loop in Character.BodyTransition

    }

    public static Color SetAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

}
