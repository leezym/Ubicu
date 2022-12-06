using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ButtonsItems
{
    public GameObject useButton;
    public GameObject buyButton;
}

[System.Serializable]
public class AllItems
{
    public int[] item = new int[0];
}

public class CustomizationManager : MonoBehaviour
{
    [Header("CHECK POSITIONS")]
    public Transform[] fondosItemTransform = new Transform[0];
    public Transform[] figurasItemTransform = new Transform[0];
    public GameObject checkFondosSprite;
    public GameObject checkFigurasSprite;

    [Header("DATA")]
    public ButtonsItems[] buttonsFondosItemsArray;
    public ButtonsItems[] buttonsFigurasItemsArray;
    public AllItems[] allFondosItemsArray = new AllItems[5]; // cuales fondos se tienen comprados
    public AllItems[] allFigurasItemsArray = new AllItems[5]; // cuales figuras se tienen compradas
    public int[] idItemFondosArray; // cual fondo se tiene seleccionado actualmente
    public int[] idItemFigurasArray; // cual figura se tiene seleccionada actualmente
    public int tempIdFondosItem;
    public int tempIdFigurasItem;
    public int idCustomization;
    public int tempIdCustomization;
    public TMP_Text[] priceTextItemFondosArray = new TMP_Text[3]; // textos de los precios de los items Fondos
    public TMP_Text[] priceTextItemFigurasArray = new TMP_Text[3]; // textos de los precios de los items Figuras
    public int[,] priceItemFondosArray = new int[5, 3] { 
                                                        { 0, 0, 0 }, // abstracto
                                                        { 700, 900, 900 }, // astral
                                                        { 900, 900, 1050 }, // naturaleza
                                                        { 1050, 1050, 1050 }, // mar
                                                        { 0, 0, 0 }, // anatomia
                                                    };
    public int[,] priceItemFigurasArray = new int[5, 3] { 
                                                        { 0, 0, 0 }, // abstracto
                                                        { 800, 1000, 1000 }, // astral
                                                        { 1000, 1000, 1150 }, // naturaleza
                                                        { 1150, 1150, 1150 }, // mar
                                                        { 0, 0, 0 }, // anatomia
                                                    };
    
    [Header("CUSTOMIZABLE OBJECTS")]
    public GameObject[] playerImage = new GameObject[2]; // jugadores
    public Image backgroundImage; // fondo del juego
    public Image settingsImage; // boton configuracion
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
    public Sprite[] velocimetroSprites = new Sprite[0];
    public Sprite[] settingsSprites = new Sprite[0];
    public Color[] colorText = new Color[0];
    public Sprite[] circuloPreviewSprites = new Sprite[0];
    public string[] descriptionsFondos = new string[0];
    public string[] descriptionsFiguras = new string[0];

    [Header("ABSTRACTO")]
    public Sprite[] fondosSpritesAbstracto = new Sprite[0];
    public Sprite[] circuloSpritesAbstracto = new Sprite[0];
    public Sprite[] velocimetroSpritesAbstracto = new Sprite[0];
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
    public Sprite[] velocimetroSpritesAstral = new Sprite[0];
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
    public Sprite[] velocimetroSpritesNaturaleza = new Sprite[0];
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
    public Sprite[] velocimetroSpritesMar = new Sprite[0];
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
    public Sprite[] velocimetroSpritesAnatomia = new Sprite[0];
    public Sprite[] settingsSpritesAnatomia = new Sprite[0];
    public Color[] colorTextAnatomia = new Color[0];// {new Color(241,25,102)};
    public Sprite[] fondosItemSpritesAnatomia = new Sprite[0];
    public Sprite[] figurasItemSpritesAnatomia = new Sprite[0];
    public Sprite[] circuloPreviewSpritesAnatomia = new Sprite[0];
    public Sprite headerSpriteAnatomia;
    public string[] descriptionsFondosAnatomia = new string[0];
    public string[] descriptionsFigurasAnatomia = new string[0];


    public void SetTempIdFondosItem(int id)
    {
        tempIdFondosItem = id;
        backgroundPreviewFondos.sprite = fondosSprites[id];
        playerPreviewFondos.sprite = circuloPreviewSprites[PlayerPrefs.GetInt("idItemFiguras")];
        // descripcion
    }
    public void SetTempIdFigurasItem(int id)
    {
        tempIdFigurasItem = id;
        backgroundPreviewFiguras.sprite = fondosSprites[PlayerPrefs.GetInt("idItemFondos")];
        playerPreviewFiguras.sprite = circuloPreviewSprites[id];
        // descripcion
    }

    public void SetIdFondosItem() { SetIdFondosItem(tempIdFondosItem); }
    public void SetIdFondosItem(int id)
    {
        if(id >= 0)
        {
            backgroundImage.sprite = fondosSprites[id];
            settingsImage.sprite = settingsSprites[id];
            foreach(TMP_Text t in textsArray)
                t.color = colorText[id];
            checkFondosSprite.SetActive(true);
            checkFondosSprite.transform.parent = fondosItemTransform[id];
            checkFondosSprite.transform.localPosition = new Vector2(0,0);
            idItemFondosArray[idCustomization] = id;
        }
        else
        {
            checkFondosSprite.SetActive(false);
        }
    }

