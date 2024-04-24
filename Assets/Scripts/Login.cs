using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.IO;
using Newtonsoft.Json;


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
            if (File.Exists(GameData.Instance.rutaArchivoPaciente) && File.Exists(GameData.Instance.rutaArchivoFisioterapia) && File.Exists(GameData.Instance.rutaArchivoRecompensa) && File.Exists(GameData.Instance.rutaArchivoPersonalizacion))
            {
                string json = File.ReadAllText(GameData.Instance.rutaArchivoPaciente);
                //GameData.Instance.jsonObjectUser = JsonUtility.FromJson<Data>(json);
                GameData.Instance.jsonObjectUser.user = JsonUtility.FromJson<User>(json);


                /*if(isGuest)
                    GameData.Instance.jsonObjectUser.user = JsonUtility.FromJson<User>(json);
                else
                    GameData.Instance.jsonObjectUser = JsonUtility.FromJson<Data>(json);       */
                    Debug.Log(GameData.Instance.jsonObjectUser.user.cedula);

                if(userInputField.text == GameData.Instance.jsonObjectUser.user.cedula)
                {
                    GameData.Instance.jsonObjectExercises = JsonConvert.DeserializeObject<Exercises>("{\"array\": [" + File.ReadAllText(GameData.Instance.rutaArchivoFisioterapia) + "] }");
                    GameData.Instance.scriptsGroup.exercisesManager.CreateExercises();

                    GameData.Instance.jsonObjectRewards = JsonConvert.DeserializeObject<Rewards>(File.ReadAllText(GameData.Instance.rutaArchivoRecompensa));
                    GameData.Instance.scriptsGroup.rewardsManager.GetAllBadges();

                    GameData.Instance.jsonObjectCustomizations = JsonConvert.DeserializeObject<Customizations>(File.ReadAllText(GameData.Instance.rutaArchivoPersonalizacion));
                    GameData.Instance.scriptsGroup.customizationManager.CreateCustomizations();

                    GameData.Instance.jsonObjectUser.user.cedula = userInputField.text;

                    yield return new WaitForSeconds(1f);
                    UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);
                }
                else
                    NotificationsManager.Instance.WarningNotifications("El usuario no coincide con la fisioterapia asignada");
            }
            else
            {
                /*if(isGuest)
                    NotificationsManager.Instance.WarningNotifications("¿Cómo obtiene datos? ¿Terapia estandar?");             
                else*/
                    NotificationsManager.Instance.WarningNotifications("¡No tienes terapias! Por favor inicia sesión con usuario y contraseña para descargar la terapia");
            }
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
            //UnityWebRequest www = UnityWebRequest.Post("http://localhost:5000/authenticatePatient", form);

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

                //Subir data local primero
                if (File.Exists(GameData.Instance.rutaArchivoPaciente) && File.Exists(GameData.Instance.rutaArchivoFisioterapia) && File.Exists(GameData.Instance.rutaArchivoRecompensa) && File.Exists(GameData.Instance.rutaArchivoPersonalizacion))
                {
                    StartCoroutine(GameData.Instance.scriptsGroup.rewardsManager.UpdateReward(GameData.Instance.rutaArchivoRecompensa));
                    StartCoroutine(GameData.Instance.scriptsGroup.customizationManager.UpdateCustomizations(GameData.Instance.rutaArchivoPersonalizacion));
                    StartCoroutine(GameData.Instance.scriptsGroup.exercisesManager.UpdateResults(GameData.Instance.rutaArchivoResultados));

                    //Load();              
                }
                else
                {
                    Load();

                    /*GameData.Instance.UpdateLocalUser(JsonConvert.SerializeObject(GameData.Instance.jsonObjectUser));
                    Debug.Log(GameData.Instance.jsonObjectExercises.array.Count);
                    Debug.Log(GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises]);
                    
                */}

                Debug.Log(GameData.Instance.jsonObjectExercises.array.Count);
                Debug.Log(GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises]);
                
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

    public IEnumerator Load()
    {
        yield return StartCoroutine(GameData.Instance.scriptsGroup.exercisesManager.GetExercises());
        yield return StartCoroutine(GameData.Instance.scriptsGroup.rewardsManager.GetRewards());
        yield return StartCoroutine(GameData.Instance.scriptsGroup.customizationManager.GetCustomizations());

        GameData.Instance.scriptsGroup.exercisesManager.UpdateLocalExercise(JsonConvert.SerializeObject(GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises]));
        GameData.Instance.scriptsGroup.rewardsManager.UpdateLocalReward(GameData.Instance.scriptsGroup.rewardsManager.GetJsonRewards());
        GameData.Instance.scriptsGroup.customizationManager.UpdateLocalCustomizations(GameData.Instance.scriptsGroup.customizationManager.GetJsonCustomizations());
}
}
