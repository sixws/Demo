using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    public SpriteRenderer dot;
    public Color dotHighlinghtColour;
    Color originalDotColour;
    public LayerMask targetMask;
    void Start()
    {
        Cursor.visible = false;
        originalDotColour = dot.color;
    }
    void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(1, Vector3.forward);
    }
   public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighlinghtColour;
        }
        else
        {
            dot.color = originalDotColour;
        }
    }
}
