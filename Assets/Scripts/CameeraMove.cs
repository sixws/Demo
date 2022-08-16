using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameeraMove : MonoBehaviour
{
    public MapGenerator mapGenerator;
    Vector3[] position;
    int index;
    // Start is called before the first frame update
    void Start()
    {
        position = new Vector3[] { new Vector3(-0.200000003f, 19.8999996f, -7f),
                                   new Vector3(-0.150000006f,22.6000004f,-8.42000008f),
                                   new  Vector3(-0.150000006f,22.5400009f,-9.06999969f),
                                   new Vector3(0.180000007f,27.0949993f,-12.0909996f) ,
                                   new Vector3(0.180000007f,28.6299992f,-12.5699997f) }; 
        transform.position = position[0];
        index = mapGenerator.mapIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (index != mapGenerator.mapIndex)
        {
            StartCoroutine("CameerMove");
            index = mapGenerator.mapIndex;
        }
    }
    IEnumerator CameerMove()
    {
        float speed =1f;
        float perent=0;
        while (perent<=1)
        {
            perent +=Time.deltaTime*speed;
            transform.position = Vector3.Lerp(new Vector3(0, 9.60000038f, -4.1500001f), position[mapGenerator.mapIndex],perent);
            yield return null;
        }
    }
}
