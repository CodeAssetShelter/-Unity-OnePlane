using System.Collections;
using System.Collections.Generic;
using UnityEngine;


////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////
// This Module only control about missile
////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////

public class BOSSMissileModule : MonoBehaviour {

    // Common Settings
    [HideInInspector]
    public int patternCount;

    enum EnemyState { Spawned = 0, Move, Idle, Play, StateCount }
    EnemyState enemyState;

    enum BossPatternState { Shuffle, Selected, Launch, Cooldown, StateCount }
    BossPatternState bossPatternState;

    public enum BossPatternName { Normal = 0, Side, Round, RandomSpawn, StateCount, BigLaser }
    BossPatternName bossPatternName;

    // Debug Options
    public BossPatternName alwaysThisPattern;


    // Prefabs
    [Header ("- Missiles")]
    public GameObject bossMissile;
    public AudioClip soundMissileNormal;
    public AudioClip soundMissileRandom;
    public AudioClip soundMissileSide;
    public AudioClip soundMissileRound;
    private AudioClip soundSendClip;
    private SoundManager.Speaker soundSendSpeaker = SoundManager.Speaker.Center;
    // LaunchMissile
    //private int bossPatternNumber = 0;

    private float shotTimerCheck = 0.0f;
    public float shotTimer;

    // BOSS Values (Move with opCurves)
    public OPCurves opCurves;


    // Missile common values
    private float missileSpeed = 1.0f;
    public float spawnMissileSpotX = 0;
    public float spawnMissileSpotY = 0;

    [Range(10,1000)]
    public int missileStackMin = 10;
    [Range(30,1000)]
    public int missileStackMax = 30;
    private int missileSaveStack = 0;
    private int missileLaunchStack = 0;


    // Missile side attack values
    private bool missileSideChase = false;
    private bool missileSideIsLeft = false;
    public float missileSideSpeed = 1.0f;
    private int missileSideStack = 0;
    private int missileSideLaunchStack = 0;
    public float missileSideTimer = 2.0f;
    private float missileSideTimerCheck = 0f;

    // Missile round attack values
    private int missileEa = 1;
    private int missileRoopEa = 0;
    private float missileFullAngle = 360f;
    private float missileAngle = 0;
    //private int missileRoundAttackCheck = 0;
    public int missileRoundAttackStack = 3;
    private float variableAngle = 0;

    // Chase Values
    public float chaseSpeedMin = 0;
    public float chaseSpeedMax = 0.5f;
    public float chaseStaticSpeed = 1.0f;
    private float chaseSpeed = 1.0f;


    // CoolDown Values
    public float cooldownTimerCheck = 2.0f;
    public float cooldownTimer = 0.0f;


    // Missile Orbit Values
    private Vector2 bezierStart = Vector2.zero;
    private Vector2 bezierCenter = Vector2.zero;
    private Vector2 bezierEnd = Vector2.zero;
    private Vector2 direction = Vector2.zero;

    private Vector2 playerPos = Vector2.zero;
    private Vector2 targetPos = Vector2.zero;




    // Screen Size (temporary)
    private float screenHeight;
    private float screenWidth;

    public void InitBossMissileModule(float height, float width, Vector2 playerPosition)
    {
        screenHeight = height;
        screenWidth = width;
        bossPatternState = BossPatternState.Shuffle;
        playerPos = playerPosition;

        //Debug.Log("Get PlayerPos : " + playerPosition);
    }

    // 190315 LifeBalance
    // This is update() of BOSSMissilePattern, will used by ControllerBOSS.cs
    public void LaunchMissile()
    {
        BossMissile();
    }

