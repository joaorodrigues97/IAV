using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinTest : MonoBehaviour
{

    float t = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        float h = 0.5f*(Mathf.Cos(2*t) + 1);
        Grapher.Log(h, "Cos", Color.green);
    }
}
