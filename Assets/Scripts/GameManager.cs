using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public delegate void GameManagerDelegate();
    public static GameManagerDelegate RestartEvent;
    public static GameManagerDelegate NextLevelEvent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void NextLevel()
    {
        
        NextLevelEvent?.Invoke();

        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex+1, LoadSceneMode.Single);
    }

    public static void RestartGame()
    {
        RestartEvent?.Invoke();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        
    }
}
