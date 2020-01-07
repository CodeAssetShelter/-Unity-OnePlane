using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goods : MonoBehaviour
{
    public Image planeImage;
    public Button purchaseButton;
    public Image purchasedImage;
    public Text itemName;
    public Text itemDetail;
    public Text price;
    public Text lifeValue;
    public Text shieldTimeValue;
    public Text itemSlotValue;
    public EquipButton selectButton;

    [HideInInspector]
    public int priceInt;

    public void SetGoodsInfo(Sprite sprite, string goodsName, string goodsDetail, int price, int lifeValue, int shieldTimeValue, int itemSlotValue)
    {
        planeImage.sprite = sprite;
        itemName.text = goodsName;
        itemDetail.text = goodsDetail;
        priceInt = price;
        this.lifeValue.text = " " + lifeValue;
        this.shieldTimeValue.text = " " + shieldTimeValue;
        this.itemSlotValue.text = " " + itemSlotValue;
        this.price.text = "" + price;
    }
}
