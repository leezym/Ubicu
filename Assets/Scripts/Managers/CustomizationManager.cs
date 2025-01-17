using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;

public class CustomizationManager : MonoBehaviour
{
    public static CustomizationManager Instance {get; private set;}

    [Header("UI")]
    public UI_Screen customizeMenu_Select;
    public UI_Screen customizeMenu_Items;

    [Header("CHECK POSITIONS")]
    public Transform[] fondosItemTransform = new Transform[0];
    public Transform[] figurasItemTransform = new Transform[0];
    public GameObject checkFondosSprite;
    public GameObject checkFigurasSprite;

    [Header("DATA")]
    public ButtonsItems[] buttonsFondosItemsArray;
    public ButtonsItems[] buttonsFigurasItemsArray;
    public ButtonsItems[] buttonsFondosItemsPreviewArray;
    public ButtonsItems[] buttonsFigurasItemsPreviewArray;
    public AllItems[] allFondosItemsArray = new AllItems[5]; // cuales fondos se tienen comprados
    public AllItems[] allFigurasItemsArray = new AllItems[5]; // cuales figuras se tienen compradas
    public int[] idItemFondosArray; // cual fondo se tiene seleccionado actualmente
    public int[] idItemFigurasArray; // cual figura se tiene seleccionada actualmente
    public int tempIdFondosItem;
    public int tempIdFigurasItem;
    public int tempIdFondosItemBuy;
    public int tempIdFigurasItemBuy;
    public int tempIdCustomization;
    public TMP_Text[] priceTextItemFondosArray = new TMP_Text[3]; // textos de los precios de los items Fondos
    public TMP_Text[] priceTextItemFigurasArray = new TMP_Text[3]; // textos de los precios de los items Figuras
    public int[,] priceItemFondosArray = new int[5, 3] { 
                                                        { 0, 0, 0 }, // abstracto
                                                        { 700, 900, 900 }, // astral
                                                        { 900, 900, 1050 }, // naturaleza
                                                        { 1050, 1050, 1050 }, // mar
                                                        { 1200, 1200, 1500 }, // anatomia
                                                    };
    public int[,] priceItemFigurasArray = new int[5, 3] { 
                                                        { 0, 0, 0 }, // abstracto
                                                        { 800, 1000, 1000 }, // astral
                                                        { 1000, 1000, 1150 }, // naturaleza
                                                        { 1150, 1150, 1150 }, // mar
                                                        { 1300, 1300, 1600 }, // anatomia
                                                    };
    
    [Header("CUSTOMIZABLE OBJECTS")]
    public GameObject[] playerGameObject = new GameObject[2]; // jugadores
    public Image backgroundImage; // fondo del juego
    public Image backButtonImage; // boton regresar
    public TMP_Text[] textsArray = new TMP_Text[0]; // textos del juego
    public Image playerPreviewFondos; // preview jugador pantalla Fondos
    public Image backgroundPreviewFondos; // preview fondo pantalla Fondos
    public TMP_Text descriptionPreviewFondos; // preview descripcion pantalla Fondos
    public Image playerPreviewFiguras; // preview jugador pantalla Figuras
    public Image backgroundPreviewFiguras; // preview fondo pantalla Figuras
    public TMP_Text descriptionPreviewFiguras; // preview descripcion pantalla Figuras
    public Image[] fondosItemImageArray = new Image[0]; // imagen item Fondo
    public Image[] figurasItemImageArray = new Image[0]; // imagen item Figura
    public Image[] headerImage = new Image[3]; // son 3 Header, pantalla Fondos y pantalla Figuras
    public TMP_Text[] subtitleHeader = new TMP_Text[3]; // son 3 subtitulos del tema, pantalla Fondos y pantalla Figuras
    
    [Header("GENERAL")]
    public Sprite[] fondosSprites = new Sprite[0];
    public Sprite[] circuloSprites = new Sprite[0];
    public Sprite[] settingsSprites = new Sprite[0];
    public Color[] colorText = new Color[0];
    public Sprite[] circuloPreviewSprites = new Sprite[0];
    public string[] descriptionsFondos = new string[0];
    public string[] descriptionsFiguras = new string[0];

