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

            string responseText = www.downloadHandler.text;
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log(form.data);

                userInputField.text = responseText;
                passInputField.text = "";
            }
            else
            {
                GameData.Instance.jsonObjectUser = JsonUtility.FromJson<Data>(responseText);
                Debug.Log("Response token:" + GameData.Instance.jsonObjectUser.token);
                Debug.Log("Response name:" + GameData.Instance.jsonObjectUser.user.nombre);
                StopCoroutine(OnLogin());
                StartCoroutine(GameData.Instance.scriptsGroup.exercisesManager.GetExercises());
                UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);

            }
        }
        else
            Debug.Log("llena los datos");
    }
}
