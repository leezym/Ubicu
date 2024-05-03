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
    [Header("ATTACHED")]
    public TMP_InputField userInputField;
    public TMP_InputField passInputField;
    public Button loginButton;
    public Toggle notInternet;
    public GameObject passTitle;

    void Start()
    {
        loginButton.GetComponent<Button>().onClick.AddListener(()=>{
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

    IEnumerator LocalLogin()
    {
        if(userInputField.text != "")
        {
            if (File.Exists(GameData.Instance.rutaArchivoFisioterapia) && File.Exists(GameData.Instance.rutaArchivoPredeterminado))
            {
                GameData.Instance.jsonObjectUser = JsonUtility.FromJson<Data>(File.ReadAllText(GameData.Instance.rutaArchivoPaciente));
                GameData.Instance.jsonObjectExerciseDefault = JsonUtility.FromJson<Exercise>(File.ReadAllText(GameData.Instance.rutaArchivoPredeterminado));

                Exercise exercise = JsonUtility.FromJson<Exercise>(File.ReadAllText(GameData.Instance.rutaArchivoFisioterapia));

                if(File.Exists(GameData.Instance.rutaArchivoPaciente) && userInputField.text == GameData.Instance.jsonObjectUser.user.cedula)
                {
                    bool active = (DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) >= DateTime.ParseExact(exercise.fecha_inicio, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                        && DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture) <= DateTime.ParseExact(exercise.fecha_fin, "dd/MM/yyyy", CultureInfo.InvariantCulture)) ? true : false;

                    if(active)
                    {
                        LoadLocalData(exercise);

                        yield return new WaitForSeconds(1f);
                        UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);
                    }
                    else if(!active && GameData.Instance.jsonObjectExerciseDefault.nombre != exercise.nombre)
                    {
                        exercise = JsonUtility.FromJson<Exercise>(File.ReadAllText(GameData.Instance.rutaArchivoPredeterminado));

                        exercise._id = "";
                        exercise.fecha_inicio = DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                        exercise.fecha_fin = DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(exercise.frecuencia_dias - 1).ToString("dd/MM/yyyy");

                        LoadLocalData(exercise);

                        yield return new WaitForSeconds(1f);
                        UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);
                    }
                    else if(!active && GameData.Instance.jsonObjectExerciseDefault.nombre == exercise.nombre)
                        NotificationsManager.Instance.WarningNotifications("¡No tienes terapias! Por favor inicia sesión con usuario y contraseña para descargar la terapia");
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
        
        if(userInputField.text != "" && passInputField.text != "")
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

                if(responseText != "")
                    NotificationsManager.Instance.WarningNotifications(responseText.Replace('"', ' '));
                else
                    NotificationsManager.Instance.WarningNotifications("No tienes conexión a internet");

                userInputField.text = "";
                passInputField.text = "";
            }
            else
            {
                GameData.Instance.jsonObjectUser = JsonUtility.FromJson<Data>(responseText);
                
                if(File.Exists(GameData.Instance.rutaArchivoFisioterapia) && File.Exists(GameData.Instance.rutaArchivoPredeterminado) && File.Exists(GameData.Instance.rutaArchivoResultados))
                {
                    Exercise exercise = JsonUtility.FromJson<Exercise>(File.ReadAllText(GameData.Instance.rutaArchivoFisioterapia));
                    GameData.Instance.jsonObjectExerciseDefault = JsonUtility.FromJson<Exercise>(File.ReadAllText(GameData.Instance.rutaArchivoPredeterminado));

                    if(GameData.Instance.jsonObjectExerciseDefault.nombre == exercise.nombre)
                        yield return StartCoroutine(GameData.Instance.scriptsGroup.exercisesManager.CreateDefaultExercise(exercise));
                    else                    
                        yield return StartCoroutine(GameData.Instance.scriptsGroup.exercisesManager.CreateResults());
                }

                yield return StartCoroutine(GameData.Instance.scriptsGroup.rewardsManager.UpdateReward(File.ReadAllText(GameData.Instance.rutaArchivoRecompensa)));
                yield return StartCoroutine(GameData.Instance.scriptsGroup.customizationManager.UpdateCustomizations(File.ReadAllText(GameData.Instance.rutaArchivoPersonalizacion)));

                yield return StartCoroutine(GameData.Instance.scriptsGroup.exercisesManager.GetExercises());
                yield return StartCoroutine(GameData.Instance.scriptsGroup.rewardsManager.GetRewards());
                yield return StartCoroutine(GameData.Instance.scriptsGroup.customizationManager.GetCustomizations());

                UpdateLocalUser(JsonConvert.SerializeObject(GameData.Instance.jsonObjectUser));
                if(GameData.Instance.idJsonObjectExercises != -1)
                    GameData.Instance.scriptsGroup.exercisesManager.UpdateLocalExercise(GameData.Instance.rutaArchivoFisioterapia, GameData.Instance.scriptsGroup.exercisesManager.GetJsonExercise(GameData.Instance.jsonObjectExercises[GameData.Instance.idJsonObjectExercises]));
                GameData.Instance.scriptsGroup.exercisesManager.UpdateLocalExercise(GameData.Instance.rutaArchivoPredeterminado, GameData.Instance.scriptsGroup.exercisesManager.GetJsonExercise(GameData.Instance.jsonObjectExerciseDefault));
                GameData.Instance.scriptsGroup.rewardsManager.UpdateLocalReward(GameData.Instance.scriptsGroup.rewardsManager.GetJsonRewards());
                GameData.Instance.scriptsGroup.customizationManager.UpdateLocalCustomizations(GameData.Instance.scriptsGroup.customizationManager.GetJsonCustomizations());
                
                yield return new WaitForSeconds(1f);
                UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);
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
        File.WriteAllText(GameData.Instance.rutaArchivoPaciente, jsonData);

        Debug.Log("Datos del paciente locales actualizados correctamente");
    }

    void LoadLocalData(Exercise exercise)
    {
        string e = JsonConvert.SerializeObject(exercise);
        GameData.Instance.scriptsGroup.exercisesManager.UpdateLocalExercise(GameData.Instance.rutaArchivoFisioterapia, e);
        GameData.Instance.jsonObjectExercises = JsonConvert.DeserializeObject<List<Exercise>>("["+File.ReadAllText(GameData.Instance.rutaArchivoFisioterapia)+"]");
        GameData.Instance.scriptsGroup.exercisesManager.CreateExercisesSesions();

        GameData.Instance.jsonObjectRewards = JsonConvert.DeserializeObject<Rewards>(File.ReadAllText(GameData.Instance.rutaArchivoRecompensa));
        GameData.Instance.scriptsGroup.rewardsManager.GetAllBadges();

        GameData.Instance.jsonObjectCustomizations = JsonConvert.DeserializeObject<Customizations>(File.ReadAllText(GameData.Instance.rutaArchivoPersonalizacion));
        GameData.Instance.scriptsGroup.customizationManager.CreateCustomizations();
    }
}
