using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class Login : MonoBehaviour
{
    public void StartToRead(){
        StartCoroutine(OnLogin());
    }

    public IEnumerator OnLogin() //pdte revisar
    {
        GameData.Instance.startButton.interactable = false;
        GameData.Instance.dataText.text = "";

        UnityWebRequest www = UnityWebRequest.Get(GameData.URL+"verifyConnection");
        //UnityWebRequest www = UnityWebRequest.Get("http://localhost:5000/verifyConnection");

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        string responseText = www.downloadHandler.text;
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);

            if(responseText != "")
                NotificationsManager.Instance.WarningNotifications(responseText.Replace('"', ' '));
            else
                NotificationsManager.Instance.WarningNotifications("No tienes conexión a internet");
            
            yield return new WaitForSeconds(2f);
            GameData.Instance.startButton.interactable = true;
        }
        else
        {
            GameData.Instance.scriptsGroup.bluetoothPairing.CallOutputTime();
            GameData.Instance.stopButton.interactable = true;
        }
    }
}
