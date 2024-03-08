using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.IO;

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
            string rutaArchivo = Path.Combine(Application.persistentDataPath, "localData_"+userInputField.text+".json");

            if (File.Exists(rutaArchivo))
            {
                string json = File.ReadAllText(rutaArchivo);
                GameData.Instance.jsonObjectUser.token = "0";
                GameData.Instance.jsonObjectUser.user = JsonUtility.FromJson<User>(json);
                yield return new WaitForSeconds(1f);
                UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);
            }
            else
            {
                GameData.Instance.jsonObjectUser.user.cedula = userInputField.text;
                yield return new WaitForSeconds(1f);
                UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);
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
