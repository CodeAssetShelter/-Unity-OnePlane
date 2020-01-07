using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Planes : MonoBehaviour
{
    [System.Serializable]
    public class GoodsInfo
    {
        //public int spriteNumber;
        //public Sprite sprite;
        public string name;
        public bool purchased = false;
        public bool selected = false;
        [TextArea]
        public string detail;
        public int price;

        [Header("- Plane Options")]
        [Range(1, 3)]
        public int life = 1;
        [Range(3, 5)]
        public int itemSlot = 3;
        [Range(5, 15)]
        public float shield = 5;
    }

    public Sprite purchaseSprite;
    public Sprite []planeSprite;

    [SerializeField]
    public GoodsInfo[] goodsInfo;
    [SerializeField]
    [HideInInspector]
    public int selectedIndex;


    private void OnEnable()
    {
        for (int i = 0; i < goodsInfo.Length; i++)
        {
            int selectedIndex = -1;
            if (goodsInfo[i].selected == true && goodsInfo[i].purchased == true)
            {
                if (selectedIndex == -1)
                {
                    selectedIndex = i;
                    this.selectedIndex = selectedIndex;
                }
                else
                {
                    goodsInfo[selectedIndex].selected = false;

                    selectedIndex = i;
                    this.selectedIndex = selectedIndex;
                    goodsInfo[i].selected = true;
                }
            }
        }
    }

    private void SetSelectedIndex()
    {

    }

    public int GetGoodsEa()
    {
        return goodsInfo.Length;
    }
}
