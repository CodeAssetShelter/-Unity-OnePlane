using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerBOSS : MonoBehaviour {


    private ControllerPlayer player;



    // BGM
    public AudioClip bgmClip;

    // Screen Size (temporary)
    private float screenHeight;
    private float screenWidth;
    private float[] screenSize = new float[2];

    // BOSS Values
    public BOSS boss;
    public Animator bossExplosion;

    // System
    public delegate void BOSSCallback(GameObject target);
    public BOSSCallback onBossDie = null;

    enum EnemyState { Spawned = 0, Move, Idle, Play, Dead, StateCount }
    EnemyState enemyState;


    // BOSS Move value
    private float bezierSpeed = 0.0f;
    public float moveSpeed = 1f;
    public float moveMinSpeed = 0.2f;
    public float moveBreakSpeed = 0.05f;
    private Vector2 bezierStart = Vector2.zero;
    private Vector2 bezierCenter = Vector2.zero;
    private Vector2 bezierEnd = Vector2.zero;
    private Vector2 direction = Vector2.zero;

    // BOSS HP value
    public float hp = 100.0f;
    public Slider bossShieldSlider;
    private ColorBlock bossShieldColor = ColorBlock.defaultColorBlock;

    // LaunchMissile
    public BOSSMissileModule bossMissileModule;

    private Vector2 playerPos;

    private void OnDestroyAllObject()
    {
        Destroy(this.gameObject);
    }
    private void OnEnable()
    {
        GameManager.onDestroyAllObject += OnDestroyAllObject;
        GameManager.onPlayerDie += OnPlayerDie;
        //GameManager.onDestroyAllEnemy += OnPlayerDie;
        //GameManager.onBossDie += OnBossDie;

        player = PublicValueStorage.Instance.GetPlayerComponent();

        // for debug


        //onBossDie += GameManager.Instance.BossDie;

        enemyState = EnemyState.Spawned;

        bezierStart = this.gameObject.transform.position;

        SoundManager.Instance.BgmSpeaker
            (SoundManager.BGM.Boss,
             SoundManager.State.Play, bgmClip);
    }

    //private void OnBecameVisible()
    //{
    //    //GameManager.Instance.enemyPos.Add(this.gameObject);
    //}


    // Use this for initialization
    void Start() {

        bossShieldSlider = PublicValueStorage.Instance.GetBossHpBar();
        //bossShieldSlider = GameObject.Find("Boss Shield").gameObject.transform.GetChild(0).gameObject.GetComponent<Slider>();

        bossShieldSlider.gameObject.SetActive(true);
        bossShieldSlider.enabled = true;

        hp = PublicValueStorage.Instance.GetBossHp();


        ColorBlock green = new ColorBlock();
        green.disabledColor = Color.green;
        bossShieldSlider.colors = green;


        bossShieldSlider.maxValue = hp;
        bossShieldSlider.value = bossShieldSlider.maxValue;
        SetBossHpBarColor();

        //screenHeight = 2 * Camera.main.orthographicSize;
        //screenWidth = screenHeight * Camera.main.aspect;
        Vector2 screenSize = PublicValueStorage.Instance.GetScreenSize();
        screenWidth = screenSize.x;
        screenHeight = screenSize.y;

        bossMissileModule.SetScreenSize(screenHeight, screenWidth);
        boss.SetMethodGetDamaged(GetDamaged);

    }

    // Update is called once per frame
    void Update () {

        // 190307 LifeBalance
        // Game State per behavior;
        // Spawn and set move route and play rotate animation for BOSS

        if (enemyState == EnemyState.Spawned)
        {
            bezierEnd.x = 0;
            bezierEnd.y = screenHeight * 0.25f;

            enemyState = EnemyState.Move;
        }
        // Move State used to OpCurve
        if (enemyState == EnemyState.Move)
        {
            if (bezierSpeed >= 1.0f)
            {
                bezierSpeed = 0;
                enemyState = EnemyState.Play;
                //GameManager.Instance.enemyPos.Add(this.gameObject);
                GameManager.onDeadByItemBomb += GetDamagedByBomb;
                PublicValueStorage.Instance.AddEnemyPos(this.gameObject);

                boss.SetColliderSwitch(true);
                bossMissileModule.InitBossMissileModule(screenHeight, screenWidth, PublicValueStorage.Instance.GetPlayerPos());
            }
            else
            {
                if (moveSpeed >= moveMinSpeed)
                {
                    moveSpeed -= moveBreakSpeed;
                }

                bezierSpeed += Time.deltaTime * moveSpeed;

                this.transform.position = Vector2.Lerp(bezierStart, bezierEnd, bezierSpeed);



            }
        }
        if (enemyState == EnemyState.Play)
        {
            //////////////////////////////////////////////////////
            // 190308 LifeBalance
            // Boss Missile Patterns
            //////////////////////////////////////////////////////
            bossMissileModule.LaunchMissile();
        }
    }


    private void SetBossHpBarColor()
    {
        bossShieldColor.disabledColor = Color.Lerp(Color.red, Color.green, bossShieldSlider.value / bossShieldSlider.maxValue);
        bossShieldSlider.colors = bossShieldColor;
    }

    public void SetBossData(float hp)
    {
        this.hp = hp;
    }

    //
    //
    //
    // Event Handlers
    //
    //
    //

    public void GetDamaged()
    {
        hp -= 1 * player.damage;
        bossShieldSlider.value = hp;
        SetBossHpBarColor();
        if (hp <= 0)
        {
            // Do Something If boss died
            //Debug.Log("Boss is dead");
            OnBossDie();
        }
    }

    public void GetDamagedByBomb()
    {
        //Debug.Log("Boss get damaged by Player's Bomb");
        GetPercentDamaged(10);
    }

    public void GetPercentDamaged(float value)
    {
        bossShieldSlider.value -= bossShieldSlider.maxValue * 0.1f;
        hp = bossShieldSlider.value;
        SetBossHpBarColor();

        if (hp <= 0)
        {
            // Do Something If boss died
            //Debug.Log("Boss is dead");
            OnBossDie();
        }
    }

    public void OnBossDie()
    {
        enemyState = EnemyState.Dead;
        bossExplosion.gameObject.SetActive(true);
        bossExplosion.GetComponent<BossExplosion>().ExplosionSoundPlay();
        StartCoroutine(BossDie());
        //onBossDie(this.gameObject);
    }
    bool bossDie = false;
    IEnumerator BossDie()
    {
        while (true)
        {
            //Debug.Log("ss : " + bossExplosion.GetCurrentAnimatorStateInfo(0).normalizedTime);
            if (bossExplosion.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                bossShieldSlider.gameObject.SetActive(false);
                if (bossDie == false)
                {
                    PublicValueStorage.Instance.BossDie(this.gameObject);
                    bossDie = true;
                }
                yield break;
            }
            yield return null;
        }
    }


    public void OnPlayerDie()
    {
        //Debug.Log("Enemy Pilot Destroy to " + this.transform.position);
        //Destroy(this.gameObject);
    }

    public void SetCallback(BOSSCallback callback)
    {
        onBossDie = callback;
    }


    //
    //
    //
    // OnDoSomething Side
    //
    //
    //

    private void OnDisable()
    {
        GameManager.onDestroyAllObject -= OnDestroyAllObject;
        GameManager.onPlayerDie -= OnPlayerDie;
        GameManager.onDestroyAllEnemy -= OnPlayerDie;
        GameManager.onDeadByItemBomb -= GetDamagedByBomb;
    }

}
