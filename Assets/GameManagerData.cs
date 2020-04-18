using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameManagerData : ScriptableObject
{

    public List<HighscoreData> highscoreDatas;

    public bool checkPointUsed;
}
