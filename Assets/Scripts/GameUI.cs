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
    public ScoreKeeper ScoreKeeper;
    void OnNewWave(int waveNumber)
    {
       

        string[] numbers = { "һ", "��", "��", "��", "��" };
        newWaveTile.text = "-�� "+ numbers[waveNumber - 1]+" ��-";
        newWaveEnemyCount.text ="��������:"+Spawn.waves[waveNumber-1].enemyCount;
        if(waveNumber ==5)
            newWaveEnemyCount.text = "��������:����";
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
        print(endDelayTime+"ʱ��");
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
        
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Begin Scene");
    }
}
