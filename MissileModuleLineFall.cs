using System.Collections;
using System.Collections.Generic;
using UnityEngine;


////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////
// This Module only control about missile
////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////

public class MissileModuleLineFall : MonoBehaviour
{

    // Common Settings
    [HideInInspector]
    public int patternCount;

    enum EnemyState { Spawned = 0, Move, Idle, Play, StateCount }
    EnemyState enemyState;

    enum BossPatternState { Shuffle, Selected, Launch, Cooldown, StateCount }
    BossPatternState bossPatternState;

    // 버티컬 : 가로로 넓은 탄막 아래로 이동
    // 호라이즌 : 세로로 넓은 탄막 사이드로 이동
    // 라인폴 : 여러개의 긴 직선탄 가운데 실드브레이커와 일반탄이 섞여서 나감
    // -> 실드브레이커 : 실드를 켠 상태에서 본체에 닿은 경우 실드량을 깎음
    // 슬로우 레일건 : 매우 느린 속도로 직선탄을 쏨 (슬로우레일건 탄이 사라질 때 까지 라인폴 발동안함)
    public enum BossPatternName { Vertical, Horizon, LineFall, SlowRailGun, StateCount }
    BossPatternName bossPatternName;

    // Debug Options
    public BossPatternName alwaysThisPattern;
    public GameObject nullObject;

    // Prefabs
    public GameObject lineFallMissile;
    public GameObject lineFallMissileSquare;
    public GameObject lineFallSlowRailGun;

    // LaunchMissile
    //private int bossPatternNumber = 0;

    private float shotTimerCheck = 0.0f;
    public float shotTimer;

    // BOSS Values (Move with opCurves)
    public OPCurves opCurves;


    [Range(10, 1000)]
    public int missileStackMin = 10;
    [Range(30, 1000)]
    public int missileStackMax = 30;
    private int missileSaveStack = 0;
    private int missileLaunchStack = 0;




    // // // // // // // // // // //
    //
    //
    //
    // Missile Pattern Values
    //
    //
    //
    // // // // // // // // // // //

    // Common Values
    private bool nowLaunching = false;
    private ControllerPlayer player;

    // Common Spawn Spot Values
    public List<Vector2> spawnSpotTop;
    public List<Vector2> spawnSpotLeft;
    public List<Vector2> spawnSpotRight;



    // Vertical & Horizon Pattern Values
    [Header("- Vertical & Horizon values")]
    [Header("[Pattern Options]")]
    public float timer = 2.0f;
    public float timerMin = 1.0f;
    public float timerMax = 5.0f;
    private float timerChecker = 0;
    public float missileTerm = 0.5f;
    public float missileTermMin = 0.1f;
    public float missileTermMax = 1.0f;
    public int options = 35;

    private float term;
    private float gap;
    enum VerticalPattern {LeftToRight = 0, RightToLeft, StateCount }
    VerticalPattern verticalPattern;


    // LineFall Values
    [Header("- LineFall values")]
    public float speedMinLF = 0.8f;
    public float speedMaxLF = 2.0f;
    private float speedLF = 0.8f;
    private bool isLineFallAlive = false;
    public int missileEa = 5;
    public int missileEaMin = 3;
    public int missileEaMax = 10;
    enum LineFallMissilePosition { Left = 0, Middle, Right, StateCount };
    LineFallMissilePosition lineFallMissilePosition = LineFallMissilePosition.StateCount;


    // SlowRailGun Values
    [Header("- Slow RailGun values")]
    public float speedSRG = 0.05f;
    public float speedMinSRG = 0.05f;
    public float speedMaxSRG = 0.1f;
    private bool isSRGAlive = false;


    // Chase Values
    public float chaseSpeedMin = 0;
    public float chaseSpeedMax = 0.5f;
    public float chaseStaticSpeed = 1.0f;
    private float chaseSpeed = 1.0f;


    // CoolDown Values
    [Header("- Cooldown timers")]
    public float cooldownTimerCheck =0.0f;
    public float cooldownTimer = 10.0f;


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


