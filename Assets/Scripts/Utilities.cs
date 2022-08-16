using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities 
{
    public static T[] ShuffleArray<T>(T[] array,int seed)
    {
        System.Random r = new System.Random(seed);
        for (int i = 0; i < array.Length; i++)
        {
             int index = r.Next(i,array.Length);
             T temp = array[index];
             array[index] = array[i];
             array[i] = temp;
        }
        return array;
    }
}
