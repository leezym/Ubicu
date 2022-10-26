using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BluetoothPairing : MonoBehaviour
{
    AndroidJavaObject bluet;
    AndroidJavaObject alert;
    AndroidJavaClass unityPlayerClass;
    AndroidJavaObject unityActivity;
    public float prom = 0;
    public float timer = 0;

    public GameObject bluetoothContent, prefabTextBluetooth;
    public GameObject loginMenu, bluetoothMenu, loadingScreen;
    object[] parameters2 = new object[2];
    
    void Start()
    {
        CallNativePlugin();
    }

    public void Refresh(){
        CallNativePlugin();
    }

    //method that calls our native plugin.
    public void CallNativePlugin()
    {
        Debug.Log("Pairing");
        unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        bluet = new AndroidJavaObject("com.millerbsv.bluetoothhc.BluetoothT");
        string data = bluet.Call<string>("getDevices");

        if (data != null)
        {
            string[] subs = data.Split(';');
            Debug.LogWarningFormat("Devices: {0}", data);

            foreach(string s in subs)
            {                
                GameObject go = Instantiate(prefabTextBluetooth, Vector3.zero, Quaternion.identity);
                go.name = s.Substring(s.Length - 17, 17);
                go.transform.parent = bluetoothContent.transform;
                
                TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
                text.text = s.Substring(1,s.Length - 24);

                go.GetComponent<Button>().onClick.AddListener(()=>{
                    parameters2[0] = 4;
                    parameters2[1] = go.name;
                    StartCoroutine(LoadingScreen());
                });
            }
        }
    }
    void OutputTime()
    {
        
            string data = bluet.Call<string>("getData");
            Debug.LogWarningFormat("data: {0}", data);
            
            string[] strings = Regex.Split(data, Environment.NewLine);
            List<float> valuesList = new List<float>();
            float tempValue;
            for (int i = 1; i < strings.Length-1; i++)
            {
                if(float.TryParse(strings[i], out tempValue))
                {
                    valuesList.Add(tempValue);
                } else
                {
                    Console.WriteLine("error parse data {0}", strings[i]);
                }
            }
            float[] valuesDouble = valuesList.ToArray();

            if(valuesDouble.Length > 0)
            {
                prom = valuesDouble.Average();
                if(prom < 1)
                    prom = 0;
                //Debug.LogWarningFormat("promedio: {0}", prom.ToString());
                timer += Time.deltaTime;
            }
    }

    IEnumerator LoadingScreen(){
        bluetoothMenu.SetActive(false);
        loadingScreen.SetActive(true);
        bluet.Call("connectToDevice", parameters2);
        yield return new WaitForSeconds(3f);
        loadingScreen.SetActive(false);
        loginMenu.SetActive(true);
        StopCoroutine(LoadingScreen());
    }    

    public void CallOutputTime(){
        InvokeRepeating("OutputTime", 2f, 0.002f); ///pdte detener cuando no se juega
    }
}
