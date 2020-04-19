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
    public static GameManagerDelegate PauseEvent;
    public static GameManagerDelegate ResumeEvent;

    public GameManagerData data;
    public AudioSource music;

    public static bool isPaused = false;

    public HighscoreData levelData;

    public float timer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        Invoke("UpdateUI", 0.2f);

        NextLevelEvent += OnNextLevel;
        RestartEvent += OnRestart;




    }

    public void UpdateUI()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;

        foreach (HighscoreData _levelData in data.highscoreDatas)
        {
            if (_levelData.levelIndex == levelIndex)
            {
                levelData = _levelData;
                break;
            }
        }

        if (!levelData)
        {
            return;
        }


        music.Play();
        

        string stringTimer = FormatedTime(levelData.timeInSeconds);

        UIController.BestTimerEvent?.Invoke(stringTimer);
        UIController.DeathTextEvent?.Invoke(levelData.totalDeaths.ToString());
    }

    public static string FormatedTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time - minutes*60f);
        int miliseconds = Mathf.RoundToInt((time - (minutes * 60f + seconds)) * 100f);
        string niceTime = string.Format("{0:0}:{1:00}:{2:000}", minutes, seconds, miliseconds);

        return niceTime;
    }

    public static void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
        Time.timeScale = 0f;
        PauseEvent?.Invoke();
    }

    public void ResumeGameLocal()
    {
        ResumeGame();
    }

    public static void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
        Time.timeScale = 1f;
        ResumeEvent?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {

        if (isPaused)
        {
            return;
        }

        if (!levelData)
        {
            return;
        }

        timer += Time.deltaTime;

        string stringTimer = FormatedTime(timer);
        UIController.TimerEvent?.Invoke(stringTimer);
    }


    public void OnNextLevel()
    {
        if (levelData)
        {
            if (!data.checkPointUsed)
            {
                if (timer < levelData.timeInSeconds)
                {
                    levelData.timeInSeconds = timer;
                }
            }

        }





        OnRestart();
        data.checkPointUsed = false;
    }

    public void OnRestart()
    {

        if (levelData)
        {
            levelData.totalDeaths++;
        }

        if (PlayerController.local)
        {
            if (PlayerController.local.data.checkPoint)
            {
                data.checkPointUsed = true;
            }
        }



        NextLevelEvent -= OnNextLevel;
        RestartEvent -= OnRestart;
    }

    public void ChooseLevel(int levelIndex)
    {
        Time.timeScale = 1f;
        isPaused = false;
        data.checkPointUsed = true;
        NextLevelEvent?.Invoke();

        SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Single);
    }

    public void NextLevelLocal()
    {
        NextLevel();
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

    public void QuitGameLocal()
    {
        QuitGame();
    }

    public static void QuitGame()
    {
        Application.Quit();
    }


}
