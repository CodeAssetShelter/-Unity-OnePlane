using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Button button;
    public Sprite enterImage;
    public Sprite exitImage;

    private bool pointerDown = false;

    public void OnPointerUp(PointerEventData data)
    {
        pointerDown = false;
    }

    public void OnPointerDown(PointerEventData data)
    {
        pointerDown = true;
        button.image.sprite = enterImage;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (pointerDown == true)
        {
            button.image.sprite = enterImage;
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (pointerDown == true)
        {
            button.image.sprite = exitImage;
        }
    }

    public void ResetButton()
    {
        button.image.sprite = exitImage;
        button.enabled = true;
    }
}
