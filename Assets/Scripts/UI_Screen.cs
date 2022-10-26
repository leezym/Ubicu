using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Screen : MonoBehaviour
{
    #region Variables
    private Component[] screens = new Component[0];
    private 
    #endregion

    #region Main Method
    void Start()
    {
        screens = GetComponentsInChildren<UI_Screen>(true);
    }

    void Update()
    {
        
    }

    #endregion

    #region Helper Method
    public void SwtichScreens(UI_Screen aScreen)
    {

    }
    #endregion
}
