using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Obstacles : MonoBehaviour
{
    public static int INACTIVITY = 60 * 3; //60 segs x 3 min

    [Header("ATTACHED")]
    public float secondsAferApnea;
    public bool enabledCounter = true;
    public GameObject limit;
    public TMP_Text serieGameText;
    public TMP_Text repGameText;

    [Header("IN GAME")]
    public int repCounter;
    public float inactivityCounter;

    public IEnumerator ObstaclesCounter()
    {
        repGameText.text = "REPETICIÓN\n"+repCounter.ToString()+"/"+GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].repeticiones;
        /*if (!GameData.Instance.apnea)
        {
            enabledCounter = true;
            GameData.Instance.scriptsGroup.playerMovement.StopApnea();
            GameData.Instance.scriptsGroup.playerMovement.Movement();
            if(repCounter == GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].repeticiones)
            {
                GameData.Instance.playing = false;
                GameData.Instance.scriptsGroup.playerMovement.CreateGraph();
                yield return new WaitForSeconds(1.5f);
                inactivityCounter = 0;
                repCounter = 0;
                UI_System.Instance.SwitchScreens(GameData.Instance.serieGraphMenu);
                GameData.Instance.resting = true;
                StopCoroutine(ObstaclesCounter());
            }
        }
        else
        {
            GameData.Instance.scriptsGroup.playerMovement.StartApnea();
            if(enabledCounter && repCounter < GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].repeticiones)
            {
                enabledCounter = false;
                GameData.Instance.scriptsGroup.playerMovement.SaveGraphData();
                repCounter ++;
                inactivityCounter = 0;
            }
        }*/

        enabledCounter = true;
        GameData.Instance.scriptsGroup.playerMovement.Movement();
        if(repCounter == GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].repeticiones)
        {
            GameData.Instance.playing = false;
            GameData.Instance.scriptsGroup.playerMovement.CreateGraph();
            yield return new WaitForSeconds(1.5f);
            inactivityCounter = 0;
            repCounter = 0;
            UI_System.Instance.SwitchScreens(GameData.Instance.serieGraphMenu);
            GameData.Instance.resting = true;
            StopCoroutine(ObstaclesCounter());
        }
        if(enabledCounter && repCounter < GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].repeticiones)
        {
            enabledCounter = false;
            GameData.Instance.scriptsGroup.playerMovement.SaveGraphData();
            repCounter ++;
            inactivityCounter = 0;
        }
        
    }

    public void DetectInactivity()
    {
        if(inactivityCounter <= INACTIVITY)
            inactivityCounter += Time.deltaTime;
        else
        {
            GameData.Instance.playing = false;
            inactivityCounter = 0;
            repCounter = 0;
            UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);
        }
    }
}