    private void BossMissile()
    {
        if (bossPatternState == BossPatternState.Shuffle)
        {

            bossPatternState = BossPatternState.Selected;
        }
        if (bossPatternState == BossPatternState.Selected)
        {

            bossPatternName = (BossPatternName)RandomPatternSelect();
            missileSaveStack = Random.Range(missileStackMin, missileStackMax);

            if (alwaysThisPattern != BossPatternName.StateCount)
                bossPatternName = alwaysThisPattern;


            if (bossPatternName == BossPatternName.Side)
            {
                missileSideStack = missileSaveStack;
            }
            if (bossPatternName == BossPatternName.Round)
            {
                variableAngle = Random.Range(-missileFullAngle, missileFullAngle);
                SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundMissileRound);
            }

            //Debug.Log("missile Ea : " + missileEa);
            //Debug.Log("missile stack : " + missileSaveStack);

            bossPatternState = BossPatternState.Launch;
        }
        if (bossPatternState == BossPatternState.Launch)
        {
            ShotMissile();
        }
        if (bossPatternState == BossPatternState.Cooldown)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= cooldownTimerCheck)
            {
                bossPatternState = BossPatternState.Shuffle;
                cooldownTimer = 0;
            }
        }
    }

    // 190318 LifeBalance
    // Launch Missile method
    private void ShotMissile()
    {
        if (missileLaunchStack >= missileSaveStack)
        {
            bossPatternState = BossPatternState.Cooldown;
            missileSideChase = false;
            missileLaunchStack = 0;
            missileSideLaunchStack = 0;
            missileEa = 1;
            missileRoopEa = 0;
            variableAngle = 0;
            return;
        }
        shotTimerCheck += Time.deltaTime;
        if (shotTimerCheck >= shotTimer)
        {
            Vector2 spawnPosition = this.transform.position;
            bool chaseMode = false;
            chaseSpeed = chaseStaticSpeed;

            switch (bossPatternName)
            {
                case BossPatternName.Normal:
                    {
                        bezierStart = bezierCenter = bezierEnd = this.gameObject.transform.position;
                        bezierEnd.x += Random.Range(-1.0f, 1.0f);
                        bezierEnd.y += Random.Range(0.1f, -0.3f);
                        bezierCenter.x = (bezierEnd.x + bezierStart.x) * 0.5f;
                        bezierCenter.y += Random.Range(0.2f, 0.5f);

                        //ChaseTimer = Random.Range(ChaseTimerMin, ChaseTimerMax);
                        chaseMode = false;
                        targetPos = playerPos;

                        soundSendClip = soundMissileNormal;
                        soundSendSpeaker = SoundManager.Speaker.Center;
                        break;
                    }
                case BossPatternName.Side:
                    {
                        soundSendClip = soundMissileSide;

                        // missile will going to out of screed both side
                        bezierStart = bezierCenter = bezierEnd = this.gameObject.transform.position;
                        targetPos = bezierEnd;

                        chaseSpeed = missileSideSpeed;
                        // Launch Mode
                        if (missileSideChase == false)
                        {

                            // Launch Right side
                            if (missileSideIsLeft == false)
                            {
                                targetPos.x += screenWidth * 2;
                            }
                            else
                            {
                                targetPos.x -= screenWidth * 2;
                            }
                            chaseMode = true;

                            missileSideIsLeft = !missileSideIsLeft;

                            soundSendSpeaker = SoundManager.Speaker.Center;

                            direction = opCurves.SeekDirection(this.transform.position, targetPos);

                            missileSideLaunchStack++;
                            missileLaunchStack--;

                            // Switch Attack Mode
                            if (missileSideLaunchStack >= missileSaveStack)
                            {
                                missileSideChase = true;
                                missileLaunchStack = 0;
                                missileSideLaunchStack = 0;
                            }
                        }
                        // Attack Mode
                        else
                        {
                            // Attack from right side
                            if (missileSideIsLeft == false)
                            {
                                spawnPosition = playerPos;
                                spawnPosition.x = screenWidth + 0.5f;

                                soundSendSpeaker = SoundManager.Speaker.Right;
                            }
                            // Attack from left side
                            else
                            {
                                spawnPosition = playerPos;
                                spawnPosition.x = -screenWidth - 0.5f;

                                soundSendSpeaker = SoundManager.Speaker.Left;
                            }
                            chaseMode = true;
                            missileSideIsLeft = !missileSideIsLeft;
                            bezierStart = bezierCenter = bezierEnd = spawnPosition;
                            direction = opCurves.SeekDirection(spawnPosition, playerPos);

                            //targetPos = playerPos;
                        }                       
                        break;
                    }
                case BossPatternName.BigLaser:
                    {
                        //Debug.Log("BigLaser is not finished, Change Cooldown");
                        bossPatternState = BossPatternState.Cooldown;
                        return;
                        
                        //break;
                    }
                case BossPatternName.Round:
                    {
                        missileEa = 10;
                        missileAngle = missileFullAngle / missileEa;
                        missileRoopEa = Random.Range(1, (int)(missileStackMax * 0.5f));
                        bezierStart = bezierCenter = bezierEnd = spawnPosition;
                        targetPos.y = -(screenHeight * 2);
                        //direction = opCurves.SeekDirectionToPlayer(this.transform.position, playerPos);
                        direction = Vector2.down;

                        chaseMode = true;

                        soundSendClip = soundMissileRound;
                        soundSendSpeaker = SoundManager.Speaker.Center;

                        break;
                    }
                case BossPatternName.RandomSpawn:
                    {
                        soundSendClip = soundMissileRandom;

                        spawnPosition.x = (Random.Range(-screenWidth, screenWidth)) * 0.5f;
                        spawnPosition.y = (Random.Range(-(screenHeight * 0.25f), screenHeight));
                        direction = opCurves.SeekDirection(spawnPosition, PublicValueStorage.Instance.GetPlayerPos());
                        chaseMode = true;

                        break;
                    }
                default:
                    //Debug.Log("BossShotMissile Module Error!");
                    break;
            }
            // Missile display on Hierarchy Root
            //Debug.Log("in create");
            
            for (int i = 0; i < missileEa; i++)
            {
                if (bossPatternName == BossPatternName.Round)
                {
                    Quaternion v3Rotation = Quaternion.Euler(0f, 0, (missileAngle * i) + variableAngle);  // 회전각 
                    Vector3 v3Direction = Vector3.down;
                    Vector3 v3RotatedDirection = v3Rotation * v3Direction;
                    direction = v3RotatedDirection;
                }


                GameObject temp = Instantiate(bossMissile, spawnPosition, Quaternion.Euler(0, 0, 0));
                BOSSMissile temp2 = temp.GetComponent<BOSSMissile>();
                
                if (missileEa == 1)
                    SoundManager.Instance.ShortSpeaker(soundSendSpeaker, soundSendClip);

                temp2.SetRoute(direction, bezierStart, bezierCenter, bezierEnd, chaseSpeed * PublicValueStorage.Instance.GetAddSpeedRivisionValue(), chaseMode, this.gameObject.transform.position, (int)bossPatternName);
                temp2.SetPlayerPosition(playerPos);
            }
            
            shotTimerCheck = 0;
            missileLaunchStack++;
        }
    }


    public void SetScreenSize(float height, float width)
    {
        screenHeight = height;
        screenWidth = width;
    }

    public int RandomPatternSelect()
    {
        return Random.Range(0, (int)BossPatternName.StateCount);
    }

    public void SetMissileData(GameObject target, Vector2 start, Vector2 center, Vector2 end, float speed, int patternNumber, bool chaseMode)
    {
        if (chaseMode == false)
        {
            switch (patternNumber)
            {
                case (int)BossPatternName.Side:
                    end.x = (end.x > 0) ? end.x += screenWidth : end.x -= screenWidth;
                    SpawnNormal(target, start, center, end, speed);
                    break;

                case (int)BossPatternName.BigLaser:
                    SpawnSideAttack(target, start, center, end, speed);
                    break;
                default:
                    break;
            }
        }
        else
        {

        }

    }



    public void SpawnNormal(GameObject target, Vector2 start, Vector2 center, Vector2 end, float speed)
    {
        //target.transform.position = Vector2.Lerp(start, end, speed);
        opCurves.Bezier2DLerp(target, start, center, end, speed);
    }

    public void SpawnSideAttack(GameObject target, Vector2 start, Vector2 center, Vector2 end, float speed)
    {
        target.transform.position = Vector2.Lerp(start, end, speed);
    }
}
