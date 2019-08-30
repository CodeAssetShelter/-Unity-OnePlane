using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour {
    private static UICanvas UIinstance = null;

    public Text op_overheat;
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
                    Debug.LogError("No active UICanvas Obj");
                }
            }

            return UIinstance;
        }
    }

    public void OP_PrintOverheat(bool active)
    {
        if (active == true)
            op_overheat.gameObject.SetActive(true);
        else
            op_overheat.gameObject.SetActive(false);
    }
}
