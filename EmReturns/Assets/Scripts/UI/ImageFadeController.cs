using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ImageFadeController : MonoBehaviour
{
    public Image image;
    public float fadeDuration = 0.5f;
    public float minSize = 1;
    public float maxSize = 30;

    private int lastFadeDirection = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            if (lastFadeDirection == -1)
            {
                StartCoroutine(FadeCoroutine(1));
                lastFadeDirection = 1;
            }
            else
            {
                StartCoroutine(FadeCoroutine(-1));
                lastFadeDirection = -1;
            }
        }
    }

    IEnumerator FadeCoroutine(int direction)
    {
        float stepTime = 0.01f;
        float currentDuration = 0;
        float startSize = direction == 1 ? minSize : maxSize;
        float endSize = direction == 1 ? maxSize : minSize;

        if (direction == 1) image.gameObject.SetActive(true);

        while (currentDuration < fadeDuration)
        {
            image.rectTransform.localScale = Vector3.one * Mathf.Lerp(startSize, endSize, currentDuration / fadeDuration);
            image.rectTransform.eulerAngles = new Vector3 (0, 0, 360 * (currentDuration / fadeDuration));
            yield return new WaitForSeconds(stepTime);
            currentDuration += stepTime;
        }

        if(direction == -1) image.gameObject.SetActive(false);
    }
}
