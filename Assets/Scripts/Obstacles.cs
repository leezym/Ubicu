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
    public ScriptsGroup scriptsGroup;
    public int counter;
    public bool apnea;
    
    void Start()
    {
        scriptsGroup = FindObjectOfType<ScriptsGroup>();
    }

    void Update()
    {
        if(scriptsGroup.gameData.playing)
            StartCoroutine(ObstaclesCounter());
    }

    public void InvokeApenaTest()
    {
        float time = scriptsGroup.exercisesManager.jsonObjectExercises.array[scriptsGroup.gameData.idJsonObjectExercises].apnea + (secondsAferApnea*2);
        InvokeRepeating("CallApenaTest", 0.25f, time);
    }
    
    public void CallApenaTest()
    {
        StartCoroutine(ApenaTest());
    }

    IEnumerator ApenaTest()
    {
        apnea = false;
        limit.SetActive(true);
        yield return new WaitForSeconds(secondsAferApnea);
        apnea = true;
        limit.SetActive(false);
        yield return new WaitForSeconds(scriptsGroup.exercisesManager.jsonObjectExercises.array[scriptsGroup.gameData.idJsonObjectExercises].apnea);
        apnea = false;
        limit.SetActive(false);
        yield return new WaitForSeconds(secondsAferApnea);
    }

    IEnumerator ObstaclesCounter()
    {
        repGameText.text = "REPETICIÓN\n"+counter.ToString()+"/"+scriptsGroup.exercisesManager.jsonObjectExercises.array[scriptsGroup.gameData.idJsonObjectExercises].repeticiones;
        if(counter < scriptsGroup.exercisesManager.jsonObjectExercises.array[scriptsGroup.gameData.idJsonObjectExercises].repeticiones)
        {
            if (!apnea)
            {
                enabledCounter = true;
                scriptsGroup.playerMovement.Movement();
            }
            if(apnea && enabledCounter)
            {
                enabledCounter = false;
                yield return new WaitForSeconds(scriptsGroup.exercisesManager.jsonObjectExercises.array[scriptsGroup.gameData.idJsonObjectExercises].apnea + secondsAferApnea);
                scriptsGroup.playerMovement.SaveGraphData();
                counter++;
            }
        }
        else
        {
            CancelInvoke(); //apneatest
            scriptsGroup.gameData.playing = false;
            counter = 0;
            scriptsGroup.playerMovement.CreateGraph();
            yield return new WaitForSeconds(1.5f);
            UI_System uI_System = FindObjectOfType<UI_System>();
            uI_System.SwitchScreens(serieGraph);
            scriptsGroup.gameData.resting = true;
        }
        StopCoroutine(ObstaclesCounter());
    }
}
