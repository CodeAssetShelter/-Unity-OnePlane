using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public GameObject content;
    public GameObject goodsPrefab;

    public Planes planes;
    private Goods selected;

    public Text credit;

    private void OnEnable()
    {
        credit.text = "" + PublicValueStorage.Instance.RefreshCredit(0);

        if (content.transform.childCount >= 1)
        {
            return;
        }

        for(int i = 0; i < planes.goodsInfo.Length; i++)
        {
            GameObject temp = Instantiate(goodsPrefab, content.transform);
            Goods info = temp.GetComponent<Goods>();
            int selectedIndex = -1;

            info.SetGoodsInfo
                (planes.planeSprite[i], planes.goodsInfo[i].name, 
                planes.goodsInfo[i].detail, planes.goodsInfo[i].price,
                planes.goodsInfo[i].life, (int)planes.goodsInfo[i].shield,
                planes.goodsInfo[i].itemSlot);
            if (planes.goodsInfo[i].purchased == true)
            {
                PurchasedAlready(info);
            }
            if (planes.goodsInfo[i].selected == true)
            {
                if (selectedIndex == -1)
                {
                    selectedIndex = i;
                    planes.selectedIndex = selectedIndex;
                    selected = info;
                }
                else
                {
                    // Change selected new one
                    planes.goodsInfo[selectedIndex].selected = false;

                    selectedIndex = i;
                    planes.selectedIndex = selectedIndex;
                    planes.goodsInfo[i].selected = true;
                    selected = info;
                }
            }

            temp.SetActive(true);
        }
    }


    //
    //
    // Button event With Purchase
    //
    //
    //

    // 190509 LifeBalance
    // If user purchase item, goods layer will be dark color.
    public void PurchasedAlready(Goods goods)
    {
        goods.purchaseButton.interactable = false;
        goods.purchasedImage.color = new Color(0, 0, 0, 0.5f);
        goods.selectButton.gameObject.SetActive(true);
    }

    public void Purchased(Goods goods)
    {
        //int tempCredit = PublicValueStorage.Instance.RefreshCredit((-1) * goods.priceInt);
        int tempCredit = PublicValueStorage.Instance.RefreshCredit(0);

        if (tempCredit <= goods.priceInt)
        {
            SoundManager.Instance.UiSpeaker(SoundManager.UISound.PurchaseFail);
            return;
        }
        else
        {
            tempCredit = PublicValueStorage.Instance.RefreshCredit((-1) * goods.priceInt);
            goods.purchaseButton.interactable = false;
            goods.purchasedImage.color = new Color(0, 0, 0, 0.5f);
            credit.text = "" + tempCredit;
        
            for(int i = 0; i < planes.goodsInfo.Length; i++)
            {
                if (planes.goodsInfo[i].name == goods.itemName.text)
                {
                    planes.goodsInfo[i].purchased = true;
                    goods.selectButton.gameObject.SetActive(true);
                    PublicValueStorage.Instance.RefreshCredit(0);
                    SaveData.Instance.SaveUserData();
                }
            }

            SoundManager.Instance.UiSpeaker(SoundManager.UISound.Purchase);
        }

    }

    
    //
    //
    // Button With choose plane
    //
    //

    public void SelectPlanes(Goods goods)
    {
        for (int i = 0; i < planes.goodsInfo.Length; i++)
        {
            if (planes.goodsInfo[i].name == goods.itemName.text)
            {
                // Change selected object new one
                //Debug.Log("1 : " + selected.itemName.text);
                selected.selectButton.ResetButton();

                planes.goodsInfo[planes.selectedIndex].selected = false;
                planes.goodsInfo[i].selected = true;

                selected = goods;

                planes.selectedIndex = i;

                SaveData.Instance.SaveUserData();
                //Debug.Log("Selected");
            }
        }
    }
}
