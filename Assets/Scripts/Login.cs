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
    public UI_Screen sessionMenu;

    [Header("IN GAME")]
    public ScriptsGroup scriptsGroup;
    public Data jsonObject;

    public void Start()
    {
        scriptsGroup = FindObjectOfType<ScriptsGroup>();
    }

    public void LogIn(){
        StartCoroutine(OnLogin());
    }

    public IEnumerator OnLogin()
    {
        WWWForm form = new WWWForm();
        form.AddField("cedula", userInputField.text);
        form.AddField("password", passInputField.text);

        UnityWebRequest www = UnityWebRequest.Post("http://d2yaaz8bde1qj3.cloudfront.net/AuthenticateUser", form);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            Debug.Log(form.data);

            userInputField.text = "ERROR";
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

            UI_System uI_System = FindObjectOfType<UI_System>();
            uI_System.SwitchScreens(sessionMenu);

            StopCoroutine(OnLogin());
            StartCoroutine(scriptsGroup.exercisesManager.GetExercises());
        }
    }
}
