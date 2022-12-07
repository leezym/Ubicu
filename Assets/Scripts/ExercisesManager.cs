using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Globalization;

public class ExercisesManager : MonoBehaviour
{
    public static int START_HOUR_EXERCISE = 9;

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

    [Header("IN GAME")]
    public Transform sessionTitlePrefab;
    public int sesiones;
    public List<GameObject> sesionesList = new List<GameObject>();
    public int[] exerciseHourArray = new int[0];
    public float extraMinuteToWaitForExercise;

    public IEnumerator GetExercises()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_user", GameData.Instance.scriptsGroup.login.jsonObject.user._id);
        form.AddField("token", GameData.Instance.scriptsGroup.login.jsonObject.token);

        UnityWebRequest www = UnityWebRequest.Post("http://d2yaaz8bde1qj3.cloudfront.net/allEjerciciosByUser", form);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            Debug.Log(form.data);
        }
        else
        {
            Debug.Log("Post request complete!" + " Response Code: " + www.responseCode);
            string responseText = www.downloadHandler.text;
            Debug.Log("Response Text:" + responseText);

            GameData.Instance.jsonObjectExercises = JsonUtility.FromJson<Exercises>("{\"array\":" + responseText + "}");

            bool emptyExercise = false; // array.Count = 0
            bool uniqueExercise = false; //  array.Count = 1
            bool currentDate = false; // fecha_inicio y fecha_fin dentro de DateTime.Today
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
                    if (DateTime.Today >= DateTime.Parse(GameData.Instance.jsonObjectExercises.array[0].fecha_inicio, new CultureInfo("es-ES")) && DateTime.Today <= DateTime.Parse(GameData.Instance.jsonObjectExercises.array[0].fecha_fin, new CultureInfo("es-ES")))
                        currentDate = true;
                    else
                        currentDate = false;
                }
                else
                {
                    uniqueExercise = false;
                    currentDate = true;
                    if (DateTime.Today >= DateTime.Parse(GameData.Instance.jsonObjectExercises.array[GameData.Instance.jsonObjectExercises.array.Count-1].fecha_inicio, new CultureInfo("es-ES")) && DateTime.Today <= DateTime.Parse(GameData.Instance.jsonObjectExercises.array[GameData.Instance.jsonObjectExercises.array.Count-1].fecha_fin, new CultureInfo("es-ES")))
                    {
                        GameData.Instance.idJsonObjectExercises = GameData.Instance.jsonObjectExercises.array.Count-1;
                    }
                    else if (DateTime.Today >= DateTime.Parse(GameData.Instance.jsonObjectExercises.array[GameData.Instance.jsonObjectExercises.array.Count-2].fecha_inicio, new CultureInfo("es-ES")) && DateTime.Today <= DateTime.Parse(GameData.Instance.jsonObjectExercises.array[GameData.Instance.jsonObjectExercises.array.Count-2].fecha_fin, new CultureInfo("es-ES")))
                    {
                        GameData.Instance.idJsonObjectExercises = GameData.Instance.jsonObjectExercises.array.Count-2;
                    }
                    else
                        currentDate = false;
                }
            }

            // la cantidad de sesiones es de acuerdo al campo cada cuantas horas, es decir 12h/ejercicio.frecuencia_horas 
            int sesiones = AddExercise(emptyExercise, uniqueExercise, currentDate);

            if(sesiones == 0)
            {
                GameObject go = Instantiate(sessionPrefab, Vector3.zero, Quaternion.identity);
                go.SetActive(true);
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
                    sessionTitlePrefab.GetComponent<TMP_Text>().text = "Sesión " + exerciseHourArray[i] + ":00";

                    sesionesList.Add(go);

                    go.GetComponent<Button>().interactable = false;
                }
            }
            StopCoroutine(GetExercises());
            //Debug.Log("Response data:" + GameData.Instance.jsonObjectExercises.array[0]._id);
        }
    }

    public void TestGetExercise()
    {
        GameData.Instance.jsonObjectExercises.array = new List<Exercise>{
            new Exercise{
                _id = "1",
                id_user = "1",
                nombre = "Elizabeth Moncada",
                duracion_total = 30,
                frecuencia_dias = 1,
                frecuencia_horas = 1,
                repeticiones = 3,
                series = 2,
                periodos_descanso = 10,
                fecha_inicio = "12/12/2022",
                fecha_fin = "18/12/2022",
                apnea = 3,
                flujo = 1200
            }, 
            new Exercise{
                _id = "2",
                id_user = "1",
                nombre = "Elizabeth Moncada",
                duracion_total = 30,
                frecuencia_dias = 1,
                frecuencia_horas = 1,
                repeticiones = 2,
                series = 3,
                periodos_descanso = 10,
                fecha_inicio = "05/12/2022",
                fecha_fin = "11/12/2022",
                apnea = 2,
                flujo = 1200
            }
        };

        bool emptyExercise = false; // array.Count = 0
        bool uniqueExercise = false; //  array.Count = 1
        bool currentDate = false; // fecha_inicio y fecha_fin dentro de DateTime.Today
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
                if (DateTime.Today >= DateTime.Parse(GameData.Instance.jsonObjectExercises.array[0].fecha_inicio, new CultureInfo("es-ES")) && DateTime.Today <= DateTime.Parse(GameData.Instance.jsonObjectExercises.array[0].fecha_fin, new CultureInfo("es-ES")))
                    currentDate = true;
                else
                    currentDate = false;
            }
            else
            {
                uniqueExercise = false;
                currentDate = true;
                if (DateTime.Today >= DateTime.Parse(GameData.Instance.jsonObjectExercises.array[GameData.Instance.jsonObjectExercises.array.Count-1].fecha_inicio, new CultureInfo("es-ES")) && DateTime.Today <= DateTime.Parse(GameData.Instance.jsonObjectExercises.array[GameData.Instance.jsonObjectExercises.array.Count-1].fecha_fin, new CultureInfo("es-ES")))
                { 
                    GameData.Instance.idJsonObjectExercises = GameData.Instance.jsonObjectExercises.array.Count-1;
                }
                else if (DateTime.Today >= DateTime.Parse(GameData.Instance.jsonObjectExercises.array[GameData.Instance.jsonObjectExercises.array.Count-2].fecha_inicio, new CultureInfo("es-ES")) && DateTime.Today <= DateTime.Parse(GameData.Instance.jsonObjectExercises.array[GameData.Instance.jsonObjectExercises.array.Count-2].fecha_fin, new CultureInfo("es-ES")))
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
                sessionTitlePrefab.GetComponent<TMP_Text>().text = "Sesión " + exerciseHourArray[i] + ":00";

                sesionesList.Add(go);

                go.GetComponent<Button>().interactable = false;
            }
        }
    }

    int AddExercise(bool emptyExercise, bool uniqueExercise, bool currentDate)
    {
        int add = -1;
        
        if(emptyExercise == true || (emptyExercise == false && uniqueExercise == true && currentDate == false) || (emptyExercise == false && uniqueExercise == false && currentDate == false))
            add = 0;
        if((emptyExercise == false && uniqueExercise == true && currentDate == true) || (emptyExercise == false && uniqueExercise == false && currentDate == true))
            add = 12/GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_horas;

        return add;        
    }

    void AddExcersiseData()
    {
        exerciseRepPrefab.text = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].repeticiones.ToString();
        exerciseSeriePrefab.text = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series.ToString();
        exerciseApneaPrefab.text = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].apnea.ToString();
        exerciseDescansoPrefab.text = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].periodos_descanso.ToString();
        exerciseFlujoPrefab.text = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo.ToString()+"ml";
           
        if(DateTime.Today == DateTime.Parse(PlayerPrefs.GetString("currentExerciseDate"), new CultureInfo("es-ES")) && PlayerPrefs.GetString("exerciseHourArray") != "")
        {
            // actualizar de acuerdo a la DB local
            exerciseHourArray = Array.ConvertAll(PlayerPrefs.GetString("exerciseHourArray").Split(","), int.Parse);
        }
        else
        {
            PlayerPrefs.SetString("currentExerciseDate", DateTime.Today.ToString());
            int hours = START_HOUR_EXERCISE;
            exerciseHourArray = new int[sesiones];
            for(int i = 0; i < sesiones; i++)
            {
                exerciseHourArray[i] = hours;
                hours += GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_horas;
            }
        }

        //extraMinuteToWaitForExercise = (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_horas == 1 ? 30f : 59f); //minutos
        extraMinuteToWaitForExercise = 59f; //test
    }

    public void SaveExercise()
    {
        PlayerPrefs.SetString("exerciseHourArray", string.Join(",", GameData.Instance.scriptsGroup.exercisesManager.exerciseHourArray));
    }
}
