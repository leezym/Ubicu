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
    public static float CIRCLE_PLUS_SCALE = 0.05f;
    public static float LUNG_MINIMUM_SCALE = 0.23f;
    public static float LUNG_MAXIMUM_SCALE = 0.72f;
    public static float LUNG_PLUS_SCALE = 0.03f;
    [Header("ATTACHED")]
    public float minimunScale;
    public float maximunScale;
    public float plusScale;
    public int speedStandarScale;
    public GameObject pause;
    public TMP_Text pauseText;
    public GameObject graphPrefab;
    public GameObject goalGraph;
    public Transform graphStructure;
    public TMP_Text restText;
    public TMP_Text seriesTextGame;
    public TMP_Text seriesTextGraph;

    [Header("IN GAME")]
    public GameObject player;
    public float maxTargetScale;
    public float maxFlow;
    float apneaCount;
    float restCount;
    int seriesCount;
    public float timeDuringGame;
    public List<float> tempGraphFlow;
    public List<float> tempGraphTime;

    public void InitializeLevel()
    {
        player.transform.localScale = new Vector2(minimunScale,minimunScale);
        restCount = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].periodos_descanso;
        apneaCount = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].apnea;
        
        for (int i = 0; i < GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series; i++)
            GameData.Instance.exerciseSeries.Add(new ExerciseData { tiempo = new List<float>(), flujo = new List<float>() });
    }    
    public void Movement()
    {
        //test
        GameData.Instance.scriptsGroup.exercisesManager.exerciseFlujoPrefab.text = Math.Round(maxFlow, 1).ToString()+" mL";

        if(GameData.Instance.inspiration)
        {
            // sin maximo
            /*if(GameData.Instance.scriptsGroup.bluetoothPairing.prom > maxFlow)
            {
                maxFlow = GameData.Instance.scriptsGroup.bluetoothPairing.prom;
                timeDuringGame = GameData.Instance.scriptsGroup.bluetoothPairing.timer;
            }

            if(GameData.Instance.scriptsGroup.bluetoothPairing.prom <= GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo)
            {
                maxTargetScale = (GameData.Instance.scriptsGroup.bluetoothPairing.prom * maximunScale / GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo) + minimunScale;                    
            }
            else
            {
                maxTargetScale = maximunScale;
            }*/

            // con maximo
            if(GameData.Instance.scriptsGroup.bluetoothPairing.prom > maxFlow)
            {
                maxFlow = GameData.Instance.scriptsGroup.bluetoothPairing.prom;
                timeDuringGame = GameData.Instance.scriptsGroup.bluetoothPairing.timer;

                if(maxFlow < GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo)
                {
                    maxTargetScale = (maxFlow * maximunScale / GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo) + minimunScale;                    
                }
                else
                {
                    maxTargetScale = maximunScale + plusScale;
                }
            }
        }
        else
            maxTargetScale = 0;
        
        if (player.transform.localScale.x < minimunScale)
            player.transform.localScale = new Vector2(minimunScale,minimunScale);
        else if (player.transform.localScale.x > maximunScale)
            player.transform.localScale = new Vector2(maximunScale,maximunScale);
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
            GameData.Instance.scriptsGroup.bluetoothPairing.timer = 0;
            UI_System.Instance.SwitchScreens(GameData.Instance.exerciseMenu_Game);
            GameData.Instance.playing = true;
            seriesTextGame.text = "SERIE "+ (seriesCount+1);
            seriesTextGraph.text = "SERIE "+ (seriesCount+1);
        }
        else
        {
            seriesCount = 0;
            StartCoroutine(GameData.Instance.scriptsGroup.exercisesManager.SendResults());
            GameData.Instance.exerciseHourArray[GameData.Instance.idListHourExercises] = 0; // si se finalizó se coloca 0
            GameData.Instance.idListHourExercises = -1;
            GameData.Instance.scriptsGroup.rewardsManager.CalculateRewards();
            //UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);
        }
    }

    public void StartApnea()
    {
        pauseText.text = ((int)apneaCount+1).ToString();
        pause.SetActive(true);
        if(apneaCount >= 0)
        {
            apneaCount -= Time.deltaTime;
        }
    }

    public void StopApnea()
    {
        pause.SetActive(false);
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
            GameObject go = Instantiate(graphPrefab, Vector3.zero, Quaternion.identity);
            go.SetActive(true);
            go.transform.parent = graphStructure.transform;
            go.transform.localScale = new Vector3(1,1,1);

            //X position
            float pointGraphPositionX = (i + 1) * (graphStructure.GetComponent<RectTransform>().rect.width / (tempGraphFlow.Count + 1)); //+1 para dejar un margen visual de la gráfica
            // Y position
            float pointGraphPositionY = graphStructure.GetComponent<RectTransform>().rect.height * tempGraphFlow[i] / maxValue;
            go.transform.localPosition = new Vector2(pointGraphPositionX, pointGraphPositionY);

            if(pointGraphPositionY >= goalGraphPositionY)
            {
                Transform checkPrefab = go.transform.Find("Check");
                checkPrefab.gameObject.SetActive(true);
            }

            Image imageLinePrefab = go.transform.Find("Line").GetComponent<Image>();
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
