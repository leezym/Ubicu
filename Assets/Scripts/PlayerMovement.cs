using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
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
    public UI_Screen exerciseMenu_Game;
    public UI_Screen sessionMenu;

    [Header("IN GAME")]
    public float targetScale;
    public float maxTargetScale;
    public float maxFlow;
    public List<float> graphPointList;
    float restCount;
    float seriesCount;
    float apneaCount;

    public void InitializeLevel()
    {
        transform.localScale = new Vector2(minimunScale,minimunScale);
        restCount = GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].periodos_descanso;
        apneaCount = GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].apnea;
    }
    
    public void Movement()
    {
        GameData.Instance.scriptsGroup.exercisesManager.exerciseFlujoPrefab.text = "prom: "+GameData.Instance.scriptsGroup.bluetoothPairing.prom+"\nmaxFlow: " +maxFlow+"\ninsp: "+GameData.Instance.inspiration;

        if(GameData.Instance.inspiration)
        {
            // convert scale
            if(GameData.Instance.scriptsGroup.bluetoothPairing.prom <= GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo)
            {
                targetScale = (GameData.Instance.scriptsGroup.bluetoothPairing.prom * maximunScale / GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo) + minimunScale;
                if (targetScale > maxTargetScale)
                {
                    maxTargetScale = targetScale;
                    maxFlow = (GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo * (maxTargetScale - minimunScale))/maximunScale;
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
            
        transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxTargetScale, maxTargetScale), Time.deltaTime * speedStandarScale);
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
        UI_System uI_System = FindObjectOfType<UI_System>();
        GameData.Instance.resting = false;
        restCount = GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].periodos_descanso;
        if(seriesCount < GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].series)
        {
            uI_System.SwitchScreens(exerciseMenu_Game);
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
            uI_System.SwitchScreens(sessionMenu);
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
        apneaCount = GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].apnea;
    }

    public void SaveGraphData()
    {
        //graphPointList = new List<float>{800, 120, 600, 900}; //test
        graphPointList.Add(maxFlow);
        maxTargetScale = 0;
        maxFlow = 0;
    }

    public void CreateGraph()
    {
        float maxValue = 0;
        if(Mathf.Max(graphPointList.ToArray()) > GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo)
            maxValue = Mathf.Max(graphPointList.ToArray());
        else
            maxValue = GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo;
        
        float graphPositionY = 0;
        for(int i = 0; i < graphPointList.Count; i++)
        {
            GameObject go = Instantiate(graphPrefab, Vector3.zero, Quaternion.identity);
            go.SetActive(true);
            go.transform.parent = graphStructure.transform;
            go.transform.localScale = new Vector3(1,1,1);

            //X position
            float graphPositionX = (i + 1) * (graphStructure.GetComponent<RectTransform>().rect.width / (graphPointList.Count + 1)); //+1 para dejar un margen visual de la gráfica
            // Y position
            graphPositionY = graphStructure.GetComponent<RectTransform>().rect.height * graphPointList[i] / maxValue;
            go.transform.localPosition = new Vector2(graphPositionX, graphPositionY);

            Image imageLinePrefab = go.transform.Find("Line").GetComponent<Image>();
            imageLinePrefab.rectTransform.sizeDelta = new Vector2(imageLinePrefab.rectTransform.sizeDelta.x, graphPositionY);
        }

        graphPositionY = graphStructure.GetComponent<RectTransform>().rect.height * GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].flujo / maxValue;
        goalGraph.transform.localPosition = new Vector2(0f, graphPositionY);
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
