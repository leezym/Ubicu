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
using Newtonsoft.Json.Linq;

public class ExercisesManager : MonoBehaviour
{
    public static ExercisesManager Instance { get; private set; }

    [Header("UI")]
    public UI_Screen sessionMenu;
    public UI_Screen exerciseMenu_Game;
    public UI_Screen serieGraphMenu;

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
    public GameObject[] sessionPrefab = new GameObject[13];

    [Header("IN GAME")]
    public Transform sessionTitlePrefab;
    public int sesiones;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public IEnumerator GetExerciseDate()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_patient", GameData.Instance.jsonObjectUser.user._id);
        form.AddField("token", GameData.Instance.jsonObjectUser.token);

        UnityWebRequest www = UnityWebRequest.Post(GameData.URL + "allExerciseDateByPatient", form);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        string responseText = www.downloadHandler.text;
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("GetExerciseDate: " + www.error);
            Debug.Log("GetExerciseDate: " + form.data);
        }
        else
        {
            GameData.Instance.jsonObjectExerciseDate = JsonConvert.DeserializeObject<ExerciseDate>(responseText);

            if (string.IsNullOrEmpty(GameData.Instance.jsonObjectExerciseDate.current_exercise_date)) // fecha actual
                GameData.Instance.jsonObjectExerciseDate.current_exercise_date = DateTime.Today.ToString("dd/MM/yyyy");

            SendExerciseDate();
        }
    }

    public IEnumerator CreateDefaultExercise(Exercise exercise)
    {
        WWWForm form = new WWWForm();

        form.AddField("token", GameData.Instance.jsonObjectUser.token);
        form.AddField("nombre", exercise.nombre);
        form.AddField("duracion_total", exercise.duracion_total);
        form.AddField("frecuencia_dias", exercise.frecuencia_dias);
        form.AddField("frecuencia_horas", exercise.frecuencia_horas);
        form.AddField("repeticiones", exercise.repeticiones);
        form.AddField("series", exercise.series);
        form.AddField("periodos_descanso", exercise.periodos_descanso);
        form.AddField("apnea", exercise.apnea);
        form.AddField("flujo", exercise.flujo);
        form.AddField("hora_inicio", exercise.hora_inicio);
        form.AddField("fecha_inicio", exercise.fecha_inicio);
        form.AddField("fecha_fin", DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"));
        form.AddField("id_patient", GameData.Instance.jsonObjectUser.user._id);

        UnityWebRequest www = UnityWebRequest.Post(GameData.URL + "createEjercicio", form);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        string responseText = www.downloadHandler.text;
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("CreateDefaultExercise: " + www.error);
            Debug.Log("CreateDefaultExercise: " + form.data);
        }
        else
        {
            Debug.Log("Datos de ejercicio predeterminado creado correctamente");
            exercise = JsonUtility.FromJson<Exercise>(responseText);

            StartCoroutine(CreateResults(id_ejercicio: exercise._id));
        }
    }

    public IEnumerator GetExercises()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_patient", GameData.Instance.jsonObjectUser.user._id);
        form.AddField("token", GameData.Instance.jsonObjectUser.token);

        UnityWebRequest www = UnityWebRequest.Post(GameData.URL + "allEjerciciosByPatient", form);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        string responseText = www.downloadHandler.text;
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("GetExercises: " + www.error);
            Debug.Log("GetExercises: " + form.data);
        }
        else
        {
            List<Exercise> exercisesData = JsonConvert.DeserializeObject<List<Exercise>>(responseText);

            foreach (var exercise in exercisesData)
            {
                if (exercise.nombre == "Predeterminado" && exercise.fecha_inicio == null && exercise.fecha_fin == null)
                {
                    GameData.Instance.jsonObjectExerciseDefault = exercise;
                }
                else if (exercise.nombre != "Predeterminado")
                {
                    GameData.Instance.jsonObjectExercises.Add(exercise);
                }
            }

            if (string.IsNullOrEmpty(GameData.Instance.jsonObjectExerciseDefault.nombre))
                NotificationsManager.Instance.WarningNotifications("¡No tienes un ejercicio predeterminado! Por favor dile a tu fisioterapeuta que te cree uno.\nEste ejercicio te permite trabajar sin conexión a internet.");

            UpdateLocalExercise(GameData.Instance.ObtenerRutaPredeterminado(GameData.Instance.jsonObjectUser.user.cedula), GetJsonExercise(GameData.Instance.jsonObjectExerciseDefault));

            CreateExercisesSesions();
        }
    }

    public string GetJsonExercise(Exercise exercise)
    {
        return JsonConvert.SerializeObject(exercise);
    }

    public void CreateExercisesSesions()
    {
        bool emptyExercise = false; // array.Count = 0
        bool uniqueExercise = false; //  array.Count = 1
        bool currentDate = false; // fecha_inicio y fecha_fin dentro de DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture
        GameData.Instance.idJsonObjectExercises = -1;

        if (GameData.Instance.jsonObjectExercises.Count == 0)
        {
            emptyExercise = true;
        }
        else
        {
            if (GameData.Instance.jsonObjectExercises.Count == 1)
            {
                uniqueExercise = true;
                GameData.Instance.idJsonObjectExercises = 0;
                if (DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) >= DateTime.ParseExact(GameData.Instance.jsonObjectExercises[0].fecha_inicio, "dd/MM/yyyy", CultureInfo.InvariantCulture) && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) <= DateTime.ParseExact(GameData.Instance.jsonObjectExercises[0].fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                    currentDate = true;
            }
            else
            {
                if (DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) >= DateTime.ParseExact(GameData.Instance.jsonObjectExercises[GameData.Instance.jsonObjectExercises.Count - 1].fecha_inicio, "dd/MM/yyyy", CultureInfo.InvariantCulture) && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) <= DateTime.ParseExact(GameData.Instance.jsonObjectExercises[GameData.Instance.jsonObjectExercises.Count - 1].fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                {
                    GameData.Instance.idJsonObjectExercises = GameData.Instance.jsonObjectExercises.Count - 1;
                    currentDate = true;
                }
                else if (DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) >= DateTime.ParseExact(GameData.Instance.jsonObjectExercises[GameData.Instance.jsonObjectExercises.Count - 2].fecha_inicio, "dd/MM/yyyy", CultureInfo.InvariantCulture) && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) <= DateTime.ParseExact(GameData.Instance.jsonObjectExercises[GameData.Instance.jsonObjectExercises.Count - 2].fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture))
                {
                    GameData.Instance.idJsonObjectExercises = GameData.Instance.jsonObjectExercises.Count - 2;
                    currentDate = true;
                }
            }
        }

        // la cantidad de sesiones es de acuerdo al campo cada cuantas horas, es decir 12h/ejercicio.frecuencia_horas + 1
        sesiones = AddExercise(emptyExercise, uniqueExercise, currentDate);

        Debug.Log(sesiones);

        if (sesiones == 0)
        {
            sessionPrefab[0].SetActive(true);
            sessionPrefab[0].GetComponent<Button>().interactable = false;
            sessionTitlePrefab = sessionPrefab[0].transform.Find("TitleText");
            sessionTitlePrefab.GetComponent<TMP_Text>().text = "No hay sesiones";
        }
        else
        {
            // POST_APNEA teniendo en cuenta los 8seg del dispositivo UBICU
            PlayerMovement.Instance.POST_APNEA = GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].apnea == 1 ? 5f
            : GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].apnea == 2 ? 4f
            : 3f; // segundos de descanso minimo postapnea antes de comenzar a tomar aire

            AddExcersiseData();
            for (int i = 0; i < sesiones; i++)
            {
                sessionPrefab[i].SetActive(true);
                sessionPrefab[i].GetComponent<Button>().interactable = false;
                sessionTitlePrefab = sessionPrefab[i].transform.Find("TitleText");
                sessionTitlePrefab.GetComponent<TMP_Text>().text = "Sesión " + (GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].hora_inicio + (GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].frecuencia_horas * i)) + ":00";
            }

            UpdateLocalExercise(GameData.Instance.ObtenerRutaFisioterapia(GameData.Instance.jsonObjectUser.user.cedula), GetJsonExercise(GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises]));
        }
    }

    int AddExercise(bool emptyExercise, bool uniqueExercise, bool currentDate)
    {
        int add = -1;

        if (emptyExercise || (!emptyExercise && uniqueExercise && !currentDate) || (!emptyExercise && !uniqueExercise && !currentDate))
            add = 0;
        if ((!emptyExercise && uniqueExercise && currentDate) || (!emptyExercise && !uniqueExercise && currentDate))
            add = (12 / GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].frecuencia_horas) + 1;

        return add;
    }

    void AddExcersiseData()
    {
        exerciseRepPrefab.text = GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].repeticiones.ToString();
        exerciseSeriePrefab.text = GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].series.ToString();
        exerciseApneaPrefab.text = GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].apnea.ToString();
        exerciseDescansoPrefab.text = GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].periodos_descanso.ToString();
        exerciseFlujoPrefab.text = GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].flujo.ToString() + "ml";

        GameData.Instance.exerciseHourArray = new int[sesiones];

        if (string.IsNullOrEmpty(GameData.Instance.jsonObjectExerciseDate.exercise_hour_array) ||
            string.IsNullOrEmpty(GameData.Instance.jsonObjectExerciseDate.current_exercise_date) ||
            DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) != DateTime.ParseExact(GameData.Instance.jsonObjectExerciseDate.current_exercise_date, "dd/MM/yyyy", CultureInfo.InvariantCulture))
        {
            GameData.Instance.jsonObjectExerciseDate.current_exercise_date = DateTime.Today.ToString("dd/MM/yyyy");
            int hours = GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].hora_inicio;

            for (int i = 0; i < sesiones; i++)
            {
                GameData.Instance.exerciseHourArray[i] = hours;
                hours += GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises].frecuencia_horas;
            }
        }
        else
        {
            GameData.Instance.exerciseHourArray = GameData.Instance.SafeSplitToInt(GameData.Instance.jsonObjectExerciseDate.exercise_hour_array, ','); //Array.ConvertAll(GameData.Instance.jsonObjectExerciseDate.exercise_hour_array.Split(","), int.Parse);
        }

        SendExerciseDate();
    }

    public void UpdateLocalExercise(string path, string jsonData)
    {
        File.WriteAllText(path, jsonData);

        Debug.Log("Datos de fisioterapia locales actualizados correctamente");
    }

    public void UpdateLocalExerciseDate(string jsonData)
    {
        File.WriteAllText(GameData.Instance.ObtenerRutaFechaEjercicio(GameData.Instance.jsonObjectUser.user.cedula), jsonData);

        Debug.Log("Dato de fecha de ejercicio local actualizada correctamente");
    }

    public IEnumerator UpdateExerciseDate(string jsonData)
    {
        UnityWebRequest www = UnityWebRequest.Put(GameData.URL + "updateExerciseDate", jsonData);

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("x-access-token", GameData.Instance.jsonObjectUser.token);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("UpdateExerciseDate: " + www.error);
        }
        else
        {
            Debug.Log("Dato de fecha de ejercicio actualizada correctamente");
        }
    }

    public IEnumerator SendResults()
    {
        if (!Login.Instance.notInternet.isOn)
        {
            WWWForm form = new WWWForm();

            form.AddField("token", GameData.Instance.jsonObjectUser.token);
            form.AddField("id_ejercicio", GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises]._id);
            form.AddField("fecha", GameData.Instance.jsonObjectExerciseDate.current_exercise_date);
            form.AddField("hora", GameData.Instance.exerciseHourArray[GameData.Instance.idListHourExercises]);
            form.AddField("datos", JsonConvert.SerializeObject(GameData.Instance.exerciseSeries));

            UnityWebRequest www = UnityWebRequest.Post(GameData.URL + "createResult", form);

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("SendResults: " + www.error);
                Debug.Log("SendResults: " + form.data);
            }
            else
            {
                Debug.Log("Datos de resultados creados correctamente");

                GameData.Instance.exerciseHourArray[GameData.Instance.idListHourExercises] = 0; // si se finalizó se coloca 0
                GameData.Instance.idListHourExercises = -1;
                SendExerciseDate();
            }
        }
        else
        {
            Dictionary<string, object> formData = new Dictionary<string, object>();

            formData.Add("id_ejercicio", GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises]._id);
            formData.Add("fecha", GameData.Instance.jsonObjectExerciseDate.current_exercise_date);
            formData.Add("hora", GameData.Instance.exerciseHourArray[GameData.Instance.idListHourExercises]);
            formData.Add("datos", JsonConvert.SerializeObject(GameData.Instance.exerciseSeries));

            string jsonData = JsonConvert.SerializeObject(formData);

            if (File.Exists(GameData.Instance.ObtenerRutaResultados(GameData.Instance.jsonObjectUser.user.cedula)))
            {
                JArray jsonArray;
                string contenido = File.ReadAllText(GameData.Instance.ObtenerRutaResultados(GameData.Instance.jsonObjectUser.user.cedula));

                if (!string.IsNullOrWhiteSpace(contenido))
                {
                    jsonArray = JArray.Parse(contenido);
                }
                else
                {
                    jsonArray = new JArray();
                }

                jsonArray.Add(JObject.Parse(jsonData));
                File.WriteAllText(GameData.Instance.ObtenerRutaResultados(GameData.Instance.jsonObjectUser.user.cedula), jsonArray.ToString());
            }
            else
            {
                JArray jsonArray = new JArray
                {
                    JObject.Parse(jsonData)
                };

                File.WriteAllText(GameData.Instance.ObtenerRutaResultados(GameData.Instance.jsonObjectUser.user.cedula), jsonArray.ToString());
            }

            Debug.Log("Datos de resultados locales creados correctamente");

            GameData.Instance.exerciseHourArray[GameData.Instance.idListHourExercises] = 0; // si se finalizó se coloca 0
            GameData.Instance.idListHourExercises = -1;
            SendExerciseDate();
        }

        GameData.Instance.exerciseSeries.Clear(); //= new List<ExerciseData>();
    }

    public string GetJsonExerciseDate()
    {
        GameData.Instance.jsonObjectExerciseDate.exercise_hour_array = string.Join(",", GameData.Instance.exerciseHourArray);

        return JsonConvert.SerializeObject(GameData.Instance.jsonObjectExerciseDate);
    }

    public void SendExerciseDate()
    {
        string jsonData = GetJsonExerciseDate();

        UpdateLocalExerciseDate(jsonData);

        if (!Login.Instance.notInternet.isOn)
        {
            StartCoroutine(UpdateExerciseDate(jsonData));
        }
    }

    public IEnumerator CreateResults(string id_ejercicio = "")
    {
        // Leer el archivo de texto completo
        string fileContent = File.ReadAllText(GameData.Instance.ObtenerRutaResultados(GameData.Instance.jsonObjectUser.user.cedula));

        // Dividir el contenido del archivo en cada conjunto de datos
        JArray dataSets = JArray.Parse(fileContent);

        foreach (JObject jsonObject in dataSets)
        {
            if (!string.IsNullOrEmpty(id_ejercicio))
                jsonObject["id_ejercicio"] = id_ejercicio;

            WWWForm form = new WWWForm();

            form.AddField("token", GameData.Instance.jsonObjectUser.token);
            form.AddField("id_ejercicio", jsonObject["id_ejercicio"].ToString());
            form.AddField("fecha", jsonObject["fecha"].ToString());
            form.AddField("hora", jsonObject["hora"].ToString());
            form.AddField("datos", jsonObject["datos"].ToString());

            UnityWebRequest www = UnityWebRequest.Post(GameData.URL + "createResult", form);

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("CreateResults: " + www.error);
                Debug.Log("CreateResults: " + form.data);
            }
            else
            {
                File.Delete(GameData.Instance.ObtenerRutaResultados(GameData.Instance.jsonObjectUser.user.cedula));
                Debug.Log("Datos de resultados creados correctamente (locales)");
            }
        }
    }
}
