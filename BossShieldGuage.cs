using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossShieldGuage : MonoBehaviour
{
    Slider BossHpBar;

    private void Start()
    {
    }

    private void OnEnable()
    {
        BossHpBar = this.GetComponent<Slider>();
        BossHpBar.value = BossHpBar.maxValue;
        ColorBlock green = ColorBlock.defaultColorBlock;
        green.disabledColor = Color.green;
        BossHpBar.colors = green;
    } 
}
