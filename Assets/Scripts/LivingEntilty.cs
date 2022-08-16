
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntilty : MonoBehaviour,Idamageble
{
    public float health;
    public float startingHealth;
    protected bool dead = false;
    public event Action OnDath;
    public virtual void TaskHit(float damage, Vector3 hitPoint,Vector3 hitDirection)
    {
        TashDamage(damage);
    }

  protected virtual void Start()
    {
        health = startingHealth;
    }
    void Update()
    {
        
    }
  public virtual void Die()
  {
        dead = true;
        OnDath();
        Destroy(gameObject);
  }

    public virtual void TashDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
            Die();
    }
}
