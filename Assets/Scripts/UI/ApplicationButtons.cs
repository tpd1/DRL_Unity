using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationButtons : MonoBehaviour
{

    public static void ExitGame()
    {
        Application.Quit();
    }
    
    public static void PauseGame()
    {
        Time.timeScale = 0;
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1;
    }
    
}
