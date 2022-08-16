using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject mainMenuHolder;
    public GameObject optionsMenuHoder;

    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public int[] screenWidths;
    int activeScreenResIndex;
    public Toggle fullscreemToggle;
    public void Play() => SceneManager.LoadScene("GameScene");
    
    public void Quit()=>Application.Quit();
    private void Start()
    {
        activeScreenResIndex = PlayerPrefs.GetInt("screen res index");
        bool isFullscreen =(PlayerPrefs.GetInt("fullscreen")==1)?true:false;
        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = i == activeScreenResIndex;
        }
        fullscreemToggle.isOn = isFullscreen;
    }
    public void OptionsMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHoder.SetActive(true);
    }
    public void MainMenu()
    {
        mainMenuHolder.SetActive(true);
        optionsMenuHoder.SetActive(false);
    }
    public void SetScreenResolution(int i)
    {
        if (resolutionToggles[i].isOn)
        {
            activeScreenResIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i],(int)(screenWidths[i] / aspectRatio), false);
            PlayerPrefs.SetInt("screen res index", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }
    public void SetFullscreen(bool isFullscreen)
    {
        print(isFullscreen);
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullscreen;
        }
        if (isFullscreen)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            SetScreenResolution(activeScreenResIndex);
            PlayerPrefs.Save();
        }
        PlayerPrefs.SetInt("fullscreen", (isFullscreen) ? 1 : 0);
    }
   
    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolumePercent(value, AudioManager.AudioChannel.Master);
    }
    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolumePercent(value, AudioManager.AudioChannel.Music);
    }
    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolumePercent(value, AudioManager.AudioChannel.sfx);
    }
}
