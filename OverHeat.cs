using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverHeat : MonoBehaviour
{

    private void OnDestroyAllObject()
    {
        this.gameObject.SetActive(false);
        GameManager.onDestroyAllObject -= OnDestroyAllObject;
    }
    private void OnEnable()
    {
        GameManager.onDestroyAllObject += OnDestroyAllObject;
        SoundManager.Instance.UiSpeaker(SoundManager.UISound.OverHeat);
    }
}
