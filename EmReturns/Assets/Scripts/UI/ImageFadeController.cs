using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ImageFadeController : MonoBehaviour
{
    public Image image;
    public float fadeDuration = 0.5f;
    public float minSize = 1;
    public float maxSize = 30;
    public AudioClip fadeInClip;
    public AudioClip fadeOutClip;

    private static ImageFadeController instance;
    private int lastFadeDirection = -1;

    public static ImageFadeController Instance
    {
        get { return instance; }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        image.gameObject.SetActive(true);
        AudioManager.Instance.Play2dFx(Vector3.zero, fadeInClip, 0.5f);
        StartCoroutine(FadeCoroutine(-1));
    }

    // Update is called once per frame
    void Update()
    {
        //if (Keyboard.current.oKey.wasPressedThisFrame)
        //{
        //    if (lastFadeDirection == -1)
        //    {
        //        StartCoroutine(FadeCoroutine(1));
        //        lastFadeDirection = 1;
        //    }
        //    else
        //    {
        //        StartCoroutine(FadeCoroutine(-1));
        //        lastFadeDirection = -1;
        //    }
        //}
    }

    public void FadeAndGoToScene(int sceneIndex)
    {
        StartCoroutine(FadeCoroutine(1, sceneIndex));
    }

    public void FadeAndGoToLevel(int levelIndex)
    {
        StartCoroutine(FadeCoroutine(1, 2, levelIndex));
    }

    IEnumerator FadeCoroutine(int direction, int sceneIndex = -1, int levelIndex = -1)
    {
        float stepTime = 0.01f;
        float currentDuration = 0;
        float startSize = direction == 1 ? minSize : maxSize;
        float endSize = direction == 1 ? maxSize : minSize;

        if (direction == 1) { 
            image.gameObject.SetActive(true);
            AudioManager.Instance.Play2dFx(Vector3.zero, fadeOutClip, 0.3f);
        }
        else
        {
            AudioManager.Instance.Play2dFx(Vector3.zero, fadeInClip, 0.3f);
        }

        while (currentDuration < fadeDuration)
        {
            image.rectTransform.localScale = Vector3.one * Mathf.Lerp(startSize, endSize, currentDuration / fadeDuration);
            image.rectTransform.eulerAngles = new Vector3(0, 0, 360 * (currentDuration / fadeDuration));
            yield return new WaitForSeconds(stepTime);
            currentDuration += stepTime;
        }

        if(direction == -1) image.gameObject.SetActive(false);

        // Go to scene
        if(sceneIndex != -1)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        // Go to level
        if (levelIndex != -1)
        {
            GameManager.Instance.SetLevel(levelIndex);
            SceneManager.LoadScene("MainScene");
        }
    }
}
