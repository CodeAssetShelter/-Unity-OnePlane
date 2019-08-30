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
    public EquipButton selectButton;

    [HideInInspector]
    public int priceInt;

    public void SetGoodsInfo(Sprite sprite, string goodsName, string goodsDetail, int price)
    {
        planeImage.sprite = sprite;
        itemName.text = goodsName;
        itemDetail.text = goodsDetail;
        priceInt = price;
        this.price.text = "" + price;
    }
}
