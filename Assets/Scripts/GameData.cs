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

    /*public Exercises m_jsonObjectExercises;
    public bool jsonObjectExercises
    {
        get { return m_jsonObjectExercises; }
        set { m_jsonObjectExercises = value; }
    }*/

    private int m_idListHourExercises;
    public int idListHourExercises
    {
        get { return m_idListHourExercises; }
        set { m_idListHourExercises = value; }
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        Debug.Log("Start");
        scriptsGroup.rewardsManager.LoadReward();
        idListHourExercises = -1;
    }
    
    void Update()
    {
        if(playing)
        {
            StartCoroutine(scriptsGroup.obstacles.ObstaclesCounter());
        }
        //else
            //ya se acaben las series de la sesion

        if(resting)
            scriptsGroup.playerMovement.RestingPlayer();

        
        // select available session
        if(scriptsGroup.login.sessionMenu.gameObject.activeSelf)
        {
            for(int i = 0; i < scriptsGroup.exercisesManager.exerciseHourArray.Length; i++)
            {
                // detectar que ejercicio se debe activar
                if(DateTime.Now.Hour >= scriptsGroup.exercisesManager.exerciseHourArray[i] 
                    && DateTime.Now.Hour <= (scriptsGroup.exercisesManager.exerciseHourArray[i] + scriptsGroup.exercisesManager.extraHourToWaitForExercise))
                {
                    scriptsGroup.exercisesManager.sesionesList[i].GetComponent<Button>().interactable = true;
                    scriptsGroup.exercisesManager.sesionesList[i].GetComponent<Image>().sprite = scriptsGroup.exercisesManager.currentSessionSprite;
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
    }

    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        scriptsGroup.rewardsManager.SaveReward();
        scriptsGroup.exercisesManager.SaveExercise();
        PlayerPrefs.Save();
    }
}
