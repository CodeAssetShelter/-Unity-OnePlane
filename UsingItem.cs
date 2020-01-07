using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsingItem : MonoBehaviour
{
    private float itemDuration = 0;
    private float itemTimer = 0;
    private Image image;

    Color colorFade;

    private void OnDestroyAllObject()
    {
        Destroy(this.gameObject);
    }
    public void SetItem(float duration, Sprite sprite)
    {
        GameManager.onDestroyAllObject += OnDestroyAllObject;
        image = this.GetComponent<Image>();
        itemDuration = duration;
        image.sprite = sprite;
        colorFade = image.color;
        StartCoroutine(StartTimer());
    }
    private void OnDestroy()
    {
        GameManager.onDestroyAllObject -= OnDestroyAllObject;
    }

    public void ResetTimer()
    {
        itemTimer = 0;
    }

    IEnumerator StartTimer()
    {
        while (true)
        {
            itemTimer += Time.deltaTime;
            if (itemTimer > (itemDuration * 0.7f))
            {
                if (colorFade.a == 0) colorFade.a = 1;
                else colorFade.a = 0;
                image.color = colorFade;
            }
            else
            {
                colorFade.a = 1;
                image.color = colorFade;
            }
            if (itemTimer > itemDuration)
            {
                //Destroy(this.gameObject);
                yield break;
            }                
            yield return null;
        }
    }
}
