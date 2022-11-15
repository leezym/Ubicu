using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Obstacles : MonoBehaviour
{
    [Header("ATTACHED")]
    public float secondsAferApnea;
    public bool enabledCounter = true;
    public GameObject limit;
    public UI_Screen serieGraph;
    public TMP_Text serieGameText;
    public TMP_Text repGameText;

    [Header("IN GAME")]
    public int repCounter;

    public IEnumerator ObstaclesCounter()
    {
        repGameText.text = "REPETICIÓN\n"+repCounter.ToString()+"/"+GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].repeticiones;
        if (!GameData.Instance.apnea) //manejador de estados if 0 -> 0 inspiracion if 1 -> 0 expiracion
        {
            enabledCounter = true;
            GameData.Instance.scriptsGroup.playerMovement.StopApnea();
            GameData.Instance.scriptsGroup.playerMovement.Movement();
            if(repCounter == GameData.Instance.scriptsGroup.exercisesManager.jsonObjectExercises.array[GameData.Instance.idJsonObjectExercises].repeticiones)
            {
                repCounter = 0;
                GameData.Instance.scriptsGroup.playerMovement.CreateGraph();
                yield return new WaitForSeconds(3f);
                GameData.Instance.playing = false;
                UI_System uI_System = FindObjectOfType<UI_System>();
                uI_System.SwitchScreens(serieGraph);
                GameData.Instance.resting = true;
                StopCoroutine(ObstaclesCounter());
            }
        }
        else
        {
            GameData.Instance.scriptsGroup.playerMovement.StartApnea();
            if(enabledCounter)
            {
                enabledCounter = false;
                GameData.Instance.scriptsGroup.playerMovement.SaveGraphData();
                repCounter ++;
            }
        }
    }
}
