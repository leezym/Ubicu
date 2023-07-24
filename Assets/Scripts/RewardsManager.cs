using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using Newtonsoft.Json;

[System.Serializable]
public class BadgePointsContainer
{
    public int[] badgesPoints;
}

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
    public BadgePointsContainer[] badgesPoints;
    public TMP_Text badgesTitle;
    public TMP_Text badgesSubTitle;
    public Image badgesBigImage;
    public TMP_Text badgesDescription;
    

    [Header("IN GAME")]
    public int serieReward;
    public AllItems[] allBadgesArray = new AllItems[4]; // cuales insignias se han ganado

    public IEnumerator GetRewards()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_patient", GameData.Instance.jsonObjectUser.user._id);
        form.AddField("token", GameData.Instance.jsonObjectUser.token);

        UnityWebRequest www = UnityWebRequest.Post(GameData.URL+"allRewardsByPatient", form);
        //UnityWebRequest www = UnityWebRequest.Post("http://localhost:5000/allRewardsByPatient", form);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        string responseText = www.downloadHandler.text;

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            Debug.Log(form.data);
        }
        else
        {
            GameData.Instance.jsonObjectRewards = JsonConvert.DeserializeObject<Rewards>(responseText);
            GetAllBadges();
        }
        StopCoroutine(GetRewards());
    }

    public IEnumerator SendReward()
    {
        WWWForm form = new WWWForm();
        
        SetAllBadges();
        string jsonData = JsonConvert.SerializeObject(GameData.Instance.jsonObjectRewards);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        
        UnityWebRequest www = UnityWebRequest.Put(GameData.URL+"updateRewards", jsonData);
        //UnityWebRequest www = UnityWebRequest.Post("http://localhost:5000/updateRewards", jsonData);

        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            Debug.Log(form.data);
        }
        else
        {
            Debug.Log("Datos de recompensas actualizados correctamente");
        }
            
        StopCoroutine(SendReward());
    }

    public void CalculateRewards()
    {
        serieReward = (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series * RewardsManager.SERIE_REWARD);
        GameData.Instance.jsonObjectRewards.session_reward += RewardsManager.SESSION_REWARD;
        GameData.Instance.jsonObjectRewards.total_reward += (serieReward + RewardsManager.SESSION_REWARD);
        
        NotificationsManager.Instance.WarningNotifications("¡FELICITACIONES!\nGanaste <b>"+serieReward+" Ubicoins</b> por cada serie realizada y <b>"+RewardsManager.SESSION_REWARD+" Ubicoins</b> por el ejercicio completo");
        NotificationsManager.Instance.SetCloseFunction(GameData.Instance.sessionMenu);

        GameData.Instance.jsonObjectRewards.total_series += GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series;
        GameData.Instance.jsonObjectRewards.total_sessions++;

        if(GameData.Instance.jsonObjectRewards.session_reward == (GameData.Instance.scriptsGroup.exercisesManager.sesiones * RewardsManager.SESSION_REWARD))
        {
            GameData.Instance.jsonObjectRewards.day_reward += RewardsManager.DAY_REWARD;
            GameData.Instance.jsonObjectRewards.total_reward += RewardsManager.DAY_REWARD;
            GameData.Instance.jsonObjectRewards.session_reward = 0;
            NotificationsManager.Instance.SetChangeTextFunction("¡FELICITACIONES!\nGanaste <b>"+RewardsManager.DAY_REWARD+" Ubicoins</b> por completar un día de fisioterapias");
            GameData.Instance.jsonObjectRewards.total_days++;

            if(GameData.Instance.jsonObjectRewards.day_reward == (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_dias * RewardsManager.DAY_REWARD) && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) == DateTime.ParseExact(GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture))
            {
                GameData.Instance.jsonObjectRewards.total_reward += RewardsManager.WEEK_REWARD;
                GameData.Instance.jsonObjectRewards.day_reward = 0;
                NotificationsManager.Instance.SetChangeTextFunction("¡FELICITACIONES!\nGanaste <b>"+RewardsManager.WEEK_REWARD+" Ubicoins</b> por completar una semana de fisioterapias");
                GameData.Instance.jsonObjectRewards.total_weeks++;
            }
        }       
        
        CalculateBadges();
        StartCoroutine(SendReward());
    }

    public void CalculateBadges()
    {    
        for(int j = 0; j < badgesPoints[0].badgesPoints.Length; j++)
        {
            if(GameData.Instance.jsonObjectRewards.total_series >= badgesPoints[0].badgesPoints[j] && allBadgesArray[0].item[j] == 0)
            {
                allBadgesArray[0].item[j] = 1;
                NotificationsManager.Instance.SetChangeTextFunction("¡FELICITACIONES!\nGanaste la insignia <b>"+badgesNames[j]+"</b> en Series");
            }
            if(GameData.Instance.jsonObjectRewards.total_sessions >= badgesPoints[1].badgesPoints[j] && allBadgesArray[1].item[j] == 0)
            {
                allBadgesArray[1].item[j] = 1;
                NotificationsManager.Instance.SetChangeTextFunction("¡FELICITACIONES!\nGanaste la insignia <b>"+badgesNames[j]+"</b> en Sesiones");
            }
            if(GameData.Instance.jsonObjectRewards.total_days >= badgesPoints[2].badgesPoints[j] && allBadgesArray[2].item[j] == 0)
            {
                allBadgesArray[2].item[j] = 1;
                NotificationsManager.Instance.SetChangeTextFunction("¡FELICITACIONES!\nGanaste la insignia <b>"+badgesNames[j]+"</b> en Días");
            }
            if(GameData.Instance.jsonObjectRewards.total_weeks >= badgesPoints[3].badgesPoints[j] && allBadgesArray[3].item[j] == 0)
            {
                allBadgesArray[3].item[j] = 1;
                NotificationsManager.Instance.SetChangeTextFunction("¡FELICITACIONES!\nGanaste la insignia <b>"+badgesNames[j]+"</b> en Semanas");
            }
        }
    }

    public void ShowInfoBadges(string insignia)
    {
        string[] info = Regex.Split(insignia, ","); // item(0), valor(1)
        int i = int.Parse(info[1]);
        int j = 0;
        badgesTitle.text = badgesNames[i];
        badgesDescription.text = "Esta medalla se le otorga a aquellos que lograron completar <b>";
        
        if(info[0] == "series")
        {
            j = 0;
            badgesSubTitle.text = "en Series";
            badgesBigImage.sprite = seriesBadgesBigSprite[i];
            badgesDescription.text += badgesPoints[0].badgesPoints[i]+"</b> series";
        }
        else if(info[0] == "sesiones")
        {
            j = 1;
            badgesSubTitle.text = "en Sesiones";
            badgesBigImage.sprite = sessionsBadgesBigSprite[i];
            badgesDescription.text += badgesPoints[1].badgesPoints[i]+"</b> sesiones";
        }
        else if(info[0] == "dias")
        {
            j = 2;
            badgesSubTitle.text = "en Días";
            badgesBigImage.sprite = daysBadgesBigSprite[i];
            badgesDescription.text += badgesPoints[2].badgesPoints[i]+"</b> días";
        }
        else if(info[0] == "semanas")
        {
            j = 3;
            badgesSubTitle.text = "en Semanas";
            badgesBigImage.sprite = weeksBadgesBigSprite[i];
            badgesDescription.text += badgesPoints[3].badgesPoints[i]+"</b> semanas";
        }
        
        if(allBadgesArray[j].item[i] == 1) //si tienes la insignia muestras la info
            UI_System.Instance.SwitchScreens(GameData.Instance.infoBadgesMenu);
        else
        {
            NotificationsManager.Instance.WarningNotifications("Aún no tienes la insignia <b>"+badgesTitle.text+" "+badgesSubTitle.text+"</b>");
            NotificationsManager.Instance.SetCloseFunction();
        }
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

    public void GetAllBadges()
    {
        string[] temp;
        temp = Array.ConvertAll(GameData.Instance.jsonObjectRewards.all_badges_array.Split(";"), x => x.ToString());
        for(int i = 0; i < allBadgesArray.Length; i++)
        {
            allBadgesArray[i].item = Array.ConvertAll(temp[i].Split(","), int.Parse);
        }
    }
    public void SetAllBadges()
    {
        GameData.Instance.jsonObjectRewards.all_badges_array = "";
        for(int i = 0; i < allBadgesArray.Length; i++)
        {
            GameData.Instance.jsonObjectRewards.all_badges_array += string.Join(",", allBadgesArray[i].item)+";";
        }
    }

    void Update()
    {
        foreach(TMP_Text t in textReward)
            t.text = GameData.Instance.jsonObjectRewards.total_reward.ToString();
    }
}