    //private void Start()
    //{
    //    screenHeight = PublicValueStorage.Instance.GetScreenSize().y;
    //    screenWidth = PublicValueStorage.Instance.GetScreenSize().x;
    //    CreateOptionCoord();
    //}

    public void InitBossMissileModule(float height, float width, GameObject player)
    {
        screenHeight = height;
        screenWidth = width;
        Debug.Log("Get PlayerPos : " + height + " " + width);


        bossPatternState = BossPatternState.Shuffle;
        playerPos = player.transform.position;
        this.player = player.GetComponent<ControllerPlayer>();

        if (spawnSpotLeft.Count == 0) CreateOptionCoord();

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
            Debug.Log("Stack is : " + missileSaveStack);


            if (alwaysThisPattern != BossPatternName.StateCount)
                bossPatternName = alwaysThisPattern;


            if (bossPatternName == BossPatternName.Vertical)
            {
                verticalPattern = (VerticalPattern)Random.Range(0, (int)VerticalPattern.StateCount);
            }


            if (bossPatternName == BossPatternName.LineFall)
            {
                speedLF = Random.Range(speedMinLF, speedMaxLF);
                player.InitLineFall(true);
                isLineFallAlive = true;
            }
            else
            {
                player.InitLineFall(false);
            }


            if (bossPatternName == BossPatternName.SlowRailGun)
            {
                speedSRG = Random.Range(speedMinSRG, speedMaxSRG);
                missileSaveStack = 1;
                isSRGAlive = true;
            }


            Debug.Log("Selected : " + bossPatternName);

            bossPatternState = BossPatternState.Launch;
        }


        if (bossPatternState == BossPatternState.Launch)
        {

            ShotMissile();
        }


