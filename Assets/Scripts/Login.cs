using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Globalization;

[Serializable]
public class Data
{
    public string token;
    public User user;
}

[Serializable]
public class User
{
    public string _id;
    public string nombre;
    public string cedula;
    public string tel;
    public string email;
    public int edad;
    public float peso;
    public float altura;
    public string ciudad;
    public string direccion;
    public string __v;
}

[Serializable]
public class Exercise
{
    public string _id;
    public string id_user;
    public string nombre;
    public int duracion_total;
    public int frecuencia_dias;
    public int frecuencia_horas;
    public int repeticiones;
    public int series;
    public int periodos_descanso;
    public string fecha_inicio;
    public string fecha_fin;
    public int apnea;
    public int flujo;
    public string __v;
}

[Serializable]
public class Exercises
{
    public List<Exercise> array;
}

[Serializable]
public class ExerciseValue
{
    public List<ExerciseItem> array = new List<ExerciseItem>();
}

[Serializable]
public class ExerciseItem
{
    public string time;
    public float volume;
}

public class Login : MonoBehaviour
{
    [Header("ATTACHED")]
    public TMP_InputField userInputField;
    public TMP_InputField passInputField;
    public GameObject sessionPrefab;
    public Transform sessionContent;
    public TMP_Text exerciseRepPrefab;
    public TMP_Text exerciseSeriePrefab;
    public TMP_Text exerciseApneaPrefab;
    public TMP_Text exerciseDescansoPrefab;
    public TMP_Text exerciseFlujoPrefab;
    public GameObject loginMenu, sessionMenu, exerciseMenu;

    [Header("IN GAME")]
    public Transform sessionTitlePrefab;
        
    public Data jsonObject;
    public int idJsonObjectExercises;
    public Exercises jsonObjectExercises;

    public void LogIn(){
        //StartCoroutine(OnLogin());
        TestGetExercise(); // test
    }