    public void SetIdFigurasItem() { SetIdFigurasItem(tempIdFigurasItem); }
    public void SetIdFigurasItem(int id)
    {
        if(id >= 0)
        {
            playerImage[0].transform.Find("Player").GetComponent<Image>().sprite = circuloSprites[id];
            playerImage[1].transform.Find("Player").GetComponent<Image>().sprite = circuloSprites[id];
            checkFigurasSprite.SetActive(true);
            checkFigurasSprite.transform.parent = figurasItemTransform[id];
            checkFigurasSprite.transform.localPosition = new Vector2(0,0);
            idItemFigurasArray[idCustomization] = id;
        }
        else
        {
            checkFigurasSprite.SetActive(false);
        }
    }

    public void SetIdCustomization(int id)
    {
        if(id == 0)
            SetArrayCustomization("Abstracto", headerSpriteAbstracto, fondosItemSpritesAbstracto, figurasItemSpritesAbstracto, fondosSpritesAbstracto, circuloSpritesAbstracto, velocimetroSpritesAbstracto, settingsSpritesAbstracto, colorTextAbstracto, circuloPreviewSpritesAbstracto, descriptionsFondosAbstracto, descriptionsFigurasAbstracto);
        else if (id == 1)
            SetArrayCustomization("Astral", headerSpriteAstral, fondosItemSpritesAstral, figurasItemSpritesAstral, fondosSpritesAstral, circuloSpritesAstral, velocimetroSpritesAstral, settingsSpritesAstral, colorTextAstral, circuloPreviewSpritesAstral, descriptionsFondosAstral, descriptionsFigurasAstral);
        else if (id == 2)
            SetArrayCustomization("Naturaleza", headerSpriteNaturaleza, fondosItemSpritesNaturaleza, figurasItemSpritesNaturaleza, fondosSpritesNaturaleza, circuloSpritesNaturaleza, velocimetroSpritesNaturaleza, settingsSpritesNaturaleza, colorTextNaturaleza, circuloPreviewSpritesNaturaleza, descriptionsFondosNaturaleza, descriptionsFigurasNaturaleza);        
        else if (id == 3)
            SetArrayCustomization("Mar", headerSpriteMar, fondosItemSpritesMar, figurasItemSpritesMar, fondosSpritesMar, circuloSpritesMar, velocimetroSpritesMar, settingsSpritesMar, colorTextMar, circuloPreviewSpritesMar, descriptionsFondosMar, descriptionsFigurasMar);
        else if (id == 4)
            SetArrayCustomization("Anatomía", headerSpriteAnatomia, fondosItemSpritesAnatomia, figurasItemSpritesAnatomia, fondosSpritesAnatomia, circuloSpritesAnatomia, velocimetroSpritesAnatomia, settingsSpritesAnatomia, colorTextAnatomia, circuloPreviewSpritesAnatomia, descriptionsFondosAnatomia, descriptionsFigurasAnatomia);

        if (idItemFondosArray.Length > 0) SetIdFondosItem(idItemFondosArray[id]);
        if (idItemFigurasArray.Length > 0) SetIdFigurasItem(idItemFigurasArray[id]);

        // colocar los precios
        for(int i = 0 ; i < priceTextItemFondosArray.Length; i++) // o priceTextItemFigurasArray
        {
            priceTextItemFondosArray[i].text = priceItemFondosArray[id , i].ToString();
            priceTextItemFigurasArray[i].text = priceItemFigurasArray[id , i].ToString();
        }
        
        ValidateFullItems(id);
        tempIdCustomization = idCustomization;
        idCustomization = id;
    }

    public void SetArrayCustomization(string subtitleHeaderText, Sprite headerSprite, Sprite[] fondosItemSprites, Sprite[] figurasItemSprites, Sprite[] fondosSprites, Sprite[] circuloSprites, Sprite[] velocimetroSprites, Sprite[] settingsSprites, 
                                    Color[] colorText, Sprite[] circuloPreviewSprites, string[] descriptionsFondos, string[] descriptionsFiguras)
    {
        if(subtitleHeaderText == "Anatomía")
        {
            playerImage[0].SetActive(false); //circle
            playerImage[1].SetActive(true); //lung
        }
        else
        {
            playerImage[0].SetActive(true); //circle
            playerImage[1].SetActive(false); //lung
        }
        this.fondosSprites = fondosSprites.Clone() as Sprite[];
        this.circuloSprites = circuloSprites.Clone() as Sprite[];
        this.velocimetroSprites = velocimetroSprites.Clone() as Sprite[];
        this.settingsSprites = settingsSprites.Clone() as Sprite[];
        this.colorText = colorText.Clone() as Color[];
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
        temp = Array.ConvertAll(PlayerPrefs.GetString("allFondosItemsArray").Split(";"), x => x.ToString());
        for(int i = 0; i < allFondosItemsArray.Length; i++)
        {
            allFondosItemsArray[i].item = Array.ConvertAll(temp[i].Split(","), int.Parse);
        }
    }
    public void SetAllFondosItems()
    {        
        string s = "";
        for(int i = 0; i < allFondosItemsArray.Length; i++)
        {
            s += string.Join(",", allFondosItemsArray[i].item)+";";
        }
        PlayerPrefs.SetString("allFondosItemsArray", s);
    }
    
