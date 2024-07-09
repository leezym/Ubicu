using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.IO;

public class GameData : MonoBehaviour
{
    public static GameData Instance {get; private set;}
    public static string URL = "https://server.ubicu.co/";
    //public static string URL = "http://localhost:5000/";

    public string rutaArchivoPaciente => Path.Combine(Application.temporaryCachePath, "paciente.txt");
    public string rutaArchivoFisioterapia => Path.Combine(Application.temporaryCachePath, "fisioterapia.txt");
    public string rutaArchivoPredeterminado => Path.Combine(Application.temporaryCachePath, "predeterminado.txt");
    public string rutaArchivoRecompensa => Path.Combine(Application.temporaryCachePath, "recompensa.txt");
    public string rutaArchivoPersonalizacion => Path.Combine(Application.temporaryCachePath, "personalizacion.txt");
    public string rutaArchivoResultados => Path.Combine(Application.temporaryCachePath, "resultados.txt");

    [Header("BOOLEAN")]
    public bool m_playing = false;
    public bool playing
    {
        get { return m_playing; }
        set { m_playing = value; }
    }

    public bool m_resting = false;
    public bool resting
    {
        get { return m_resting; }
        set { m_resting = value; }
    }

    public bool m_apnea = false;
    public bool apnea
    {
        get { return m_apnea; }
        set { m_apnea = value; }
    }

    public bool m_inspiration = false;
    public bool inspiration
    {
        get { return m_inspiration; }
        set { m_inspiration = value; }
    }

    [Header("ID")]

    public int m_idJsonObjectExercises;
    public int idJsonObjectExercises
    {
        get { return m_idJsonObjectExercises; }
        set { m_idJsonObjectExercises = value; }
    }

    public int m_idListHourExercises;
    public int idListHourExercises
    {
        get { return m_idListHourExercises; }
        set { m_idListHourExercises = value; }
    }

    [Header("JSON")]
    public Data m_jsonObjectUser;
    public Data jsonObjectUser
    {
        get { return m_jsonObjectUser; }
        set { m_jsonObjectUser = value; }
    }

    public Exercise m_jsonObjectExerciseDefault;
    public Exercise jsonObjectExerciseDefault
    {
        get { return m_jsonObjectExerciseDefault; }
        set { m_jsonObjectExerciseDefault = value; }
    }

    public List<Exercise> m_jsonObjectExercises;
    public List<Exercise> jsonObjectExercises
    {
        get { return m_jsonObjectExercises; }
        set { m_jsonObjectExercises = value; }
    }

    public int[] m_exerciseHourArray;
    public int[] exerciseHourArray
    {
        get { return m_exerciseHourArray; }
        set { m_exerciseHourArray = value; }
    }

    public List<ExerciseData> m_exerciseSeries;
    public List<ExerciseData> exerciseSeries
    {
        get { return m_exerciseSeries; }
        set { m_exerciseSeries = value; }
    }

    public Rewards m_jsonObjectRewards;
    public Rewards jsonObjectRewards
    {
        get { return m_jsonObjectRewards; }
        set { m_jsonObjectRewards = value; }
    }

    public Customizations m_jsonObjectCustomizations;
    public Customizations jsonObjectCustomizations
    {
        get { return m_jsonObjectCustomizations; }
        set { m_jsonObjectCustomizations = value; }
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;     
    }

    void Start()
    {
        string[] archivosPersistentData = Directory.GetFiles(Application.persistentDataPath);
        foreach (string archivo in archivosPersistentData)
        {
            File.Delete(archivo);
        }

        idListHourExercises = -1;

        if(PlayerPrefs.GetString("currentExerciseDate") == "") // fecha actual
            PlayerPrefs.SetString("currentExerciseDate", DateTime.Today.ToString("dd/MM/yyyy"));
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ExitApp();
        if(playing)
        {
            //bool de apnea
            if(apnea && !PlayerMovement.Instance.apneaBool)
                PlayerMovement.Instance.apneaBool = true;
                        
            // contador de inactividad
            Obstacles.Instance.DetectInactivity();

            PlayerMovement.Instance.Movement();
            StartCoroutine(Obstacles.Instance.ObstaclesCounter());
        }

        if(resting)
            PlayerMovement.Instance.RestingPlayer();
        
        // seleccionar sesion disponible
        if(ExercisesManager.Instance.sessionMenu.gameObject.GetComponent<CanvasGroup>().alpha != 0)
        {
            for(int i = 0; i < exerciseHourArray.Length; i++)
            {
                int horaActual = int.Parse(DateTime.Now.Hour.ToString(CultureInfo.InvariantCulture));
                int minutoActual = int.Parse(DateTime.Now.Minute.ToString(CultureInfo.InvariantCulture));
                
                // detectar cual ejercicio se debe activar
                if(horaActual == exerciseHourArray[i] && minutoActual <= ExercisesManager.Instance.extraMinuteToWaitForExercise)
                {
                    ExercisesManager.Instance.sessionPrefab[i].GetComponent<Button>().interactable = true;
                    ExercisesManager.Instance.sessionPrefab[i].GetComponent<Image>().sprite = ExercisesManager.Instance.currentSessionSprite;
                    // almacenar el id del ejercicio activado
                    idListHourExercises = i;
                }
                
                if ((exerciseHourArray[i] < horaActual) || (horaActual == exerciseHourArray[i] && minutoActual > ExercisesManager.Instance.extraMinuteToWaitForExercise))
                {
                    ExercisesManager.Instance.sessionPrefab[i].GetComponent<Button>().interactable = false;
                    
                    // pregunta si ya finalizó los ejercicios pasados
                    if (exerciseHourArray[i] == 0)
                        ExercisesManager.Instance.sessionPrefab[i].GetComponent<Image>().sprite = ExercisesManager.Instance.finishedSessionSprite;
                    // pregunta si esta disponible los ejercicios pasados y coloca que no se finalizó
                    else
                        ExercisesManager.Instance.sessionPrefab[i].GetComponent<Image>().sprite = ExercisesManager.Instance.notFinishedSessionSprite;
                }         
            }
        }

        // detectar cuando lanzar sonido de motivacion
        if(ExercisesManager.Instance.exerciseMenu_Game.gameObject.GetComponent<CanvasGroup>().alpha != 0)
        {
            SoundsManager.Instance.StopMotivationSound();
            SoundsManager.Instance.StopSignalSound();

            if(inspiration && !PlayerMovement.Instance.apneaBool)
                StartCoroutine(SoundsManager.Instance.PlayMotivationSound());
            
            if(!inspiration && PlayerMovement.Instance.apneaBool)
                SoundsManager.Instance.AddSound();
        }
    }

    public void UpdateLocalUser(string jsonData)
    {  
        File.WriteAllText(rutaArchivoPaciente, jsonData);

        Debug.Log("Datos de paciente locales actualizados correctamente");
    } 

    public void SaveLocalData()
    {
        ExercisesManager.Instance.SaveExercise();

        PlayerPrefs.Save();
    }

    public void ExitApp()
    {
        // notificacion de salir
        NotificationsManager.Instance.QuestionNotifications("Quieres salir de la aplicación?");
        // si
        NotificationsManager.Instance.SetYesButton(()=>{
            SaveLocalData();
            Application.Quit();
        });
    }

    void OnApplicationQuit()
    {
        ExitApp();
    }
}
