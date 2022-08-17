using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;
    public AudioClip menuTheme;
    string sceneName;
    private void Start()
    {
        OnLevelWasLoaded(0);
    }
    
    void Update()
    {

    }
    void OnLevelWasLoaded(int sceneIndex)
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        if(newSceneName != sceneName)
        {
            sceneName = newSceneName;
            Invoke("PlayMusic", .2f);
        }
    }
    public void StopMusic()
    {

    }
    void PlayMusic()
    {
        AudioClip clipToPlay = null;
        if (sceneName == "GameScene")
        {
            clipToPlay = mainTheme;
        }
        else
        {
            clipToPlay = menuTheme;
        }
        if(clipToPlay != null)
        {
            AudioManager.instance.PlayMusic(clipToPlay, 2);
            Invoke("PlayMusic", clipToPlay.length);
        }

    }
}
