using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void openGame()
    {
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>().StopMusic();
        SceneManager.LoadScene("Game");
    }

    public void InstructionsMenu()
    {
        SceneManager.LoadScene("Instructions");
    }
    public void mainMenu()
    {
        SceneManager.LoadScene("Start");
    }
    public void exitGame()
    {
        Application.Quit();
    }
}
