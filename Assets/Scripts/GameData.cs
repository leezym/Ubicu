using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class GameData : MonoBehaviour
{
    public static GameData Instance {get; private set;}
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

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        //PlayerPrefs.DeleteAll(); //test
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if(PlayerPrefs.GetString("currentExerciseDate") == "") // fecha actual
            PlayerPrefs.SetString("currentExerciseDate", DateTime.Today.ToString("dd/MM/yyyy"));
        
        if(PlayerPrefs.GetString("idItemFondosArray") == "")
            PlayerPrefs.SetString("idItemFondosArray", string.Join(",", "0,-1,-1,-1,-1")); // 0 es default abstract
        
        if(PlayerPrefs.GetString("idItemFigurasArray") == "")
            PlayerPrefs.SetString("idItemFigurasArray", string.Join(",", "0,-1,-1,-1,-1")); // 0 es default abstract
        
        if(PlayerPrefs.GetString("allFondosItemsArray") == "")
        {
            string s = "";
            for(int i = 0; i < 5; i++)
            {
                if(i == 0)
                    s += string.Join(",", "1,1,1")+";"; // 1 activado
                else
                    s += string.Join(",", "0,0,0")+";"; // 0 desactivado
            }
            PlayerPrefs.SetString("allFondosItemsArray", s);
        }

        if(PlayerPrefs.GetString("allFigurasItemsArray") == "")
        {
            string s = "";
            for(int i = 0; i < 5; i++)
            {
                if(i == 0)
                    s += string.Join(",", "1,1,1")+";"; // 1 activado
                else
                    s += string.Join(",", "0,0,0")+";"; // 0 desactivado
            }
            PlayerPrefs.SetString("allFigurasItemsArray", s);
        }

        if(PlayerPrefs.GetString("allBadgesArray") == "")
        {
            string s = "";
            for(int i = 0; i < 4; i++)
            {
                s += string.Join(",", "0,0,0,0,0,0,0")+";"; // 0 desactivado
            }
            PlayerPrefs.SetString("allBadgesArray", s);
        }
    }

    void Start()
    {
        //scriptsGroup.customizationManager.LoadCustomization();
        //scriptsGroup.rewardsManager.LoadReward();
        idListHourExercises = -1;
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
            GameData.Instance.scriptsGroup.playerMovement.Movement();
            StartCoroutine(scriptsGroup.obstacles.ObstaclesCounter());
            // contador de inactividad
            scriptsGroup.obstacles.DetectInactivity();
        }//else
            //ya se acaben las series de la sesion

        if(resting)
        {
            scriptsGroup.playerMovement.RestingPlayer();
        }
        
        // seleccionar sesion disponible
        if(sessionMenu.gameObject.GetComponent<CanvasGroup>().alpha != 0)
        {
            for(int i = 0; i < exerciseHourArray.Length; i++)
            {
                // detectar cual ejercicio se debe activar
                if(DateTime.Now.Hour == exerciseHourArray[i]
                    && DateTime.Now.Minute <= scriptsGroup.exercisesManager.extraMinuteToWaitForExercise)
                {
                    scriptsGroup.exercisesManager.sesionesList[i].GetComponent<Button>().interactable = true;
                    scriptsGroup.exercisesManager.sesionesList[i].GetComponent<Image>().sprite = scriptsGroup.exercisesManager.currentSessionSprite;
                    // almacenar el id del ejercicio activado
                    idListHourExercises = i;
                }
                
                if ((exerciseHourArray[i] < DateTime.Now.Hour) || (DateTime.Now.Hour == exerciseHourArray[i] && DateTime.Now.Minute > scriptsGroup.exercisesManager.extraMinuteToWaitForExercise))
                {
                    scriptsGroup.exercisesManager.sesionesList[i].GetComponent<Button>().interactable = false;
                    
                    // pregunta si ya finalizó los ejercicios pasados
                    if (exerciseHourArray[i] == 0)
                        scriptsGroup.exercisesManager.sesionesList[i].GetComponent<Image>().sprite = scriptsGroup.exercisesManager.finishedSessionSprite;
                    // pregunta si esta disponible los ejercicios pasados y coloca que no se finalizó
                    else
                        scriptsGroup.exercisesManager.sesionesList[i].GetComponent<Image>().sprite = scriptsGroup.exercisesManager.notFinishedSessionSprite;
                }         
            }
        }

        // detectar cuando lanzar sonido de motivacion
        if(exerciseMenu_Game.gameObject.GetComponent<CanvasGroup>().alpha != 0){
            if(inspiration && GameData.Instance.scriptsGroup.bluetoothPairing.prom > 200)
                scriptsGroup.soundsManager.PlayRandomSound();
            scriptsGroup.soundsManager.StopRandomSound();
        }
    }

    public void SaveLocalData()
    {
        scriptsGroup.rewardsManager.SaveReward();
        scriptsGroup.exercisesManager.SaveExercise();
        scriptsGroup.customizationManager.SaveCustomization();
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        SaveLocalData();
    }
}
