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
using System.IO;

public class ExercisesManager : MonoBehaviour
{
    [Header("ATTACHED")]
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
    public GameObject[] sessionPrefab;
    public Transform sessionTitlePrefab;
    public int sesiones;
    public float extraMinuteToWaitForExercise;

    public IEnumerator GetExercises()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_patient", GameData.Instance.jsonObjectUser.user._id);
        form.AddField("token", GameData.Instance.jsonObjectUser.token);

        UnityWebRequest www = UnityWebRequest.Post(GameData.URL+"allEjerciciosByPatient", form);
        //UnityWebRequest www = UnityWebRequest.Post("http://localhost:5000/allEjerciciosByPatient", form);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        string responseText = www.downloadHandler.text;
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
            Debug.Log(form.data);
        }
        else
        {
            GameData.Instance.jsonObjectExercises = JsonUtility.FromJson<Exercises>("{\"array\":" + responseText + "}");
            CreateExercises();
        }
    }

    public void CreateExercises()
    {
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

        // POST_APNEA teniendo en cuenta los 8seg del dispositivo UBICU
        GameData.Instance.scriptsGroup.playerMovement.POST_APNEA = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].apnea == 1 ? 5f 
        : GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].apnea == 2 ? 4f 
        : 3f; // segundos de descanso minimo postapnea antes de comenzar a tomar aire

        // si la fecha del ejercicio actual es diferente a la ultima almacenada
        if(PlayerPrefs.GetString("currentExerciseFinalDate") != "" && DateTime.ParseExact(PlayerPrefs.GetString("currentExerciseFinalDate"), "dd/MM/yyyy", CultureInfo.InvariantCulture) != DateTime.ParseExact(GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture)) // fecha fin ejercicio actual
        {
            GameData.Instance.scriptsGroup.rewardsManager.serieReward = 0;
            GameData.Instance.jsonObjectRewards.session_reward = 0;
            GameData.Instance.jsonObjectRewards.day_reward = 0;
            PlayerPrefs.SetString("currentExerciseFinalDate", GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].fecha_fin);
        }

        // la cantidad de sesiones es de acuerdo al campo cada cuantas horas, es decir 12h/ejercicio.frecuencia_horas 
        sesiones = AddExercise(emptyExercise, uniqueExercise, currentDate);

        if(sesiones == 0)
        {
            sessionPrefab[0].SetActive(true);
            sessionPrefab[0].GetComponent<Button>().interactable = false;
            sessionTitlePrefab = sessionPrefab[0].transform.Find("TitleText");
            sessionTitlePrefab.GetComponent<TMP_Text>().text = "No hay sesiones";
        }
        else
        {
            AddExcersiseData();
            for(int i = 0; i < sesiones; i++)
            {
                sessionPrefab[i].SetActive(true);
                sessionPrefab[i].GetComponent<Button>().interactable = false;
                sessionTitlePrefab = sessionPrefab[i].transform.Find("TitleText");
                sessionTitlePrefab.GetComponent<TMP_Text>().text = "Sesión " + (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].hora_inicio + (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_horas * i)) + ":00";
            }
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
        
        GameData.Instance.exerciseHourArray = new int[sesiones];
        if(DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) != DateTime.ParseExact(PlayerPrefs.GetString("currentExerciseDate"), "dd/MM/yyyy", CultureInfo.InvariantCulture) || PlayerPrefs.GetString("exerciseHourArray") == "")
        {
            PlayerPrefs.SetString("currentExerciseDate", DateTime.Today.ToString("dd/MM/yyyy"));
            int hours = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].hora_inicio;
            for(int i = 0; i < sesiones; i++)
            {
                GameData.Instance.exerciseHourArray[i] = hours;
                hours += GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_horas;
            }
        }
        else
        {
            // actualizar de acuerdo a la DB local
            GameData.Instance.exerciseHourArray = Array.ConvertAll(PlayerPrefs.GetString("exerciseHourArray").Split(","), int.Parse);
        }

        extraMinuteToWaitForExercise = (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].frecuencia_horas == 1 ? 30f : 59f); // minutos
    }

    public void SaveExercise()
    {
        PlayerPrefs.SetString("exerciseHourArray", string.Join(",", GameData.Instance.exerciseHourArray));
    }
    
    public IEnumerator SendResults() //pdte convertir form en json
    {
        Dictionary<string, object> formData = new Dictionary<string, object>();
        formData.Add("id_ejercicio", GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises]._id);
        formData.Add("fecha", PlayerPrefs.GetString("currentExerciseDate"));
        formData.Add("hora", GameData.Instance.exerciseHourArray[GameData.Instance.idListHourExercises]);
        formData.Add("datos", JsonConvert.SerializeObject(GameData.Instance.exerciseSeries));

        string jsonData = JsonConvert.SerializeObject(formData);
        
        string rutaArchivo = Path.Combine(Application.persistentDataPath, "fisioterapia.txt");

        // Verificar si el archivo ya existe
        if (File.Exists(rutaArchivo))
        {
            // Leer el contenido actual del archivo
            string contenido = File.ReadAllText(rutaArchivo);

            // Verificar si el contenido no está vacío
            if (!string.IsNullOrEmpty(contenido))
            {
                // Agregar coma al final del contenido existente si es un array JSON
                if (contenido.Trim().EndsWith("}"))
                {
                    contenido = contenido.TrimEnd() + ",";
                }
            }

            // Insertar el nuevo JSON en el archivo
            File.AppendAllText(rutaArchivo, jsonData);
        }
        else
        {
            // Si el archivo no existe, simplemente escribir el nuevo JSON
            File.WriteAllText(rutaArchivo, jsonData);
        }

        Debug.Log("Datos de ejercicios locales actualizados correctamente");

        if(!GameData.Instance.scriptsGroup.login.notInternet.isOn)
        {
            WWWForm form = new WWWForm();
            string json = JsonConvert.SerializeObject(GameData.Instance.exerciseSeries);

            form.AddField("id_ejercicio", GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises]._id);
            form.AddField("fecha", PlayerPrefs.GetString("currentExerciseDate"));
            form.AddField("hora", GameData.Instance.exerciseHourArray[GameData.Instance.idListHourExercises]);
            form.AddField("datos", json);            

            UnityWebRequest www = UnityWebRequest.Post(GameData.URL+"createResult", form);
            //UnityWebRequest www = UnityWebRequest.Post("http://localhost:5000/createResult", form);

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                Debug.Log(form.data);
            }
            else
            {
                Debug.Log("Datos de ejercicios actualizados correctamente");

                GameData.Instance.exerciseHourArray[GameData.Instance.idListHourExercises] = 0; // si se finalizó se coloca 0
                GameData.Instance.idListHourExercises = -1;
                GameData.Instance.exerciseSeries = new List<ExerciseData>();    
            }
        }
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
