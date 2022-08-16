using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody rig;
    Vector3 Velocity;
    public Enemy Enemy;
    void Start()
    {
        rig = GetComponent<Rigidbody>();  
    }
    public void Move(Vector3 _Velocity) => Velocity = _Velocity;
    private void FixedUpdate()
    {
        rig.AddForce(Velocity);
        if (Velocity.x == 0)
            rig.velocity = new Vector3(0, rig.velocity.y, rig.velocity.z);
        if(Velocity.z == 0)
            rig.velocity = new Vector3(rig.velocity.x, rig.velocity.y, 0);
        rig.velocity = new Vector3(Mathf.Clamp(rig.velocity.x, -5, 5), rig.velocity.y, Mathf.Clamp(rig.velocity.z, -5, 5));
    }
    public void LookAt(Vector3 point)
    {
        Vector3 vector =new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(vector);
    }
}
