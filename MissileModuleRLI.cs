using System.Collections;
using System.Collections.Generic;
using UnityEngine;



////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////
// This Module only control about missile
////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////


public class MissileModuleRLI : MonoBehaviour
{
    // Debug
    public bool DebugModule = false;
    public bool AllStop = false;
    //public ControllerPlayer debugPlayer;
    public ControllerPlayer player;

    // Common Settings
    [HideInInspector]
    public int patternCount;

    enum EnemyState { Spawned = 0, Move, Idle, Play, StateCount }
    EnemyState enemyState;

    enum BossPatternState { Shuffle, Selected, Launch, Cooldown, StateCount }
    BossPatternState bossPatternState;

    public enum BossPatternName { EditLadder = 0, EditRoute, DamageChance, Attack, StateCount, Straight, RopeLadderIlluminator, FirstLaunch }
    BossPatternName bossPatternName;

    public enum LadderState { Three = 0, Four, StateCount }
    public LadderState ladderState;

    // Debug Options
    public BossPatternName alwaysThisPattern;

    public delegate void routeHandler();
    public static event routeHandler destroyAllRoute;

    private void OnDestroyAllRoute()
    {
        //Debug.Log("Connected Ladder : " + destroyAllRoute.GetInvocationList().Length);
        if (destroyAllRoute == null)
        {
            //Debug.Log("Is empty");
            return;
        }
        destroyAllRoute();
    }

    private void ClearDestroyAllRouteInvoke()
    {
        destroyAllRoute = null;
    }


    // Class
    [System.Serializable]
    public class LineDotPosition
    {
        public Vector2 start, end;

        public void SetPosition(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }

        public void SetGapY(float gap)
        {
            start.y -= gap;
            end.y += gap;
        }
        public void SetGapX(float gap)
        {
            start.x += gap;
            end.x += gap;
        }

        public void AddXPosition(float x)
        {
            start.x += x;
            end.x += x;
        }
    }

    [System.Serializable]
    public class LadderPrefab
    {
        public static Vector2 originalPosition = Vector2.zero;
        public static string name = "Ladder ";
        public static int index = 0;

        public GameObject ladder, start, end;



        public static void SetOriginalPosition(Vector2 position)
        {
            originalPosition = position;
        }
        public static Vector3 GetOriginalPosition()
        {
            return originalPosition;
        }

        public LadderPrefab(GameObject ladder)
        {
            this.ladder = ladder;
            this.ladder.name = name + index;
            index++;

            start = ladder.transform.GetChild(0).gameObject;
            end = ladder.transform.GetChild(1).gameObject;
        }
    }


    public class routeInfo
    {
        public enum EndType { Up = 0, Middle, Down, StateCount, MissingNo }
        public enum LastDirection { Left = 0, Right, StateCount }

        private Vector3 lastPosLeft, lastPosRight;

        public routeInfo()
        {
            lastPosLeft = lastPosRight = Vector3.zero;
        }

        public void SetLastPosition(Vector3 position, LastDirection direction)
        {
            switch (direction)
            {
                case LastDirection.Left:
                    lastPosLeft = position;
                    break;

                case LastDirection.Right:
                    lastPosRight = position;
                    break;

                default:
                    //Debug.LogError("Set Last Position Error");
                    break;
            }
        } 

        public void GetNextYPosition(ref Vector3 myPosition, LastDirection direction, float routeTerm)
        {
            switch (direction)
            {
                case LastDirection.Left:
                    if (lastPosLeft.y - myPosition.y >= (routeTerm * 2.0f))
                        routeTerm *= 1.5f;
                        
                    myPosition.y = Random.Range(myPosition.y, lastPosLeft.y - routeTerm);
                    break;

                case LastDirection.Right:
                    if (lastPosRight.y - myPosition.y >= (routeTerm * 2.0f))
                        routeTerm *= 1.5f;

                    myPosition.y = Random.Range(myPosition.y, lastPosRight.y - routeTerm);
                    break;

                default:
                    //Debug.LogError("Get Next Position Error");
                    break;
            }
        }

        public void Reset(Vector3 resetPosition)
        {
            lastPosLeft = lastPosRight = resetPosition;
        }
    }

    //
    [Header("- Sound")]
    public AudioClip soundEditLadder;
    public AudioClip soundEditRoute;
    public AudioClip soundRLIMissile;
    private AudioClip soundSendClip;
    private SoundManager.Speaker soundSendSpeaker = SoundManager.Speaker.Center;

    // Prefabs
    [Header("- Prefabs")]
    public GameObject ladderPrefab;
    public LadderRoute routePrefab;
    public GameObject RLIMissilePrefab;
    public SpriteRenderer boss;
    public GameObject routeLeftBox;
    public GameObject routeRightBox;

    // LaunchMissile Timers
    [Header("- LaunchMissile Timers")]
    private float shotTimerCheck = 0.0f;
    public float shotTimer;


    // BOSS Values (Move with opCurves)
    [Header("- opCurves")]
    public OPCurves opCurves;
    private bool attackSetRoute = false;

    private List<int> attackLadderIndex = new List<int>();
    private int attackRoutes = 0;
    private int attackRoutesMin = 1;

