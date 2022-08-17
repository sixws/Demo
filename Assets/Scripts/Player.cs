using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerController))]
public class Player : LivingEntilty
{
    public  float speed =1000f;
    Vector3 moveInput;
    Vector3 moveeVelocity;
    PlayerController controller;
    GunController gunController;
    Plane plane;
    public Crosshairs crosshairs;
   protected override  void Start()
    {
        base.Start();
    }
    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        FindObjectOfType<Spawn>().OnNewWave += OnNewWave;
    }
    void Update()
    {
        if (transform.position.y < -5)
        {
            Die();
        }
        #region 玩家移動
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveeVelocity = moveInput * speed;
        controller.Move(moveeVelocity);
        #endregion
        #region 玩家跟隨鼠標方向
        if (!GameUI.stop)
        {
            plane = new Plane(Vector3.up, Vector3.zero * gunController.GunHeight);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            if (plane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                controller.LookAt(point);
                Debug.DrawLine(ray.origin, point, Color.red);
                crosshairs.transform.position = point;
            }
            crosshairs.DetectTargets(ray);
            #endregion
            #region 开火
            if (Input.GetMouseButton(0))
            {
                gunController.OnTriggerHold();
            }
            if (Input.GetMouseButtonUp(0))
                gunController.OnTriggerReleas();
            if (Input.GetKeyDown(KeyCode.R))
                gunController.Reload();
            #endregion
        }

    }
    void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();
    }

    public override void TashDamage(float damage)
    {
        base.TashDamage(damage);
    }
}
