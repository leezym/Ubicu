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
            // PDTE VALIDAR COMO SUBIR SI EXISTEN DATOS LOCALES: OBTENER EL USUARIO, ACTUALIZAR LOS DATOS EN LA DB Y LEERLOS NUEVAMENTE 
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
            //string rutaArchivo = Path.Combine(Application.persistentDataPath, "localData_"+userInputField.text+".json");
            string rutaArchivoPaciente = Path.Combine(Application.persistentDataPath, "paciente.txt");
            string rutaArchivoFisioterapia = Path.Combine(Application.persistentDataPath, "fisioterapia.txt");
            string rutaArchivoRecompensa = Path.Combine(Application.persistentDataPath, "recompensa.txt");
            string rutaArchivoPersonalizacion = Path.Combine(Application.persistentDataPath, "personalizacion.txt");
            
            GameData.Instance.paciente.text = File.ReadAllText(rutaArchivoPaciente);
            GameData.Instance.fisioterapia.text = File.ReadAllText(rutaArchivoFisioterapia);
            GameData.Instance.recompensa.text = File.ReadAllText(rutaArchivoRecompensa);
            GameData.Instance.personalizacion.text = File.ReadAllText(rutaArchivoPersonalizacion);

            if (File.Exists(rutaArchivoPaciente) && File.Exists(rutaArchivoFisioterapia) && File.Exists(rutaArchivoRecompensa) && File.Exists(rutaArchivoPersonalizacion))
            {
                string json = File.ReadAllText(rutaArchivoPaciente);
                GameData.Instance.jsonObjectUser.token = "0";
                GameData.Instance.jsonObjectUser.user = JsonUtility.FromJson<User>(json);

                if(userInputField.text == GameData.Instance.jsonObjectUser.user.cedula)
                {
                    GameData.Instance.jsonObjectExercises = JsonConvert.DeserializeObject<Exercises>("{\"array\": [" + GameData.Instance.fisioterapia + "] }");
                    GameData.Instance.scriptsGroup.exercisesManager.CreateExercises();

                    GameData.Instance.jsonObjectRewards = JsonConvert.DeserializeObject<Rewards>(GameData.Instance.recompensa.ToString());
                    GameData.Instance.scriptsGroup.rewardsManager.GetAllBadges();

                    GameData.Instance.jsonObjectCustomizations = JsonConvert.DeserializeObject<Customizations>(GameData.Instance.personalizacion.ToString());
                    GameData.Instance.scriptsGroup.customizationManager.CreateCustomizations();

                    GameData.Instance.jsonObjectUser.user.cedula = userInputField.text;

                    yield return new WaitForSeconds(1f);
                    UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);
                }
                else
                    NotificationsManager.Instance.WarningNotifications("El usuario no coincide con la fisioterapia asignada");
            }
            else // como saber si es un usuario con cuenta en la nube? si tiene cuenta no le crea datos, sino tiene cuenta le crea datos vacios
            {
                
            }
        }
        else
            NotificationsManager.Instance.WarningNotifications("Ingresa los datos");

    }

    IEnumerator OnLogin()
    {
        //si hay data LOCAL se debe actualizar la DB primero y luego leer PDTE

        loginButton.interactable = false;

        WWWForm form = new WWWForm();
        if(userInputField.text != "" && passInputField.text != "")
        {
            form.AddField("cedula", userInputField.text);
            form.AddField("password", passInputField.text);

            //UnityWebRequest www = UnityWebRequest.Post("http://localhost:5000/authenticatePatient", form);
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
                yield return StartCoroutine(GameData.Instance.scriptsGroup.exercisesManager.GetExercises());
                yield return StartCoroutine(GameData.Instance.scriptsGroup.rewardsManager.GetRewards());
                yield return StartCoroutine(GameData.Instance.scriptsGroup.customizationManager.GetCustomizations());
                yield return new WaitForSeconds(1f);
                UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);
            }
            yield return new WaitForSeconds(2f);
            loginButton.interactable = true;

        }
        else{
            loginButton.interactable = true;
            NotificationsManager.Instance.WarningNotifications("Ingresa los datos");
        }

    }
}
