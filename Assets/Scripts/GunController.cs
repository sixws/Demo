using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private Gun equippedGun;
    private  Transform weaponHold;
    public Gun[] allGun;
   
    void Start()
    {
        weaponHold= transform.GetChild(0);
    }
    
    public float GunHeight
    {
        get => weaponHold.position.y;
    }
    void Update()
    {
 
    }
     void EquipGun(Gun gunToEquip)
    {
        if (equippedGun != null)
            Destroy(equippedGun.gameObject);
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        equippedGun.transform.parent = weaponHold.transform;
        equippedGun.transform.position = weaponHold.position;
    }
    public void EquipGun(int Index)
    {
        EquipGun(allGun[Index]);
    }
    public void OnTriggerHold()=>equippedGun.OnTriggerHold();
    public void OnTriggerReleas() => equippedGun.OnTriggerReleas();
    public void Reload()
    {
        equippedGun.Reload();
    }
}
