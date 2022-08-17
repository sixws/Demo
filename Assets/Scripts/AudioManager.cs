using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{   public enum AudioChannel {Master,sfx,Music }
    public float masterVolumePercent  {get;private set; }//主音量
    public float sfxVolumePercent { get; private set; }//音效音量
    public float musicVolumePercent { get; private set; }//背景音乐

    AudioSource[] musicSources;//背景音乐数组
    int activeMusicSourceIndexer;//背景音乐数组下标
    public static AudioManager instance;//单例
    Transform playerT;//玩家位置
    Transform audioListener;//音效位置
    SoundLibrary soundLibrary;//音效库
    AudioSource sfx2DSource;//2D音效
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            soundLibrary = GetComponent<SoundLibrary>();//得到音效库

            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)//添加音效主件
            {
                GameObject newMusciSource = new GameObject("Music source" + (i + 1));
                musicSources[i] = newMusciSource.AddComponent<AudioSource>();
                newMusciSource.transform.parent = transform;
            }

            audioListener = FindObjectOfType<AudioListener>().transform;//得到音效位置

            if(FindObjectOfType<Player>()!=null)
            playerT = FindObjectOfType<Player>().transform;//得到玩家位置

            GameObject newSfx2DSource = new GameObject("sfx2DSource");
            sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
            sfx2DSource.transform.parent = transform;
            masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1);
            sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1);
            musicVolumePercent = PlayerPrefs.GetFloat("music vol", 1);
        }
    }
    private void Update()
    {
        if (playerT != null)
        {
            audioListener.position = playerT.position;//把玩家位置赋值给音效 
        }
    }
    public void PlaySound(AudioClip clip,Vector3 pos)//播放音效
    {
        AudioSource.PlayClipAtPoint(clip,pos,masterVolumePercent *sfxVolumePercent);
    }
    public void PlaySound(string name,Vector3 pos)
    {
        PlaySound(soundLibrary.GetClipFromName(name),pos);
    }
    public void PlaySound2D(string name)
    {
        sfx2DSource.PlayOneShot(soundLibrary.GetClipFromName(name), masterVolumePercent * sfxVolumePercent);
    }
    public void PlayMusic(AudioClip clip,float fadeDuration =1)//播放背景音乐
    {
        activeMusicSourceIndexer = 1 - activeMusicSourceIndexer;//设置数组下标 只会在0和1之间 
        musicSources[activeMusicSourceIndexer].clip = clip; //赋值切片音乐
        musicSources[activeMusicSourceIndexer].Play();//播放音月
        StartCoroutine(AnimateMusicCroofade(fadeDuration));//实现音乐淡入淡出效果
    }
    public void SetVolumePercent(float volumePercent, AudioChannel audioChannel)
    {
        switch (audioChannel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.sfx:
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;       
        }
        musicSources[0].volume = masterVolumePercent * musicVolumePercent;
        musicSources[1].volume = masterVolumePercent * musicVolumePercent;
        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
        PlayerPrefs.Save();
    }
    IEnumerator AnimateMusicCroofade(float duration)
    {
        float percent = 0;
        while (percent <= 1)
        {
            percent += Time.deltaTime * 1 / duration;//设置速度 淡入淡出的速度
            musicSources[activeMusicSourceIndexer].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);//淡入淡出播放音乐
            musicSources[1 - activeMusicSourceIndexer].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);//当然调查
            yield return null;
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        if (playerT == null)
        {
            if (FindObjectOfType<Player>() != null)
                playerT = FindObjectOfType<Player>().transform;
        }
    }
}