    private bool attackSetTurnCount = false;
    private int attackTurns = 1;
    private int attackTurnMax = 2;
    private int attackTurnCount = 0;
    private bool attackForceMode = false;
    private bool attackLastBullet = false;

    // Launch Ladder
    [Header("- Launch Ladder values")] 
    public float LadderAnimationLeftToRightSpeed = 2.0f;
    public float LadderAnimationUpToDownSpeed = 2.0f;

    [SerializeField]
    private float LadderAnimationUpToDownValue = 0;
    [SerializeField]
    private float LadderAnimationLeftToRightValue = 0;
    [SerializeField]
    private bool LadderUpToDownAnimation = true, LadderLeftToRightAnimation = false;

    private List<LadderPrefab> LadderList;
    private List<Vector3> LadderPositionThreeList;
    private List<Vector3> LadderPositionFourList;
    private List<Vector3> LadderRightEndPositionList = new List<Vector3>();
    private SpriteRenderer parentSpriteRenderer;
    private bool firstLaunch;
    private bool firstEditRoute = true;
    private bool firstLaunchPattern;
    private bool editRoute = false;
    private bool editLadderEndOfMergeAnimation = false;
    private bool editLadderEndOfSpreadAnimation = true;


    private float LadderGapXThree = 0, LadderGapXFour = 0;

    private Vector2 LadderUpPosition, LadderDownPosition;


    // EditRoute values
    [Header("- EditRoute values")]
    public int routeMin = 10;
    public int routeMax = 15;
    private int routeEa = 0;
    private int routeLaunchedEa = 0;


    private bool routeBossPositionChange = false;
    private int routeBossPositionIndex = -1;
    private Vector3 routeBossCurrentPosition, routeBossNextPosition;
    private float routeBossMoveProcess = 0;
    private float routeBossMoveSpeed = 2.0f;

    private float routeProcess = 0;
    private float routeTerm = 0;
    private float routeTermProcess = 0;
    private Vector3[] routeStart = new Vector3[2];

    private routeInfo[] routeInfos = new routeInfo[2];

    // Missile common values
    [Header("- Missile common values")]
    [Range(10, 1000)]
    public int missileStackMin = 10;
    [Range(30, 1000)]
    public int missileStackMax = 30;

    private float missileSpeed = 1.0f;
    private int missileSaveStack = 0;
    private int missileLaunchStack = 0;
    private float PVCrivision = 1.0f;

    // Chase Values
    [Header("- Chase values")]
    public float chaseSpeedMin = 0;
    public float chaseSpeedMax = 0.5f;
    public float chaseStaticSpeed = 1.0f;
    private float chaseSpeed = 1.0f;


    // CoolDown Values
    [Header("- CoolDown values")]
    public float cooldownTimerCheck = 2.0f;
    public float cooldownTimer = 0.0f;


    // Missile Orbit Values
    private Vector2 bezierStart = Vector2.zero;
    private Vector2 bezierCenter = Vector2.zero;
    private Vector2 bezierEnd = Vector2.zero;
    private Vector2 direction = Vector2.zero;

    private Vector2 targetPos = Vector2.zero;

    // Screen Size (temporary)
    private float screenHeight;
    private float screenWidth;

    public void InitBossMissileModule(float height, float width, GameObject player)
    {
        LadderList = new List<LadderPrefab>();


        screenHeight = height;
        screenWidth = width;
        bossPatternState = BossPatternState.Shuffle;
        this.player = player.GetComponent<ControllerPlayer>();
        missileSpeed = PublicValueStorage.Instance.GetAddSpeedRivisionValue();

        //GameObject copyLadder = Instantiate(Ladder, this.transform.parent);
        LadderPrefab temp = new LadderPrefab(Instantiate(ladderPrefab, this.transform.parent));
        LadderList.Add(temp);
        firstLaunch = true;

        if (DebugModule == true)
        {
            player.transform.position = new Vector3(0, -4.1f, 0);
        }

    }

    // 190315 LifeBalance
    // This is update() of BOSSMissilePattern, will used by ControllerBOSS.cs
    public void LaunchMissile()
    {
        //Debug.Log("::: " + bossPatternState);
        if (AllStop != true)
        {
            BossMissile();

            if (routeBossPositionChange == true)
            {
                if (routeBossMoveProcess == 0)
                {
                    RotateBossToDefault();
                }

                routeBossMoveProcess += Time.deltaTime * routeBossMoveSpeed;
                this.transform.parent.GetChild(0).transform.position =
                    Vector3.Lerp(routeBossCurrentPosition, routeBossNextPosition, routeBossMoveProcess);


                if (routeBossMoveProcess >= 1.0f)
                {
                    routeBossPositionChange = false;
                    routeBossMoveProcess = 0;
                }
            }
        }
    }