    public IEnumerator OnLogin()
    {
        WWWForm form = new WWWForm();
        form.AddField("cedula", userInputField.text);
        form.AddField("password", passInputField.text);

        UnityWebRequest www = UnityWebRequest.Post("http://d2yaaz8bde1qj3.cloudfront.net/AuthenticateUser", form);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            Debug.Log(form.data);

            userInputField.text = "ERROR";
            passInputField.text = "";
        }
        else
        {
            Debug.Log("Post request complete!" + " Response Code: " + www.responseCode);
            string responseText = www.downloadHandler.text;
            Debug.Log("Response Text:" + responseText);

            jsonObject = JsonUtility.FromJson<Data>(responseText);

            string json = JsonUtility.ToJson(jsonObject);
            Debug.Log("Response json:" + json);
            Debug.Log("Response token:" + jsonObject.token);
            Debug.Log("Response name:" + jsonObject.user.nombre);

            loginMenu.SetActive(false);
            sessionMenu.SetActive(true);

            StopCoroutine(OnLogin());
            StartCoroutine(GetExercises());
        }
    }

    public IEnumerator GetExercises()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_user", jsonObject.user._id);
        form.AddField("token", jsonObject.token);

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
            bool currentDate = false; // fecha_inicio y fecha_fin dentro de DateTime.Now
            idJsonObjectExercises = -1;

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
                    idJsonObjectExercises = 0;
                    if (DateTime.Now >= DateTime.Parse(jsonObjectExercises.array[0].fecha_inicio, new CultureInfo("de-DE")) && DateTime.Now <= DateTime.Parse(jsonObjectExercises.array[0].fecha_fin, new CultureInfo("de-DE")))
                        currentDate = true;
                    else
                        currentDate = false;
                }
                else
                {
                    uniqueExercise = false;
                    currentDate = true;
                    if (DateTime.Now >= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-1].fecha_inicio, new CultureInfo("de-DE")) && DateTime.Now <= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-1].fecha_fin, new CultureInfo("de-DE")))
                    {
                        idJsonObjectExercises = jsonObjectExercises.array.Count-1;
                    }
                    else if (DateTime.Now >= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-2].fecha_inicio, new CultureInfo("de-DE")) && DateTime.Now <= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-2].fecha_fin, new CultureInfo("de-DE")))
                    {
                        idJsonObjectExercises = jsonObjectExercises.array.Count-2;
                    }
                    else
                        currentDate = false;
                }
            }

            // la cantidad de sesiones es de acuerdo al campo cada cuantas horas, es decir 12h/ejercicio.frecuencia_horas 
            int sesiones = AddExercise(emptyExercise, uniqueExercise, currentDate, idJsonObjectExercises);

            if(sesiones == 0)
            {
                GameObject go = Instantiate(sessionPrefab, Vector3.zero, Quaternion.identity);
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
                    go.transform.parent = sessionContent.transform;
                    go.transform.localScale = new Vector3(1,1,1);

                    sessionTitlePrefab = go.transform.Find("TitleText");
                    sessionTitlePrefab.GetComponent<TMP_Text>().text = "Sesión " + i;

                    go.GetComponent<Button>().onClick.AddListener(()=>{
                        sessionMenu.SetActive(false);
                        exerciseMenu.SetActive(true);
                        AddExcersiseData();                 
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
                repeticiones = 5,
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
                fecha_inicio = "31/10/2022",
                fecha_fin = "06/11/2022",
                apnea = 2,
                flujo = 600
            }
        };

        bool emptyExercise = false; // array.Count = 0
        bool uniqueExercise = false; //  array.Count = 1
        bool currentDate = false; // fecha_inicio y fecha_fin dentro de DateTime.Now
        idJsonObjectExercises = -1;

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
                idJsonObjectExercises = 0;
                if (DateTime.Now >= DateTime.Parse(jsonObjectExercises.array[0].fecha_inicio, new CultureInfo("de-DE")) && DateTime.Now <= DateTime.Parse(jsonObjectExercises.array[0].fecha_fin, new CultureInfo("de-DE")))
                    currentDate = true;
                else
                    currentDate = false;
            }
            else
            {
                uniqueExercise = false;
                currentDate = true;
                if (DateTime.Now >= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-1].fecha_inicio, new CultureInfo("de-DE")) && DateTime.Now <= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-1].fecha_fin, new CultureInfo("de-DE")))
                {
                    idJsonObjectExercises = jsonObjectExercises.array.Count-1;
                }
                else if (DateTime.Now >= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-2].fecha_inicio, new CultureInfo("de-DE")) && DateTime.Now <= DateTime.Parse(jsonObjectExercises.array[jsonObjectExercises.array.Count-2].fecha_fin, new CultureInfo("de-DE")))
                {
                    idJsonObjectExercises = jsonObjectExercises.array.Count-2;
                }
                else
                    currentDate = false;
            }
        }

        // la cantidad de sesiones es de acuerdo al campo cada cuantas horas, es decir 12h/ejercicio.frecuencia_horas 
        int sesiones = AddExercise(emptyExercise, uniqueExercise, currentDate, idJsonObjectExercises);

        if(sesiones == 0)
        {
            GameObject go = Instantiate(sessionPrefab, Vector3.zero, Quaternion.identity);
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
                go.transform.parent = sessionContent.transform;
                go.transform.localScale = new Vector3(1,1,1);

                sessionTitlePrefab = go.transform.Find("TitleText");
                sessionTitlePrefab.GetComponent<TMP_Text>().text = "Sesión " + i;

                go.GetComponent<Button>().onClick.AddListener(()=>{
                    sessionMenu.SetActive(false);
                    exerciseMenu.SetActive(true);
                    AddExcersiseData();           
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

    void AddExcersiseData()
    {
        exerciseRepPrefab.text = jsonObjectExercises.array[idJsonObjectExercises].repeticiones.ToString();
        exerciseSeriePrefab.text = jsonObjectExercises.array[idJsonObjectExercises].series.ToString();
        exerciseApneaPrefab.text = jsonObjectExercises.array[idJsonObjectExercises].apnea.ToString();
        exerciseDescansoPrefab.text = jsonObjectExercises.array[idJsonObjectExercises].periodos_descanso.ToString();
        exerciseFlujoPrefab.text = jsonObjectExercises.array[idJsonObjectExercises].flujo.ToString()+"ml";
    }
}
