using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerLaserNo5Ver2 : MonoBehaviour
{
    // SPAWN STATE
    enum SpawnAnimation { Start = 0, Move, Arrived, LaunchMissile, StateCount}
    SpawnAnimation spawnAnimation;

    // Child
    //public PlayerDetector playerDetector;

    // BGM
    public AudioClip bgmClip;

    // Components
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    private MissileModuleLaserNo5Ver2 missileModule;


    // Player gameObject
    protected ControllerPlayer player = null;


    // for Player Trigger
    private bool foundPlayer = false;
    private bool isPlayerDead = false;


    // Spawn Animation values
    private Vector3 spawnStart;
    private Vector3 spawnEnd;
    private float spawnProcess = 0;
    [SerializeField]
    private float spawnSpeed;
    [SerializeField]
    private float spawnSpeedMin = 0.4f;

    [Range(20, 100)]
    public float playerShieldDivideValue = 30;
    private float playerShieldMaxValue = 5;
    private UnityEngine.UI.Slider playerShieldGauge;
    private RectTransform playerShieldGuageRect;
    private Vector2 playerShieldGuageOriginalOffsetMax;
    private Vector2 playerShieldGuageFixedOffsetMax;


    private void OnDestroyAllObject()
    {
        Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {        
        InitOnStart();

        SoundManager.Instance.BgmSpeaker
            (SoundManager.BGM.Boss,
            SoundManager.State.Play, bgmClip);

        GameManager.onPlayerDie += IsPlayerDied;

        playerShieldGauge = GameObject.Find("Player Shield Gauge").GetComponent<UnityEngine.UI.Slider>();
        playerShieldGuageRect = playerShieldGauge.GetComponent<RectTransform>();

        playerShieldMaxValue = playerShieldGauge.maxValue;

        playerShieldGuageOriginalOffsetMax = playerShieldGuageRect.offsetMax;
        //Debug.Log("Orig offsetMax : " + playerShieldGuageRect.offsetMax);
        //float newOffsetMaxY =
        //     (playerShieldGuageRect.offsetMax.y * 2.0f) -
        //     ((playerShieldGuageRect.offsetMax.y * 0.01f) * playerShieldDivideValue);
        //Debug.Log("MAX : " + playerShieldGuageRect.offsetMin.y);
        //Debug.Log("MIN : " + (playerShieldGuageRect.offsetMin.y + playerShieldGuageRect.offsetMax.y));
        float newOffsetMaxY =
            (playerShieldGuageRect.offsetMin.y + playerShieldGuageRect.offsetMax.y)
            * 0.01f * playerShieldDivideValue;
        //Debug.Log("1% : " + ((playerShieldGuageRect.offsetMin.y - playerShieldGuageRect.offsetMax.y)
          //  * 0.01f));
        //Debug.Log("NEW * Percent : " + newOffsetMaxY);
        playerShieldGuageFixedOffsetMax = new Vector2(playerShieldGuageRect.offsetMax.x, newOffsetMaxY);
        //Debug.Log("Now offsetMax : " + playerShieldGuageFixedOffsetMax);

        SoundManager.Instance.BgmSpeaker
            (SoundManager.BGM.Boss, SoundManager.State.Play, bgmClip);
    }

    // Update is called once per frame
    void Update()
    {
        switch (spawnAnimation)
        {
            case SpawnAnimation.Start:
                ChangeState(SpawnAnimation.Move);
                break;

            case SpawnAnimation.Move:
                Spawn();
                break;

            case SpawnAnimation.Arrived:
                ChangeState(SpawnAnimation.LaunchMissile);
                missileModule.InitModule(spawnStart);
                break;

            case SpawnAnimation.LaunchMissile:
                missileModule.RunModule();
                break;

            default:
                //Debug.LogError("Error");
                break;
        }
    }

    private void Spawn()
    {
        if (spawnProcess >= 1.0f)
        {
            ChangeState(SpawnAnimation.Arrived);
            this.GetComponent<Rigidbody2D>().simulated = true;

            playerShieldGauge.maxValue *= playerShieldDivideValue * 0.01f;
            playerShieldGauge.value = playerShieldGauge.maxValue;
        }
        else
        {
            spawnProcess += spawnSpeed * Time.deltaTime;
            this.transform.position = Vector3.Lerp(spawnStart, spawnEnd, spawnProcess);
            playerShieldGuageRect.offsetMax =
                Vector2.Lerp(playerShieldGuageOriginalOffsetMax,
                             playerShieldGuageFixedOffsetMax, spawnProcess);
        }
    }

    private void InitOnStart()
    {
        spawnAnimation = SpawnAnimation.Start;

        circleCollider = this.GetComponent<CircleCollider2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        missileModule = this.GetComponent<MissileModuleLaserNo5Ver2>();

        player = PublicValueStorage.Instance.GetPlayerComponent();
        circleCollider.radius = spriteRenderer.bounds.extents.x * 0.7f;

        // Spawn Values
        spawnStart = PublicValueStorage.Instance.GetBossSpawnPosition() * 3.5f;
        spawnEnd = PublicValueStorage.Instance.GetBossSpawnPosition();

        //Debug.Log("start : " + spawnStart);
        //Debug.Log("end : " + spawnEnd);

        if (spawnSpeed <= spawnSpeedMin)
        {
            spawnSpeed = spawnSpeedMin;
        }
    }

    private void ChangeState(SpawnAnimation spawnState)
    {
        spawnAnimation = spawnState;
    }

    public MissileModuleLaserNo5Ver2 GetMissileModule()
    {
        return missileModule;
    }

    private void IsPlayerDied()
    {
        //Debug.Log("Dead");
        if (isPlayerDead == false)
        {
            if (player == null)
            {
                isPlayerDead = true;
                //Debug.Log("Dead");
            }
        }
    }
}
