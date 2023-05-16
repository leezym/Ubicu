using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using System.Text.RegularExpressions;


public class RewardsManager : MonoBehaviour
{
    public static int SERIE_REWARD = 25;
    public static int SESSION_REWARD = 100;
    public static int DAY_REWARD = 150;
    public static int WEEK_REWARD = 300;
    
    [Header("ATTACHED")]
    public TMP_Text[] textReward;
    public Sprite[] blockedBadgesSprite;
    public Sprite[] seriesBadgesSmallSprite;
    public Sprite[] sessionsBadgesSmallSprite;
    public Sprite[] daysBadgesSmallSprite;
    public Sprite[] weeksBadgesSmallSprite;
    public Image[] seriesBadgesSmallImage;
    public Image[] sessionsBadgesSmallImage;
    public Image[] daysBadgesSmallImage;
    public Image[] weeksBadgesSmallImage;
    public string[] badgesNames;
    public Sprite[] seriesBadgesBigSprite;
    public Sprite[] sessionsBadgesBigSprite;
    public Sprite[] daysBadgesBigSprite;
    public Sprite[] weeksBadgesBigSprite;
    public int[] badgesPoints;
    public TMP_Text badgesTitle;
    public TMP_Text badgesSubTitle;
    public Image badgesBigImage;
    public TMP_Text badgesDescription;
    

    [Header("IN GAME")]
    public int serieReward;
    public int sessionReward;
    public int dayReward;
    public int totalReward;
    public int totalSeries;
    public int totalSessions;
    public int totalDays;
    public int totalWeeks;
    public AllItems[] allBadgesArray = new AllItems[4]; // cuales insignias se han ganado

    void Start()
    {
        LoadReward();
    }

    public void LoadReward()
    {
        sessionReward = PlayerPrefs.GetInt("sessionReward");
        dayReward = PlayerPrefs.GetInt("dayReward");
        totalReward = PlayerPrefs.GetInt("totalReward");
        totalSeries = PlayerPrefs.GetInt("totalSeries");
        totalSessions = PlayerPrefs.GetInt("totalSessions");
        totalDays = PlayerPrefs.GetInt("totalDays");
        totalWeeks = PlayerPrefs.GetInt("totalWeeks");
        GetAllBadges();
    }
    
    public void SaveReward()
    {
        PlayerPrefs.SetInt("sessionReward", sessionReward);
        PlayerPrefs.SetInt("dayReward", dayReward);
        PlayerPrefs.SetInt("totalReward", totalReward);
        PlayerPrefs.SetInt("totalSeries", totalSeries);
        PlayerPrefs.SetInt("totalSessions", totalSessions);
        PlayerPrefs.SetInt("totalDays", totalDays);
        PlayerPrefs.SetInt("totalWeeks", totalWeeks);
        SetAllBadges();
    }

    public void CalculateRewards() //pdte como reiniciar al final de semana o como subir a final de semana a mongodb
    {
        serieReward = (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series * RewardsManager.SERIE_REWARD);
        sessionReward += RewardsManager.SESSION_REWARD;
        totalReward += (serieReward + RewardsManager.SESSION_REWARD);
        
        NotificationsManager.Instance.WarningNotifications("¡FELICITACIONES!\nGanaste <b>"+serieReward+" Ubicoins</b> por cada serie realizada y <b>"+RewardsManager.SESSION_REWARD+" Ubicoins</b> por el ejercicio completo");
        NotificationsManager.Instance.SetCloseFunction(GameData.Instance.sessionMenu);

        totalSeries += GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series;
        totalSessions++;

        if(sessionReward == (GameData.Instance.scriptsGroup.exercisesManager.sesiones * RewardsManager.SESSION_REWARD))
        {
            dayReward += RewardsManager.DAY_REWARD;
            totalReward += RewardsManager.DAY_REWARD;
            sessionReward = 0;
            NotificationsManager.Instance.SetChangeTextFunction("¡FELICITACIONES!\nGanaste <b>"+RewardsManager.DAY_REWARD+" Ubicoins</b> por completar un día de fisioterapias");
            totalDays++;

            if(dayReward == (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_dias * RewardsManager.DAY_REWARD) && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) == DateTime.ParseExact(GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture))
            {
                totalReward += RewardsManager.WEEK_REWARD;
                dayReward = 0;
                NotificationsManager.Instance.SetChangeTextFunction("¡FELICITACIONES!\nGanaste <b>"+RewardsManager.WEEK_REWARD+" Ubicoins</b> por completar una semana de fisioterapias");
                totalWeeks++;
            }
        }       
        
