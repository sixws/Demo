using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeathBar : MonoBehaviour
{
    public Image bar;
    public Image barBack;
    private LivingEntilty player;
    float hp;
    bool m =true;
    void Start()
    {
        player =FindObjectOfType<Player>();
        hp = player.health;
    }
    void Update()
    {
        if (player.health != hp &&m)
        {
            StartCoroutine(Hp(player.startingHealth, player.health));
            hp = player.health;
        }        
    }
    IEnumerator Hp(float stratHp,float hp)
    {
        bar.fillAmount = hp / stratHp;
        float speed = 0.5f;
        float percent = 0;
        float s = barBack.fillAmount;
        while (percent<=1)
        {
            if (barBack.fillAmount == bar.fillAmount)
                break;
            percent+=Time.deltaTime* speed;
            barBack.fillAmount=Mathf.Lerp(s,bar.fillAmount, percent);     
            yield return null;
        }
    } 
}
