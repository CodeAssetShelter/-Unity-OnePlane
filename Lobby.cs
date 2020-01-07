using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    public delegate void MenuHandler();
    public static event MenuHandler onActiveGameStart;
    public static event MenuHandler onActiveShop;
    public static event MenuHandler onActiveConfig;
    public static event MenuHandler onDeactiveShop;

    public UnityEngine.UI.Text lobbyCredit;

    private void OnEnable()
    {
        DeactivateShop();
        lobbyCredit.text = "" + PublicValueStorage.Instance.RefreshCredit(0);
    }

    public void GameStart()
    {
        onActiveGameStart();
    }

    public void DeactivateShop()
    {
        onDeactiveShop?.Invoke();
    }

    public void SetCallbacks(MenuHandler ActiveGameStart, MenuHandler ActiveShop, MenuHandler DeactiveShop, MenuHandler ActiveConfig)
    {
        onActiveGameStart = ActiveGameStart;
        onActiveShop = ActiveShop;
        onActiveConfig = ActiveConfig;

        onDeactiveShop = DeactiveShop;
    }

    public void SetCallBacksTest(MenuHandler ActiveGameStart)
    {
        onActiveGameStart = ActiveGameStart;
    }

    public void buttonDownSound()
    {
        SoundManager.Instance.UiSpeaker(SoundManager.UISound.Button);
    }

    public void EquipSound()
    {
        SoundManager.Instance.UiSpeaker(SoundManager.UISound.ChangePlane);
    }

    public void PurchaseSound()
    {
        SoundManager.Instance.UiSpeaker(SoundManager.UISound.Purchase);
    }

    public void PurchaseFailSound()
    {
        SoundManager.Instance.UiSpeaker(SoundManager.UISound.PurchaseFail);
    }
}
