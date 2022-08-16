using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public float forceMin,forceMax;

    public Rigidbody myRigidbody;
    float lifetime = 4;
    float fadetime = 2;
    void Start()
    {
        float force =Random.Range(forceMin,forceMax);
        myRigidbody.AddForce (transform.right*force);
        myRigidbody.AddTorque(Random.insideUnitSphere*force);
        StartCoroutine(Fade());
    }
    void Update()
    {
        
    }
    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifetime);
        float percent = 0;
        float fadeSpeed = 1 / fadetime;
        Material mat = GetComponent<Renderer>().material;
        Color initlalColour = mat.color;
        while (percent<1)
        {
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initlalColour, Color.clear, percent);
            yield return null;
        }
        Destroy(gameObject);
    }
}
