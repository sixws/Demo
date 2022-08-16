using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed;
    float moveDistance;
    public LayerMask collisionMask;
    private float damage =1;
    private float skinWadth = 0.1f;
    private void Start()
    {
        Destroy(gameObject,2);
        Collider[] initalCollisions = Physics.OverlapSphere(transform.position, .5f, collisionMask);
        if (initalCollisions.Length > 0)
            OnHitObject(initalCollisions[0],transform.position);
    }
    void Update()
    {
        moveDistance = speed * Time.deltaTime;
        transform.Translate(Vector3.forward * moveDistance);
        CheckColsions(moveDistance);
    }
    public void SetSpeed(float newSpeed)=>speed = newSpeed;
    void CheckColsions(float moveDistance)
    {
        Ray ray =new Ray(transform.position,transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, moveDistance+ skinWadth, collisionMask, QueryTriggerInteraction.Collide))
            OnHitObject(hit.collider,hit.point);
    }
    void OnHitObject(Collider c,Vector3 hitPoint)
    {
        Idamageble idamageble = c.GetComponent<Idamageble>();
        if (idamageble != null)
        {
            idamageble.TaskHit(damage, hitPoint,transform.forward);
        }
        Destroy(gameObject);
    }
}
