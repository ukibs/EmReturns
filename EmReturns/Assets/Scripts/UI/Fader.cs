using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    //
    public int fadeDirection = 1;
    public float fadeDuration = 1;
    public float maxAlpha = 1;

    //
    private Image image;
    private bool isPlaying = false;
    private float currentFade;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentFade < fadeDuration)
        {
            float dt = Time.deltaTime;
            currentFade += dt * fadeDirection;
            image.color = new Color(image.color.r, image.color.g, image.color.b, (currentFade / fadeDuration) * maxAlpha);
        }
    }
}
