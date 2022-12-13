using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public static float CIRCLE_MINIMUM_SCALE = 0.08f;
    public static float CIRCLE_MAXIMUM_SCALE = 0.8f;
    public static float LUNG_MINIMUM_SCALE = 0.23f;
    public static float LUNG_MAXIMUM_SCALE = 0.75f;
    [Header("ATTACHED")]
    public float minimunScale;
    public float maximunScale;
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
    public float targetScale;
    public float maxTargetScale;
    public float maxFlow;
    public List<float> graphPointList;
    float restCount;
    float seriesCount;
    float apneaCount;

    public void InitializeLevel()
    {
        player.transform.localScale = new Vector2(minimunScale,minimunScale);
        restCount = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].periodos_descanso;
        apneaCount = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].apnea;
    }
    
    public void Movement()
    {
        //test
        //GameData.Instance.scriptsGroup.exercisesManager.exerciseFlujoPrefab.text = "prom: "+GameData.Instance.scriptsGroup.bluetoothPairing.prom+"\nmaxFlow: " +maxFlow+"\ninsp: "+GameData.Instance.inspiration;

        if(GameData.Instance.inspiration)
        {
            // convert scale
            if(GameData.Instance.scriptsGroup.bluetoothPairing.prom <= GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo)
            {
                targetScale = (GameData.Instance.scriptsGroup.bluetoothPairing.prom * maximunScale / GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo) + minimunScale;
                if (targetScale > maxTargetScale)
                {
                    maxTargetScale = targetScale;
                    maxFlow = (GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo * (maxTargetScale - minimunScale))/maximunScale;
                }
            }
            else
            {
                targetScale = maximunScale;
                maxFlow = GameData.Instance.scriptsGroup.bluetoothPairing.prom;
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
            UI_System.Instance.SwitchScreens(GameData.Instance.exerciseMenu_Game);
            GameData.Instance.playing = true;
            seriesTextGame.text = "SERIE "+ (seriesCount+1);
            seriesTextGraph.text = "SERIE "+ (seriesCount+1);
            seriesCount++;
        }
        else
        {
            seriesCount = 0;
            GameData.Instance.scriptsGroup.rewardsManager.CalculateRewards();
            GameData.Instance.scriptsGroup.exercisesManager.sesionesList[GameData.Instance.idListHourExercises].GetComponent<Button>().interactable = false;
            GameData.Instance.scriptsGroup.exercisesManager.sesionesList[GameData.Instance.idListHourExercises].GetComponent<Image>().sprite = GameData.Instance.scriptsGroup.exercisesManager.finishedSessionSprite;
            GameData.Instance.scriptsGroup.exercisesManager.exerciseHourArray[GameData.Instance.idListHourExercises] = 0;
            GameData.Instance.idListHourExercises = -1;
            GameData.Instance.scriptsGroup.bluetoothPairing.StopOutputTime();
            UI_System.Instance.SwitchScreens(GameData.Instance.sessionMenu);
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

    public void SaveGraphData()
    {
        graphPointList.Add(maxFlow);
        maxTargetScale = 0;
        maxFlow = 0;
    }

    public void CreateGraph()
    {
        float maxValue = 0;
        if(Mathf.Max(graphPointList.ToArray()) > GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo)
            maxValue = Mathf.Max(graphPointList.ToArray());
        else
            maxValue = GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo;
        
        float goalGraphPositionY = graphStructure.GetComponent<RectTransform>().rect.height * GameData.Instance.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo / maxValue;
        goalGraph.transform.localPosition = new Vector2(0f, goalGraphPositionY);

        for(int i = 0; i < graphPointList.Count; i++)
        {
            GameObject go = Instantiate(graphPrefab, Vector3.zero, Quaternion.identity);
            go.SetActive(true);
            go.transform.parent = graphStructure.transform;
            go.transform.localScale = new Vector3(1,1,1);

            //X position
            float pointGraphPositionX = (i + 1) * (graphStructure.GetComponent<RectTransform>().rect.width / (graphPointList.Count + 1)); //+1 para dejar un margen visual de la gráfica
            // Y position
            float pointGraphPositionY = graphStructure.GetComponent<RectTransform>().rect.height * graphPointList[i] / maxValue;
            go.transform.localPosition = new Vector2(pointGraphPositionX, pointGraphPositionY);

            if(pointGraphPositionY >= goalGraphPositionY)
            {
                Transform checkPrefab = go.transform.Find("Check");
                checkPrefab.gameObject.SetActive(true);
            }

            Image imageLinePrefab = go.transform.Find("Line").GetComponent<Image>();
            imageLinePrefab.rectTransform.sizeDelta = new Vector2(imageLinePrefab.rectTransform.sizeDelta.x, pointGraphPositionY);
        }

    }

    public void DeleteGraph()
    {
        foreach (Transform point in graphStructure.transform)
        {
            if(point.gameObject.name == "GraphPrefab(Clone)")
                Destroy(point.gameObject);
        }        
        graphPointList = new List<float>();
    }
}
