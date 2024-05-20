using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationsManager : MonoBehaviour
{
    public static NotificationsManager Instance {get; private set;}
    public GameObject notificationsMenu;
    public TMP_Text notificationsText;
    public Button notificationsNextButton;
    public Button notificationsYesButton;
    public Button notificationsNoButton;
    public Button notificationsCloseButton;
    public List<string> multipleNotifications;

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void WarningNotifications(string text)
    {
        notificationsMenu.SetActive(true);
        notificationsText.text = text;
        notificationsYesButton.gameObject.SetActive(false);
        notificationsNoButton.gameObject.SetActive(false);
        notificationsCloseButton.gameObject.SetActive(true);
        notificationsNextButton.gameObject.SetActive(false);
    }
    
    public void QuestionNotifications(string text)
    {
        notificationsMenu.SetActive(true);
        notificationsText.text = text;
        notificationsYesButton.gameObject.SetActive(true);
        notificationsNoButton.gameObject.SetActive(true);
        notificationsCloseButton.gameObject.SetActive(false);
        notificationsNextButton.gameObject.SetActive(false);
    }

    public void SetCloseFunction(UI_Screen screen)
    {
        notificationsCloseButton.onClick.RemoveAllListeners();
        notificationsCloseButton.onClick.AddListener(()=>{
            UI_System.Instance.SwitchScreens(screen);
        });
    }

    public void SetCloseFunction()
    {
        notificationsCloseButton.onClick.RemoveAllListeners();
    }

    public void SetChangeTextFunction(string text)
    {
        notificationsNextButton.onClick.RemoveAllListeners();
        notificationsNextButton.gameObject.SetActive(true);
        multipleNotifications.Add(text);

        notificationsNextButton.onClick.AddListener(()=>{
            WarningNotifications(multipleNotifications[0]);
            multipleNotifications.RemoveAt(0);
            if(multipleNotifications.Count > 0)
                notificationsNextButton.gameObject.SetActive(true);
        });
    }

    public void SetYesButton(Action function)
    {
        notificationsYesButton.onClick.RemoveAllListeners();
        notificationsYesButton.onClick.AddListener(()=>{
            function();
        });
    }
}