        if (bossPatternState == BossPatternState.Cooldown)
        {
            cooldownTimerCheck += Time.deltaTime;
            if ((cooldownTimer <= cooldownTimerCheck) && (isLineFallAlive == false || isSRGAlive == false))
            {
                bossPatternState = BossPatternState.Shuffle;
                cooldownTimerCheck = 0;
            }
        }
    }

    // 190318 LifeBalance
    // Launch Missile method
    private void ShotMissile()
    {
        if (missileLaunchStack >= missileSaveStack)
        {
            Debug.Log("Shuffle");
            bossPatternState = BossPatternState.Cooldown;
            missileLaunchStack = 0;
            isLineFallAlive = false;
            isSRGAlive = false;
            return;
        }
        shotTimerCheck += Time.deltaTime;
        if (shotTimerCheck >= shotTimer)
        {
            Vector2 spawnPosition = this.transform.position;
            bool chaseMode = false;
            chaseSpeed = chaseStaticSpeed;

            //Debug.Log("missile : " + missileLaunchStack);
            switch (bossPatternName)
            {
                case BossPatternName.Vertical:
                    {
                        //bezierStart = bezierCenter = bezierEnd = this.gameObject.transform.position;
                        //ChaseTimer = Random.Range(ChaseTimerMin, ChaseTimerMax);

                        chaseMode = true;
                        targetPos = playerPos;

                        int missileStorage = spawnSpotTop.Count;


                        for (int i = 2; i < missileStorage; i += 2)
                        {
                            //Debug.Log("Pattern num : " + verticalPattern);
                            Vector2 target = spawnSpotTop[i];
                            target.y -= screenHeight + (term * 3);
                            //target.x += gap * 2;
                            direction = opCurves.SeekDirection(spawnSpotTop[i], target);

                            spawnPosition = spawnSpotTop[i];

                            GameObject temp = Instantiate(lineFallMissile, spawnPosition, Quaternion.Euler(0, 0, 0));
                            LineFallMissile temp2 = temp.GetComponent<LineFallMissile>();
                            temp2.SetRoute(direction, bezierStart, bezierCenter, bezierEnd, chaseSpeed, chaseMode, this.gameObject.transform.position);
                            temp2.SetPlayerPosition(playerPos);
                        }

                        break;
                    }
                case BossPatternName.Horizon:
                    {
                        // missile will going to out of screed both side
                        bezierStart = bezierCenter = bezierEnd = this.gameObject.transform.position;
                        targetPos = bezierEnd;

                        chaseMode = true;

                        int missileStorage = spawnSpotLeft.Count;


                        for (int i = 5; i < missileStorage; i += 2)
                        {
                            if (verticalPattern == VerticalPattern.LeftToRight)
                            {
                                Vector2 target = spawnSpotRight[i];
                                //target.y -= term * 3;
                                direction = opCurves.SeekDirection(spawnSpotLeft[i], target);

                                spawnPosition = spawnSpotLeft[i];

                            }
                            if (verticalPattern == VerticalPattern.RightToLeft)
                            {
                                Vector2 target = spawnSpotLeft[i];
                                //target.y -= term * 3;
                                direction = opCurves.SeekDirection(spawnSpotRight[i], target);

                                spawnPosition = spawnSpotRight[i];

                            }

                            GameObject temp = Instantiate(lineFallMissile, spawnPosition, Quaternion.Euler(0, 0, 0));
                            LineFallMissile temp2 = temp.GetComponent<LineFallMissile>();
                            temp2.SetRoute(direction, bezierStart, bezierCenter, bezierEnd, chaseSpeed, chaseMode, this.gameObject.transform.position);
                            temp2.SetPlayerPosition(playerPos);
                        }


                        break;
                    }
                case BossPatternName.LineFall:
                    {
                        targetPos = spawnPosition;

                        spawnPosition = playerPos;
                        spawnPosition.y += screenHeight;

                        //Debug.Log("H2 : " + screenHeight);
                        chaseMode = true;


                        direction = opCurves.SeekDirection(spawnPosition, targetPos);

                        lineFallMissilePosition = CreateLineFallMissilePos();

                        GameObject temp = Instantiate(lineFallMissileSquare, spawnPosition, Quaternion.Euler(0, 0, 0));                       
                        LineFallMissileSquare temp2 = temp.GetComponent<LineFallMissileSquare>();


                        temp2.SetRoute(direction, (int)lineFallMissilePosition, speedLF, this.gameObject.transform.position);
                        temp2.SetPlayerPosition(playerPos);

                        break;
                    }
                case BossPatternName.SlowRailGun:
                    {
                        targetPos = spawnPosition;

                        spawnPosition = playerPos;
                        spawnPosition.y += screenHeight * 0.5f;

                        direction = opCurves.SeekDirection(spawnPosition, targetPos);

                        lineFallMissilePosition = CreateLineFallMissilePos();

                        GameObject temp = Instantiate(lineFallSlowRailGun, spawnPosition, Quaternion.Euler(0, 0, 0));
                        LineFallMissile temp2 = temp.GetComponent<LineFallMissile>();

                        Debug.Log("Speed : " + speedSRG);

                        temp2.SetRoute(direction, Vector2.zero, Vector2.zero, Vector2.zero, speedSRG, true, this.gameObject.transform.position);
                        temp2.SetPlayerPosition(playerPos);

                        break;
                    }
                default:
                    Debug.Log("BossShotMissile Module Error!");
                    break;
            }


            // Missile display on Hierarchy Root
            //Debug.Log("in create");
            //if (bossPatternName == BossPatternName.Horizon || bossPatternName == BossPatternName.Vertical)
            //{
            //    int missileStorage;
            //    if (bossPatternName == BossPatternName.Horizon) missileStorage = spawnSpotLeft.Count;
            //    else missileStorage = spawnSpotTop.Count;


            //    for (int i = 5; i < missileStorage; i += 2)
            //    {

            //        Vector2 spawnStart = Vector2.zero;
            //        Debug.Log("Pattern num : " + verticalPattern);
            //        if (verticalPattern == VerticalPattern.LeftToRight)
            //        {
            //            Vector2 target = spawnSpotRight[i];
            //            target.y -= term * 3;
            //            direction = opCurves.SeekDirection(spawnSpotLeft[i], target);

            //            spawnStart = spawnSpotLeft[i];

            //        }
            //        if (verticalPattern == VerticalPattern.RightToLeft)
            //        {
            //            Vector2 target = spawnSpotLeft[i];
            //            target.y -= term * 3;
            //            direction = opCurves.SeekDirection(spawnSpotRight[i], target);

            //            spawnStart = spawnSpotRight[i];

            //        }
            //        //if (bossPatternName == BossPatternName.Vertical)
            //        //{
            //        //    Vector2 target = spawnSpotTop[i];
            //        //    target.y += screenHeight + (term * 3);
            //        //    target.x += gap * 2;
            //        //    direction = opCurves.SeekDirection(spawnSpotTop[i], target);

            //        //    spawnPosition = spawnSpotTop[i];
            //        //}

            //        GameObject temp = Instantiate(bossMissile, spawnStart, Quaternion.Euler(0, 0, 0));
            //        LineFallMissile temp2 = temp.GetComponent<LineFallMissile>();
            //        temp2.SetRoute(direction, bezierStart, bezierCenter, bezierEnd, chaseSpeed, chaseMode, this.gameObject.transform.position, (int)bossPatternName);
            //        temp2.SetPlayerPosition(playerPos);
            //    }
            //}
            //else
            //{
            //    GameObject temp = Instantiate(bossMissile, spawnPosition, Quaternion.Euler(0, 0, 0));
            //    LineFallMissile temp2 = temp.GetComponent<LineFallMissile>();

            //    Debug.Log("not this");
            //    temp2.SetRoute(direction, bezierStart, bezierCenter, bezierEnd, chaseSpeed, chaseMode, this.gameObject.transform.position, (int)bossPatternName);
            //    temp2.SetPlayerPosition(playerPos);
            //}

            shotTimerCheck = 0;
            missileLaunchStack++;
        }
    }


    private void CreateOptionCoord()
    {

        term = screenWidth * 0.05f;
        for (int i = 0; i < options; i++)
        {
            spawnSpotLeft.Add(new Vector2(-1 * (screenWidth * 0.6f), (screenHeight * 0.6f) - (term * i)));
            //GameObject temp = Instantiate(nullObject, spawnSpotLeft[i], Quaternion.Euler(0, 0, 0));
            //temp.name = "Left" + i;
            spawnSpotRight.Add(new Vector2(screenWidth * 0.6f, (screenHeight * 0.6f) - (term * i)));
            //temp = Instantiate(nullObject, spawnSpotRight[i], Quaternion.Euler(0, 0, 0));
            //temp.name = "Right" + i;

        }

        gap = (spawnSpotRight[0].x - spawnSpotLeft[0].x) / term;

        for (int i = 0; i < gap - 2; i++)
        {
            spawnSpotTop.Add(new Vector2((-1 * (screenWidth * 0.6f)) + (term * (i + 1)), screenHeight * 0.6f));
            //GameObject temp = Instantiate(nullObject, spawnSpotTop[i], Quaternion.Euler(0, 0, 0));
            //temp.name = "Top" + i;
        }
    }

    // 190603 LifeBalance
    // Randomize LineFall Missile Location
    private LineFallMissilePosition CreateLineFallMissilePos()
    {
        LineFallMissilePosition result = lineFallMissilePosition;

        int min = (int)LineFallMissilePosition.Left;
        int max = (int)LineFallMissilePosition.StateCount;

        switch (result)
        {
            case LineFallMissilePosition.Left:
                result = (LineFallMissilePosition)Random.Range(min, 2);
                break;
            case LineFallMissilePosition.Middle:
                result = (LineFallMissilePosition)Random.Range(min, max);
                break;
            case LineFallMissilePosition.Right:
                result = (LineFallMissilePosition)Random.Range(1, max);
                break;
            default:
                result = (LineFallMissilePosition)Random.Range(min, max);
                break;
        }

        Debug.Log(result);

        return result;
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

    private void OnDisable()
    {
        player.InitLineFall(false);
    }
}
