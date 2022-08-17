using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;
    public RectTransform newWaveBanner;
    public Text newWaveTile;
    public Text newWaveEnemyCount;
    Spawn Spawn;
    public Color to;
    public Text scprseUI;
    public GameObject stopUI;
    private AudioSource clip;
    public static bool stop = false;
    void OnNewWave(int waveNumber)
    {
       

        string[] numbers = { "一", "二", "三", "四", "五" };
        newWaveTile.text = "-第 "+ numbers[waveNumber - 1]+" 波-";
        newWaveEnemyCount.text ="敌人数量:"+Spawn.waves[waveNumber-1].enemyCount;
        if(waveNumber ==5)
            newWaveEnemyCount.text = "敌人数量:无限";
        StartCoroutine("AnimateNewWaveBanner");
    }
    private void Awake()
    {
        Spawn =FindObjectOfType<Spawn>();
        Spawn.OnNewWave += OnNewWave;
    }
    private void Start()
    {
        FindObjectOfType<Player>().OnDath += OnGameOver;
    }
    void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear,to, 1));
        gameOverUI.SetActive(true);
    }
    IEnumerator Fade(Color form,Color to,float time)
    {
        float speed = 1 / time;
        float percent = 0;
        while (percent<=1)
        {
            percent+=Time.deltaTime*speed;
            fadePlane.color =Color.Lerp(form,to,percent);
            yield return null;  
        }
    }
    public void StatNewScene() => SceneManager.LoadScene("GameScene");
    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 2f;
        float spped = 2.5f;
        float percent = 0;
        int dir = 1;
        float endDelayTime = 1 / spped+Time.time + delayTime;  // 0+1/4 //1/4+0+1;
        while (percent>=0)
        {
            percent += Time.deltaTime * spped * dir;
            if (percent >= 1)
            {
                percent = 1;
                if (Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }
            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(40, 520, percent);
            yield return null;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            stop = true;
            Time.timeScale = 0;
            Cursor.visible = false;
            stopUI.SetActive(true);
            if (FindObjectsOfType<AudioSource>() != null)
            {
                AudioSource[] clips = FindObjectsOfType<AudioSource>();
                for (int i = 0; i < clips.Length; i++)
                {
                    if (clips[i].clip!= null)
                    {
                        clip = clips[i];
                    }
                }
                clip.mute = true;
            } 
          
        }
        else if  (Input.GetKeyDown(KeyCode.Space))
        {
            Cursor.visible = false;
            stop = false;
            Time.timeScale = 1;
            stopUI.SetActive(false);
            clip.mute = false;
            
        }

    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Begin Scene");
    }
}
