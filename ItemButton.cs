using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour {

    //이거해
    public int buttonNum;
    public Image buttonImage;

    public void ActivateItem()
    {
        // 보낼 아이템 인덱스를 게임 매니저에 전달
        //Debug.Log("My number is : " + buttonNum);
        //GameManager.Instance.UseItem(buttonNum);
        PublicValueStorage.Instance.UseItem(buttonNum);
    }

    public void SetButtonImage(Sprite sprite)
    {
        buttonImage.sprite = sprite;
    }

    public void SetButtonNumber(int number)
    {
        buttonNum = number;
    }
}
