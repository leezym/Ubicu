using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizationManager : MonoBehaviour
{
    [Header("CHECK POSITIONS")]
    public Transform[] fondosItemTransform = new Transform[0];
    public Transform[] figurasItemTransform = new Transform[0];
    public GameObject checkFondosSprite;
    public GameObject checkFigurasSprite;

    [Header("CUSTOMIZABLE OBJECTS")]
    public Image playerImage;
    public Image backgroundImage;
    public Image settingsImage;
    public TMP_Text[] texts = new TMP_Text[0];
    public Image playerPreviewFondos;
    public Image backgroundPreviewFondos;
    public TMP_Text descriptionPreviewFondos;
    public Image playerPreviewFiguras;
    public Image backgroundPreviewFiguras;
    public TMP_Text descriptionPreviewFiguras;

    [Header("GENERAL")]
    public Sprite[] fondosSprites = new Sprite[0];
    public Sprite[] circuloSprites = new Sprite[0];
    public Sprite[] velocimetroSprites = new Sprite[0];
    public Sprite[] settingsSprites = new Sprite[0];
    public Color[] colorText = new Color[0];
    public Sprite[] circuloPreviewSprites = new Sprite[0];
    public string[] descriptionsFondos = new string[0];
    public string[] descriptionsFiguras = new string[0];
    public int[] idItemFondosArray;
    public int[] idItemFigurasArray;
    public int tempIdFondosItem;
    public int tempIdFigurasItem;

    [Header("ABSTRACTO")]
    public Sprite[] fondosSpritesAbstracto = new Sprite[0];
    public Sprite[] circuloSpritesAbstracto = new Sprite[0];
    public Sprite[] velocimetroSpritesAbstracto = new Sprite[0];
    public Sprite[] settingsSpritesAbstracto = new Sprite[0];
    public Color[] colorTextAbstracto = new Color[0];// {new Color(5,84,195), new Color(0,104,55), new Color(81,0,148)};
    public Sprite[] circuloPreviewSpritesAbstracto = new Sprite[0];
    public string[] descriptionsFondosAbstracto = new string[0];
    public string[] descriptionsFigurasAbstracto = new string[0];

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
        backgroundImage.sprite = fondosSprites[id];
        settingsImage.sprite = settingsSprites[id];
        foreach(TMP_Text t in texts)
            t.color = colorText[id];
        checkFondosSprite.transform.parent = fondosItemTransform[id];
        checkFondosSprite.transform.localPosition = new Vector2(0,0);
        idItemFondosArray[PlayerPrefs.GetInt("idCustomization")] = id;
        PlayerPrefs.SetString("idItemFondosArray", string.Join(",", idItemFondosArray));
    }

    public void SetIdFigurasItem() { SetIdFigurasItem(tempIdFigurasItem); }
    public void SetIdFigurasItem(int id)
    {        
        playerImage.sprite = circuloSprites[id];
        checkFigurasSprite.transform.parent = figurasItemTransform[id];
        checkFigurasSprite.transform.localPosition = new Vector2(0,0);
        idItemFigurasArray[PlayerPrefs.GetInt("idCustomization")] = id;
        PlayerPrefs.SetString("idItemFigurasArray", string.Join(",", idItemFigurasArray));
    }

    public void SetIdCustomization(int id)
    {
        PlayerPrefs.SetInt("idCustomization", id);
        if(id == 0)
        {
            fondosSprites = fondosSpritesAbstracto.Clone() as Sprite[];
            circuloSprites = circuloSpritesAbstracto.Clone() as Sprite[];
            velocimetroSprites = velocimetroSpritesAbstracto.Clone() as Sprite[];
            settingsSprites = settingsSpritesAbstracto.Clone() as Sprite[];
            colorText = colorTextAbstracto.Clone() as Color[];
            circuloPreviewSprites = circuloPreviewSpritesAbstracto.Clone() as Sprite[];
            descriptionsFondos = descriptionsFondosAbstracto.Clone() as string[];
            descriptionsFiguras = descriptionsFigurasAbstracto.Clone() as string[];
        }
        else if (id == 1)
        {

        }
        else if (id == 2)
        {

        }
        else if (id == 3)
        {

        }
        else if (id == 4)
        {

        }
    }

}
