using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("ATTACHED")]
    public float minimunScale;
    public float maximunScale;
    public int speedScale;
    public BluetoothPairing bluetoothPairingScript;
    public Login loginScript;
    public GameObject pause;
    public TMP_Text pauseText;

    //[Header("IN GAME")]
    void Start()
    {
        transform.localScale = new Vector2(minimunScale,minimunScale);
    }

    void Update()
    {

        loginScript.exerciseFlujoPrefab.text = bluetoothPairingScript.prom.ToString() +"ml";
        // convert
        float targetScale = bluetoothPairingScript.prom * maximunScale / loginScript.jsonObjectExercises.array[loginScript.idJsonObjectExercises].flujo;

        if (targetScale < minimunScale)
            transform.localScale = new Vector2(minimunScale,minimunScale);
        else if (targetScale > maximunScale)
            //transform.localScale = new Vector2(maximunScale,maximunScale);
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(targetScale, targetScale), Time.deltaTime / 10);
        else
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(targetScale, targetScale), Time.deltaTime * speedScale);  
    }

    // public void StartMovement()
    // {
    //     StartCoroutine(MovementIn());
    // }

    // IEnumerator MovementIn()
    // {
    //     for (int i = 0; i < loginScript.jsonObjectExercises.array[loginScript.idJsonObjectExercises].flujo; i++)
    //     {
    //         float targetScale = (i * maximunScale / loginScript.jsonObjectExercises.array[loginScript.idJsonObjectExercises].flujo) + minimunScale;
    //         if (transform.localScale.x < minimunScale)
    //             transform.localScale = new Vector2(minimunScale,minimunScale);
    //         else if (transform.localScale.x > maximunScale)
    //             transform.localScale = new Vector2(maximunScale,maximunScale);
    //         else
    //             transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(targetScale, targetScale), Time.deltaTime * speedScale);
            
    //         yield return new WaitForSeconds(0.005f);
    //     }

    //     pause.SetActive(true);
    //     for(int i = loginScript.jsonObjectExercises.array[loginScript.idJsonObjectExercises].apnea; i > 0; i--)
    //     {
    //         pauseText.text = i.ToString();
    //         yield return new WaitForSeconds(1f);
    //     }
    //     pause.SetActive(false);

    //     StopCoroutine(MovementIn());
    //     StartCoroutine(MovementOut());
    // }
    // IEnumerator MovementOut()
    // {
    //     for (int i = loginScript.jsonObjectExercises.array[loginScript.idJsonObjectExercises].flujo; i > 0; i--)
    //     {
    //         float targetScale = (i * maximunScale / loginScript.jsonObjectExercises.array[loginScript.idJsonObjectExercises].flujo) + minimunScale;
    //         if (transform.localScale.x < minimunScale)
    //             transform.localScale = new Vector2(minimunScale,minimunScale);
    //         else if (transform.localScale.x > maximunScale)
    //             transform.localScale = new Vector2(maximunScale,maximunScale);
    //         else
    //             transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(targetScale, targetScale), Time.deltaTime * speedScale);
            
    //         yield return new WaitForSeconds(0.005f);
    //     }
    //     StopCoroutine(MovementOut());
    //     StartCoroutine(MovementIn());
    // }
}
