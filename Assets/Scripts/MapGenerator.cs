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
        }//ÿ֡ɾ��
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Transform newTile = Instantiate(tilePrefabs);
                newTile.position = new Vector3(-currentMap.mapSize.x / 2f + x + 0.5f, 0, -currentMap.mapSize.y / 2f + y + 0.5f)* tileSize;
                newTile.localScale *= outinePercent * tileSize;
                newTile.parent = mapPoint;
                coordList.Add(new Coord(x, y));//��ÿ����Ƭ��λ����Ϣ����list��������������ϰ���
                tileMapList.Add(new Coord(x, y));
                tileMap[x, y] = newTile.transform;
            }
        }//������Ƭ
        int currentObstacleCount = 0;

        coordQueue = new Queue<Coord>(Utilities.ShuffleArray<Coord>(coordList.ToArray(), currentMap.seed));//�õ�һ�����ҵĶ��� 
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);//�ϰ�������
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];//����һ����ά�������ڼ�¼�����ǿ�ͨ������
            for (int i = 0; i < obstacleCount; i++)
            {
                Coord randomCoord = GetRandomCoord();//ȡ�������е�Ԫ��
                currentObstacleCount++;//��ʵ�ϰ�������++
                obstacleMap[randomCoord.x, randomCoord.y] = true;//�ȼ����������Դ����ϰ���
                if (randomCoord != currentMap.mapCentre && MapIsFullyAccessble(obstacleMap, currentObstacleCount))
                {
                    float Height  =Mathf.Lerp(currentMap.minObstacleHeight,currentMap.maxObstacleHeight,(float)prng.NextDouble());
                     Transform newObs = Instantiate(obsPrefabs); //ʵ�����ϰ���
                    newObs.position = new Vector3((-currentMap.mapSize.x / 2f + randomCoord.x + 0.5f) * tileSize, Height/2, (-currentMap.mapSize.y / 2f + randomCoord.y + 0.5f) * tileSize);//�����ϰ���λ��
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
        Queue <Coord> ququ = new Queue<Coord> ();//��һ����������¼����������Ƭ
        bool[,] mapLogs = new bool[obstaqleMap.GetLength(0), obstaqleMap.GetLength(1)];//һ�����ڱ���Ƿ��Ѿ�������bool����
        mapLogs[currentMap.mapCentre.x, currentMap.mapCentre.y] = true;//ֱ�Ӱ����ĵ�����Ϊtrue��Ϊ���ĵ��Ѿ������� ���ҿ϶����������ϰ���
        ququ.Enqueue(currentMap.mapCentre);
        while (ququ.Count>0)//һ��whileѭ�����ڱ��������е�Ԫ��
        {
           Coord tile = ququ.Dequeue();
            for (int x = -1; x <=1; x++)///������forѭ������Ԫ����Χ���ĸ�����Ԫ��
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x+x;  //����д��ֻ�Ǽ����Χ�˸�Ԫ�� �Խ��ߵ�Ҳ�����������Ҫ�ټӸ����� ֻ���
                    int neighbourY = tile.y+y;
                    if (x == 0 || y == 0)//��������ֻ�����Χ���������ĸ�Ԫ�ز����׾��Լ�����ͼ
                    {
                        if(neighbourX>=0 && neighbourX<obstaqleMap.GetLength(0)&&neighbourY>=0 && neighbourY < obstaqleMap.GetLength(1))//����ǵ�ͼ�ı߽��� ֻ���ڵ�ͼ���������ϰ���
                        {
                            if (!mapLogs[neighbourX, neighbourY] && !obstaqleMap[neighbourX,neighbourY])//����ж����ж��Ƿ������Ƭ�Ƿ��Ѿ��������ˣ���������˾Ͳ����ڼ����
                            {
                                ququ.Enqueue(new Coord(neighbourX, neighbourY));
                                mapLogs[neighbourX, neighbourY] = true;//����Ƭ����Ϊ�Ѿ�������
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