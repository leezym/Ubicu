using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class Login : MonoBehaviour
{
    [Header("ATTACHED")]
    public TMP_InputField userInputField;
    public TMP_InputField passInputField;
    public Button loginButton;

    public void LogIn(){
        StartCoroutine(OnLogin());
    }

    public IEnumerator OnLogin()
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
