
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[SetUpFixture]
public class Setup_Login
{
    public static UI_System UISystemObject { get; private set; }
    public static UI_Screen UIScreenObject { get; private set; }
    public static GameData GameDataObject { get; private set; }
    public static ExercisesManager ExerciseManagerObject { get; private set; }
    public static RewardsManager RewardsManagerObject { get; private set; }
    public static CustomizationManager CustomizationManagerObject { get; private set; }
    public static NotificationsManager NotificationsManagerObject { get; private set; }
    public static Login LoginObject { get; private set; }
    public static PlayerMovement PlayerMovementObject { get; private set; }

    [OneTimeSetUp]
    public void RunBeforeAnyTests2()
    {
        UISystemObject = new GameObject().AddComponent<UI_System>();
        UIScreenObject = new GameObject().AddComponent<UI_Screen>();
        GameDataObject = new GameObject().AddComponent<GameData>();
        ExerciseManagerObject = new GameObject().AddComponent<ExercisesManager>();
        RewardsManagerObject = new GameObject().AddComponent<RewardsManager>();
        CustomizationManagerObject = new GameObject().AddComponent<CustomizationManager>();
        NotificationsManagerObject = new GameObject().AddComponent<NotificationsManager>();
        LoginObject = new GameObject().AddComponent<Login>();
        PlayerMovementObject = new GameObject().AddComponent<PlayerMovement>();

        // Configuración
        LoginObject.loginButton = new GameObject().AddComponent<Button>();
        LoginObject.userInputField = new GameObject().AddComponent<TMP_InputField>();
        LoginObject.passInputField = new GameObject().AddComponent<TMP_InputField>();
        LoginObject.notInternet = new GameObject().AddComponent<Toggle>();
        LoginObject.passTitle = new GameObject();

        NotificationsManagerObject.notificationsMenu = new GameObject();
        NotificationsManagerObject.notificationsText = new GameObject().AddComponent<TextMeshProUGUI>();
        NotificationsManagerObject.notificationsNextButton = new GameObject().AddComponent<Button>();
        NotificationsManagerObject.notificationsYesButton = new GameObject().AddComponent<Button>();
        NotificationsManagerObject.notificationsNoButton = new GameObject().AddComponent<Button>();
        NotificationsManagerObject.notificationsCloseButton = new GameObject().AddComponent<Button>();     
    }
}