    [Header("ABSTRACTO")]
    public Sprite[] fondosSpritesAbstracto = new Sprite[0];
    public Sprite[] circuloSpritesAbstracto = new Sprite[0];
    public Sprite[] settingsSpritesAbstracto = new Sprite[0];
    public Color[] colorTextAbstracto = new Color[0];// {new Color(5,84,195), new Color(0,104,55), new Color(81,0,148)};
    public Sprite[] fondosItemSpritesAbstracto = new Sprite[0];
    public Sprite[] figurasItemSpritesAbstracto = new Sprite[0];
    public Sprite[] circuloPreviewSpritesAbstracto = new Sprite[0];
    public Sprite headerSpriteAbstracto;
    public string[] descriptionsFondosAbstracto = new string[0];
    public string[] descriptionsFigurasAbstracto = new string[0];

    [Header("ASTRAL")]
    public Sprite[] fondosSpritesAstral = new Sprite[0];
    public Sprite[] circuloSpritesAstral = new Sprite[0];
    public Sprite[] settingsSpritesAstral = new Sprite[0];
    public Color[] colorTextAstral = new Color[0];// {new Color(255,242,42)};
    public Sprite[] fondosItemSpritesAstral = new Sprite[0];
    public Sprite[] figurasItemSpritesAstral = new Sprite[0];
    public Sprite[] circuloPreviewSpritesAstral = new Sprite[0];
    public Sprite headerSpriteAstral;
    public string[] descriptionsFondosAstral = new string[0];
    public string[] descriptionsFigurasAstral = new string[0];
    
    [Header("NATURALEZA")]
    public Sprite[] fondosSpritesNaturaleza = new Sprite[0];
    public Sprite[] circuloSpritesNaturaleza = new Sprite[0];
    public Sprite[] settingsSpritesNaturaleza = new Sprite[0];
    public Color[] colorTextNaturaleza = new Color[0];// {new Color(5,84,195)};
    public Sprite[] fondosItemSpritesNaturaleza = new Sprite[0];
    public Sprite[] figurasItemSpritesNaturaleza = new Sprite[0];
    public Sprite[] circuloPreviewSpritesNaturaleza = new Sprite[0];
    public Sprite headerSpriteNaturaleza;
    public string[] descriptionsFondosNaturaleza = new string[0];
    public string[] descriptionsFigurasNaturaleza = new string[0];

    [Header("MAR")]
    public Sprite[] fondosSpritesMar = new Sprite[0];
    public Sprite[] circuloSpritesMar = new Sprite[0];
    public Sprite[] settingsSpritesMar = new Sprite[0];
    public Color[] colorTextMar = new Color[0];// {new Color(255,255,255)};
    public Sprite[] fondosItemSpritesMar = new Sprite[0];
    public Sprite[] figurasItemSpritesMar = new Sprite[0];
    public Sprite[] circuloPreviewSpritesMar = new Sprite[0];
    public Sprite headerSpriteMar;
    public string[] descriptionsFondosMar = new string[0];
    public string[] descriptionsFigurasMar = new string[0];

    [Header("ANATOMIA")]
    public Sprite[] fondosSpritesAnatomia = new Sprite[0];
    public Sprite[] circuloSpritesAnatomia = new Sprite[0];
    public Sprite[] settingsSpritesAnatomia = new Sprite[0];
    public Color[] colorTextAnatomia = new Color[0];// {new Color(241,25,102)};
    public Sprite[] fondosItemSpritesAnatomia = new Sprite[0];
    public Sprite[] figurasItemSpritesAnatomia = new Sprite[0];
    public Sprite[] circuloPreviewSpritesAnatomia = new Sprite[0];
    public Sprite headerSpriteAnatomia;
    public string[] descriptionsFondosAnatomia = new string[0];
    public string[] descriptionsFigurasAnatomia = new string[0];

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public IEnumerator GetCustomizations()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_patient", GameData.Instance.jsonObjectUser.user._id);
        form.AddField("token", GameData.Instance.jsonObjectUser.token);