    private void BossMissile()
    {
        //Debug.Log("Pattern : " + bossPatternName);
        //Debug.Log("Pattern : " + bossPatternState);
        if (bossPatternState == BossPatternState.Shuffle)
        {
            bossPatternState = BossPatternState.Selected;
            //Debug.Log("Selected");
        }
        if (bossPatternState == BossPatternState.Selected)
        {

            bossPatternName = (BossPatternName)RandomPatternSelect();
            missileSaveStack = Random.Range(missileStackMin, missileStackMax);
            if (firstLaunch == true)
            {
                firstLaunchLadder();
            }
            else
            {
                if (alwaysThisPattern != BossPatternName.StateCount)
                {
                    bossPatternName = alwaysThisPattern;
                }
                if (firstLaunchPattern == true)
                {
                    bossPatternName = BossPatternName.FirstLaunch;
                    SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundEditLadder);
                }
                else
                {
                    // if RLI is first luanched,
                    // RLI Pattern must be EditRoute
                    if (firstEditRoute == true)
                    {
                        bossPatternName = BossPatternName.EditRoute;

                        routeTerm = LadderList[1].start.transform.position.y - LadderList[1].end.transform.position.y;
                        routeEa = Random.Range(routeMin, routeMax);
                        routeTerm /= routeEa;

                        firstEditRoute = false;
                        editRoute = false;

                        for (int i = 0; i < routeInfos.Length; i++)
                        {
                            routeStart[i] = LadderList[i + 1].start.transform.position;
                            routeInfos[i] = new routeInfo();
                            routeInfos[i].Reset(routeStart[i]);
                        }
                    }
                    else
                    {
                        if (editRoute == true)
                        {
                            bossPatternName = BossPatternName.EditRoute;
                            editRoute = false;
                        }
                        else if (bossPatternName == BossPatternName.EditRoute)
                        {
                            //Debug.Log("EditRoute Must set after EditLadder");
                            bossPatternName = BossPatternName.StateCount;
                            cooldownTimerCheck = cooldownTimer - 1.0f;
                        }

                        if (attackForceMode == true)
                        {
                            bossPatternName = BossPatternName.Attack;
                            if (attackSetTurnCount == false)
                            {
                                attackTurns = Random.Range(1, attackTurnMax);
                                attackSetTurnCount = true;

                                RotateBossToDefault();
                            }
                            else
                            {
                                attackTurnCount++;

                                // Turn to Player's attack chance
                                if (attackTurnCount >= attackTurns)
                                {
                                    //Debug.Log("Count Reset");
                                    attackForceMode = false;
                                    attackSetTurnCount = false;
                                    attackTurnCount = 0;
                                    bossPatternName = BossPatternName.DamageChance;
                                }
                            }

                        }

                        switch (bossPatternName)
                        {
                            case BossPatternName.EditLadder:
                                ladderState = (LadderState)Random.Range(0, (int)LadderState.StateCount);
                                SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundEditLadder);
                                OnDestroyAllRoute();
                                ClearDestroyAllRouteInvoke();
                                break;

                            case BossPatternName.EditRoute:
                                SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundEditRoute);
                                routeTerm = LadderList[1].start.transform.position.y - LadderList[1].end.transform.position.y;
                                routeEa = Random.Range(routeMin, routeMax);
                                routeTerm /= routeEa;

                                firstEditRoute = false;
                                editRoute = false;
                                routeProcess = 0;

                                routeBossPositionIndex = 0;
                                routeBossPositionChange = true;
                                switch (ladderState)
                                {
                                    case LadderState.Three:
                                        routeBossPositionIndex = Random.Range(0, LadderPositionThreeList.Count);
                                        break;

                                    case LadderState.Four:
                                        routeBossPositionIndex = Random.Range(0, LadderPositionFourList.Count);
                                        break;

                                    default:
                                        break;
                                }

                                routeBossCurrentPosition = this.transform.parent.GetChild(0).position;
                                routeBossNextPosition = 
                                    new Vector3
                                    (LadderList[routeBossPositionIndex].start.transform.position.x, 
                                    routeBossCurrentPosition.y, 
                                    routeBossCurrentPosition.z);

                                for (int i = 0; i < routeInfos.Length; i++)
                                {
                                    routeStart[i] = LadderList[i + 1].start.transform.position;
                                    routeInfos[i] = new routeInfo();
                                    routeInfos[i].Reset(routeStart[i]);
                                }

                                LadderRightEndPositionList.Clear();
                                break;

                            case BossPatternName.DamageChance:
                                break;

                            case BossPatternName.Attack:
                                break;

                            case BossPatternName.Straight:
                                break;

                            case BossPatternName.RopeLadderIlluminator:
                                break;

                            default:
                                break;
                        }
                    }
                }

                //Debug.Log("Selected : " + bossPatternName);
                //Debug.Log("missile Ea : " + missileEa);
                //Debug.Log("missile stack : " + missileSaveStack);

