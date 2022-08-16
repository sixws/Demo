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
    Wave currentWave;//当前波
    int curentWaveNumber;//当前波数 ，用于记录当前是第几波敌人用于更新波数
    int enemiesRemainingToSpawn;//敌人当前波数，未生成敌人数量
    float nextSpawnTime;//当前波数内下一个敌人生成的时间间隔
    int enemiesRemainingAlive;//当前波数敌人剩余数量
    float tmeBetweenCampingChecks = 2; //玩家在一个待的时间
    float nextCampCheckTime; //记录时间
    float campThresholdDistance = 1.5f;//距离 
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
        float spawnDelay = 1;//生成时间
        float spawnTimer = 0;//用于记录时间
        float tileFlashSpeed = 4;//瓦片闪烁sudu
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
    #region 波数类
    [Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;//敌人数量
        public float timeBetweeSpaans;//敌人生成间隔
        public float moveSpeed;//每波敌人的移动速度
        public int hitsTokillPlayer;//攻击玩家的伤害
        public float ememyHealth;//血量
        public Color skinColour;//敌人颜色
    }
    #endregion
}
