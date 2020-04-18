using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{

    public delegate void UIDelegate(Color color);
    public static UIDelegate UIColorChangeEvent;

    public delegate void UITextDelegate(string text);
    public static UITextDelegate DeathTextEvent;
    public static UITextDelegate TimerEvent;
    public static UITextDelegate BestTimerEvent;

    public List<Image> images;

    public TextMeshProUGUI deathText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI bestTimeText;


    // Start is called before the first frame update
    void Start()
    {
        UIColorChangeEvent += ChangeColor;
        GameManager.RestartEvent += OnRestart;
        GameManager.NextLevelEvent += OnRestart;
        DeathTextEvent += UpdateDeathText;
        TimerEvent += UpdateTimer;
        BestTimerEvent += UpdateBestTimer;
    }


    public void ChangeColor(Color color)
    {
        foreach(Image image in images)
        {
            image.color = color;
        }
    }

    public void OnRestart()
    {
        UIColorChangeEvent -= ChangeColor;
        GameManager.RestartEvent -= OnRestart;
        GameManager.NextLevelEvent -= OnRestart;
        DeathTextEvent -= UpdateDeathText;
        TimerEvent -= UpdateTimer;
        BestTimerEvent -= UpdateBestTimer;
    }


    public void UpdateDeathText(string deaths)
    {
        deathText.text = "Deaths: " + deaths;
    }

    public void UpdateTimer(string time)
    {
        timerText.text = "Timer: " + time;
    }

    public void UpdateBestTimer(string time)
    {
        bestTimeText.text = "Best time: " + time;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
