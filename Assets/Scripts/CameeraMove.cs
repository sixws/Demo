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
        position = new Vector3[] { new Vector3(0.180000007f,17.7099991f,-7.23000002f),
                                   new Vector3(0.180000007f,23.6599998f,-9.93999958f),
                                   new Vector3(0.180000007f,27.6900005f,-11.21f),
                                   new Vector3(0.180000007f,33.6699982f,-14.1599998f) ,
                                   new Vector3(0.180000007f,33.6699982f,-14.1599998f) }; 
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
