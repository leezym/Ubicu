using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public static float CIRCLE_MINIMUM_SCALE = 0.08f;
    public static float CIRCLE_MAXIMUM_SCALE = 0.75f;
    public static float CIRCLE_PLUS_SCALE = 0.83f;
    public static float LUNG_MINIMUM_SCALE = 0.23f;
    public static float LUNG_MAXIMUM_SCALE = 0.72f;
    public static float LUNG_PLUS_SCALE = 0.76f;
    public float POST_APNEA = 0f;

    
    [Header("ATTACHED")]
    public float minimunScale;
    public float maximunScale;
    public float plusScale;
    public int speedStandarScale;
    public GameObject pause;
    public TMP_Text pauseText;
    public GameObject[] graphPrefab;
    public GameObject goalGraph;
    public Transform graphStructure;
    public TMP_Text restText;
    public TMP_Text seriesTextGame;
    public TMP_Text seriesTextGraph;
    public Button buttonPlayGame;

    [Header("IN GAME")]
    public GameObject player;
    public float maxTargetScale;
    public float maxFlow;
    public bool apneaBool;
    float apneaCount;
    float restCount;
    public int seriesCount;
    public float timeDuringGame;
    public List<float> tempGraphFlow;
    public List<float> tempGraphTime;

    public void InitializeLevel()
    {
        StartCoroutine(CallInitializeLevel());
    }    

    IEnumerator CallInitializeLevel()
    {
        buttonPlayGame.interactable = false;
        player.transform.localScale = new Vector2(minimunScale,minimunScale);
        restCount = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].periodos_descanso;
        apneaCount = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].apnea;
        
        for (int i = 0; i < GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series; i++)
            GameData.Instance.exerciseSeries.Add(new ExerciseData { tiempo = new List<float>(), flujo = new List<float>() });
        
        yield return new WaitForSeconds(2f);
        buttonPlayGame.interactable = true;
    }

    public void Movement()
    {
        //test
        //GameData.Instance.scriptsGroup.exercisesManager.exerciseFlujoPrefab.text = "I:"+GameData.Instance.inspiration+"-A:"+apneaBool+"\nMx:"+maxFlow.ToString()+"-Curr:"+GameData.Instance.scriptsGroup.bluetoothPairing.prom.ToString();
        //GameData.Instance.scriptsGroup.exercisesManager.exerciseFlujoPrefab.text = "I:"+GameData.Instance.inspiration+"-A:"+apneaBool;
        
        if(GameData.Instance.inspiration && !apneaBool)
        {
            if(GameData.Instance.scriptsGroup.bluetoothPairing.prom > maxFlow)
            {
                maxFlow = GameData.Instance.scriptsGroup.bluetoothPairing.prom;
                timeDuringGame = GameData.Instance.scriptsGroup.bluetoothPairing.timer;
                if(maxFlow <= GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo)
                    maxTargetScale = (maxFlow * maximunScale / GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo) + minimunScale;                    
                else
                    maxTargetScale = plusScale;
            }
        }
        else if(apneaBool)
            maxTargetScale = 0;
                
        if (player.transform.localScale.x < minimunScale)
            player.transform.localScale = new Vector2(minimunScale,minimunScale);
        else
            player.transform.localScale = Vector2.Lerp(player.transform.localScale, new Vector2(maxTargetScale, maxTargetScale), Time.deltaTime * speedStandarScale);
    }

    public void RestingPlayer()
    {        
        restText.text = "Descansa por " + (int)restCount + " segundos...";
        if(restCount >= 0)
            restCount -= Time.deltaTime;
        else
            ContinueGame();
    }

    public void ContinueGame()
    {
        DeleteGraph();
        GameData.Instance.resting = false;
        restCount = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].periodos_descanso;

        if(seriesCount < GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series)
        {
            GameData.Instance.scriptsGroup.bluetoothPairing.CallOutputTime();
            UI_System.Instance.SwitchScreens(GameData.Instance.exerciseMenu_Game);
            GameData.Instance.playing = true;
            seriesTextGame.text = "SERIE "+ (seriesCount+1);
            seriesTextGraph.text = "SERIE "+ (seriesCount+1);
        }
        else
        {
            seriesCount = 0;
            StartCoroutine(GameData.Instance.scriptsGroup.exercisesManager.SendResults());
            GameData.Instance.scriptsGroup.rewardsManager.CalculateRewards();
        }
    }

    public IEnumerator StartApnea()
    {        
        GameData.Instance.scriptsGroup.soundsManager.activeSignalSound = true;

        pause.SetActive(true);
        if(apneaCount >= 0)
        {
            apneaCount -= Time.deltaTime;
            pauseText.text = "MANTENGA\nEL AIRE\n" + ((int)apneaCount+1).ToString();
        }
        else
        {
            pauseText.text = "BOTA EL AIRE";
            yield return new WaitForSeconds(1f);
            StartCoroutine(StopApnea());
        }
    }

    IEnumerator StopApnea()
    {
        pause.SetActive(false);
        yield return new WaitForSeconds(POST_APNEA);
        apneaBool = false;
        apneaCount = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].apnea;
    }

    public void SaveData(float flow, float time)
    {
        GameData.Instance.exerciseSeries[seriesCount].flujo.Add(flow);
        GameData.Instance.exerciseSeries[seriesCount].tiempo.Add(time);
    }

    public void SaveGraphData()
    {
        tempGraphFlow.Add(maxFlow);
        tempGraphTime.Add(timeDuringGame);
        
        maxTargetScale = 0;
        maxFlow = 0;
        timeDuringGame = 0;
    }

    public void CreateGraph()
    {
        float maxValue = 0;
        if(Mathf.Max(tempGraphFlow.ToArray()) > GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo)
            maxValue = Mathf.Max(tempGraphFlow.ToArray());
        else
            maxValue = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo;
        
        float goalGraphPositionY = graphStructure.GetComponent<RectTransform>().rect.height * GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo / maxValue;
        goalGraph.transform.localPosition = new Vector2(0f, goalGraphPositionY);

        for(int i = 0; i < tempGraphFlow.Count; i++)
        {
            graphPrefab[i].SetActive(true);
            //X position
            float pointGraphPositionX = (i + 1) * (graphStructure.GetComponent<RectTransform>().rect.width / (tempGraphFlow.Count + 1)); //+1 para dejar un margen visual de la gráfica
            // Y position
            float pointGraphPositionY = graphStructure.GetComponent<RectTransform>().rect.height * tempGraphFlow[i] / maxValue;
            graphPrefab[i].transform.localPosition = new Vector2(pointGraphPositionX, pointGraphPositionY);

            Transform checkPrefab = graphPrefab[i].transform.Find("Check");
            checkPrefab.gameObject.SetActive(pointGraphPositionY >= goalGraphPositionY ? true : false);

            Image imageLinePrefab = graphPrefab[i].transform.Find("Line").GetComponent<Image>();
            imageLinePrefab.rectTransform.sizeDelta = new Vector2(imageLinePrefab.rectTransform.sizeDelta.x, pointGraphPositionY);
        }

        seriesCount++;
    }

    public void DeleteGraph()
    {        
        tempGraphFlow = new List<float>();
        tempGraphTime = new List<float>();

        foreach (Transform point in graphStructure.transform)
        {
            if(point.gameObject.name == "GraphPrefab(Clone)")
                Destroy(point.gameObject);
        }
    }
}