    public void GetAllFigurasItems()
    {
        string[] temp;
        temp = Array.ConvertAll(PlayerPrefs.GetString("allFigurasItemsArray").Split(";"), x => x.ToString());
        for(int i = 0; i < allFigurasItemsArray.Length; i++)
        {
            allFigurasItemsArray[i].item = Array.ConvertAll(temp[i].Split(","), int.Parse);
        }
    }
    public void SetAllFigurasItems()
    {
        string s = "";
        for(int i = 0; i < allFigurasItemsArray.Length; i++)
        {
            s += string.Join(",", allFigurasItemsArray[i].item)+";";
        }
        PlayerPrefs.SetString("allFigurasItemsArray", s);
    }   
    
    public void BuyFondo(int id)
    {
        if(priceItemFondosArray[idCustomization , id] <= GameData.Instance.scriptsGroup.rewardsManager.totalReward)
        {
            // notificacion de preguntar pdte
                // si
                allFondosItemsArray[idCustomization].item[id] = 1;
                GameData.Instance.scriptsGroup.rewardsManager.totalReward -= priceItemFondosArray[idCustomization , id];
        }
        else
        {
            //notificacion de que no alcanza
            NotificationsManager.Instance.WarningNotifications("No tienes dinero suficiente para comprar el fondo.\n¡Sigue haciendo tus terapias!");
            NotificationsManager.Instance.SetCloseFunction(null);
        }
        ValidateFullItems(idCustomization);
    }

    public void BuyFigura(int id)
    {
        if(priceItemFigurasArray[idCustomization , id] <= GameData.Instance.scriptsGroup.rewardsManager.totalReward)
        {
            // notificacion de preguntar
                // si
                allFigurasItemsArray[idCustomization].item[id] = 1;
                GameData.Instance.scriptsGroup.rewardsManager.totalReward -= priceItemFigurasArray[idCustomization , id];
        }
        else
        {
            //notificacion de que no alcanza
            NotificationsManager.Instance.WarningNotifications("No tienes dinero suficiente para comprar la figura.\n¡Sigue haciendo tus terapias!");
            NotificationsManager.Instance.SetCloseFunction(null);
        }
        ValidateFullItems(idCustomization);
    }

    public void SaveCustomization()
    {
        PlayerPrefs.SetInt("idCustomization", idCustomization);
        PlayerPrefs.SetString("idItemFondosArray", string.Join(",", idItemFondosArray));
        PlayerPrefs.SetString("idItemFigurasArray", string.Join(",", idItemFigurasArray));
        SetAllFondosItems();
        SetAllFigurasItems();
    }

    public void LoadCustomization()
    {
        idCustomization = PlayerPrefs.GetInt("idCustomization");
        SetIdCustomization(idCustomization);
        idItemFondosArray = Array.ConvertAll(PlayerPrefs.GetString("idItemFondosArray").Split(","), int.Parse);
        idItemFigurasArray = Array.ConvertAll(PlayerPrefs.GetString("idItemFigurasArray").Split(","), int.Parse);
        SetIdFondosItem(idItemFondosArray[idCustomization]);
        SetIdFigurasItem(idItemFigurasArray[idCustomization]);
        GetAllFondosItems();
        GetAllFigurasItems();
    }

    public void VerifyCompleteTheme()
    {
        if (allFondosItemsArray[idCustomization].item.Sum() == 0 || allFigurasItemsArray[idCustomization].item.Sum() == 0)
        {
            // notificacion de no poder colocar el tema por dinero
            NotificationsManager.Instance.WarningNotifications("No tienes dinero suficiente para completar el tema.\n¡Sigue haciendo tus terapias!");
            NotificationsManager.Instance.SetCloseFunction(GameData.Instance.customizeMenu_Select);
            idCustomization = tempIdCustomization;
        }
        else if (idItemFondosArray[idCustomization] >= 0 && idItemFigurasArray [idCustomization] >= 0)
        {
            UI_System.Instance.SwitchScreens(GameData.Instance.customizeMenu_Select);
        }
        else
        {
            // notificacion de no poder colocar el tema porque no ha seleccionado un item
            NotificationsManager.Instance.WarningNotifications("Selecciona un fondo y una figura.");
            NotificationsManager.Instance.SetCloseFunction(null);
            idCustomization = tempIdCustomization;
        }
    }
}
