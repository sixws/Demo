using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public bool devMode;
    LivingEntilty player;
    public Transform playerT;
    public MapGenerator map;
    public Enemy enemy;
    public  Wave[] waves;
    Wave currentWave;//��ǰ��
    int curentWaveNumber;//��ǰ���� �����ڼ�¼��ǰ�ǵڼ����������ڸ��²���
    int enemiesRemainingToSpawn;//���˵�ǰ������δ���ɵ�������
    float nextSpawnTime;//��ǰ��������һ���������ɵ�ʱ����
    int enemiesRemainingAlive;//��ǰ��������ʣ������
    float tmeBetweenCampingChecks = 2; //�����һ������ʱ��
    float nextCampCheckTime; //��¼ʱ��
    float campThresholdDistance = 1.5f;//���� 
    Vector3 campPositiionOld;
    bool isCamping;
    bool isDisabled;
    public System.Action<int> OnNewWave;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerT = player.transform;
        player.OnDath += OnPlayerDath;
        campPositiionOld =playerT.position;
        NextWave();
    }
    void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + tmeBetweenCampingChecks;
                isCamping = (playerT.position - campPositiionOld).sqrMagnitude <= Mathf.Pow(campThresholdDistance, 2);
                campPositiionOld = playerT.position;
            }
            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweeSpaans;
                StartCoroutine("SpawnEnemy");
            }
        }
        if (devMode)
        {
           if (Input.GetKeyDown(KeyCode.Space))
           {
                StopCoroutine("SpawnEnemy");
                foreach (Enemy item in FindObjectsOfType<Enemy>())
                {
                    Destroy(item.gameObject);
                }
                NextWave();
            }
        }
    }
    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFornPositinon(Vector3.zero).position + Vector3.up * 3;
    }
    IEnumerator SpawnEnemy()
    {
        Transform tileTranform =map.GetReandomTransform();
        if (isCamping)
            tileTranform = map.GetTileFornPositinon(playerT.position);
        Material tileMat = tileTranform.GetComponent<Renderer>().material;
        float spawnDelay = 1;//����ʱ��
        float spawnTimer = 0;//���ڼ�¼ʱ��
        float tileFlashSpeed = 4;//��Ƭ��˸sudu
        Color initalColor = Color.white;
        Color flashColor = Color.red;
        while (spawnTimer< spawnDelay)
        {
            spawnTimer+=Time.deltaTime;
            tileMat.color = Color.Lerp(initalColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            yield return null;
        }
        Enemy spawnEnemy = Instantiate(enemy, tileTranform.position+Vector3.up, Quaternion.identity);
        spawnEnemy.SetCharacteristics(currentWave.ememyHealth, currentWave.skinColour,currentWave.hitsTokillPlayer, currentWave.moveSpeed);
        spawnEnemy.OnDath += OnEnemyDath;
    }
    void NextWave()
    {
      
        curentWaveNumber++;
        if(curentWaveNumber-1<waves.Length)
        {
            if (curentWaveNumber > 1)
                AudioManager.instance.PlaySound2D("Level Completed");
            currentWave = waves[curentWaveNumber - 1];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            if (OnNewWave != null)
            {
                OnNewWave(curentWaveNumber);
            }
            ResetPlayerPosition();
        }
    }
    void OnEnemyDath()
    {
        enemiesRemainingAlive--;
        if(enemiesRemainingAlive <= 0)
        {
            Invoke("NextWave", 2f);
        }
    }
    void OnPlayerDath()=>isDisabled = true;
    #region ������
    [Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;//��������
        public float timeBetweeSpaans;//�������ɼ��
        public float moveSpeed;//ÿ�����˵��ƶ��ٶ�
        public int hitsTokillPlayer;//������ҵ��˺�
        public float ememyHealth;//Ѫ��
        public Color skinColour;//������ɫ
    }
    #endregion
}
