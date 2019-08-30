using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This Script will replace GameManager.cs' Intances
/// </summary>
public class PublicValueStorage : MonoBehaviour
{
    private static PublicValueStorage _instance;
    public static PublicValueStorage Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(PublicValueStorage)) as PublicValueStorage;

                if (_instance == null)
                {
                    Debug.LogError("No Active GameManager!");
                }
            }

            return _instance;
        }
    }


    // Event Handler
    public delegate void GMHandler(GameObject target = null);
    public static event GMHandler onAddMissileScore;
    public static event GMHandler onAddEnemyPos;
    public static event GMHandler onBossDie;

    public delegate void ButtonHandler(int index);
    public static event ButtonHandler onUseItem;


    public delegate Vector2 GetPostionHandler(Vector2 direction);
    public static event GetPostionHandler onGetRandomEnemyPos;

    public delegate int IntHandler(int value = 0);
    public static event IntHandler onRefreshCredit;
    public static event IntHandler onGetBestScore;



    // Public Instance values
    private GameObject player;
    private ControllerPlayer controllerPlayer;
    private Slider bossHpBar;
    private Vector2 playerPos;
    private Vector2 playerConstPos;
    private Vector2 screenSize;
    private float bossHp;
    private float attackSpeedRivision = 1.0f;

    public Canvas canvas;

    void Update()
    {
        if (player == null)
            return;
        playerPos = player.transform.position;
    }


    public Canvas GetCanvas()
    {
        return canvas;
    }


    public void SetValues(GameObject player)
    {
        this.player = player;
        controllerPlayer = this.player.GetComponent<ControllerPlayer>();
    }

    public void SetPlayerConstPosition(GameObject playerPositionPrefab)
    {
        playerConstPos = playerPositionPrefab.transform.position;
    }

    public Vector2 GetPlayerConstPosition()
    {
        return playerConstPos;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public void AddEnemyPos(GameObject target)
    {
        onAddEnemyPos(target);
    }

    public Vector2 GetRandomEnemyPos()
    {
        return onGetRandomEnemyPos(Vector2.zero);
    }

    public void AddMissileScore()
    {
        onAddMissileScore();
    }

    public void UseItem(int index)
    {
        onUseItem(index);
    }

    public int RefreshCredit(int value)
    {
        return onRefreshCredit(value);
    }

    public int GetBestScore(int value = 0)
    {
        return onGetBestScore(value);
    }

    public void BossDie(GameObject target)
    {
        onBossDie(target);
    }

    public void SetBossHp(float bossHp)
    {
        this.bossHp = bossHp;
    }

    public float GetBossHp()
    {
        return this.bossHp;
    }


    public void SetCallbacks(GMHandler addMissileScore, GMHandler addEnemyPos, ButtonHandler UseItem, GetPostionHandler GetRandomEnemyPos, IntHandler RefreshCreditPVC, IntHandler GetBestScore, GMHandler BossDie)
    {
        onAddMissileScore = addMissileScore;
        onAddEnemyPos = addEnemyPos;
        onUseItem = UseItem;
        onGetRandomEnemyPos = GetRandomEnemyPos;
        onRefreshCredit = RefreshCreditPVC;
        onGetBestScore = GetBestScore;
        onBossDie = BossDie;
    }

    public Vector2 GetPlayerPos()
    {
        return playerPos;
    }


    //
    //
    //
    // about Boss create method
    //
    //
    //

    public void SetBossHpBar(Slider hpBar)
    {
        bossHpBar = hpBar;
        bossHpBar.gameObject.SetActive(false);
    }

    public void ActiveBossHpBar(bool active)
    {
        bossHpBar.enabled = active;
    }

    public Slider GetBossHpBar()
    {
        return bossHpBar;
    }

    public void SetScreenSize(float height, float width)
    {
        screenSize[0] = width;
        screenSize[1] = height;
    }

    public Vector2 GetScreenSize()
    {
        return (screenSize != Vector2.zero) ? screenSize : new Vector2(0, 3.0f);
    }

    public ControllerPlayer GetPlayerComponent()
    {
        return controllerPlayer;
    }

    //
    //
    //
    // Boss : Line Falls method to Player
    //
    //
    //

    public void LineFallChangePlayerCanMove()
    {

    }

    //
    //
    //
    // Stage Additional value
    //
    //
    //

    
    public void SetAddSpeedRivisionValue(float value)
    {
        if (attackSpeedRivision >= 3.0f) return;
        attackSpeedRivision += value;
    }
    public float GetAddSpeedRivisionValue()
    {
        return attackSpeedRivision;
    }
}
