using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;

public class Login : MonoBehaviour
{
    public static Login Instance {get; private set;}

    [Header("ATTACHED")]
    public UI_Screen loginMenu;
    public TMP_InputField userInputField;
    public TMP_InputField passInputField;
    public Button loginButton;
    public Toggle notInternet;
    public GameObject passTitle;

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        loginButton.GetComponent<Button>().onClick.AddListener(()=> {
            StartCoroutine(OnLogin());
        });
    }

    public void ConexionMode()
    {
        if(notInternet.isOn)
        {
            passTitle.SetActive(false);
            loginButton.onClick.RemoveAllListeners();
            loginButton.GetComponent<Button>().onClick.AddListener(()=>{
                StartCoroutine(LocalLogin());
            });            
        }
        else
        {
            passTitle.SetActive(true);
            loginButton.onClick.RemoveAllListeners();
            loginButton.GetComponent<Button>().onClick.AddListener(()=>{
                StartCoroutine(OnLogin());
            });
        }
    }

    public IEnumerator LocalLogin()
    {
        bool active;
        Exercise exercise = new Exercise();

        if(!string.IsNullOrEmpty(userInputField.text))
        {
            if (File.Exists(GameData.Instance.ObtenerRutaPaciente(userInputField.text)))
            {
                GameData.Instance.jsonObjectUser = JsonConvert.DeserializeObject<Data>(File.ReadAllText(GameData.Instance.ObtenerRutaPaciente(userInputField.text)));

                if(userInputField.text == GameData.Instance.jsonObjectUser.user.cedula)
                {
                    if(File.Exists(GameData.Instance.ObtenerRutaFisioterapia(GameData.Instance.jsonObjectUser.user.cedula)))
                    {
                        exercise = JsonConvert.DeserializeObject<Exercise>(File.ReadAllText(GameData.Instance.ObtenerRutaFisioterapia(GameData.Instance.jsonObjectUser.user.cedula)));

                        active = (DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) >= DateTime.ParseExact(exercise.fecha_inicio, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                        && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) <= DateTime.ParseExact(exercise.fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture)) ? true : false;
                    }
                    else
                    {
                        active = false;
                    }
                    
                    GameData.Instance.jsonObjectExerciseDefault = JsonConvert.DeserializeObject<Exercise>(File.ReadAllText(GameData.Instance.ObtenerRutaPredeterminado(GameData.Instance.jsonObjectUser.user.cedula)));

                    if(active)
                    {
                        LoadLocalData(exercise);

                        yield return new WaitForSeconds(1f);
                        UI_System.Instance.SwitchScreens(ExercisesManager.Instance.sessionMenu);
                    }
                    else if(!active)
                    {
                        if(string.IsNullOrEmpty(GameData.Instance.jsonObjectExerciseDefault.nombre))
                        {
                            NotificationsManager.Instance.WarningNotifications("¡No tienes un ejercicio predeterminado! Por favor dile a tu fisioterapeuta que te cree uno.\nEste ejercicio te permite trabajar sin conexión a internet.");
                        }
                        else
                        {
                            if(GameData.Instance.jsonObjectExerciseDefault.nombre != exercise.nombre)
                            {
                                exercise = GameData.Instance.jsonObjectExerciseDefault;
                                exercise._id = "";
                                exercise.fecha_inicio = DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                                exercise.fecha_fin = DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(exercise.frecuencia_dias - 1).ToString("dd/MM/yyyy");

                                LoadLocalData(exercise);

                                yield return new WaitForSeconds(1f);
                                UI_System.Instance.SwitchScreens(ExercisesManager.Instance.sessionMenu);
                            }
                            else if(GameData.Instance.jsonObjectExerciseDefault.nombre == exercise.nombre)
                            {
                                NotificationsManager.Instance.WarningNotifications("¡No tienes terapias! Por favor inicia sesión con usuario y contraseña para descargar la terapia");
                            }
                        }
                    }
                } 
                else
                    NotificationsManager.Instance.WarningNotifications("El usuario no coincide con la fisioterapia asignada");
            }
            else
                NotificationsManager.Instance.WarningNotifications("¡No tienes terapias! Por favor inicia sesión con usuario y contraseña para descargar la terapia");
        }
        else
            NotificationsManager.Instance.WarningNotifications("Ingresa los datos");
    }

    IEnumerator OnLogin()
    {
        loginButton.interactable = false;

        WWWForm form = new WWWForm();
        
        if(!string.IsNullOrEmpty(userInputField.text) && !string.IsNullOrEmpty(passInputField.text))
        {
            form.AddField("cedula", userInputField.text);
            form.AddField("password", passInputField.text);

            UnityWebRequest www = UnityWebRequest.Post(GameData.URL+"authenticatePatient", form);

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            string responseText = www.downloadHandler.text;
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                Debug.Log(form.data);

                if(!string.IsNullOrEmpty(responseText))
                    NotificationsManager.Instance.WarningNotifications(responseText.Replace('"', ' '));
                else
                    NotificationsManager.Instance.WarningNotifications("No tienes conexión a internet");

                userInputField.text = "";
                passInputField.text = "";
            }
            else
            {
                GameData.Instance.jsonObjectUser = JsonConvert.DeserializeObject<Data>(responseText);
                UpdateLocalUser(responseText);
            
                if(File.Exists(GameData.Instance.ObtenerRutaResultados(GameData.Instance.jsonObjectUser.user.cedula)))
                {
                    Exercise exercise = JsonConvert.DeserializeObject<Exercise>(File.ReadAllText(GameData.Instance.ObtenerRutaFisioterapia(GameData.Instance.jsonObjectUser.user.cedula)));
                    Exercise exerciseDefault = JsonConvert.DeserializeObject<Exercise>(File.ReadAllText(GameData.Instance.ObtenerRutaPredeterminado(GameData.Instance.jsonObjectUser.user.cedula)));

                    if(string.IsNullOrEmpty(exerciseDefault.nombre))
                        NotificationsManager.Instance.WarningNotifications("¡No tienes un ejercicio predeterminado! Por favor dile a tu fisioterapeuta que te cree uno.\nEste ejercicio te permite trabajar sin conexión a internet.");

                    if(exerciseDefault.nombre == exercise.nombre)
                        yield return StartCoroutine(ExercisesManager.Instance.CreateDefaultExercise(exercise));
                    else if(exerciseDefault.nombre != exercise.nombre)
                        yield return StartCoroutine(ExercisesManager.Instance.CreateResults());

                    yield return StartCoroutine(ExercisesManager.Instance.UpdateExerciseDate(File.ReadAllText(GameData.Instance.ObtenerRutaFechaEjercicio(GameData.Instance.jsonObjectUser.user.cedula))));
                    yield return StartCoroutine(RewardsManager.Instance.UpdateReward(File.ReadAllText(GameData.Instance.ObtenerRutaRecompensa(GameData.Instance.jsonObjectUser.user.cedula))));
                    yield return StartCoroutine(CustomizationManager.Instance.UpdateCustomizations(File.ReadAllText(GameData.Instance.ObtenerRutaPersonalizacion(GameData.Instance.jsonObjectUser.user.cedula))));
                }

                yield return StartCoroutine(ExercisesManager.Instance.GetExerciseDate());
                yield return StartCoroutine(ExercisesManager.Instance.GetExercises());
                yield return StartCoroutine(RewardsManager.Instance.GetRewards());
                yield return StartCoroutine(CustomizationManager.Instance.GetCustomizations());
                
                yield return new WaitForSeconds(1f);
                UI_System.Instance.SwitchScreens(ExercisesManager.Instance.sessionMenu);
            }

            yield return new WaitForSeconds(2f);
            loginButton.interactable = true;
        }
        else
        {
            loginButton.interactable = true;
            NotificationsManager.Instance.WarningNotifications("Ingresa los datos");
        }
    }

    public void UpdateLocalUser(string jsonData)
    {
        File.WriteAllText(GameData.Instance.ObtenerRutaPaciente(GameData.Instance.jsonObjectUser.user.cedula), jsonData);

        Debug.Log("Datos del paciente locales actualizados correctamente");
    }

    void LoadLocalData(Exercise exercise)
    {
        string e = JsonConvert.SerializeObject(exercise);

        GameData.Instance.jsonObjectExerciseDate = JsonConvert.DeserializeObject<ExerciseDate>(File.ReadAllText(GameData.Instance.ObtenerRutaFechaEjercicio(GameData.Instance.jsonObjectUser.user.cedula)));

        ExercisesManager.Instance.UpdateLocalExercise(GameData.Instance.ObtenerRutaFisioterapia(GameData.Instance.jsonObjectUser.user.cedula), e);
        GameData.Instance.jsonObjectExercises = JsonConvert.DeserializeObject<List<Exercise>>("["+File.ReadAllText(GameData.Instance.ObtenerRutaFisioterapia(GameData.Instance.jsonObjectUser.user.cedula))+"]");
        ExercisesManager.Instance.CreateExercisesSesions();

        GameData.Instance.jsonObjectRewards = JsonConvert.DeserializeObject<Rewards>(File.ReadAllText(GameData.Instance.ObtenerRutaRecompensa(GameData.Instance.jsonObjectUser.user.cedula)));
        RewardsManager.Instance.GetAllBadges();

        GameData.Instance.jsonObjectCustomizations = JsonConvert.DeserializeObject<Customizations>(File.ReadAllText(GameData.Instance.ObtenerRutaPersonalizacion(GameData.Instance.jsonObjectUser.user.cedula)));
        CustomizationManager.Instance.CreateCustomizations();
    }
}