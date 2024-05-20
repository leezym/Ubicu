using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Globalization;
using Newtonsoft.Json;

public class CalibrationsManager : MonoBehaviour
{
    public IEnumerator SendCalibrations()
    {
        WWWForm form = new WWWForm();
        string json = JsonConvert.SerializeObject(GameData.Instance.exerciseSeries);

        form.AddField("fecha", DateTime.Today.ToString("dd/MM/yyyy"));
        form.AddField("hora", int.Parse(DateTime.Now.Hour.ToString(CultureInfo.InvariantCulture)));
        form.AddField("minutos", int.Parse(DateTime.Now.Minute.ToString(CultureInfo.InvariantCulture)));
        form.AddField("datos", json);

        UnityWebRequest www = UnityWebRequest.Post(GameData.URL+"createCalibration", form);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
            Debug.Log(form.data);
        }
        else
        {
            GameData.Instance.exerciseSeries = new ExerciseData();
        }
    }
}
