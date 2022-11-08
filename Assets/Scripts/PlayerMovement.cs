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
    public float maxTargetScale;
    public ScriptsGroup scriptsGroup;
    public List<float> graphPointList = new List<float>();
    float targetScale;
    float restCount;
    float seriesCount;

    void Start()
    {
        scriptsGroup = FindObjectOfType<ScriptsGroup>();
        transform.localScale = new Vector2(minimunScale,minimunScale);
        //graphPointList = new List<float>{800, 120, 600, 900}; //test
        restCount = scriptsGroup.exercisesManager.jsonObjectExercises.array[scriptsGroup.gameData.idJsonObjectExercises].periodos_descanso;
    }
    
    public void MovementWhilePlaying()
    {
        //scriptsGroup.exercisesManager.exerciseFlujoPrefab.text = scriptsGroup.bluetoothPairing.prom.ToString() +"ml";
        // convert
        targetScale = (scriptsGroup.bluetoothPairing.prom * maximunScale / scriptsGroup.exercisesManager.jsonObjectExercises.array[scriptsGroup.gameData.idJsonObjectExercises].flujo) + minimunScale;

        if (targetScale < minimunScale)
            targetScale = minimunScale;
        if (targetScale > maximunScale)
            targetScale = maximunScale;

    }
    
    public void Movement()
    {
        if (targetScale > maxTargetScale)
            maxTargetScale = targetScale;

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
        scriptsGroup.obstacles.InvokeApenaTest(); //test
        DeleteGraph();
        UI_System uI_System = FindObjectOfType<UI_System>();
        scriptsGroup.gameData.resting = false;
        restCount = scriptsGroup.exercisesManager.jsonObjectExercises.array[scriptsGroup.gameData.idJsonObjectExercises].periodos_descanso;
        if(seriesCount < scriptsGroup.exercisesManager.jsonObjectExercises.array[scriptsGroup.gameData.idJsonObjectExercises].series)
        {
            uI_System.SwitchScreens(exerciseMenu_Game);
            scriptsGroup.gameData.playing = true;
            seriesTextGame.text = "SERIE "+ (seriesCount+1);
            seriesTextGraph.text = "SERIE "+ (seriesCount+1);
            seriesCount++;
        }
        else
        {
            uI_System.SwitchScreens(sessionMenu);
        }
    }

    public void SaveGraphData()
    {
        graphPointList.Add(maxTargetScale);
        maxTargetScale = 0; 
    }

    public void CreateGraph()
    {
        float maxValue = 0;
        if(Mathf.Max(graphPointList.ToArray()) > scriptsGroup.exercisesManager.jsonObjectExercises.array[scriptsGroup.gameData.idJsonObjectExercises].flujo)
            maxValue = Mathf.Max(graphPointList.ToArray());
        else
            maxValue = scriptsGroup.exercisesManager.jsonObjectExercises.array[scriptsGroup.gameData.idJsonObjectExercises].flujo;
        
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

        graphPositionY = graphStructure.GetComponent<RectTransform>().rect.height * scriptsGroup.exercisesManager.jsonObjectExercises.array[scriptsGroup.gameData.idJsonObjectExercises].flujo / maxValue;
        goalGraph.transform.localPosition = new Vector2(0f, graphPositionY);
    }

    public void DeleteGraph()
    {
        graphPointList = new List<float>();
        foreach (Transform point in graphStructure.transform)
        {
            if(point.gameObject.name == "GraphPrefab(Clone)")
                Destroy(point.gameObject);
        }        
    }
}
