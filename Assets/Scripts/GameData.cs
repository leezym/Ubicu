using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using TMPro;

public class GameData : MonoBehaviour
{
    public static GameData Instance {get; private set;}
    public static string URL = "https://server.ubicu.co/";
    public ScriptsGroup scriptsGroup;
    public UI_Screen dataMenu;

    public Button startButton;
    public Button stopButton;

    public TMP_Text dataText;
    public ScrollRect scrollRectData;


    public ExerciseData m_exerciseSeries;
    public ExerciseData exerciseSeries
    {
        get { return m_exerciseSeries; }
        set { m_exerciseSeries = value; }
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        //PlayerPrefs.DeleteAll(); //test
        Screen.sleepTimeout = SleepTimeout.NeverSleep;     
    }

    void Start()
    {
        startButton.interactable = true;
        stopButton.interactable = false;
    }

    public void StartToAdd(float flow, float time)
    {
        dataText.text += flow.ToString()+"\n";
        scrollRectData.verticalNormalizedPosition = 0;
        exerciseSeries.flujo.Add(flow);
        exerciseSeries.tiempo.Add(time);
    }

    public void StopToRead()
    {
        scriptsGroup.bluetoothPairing.StopOutputTime();
        StartCoroutine(scriptsGroup.calibrationsManager.SendCalibrations());

    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitApp();
    }

    public void ExitApp()
    {
        // notificacion de salir
        NotificationsManager.Instance.QuestionNotifications("Quieres salir de la aplicación?");
        // si
        NotificationsManager.Instance.SetYesButton(()=>{
            //PlayerPrefs.Save();
            Application.Quit();
        });
    }

    void OnApplicationQuit()
    {
        ExitApp();
    }
}
