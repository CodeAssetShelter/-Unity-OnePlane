using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEnemy : MonoBehaviour {

    // System
    public delegate void EnemyCallback(GameObject target = null);
    public EnemyCallback CallbackEnemyDie = null;
    public EnemyCallback CallbackDeadByItemBomb = null;
    public EnemyCallback CallbackAddScore = null;
    public EnemyCallback CallbackAddEnemyPos = null;


    enum EnemySpawnArea { Left = 0, LeftUp, Up, RightUp, Right, enemySpawnAreaCount }
    int enemySpawnArea = -1;

    enum EnemyState {Spawned = 0, Move, Idle, Play, EnemyStateCount}
    EnemyState enemyState;

    public Animator enemyAnim;

    [Header ("- Missiles")]
    public GameObject missile;
    public GameObject missileTwo;
    public GameObject childEnemy;
    [Range(10, 30)]
    public float missileProbability;

    // Animation, Bezier Values
    [Header("- OpCurves")]
    public OPCurves opCurves;

    [Header("- Move Values")]
    public float moveSpeed = 1f;
    private float bezierSpeed = 0.0f;
    private Vector2 bezierStart = Vector2.zero;
    private Vector2 bezierCenter = Vector2.zero;
    private Vector2 bezierEnd = Vector2.zero;
    private Vector2 direction = Vector2.zero;
    private bool setEvents = false;
    private float shotTimerCheck = 0.0f;
    public float shotTimer;

    //private bool tempAnim = false;


    // Get Data from GameManager
    private Vector2 battleBoxBottomLeft, battleBoxUpRight;


    public void SetSystemValue(int EnumSpawnArea, Vector2 BottomLeft, Vector2 UpRight)
    {
        enemySpawnArea = EnumSpawnArea;
        battleBoxBottomLeft = BottomLeft;
        battleBoxUpRight = UpRight;

        PlayRotateAnim(enemySpawnArea);
    }

    private void OnDestroyAllObject()
    {
        Destroy(this.gameObject);
    }
    private void OnDestroy()
    {
        GameManager.onDestroyAllObject -= OnDestroyAllObject;
    }
    private void OnEnable()
    {
        GameManager.onDestroyAllObject += OnDestroyAllObject;
        enemyState = EnemyState.Spawned;

        bezierStart = this.gameObject.transform.position;

    }

    // Update is called once per frame
    void Update () {

        // 190207 LifeBalance
        // Game State per behavior;
        // Spawn and set move route and play rotate animation
        if (enemyState == EnemyState.Spawned)
        {
            bezierEnd.x = Random.Range(battleBoxBottomLeft.x, battleBoxUpRight.x);
            bezierEnd.y = Random.Range(battleBoxBottomLeft.y, battleBoxUpRight.y);

            //bezierCenter.x = (bezierEnd.x + bezierStart.x) * 0.5f;
            //bezierCenter.y += Random.Range(battleBoxBottomLeft.y, battleBoxUpRight.y);

            bezierCenter = (bezierStart + bezierEnd) * 0.5f;

            switch (enemySpawnArea)
            {
                case (int)EnemySpawnArea.LeftUp:
                case (int)EnemySpawnArea.RightUp:
                    bezierCenter.x = (bezierEnd.x + bezierStart.x) * 0.5f;
                    bezierCenter.y += Random.Range(battleBoxBottomLeft.y, battleBoxUpRight.y);
                    //bezierCenter = (bezierStart + bezierEnd) * 0.5f;
                    break;
            }
            //PlayRotateAnim(enemySpawnArea);
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
            }
            else
            {
                bezierSpeed += Time.deltaTime * moveSpeed;

                if (bezierSpeed >= 0.5f && setEvents == false)
                {
                    this.transform.GetChild(0).GetComponent<Enemy>().SetEventAuto();
                    setEvents = true;
                }

                switch (enemySpawnArea)
                {
                    case (int)EnemySpawnArea.Left:
                    case (int)EnemySpawnArea.Right:
                    case (int)EnemySpawnArea.Up:
                        opCurves.Bezier2DLerp(this.gameObject,
                            bezierStart,
                            bezierCenter,
                            bezierEnd,
                            bezierSpeed);
                        break;
                    case (int)EnemySpawnArea.LeftUp:
                    case (int)EnemySpawnArea.RightUp:
                        opCurves.Bezier2DLerp(this.gameObject,
                            bezierStart,
                            bezierCenter,
                            bezierEnd,
                            bezierSpeed);
                        break;
                }
            }
        }
        if (enemyState == EnemyState.Play)
        {
            enemyAnim.SetBool("OnPosition", true);
            ShotMissile();
        }
        //ShotMissile();
    }

    public void ShotMissile()
    {
        shotTimerCheck += Time.deltaTime;
        if (shotTimerCheck >= shotTimer)
        {
            // Missile display on Hierarchy Root

            if (Random.Range(0, 100) >= missileProbability)
            {
                GameObject temp = Instantiate(missile, childEnemy.transform.position, Quaternion.Euler(0, 0, 0));
                //temp.GetComponent<MissileForEnemy>();
            }
            else
            {
                GameObject temp = Instantiate(missileTwo, childEnemy.transform.position, Quaternion.Euler(0, 0, 0));
            }
            shotTimerCheck = 0;
        }
    }

    // 190207 LifeBalance
    // Play only rotate Animation
    // Movement is played by OpCurve
    private void PlayRotateAnim(int enemySpawnArea)
    {
        switch (enemySpawnArea)
        {
            case (int)EnemySpawnArea.Left:
                enemyAnim.SetTrigger("SpawnL");
                break;
            case (int)EnemySpawnArea.LeftUp:
                enemyAnim.SetTrigger("SpawnLU");
                break;
            case (int)EnemySpawnArea.Up:
                enemyAnim.SetTrigger("SpawnU");
                break;
            case (int)EnemySpawnArea.RightUp:
                enemyAnim.SetTrigger("SpawnRU");
                break;
            case (int)EnemySpawnArea.Right:
                enemyAnim.SetTrigger("SpawnR");
                break;
            default:
                //Debug.LogError(this.name + " Error");
                break;
        }
    }

    /// 
    /// CALLBACKS
    /// 
    /// 


    // 190305 LifeBalance
    // Add Callback activate by item;
    public void SetCallbackEnemyDie(EnemyCallback callback)
    {
        CallbackEnemyDie = callback;
    }

    public void SetCallbackDeadByBomb(EnemyCallback callback)
    {
        CallbackDeadByItemBomb = callback;
    }

    public void SetCallbacks(EnemyCallback enemyDie, EnemyCallback deadByBomb, EnemyCallback addScore, EnemyCallback addEnemyPos)
    {
        CallbackEnemyDie = enemyDie;
        CallbackDeadByItemBomb = deadByBomb;
        CallbackAddScore = addScore;
        CallbackAddEnemyPos = addEnemyPos;
    }
}
