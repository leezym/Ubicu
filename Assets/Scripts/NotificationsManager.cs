using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationsManager : MonoBehaviour
{
    public static NotificationsManager Instance{get; private set;}
    public GameObject notificationsMenu;
    public TMP_Text notificationsText;
    public Button notificationsYesButton;
    public Button notificationsNoButton;
    public Button notificationsCloseButton;

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
    }
    public void QuestionNotifications(string text)
    {
        notificationsMenu.SetActive(true);
        notificationsText.text = text;
        notificationsYesButton.gameObject.SetActive(true);
        notificationsNoButton.gameObject.SetActive(true);
        notificationsCloseButton.gameObject.SetActive(false);
    }

    public void SetCloseFunction(UI_Screen screen)
    {
        notificationsCloseButton.onClick.RemoveAllListeners();
        notificationsCloseButton.onClick.AddListener(()=>{
            UI_System.Instance.SwitchScreens(screen);
        });
    }
}
