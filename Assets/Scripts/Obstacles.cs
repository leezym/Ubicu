using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Obstacles : MonoBehaviour
{
    public static int INACTIVITY = 60 * 2; //60 segs x 2 min

    [Header("ATTACHED")]
    public bool enabledCounter = true;
    public GameObject limit;
    public TMP_Text serieGameText;
    public TMP_Text repGameText;

    [Header("IN GAME")]
    public int repCounter;
    public float inactivityCounter;

    public IEnumerator ObstaclesCounter()
    {
        //repGameText.text = "REPETICIÓN\n"+repCounter.ToString()+"/"+GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].repeticiones;
        
        if (!GameData.Instance.scriptsGroup.playerMovement.apneaBool) 
        {
            enabledCounter = true;
            // VERIFICA QUE SE HAYAN ACABADO LAS REPETICIONES
            if(repCounter == GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].repeticiones)
            {
                GameData.Instance.playing = false;
                GameData.Instance.scriptsGroup.bluetoothPairing.timer = 0;
                GameData.Instance.scriptsGroup.playerMovement.CreateGraph();
                GameData.Instance.scriptsGroup.bluetoothPairing.StopOutputTime();
                yield return new WaitForSeconds(1.5f);                
                inactivityCounter = 0;
                repCounter = 0;
                UI_System.Instance.SwitchScreens(GameData.Instance.serieGraphMenu);
                GameData.Instance.resting = true;
                StopCoroutine(ObstaclesCounter());
            }
            else
            {          
                StartCoroutine(GameData.Instance.scriptsGroup.soundsManager.PlaySignalSound());
            }
        }
        else
        {
            // CUENTA LAS REPETICIONES
            if(enabledCounter && repCounter < GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].repeticiones)
            {
                enabledCounter = false;
                StartCoroutine(GameData.Instance.scriptsGroup.playerMovement.StartApnea());
                GameData.Instance.scriptsGroup.playerMovement.SaveGraphData();
                repCounter ++;
                inactivityCounter = 0;
            }
        }
    }

    public void DetectInactivity()
    {
        if(inactivityCounter <= INACTIVITY)
            inactivityCounter += Time.deltaTime;
        else
        {
            inactivityCounter = 0;
            NotificationsManager.Instance.WarningNotifications("¡Te has desconectado por inactividad!\nPor favor empieza tu terapia nuevamente");
            ExitGame();
        }
    }
    
    public void ExitGame()
    {
        GameData.Instance.playing = false;
        repCounter = 0;
        GameData.Instance.exerciseSeries = new List<ExerciseData>();
        GameData.Instance.scriptsGroup.playerMovement.seriesCount = 0;
        //GameData.Instance.scriptsGroup.bluetoothPairing.StopOutputTime();
        UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);
    }
}
