using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour {

    public GameObject slotPrefab;
    public Sprite[] itemImage;
    [Range(0, 5)]
    public int itemSlotEa = 0;
    public GameObject[] itemSlot;

	// Use this for initialization
	void Start () {
        itemSlot = new GameObject[itemSlotEa];
        for (int i = 0; i < itemSlotEa; i++)
        {
            itemSlot[i] = Instantiate(slotPrefab, this.gameObject.transform);
            //itemSlot[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = itemImage[i];
        }
    }

	// Update is called once per frame
	void Update () {
    }
}
