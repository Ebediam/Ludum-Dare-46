using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public delegate void RestartDelegate();
    public static RestartDelegate RestartEvent;

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
        RestartEvent?.Invoke();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex+1, LoadSceneMode.Single);
    }

    public static void RestartGame()
    {
        RestartEvent?.Invoke();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        
    }
}
