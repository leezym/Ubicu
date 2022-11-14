using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance{get; private set;}
    public ScriptsGroup scriptsGroup;
    private bool m_playing = false;
    public bool playing
    {
        get { return m_playing; }
        set { m_playing = value; }
    }

    private int m_idJsonObjectExercises;
    public int idJsonObjectExercises
    {
        get { return m_idJsonObjectExercises; }
        set { m_idJsonObjectExercises = value; }
    }

    private bool m_resting = false;
    public bool resting
    {
        get { return m_resting; }
        set { m_resting = value; }
    }

    private bool m_apnea = false;
    public bool apnea
    {
        get { return m_apnea; }
        set { m_apnea = value; }
    }


    private bool m_inspiration = false;
    public bool inspiration
    {
        get { return m_inspiration; }
        set { m_inspiration = value; }
    }

    /*public Exercises m_jsonObjectExercises;
    public bool jsonObjectExercises
    {
        get { return m_jsonObjectExercises; }
        set { m_jsonObjectExercises = value; }
    }*/

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    
    void Update()
    {
        if(playing)
        {
            GameData.Instance.scriptsGroup.playerMovement.DetectFlow();
            GameData.Instance.scriptsGroup.obstacles.ObstaclesCounter();            
        }
        //else
            //ya se acaben las series de la sesion

        if(resting)
            GameData.Instance.scriptsGroup.playerMovement.RestingPlayer();
    }

    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        scriptsGroup.rewardsManager.SaveReward();
    }
}
