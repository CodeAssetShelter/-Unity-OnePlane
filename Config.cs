using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Config : MonoBehaviour
{
    public Slider bgmSlider;
    public Sprite[] bgmSprites = new Sprite[4];
    public Image bgmSprite;

    public Slider effectSlider;
    public Sprite[] effectSprites = new Sprite[4];
    public Image effectSprite;

    SoundManager soundManager;

    // Start is called before the first frame update
    void OnEnable()
    {
        soundManager = SoundManager.Instance;

        bgmSlider.maxValue = effectSlider.maxValue = 1.0f;

        bgmSlider.value = SoundManager.Instance.bgmVolume;
        effectSlider.value = SoundManager.Instance.effectVolume;

        StartCoroutine(SoundSetup());
    }

    IEnumerator SoundSetup()
    {
        while (true)
        {
            soundManager.SetVolume(bgmSlider.value, effectSlider.value);
            SetSoundSprite();
            yield return null;
        }
    }

    private void SetSoundSprite()
    {
        if (bgmSlider.value <= 0f)
        {
            bgmSprite.sprite = bgmSprites[0];
        }
        else if (bgmSlider.value <= 0.35f)
        {
            bgmSprite.sprite = bgmSprites[1];
        }
        else if (bgmSlider.value <= 0.70f)
        {
            bgmSprite.sprite = bgmSprites[2];
        }
        else
        {
            bgmSprite.sprite = bgmSprites[3];
        }

        ////////////////////////////////////
        ///

        if (effectSlider.value <= 0f)
        {
            effectSprite.sprite = effectSprites[0];
        }
        else if (effectSlider.value <= 0.35f)
        {
            effectSprite.sprite = effectSprites[1];
        }
        else if (effectSlider.value <= 0.70f)
        {
            effectSprite.sprite = effectSprites[2];
        }
        else
        {
            effectSprite.sprite = effectSprites[3];
        }
    }

    private void OnDisable()
    {
        if (soundManager == null) return;

        soundManager.SetVolume(bgmSlider.value, effectSlider.value);
        StopCoroutine(SoundSetup());
    }
}
