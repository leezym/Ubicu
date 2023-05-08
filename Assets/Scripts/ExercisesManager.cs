using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Globalization;
using Newtonsoft.Json;

public class ExercisesManager : MonoBehaviour
{
    [Header("ATTACHED")]
    public GameObject sessionPrefab;
    public GameObject sessionContent;
    public TMP_Text exerciseRepPrefab;
    public TMP_Text exerciseSeriePrefab;
    public TMP_Text exerciseApneaPrefab;
    public TMP_Text exerciseDescansoPrefab;
    public TMP_Text exerciseFlujoPrefab;
    public Sprite unavailableSessionSprite;
    public Sprite currentSessionSprite;
    public Sprite finishedSessionSprite;
    public Sprite notFinishedSessionSprite;
    public VideoPlayer tutorialVideo;
    public GameObject buttonPlayVideo;

    [Header("IN GAME")]
    public Transform sessionTitlePrefab;
    public int sesiones;
    public List<GameObject> sesionesList = new List<GameObject>();
    public float extraMinuteToWaitForExercise;

    public IEnumerator GetExercises()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_patient", GameData.Instance.jsonObjectUser.user._id);
        form.AddField("token", GameData.Instance.jsonObjectUser.token);

        UnityWebRequest www = UnityWebRequest.Post("https://server.ubicu.co/allEjerciciosByPatient", form);
        //UnityWebRequest www = UnityWebRequest.Post("http://localhost:5000/allEjerciciosByPatient", form);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            Debug.Log(form.data);
        }
        else
        {
            string responseText = www.downloadHandler.text;

            GameData.Instance.jsonObjectExercises = JsonUtility.FromJson<Exercises>("{\"array\":" + responseText + "}");

            bool emptyExercise = false; // array.Count = 0
            bool uniqueExercise = false; //  array.Count = 1
            bool currentDate = false; // fecha_inicio y fecha_fin dentro de DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture
            GameData.Instance.idJsonObjectExercises = -1;

            if(GameData.Instance.jsonObjectExercises.array.Count == 0)
            {
                emptyExercise = true;
            }
            else
            {
                emptyExercise = false;
                if(GameData.Instance.jsonObjectExercises.array.Count == 1)
                {
                    uniqueExercise = true;
                    GameData.Instance.idJsonObjectExercises = 0;
                    if (DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) >= DateTime.ParseExact(GameData.Instance.jsonObjectExercises.array[0].fecha_inicio, "dd/MM/yyyy", CultureInfo.InvariantCulture) && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) <= DateTime.ParseExact(GameData.Instance.jsonObjectExercises.array[0].fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                        currentDate = true;
                    else
                        currentDate = false;
                }
                else
                {
                    uniqueExercise = false;
                    currentDate = true;
                    if (DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) >= DateTime.ParseExact(GameData.Instance.jsonObjectExercises.array[GameData.Instance.jsonObjectExercises.array.Count-1].fecha_inicio, "dd/MM/yyyy", CultureInfo.InvariantCulture) && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) <= DateTime.ParseExact(GameData.Instance.jsonObjectExercises.array[GameData.Instance.jsonObjectExercises.array.Count-1].fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                    { 
                        GameData.Instance.idJsonObjectExercises = GameData.Instance.jsonObjectExercises.array.Count-1;
                    }
                    else if (DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) >= DateTime.ParseExact(GameData.Instance.jsonObjectExercises.array[GameData.Instance.jsonObjectExercises.array.Count-2].fecha_inicio, "dd/MM/yyyy", CultureInfo.InvariantCulture) && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) <= DateTime.ParseExact(GameData.Instance.jsonObjectExercises.array[GameData.Instance.jsonObjectExercises.array.Count-2].fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                    {
                        GameData.Instance.idJsonObjectExercises = GameData.Instance.jsonObjectExercises.array.Count-2;
                    }
                    else
                    {
                        currentDate = false;
                    }
                }
            }

            // la cantidad de sesiones es de acuerdo al campo cada cuantas horas, es decir 12h/ejercicio.frecuencia_horas 
            sesiones = AddExercise(emptyExercise, uniqueExercise, currentDate);

            if(sesiones == 0)
            {
                GameObject go = Instantiate(sessionPrefab, Vector3.zero, Quaternion.identity);
                go.SetActive(true);
                go.GetComponent<Button>().interactable = false;
                go.transform.parent = sessionContent.transform;
                go.transform.localScale = new Vector3(1,1,1);

                sessionTitlePrefab = go.transform.Find("TitleText");
                sessionTitlePrefab.GetComponent<TMP_Text>().text = "No hay sesiones";
            }
            else
            {
                AddExcersiseData();
                for(int i = 0; i < sesiones; i++)
                {
                    GameObject go = Instantiate(sessionPrefab, Vector3.zero, Quaternion.identity);
                    go.SetActive(true);
                    go.transform.parent = sessionContent.transform;
                    go.transform.localScale = new Vector3(1,1,1);

                    sessionTitlePrefab = go.transform.Find("TitleText");
                    sessionTitlePrefab.GetComponent<TMP_Text>().text = "Sesión " + (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].hora_inicio + (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_horas * i)) + ":00";

                    sesionesList.Add(go);

                    go.GetComponent<Button>().interactable = false;
                }
            }
            StopCoroutine(GetExercises());
        }
    }

    int AddExercise(bool emptyExercise, bool uniqueExercise, bool currentDate)
    {
        int add = -1;
        
        if(emptyExercise == true || (emptyExercise == false && uniqueExercise == true && currentDate == false) || (emptyExercise == false && uniqueExercise == false && currentDate == false))
            add = 0;
        if((emptyExercise == false && uniqueExercise == true && currentDate == true) || (emptyExercise == false && uniqueExercise == false && currentDate == true))
            add = (12/GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_horas) + 1;

        return add;        
    }

    void AddExcersiseData()
    {
        exerciseRepPrefab.text = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].repeticiones.ToString();
        exerciseSeriePrefab.text = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series.ToString();
        exerciseApneaPrefab.text = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].apnea.ToString();
        exerciseDescansoPrefab.text = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].periodos_descanso.ToString();
        exerciseFlujoPrefab.text = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo.ToString()+"ml";
        
        if(DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) == DateTime.ParseExact(PlayerPrefs.GetString("currentExerciseDate"), "dd/MM/yyyy", CultureInfo.InvariantCulture) && PlayerPrefs.GetString("exerciseHourArray") != "")
        {
            // actualizar de acuerdo a la DB local
            GameData.Instance.exerciseHourArray = Array.ConvertAll(PlayerPrefs.GetString("exerciseHourArray").Split(","), int.Parse);
        }
        else
        {
            PlayerPrefs.SetString("currentExerciseDate", DateTime.Today.ToString("dd/MM/yyyy"));
            int hours = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].hora_inicio;
            GameData.Instance.exerciseHourArray = new int[sesiones];
            for(int i = 0; i < sesiones; i++)
            {
                GameData.Instance.exerciseHourArray[i] = hours;
                hours += GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_horas;
            }
        }

        extraMinuteToWaitForExercise = (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_horas == 1 ? 30f : 59f); // minutos
    }

    public void SaveExercise()
    {
        PlayerPrefs.SetString("exerciseHourArray", string.Join(",", GameData.Instance.exerciseHourArray));
    }
    
    public IEnumerator SendResults()
    {
        
        WWWForm form = new WWWForm();
        string json = JsonConvert.SerializeObject(GameData.Instance.exerciseSeries);

        form.AddField("id_ejercicio", GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises]._id);
        form.AddField("fecha", PlayerPrefs.GetString("currentExerciseDate"));
        form.AddField("hora", GameData.Instance.exerciseHourArray[GameData.Instance.idListHourExercises]);
        form.AddField("datos", json);

        UnityWebRequest www = UnityWebRequest.Post("https://server.ubicu.co/createResult", form);
        //UnityWebRequest www = UnityWebRequest.Post("http://localhost:5000/createResult", form);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            Debug.Log(form.data);
        }

        GameData.Instance.exerciseSeries = new List<ExerciseData>();        
        StopCoroutine(SendResults());
    }

    IEnumerator PlayVideo()
    {
        tutorialVideo.Play();
        yield return new WaitForSeconds(5f);
        buttonPlayVideo.SetActive(true);
    }

    public void CallPlayVideo()
    {
        StartCoroutine(PlayVideo());
    }
}
