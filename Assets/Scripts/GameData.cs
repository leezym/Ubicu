using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class GameData : MonoBehaviour
{
    public static GameData Instance {get; private set;}
    public static string URL = "https://server.ubicu.co/";
    public ScriptsGroup scriptsGroup;
    public UI_Screen loginMenu;
    public UI_Screen sessionMenu;
    public UI_Screen exerciseMenu_Game;
    public UI_Screen serieGraphMenu;
    public UI_Screen customizeMenu_Select;
    public UI_Screen customizeMenu_Items;
    public UI_Screen badgesMenu;
    public UI_Screen infoBadgesMenu;

    private bool m_playing = false;
    public bool playing
    {
        get { return m_playing; }
        set { m_playing = value; }
    }

    private int m_idJsonObjectExercises;
    public int idJsonObjectExercises
    {
        get { return m_idJsonObjectExercises; }
        set { m_idJsonObjectExercises = value; }
    }

    private bool m_resting = false;
    public bool resting
    {
        get { return m_resting; }
        set { m_resting = value; }
    }

    public bool m_apnea = false;
    public bool apnea
    {
        get { return m_apnea; }
        set { m_apnea = value; }
    }

    public bool m_inspiration = false;
    public bool inspiration
    {
        get { return m_inspiration; }
        set { m_inspiration = value; }
    }

    public Data m_jsonObjectUser;
    public Data jsonObjectUser
    {
        get { return m_jsonObjectUser; }
        set { m_jsonObjectUser = value; }
    }

    public Exercises m_jsonObjectExercises;
    public Exercises jsonObjectExercises
    {
        get { return m_jsonObjectExercises; }
        set { m_jsonObjectExercises = value; }
    }

    public int m_idListHourExercises;
    public int idListHourExercises
    {
        get { return m_idListHourExercises; }
        set { m_idListHourExercises = value; }
    }

    public List<ExerciseData> m_exerciseSeries;
    public List<ExerciseData> exerciseSeries
    {
        get { return m_exerciseSeries; }
        set { m_exerciseSeries = value; }
    }

    public int[] m_exerciseHourArray;
    public int[] exerciseHourArray
    {
        get { return m_exerciseHourArray; }
        set { m_exerciseHourArray = value; }
    }

    public Rewards m_jsonObjectRewards;
    public Rewards jsonObjectRewards
    {
        get { return m_jsonObjectRewards; }
        set { m_jsonObjectRewards = value; }
    }

    public Customizations m_jsonObjectCustomizations;
    public Customizations jsonObjectCustomizations
    {
        get { return m_jsonObjectCustomizations; }
        set { m_jsonObjectCustomizations = value; }
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;     
    }

    void Start()
    {
        idListHourExercises = -1;
        if(PlayerPrefs.GetString("currentExerciseDate") == "") // fecha actual
            PlayerPrefs.SetString("currentExerciseDate", DateTime.Today.ToString("dd/MM/yyyy"));
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            // notificacion de salir
            NotificationsManager.Instance.QuestionNotifications("Quieres salir de la aplicación?");
            // si
            NotificationsManager.Instance.SetYesButton(()=>{
                SaveLocalData();
                Application.Quit();
            });
        }
       
        if(playing)
        {
            //contador de apnea
            if(apnea)
                scriptsGroup.playerMovement.apneaBool = true;
                        
            // contador de inactividad
            scriptsGroup.obstacles.DetectInactivity();

            scriptsGroup.playerMovement.Movement();
            StartCoroutine(scriptsGroup.obstacles.ObstaclesCounter());
        }

        if(resting)
            scriptsGroup.playerMovement.RestingPlayer();
        
        // seleccionar sesion disponible
        if(sessionMenu.gameObject.GetComponent<CanvasGroup>().alpha != 0)
        {
            for(int i = 0; i < exerciseHourArray.Length; i++)
            {
                int horaActual = int.Parse(DateTime.Now.Hour.ToString(CultureInfo.InvariantCulture));
                int minutoActual = int.Parse(DateTime.Now.Minute.ToString(CultureInfo.InvariantCulture));
                
                // detectar cual ejercicio se debe activar
                if(horaActual == exerciseHourArray[i] && minutoActual <= scriptsGroup.exercisesManager.extraMinuteToWaitForExercise)
                {
                    scriptsGroup.exercisesManager.sessionPrefab[i].GetComponent<Button>().interactable = true;
                    scriptsGroup.exercisesManager.sessionPrefab[i].GetComponent<Image>().sprite = scriptsGroup.exercisesManager.currentSessionSprite;
                    // almacenar el id del ejercicio activado
                    idListHourExercises = i;
                }
                
                if ((exerciseHourArray[i] < horaActual) || (horaActual == exerciseHourArray[i] && minutoActual > scriptsGroup.exercisesManager.extraMinuteToWaitForExercise))
                {
                    scriptsGroup.exercisesManager.sessionPrefab[i].GetComponent<Button>().interactable = false;
                    
                    // pregunta si ya finalizó los ejercicios pasados
                    if (exerciseHourArray[i] == 0)
                        scriptsGroup.exercisesManager.sessionPrefab[i].GetComponent<Image>().sprite = scriptsGroup.exercisesManager.finishedSessionSprite;
                    // pregunta si esta disponible los ejercicios pasados y coloca que no se finalizó
                    else
                        scriptsGroup.exercisesManager.sessionPrefab[i].GetComponent<Image>().sprite = scriptsGroup.exercisesManager.notFinishedSessionSprite;
                }         
            }
        }

        // detectar cuando lanzar sonido de motivacion
        if(exerciseMenu_Game.gameObject.GetComponent<CanvasGroup>().alpha != 0)
        {
            scriptsGroup.soundsManager.StopRandomSound();
            scriptsGroup.soundsManager.StopSignalSound();
            
            if(inspiration && !apnea)
                scriptsGroup.soundsManager.PlayRandomSound();
            else if(apnea)
                scriptsGroup.soundsManager.AddSound();

        }
    }

    public void SaveLocalData()
    {
        scriptsGroup.exercisesManager.SaveExercise();
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        SaveLocalData();
    }
}
