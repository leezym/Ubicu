using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class GameData : MonoBehaviour
{
    public static GameData Instance{get; private set;}
    public ScriptsGroup scriptsGroup;
    public UI_Screen loginMenu;
    public UI_Screen sessionMenu;
    public UI_Screen exerciseMenu_Game;
    public UI_Screen serieGraphMenu;

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

    private bool m_apnea = false;
    public bool apnea
    {
        get { return m_apnea; }
        set { m_apnea = value; }
    }

    private bool m_inspiration = false;
    public bool inspiration
    {
        get { return m_inspiration; }
        set { m_inspiration = value; }
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

    /*private int m_exerciseDate;
    public int exerciseDate
    {
        get { return m_exerciseDate; }
        set { m_exerciseDate = value; }
    }*/

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        //PlayerPrefs.DeleteAll();

        if(PlayerPrefs.GetString("currentExerciseDate") == "")
            PlayerPrefs.SetString("currentExerciseDate", DateTime.Today.ToString());
        
        if(PlayerPrefs.GetString("idItemFondosArray") == "")
            PlayerPrefs.SetString("idItemFondosArray", string.Join(",", "0,-1,-1,-1,-1")); // 0 es default abstract
        
        if(PlayerPrefs.GetString("idItemFigurasArray") == "")
            PlayerPrefs.SetString("idItemFigurasArray", string.Join(",", "0,-1,-1,-1,-1")); // 0 es default abstract
            

        scriptsGroup.rewardsManager.LoadReward();
        idListHourExercises = -1;
    }
    
    void Update()
    {
        if(playing)
        {
            StartCoroutine(scriptsGroup.obstacles.ObstaclesCounter());
            // contador de inactividad
            scriptsGroup.obstacles.DetectInactivity();
        }
        //else
            //ya se acaben las series de la sesion

        if(resting)
            scriptsGroup.playerMovement.RestingPlayer();

        
        // select available session
        if(sessionMenu.gameObject.activeSelf)
        {
            for(int i = 0; i < scriptsGroup.exercisesManager.exerciseHourArray.Length; i++)
            {
                // detectar que ejercicio se debe activar
                if(DateTime.Now.Hour == scriptsGroup.exercisesManager.exerciseHourArray[i]
                    && DateTime.Now.Minute <= scriptsGroup.exercisesManager.extraHourToWaitForExercise)
                {
                    scriptsGroup.exercisesManager.sesionesList[i].GetComponent<Button>().interactable = true;
                    scriptsGroup.exercisesManager.sesionesList[i].GetComponent<Image>().sprite = scriptsGroup.exercisesManager.currentSessionSprite;
                }
                else
                {
                    //idListHourExercises = i+1;
                }
                    
                // almacenar el id del ejercicio activado
                if(scriptsGroup.exercisesManager.sesionesList[i].GetComponent<Button>().interactable)
                    idListHourExercises = i;

                // si no se hizo poner -1
                if(i < idListHourExercises && scriptsGroup.exercisesManager.exerciseHourArray[i] != 0)
                    scriptsGroup.exercisesManager.exerciseHourArray[i] = -1;
                // detectar si los anteriores al actual se hicieron o no
                if(scriptsGroup.exercisesManager.exerciseHourArray[i] == -1)
                    scriptsGroup.exercisesManager.sesionesList[i].GetComponent<Image>().sprite = scriptsGroup.exercisesManager.notFinishedSessionSprite; 
                // detectar los que ya se han hecho
                if (scriptsGroup.exercisesManager.exerciseHourArray[i] == 0)
                    scriptsGroup.exercisesManager.sesionesList[i].GetComponent<Image>().sprite = scriptsGroup.exercisesManager.finishedSessionSprite;
            }
        }

        // customize game
        scriptsGroup.customizationManager.SetIdCustomization(PlayerPrefs.GetInt("idCustomization"));
        
        scriptsGroup.customizationManager.idItemFondosArray = Array.ConvertAll(PlayerPrefs.GetString("idItemFondosArray").Split(","), int.Parse);
        scriptsGroup.customizationManager.idItemFigurasArray = Array.ConvertAll(PlayerPrefs.GetString("idItemFigurasArray").Split(","), int.Parse);

        scriptsGroup.customizationManager.SetIdFondosItem(scriptsGroup.customizationManager.idItemFondosArray[PlayerPrefs.GetInt("idCustomization")]);
        scriptsGroup.customizationManager.SetIdFigurasItem(scriptsGroup.customizationManager.idItemFigurasArray[PlayerPrefs.GetInt("idCustomization")]);

    }

    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        scriptsGroup.rewardsManager.SaveReward();
        scriptsGroup.exercisesManager.SaveExercise();
        PlayerPrefs.Save();
    }
}
