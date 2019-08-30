using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileForEnemy : MonoBehaviour {

    public GameObject player;
    //private Vector2 playerPos;
    private Vector2 parentPos;
    public OPCurves opCurves;
    public TrailRenderer enemyTrail;

    public float ChaseTimerMin = 0;
    public float ChaseTimerMax = 0.5f;
    

    public bool startChase = false;
    private float ChaseTimer = 1.0f;
    private float ChaseTimerCheck = 0.0f;
    public float missileMaxSpeed = 0.0f;
    public float missileCurrentSpeed = 0.0f;
    public float missileReflectSpeed = 2.0f;

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

        parentPos = this.transform.position;

        //opCurves = this.gameObject.GetComponent<OPCurves>();

        bezierStart = bezierCenter = bezierEnd = this.gameObject.transform.position;
        bezierEnd.x += Random.Range(-1.0f, 1.0f);
        bezierEnd.y += Random.Range(0.1f, -0.3f);
        bezierCenter.x = (bezierEnd.x + bezierStart.x) * 0.5f;
        bezierCenter.y += Random.Range(0.2f, 0.5f);

        ChaseTimer = Random.Range(ChaseTimerMin, ChaseTimerMax);

        this.gameObject.SetActive(true);

        //heading = player.transform.position - this.transform.position;
        //distance = heading.magnitude;
        //direction = heading / distance;

        missileColor = this.gameObject.GetComponent<SpriteRenderer>();
        missileColor.color = new Color(1, 0.5f, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (startChase == false)
        {
            if (bezierSpeed >= 1.0f)
            {
                startChase = true;

                //if (this.tag == "Missile")
                //{
                    //Debug.Log(player.transform.position);
                    //direction = SeekDirectionToPlayer(this.gameObject.transform.position, GameManager.Instance.GetPlayerPosition());
                //}
                //else if (this.tag == "PlayerMissile")
                //  direction = SeekDirectionToPlayer(player.transform.position, this.gameObject.transform.position);
            }
            bezierSpeed += Time.deltaTime;
            opCurves.Bezier2DLerp(this.gameObject, bezierStart, bezierCenter, bezierEnd, bezierSpeed);
            if (bezierSpeed >= 1.0f)
            {
                //direction = opCurves.SeekDirectionToPlayer(this.gameObject.transform.position, GameManager.Instance.GetPlayerPosition());
                direction = opCurves.SeekDirection(this.gameObject.transform.position, PublicValueStorage.Instance.GetPlayerPos());
            }
        }
        else
        {
            if (ChaseTimerCheck <= ChaseTimer)
            {
                ChaseTimerCheck += Time.deltaTime;
            }
            // Chase Start !
            else
            {

                if (missileCurrentSpeed <= missileMaxSpeed)
                {
                    missileCurrentSpeed += Time.deltaTime * missileMaxSpeed;

                }
                this.transform.Translate(direction * Time.deltaTime * missileCurrentSpeed);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "WasteBasket")
        {
            Destroy(this.gameObject);
        }
        if (this.tag == "Missile")
        {
            switch (collision.tag)
            {
                case "Player":
                //case "WasteBasket":
                    Destroy(this.gameObject);
                    //Debug.Log("This!");
                    break;

                case "PlayerShield":
                    this.tag = "PlayerMissile";
                    this.name = "PlayerMissile";
                    enemyTrail.enabled = true;
                    missileColor.color = Color.blue;
                    //Debug.Log(missileColor);
                    //GameManager.Instance.ScoreAdd("Missile");
                    PublicValueStorage.Instance.AddMissileScore();
                    //direction = opCurves.SeekDirectionToPlayer(this.gameObject.transform.position, GameManager.Instance.GetRandomEnemyPos(parentPos));
                    direction = opCurves.SeekDirection(this.gameObject.transform.position, PublicValueStorage.Instance.GetRandomEnemyPos());
                    missileCurrentSpeed *= missileReflectSpeed;
                    break;
            }
        }
        else if (this.tag == "PlayerMissile")
        {
            switch (collision.tag)
            {
                case "Enemy":                    
                    Destroy(this.gameObject);
                    break;
            }
        }
    }

    //public void OnBecameInvisible()
    //{
    //    Destroy(this.gameObject);
    //    Debug.Log("called");
    //}

    //private Vector2 SeekDirectionToPlayer(Vector2 start, Vector2 target)
    //{
    //    Vector2 heading = target - start;
    //    float distance = heading.magnitude;
    //    return heading / distance;
    //}

    //public void SetPlayerPos(Vector2 position)
    //{
    //    playerPos = position;
    //}

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
