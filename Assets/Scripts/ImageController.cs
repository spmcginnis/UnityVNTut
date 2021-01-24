using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ImageController : MonoBehaviour
{
    public static ImageController instance;

    public LAYER background = new LAYER();
    public LAYER cinematic = new LAYER();
    public LAYER foreground = new LAYER();

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    [System.Serializable]
    public class LAYER
    {
        public GameObject root;
        public GameObject newImageObjectReference;
        public RawImage activeImage;
        public List<RawImage> allImages = new List<RawImage>();

        public void SetTexture(Texture texture = null) // Added default of null to enable SetTexture() to remove the current active image
        {


            if (texture != null)
            {
                if (activeImage == null)
                {
                    CreateNewActiveImage();
                }
                activeImage.texture = texture;
                activeImage.color = GlobalFunctions.SetAlpha(activeImage.color, 1f);

                // Removed depricated MovieTexture used in tutorial model.
                // See https://docs.unity3d.com/Manual/VideoPlayer-MigratingFromMovieTexture.html

            }
            else
            {
                if (activeImage != null)
                {
                    allImages.Remove(activeImage);
                    GameObject.DestroyImmediate(activeImage.gameObject);
                    activeImage = null;
                }
            }
        }

        void CreateNewActiveImage()
        {
            GameObject ob = Instantiate(newImageObjectReference, root.transform) as GameObject;
            ob.SetActive(true);
            RawImage raw = ob.GetComponent<RawImage>();
            activeImage = raw; // set the image as the active image
            allImages.Add(raw); // add it to the list for use in transitions
        }

        // SECTION: Handle Image Transitions -------------------------------------------------------------------------------
        public void StartImageTransition(Texture texture = null, float speed = 2f, bool smooth = false)
        {
            if (activeImage != null && activeImage.texture == texture)
            {
                return;
            }

            StopImageTransition();
            transitioning = ImageController.instance.StartCoroutine(Transitioning(texture, speed, smooth));
        }

        void StopImageTransition()
        {
            if(isTransitioning)
            {
                ImageController.instance.StopCoroutine(transitioning);
            }

            transitioning = null;
        }

        public bool isTransitioning { get { return transitioning != null; } }
        Coroutine transitioning = null;
        IEnumerator Transitioning(Texture texture, float speed, bool smooth)
        {
            if (texture != null) // If there is an incoming texture, we will try to create or retrieve it
            {
                foreach (RawImage image in allImages) // Check the list of images for the one we are looking for
                {
                    if (image.texture == texture) // If the current image has the same texture as our target texture, then we set it active and break the loop.
                    {
                        activeImage = image;
                        break;
                    }
                }

                if (activeImage == null || activeImage.texture != texture)
                {
                    CreateNewActiveImage();
                    activeImage.texture = texture;
                    activeImage.color = GlobalFunctions.SetAlpha(activeImage.color, 0f);

                    // Omitted depricated MovieTexture handling, as above
                }
            }
            else // If there is no incoming texture, set activeImage to null.
            {
                activeImage = null;
            }

            while (GlobalFunctions.TransitionRawImage(ref activeImage, ref allImages, speed, smooth))
            {
                yield return new WaitForEndOfFrame();
            }

            StopImageTransition();
        }
    }
}
