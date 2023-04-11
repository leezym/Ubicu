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
    public TMP_Text[] textReward;

    [Header("IN GAME")]
    public int serieReward;
    public int sessionReward;
    public int dayReward;
    public int totalReward;

    public void LoadReward()
    {
        sessionReward = PlayerPrefs.GetInt("sessionReward");
        dayReward = PlayerPrefs.GetInt("dayReward");
        totalReward = PlayerPrefs.GetInt("totalReward");
    }
    
    public void SaveReward()
    {
        PlayerPrefs.SetInt("sessionReward", sessionReward);
        PlayerPrefs.SetInt("dayReward", dayReward);
        PlayerPrefs.SetInt("totalReward", totalReward); //test
    }

    public void CalculateRewards()
    {
        serieReward = (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series * RewardsManager.SERIE_REWARD);
        sessionReward += RewardsManager.SESSION_REWARD;
        totalReward += (serieReward + RewardsManager.SESSION_REWARD);
        NotificationsManager.Instance.WarningNotifications("¡FELICITACIONES!\nGanaste "+serieReward+" Ubicoins por cada serie realizada y "+RewardsManager.SESSION_REWARD+" por el ejercicio completo");
        NotificationsManager.Instance.SetCloseFunction(GameData.Instance.sessionMenu);

        if(sessionReward == (GameData.Instance.scriptsGroup.exercisesManager.sesiones * RewardsManager.SESSION_REWARD))
        {
            dayReward += RewardsManager.DAY_REWARD;
            totalReward += RewardsManager.DAY_REWARD;
            sessionReward = 0;
            NotificationsManager.Instance.SetCloseFunction("¡FELICITACIONES!\nGanaste "+RewardsManager.DAY_REWARD+" Ubicoins por completar un día de fisioterapias");
        }

        if(dayReward == (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_dias * RewardsManager.DAY_REWARD) && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) == DateTime.ParseExact(GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture))
        {
            totalReward += RewardsManager.WEEK_REWARD;
            dayReward = 0;
            NotificationsManager.Instance.SetCloseFunction("¡FELICITACIONES!\nGanaste "+RewardsManager.WEEK_REWARD+" Ubicoins por completar una semana de fisioterapias");
        }

        //pdte subir total reward a la DB
    }

    void Update()
    {
        foreach(TMP_Text t in textReward)
            t.text = totalReward.ToString();
    }
}
