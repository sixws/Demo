using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{   public enum AudioChannel {Master,sfx,Music }
    public float masterVolumePercent  {get;private set; }//������
    public float sfxVolumePercent { get; private set; }//��Ч����
    public float musicVolumePercent { get; private set; }//��������

    AudioSource[] musicSources;//������������
    int activeMusicSourceIndexer;//�������������±�
    public static AudioManager instance;//����
    Transform playerT;//���λ��
    Transform audioListener;//��Чλ��
    SoundLibrary soundLibrary;//��Ч��
    AudioSource sfx2DSource;//2D��Ч
    private void Awake()
    {
        if (instance != null)
        {
            print(111);
            Destroy(gameObject);
        }
        else
        {
            print(222);
            instance = this;
            DontDestroyOnLoad(gameObject);
            soundLibrary = GetComponent<SoundLibrary>();//�õ���Ч��

            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)//�����Ч����
            {
                GameObject newMusciSource = new GameObject("Music source" + (i + 1));
                musicSources[i] = newMusciSource.AddComponent<AudioSource>();
                newMusciSource.transform.parent = transform;
            }

            audioListener = FindObjectOfType<AudioListener>().transform;//�õ���Чλ��

            if(FindObjectOfType<Player>()!=null)
            playerT = FindObjectOfType<Player>().transform;//�õ����λ��

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
            audioListener.position = playerT.position;//�����λ�ø�ֵ����Ч 
        }
    }
    public void PlaySound(AudioClip clip,Vector3 pos)//������Ч
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
    public void PlayMusic(AudioClip clip,float fadeDuration =1)//���ű�������
    {
        activeMusicSourceIndexer = 1 - activeMusicSourceIndexer;//���������±� ֻ����0��1֮�� 
        musicSources[activeMusicSourceIndexer].clip = clip; //��ֵ��Ƭ����
        musicSources[activeMusicSourceIndexer].Play();//��������
        StartCoroutine(AnimateMusicCroofade(fadeDuration));//ʵ�����ֵ��뵭��Ч��
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
            percent += Time.deltaTime * 1 / duration;//�����ٶ� ���뵭�����ٶ�
            musicSources[activeMusicSourceIndexer].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);//���뵭����������
            musicSources[1 - activeMusicSourceIndexer].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);//��Ȼ����
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