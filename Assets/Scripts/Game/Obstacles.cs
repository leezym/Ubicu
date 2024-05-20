using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Obstacles : MonoBehaviour
{
    public static Obstacles Instance {get; private set;}

    public static int INACTIVITY = 60 * 2; //60 segs x 2 min

    [Header("ATTACHED")]
    public bool enabledCounter = true;
    public GameObject limit;
    public TMP_Text serieGameText;
    public TMP_Text repGameText;

    [Header("IN GAME")]
    public int repCounter;
    public float inactivityCounter;

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public IEnumerator ObstaclesCounter()
    {
        //repGameText.text = "REPETICIÓN\n"+repCounter.ToString()+"/"+GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].repeticiones;
        
        if (!PlayerMovement.Instance.apneaBool) 
        {
            enabledCounter = true;
            // VERIFICA QUE SE HAYAN ACABADO LAS REPETICIONES
            if(repCounter == GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].repeticiones)
            {
                GameData.Instance.playing = false;
                BluetoothPairing.Instance.timer = 0;
                PlayerMovement.Instance.CreateGraph();
                BluetoothPairing.Instance.StopOutputTime();
                yield return new WaitForSeconds(1.5f);                
                inactivityCounter = 0;
                repCounter = 0;
                UI_System.Instance.SwitchScreens(ExercisesManager.Instance.serieGraphMenu);
                GameData.Instance.resting = true;
                StopCoroutine(ObstaclesCounter());
            }
            else
            {          
                StartCoroutine(SoundsManager.Instance.PlaySignalSound());
            }
        }
        else
        {
            // CUENTA LAS REPETICIONES
            if(enabledCounter && repCounter < GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].repeticiones)
            {
                enabledCounter = false;
                StartCoroutine(PlayerMovement.Instance.StartApnea());
                PlayerMovement.Instance.SaveGraphData();
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
        PlayerMovement.Instance.seriesCount = 0;
        //BluetoothPairing.Instance.StopOutputTime();
        UI_System.Instance.SwitchScreens(ExercisesManager.Instance.sessionMenu);
    }
}
