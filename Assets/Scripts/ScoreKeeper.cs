using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
  public int Score { get; private set; }
   [SerializeField] float streakExpiredTime = 2.0f;
    float lastPressTime;
   [SerializeField] int streakCount;
    public Image comboImage;
    public GameObject comboTextGO;
    public Text scoreText;
    private float fromScore;
    private float toScore;
    public  float animationTime;

    private void Start()
    {
        Enemy.OnDeathStatic += KillEnemy;
        FindObjectOfType<Player>().OnDath += PlayerDeath;
        Score = 0;
        streakCount=0;
        comboTextGO.gameObject.SetActive(false);
    }
    private void PlayerDeath()
    {
        Enemy.OnDeathStatic -= KillEnemy;
    }
    private void Update()
    {

    }
    void KillEnemy()
    {
        if (Time.time <=    streakExpiredTime + lastPressTime)
        {
            streakCount++;
        }
        else
        {
            streakCount = 0;
            streakCount++;
            comboImage.fillAmount = 1;
        }
        StartCoroutine("UpdateComboco");
        comboTextGO.gameObject.SetActive(true);
        comboTextGO.GetComponent<Text>().text = "Combo X "+"<color=orange>" +(streakCount*100)+" </color>";
        lastPressTime = Time.time;
        RollingScore();
    }

    private void RollingScore()
    {
        fromScore = Score;  
        toScore = streakCount * 100+ fromScore;
        LeanTween.value(fromScore, toScore, animationTime).setEase(LeanTweenType.easeOutQuart).setOnUpdate((float obj)=>
        {
            fromScore = obj;
            scoreText.text = "Score:" + obj.ToString("000000");
        });
        Score = (int)toScore;
    }
    IEnumerator UpdateComboco()
    {
        float percent = 1;
        int currentStreakCount = streakCount;
        while (percent>0)
        {
            if (currentStreakCount == streakCount)
            {
                percent -= Time.deltaTime / streakExpiredTime;
                comboImage.fillAmount = percent;
            }
            else
            {
                StopCoroutine("UpdateComboco");
                StartCoroutine("UpdateComboco");
            }
            yield return null;
        }
           streakCount = 0;
        comboTextGO.gameObject.SetActive(false);
    }
}
