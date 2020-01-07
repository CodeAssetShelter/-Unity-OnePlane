using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour {
    private static UICanvas UIinstance = null;

    public Slider BossHpBar;

    public GameObject lifeBlock;
    public GameObject op_overheat;
    public bool op_overheatMessage = false;

    public static UICanvas Instance
    {
        get
        {
            if (UIinstance == null)
            {
                UIinstance = FindObjectOfType(typeof(UICanvas)) as UICanvas;

                if (UIinstance == null)
                {
                    //Debug.LogError("No active UICanvas Obj");
                    return null;
                }
            }

            return UIinstance;
        }
    }

    public void OP_PrintOverheat(bool active)
    {
        if (this == null) return;

        if (active == true)
            op_overheat.gameObject.SetActive(true);
        else
            op_overheat.gameObject.SetActive(false);
    }

    public void SetLifeBlock(int ea, Image image)
    {
        for (int i = 0; i < ea; i++)
        {
            Instantiate(image, lifeBlock.transform);
        }
    }
    public void DisableLifeBlock(int ea)
    {
        if (lifeBlock.transform.childCount < ea)
        {
            ea = lifeBlock.transform.childCount;
        }
        for (int i = 0; i < ea; i++)
        {
            Destroy(lifeBlock.transform.GetChild(0).gameObject);
        }
    }


    public Slider GetBossHpBar()
    {
        return BossHpBar;
    }
}
