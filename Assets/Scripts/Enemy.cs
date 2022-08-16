using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntilty
{
    public enum State
    {
        Idle,//����
        Chasing,//׷��
        Attack//����
    }
    public GameObject eff;
    NavMeshAgent agent;
    Transform target;
    float attackDistanceThreshold =0.5f;//����������ֵ
    float timeBetweenAttacks =1f; //����ʱ����
    float nextAttackTime;//����ʱ�����м���
    State currentState;
    float myCollisonRadius;//������ײ���뾶
    float targetCollisonRadius;//�����ײ���뾶
    Material material;
    Color color;
    bool hasTarget ;
    LivingEntilty targetEnity;
    float damage =1;
    public static event System.Action OnDeathStatic;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
       
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;        
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEnity = target.GetComponent<LivingEntilty>();
            myCollisonRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisonRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }
    protected override void Start()
    {
        base.Start();
        if (hasTarget)
        {
            currentState = State.Chasing;
            targetEnity.OnDath += OnTargetDath;
            StartCoroutine(UpdatePath(0.25f, new Vector3()));
        }
      
    }

    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float distance = (target.position - transform.position).sqrMagnitude;
                if (distance < Mathf.Pow(attackDistanceThreshold + myCollisonRadius + targetCollisonRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    AudioManager.instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
    }
    public override void TaskHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact", transform.position);
        if (damage >= health)
        {
            if (OnDeathStatic != null)
            {
                OnDeathStatic();
            }
            AudioManager.instance.PlaySound("Enemy Death", transform.position);
            GameObject obj = Instantiate(eff, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection));
            obj.GetComponent<Renderer>().material.color = material.color;
            Destroy(obj, 2);
           
        }
        base.TaskHit(damage, hitPoint, hitDirection);
    }
    #region ��������
    IEnumerator Attack()
    {
        material.color = Color.red;
        currentState =State.Attack;
        agent.enabled = false;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 originalPosition = transform.position;
        Vector3 attackPosition = target.position - dirToTarget * (targetCollisonRadius);
        float percent = 0;
        float attakcSpeed = 3;
        bool hasAppliedDamage =false;//�˺��жϱ����ظ��յ��˺�
        while (percent <= 1)
        {
            if (percent > 0.5&& !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                if(hasTarget)
                targetEnity.GetComponent<Rigidbody>().AddForce(dirToTarget * 15f, ForceMode.Impulse);
                targetEnity.TashDamage(damage);
            }
            percent += Time.deltaTime * attakcSpeed;
            float t = 4 * (-Mathf.Pow(percent, 2) + percent);//��������ᵱpercentֵ��0~1֮���ʱ���ʱ������ֵ���0~1�ٴ�1~0�����ķ��������ǵ����� ���ǵ������ǵ��˹��������Ȼ���ٻص�ԭ����λ�� �Ͳ�ֵ����������Ƿ�������
            transform.position =Vector3.Lerp(originalPosition, attackPosition, t);          
            yield return null;
        }
        material.color = color;
        currentState = State.Chasing;
        agent.enabled = true;
    }

    internal void SetCharacteristics(float ememyHealth, Color skinColour, int hitsTokillPlayer, float moveSpeed)
    {
        if(hasTarget)
            damage =Mathf.Ceil(targetEnity.startingHealth /hitsTokillPlayer);
        agent.speed = moveSpeed;
        material = GetComponent<Renderer>().material;
        material.color = skinColour;
        color = skinColour;
        startingHealth = ememyHealth;
    }
    #endregion
    #region ����Ѱ·
    IEnumerator UpdatePath(float refreshRate, Vector3 targePositon)
    {
        while (hasTarget)
        {
          if(currentState == State.Chasing)
            {
                Vector3 dirToTarget =(target.position - transform.position).normalized;
                targePositon =target.position- dirToTarget*(targetCollisonRadius+ myCollisonRadius+ attackDistanceThreshold/2);
                if (!dead)
                    agent.SetDestination(targePositon);
            }
            yield return new WaitForSeconds(refreshRate);
        }

    }
    #endregion
    #region Ŀ����������
   void OnTargetDath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }
    #endregion
}
