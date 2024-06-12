using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

public class BluetoothPairing : MonoBehaviour
{
    public static BluetoothPairing Instance {get; private set;}

    [Header("BLUETOOTH")]
    AndroidJavaObject bluet;
    AndroidJavaClass unityPlayerClass;
    AndroidJavaObject unityActivity;
    object[] parameters2 = new object[2];
    bool isConnected;

    [Header("ATTACHED")]
    public UI_Screen bluetoothMenu;
    public GameObject[] prefabTextBluetooth;
    public GameObject bluetoothContent;

    //test
    public TMP_Text stringConnection;

    [Header("IN GAME")]
    public float prom = 0;
    public float timer = 0;

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    
    void Start()
    {
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
            PlayerMovement.Instance.SaveData(prom, timer);
            prom = (PlayerMovement.Instance.apneaBool ? 0 : prom);
            GameData.Instance.inspiration = (prom > 0 ? true : false);
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
        yield return new WaitForSeconds(3f);
        UI_System.Instance.SwitchScreens(Login.Instance.loginMenu);        
    }

    /*IEnumerator LoadingScreen(){
        yield return new WaitForSeconds(0.5f);
        isConnected = bluet.Call<bool>("connectToDevice", parameters2);
        yield return new WaitForSeconds(3f);
        
        if(isConnected)
            UI_System.Instance.SwitchScreens(Login.Instance.loginMenu);        
        else
        {
            UI_System.Instance.SwitchScreens(bluetoothMenu); 
            NotificationsManager.Instance.WarningNotifications("No se puede conectar al dispositivo. Intente nuevamente."); 
        }
    }    */

    public void CallOutputTime()
    {
        InvokeRepeating("OutputTime", 0.5f, 0.05f); // 1/20 datos
    }

    public void StopOutputTime()
    {
        CancelInvoke();
    }
}
