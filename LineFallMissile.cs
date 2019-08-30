using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFallMissile : MonoBehaviour
{
    [HideInInspector]
    public GameObject player;

    // Etc
    //private Vector2 playerPos;
    private Vector2 parentPos;
    public OPCurves opCurves;

    // Missile sprite

    // Chase
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

    private SpriteRenderer missileColor;

    private void OnEnable()
    {
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

    public void SetRoute(Vector2 forPlayerDirection, Vector2 start, Vector2 center, Vector2 end, float chaseSpeed, bool chaseMode, Vector2 parentPosition)
    {
        //direction = opCurves.SeekDirectionToPlayer(this.gameObject.transform.position, targetPosition);
        direction = forPlayerDirection;
        bezierStart = start;
        bezierCenter = center;
        bezierEnd = end;
        missileMaxSpeed = chaseSpeed;
        startChase = chaseMode;
        parentPos = parentPosition;
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
            }

            //////////////////////////////////////////////////////
            // 190524 LifeBalance
            // Get spawn pattern from ModuleLineFall
            //////////////////////////////////////////////////////

            bezierSpeed += Time.deltaTime;

            opCurves.Bezier2DLerp(this.gameObject, bezierStart, bezierCenter, bezierEnd, bezierSpeed);
        }
        else
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
                    //enemyTrail.enabled = true;
                    //missileColor.color = Color.blue;
                    //Debug.Log(missileColor);
                    //GameManager.Instance.ScoreAdd("Missile");
                    PublicValueStorage.Instance.AddMissileScore();
                    direction = opCurves.SeekDirection(this.gameObject.transform.position, parentPos);
                    missileCurrentSpeed *= missileReflectSpeed;
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
