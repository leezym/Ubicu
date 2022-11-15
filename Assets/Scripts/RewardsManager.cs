using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;

public class RewardsManager : MonoBehaviour
{
    public static int SERIE_REWARD = 25;
    public static int SESSION_REWARD = 100;
    public static int DAY_REWARD = 150;
    public static int WEEK_REWARD = 300;
    
    [Header("ATTACHED")]    
    public TMP_Text textReward;

    [Header("IN GAME")]
    public int serieReward;
    public int sessionReward;
    public int dayReward;
    public int weekReward;
    public int totalReward;

    void Start()
    {
        LoadReward();
    }

    public void LoadReward()
    {
        sessionReward = PlayerPrefs.GetInt("sessionReward");
        dayReward = PlayerPrefs.GetInt("dayReward");
        totalReward = PlayerPrefs.GetInt("totalReward"); //test
        GameData.Instance.scriptsGroup.exercisesManager.exerciseHourArray = Array.ConvertAll(PlayerPrefs.GetString("exerciseHourArray").Split("/n"), int.Parse);
    }
    
    public void SaveReward() //pdte
    {
        PlayerPrefs.SetInt("sessionReward", sessionReward);
        PlayerPrefs.SetInt("dayReward", dayReward);
        PlayerPrefs.SetInt("totalReward", totalReward); //test
        PlayerPrefs.SetString("exerciseHourArray", string.Join("/n", GameData.Instance.scriptsGroup.exercisesManager.exerciseHourArray));
        PlayerPrefs.Save();
    }

    public void CalculateRewards()
    {
        serieReward = (GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series * RewardsManager.SERIE_REWARD);
        sessionReward += RewardsManager.SESSION_REWARD;
        totalReward += (serieReward + RewardsManager.SESSION_REWARD);
        
        if(sessionReward == (GameData.Instance.scriptsGroup.exercisesManager.sesiones * RewardsManager.SESSION_REWARD))
        {
            dayReward += RewardsManager.DAY_REWARD;
            totalReward += dayReward;
            sessionReward = 0;
        }

        if(dayReward == (GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_dias * RewardsManager.DAY_REWARD) && DateTime.Today == DateTime.Parse(GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].fecha_fin, new CultureInfo("de-DE")))
        {
            weekReward = RewardsManager.WEEK_REWARD;
            totalReward += weekReward;
            dayReward = 0;
            //subir total reward a la DB
        }
    }

    void Update()
    {
        textReward.text = "REWARDS:\nTotal: "+totalReward;
    }
}
