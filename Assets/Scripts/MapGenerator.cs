using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;
    public Transform obsGameobject;
    public float tileSize =1;
    public Transform mapFloor;
    public Transform NavmeshFloor;
    public Transform mapPoint;
    public Transform tilePrefabs;
    public Transform obsPrefabs;
    List<Coord> coordList;
    [Range(0, 1)]
    public float outinePercent;
    Queue<Coord> coordQueue;
    public Vector2 mapMaxSiez;
    Map currentMap;
    Transform[,] tileMap;
    Queue<Coord> queueTileMap;
    void OnNewWave(int WaveNumber)
    {
        mapIndex = WaveNumber-1;
        GenerateMap();
    }
    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        currentMap.seed = UnityEngine.Random.Range(0, 9999);
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random prng = new System.Random(currentMap.seed);
      
        coordList = new List<Coord>();
        List<Coord> tileMapList = new List<Coord>();
        if (mapPoint != null)
        {
            DestroyImmediate(mapPoint.gameObject);
            mapPoint = new GameObject().transform;
            mapPoint.parent = transform;
        }//每帧删除
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Transform newTile = Instantiate(tilePrefabs);
                newTile.position = new Vector3(-currentMap.mapSize.x / 2f + x + 0.5f, 0, -currentMap.mapSize.y / 2f + y + 0.5f)* tileSize;
                newTile.localScale *= outinePercent * tileSize;
                newTile.parent = mapPoint;
                coordList.Add(new Coord(x, y));//把每个瓦片的位置信息传入list中用于随机生成障碍物
                tileMapList.Add(new Coord(x, y));
                tileMap[x, y] = newTile.transform;
            }
        }//生成瓦片
        int currentObstacleCount = 0;

        coordQueue = new Queue<Coord>(Utilities.ShuffleArray<Coord>(coordList.ToArray(), currentMap.seed));//得到一个打乱的队列 
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);//障碍物数量
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];//申明一个二维数组用于记录哪里是可通行区域
            for (int i = 0; i < obstacleCount; i++)
            {
                Coord randomCoord = GetRandomCoord();//取出队列中的元素
                currentObstacleCount++;//真实障碍物数量++
                obstacleMap[randomCoord.x, randomCoord.y] = true;//先假设这个点可以创建障碍物
                if (randomCoord != currentMap.mapCentre && MapIsFullyAccessble(obstacleMap, currentObstacleCount))
                {
                    float Height  =Mathf.Lerp(currentMap.minObstacleHeight,currentMap.maxObstacleHeight,(float)prng.NextDouble());
                     Transform newObs = Instantiate(obsPrefabs); //实例化障碍物
                    newObs.position = new Vector3((-currentMap.mapSize.x / 2f + randomCoord.x + 0.5f) * tileSize, Height/2, (-currentMap.mapSize.y / 2f + randomCoord.y + 0.5f) * tileSize);//设置障碍物位置
                    newObs.localScale *= outinePercent* tileSize;
                    newObs.localScale = new Vector3(newObs.localScale.x, Height, newObs.localScale.y);
                    newObs.parent = mapPoint;

                    Renderer obstacleRenderer =newObs.GetComponent<Renderer>();
                    Material obstacleMateriail =new Material(obstacleRenderer.sharedMaterial);
                    float colourPercent = randomCoord.y / (float)currentMap.mapSize.y;
                     obstacleMateriail.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent);
                     obstacleRenderer.sharedMaterial = obstacleMateriail;
                     tileMapList.Remove(randomCoord);
                }
                else
                {
                   currentObstacleCount--;
                   obstacleMap[randomCoord.x, randomCoord.y] = false;
                }
            }
        queueTileMap = new Queue<Coord>(Utilities.ShuffleArray<Coord>(tileMapList.ToArray(), currentMap.seed));
        NavmeshFloor.localScale = new Vector3(mapMaxSiez.x, mapMaxSiez.y, 1) * tileSize;
        mapFloor.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize, 0.1f);
        Transform navMeshObsForward = Instantiate(obsGameobject, Vector3.forward * ((mapMaxSiez.y + currentMap.mapSize.y) / 4f) * tileSize, Quaternion.identity);
        navMeshObsForward.localScale = new Vector3(currentMap.mapSize.x, 1, mapMaxSiez.y / 2f - currentMap.mapSize.y / 2f) * tileSize;
        navMeshObsForward.parent = mapPoint;
        Transform navMeshObsBack = Instantiate(obsGameobject, Vector3.back * ((mapMaxSiez.y + currentMap.mapSize.y) / 4f) * tileSize, Quaternion.identity);
        navMeshObsBack.localScale = new Vector3(currentMap.mapSize.x, 1, mapMaxSiez.y / 2f - currentMap.mapSize.y / 2f) * tileSize;
        navMeshObsBack.parent = mapPoint;
        Transform navMeshObsLeft = Instantiate(obsGameobject, Vector3.left * ((mapMaxSiez.x + currentMap.mapSize.x) / 4f) * tileSize, Quaternion.identity);
        navMeshObsLeft.localScale = new Vector3(mapMaxSiez.x / 2f - currentMap.mapSize.x / 2f, 1,currentMap.mapSize.y) * tileSize;
        navMeshObsLeft.parent = mapPoint;
        Transform navMeshObsRight = Instantiate(obsGameobject, Vector3.right * ((mapMaxSiez.x + currentMap.mapSize.x) / 4f) * tileSize, Quaternion.identity);
        navMeshObsRight.localScale = new Vector3(mapMaxSiez.x / 2f - currentMap.mapSize.x / 2f, 1, currentMap.mapSize.y) * tileSize;
        navMeshObsRight.parent = mapPoint;

    }
    Coord GetRandomCoord()
    {
        Coord randomCoord =coordQueue.Dequeue();
        coordQueue.Enqueue(randomCoord);
        return randomCoord;
    }
    public Transform GetReandomTransform()
    {
        Coord randomCoord = queueTileMap.Dequeue();
        queueTileMap.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }
    public Transform GetTileFornPositinon(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1)/ 2f);
        x = Mathf.Clamp(x, 0, currentMap.mapSize.x - 1);
        y = Mathf.Clamp(y, 0, currentMap.mapSize.y - 1);
        return tileMap[x, y];
    }
    bool MapIsFullyAccessble(bool[,] obstaqleMap, int currentObstacleCount)
    {
        int accessibleTileCount = 1;
        Queue <Coord> ququ = new Queue<Coord> ();//用一个队列来记录遍历所有瓦片
        bool[,] mapLogs = new bool[obstaqleMap.GetLength(0), obstaqleMap.GetLength(1)];//一个用于标记是否已经检测过的bool数组
        mapLogs[currentMap.mapCentre.x, currentMap.mapCentre.y] = true;//直接把中心点设置为true因为中心点已经检测过了 并且肯定不能生成障碍物
        ququ.Enqueue(currentMap.mapCentre);
        while (ququ.Count>0)//一个while循环用于遍历队列中的元素
        {
           Coord tile = ququ.Dequeue();
            for (int x = -1; x <=1; x++)///用两个for循环遍历元素周围的四个其他元素
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x+x;  //这样写还只是检测周围八个元素 对角线的也检测了我们需要再加个条件 只检测
                    int neighbourY = tile.y+y;
                    if (x == 0 || y == 0)//这样就能只检测周围上下左右四个元素不明白就自己画个图
                    {
                        if(neighbourX>=0 && neighbourX<obstaqleMap.GetLength(0)&&neighbourY>=0 && neighbourY < obstaqleMap.GetLength(1))//这个是地图的边界检测 只能在地图里面生成障碍物
                        {
                            if (!mapLogs[neighbourX, neighbourY] && !obstaqleMap[neighbourX,neighbourY])//这个判断是判断是否这个瓦片是否已经被检测过了，如果检测过了就不用在检测了
                            {
                                ququ.Enqueue(new Coord(neighbourX, neighbourY));
                                mapLogs[neighbourX, neighbourY] = true;//把瓦片设置为已经检测过了
                                accessibleTileCount++; 
                            }
                        }
                    }
                }
            }
        }
        int s = (int)(currentMap.mapSize.x * currentMap.mapSize.y) -currentObstacleCount;

        return s == accessibleTileCount;
    }
    private void Awake()
    {
        FindObjectOfType<Spawn>().OnNewWave += OnNewWave;
    }
    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;
        public Coord(int x,int y)
        {
            this.x = x;
            this.y = y;
        }
        public static bool operator !=(Coord coord, Coord coord1)
        {
           return !(coord == coord1);
           
        }
        public static bool operator ==(Coord coord, Coord coord1)
        {     
         return coord.x == coord1.x && coord.y == coord1.y;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        [Range(0,1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight, maxObstacleHeight;
        public Color foregroundColour, backgroundColour;
        public Coord mapCentre
        {
            get => new Coord(mapSize.x / 2, mapSize.y / 2); 
        }
    }

}