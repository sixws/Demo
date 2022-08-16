using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform[] projectileSpawn;
    public enum FireMode {Auto,Burts,Single }
    public FireMode fireMode;
    public Projectile projectile;
    public float muzzkeVelocity = 35;
    public float msBetwenShots = 100;
    float nnextShotTime;
    public Transform shell;
    public Transform shellEjectio;
    MuzzleFlash muzzleFlash;
    bool Single;
    int Burts;
    public int BurtsCount;
    public Vector2 kickMinMax = new Vector2(0.05f, 0.6f);
    public Vector2 recoilAngleMinMax = new Vector2(10, 40);
    Vector3 recoilSmoothDampVeloccity;
    float recoilAngle;
    float speed;
    public  float recoilRotationSettleTime=.1f;
    public float recoilMoveSettleTime =.1f;
    int projectilesRemainingInMag;
    public int projectilesPerMag;
    bool isReloading;
    public float reloadTime = .3f;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;
    void Start()
    {
        projectilesRemainingInMag = projectilesPerMag;
        muzzleFlash =GetComponent<MuzzleFlash>();
        Burts = BurtsCount;
    }
    void Update()
    {
         transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVeloccity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDampAngle(recoilAngle, 0, ref speed, recoilRotationSettleTime);
        transform.localEulerAngles = new Vector3(recoilAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
        if(!isReloading && projectilesRemainingInMag <= 0)
        {
            Reload();
        }
    }
     void Shoot()
    {
        if(Time.time> nnextShotTime && !isReloading)
        {
            switch (fireMode)
            {
                case FireMode.Auto:
                    break;
                case FireMode.Burts:
                    if (Burts == 0)
                        return;
                    Burts--;
                    break;
                case FireMode.Single:
                    if (Single)
                        return;
                    break;
            }
            nnextShotTime = Time.time+ msBetwenShots/1000; //下一次开火的时间 1+0.1
            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation);
                newProjectile.SetSpeed(muzzkeVelocity);
                projectilesRemainingInMag--;
                if (projectilesPerMag == 0)
                    break;
            }
            Instantiate(shell, shellEjectio.position, shellEjectio.rotation);
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle -= Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            AudioManager.instance.PlaySound(shootAudio, transform.position);
        }
    }
    public void OnTriggerHold()
    {
        Shoot();
        Single = true;
    }
    public void OnTriggerReleas()
    {
        Single = false;
        Burts = BurtsCount;
    }
    public void Reload()
    {
        if (!isReloading && projectilesRemainingInMag != projectilesPerMag)
            StartCoroutine("AnimatReload");
      
    }
    public IEnumerator AnimatReload()
    {
        AudioManager.instance.PlaySound(reloadAudio, transform.position);
        isReloading = true;
        yield return new WaitForSeconds(.2f);
        float reeloadSpeed = 1f / reloadTime;
        float percent = 0;
        while (percent <= 1)
        {
            float maxReloadAngle = -30;
            percent += Time.deltaTime*reeloadSpeed;
            float t = 4 * (-Mathf.Pow(percent, 2) + percent);
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, t);
            transform.localEulerAngles = new Vector3(reloadAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
            yield return null;  
        }
        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
    }
}
