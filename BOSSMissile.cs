using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSSMissile : MonoBehaviour
{
    [HideInInspector]
    public GameObject player;

    // Etc
    //private Vector2 playerPos;
    private Vector2 parentPos;
    public OPCurves opCurves;
    public TrailRenderer enemyTrail;

    public float angle = 0.5f;
    private float angleValue = 0f;

    // Missile sprite
    public Sprite[] missileSprites;
    public GameObject missileSprite;

    // Chase
    public bool startChase = false;
    private float chaseTimer = 1.0f;
    private float chaseTimerCheck = 0.0f;
    public float missileMaxSpeed = 0.0f;
    public float missileCurrentSpeed = 0.0f;
    public float missileReflectSpeed = 2.0f;

    // Round Guided shot
    private float guidedTimerMax = 10.0f;
    private float guidedTimer = 1.0f;
    private float guidedTimerCheck = 0.0f;
    private bool guidedIsActive = false;
    private bool guidedIsChaseMode = false;
    private Vector2 playerPos = Vector2.zero;



    public enum BossPatternName { Normal = 0, Side, BigLaser, Round, RandomSpawn, StateCount }
    BossPatternName bossPatternName;

    // OpCurve
    //private float distance;
    //private Vector2 heading; 
    private Vector2 direction;
    private Vector2 bezierStart, bezierCenter, bezierEnd;

    public float bezierSpeed = 0.0f;

    private SpriteRenderer missileColor;

    private void OnEnable()
    {
        GameManager.onPlayerDie += OnPlayerDie;
        GameManager.onDestroyAllEnemy += OnPlayerDie;
    }

    // Use this for initialization
    void Start()
    {

        enemyTrail.enabled = false;

        //parentPos = this.transform.position;
        this.gameObject.SetActive(true);

        //heading = player.transform.position - this.transform.position;
        //distance = heading.magnitude;
        //direction = heading / distance;
    }

    public void SetRoute(Vector2 forPlayerDirection, Vector2 start, Vector2 center, Vector2 end, float chaseSpeed, bool chaseMode, Vector2 parentPosition, int pattern)
    {
        //direction = opCurves.SeekDirectionToPlayer(this.gameObject.transform.position, targetPosition);
        direction = forPlayerDirection;
        bezierStart = start;
        bezierCenter = center;
        bezierEnd = end;
        missileMaxSpeed = chaseSpeed;
        startChase = chaseMode;
        bossPatternName = (BossPatternName)pattern;
        parentPos = parentPosition;

        
        if (bossPatternName == BossPatternName.Side)
        {
            bezierSpeed += 0.9f;
        }

        if (bossPatternName == BossPatternName.Round)
        {
            guidedIsActive = Random.Range(0, 100) > 95 ? true : false;
            if (guidedIsActive == true)
                guidedTimer = Random.Range(2.5f, guidedTimerMax);
        }
    }

    public void SetPlayerPosition(Vector2 playerPosition)
    {
        playerPos = playerPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (startChase == false)
        {
            if (bezierSpeed >= 1.0f)
            {
                startChase = true;
                if(bossPatternName == BossPatternName.Normal || bossPatternName == BossPatternName.Side)
                    direction = opCurves.SeekDirection(this.gameObject.transform.position, playerPos);
            }

            //////////////////////////////////////////////////////
            // 190308 LifeBalance
            // Get spawn pattern from BOSSMissilePattern
            //////////////////////////////////////////////////////

            bezierSpeed += Time.deltaTime;

            opCurves.Bezier2DLerp(this.gameObject, bezierStart, bezierCenter, bezierEnd, bezierSpeed);
        }
        else
        {
            //////////////////////////////////////////////////////
            // 190308 LifeBalance
            // Get attack pattern from BOSSMissilePattern
            //////////////////////////////////////////////////////

            if (chaseTimerCheck <= chaseTimer)
            {
                chaseTimerCheck += Time.deltaTime;
            }
            // Chase Start !
            else
            {

                // common moved area
                if (missileCurrentSpeed <= missileMaxSpeed)
                {
                    missileCurrentSpeed += Time.deltaTime * missileMaxSpeed;
                }
                float rad = Mathf.Atan2(direction.x, direction.y);
                float degree = rad * Mathf.Rad2Deg;

                missileSprite.transform.eulerAngles = new Vector3(0, 0, -degree);

                this.transform.Translate(direction * Time.deltaTime * missileCurrentSpeed);


                // guided area by RoundShot
                if (bossPatternName == BossPatternName.Round)
                {
                    if (guidedIsActive == true)
                    {
                        if (guidedTimerCheck >= guidedTimer)
                        {
                            guidedIsChaseMode = true;
                            guidedIsActive = false;
                            this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                            direction = opCurves.SeekDirection(this.gameObject.transform.position, playerPos);
                        }
                        this.transform.Rotate(0, 0, 10f * Time.deltaTime);
                        guidedTimerCheck += Time.deltaTime;
                    }
                    if (guidedIsActive == false)
                    {
                        if (guidedIsChaseMode == false)
                        {
                            this.transform.Rotate(0, 0, 10f * Time.deltaTime);
                        }
                    }

                    //angleValue += angle;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Name : BOSSMissile - " +  collision.tag);
        if (this.tag == "Missile")
        {
            this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            if (collision.tag == "WasteBasket")
            {
                Destroy(this.gameObject);
            }
            switch (collision.tag)
            {
                case "Player":
                    //case "WasteBasket":
                    //Debug.Log("If Player : " +collision.tag);
                    Destroy(this.gameObject);
                    break;

                case "PlayerShield":
                    this.tag = "PlayerMissile";
                    this.name = "PlayerMissile";
                    enemyTrail.enabled = true;
                    //missileColor.color = Color.blue;
                    //Debug.Log(missileColor);
                    //GameManager.Instance.ScoreAdd("Missile");
                    PublicValueStorage.Instance.AddMissileScore();
                    bossPatternName = BossPatternName.StateCount;
                    direction = opCurves.SeekDirection(this.gameObject.transform.position, parentPos);
                    missileCurrentSpeed = missileReflectSpeed;
                    break;
            }
        }


        else if (this.tag == "PlayerMissile")
        {
            if (collision.tag == "WasteBasket")
            {
                Destroy(this.gameObject);
            }
            switch (collision.tag)
            {
                case "Enemy":
                case "BOSS":
                    Destroy(this.gameObject);
                    break;
            }
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
    }
}