        UnityWebRequest www = UnityWebRequest.Post(GameData.URL+"allCustomizationsByPatient", form);

        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        string responseText = www.downloadHandler.text;

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
            Debug.Log(form.data);
        }
        else
        {
            GameData.Instance.jsonObjectCustomizations = JsonConvert.DeserializeObject<Customizations>(responseText);
            UpdateLocalCustomizations(responseText);
            
            CreateCustomizations();
        }
    }

    public void CreateCustomizations()
    {
        SetIdPlayerCustomization(GameData.Instance.jsonObjectCustomizations.id_customization);
        idItemFondosArray = Array.ConvertAll(GameData.Instance.jsonObjectCustomizations.id_item_fondos_array.Split(","), int.Parse);
        idItemFigurasArray = Array.ConvertAll(GameData.Instance.jsonObjectCustomizations.id_item_figuras_array.Split(","), int.Parse);
        SetIdFondosItem(idItemFondosArray[GameData.Instance.jsonObjectCustomizations.id_customization]);
        SetIdFigurasItem(idItemFigurasArray[GameData.Instance.jsonObjectCustomizations.id_customization]);
        GetAllFondosItems();
        GetAllFigurasItems();
    }

    public string GetJsonCustomizations()
    {
        GameData.Instance.jsonObjectCustomizations.id_item_fondos_array = string.Join(",", idItemFondosArray);
        GameData.Instance.jsonObjectCustomizations.id_item_figuras_array = string.Join(",", idItemFigurasArray);
        SetAllFondosItems();
        SetAllFigurasItems();

        return JsonConvert.SerializeObject(GameData.Instance.jsonObjectCustomizations);
    }
    public void SendCustomizations()
    {      
        string jsonData = GetJsonCustomizations();

        UpdateLocalCustomizations(jsonData);

        if(!Login.Instance.notInternet.isOn)
        {            
            StartCoroutine(UpdateCustomizations(jsonData));
        }
    }

    public void UpdateLocalCustomizations(string jsonData)
    {
        File.WriteAllText(GameData.Instance.ObtenerRutaPersonalizacion(GameData.Instance.jsonObjectUser.user.cedula), jsonData);

        Debug.Log("Datos de personalizaciones locales actualizados correctamente");
    }

    public IEnumerator UpdateCustomizations(string jsonData)
    {
        UnityWebRequest www = UnityWebRequest.Put(GameData.URL+"updateCustomizations", jsonData);

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("x-access-token", GameData.Instance.jsonObjectUser.token);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Datos de personalización actualizados correctamente");
        }
    }

    public void SetTempIdFondosItem(int id)
    {
        tempIdFondosItem = id;
        backgroundPreviewFondos.sprite = fondosSprites[id];
        descriptionPreviewFondos.text = descriptionsFondos[id];
        buttonsFondosItemsPreviewArray[0].buyButton.transform.Find("Price").GetComponent<TMP_Text>().text = priceItemFondosArray[GameData.Instance.jsonObjectCustomizations.id_customization , id].ToString();

        if(idItemFigurasArray[GameData.Instance.jsonObjectCustomizations.id_customization] < 0) // si no ha comprado nada debe salir la combinacion con el item 0
            playerPreviewFondos.sprite = circuloPreviewSprites[idItemFigurasArray[0]];
        else
            playerPreviewFondos.sprite = circuloPreviewSprites[idItemFigurasArray[GameData.Instance.jsonObjectCustomizations.id_customization]];

        if(allFondosItemsArray[GameData.Instance.jsonObjectCustomizations.id_customization].item[id] == 0)
        {
            buttonsFondosItemsPreviewArray[0].useButton.SetActive(false);
            buttonsFondosItemsPreviewArray[0].buyButton.SetActive(true);
        }
        else
        {
            buttonsFondosItemsPreviewArray[0].useButton.SetActive(true);
            buttonsFondosItemsPreviewArray[0].buyButton.SetActive(false);
        }
    }
    public void SetTempIdFigurasItem(int id)
    {
        tempIdFigurasItem = id;
        playerPreviewFiguras.sprite = circuloPreviewSprites[id];
        descriptionPreviewFiguras.text = descriptionsFiguras[id];
        buttonsFigurasItemsPreviewArray[0].buyButton.transform.Find("Price").GetComponent<TMP_Text>().text = priceItemFigurasArray[GameData.Instance.jsonObjectCustomizations.id_customization , id].ToString();

        if(idItemFondosArray[GameData.Instance.jsonObjectCustomizations.id_customization] < 0) // si no ha comprado nada debe salir la combinacion con el item 0
            backgroundPreviewFiguras.sprite = fondosSprites[idItemFondosArray[0]];
        else
            backgroundPreviewFiguras.sprite = fondosSprites[idItemFondosArray[GameData.Instance.jsonObjectCustomizations.id_customization]];

        if(allFigurasItemsArray[GameData.Instance.jsonObjectCustomizations.id_customization].item[id] == 0) // si no ha comprado sale el boton de comprar activo
        {     
            buttonsFigurasItemsPreviewArray[0].buyButton.SetActive(true);
            buttonsFigurasItemsPreviewArray[0].useButton.SetActive(false);
        }
        else
        {
            buttonsFigurasItemsPreviewArray[0].buyButton.SetActive(false);
            buttonsFigurasItemsPreviewArray[0].useButton.SetActive(true);
        }
    }

    public void SetIdFondosItem() { SetIdFondosItem(tempIdFondosItem); }
    public void SetIdFondosItem(int id)
    {
        if(id >= 0)
        {
            backgroundImage.sprite = fondosSprites[id];
            backButtonImage.color = colorText[id];
            foreach(TMP_Text t in textsArray)
                t.color = colorText[id];
            checkFondosSprite.SetActive(true);
            checkFondosSprite.transform.parent = fondosItemTransform[id];
            checkFondosSprite.transform.localPosition = new Vector2(0,0);
            idItemFondosArray[GameData.Instance.jsonObjectCustomizations.id_customization] = id;
        }
        else
        {
            checkFondosSprite.SetActive(false);
        }
    }

    public void SetIdFigurasItem() { SetIdFigurasItem(tempIdFigurasItem); }
    public void SetIdFigurasItem(int id)
    {
        if(id >= 0) // verificar si esta en modo circulo  o pulmones
        {
            playerGameObject[0].transform.Find("Circle").GetComponent<Image>().sprite = circuloSprites[id]; //circle 
            playerGameObject[1].transform.Find("Lung").GetComponent<Image>().sprite = circuloSprites[id]; //lung
            checkFigurasSprite.SetActive(true);
            checkFigurasSprite.transform.parent = figurasItemTransform[id];
            checkFigurasSprite.transform.localPosition = new Vector2(0,0);
            idItemFigurasArray[GameData.Instance.jsonObjectCustomizations.id_customization] = id;
        }
        else
        {
            checkFigurasSprite.SetActive(false);
        }
    }

    public void SetIdCustomization(int id)
    {
        tempIdCustomization = GameData.Instance.jsonObjectCustomizations.id_customization;
        GameData.Instance.jsonObjectCustomizations.id_customization = id;

        if(id == 0)
            SetArrayCustomization("Abstracto", headerSpriteAbstracto, fondosItemSpritesAbstracto, figurasItemSpritesAbstracto, circuloPreviewSpritesAbstracto, descriptionsFondosAbstracto, descriptionsFigurasAbstracto);
        else if (id == 1)
            SetArrayCustomization("Astral", headerSpriteAstral, fondosItemSpritesAstral, figurasItemSpritesAstral, circuloPreviewSpritesAstral, descriptionsFondosAstral, descriptionsFigurasAstral);
        else if (id == 2)
            SetArrayCustomization("Naturaleza", headerSpriteNaturaleza, fondosItemSpritesNaturaleza, figurasItemSpritesNaturaleza, circuloPreviewSpritesNaturaleza, descriptionsFondosNaturaleza, descriptionsFigurasNaturaleza);        
        else if (id == 3)
            SetArrayCustomization("Mar", headerSpriteMar, fondosItemSpritesMar, figurasItemSpritesMar, circuloPreviewSpritesMar, descriptionsFondosMar, descriptionsFigurasMar);
        else if (id == 4)
            SetArrayCustomization("Anatomía", headerSpriteAnatomia, fondosItemSpritesAnatomia, figurasItemSpritesAnatomia, circuloPreviewSpritesAnatomia, descriptionsFondosAnatomia, descriptionsFigurasAnatomia);

        ValidateFullItems(id);

        // colocar los precios
        for(int i = 0 ; i < priceTextItemFondosArray.Length; i++) // o priceTextItemFigurasArray
        {
            priceTextItemFondosArray[i].text = priceItemFondosArray[id , i].ToString();
            priceTextItemFigurasArray[i].text = priceItemFigurasArray[id , i].ToString();
        }        
    }

    public void SetArrayCustomization(string subtitleHeaderText, Sprite headerSprite, Sprite[] fondosItemSprites, Sprite[] figurasItemSprites,
                                    Sprite[] circuloPreviewSprites, string[] descriptionsFondos, string[] descriptionsFiguras)
    {
        this.circuloPreviewSprites = circuloPreviewSprites.Clone() as Sprite[];
        this.descriptionsFondos = descriptionsFondos.Clone() as string[];
        this.descriptionsFiguras = descriptionsFiguras.Clone() as string[];

        // Set Custom Item Menu
        foreach(Image i in headerImage)
            i.sprite = headerSprite;

        foreach(TMP_Text t in subtitleHeader)
            t.text = subtitleHeaderText;

        for(int i = 0; i < fondosItemImageArray.Length; i++) //or figurasItemImageArray.Length
        {
            fondosItemImageArray[i].sprite = fondosItemSprites[i];
            figurasItemImageArray[i].sprite = figurasItemSprites[i];
        }
    }

    public void ValidateFullItems(int id)
    {
        bool tempEnabled;
        for(int j = 0; j < allFondosItemsArray[id].item.Length; j++)
        {
            if(allFondosItemsArray[id].item[j] == 0)
                tempEnabled = true;
            else
                tempEnabled = false;
            buttonsFondosItemsArray[j].buyButton.SetActive(tempEnabled);
            buttonsFondosItemsArray[j].useButton.SetActive(!tempEnabled);
        }

        for(int j = 0; j < allFigurasItemsArray[id].item.Length; j++)
        {
            if(allFigurasItemsArray[id].item[j] == 0)
                tempEnabled = true;
            else
                tempEnabled = false;
            buttonsFigurasItemsArray[j].buyButton.SetActive(tempEnabled);
            buttonsFigurasItemsArray[j].useButton.SetActive(!tempEnabled);  
        }
    }

    public void GetAllFondosItems()
    {
        string[] temp;
        temp = Array.ConvertAll(GameData.Instance.jsonObjectCustomizations.all_fondos_items_array.Split(";"), x => x.ToString());
        for(int i = 0; i < allFondosItemsArray.Length; i++)
        {
            allFondosItemsArray[i].item = Array.ConvertAll(temp[i].Split(","), int.Parse);
        }
    }
    public void SetAllFondosItems()
    {        
        GameData.Instance.jsonObjectCustomizations.all_fondos_items_array = "";
        for(int i = 0; i < allFondosItemsArray.Length; i++)
        {
            GameData.Instance.jsonObjectCustomizations.all_fondos_items_array += string.Join(",", allFondosItemsArray[i].item)+";";
        }
    }
    
    public void GetAllFigurasItems()
    {
        string[] temp;
        temp = Array.ConvertAll(GameData.Instance.jsonObjectCustomizations.all_figuras_items_array.Split(";"), x => x.ToString());
        for(int i = 0; i < allFigurasItemsArray.Length; i++)
        {
            allFigurasItemsArray[i].item = Array.ConvertAll(temp[i].Split(","), int.Parse);
        }
    }
    public void SetAllFigurasItems()
    {
        GameData.Instance.jsonObjectCustomizations.all_figuras_items_array = "";
        for(int i = 0; i < allFigurasItemsArray.Length; i++)
        {
            GameData.Instance.jsonObjectCustomizations.all_figuras_items_array += string.Join(",", allFigurasItemsArray[i].item)+";";
        }
    }

    public void BuyFondo() { BuyFondo(tempIdFondosItem); }
    public void BuyFondo(int id)
    {
        if(priceItemFondosArray[GameData.Instance.jsonObjectCustomizations.id_customization , id] <= GameData.Instance.jsonObjectRewards.total_reward)
        {
            NotificationsManager.Instance.QuestionNotifications("¿Seguro que quieres obtener este fondo?");
            // si
            NotificationsManager.Instance.SetYesButton(()=>{
                allFondosItemsArray[GameData.Instance.jsonObjectCustomizations.id_customization].item[id] = 1;
                GameData.Instance.jsonObjectRewards.total_reward -= priceItemFondosArray[GameData.Instance.jsonObjectCustomizations.id_customization , id];
                buttonsFondosItemsPreviewArray[0].useButton.SetActive(true);
                buttonsFondosItemsPreviewArray[0].buyButton.SetActive(false);
                ValidateFullItems(GameData.Instance.jsonObjectCustomizations.id_customization);
                SendCustomizations();                
            });
        }
        else
        {
            //notificacion de que no alcanza
            NotificationsManager.Instance.WarningNotifications("No tienes UbiCoins suficientes para comprar el fondo.\n¡Sigue haciendo tu fisioterapia!");
            NotificationsManager.Instance.SetCloseFunction();
        }
    }

    public void BuyFigura() { BuyFigura(tempIdFigurasItem); }
    public void BuyFigura(int id)
    {
        if(priceItemFigurasArray[GameData.Instance.jsonObjectCustomizations.id_customization , id] <= GameData.Instance.jsonObjectRewards.total_reward)
        {
            // notificacion de preguntar
            NotificationsManager.Instance.QuestionNotifications("¿Seguro que quieres obtener esta figura?");
            // si
            NotificationsManager.Instance.SetYesButton(()=>{
                allFigurasItemsArray[GameData.Instance.jsonObjectCustomizations.id_customization].item[id] = 1;
                GameData.Instance.jsonObjectRewards.total_reward -= priceItemFigurasArray[GameData.Instance.jsonObjectCustomizations.id_customization , id];
                buttonsFigurasItemsPreviewArray[0].useButton.SetActive(true);
                buttonsFigurasItemsPreviewArray[0].buyButton.SetActive(false);
                ValidateFullItems(GameData.Instance.jsonObjectCustomizations.id_customization);
                SendCustomizations();
            });
        }
        else
        {
            //notificacion de que no alcanza
            NotificationsManager.Instance.WarningNotifications("No tienes UbiCoins suficientes para comprar la figura.\n¡Sigue haciendo tu fisioterapia!");
            NotificationsManager.Instance.SetCloseFunction();
        }
    }

    public void VerifyCompleteTheme()
    {
        int fondoSum = 0;
        int[] fondoItems = allFondosItemsArray[GameData.Instance.jsonObjectCustomizations.id_customization].item;
        for (int i = 0; i < fondoItems.Length; i++)
        {
            fondoSum += fondoItems[i];
        }

        int figuraSum = 0;
        int[] figuraItems = allFigurasItemsArray[GameData.Instance.jsonObjectCustomizations.id_customization].item;
        for (int i = 0; i < figuraItems.Length; i++)
        {
            figuraSum += figuraItems[i];
        }

        if (fondoSum == 0 || figuraSum == 0)
        {
            // notificacion de no poder colocar el tema por UbiCoins
            NotificationsManager.Instance.WarningNotifications("No tienes UbiCoins suficientes para completar el tema.\n¡Sigue haciendo tu fisioterapia!");
            NotificationsManager.Instance.SetCloseFunction(customizeMenu_Select);
            GameData.Instance.jsonObjectCustomizations.id_customization = tempIdCustomization;
        }
        else if (idItemFondosArray[GameData.Instance.jsonObjectCustomizations.id_customization] >= 0 && idItemFigurasArray [GameData.Instance.jsonObjectCustomizations.id_customization] >= 0)
        {
            //notificacion de tema seleccionado
            NotificationsManager.Instance.WarningNotifications("¡Tema seleccionado!");
            NotificationsManager.Instance.SetCloseFunction(customizeMenu_Select);
            SetIdPlayerCustomization(GameData.Instance.jsonObjectCustomizations.id_customization);
            SendCustomizations();
        }
        else
        {
            // notificacion de no poder colocar el tema porque no ha seleccionado un item
            NotificationsManager.Instance.WarningNotifications("Por favor selecciona un fondo y una figura para usar el tema.");
            GameData.Instance.jsonObjectCustomizations.id_customization = tempIdCustomization;
        }
    }

    void SetIdPlayerCustomization(int id)
    {
        if(id == 0)
            SetPlayerCustomization("Abstracto", fondosSpritesAbstracto, circuloSpritesAbstracto, settingsSpritesAbstracto, colorTextAbstracto);
        else if (id == 1)
            SetPlayerCustomization("Astral", fondosSpritesAstral, circuloSpritesAstral, settingsSpritesAstral, colorTextAstral);
        else if (id == 2)
            SetPlayerCustomization("Naturaleza", fondosSpritesNaturaleza, circuloSpritesNaturaleza, settingsSpritesNaturaleza, colorTextNaturaleza);
        else if (id == 3)
            SetPlayerCustomization("Mar", fondosSpritesMar, circuloSpritesMar, settingsSpritesMar, colorTextMar);
        else if (id == 4)
            SetPlayerCustomization("Anatomía", fondosSpritesAnatomia, circuloSpritesAnatomia, settingsSpritesAnatomia, colorTextAnatomia);

        if (idItemFondosArray.Length > 0) SetIdFondosItem(idItemFondosArray[id]);
        if (idItemFigurasArray.Length > 0) SetIdFigurasItem(idItemFigurasArray[id]);   
    }

    void SetPlayerCustomization(string subtitleHeaderText, Sprite[] fondosSprites, Sprite[] circuloSprites, Sprite[] settingsSprites, Color[] colorText)
    {
        if(subtitleHeaderText == "Anatomía")
        {
            playerGameObject[0].SetActive(false); //circle
            playerGameObject[1].SetActive(true); //lung
            PlayerMovement.Instance.minimunScale = PlayerMovement.LUNG_MINIMUM_SCALE;
            PlayerMovement.Instance.maximunScale = PlayerMovement.LUNG_MAXIMUM_SCALE;
            PlayerMovement.Instance.plusScale = PlayerMovement.LUNG_PLUS_SCALE;
            PlayerMovement.Instance.player = playerGameObject[1].transform.Find("Lung").gameObject;
        }
        else
        {
            playerGameObject[0].SetActive(true); //circle
            playerGameObject[1].SetActive(false); //lung
            PlayerMovement.Instance.minimunScale = PlayerMovement.CIRCLE_MINIMUM_SCALE;
            PlayerMovement.Instance.maximunScale = PlayerMovement.CIRCLE_MAXIMUM_SCALE;
            PlayerMovement.Instance.plusScale = PlayerMovement.CIRCLE_PLUS_SCALE;
            PlayerMovement.Instance.player = playerGameObject[0].transform.Find("Circle").gameObject;
        }

        this.fondosSprites = fondosSprites.Clone() as Sprite[];
        this.circuloSprites = circuloSprites.Clone() as Sprite[];
        this.settingsSprites = settingsSprites.Clone() as Sprite[];
        this.colorText = colorText.Clone() as Color[];
    }
}