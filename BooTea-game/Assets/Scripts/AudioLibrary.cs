using System.Collections.Generic;
using UnityEngine;

public class AudioLibrary : MonoBehaviour
{
    [SerializeField] private AudioGroup[] audioGroups;
    private Dictionary<string, List<AudioClip>> audioDictionary;

    public void Awake()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        audioDictionary = new Dictionary<string, List<AudioClip>>();
        foreach (AudioGroup audioGroup in audioGroups)
        {
            audioDictionary[audioGroup.name] = audioGroup.audioClips;
        }
    }

    public AudioClip GetRandomClip(string name)
    {
        if (audioDictionary.ContainsKey(name))
        {
            List<AudioClip> audioClips = audioDictionary[name];
            if (audioClips.Count > 0)
            {
                return audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
            }
        }
        return null;
    }

    public AudioClip[] GetClips(string name)
    {
        if (audioDictionary.ContainsKey(name))
        {
            return audioDictionary[name].ToArray();
        }
        return new AudioClip[0];
    }
}

[System.Serializable]
public struct AudioGroup
{
    public string name;
    public List<AudioClip> audioClips;
}