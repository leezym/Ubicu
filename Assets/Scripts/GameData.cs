using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
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

    /*public Exercises m_jsonObjectExercises;
    public bool jsonObjectExercises
    {
        get { return m_jsonObjectExercises; }
        set { m_jsonObjectExercises = value; }
    }*/

    public void Start()
    {
        scriptsGroup = FindObjectOfType<ScriptsGroup>();
    }
    
    void Update()
    {
        if(playing)
            scriptsGroup.playerMovement.MovementWhilePlaying();
        //else
            //ya se acaben las series de la sesion

        if(resting)
            scriptsGroup.playerMovement.RestingPlayer();
    }
}
