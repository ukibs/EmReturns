using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EM_VoiceController : MonoBehaviour
{
    [SerializeField] public EM_VoiceGroup[] voiceGroups;

    private AudioSource audioSource;
    private static EM_VoiceController instance;

    public static EM_VoiceController Instance
    {
        get { return instance; }
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayVoiceGroup(string groupName)
    {
        EM_VoiceGroup voiceGroup = voiceGroups.FirstOrDefault(p => p.groupName == groupName);
        audioSource.clip = voiceGroup.voiceClips[Random.Range(0, voiceGroup.voiceClips.Length)];
        audioSource.Play();
    }
}

[System.Serializable]
public class EM_VoiceGroup
{
    public string groupName;
    public AudioClip[] voiceClips;
}