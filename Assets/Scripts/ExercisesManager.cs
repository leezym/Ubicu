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
    [Header("ATTACHED")]
    public GameObject sessionPrefab;
    public GameObject sessionContent;
    public TMP_Text exerciseRepPrefab;
    public TMP_Text exerciseSeriePrefab;
    public TMP_Text exerciseApneaPrefab;
    public TMP_Text exerciseDescansoPrefab;
    public TMP_Text exerciseFlujoPrefab;

    [Header("IN GAME")]
    public ScriptsGroup scriptsGroup;
    public Transform sessionTitlePrefab;
    public Exercises jsonObjectExercises;

    public void Start()
    {
        scriptsGroup = FindObjectOfType<ScriptsGroup>();
        TestGetExercise();
    }
    
    public IEnumerator GetExercises()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_user", scriptsGroup.login.jsonObject.user._id);
        form.AddField("token", scriptsGroup.login.jsonObject.token);

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

            jsonObjectExercises = JsonUtility.FromJson<Exercises>("{\"array\":" + responseText + "}");

            bool emptyExercise = false; // array.Count = 0
            bool uniqueExercise = false; //  array.Count = 1
            bool currentDate = false; // fecha_inicio y fecha_fin dentro de DateTime.Today
            scriptsGroup.gameData.idJsonObjectExercises = -1;

            if(jsonObjectExercises.array.Count == 0)
            {
                emptyExercise = true;
            }
            else
            {
                emptyExercise = false;
                if(jsonObjectExercises.array.Count == 1)
                {
                    uniqueExercise = true;
                    scriptsGroup.gameData.idJsonObjectExercises = 0;
                    if (DateTime.Today >= DateTime.Parse(jsonObjectExercises.array[0].fecha_inicio, new CultureInfo("de-DE")) && DateTime.Today <= DateTime.Parse(jsonObjectExercises.array[0].fecha_fin, new CultureInfo("de-DE")))
                        currentDate = true;
                    else
                        currentDate = false;
                }
                else
                {
                    uniqueExercise = false;
                    currentDate = true;
                    if (DateTime.Today >= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-1].fecha_inicio, new CultureInfo("de-DE")) && DateTime.Today <= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-1].fecha_fin, new CultureInfo("de-DE")))
                    {
                        scriptsGroup.gameData.idJsonObjectExercises = jsonObjectExercises.array.Count-1;
                    }
                    else if (DateTime.Today >= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-2].fecha_inicio, new CultureInfo("de-DE")) && DateTime.Today <= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-2].fecha_fin, new CultureInfo("de-DE")))
                    {
                        scriptsGroup.gameData.idJsonObjectExercises = jsonObjectExercises.array.Count-2;
                    }
                    else
                        currentDate = false;
                }
            }

            // la cantidad de sesiones es de acuerdo al campo cada cuantas horas, es decir 12h/ejercicio.frecuencia_horas 
            int sesiones = AddExercise(emptyExercise, uniqueExercise, currentDate, scriptsGroup.gameData.idJsonObjectExercises);

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
                for(int i = 1; i <= sesiones; i++)
                {
                    GameObject go = Instantiate(sessionPrefab, Vector3.zero, Quaternion.identity);
                    go.SetActive(true);
                    go.transform.parent = sessionContent.transform;
                    go.transform.localScale = new Vector3(1,1,1);

                    sessionTitlePrefab = go.transform.Find("TitleText");
                    sessionTitlePrefab.GetComponent<TMP_Text>().text = "Sesión " + i;

                    go.GetComponent<Button>().onClick.AddListener(()=>{
                        AddExcersiseData(scriptsGroup.gameData.idJsonObjectExercises);              
                    });
                }
            }
            StopCoroutine(GetExercises());
            //Debug.Log("Response data:" + jsonObjectExercises.array[0]._id);
        }
    }

    public void TestGetExercise()
    {
        jsonObjectExercises.array = new List<Exercise>{
            new Exercise{
                _id = "1",
                id_user = "1",
                nombre = "Elizabeth Moncada",
                duracion_total = 30,
                frecuencia_dias = 7,
                frecuencia_horas = 4,
                repeticiones = 2,
                series = 3,
                periodos_descanso = 30,
                fecha_inicio = "07/11/2022",
                fecha_fin = "13/11/2022",
                apnea = 3,
                flujo = 600
            }, 
            new Exercise{
                _id = "2",
                id_user = "1",
                nombre = "Elizabeth Moncada",
                duracion_total = 30,
                frecuencia_dias = 7,
                frecuencia_horas = 2,
                repeticiones = 2,
                series = 3,
                periodos_descanso = 30,
                fecha_inicio = "14/11/2022",
                fecha_fin = "20/11/2022",
                apnea = 2,
                flujo = 600
            }
        };

        bool emptyExercise = false; // array.Count = 0
        bool uniqueExercise = false; //  array.Count = 1
        bool currentDate = false; // fecha_inicio y fecha_fin dentro de DateTime.Today
        scriptsGroup.gameData.idJsonObjectExercises = -1;

        if(jsonObjectExercises.array.Count == 0)
        {
            emptyExercise = true;
        }
        else
        {
            emptyExercise = false;
            if(jsonObjectExercises.array.Count == 1)
            {
                uniqueExercise = true;
                scriptsGroup.gameData.idJsonObjectExercises = 0;
                if (DateTime.Today >= DateTime.Parse(jsonObjectExercises.array[0].fecha_inicio, new CultureInfo("de-DE")) && DateTime.Today <= DateTime.Parse(jsonObjectExercises.array[0].fecha_fin, new CultureInfo("de-DE")))
                    currentDate = true;
                else
                    currentDate = false;
            }
            else
            {
                uniqueExercise = false;
                currentDate = true;
                if (DateTime.Today >= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-1].fecha_inicio, new CultureInfo("de-DE")) && DateTime.Today <= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-1].fecha_fin, new CultureInfo("de-DE")))
                { 
                    scriptsGroup.gameData.idJsonObjectExercises = jsonObjectExercises.array.Count-1;
                }
                else if (DateTime.Today >= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-2].fecha_inicio, new CultureInfo("de-DE")) && DateTime.Today <= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-2].fecha_fin, new CultureInfo("de-DE")))
                {
                    scriptsGroup.gameData.idJsonObjectExercises = jsonObjectExercises.array.Count-2;
                }
                else
                {
                    currentDate = false;
                }
            }
        }

        // la cantidad de sesiones es de acuerdo al campo cada cuantas horas, es decir 12h/ejercicio.frecuencia_horas 
        int sesiones = AddExercise(emptyExercise, uniqueExercise, currentDate, scriptsGroup.gameData.idJsonObjectExercises);

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
            for(int i = 1; i <= sesiones; i++)
            {
                GameObject go = Instantiate(sessionPrefab, Vector3.zero, Quaternion.identity);
                go.SetActive(true);
                go.transform.parent = sessionContent.transform;
                go.transform.localScale = new Vector3(1,1,1);

                sessionTitlePrefab = go.transform.Find("TitleText");
                sessionTitlePrefab.GetComponent<TMP_Text>().text = "Sesión " + i;

                go.GetComponent<Button>().onClick.AddListener(()=>{
                    AddExcersiseData(scriptsGroup.gameData.idJsonObjectExercises);           
                });
            }
        }
    }

    int AddExercise(bool emptyExercise, bool uniqueExercise, bool currentDate, int idJsonObjectExercises)
    {
        int add = -1;
        
        if(emptyExercise == true || (emptyExercise == false && uniqueExercise == true && currentDate == false) || (emptyExercise == false && uniqueExercise == false && currentDate == false))
            add = 0;
        if((emptyExercise == false && uniqueExercise == true && currentDate == true) || (emptyExercise == false && uniqueExercise == false && currentDate == true))
            add = 12/jsonObjectExercises.array[idJsonObjectExercises].frecuencia_horas;

        return add;        
    }

    void AddExcersiseData(int idJsonObjectExercises)
    {
        exerciseRepPrefab.text = jsonObjectExercises.array[idJsonObjectExercises].repeticiones.ToString();
        exerciseSeriePrefab.text = jsonObjectExercises.array[idJsonObjectExercises].series.ToString();
        exerciseApneaPrefab.text = jsonObjectExercises.array[idJsonObjectExercises].apnea.ToString();
        exerciseDescansoPrefab.text = jsonObjectExercises.array[idJsonObjectExercises].periodos_descanso.ToString();
        exerciseFlujoPrefab.text = jsonObjectExercises.array[idJsonObjectExercises].flujo.ToString()+"ml";
    }
}
