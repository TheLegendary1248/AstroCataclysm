using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Keeps track of score, dps, chain, etc.
/// </summary>
public class GameStatistics : MonoBehaviour
{
    public bool inGame = true;
    static int TotalScore = 0; public static int Score { set { TotalScore += value; } }
    



    public void SetEndStats()//Set the statistics
    {


    }
    public void RetrieveHigh() //Get highscores
    {

    }
}
