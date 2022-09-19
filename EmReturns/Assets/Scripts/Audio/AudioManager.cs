using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //
    public AudioSource musicAS;
    public GameObject fx3dObjectPrefab;
    public GameObject fx2dObjectPrefab;
    public AudioSource emLoadFx;

    //
    private static AudioManager instance;

    //
    public static AudioManager Instance { get { return instance; } }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //
        musicAS.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //
    public void Play3dFx(Vector3 position, AudioClip clip, float volume = 1)
    {
        //Debug.Log("Playing clip: " + clip.name);
        GameObject fxObject = Instantiate(fx3dObjectPrefab, position, Quaternion.identity);
        AudioSource audioSource = fxObject.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(fxObject, clip.length);
    }

    //
    public void Play2dFx(Vector3 position, AudioClip clip, float volume = 1)
    {
        GameObject fxObject = Instantiate(fx2dObjectPrefab, position, Quaternion.identity);
        AudioSource audioSource = fxObject.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(fxObject, clip.length);
    }

    //
    public void PlayLoadFx(AudioClip clip, bool loop, float volume = 1)
    {
        emLoadFx.clip = clip;
        emLoadFx.volume = volume;
        emLoadFx.loop = loop;
        emLoadFx.Play();
    }

    //
    public void StopLoadFx()
    {
        emLoadFx.Stop();
    }

    //
    public void PlayMusic(AudioClip clip, float volume = 1, bool loop = true) 
    { 
        musicAS.loop = loop;
        musicAS.clip = clip;
        musicAS.volume = volume;
        musicAS.Play();
    }
}
