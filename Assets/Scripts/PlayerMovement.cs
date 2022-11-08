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
    public Transform graphStructure;
    public Transform goalGraph;
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
            transform.localScale = new Vector2(minimunScale,minimunScale);
        else if (targetScale > maximunScale)
            transform.localScale = new Vector2(maximunScale,maximunScale);
    }
    
    public void Movement()
    {
        if (targetScale > maxTargetScale)
            maxTargetScale = targetScale;

        transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(targetScale, targetScale), Time.deltaTime / speedStandarScale);
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
        scriptsGroup.obstacles.InvokeApenaTest();
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
        float maxValue = Mathf.Max(graphPointList.ToArray());
        float graphPositionY = graphStructure.GetComponent<RectTransform>().rect.height * scriptsGroup.exercisesManager.jsonObjectExercises.array[scriptsGroup.gameData.idJsonObjectExercises].flujo / maxValue;
        goalGraph.localPosition = new Vector2(goalGraph.localPosition.x, graphPositionY);

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
    }
}
