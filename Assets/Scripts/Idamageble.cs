using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Idamageble 
{
    public void TaskHit(float damage, Vector3 hitPoint, Vector3 hitDirection);
    public void TashDamage(float damage);
}
