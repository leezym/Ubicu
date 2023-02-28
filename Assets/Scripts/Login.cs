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

    [Header("IN GAME")]
    public Data jsonObject;

    public void LogIn(){
        StartCoroutine(OnLogin());
    }

    public IEnumerator OnLogin()
    {
        WWWForm form = new WWWForm();
        if(userInputField.text != "" && passInputField.text != "")
        {
            form.AddField("cedula", userInputField.text);
            form.AddField("password", passInputField.text);

            UnityWebRequest www = UnityWebRequest.Post("http://localhost:5000/authenticatePatient", form);
            //UnityWebRequest www = UnityWebRequest.Post("https://server.ubicu.co/authenticatePatient", form);

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(form.data);

                userInputField.text = www.error;
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

                UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);

                StopCoroutine(OnLogin());
                StartCoroutine(GameData.Instance.scriptsGroup.exercisesManager.GetExercises());
                UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);

            }
        }
    }
}