                bossPatternState = BossPatternState.Launch;
            }
        }
        if (bossPatternState == BossPatternState.Launch)
        {
            ShotMissile();
        }
        if (bossPatternState == BossPatternState.Cooldown)
        {
            cooldownTimerCheck += Time.deltaTime;

            if (cooldownTimer <= cooldownTimerCheck)
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
            //Debug.Log("Shuffle");
            bossPatternState = BossPatternState.Cooldown;
            missileLaunchStack = 0;
            attackSetRoute = false;
            shotTimer = 0;

            if (bossPatternName == BossPatternName.EditRoute)
            {
                //Debug.Log("ForceMode");
                attackForceMode = true;
            }
            if (bossPatternName == BossPatternName.Attack)
            {
                shotTimer = 0;
            }

            return;
        }
        shotTimerCheck += Time.deltaTime;
        if (shotTimerCheck >= shotTimer)
        {
            Vector2 spawnPosition = this.transform.position;
            chaseSpeed = chaseStaticSpeed;

            switch (bossPatternName)
            {
                case BossPatternName.EditLadder:
                    // end method
                    missileLaunchStack = -1;
                    EditLadder();
                    break;

                case BossPatternName.EditRoute:
                    missileLaunchStack = -1;
                    EditRoute();
                    break;

                case BossPatternName.DamageChance:
                    StartAttack(true);
                    break;

                case BossPatternName.Attack:
                    StartAttack(false);
                    break;

                case BossPatternName.Straight:
                    break;

                case BossPatternName.RopeLadderIlluminator:
                    break;

                // Only run first time when RLI launched
                case BossPatternName.FirstLaunch:
                    missileLaunchStack = -1;
                    LaunchThreeLadder();
                    //LaunchFourLadder();
                    break;

                default:
                    //Debug.Log("BossShotMissile Module Error!");
                    break;
            }

            // Missile display on Hierarchy Root

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

    private void firstLaunchLadder()
    {
        //float gapY = boss.bounds.extents.y;
        float gapY = screenWidth * 0.2f;
        //Debug.Log("Gap Y : " + Camera.main.orthographicSize);
        LadderPositionThreeList = new List<Vector3>();
        LadderPositionFourList = new List<Vector3>();

        // Set Calculate width gap
        float size = screenWidth - (gapY * 2);
        LadderGapXThree = size / 3.0f;
        LadderGapXFour = size / 3.0f;


        Vector2 basePosition = Vector2.zero;
        basePosition.x -= LadderGapXThree;

        // Set Ladder three only x position
        for (int i = 0; i < 3; i++)
        {
            LadderPositionThreeList.Add(basePosition);
            basePosition.x += LadderGapXThree;
        }


        // Set Ladder four only x position
        basePosition = Vector2.zero;
        basePosition.x -= LadderGapXFour + (LadderGapXFour * 0.5f);
        for (int i = 0; i < 4; i++)
        {
            LadderPositionFourList.Add(basePosition);
            basePosition.x += LadderGapXFour;
        }

        // Set Ladder Position
        LadderUpPosition = this.transform.parent.position;
        LadderUpPosition.y -= gapY;

        LadderDownPosition = player.transform.position;
        LadderDownPosition.y += gapY;

        LadderList[0].ladder.transform.position = LadderUpPosition;
        //LadderList[0].start.transform.position = LadderUpPosition;
        LadderList[0].end.transform.position = LadderDownPosition;
        LadderList[0].ladder.SetActive(true);

        bossPatternName = BossPatternName.FirstLaunch;

        firstLaunch = false;
        firstLaunchPattern = true;
    }

    private void LaunchThreeLadder()
    {
        if (bossPatternName == BossPatternName.FirstLaunch)
        {
            if (LadderUpToDownAnimation == true)
            {
                if (LadderAnimationUpToDownValue >= 1.0f)
                {
                    LadderPrefab.SetOriginalPosition(LadderList[0].ladder.transform.position);
                    for (int i = 0; i < LadderPositionThreeList.Count; i++)
                    {
                        GameObject temp = Instantiate(LadderList[0].ladder, this.transform.parent);
                        LadderPrefab copyLadder = new LadderPrefab(temp);

                        LadderList.Add(copyLadder);
                    }

                    LadderList[LadderList.Count - 1].ladder.SetActive(false);

                    LadderUpToDownAnimation = false;
                    LadderLeftToRightAnimation = true;
                }
                else
                {
                    LadderAnimationUpToDownValue += Time.deltaTime * LadderAnimationUpToDownSpeed;

                    LadderList[0].end.transform.position =
                        Vector2.Lerp(LadderList[0].start.transform.position,
                                    LadderDownPosition,
                                    LadderAnimationUpToDownValue);
                }
            }

            // Defalut Line is Three
            if (LadderLeftToRightAnimation == true)
            {
                //
                //
                //
                //
                //
                // End of Method
                if (LadderAnimationLeftToRightValue >= 1.0f)
                {
                    missileLaunchStack = missileSaveStack;

                    ladderState = LadderState.Three;
                    LadderLeftToRightAnimation = false;

                    LadderAnimationLeftToRightValue = 0;

                    cooldownTimerCheck = cooldownTimer - 1.0f;
                    firstLaunchPattern = false;

                    // Debug Only
                    player.InitRLI(true, LadderPositionThreeList);
                }
                else
                {
                    LadderAnimationLeftToRightValue += Time.deltaTime * LadderAnimationLeftToRightSpeed;
                    for (int i = 0; i < LadderPositionThreeList.Count; i++)
                    {
                        LadderList[i].ladder.transform.position =
                            Vector2.Lerp(LadderPrefab.GetOriginalPosition(),
                            LadderPrefab.GetOriginalPosition() + LadderPositionThreeList[i],
                            LadderAnimationLeftToRightValue);
                    }
                }
            }
        }
        else
        {
            // Merge ladder Animation
            if (editLadderEndOfMergeAnimation == false)
            {
                LadderAnimationLeftToRightValue += Time.deltaTime * LadderAnimationLeftToRightSpeed;
                for (int i = 0; i < LadderList.Count; i++)
                {
                    LadderList[i].ladder.transform.position =
                        Vector2.Lerp(LadderList[i].ladder.transform.position,
                                     LadderPrefab.GetOriginalPosition(),
                                     LadderAnimationLeftToRightValue);
                }

                // EoM
                if (LadderAnimationLeftToRightValue >= 1.3f)
                {
                    editLadderEndOfMergeAnimation = true;
                    editLadderEndOfSpreadAnimation = false;

                    // Must deactive ladderList's item of last index
                    LadderList[LadderList.Count - 1].ladder.SetActive(false);
                    LadderAnimationLeftToRightValue = 0;
                }
            }

            // Spread Ladder Animation
            if (editLadderEndOfSpreadAnimation == false)
            {
                LadderAnimationLeftToRightValue += Time.deltaTime * LadderAnimationLeftToRightSpeed;
                for (int i = 0; i < LadderPositionThreeList.Count; i++)
                {
                    LadderList[i].ladder.transform.position =
                        Vector2.Lerp(LadderPrefab.GetOriginalPosition(),
                        LadderPrefab.GetOriginalPosition() + LadderPositionThreeList[i],
                        LadderAnimationLeftToRightValue);
                }

                // EoM
                if (LadderAnimationLeftToRightValue >= 1.0f)
                {
                    editLadderEndOfMergeAnimation = false;
                    editLadderEndOfSpreadAnimation = true;

                    LadderAnimationLeftToRightValue = 0;

                    editRoute = true;

                    cooldownTimerCheck = cooldownTimer - 1.0f;

                    player.InitRLI(true, LadderPositionThreeList);
                    QuitShotMissile();

                    //PatternAllStop();
                }

            }
        }
    }

    private void LaunchFourLadder()
    {
        if (bossPatternName == BossPatternName.FirstLaunch)
        {
            if (LadderUpToDownAnimation == true)
            {
                if (LadderAnimationUpToDownValue >= 1.0f)
                {
                    LadderPrefab.SetOriginalPosition(LadderList[0].ladder.transform.position);
                    for (int i = 0; i < LadderPositionFourList.Count - 1; i++)
                    {
                        GameObject temp = Instantiate(LadderList[0].ladder, this.transform.parent);
                        LadderPrefab copyLadder = new LadderPrefab(temp);

                        LadderList.Add(copyLadder);

                    }

                    //LadderList[LadderList.Count - 1].ladder.SetActive(false);

                    LadderUpToDownAnimation = false;
                    LadderLeftToRightAnimation = true;
                }
                else
                {
                    LadderAnimationUpToDownValue += Time.deltaTime * LadderAnimationUpToDownSpeed;

                    LadderList[0].end.transform.position =
                        Vector2.Lerp(LadderList[0].start.transform.position,
                                    LadderDownPosition,
                                    LadderAnimationUpToDownValue);
                }
            }

            // Default Line is Three
            if (LadderLeftToRightAnimation == true)
            {
                //
                //
                //
                //
                //
                //
                // End of Method
                if (LadderAnimationLeftToRightValue >= 1.0f)
                {
                    missileLaunchStack = missileSaveStack;
                    ladderState = LadderState.Four;
                    LadderLeftToRightAnimation = false;

                    LadderAnimationLeftToRightValue = 0;
                    firstLaunchPattern = false;
                    cooldownTimerCheck = cooldownTimer - 1.0f;

                    player.InitRLI(true, LadderPositionFourList);
                }
                else
                {
                    LadderAnimationLeftToRightValue += Time.deltaTime * LadderAnimationLeftToRightSpeed;
                    for (int i = 0; i < LadderPositionFourList.Count; i++)
                    {
                        LadderList[i].ladder.transform.position =
                            Vector2.Lerp(LadderPrefab.GetOriginalPosition(),
                            LadderPrefab.GetOriginalPosition() + LadderPositionFourList[i],
                            LadderAnimationLeftToRightValue);
                    }
                }
            }
        }
        else
        {
            // Merge ladder Animation
            if (editLadderEndOfMergeAnimation == false)
            {
                LadderAnimationLeftToRightValue += Time.deltaTime * LadderAnimationLeftToRightSpeed;
                for (int i = 0; i < LadderList.Count; i++)
                {
                    LadderList[i].ladder.transform.position =
                        Vector2.Lerp(LadderList[i].ladder.transform.position,
                                     LadderPrefab.GetOriginalPosition(),
                                     LadderAnimationLeftToRightValue);
                }


                // EoM
                if (LadderAnimationLeftToRightValue >= 1.3f)
                {
                    editLadderEndOfMergeAnimation = true;
                    editLadderEndOfSpreadAnimation = false;

                    // Must active ladderList's item of last index
                    LadderList[LadderList.Count - 1].ladder.SetActive(true);

                    LadderAnimationLeftToRightValue = 0;                   
                }

            }

            // Spread Ladder Animation
            if (editLadderEndOfSpreadAnimation == false)
            {
                LadderAnimationLeftToRightValue += Time.deltaTime * LadderAnimationLeftToRightSpeed;
                for (int i = 0; i < LadderPositionFourList.Count; i++)
                {
                    LadderList[i].ladder.transform.position =
                        Vector2.Lerp(LadderPrefab.GetOriginalPosition(),
                        LadderPrefab.GetOriginalPosition() + LadderPositionFourList[i],
                        LadderAnimationLeftToRightValue);
                }

                // EoM
                if (LadderAnimationLeftToRightValue >= 1.0f)
                {
                    editLadderEndOfMergeAnimation = false;
                    editLadderEndOfSpreadAnimation = true;

                    LadderAnimationLeftToRightValue = 0;

                    editRoute = true;
                    cooldownTimerCheck = cooldownTimer - 1.0f;

                    QuitShotMissile();

                    player.InitRLI(true, LadderPositionFourList);
                    //PatternAllStop();
                }

            }
        }
    }

    private void EditRoute()
    {
        switch (ladderState)
        {
            case LadderState.Three:
                MakeRoutes(3);
                break;
            case LadderState.Four:
                MakeRoutes(4);
                break;
            default:
                //Debug.LogError("LadderState is not valid");
                break;
        }
    }


    private void EditLadder()
    {
        if (ladderState == LadderState.Three)
        {
            LaunchThreeLadder();
        }
        if (ladderState == LadderState.Four)
        {
            LaunchFourLadder();
        }
    }

    private void MakeRoutes(int routeType)
    {
        // route ea is 3
        if (routeType == 3)
        {
            if (routeLaunchedEa >= routeEa - 2)
            {
                QuitShotMissile();
                routeLaunchedEa = 0;
                cooldownTimerCheck = cooldownTimer - 1.0f;
                //Debug.Log("Connected Ladder 2 : " + destroyAllRoute.GetInvocationList().Length);
                //PatternAllStop();
                return;
            }

            if (routeProcess >= 1.0f)
            {
                //Debug.Log("Quit!");
                routeProcess = 0;
                missileLaunchStack = missileSaveStack;
                cooldownTimerCheck = cooldownTimer - 1.0f;
                return;
            }
            routeProcess += 2.0f * Time.smoothDeltaTime;
            routeStart[0] = Vector2.Lerp(LadderList[1].start.transform.position,
                                      LadderList[1].end.transform.position,
                                      routeProcess);

            routeTermProcess = LadderList[1].start.transform.position.y - routeStart[0].y;
            //Debug.Log(routeTermProcess + " / " + routeTerm + " = " + routeTermProcess / routeTerm + " // " + routeStart);

            // When routeTermProcess reach setup routeTerm
            if (routeTermProcess / routeTerm > routeLaunchedEa + 1)
            {

                LadderRoute route = Instantiate(routePrefab.gameObject,
                    routeStart[0],
                    Quaternion.Euler(0, 0, 0)).GetComponent<LadderRoute>();

                Vector3 end = routeStart[0];

                int leftRightType = Random.Range(0, 2);
                bool leftSideType = true;

                int upDownType;
                upDownType = Random.Range(0,
                          (int)routeInfo.EndType.StateCount);
                // If make Route is first time, Do not create upper route
                if (routeTermProcess / routeTerm == 1)
                {
                    //upDownType = Random.Range((int)routeInfo.EndType.Middle, 
                    //                          (int)routeInfo.EndType.StateCount);
                }
                // When make Route is after first time,
                // Check previous upDownType and Set Route direction
                // (Temporary condition)
                else
                {
                    upDownType = Random.Range(0,
                                              (int)routeInfo.EndType.StateCount);
                }

                switch (upDownType)
                {
                    case 0: // UP
                        end.y += routeTerm * 0.2f;
                        break;

                    case 1: // MIDDLE
                        break;

                    case 2: // DOWN
                        end.y -= routeTerm * 0.2f;
                        break;

                    default:
                        //Debug.LogError("Somethings wrong...");
                        break;
                }

                switch (leftRightType)
                {
                    case 0: // RIGHT
                        end.x = LadderList[2].ladder.transform.position.x;
                        route.gameObject.transform.parent = routeRightBox.transform;

                        if (upDownType == 0)
                        {
                            routeInfos[0].GetNextYPosition(ref end, routeInfo.LastDirection.Right, routeTerm);
                        }

                        routeInfos[0].SetLastPosition(end, routeInfo.LastDirection.Right);

                        leftSideType = false;
                        break;
                    case 1: // LEFT
                        end.x = LadderList[0].ladder.transform.position.x;
                        route.gameObject.transform.parent = routeLeftBox.transform;

                        if (upDownType == 0)
                        {
                            routeInfos[0].GetNextYPosition(ref end, routeInfo.LastDirection.Left, routeTerm);
                        }

                        routeInfos[0].SetLastPosition(end, routeInfo.LastDirection.Left);

                        leftSideType = true;
                        break;

                    default:
                        //Debug.LogError("Somethings wrong . . . .");
                        break;
                }

                route.SetPosition(routeStart[0], end, leftSideType);

                destroyAllRoute += route.GetOnDestroyRoute().Invoke;

                routeLaunchedEa++;
            }
        }

        // route ea is 4
        if (routeType == 4)
        {
            for (int i = 0; i < routeInfos.Length;)
            {
                if (routeLaunchedEa >= routeEa - 2)
                {
                    QuitShotMissile();
                    routeLaunchedEa = 0;
                    routeProcess = 0;

                    i++;

                    //PatternAllStop();
                    continue;
                }

                if (routeProcess >= 1.0f)
                {
                    routeProcess = 0;
                    missileLaunchStack = missileSaveStack;
                    cooldownTimerCheck = cooldownTimer - 1.0f;
                    return;
                }
                routeProcess += 2.0f * Time.smoothDeltaTime;
                routeStart[i] = Vector2.Lerp(LadderList[i + 1].start.transform.position,
                                          LadderList[i + 1].end.transform.position,
                                          routeProcess);

                routeTermProcess = LadderList[i + 1].start.transform.position.y - routeStart[i].y;
                //Debug.Log(routeTermProcess + " / " + routeTerm + " = " + routeTermProcess / routeTerm + " // " + routeStart);

                // When routeTermProcess reach setup routeTerm
                if (routeTermProcess / routeTerm > routeLaunchedEa + 1)
                {
                    int leftRightType = Random.Range(0, 2);
                    if (i == 0)
                    {         
                        // Index 0, Type Right
                        if (leftRightType == 0)
                        {
                            if (Random.Range(0, 100) <= 10)
                                leftRightType = 1;
                        }
                    }
                    else if (i == 1)
                    {
                        bool tooClose = false;
                        //Debug.Log(LadderRightEndPositionList.Count);
                        for(int index = 0; index < LadderRightEndPositionList.Count; index++)
                        {
                            if (routeStart[i].y <= LadderRightEndPositionList[index].y + (routeTerm * 0.35f) &&
                                routeStart[i].y >= LadderRightEndPositionList[index].y - (routeTerm * 0.35f))
                            {
                                tooClose = true;
                                break;
                            }
                        }

                        if (tooClose == true)
                        {
                            routeLaunchedEa++;
                            continue;
                        }

                        leftRightType = 0;
                        if (Random.Range(0, 100) <= 10)
                        {
                            routeLaunchedEa++;
                            continue;
                        }


                    }

                    LadderRoute route = Instantiate(routePrefab.gameObject,
                        routeStart[i],
                        Quaternion.Euler(0, 0, 0)).GetComponent<LadderRoute>();

                    Vector3 end = routeStart[i];


                    bool leftSideType = true;

                    int upDownType;
                    upDownType = Random.Range(0,
                              (int)routeInfo.EndType.StateCount);


                    switch (upDownType)
                    {
                        case 0: // UP
                            end.y += routeTerm * 0.2f;
                            break;

                        case 1: // MIDDLE
                            break;

                        case 2: // DOWN
                            end.y -= routeTerm * 0.2f;
                            break;

                        default:
                            //Debug.LogError("Somethings wrong...");
                            break;
                    }

                    switch (leftRightType)
                    {
                        case 0: // RIGHT
                            end.x = LadderList[i + 2].ladder.transform.position.x;
                            route.gameObject.transform.parent = routeRightBox.transform;

                            if (upDownType == 0)
                            {
                                routeInfos[i].GetNextYPosition(ref end, routeInfo.LastDirection.Right, routeTerm);
                            }

                            routeInfos[i].SetLastPosition(end, routeInfo.LastDirection.Right);

                            leftSideType = false;

                            // Add end position in LadderRightEndPositionList,
                            // for not crash i==0's right end position and i==1's start position
                            if (i == 0)
                            {
                                LadderRightEndPositionList.Add(end);
                            }

                            break;
                        case 1: // LEFT
                            end.x = LadderList[i].ladder.transform.position.x;
                            route.gameObject.transform.parent = routeLeftBox.transform;

                            if (upDownType == 0)
                            {
                                routeInfos[i].GetNextYPosition(ref end, routeInfo.LastDirection.Left, routeTerm);
                            }

                            routeInfos[i].SetLastPosition(end, routeInfo.LastDirection.Left);

                            leftSideType = true;
                            break;

                        default:
                            //Debug.LogError("Somethings wrong . . . .");
                            break;
                    }

                    route.name = name + routeLaunchedEa;
                    route.SetPosition(routeStart[i], end, leftSideType);
                    destroyAllRoute += route.GetOnDestroyRoute().Invoke;
                   
                    routeLaunchedEa++;
                }
            }
        }
    }

    private void StartAttack(bool isPlayerAttack)
    {
        if (isPlayerAttack == false)
        {
            if (attackSetRoute == false)
            {
                attackLadderIndex.Clear();

                int attackRouteMax;
                switch (ladderState)
                {
                    case LadderState.Three:
                        attackRouteMax = LadderPositionThreeList.Count;
                        break;

                    case LadderState.Four:
                        attackRouteMax = LadderPositionFourList.Count;
                        break;

                    default:
                        //Debug.LogError("attackRouteMax value is 2 !");
                        attackRouteMax = 2;
                        break;
                }
                attackRoutes = Random.Range(attackRoutesMin, attackRouteMax);

                List<int> exclude = new List<int>();
                for (int i = 0; i < attackRouteMax; i++)
                {
                    exclude.Add(i);
                }

                for (int i = 0; i < attackRoutes; i++)
                {
                    int temp = Random.Range(0, exclude.Count);
                    attackLadderIndex.Add(exclude[temp]);
                    exclude.Remove(exclude[temp]);
                }
                attackSetRoute = true;

                shotTimer = 0.5f;
            }
            // 공격 패턴별 제작
            // Boss Turn
            else
            {

                int positionIndex = Random.Range(0, attackLadderIndex.Count);

                RotateBossToLadder(positionIndex);

                RLIMissile missile = Instantiate(RLIMissilePrefab,
                    LadderList[positionIndex].start.transform.position,
                    Quaternion.Euler(0, 0, 0)).GetComponent<RLIMissile>();

                missile.SetMissileInfo
                    (LadderList[positionIndex].start.transform.position,
                    Vector3.down, missileSpeed);

                if (Random.Range(0, 100) < 10)
                {
                    if (player != null)
                        player.ActivateGetItem(6);
                }
                SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundRLIMissile);

                //Debug.Log("Stock : " + attackLadderIndex.Count + " // position : " + positionIndex);

                // Last Missile
                if (missileLaunchStack == missileSaveStack - 1)
                {
                    missile.SetParentComponent(this);
                    cooldownTimerCheck = -20.0f;
                }
            }
        }
        // Player's Attack
        else
        {
            shotTimer = 0.5f;
            for (int i = 0; i < LadderList.Count; i++)
            {
                if (player.transform.position.x == LadderList[i].end.transform.position.x)
                {
                    RLIMissile missile = Instantiate(RLIMissilePrefab,
                        LadderList[i].end.transform.position,
                        Quaternion.Euler(0, 0, 0)).GetComponent<RLIMissile>();

                    missile.SetMissileInfo
                        (LadderList[i].end.transform.position, 
                        Vector3.up, missileSpeed);

                    missile.tag = "PlayerMissile";

                    SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundRLIMissile);

                    // Last Missile
                    if (missileLaunchStack == missileSaveStack - 1)
                    {
                        missile.SetParentComponent(this);
                        cooldownTimerCheck = -20.0f;
                    }
                    return;
                }
            }

            missileLaunchStack -= 1;
        }
    }

    // When Missile Shot from boss, Rotate boss' eulerRotation this object to target ladder
    Quaternion bossTargetRotation = Quaternion.identity;
    Quaternion bossOriginalRotation = Quaternion.identity;
    float bossRotateProcess = 0;
    float bossRotateSpeed = 5.0f;
    private void RotateBossToLadder(int attackIndex)
    {
        bossOriginalRotation = boss.transform.rotation;
        bossTargetRotation = 
            Quaternion.FromToRotation(Vector3.up, 
             boss.transform.position - LadderList[attackIndex].start.transform.position);
        StartCoroutine(CoroutineRotateToLadder());
    }
    IEnumerator CoroutineRotateToLadder()
    {
        while (true)
        {
            bossRotateProcess += Time.deltaTime * bossRotateSpeed;
            boss.transform.rotation =
                Quaternion.Lerp(bossOriginalRotation, bossTargetRotation, bossRotateProcess);
            if (bossRotateProcess >= 1.0f)
            {
                bossRotateProcess = 0;
                StopCoroutine(CoroutineRotateToLadder());
                yield break;
            }
            yield return null;
        }
    }
    private void RotateBossToDefault()
    {
        bossOriginalRotation = boss.transform.rotation;
        bossTargetRotation = Quaternion.identity;
        StartCoroutine(CoroutineRotateToLadder());
    }
    IEnumerator CoroutineRotateToDefault()
    {
        while (true)
        {
            bossRotateProcess += Time.deltaTime * bossRotateSpeed;
            boss.transform.rotation =
                Quaternion.Lerp(bossOriginalRotation, bossTargetRotation, bossRotateProcess);
            if (bossRotateProcess >= 1.0f)
            {
                bossRotateProcess = 0;
                StopCoroutine(CoroutineRotateToDefault());
                yield break;
            }
            yield return null;
        }
    }


    public void DestroyLastBullet()
    {
        RotateBossToDefault();
        cooldownTimerCheck = cooldownTimer - 1.0f; 
    }

    private void QuitShotMissile()
    {
        missileLaunchStack = missileSaveStack;
    }

    private void PatternAllStop()
    {
        AllStop = true;
    }

    private void OnDestroy()
    {
        //debugPlayer.InitRLI(false);
        ClearDestroyAllRouteInvoke();
        player.InitRLI(false);   
    }
}
