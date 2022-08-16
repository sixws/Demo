using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] soundGroups;
    Dictionary<string, AudioClip[]> soundGroupsDict = new Dictionary<string, AudioClip[]>();
    private void Awake()
    {
        foreach (var item in soundGroups)
        {
            soundGroupsDict.Add(item.groupID, item.group);
        }
    }
    public AudioClip GetClipFromName(string name)
    {
        if (soundGroupsDict.ContainsKey(name))
        {
            AudioClip[] sounds = soundGroupsDict[name];
            return sounds[Random.Range(0, sounds.Length)];
        }
        return null;
    }
    [System.Serializable]
    public class SoundGroup
    {
        public string groupID;
        public AudioClip[] group;
    }
}