        CalculateBadges();
        GameData.Instance.SaveLocalData();
        //pdte subir total reward a la DB
    }

    /*public IEnumerator CalculateRewards() //pdte como reiniciar al final de semana o como subir a final de semana a mongodb
    {
        WWWForm form = new WWWForm();
        form.AddField("cedula", userInputField.text);
        form.AddField("password", passInputField.text);
        UnityWebRequest www = UnityWebRequest.Post("https://server.ubicu.co/authenticatePatient", form);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        string responseText = www.downloadHandler.text;
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            Debug.Log(form.data);
        }

        serieReward = (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series * RewardsManager.SERIE_REWARD);
        sessionReward += RewardsManager.SESSION_REWARD;
        totalReward += (serieReward + RewardsManager.SESSION_REWARD);
        NotificationsManager.Instance.WarningNotifications("¡FELICITACIONES!\nGanaste <b>"+serieReward+" Ubicoins</b> por cada serie realizada y <b>"+RewardsManager.SESSION_REWARD+" Ubicoins</b> por el ejercicio completo");
        NotificationsManager.Instance.SetCloseFunction(GameData.Instance.sessionMenu);

        totalSeries += GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series;
        totalSessions++;

        if(sessionReward == (GameData.Instance.scriptsGroup.exercisesManager.sesiones * RewardsManager.SESSION_REWARD))
        {
            dayReward += RewardsManager.DAY_REWARD;
            totalReward += RewardsManager.DAY_REWARD;
            sessionReward = 0;
            NotificationsManager.Instance.SetCloseFunction("¡FELICITACIONES!\nGanaste <b>"+RewardsManager.DAY_REWARD+" Ubicoins</b> por completar un día de fisioterapias");
            totalDays++;
        }

        if(dayReward == (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_dias * RewardsManager.DAY_REWARD) && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) == DateTime.ParseExact(GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture))
        {
            totalReward += RewardsManager.WEEK_REWARD;
            dayReward = 0;
            NotificationsManager.Instance.SetCloseFunction("¡FELICITACIONES!\nGanaste <b>"+RewardsManager.WEEK_REWARD+" Ubicoins</b> por completar una semana de fisioterapias");
            totalWeeks++;   
        }
        //pdte subir total reward a la DB
    }*/

    public void CalculateBadges()
    {    
        for(int j = 0; j < badgesPoints.Length; j++)
        {
            if(totalSeries >= badgesPoints[j] && allBadgesArray[0].item[j] == 0)
            {
                allBadgesArray[0].item[j] = 1;
                NotificationsManager.Instance.SetChangeTextFunction("¡FELICITACIONES!\nGanaste la insignia <b>"+badgesNames[j]+"</b> en Series");
            }
            if(totalSessions >= badgesPoints[j] && allBadgesArray[1].item[j] == 0)
            {
                allBadgesArray[1].item[j] = 1;
                NotificationsManager.Instance.SetChangeTextFunction("¡FELICITACIONES!\nGanaste la insignia <b>"+badgesNames[j]+"</b> en Sesiones");
            }
            if(totalDays >= badgesPoints[j] && allBadgesArray[2].item[j] == 0)
            {
                allBadgesArray[2].item[j] = 1;
                NotificationsManager.Instance.SetChangeTextFunction("¡FELICITACIONES!\nGanaste la insignia <b>"+badgesNames[j]+"</b> en Días");
            }
            if(totalWeeks >= badgesPoints[j] && allBadgesArray[3].item[j] == 0)
            {
                allBadgesArray[3].item[j] = 1;
                NotificationsManager.Instance.SetChangeTextFunction("¡FELICITACIONES!\nGanaste la insignia <b>"+badgesNames[j]+"</b> en Semanas");
            }
        }
    }

    public void ShowInfoBadges(string insignia)
    {
        string[] info = Regex.Split(insignia, ",");
        int i = int.Parse(info[1]);
        int j = 0;
        badgesTitle.text = badgesNames[i];
        badgesDescription.text = "Esta medalla se le otorga a aquellos que lograron completar <b>"+badgesPoints[i]+"</b>";
        
        if(info[0] == "series")
        {
            j = 0;
            badgesSubTitle.text = "en Series";
            badgesBigImage.sprite = seriesBadgesBigSprite[i];
            badgesDescription.text += " series";
        }
        else if(info[0] == "sesiones")
        {
            j = 1;
            badgesSubTitle.text = "en Sesiones";
            badgesBigImage.sprite = sessionsBadgesBigSprite[i];
            badgesDescription.text += " sesiones";
        }
        else if(info[0] == "dias")
        {
            j = 2;
            badgesSubTitle.text = "en Días";
            badgesBigImage.sprite = daysBadgesBigSprite[i];
            badgesDescription.text += " días";
        }
        else if(info[0] == "semanas")
        {
            j = 3;
            badgesSubTitle.text = "en Semanas";
            badgesBigImage.sprite = weeksBadgesBigSprite[i];
            badgesDescription.text += " semanas";
        }
        
        if(allBadgesArray[j].item[i] == 1) //si tienes la insignia muestras la info
            UI_System.Instance.SwitchScreens(GameData.Instance.infoBadgesMenu);
        else
        {
            NotificationsManager.Instance.WarningNotifications("Aún no tienes la insignia <b>"+badgesTitle.text+" "+badgesSubTitle.text+"</b>");
            NotificationsManager.Instance.SetCloseFunction();
        }
    }

    public void GetAllBadges()
    {
        string[] temp = Array.ConvertAll(PlayerPrefs.GetString("allBadgesArray").Split(";"), x => x.ToString());
        for(int i = 0; i < allBadgesArray.Length; i++)
        {
            allBadgesArray[i].item = Array.ConvertAll(temp[i].Split(","), int.Parse);
        }
    }
    public void SetAllBadges()
    {
        string s = "";
        for(int i = 0; i < allBadgesArray.Length; i++)
        {
            s += string.Join(",", allBadgesArray[i].item)+";";
        }
        PlayerPrefs.SetString("allBadgesArray", s);
    }

    public void EnabledBadges(){
        for(int i = 0; i < allBadgesArray.Length; i++)
        {
           for(int j = 0; j < allBadgesArray[i].item.Length; j++)
            {
                seriesBadgesSmallImage[j].sprite = (allBadgesArray[0].item[j] == 1 ? seriesBadgesSmallSprite[j] : blockedBadgesSprite[j]);
                sessionsBadgesSmallImage[j].sprite = (allBadgesArray[1].item[j] == 1 ? sessionsBadgesSmallSprite[j] : blockedBadgesSprite[j]);
                daysBadgesSmallImage[j].sprite = (allBadgesArray[2].item[j] == 1 ? daysBadgesSmallSprite[j] : blockedBadgesSprite[j]);
                weeksBadgesSmallImage[j].sprite = (allBadgesArray[3].item[j] == 1 ? weeksBadgesSmallSprite[j] : blockedBadgesSprite[j]);
            }
        }
    }

    void Update()
    {
        foreach(TMP_Text t in textReward)
            t.text = totalReward.ToString();
    }
}
