using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScript : MonoBehaviour
{
    public GameObject weatherSystem;
    private TMPro.TextMeshProUGUI timeText;
    // Start is called before the first frame update
    void Start()
    {
        timeText = GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(weatherSystem.GetComponent<WeatherSystem>().rainStart)
        {
            timeText.color = Color.red;
            timeText.text = "Danger Time : " + (int)weatherSystem.GetComponent<WeatherSystem>().dangerTime + "s";
        }
        else
        {
            timeText.color = Color.green;
            timeText.text = "Safe Time : " + (int)weatherSystem.GetComponent<WeatherSystem>().safeTime + "s";
        }
    }
}
