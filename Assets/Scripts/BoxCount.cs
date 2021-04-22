using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCount : MonoBehaviour
{

    public GameObject player;
    private TMPro.TextMeshProUGUI boxText;
    // Start is called before the first frame update
    void Start()
    {
        boxText = GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        boxText.text = "X " + player.GetComponent<Player>().counter;
    }
}
