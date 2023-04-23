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
    public GameObject prefabTextBluetooth;
    public GameObject bluetoothContent;
    public UI_Screen login;

    //test
    public TMP_Text stringConnection;

    [Header("IN GAME")]
    public float prom = 0;
    public float timer = 0;
    public List<GameObject> bluetoothList;
    
    void Start()
    {
        CallNativePlugin();
    }

    public void Refresh(){
        foreach(GameObject g in bluetoothList)
            Destroy(g);
        bluetoothList = new List<GameObject>();
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
            Debug.LogWarningFormat("Devices: {0}", data);

            foreach(string s in devices)
            {                
                GameObject go = Instantiate(prefabTextBluetooth, Vector3.zero, Quaternion.identity);
                go.SetActive(true);
                go.name = s.Substring(s.Length - 17, 17); //MAC
                go.transform.parent = bluetoothContent.transform;
                
                TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
                text.text = s.Substring(1,s.Length - 24);

                go.GetComponent<Button>().onClick.AddListener(()=>{
                    parameters2[0] = 4;
                    parameters2[1] = go.name; //MAC
                    StartCoroutine(LoadingScreen());
                });

                bluetoothList.Add(go);
            }
        }
    }
    /*public void OutputTime()
    {
        string data = bluet.Call<string>("getData");
        
        string[] info = Regex.Split(data, Environment.NewLine);
        List<float> valuesList = new List<float>();
        float tempValue;
        int apneaValue;
        bool apnea;

        for (int i = 1; i < info.Length-1; i++)
        {
            string[] patientData = Regex.Split(info[i], ",");
            // patientData[0] -> flujo
            // patientData[1] -> apnea
            // patientData[2] -> frecuencia respiratoria
            if(float.TryParse(patientData[0], out tempValue))
            {
                valuesList.Add(tempValue);
                GameData.Instance.inspiration = (tempValue == 0 ? false : true);
            }
            else
            {
                Console.WriteLine("error parse data {0}", patientData[0]);
            }

            GameData.Instance.apnea = false;
            if(int.TryParse(patientData[1], out apneaValue))
            {
                GameData.Instance.apnea = (apneaValue == 1 ? true : false);
            }
            else
            {
                Console.WriteLine("error parse data {1}", patientData[1]);
            }
        }

        float[] valuesDouble = valuesList.ToArray();

        if(valuesDouble.Length > 0)
            prom = valuesDouble.Average();
        timer += Time.deltaTime;
    }*/

    public void OutputTime()
    {
        timer += Time.deltaTime;
        int apneaValue;
        
        string data = bluet.Call<string>("getData");
        string[] patientData = Regex.Split(data, ",");
        // patientData[0] -> flujo
        // patientData[1] -> apnea
        // patientData[2] -> frecuencia respiratoria
        if(float.TryParse(patientData[0], NumberStyles.Any, CultureInfo.InvariantCulture, out prom))
        {
            GameData.Instance.scriptsGroup.playerMovement.SaveData(prom, timer);
            GameData.Instance.inspiration = (prom == 0 ? false : true);
        }
        else
        {
            Console.WriteLine("error parse data {0}", patientData[0]);
        }

        if(int.TryParse(patientData[1], out apneaValue))
        {
            GameData.Instance.apnea = (apneaValue == 1 ? true : false);
        }
        else
        {
            Console.WriteLine("error parse data {1}", patientData[1]);
        }
    }

    IEnumerator LoadingScreen(){
        yield return new WaitForSeconds(0.5f);
        bluet.Call("connectToDevice", parameters2);
        //Debug.Log(connectionStatus);
        /*BluetoothConnector.Call("PrintOnScreen", context, connectionStatus);
            if (connectionStatus == "Connected") return true;
            else return false;*/
        yield return new WaitForSeconds(3f);
        UI_System.Instance.SwitchScreens(GameData.Instance.loginMenu);
        StopCoroutine(LoadingScreen());
        
    }    

    public void CallOutputTime(){
        InvokeRepeating("OutputTime", 0.5f, 0.05f); // 1/20 datos
    }

    public void StopOutputTime()
    {
        CancelInvoke();
    }
}
