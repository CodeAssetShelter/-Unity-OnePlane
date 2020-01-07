using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFallMissileSquare : MonoBehaviour
{
    [HideInInspector]
    public GameObject player;

    // Etc
    //private Vector2 playerPos;
    private Vector2 parentPos;
    public OPCurves opCurves;

    // Missile Location
    [Header("- Missile Location")]
    public float betweenMissileTerm = 1.35f;
    public const int missileEa = 3;


    // Missile sprite
    [Header("- Missile Prefab")]
    public GameObject normalMissile;
    public GameObject shieldBreaker;

    // Chase
    [Header("- Missile Chase Value")]
    public bool startChase = false;
    private float chaseTimer = 1.0f;
    private float chaseTimerCheck = 0.0f;
    public float missileMaxSpeed = 0.0f;
    public float missileCurrentSpeed = 0.0f;
    public float missileReflectSpeed = 2.0f;


    private Vector2 playerPos = Vector2.zero;


    // OpCurve
    //private float distance;
    //private Vector2 heading; 
    private Vector2 direction;
    private Vector2 bezierStart, bezierCenter, bezierEnd;

    public float bezierSpeed = 0.0f;

    private void OnDestroyAllObject()
    {
        Destroy(this.gameObject);
    }
    private void OnEnable()
    {
        GameManager.onDestroyAllObject += OnDestroyAllObject;
        GameManager.onPlayerDie += OnPlayerDie;
        GameManager.onDestroyAllEnemy += OnPlayerDie;
    }

    // Use this for initialization
    void Start()
    {
        //parentPos = this.transform.position;
        this.gameObject.SetActive(true);

        //heading = player.transform.position - this.transform.position;
        //distance = heading.magnitude;
        //direction = heading / distance;


    }

    public void SetRoute(Vector2 forPlayerDirection, int shieldBreakerPosition, float chaseSpeed, Vector2 parentPosition)
    {
        direction = forPlayerDirection;
        missileMaxSpeed = chaseSpeed;
        parentPos = parentPosition;

        CreateLineFallMissile(shieldBreakerPosition);
    }

    private void CreateLineFallMissile(int shieldBreakerPosition)
    {
        Vector2 missileSpawn = this.transform.position;
        missileSpawn.x -= betweenMissileTerm;
        for (int i = 0; i < missileEa; i++)
        {
            Instantiate((i == shieldBreakerPosition) ? this.shieldBreaker : normalMissile, 
                missileSpawn, Quaternion.Euler(0, 0, 0), this.transform);
            missileSpawn.x += betweenMissileTerm;
        }
    }

    public void SetPlayerPosition(Vector2 playerPosition)
    {
        playerPos = playerPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //////////////////////////////////////////////////////
        // 190524 LifeBalance
        // Get attack pattern from ModuleLineFall
        //////////////////////////////////////////////////////

        if (chaseTimerCheck <= chaseTimer)
        {
            chaseTimerCheck += Time.deltaTime;
        }
        // Chase Start !
        else
        {
            this.transform.Translate(direction * Time.deltaTime * missileMaxSpeed);
        }
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Name : BOSSMissile - " +  collision.tag);
        if (collision.tag == "WasteBasket")
        {
            Destroy(this.gameObject);
        }
        switch (collision.tag)
        {
            case "Player":
                //case "WasteBasket":
                //Debug.Log("If Player : " +collision.tag);
                PublicValueStorage.Instance.AddMissileScore();
                break;
        }
    }

    public void OnPlayerDie()
    {
        Destroy(this.gameObject);
    }

    private void OnDisable()
    {
        GameManager.onPlayerDie -= OnPlayerDie;
        GameManager.onDestroyAllEnemy -= OnPlayerDie;
        GameManager.onDestroyAllObject -= OnDestroyAllObject;
    }
}
