using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void Retry(int levelIndex) 
    {
        SceneManager.LoadScene(levelIndex);
    }
    public void Exit() 
    {
        Application.Quit();
    }
    public void Menu() 
    {
        SceneManager.LoadScene("Menu");
    }
}
