using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    public float dangerTime;
    public float safeTime;
    public bool rainStart;
    public Light directionalLight;
    // Start is called before the first frame update
    void Start()
    {
        dangerTime = 0;
        safeTime = 30;
        rainStart = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!rainStart)
        {
            directionalLight.color = Color.yellow;
            if (safeTime > 0)
            {
                safeTime -= Time.deltaTime;
            }
            else
            {
                rainStart = true;
                dangerTime = Random.Range(10, 20);
            }
        }
        else
        {
            directionalLight.color = Color.blue;
            if (dangerTime > 0)
            {
                dangerTime -= Time.deltaTime;
            }
            else
            {
                rainStart = false;
                safeTime = Random.Range(20, 30);
            }
        }
    }
}
