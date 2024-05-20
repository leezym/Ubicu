using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

public class BluetoothPairing : MonoBehaviour
{
    AndroidJavaObject bluet;
    AndroidJavaObject alert;
    AndroidJavaClass unityPlayerClass;
    AndroidJavaObject unityActivity;
    object[] parameters2 = new object[2];

    [Header("ATTACHED")]
    public GameObject[] prefabTextBluetooth;
    public GameObject bluetoothContent;

    //test
    public TMP_Text stringConnection;

    [Header("IN GAME")]
    public float flow = 0;
    public float vol = 0; //ISABELLA
    public float timer = 0;
    
    void Start()
    {
        //GameData.Instance.StartToAdd(0, 0); //JUAN DAVID
        GameData.Instance.StartToAdd(0, 0, 0); //ISABELLA
        CallNativePlugin();
    }

    public void Refresh(){
        foreach(GameObject g in prefabTextBluetooth)
        {
            g.SetActive(false);
            g.GetComponentInChildren<TextMeshProUGUI>().text = "";
            g.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        CallNativePlugin();
    }

    //method that calls our native plugin.
    public void CallNativePlugin()
    {
        unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        bluet = new AndroidJavaObject("com.millerbsv.bluetoothhc.BluetoothT");
        string data = bluet.Call<string>("getDevices");
        if (data != null)
        {
            string[] devices = data.Split(';');
            for(int i = 0; i < devices.Length-1; i++)
            {
                string device = devices[i];
                prefabTextBluetooth[i].SetActive(true);
                TextMeshProUGUI text = prefabTextBluetooth[i].GetComponentInChildren<TextMeshProUGUI>();
                text.text = device.Substring(1,device.Length - 24); //NOMBRE

                prefabTextBluetooth[i].GetComponent<Button>().onClick.AddListener(()=>{
                    parameters2[0] = 4;
                    parameters2[1] = device.Substring(device.Length - 17, 17); //MAC
                    StartCoroutine(LoadingScreen());
                });
            }
        }
    }
    
    /*public void OutputTime() //JUAN DAVID
    {
        timer += Time.deltaTime;
        
        string data = bluet.Call<string>("getData");
        string[] patientData = Regex.Split(data, ",");
        // patientData[0] -> flujo
        // patientData[1] -> apnea
        // patientData[2] -> frecuencia respiratoria
        
        if(float.TryParse(patientData[0], NumberStyles.Any, CultureInfo.InvariantCulture, out flow))
        {
            GameData.Instance.StartToAdd(flow, timer);
        }
        else
        {
            Console.WriteLine("error parse data {0}", patientData[0]);
        }
    }*/

    public void OutputTime() //ISABELLA
    {
        timer += Time.deltaTime;
        
        string data = bluet.Call<string>("getData");
        string[] patientData = Regex.Split(data, ",");
        // patientData[0] -> flujo
        // patientData[1] -> volumen
    
        
        if(float.TryParse(patientData[0], NumberStyles.Any, CultureInfo.InvariantCulture, out flow) && float.TryParse(patientData[1], NumberStyles.Any, CultureInfo.InvariantCulture, out vol))
        {
            GameData.Instance.StartToAdd(flow, vol, timer);
        }
        else
        {
            Console.WriteLine("error parse data {0}", patientData[0]);
            Console.WriteLine("error parse data {1}", patientData[1]);
        }
    }

    IEnumerator LoadingScreen(){
        yield return new WaitForSeconds(0.5f);
       bluet.Call("connectToDevice", parameters2);
        yield return new WaitForSeconds(3f);
        UI_System.Instance.SwitchScreens(GameData.Instance.dataMenu);       
    }

    public void CallOutputTime()
    {
        InvokeRepeating("OutputTime", 0.5f, 0.05f); // 1/20 datos
    }

    public void StopOutputTime()
    {
        CancelInvoke();
    }
}
