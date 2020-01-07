using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MissileModuleLaserNo5 : MonoBehaviour
{
    //InitBossMissileModule(screenHeight, screenWidth, PublicValueStorage.Instance.GetPlayer());
    //SetScreenSize(screenHeight, screenWidth);
    //LaunchMissile();



    public void InitBossMissileModule(float height, float width, GameObject x)
    {

    }

    // This method alternate Update()
    public void LaunchMissile()
    {

    }
}





////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////
// This Module only control about missile
////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////


//public class MissileModuleLaserNo5 : MonoBehaviour
//{

//    // Common Settings
//    [HideInInspector]
//    public int patternCount;

//    enum EnemyState { Spawned = 0, Move, Idle, Play, StateCount }
//    EnemyState enemyState;

//    enum BossPatternState { Shuffle, Selected, Launch, Cooldown, StateCount }
//    BossPatternState bossPatternState;

//    public enum BossPatternName { ReflectLaser = 0, RapidLaser, LaserNo5, DamageChance, SpawnOptions, BreakTimeBeforeCooldown ,StateCount }
//    BossPatternName bossPatternName;
    
//    // Debug Options
//    public BossPatternName alwaysThisPattern;


//    // Prefabs
//    public Laser laser;


//    // LaunchMissile Timers
//    //private int bossPatternNumber = 0;
//    private float shotTimerCheck = 0.0f;
//    public float shotTimer;


//    // BOSS Values (Move with opCurves)
//    public OPCurves opCurves;
//    public ControllerLaserNo5 parent;


//    // SpawnOptions Value
//    [Header("- Spawn Options")]
//    public Transform optionSlotConstPos;
//    private Vector2 ganjiSpawnPosition;
//    private LaserNo5Option[] optionController;
//    private int optionsEa = 0;
//    // 나중에 여기다가 옵션 액세스 전역변수 추가함 private 으로
//    public GameObject optionPrefab;
//    public float spawnSpeed = 0.5f;



//    // Missile common values
//    private float missileSpeed = 1.0f;
//    public float spawnMissileSpotX = 0;
//    public float spawnMissileSpotY = 0;
//    List<int>optionActiveIndexList = new List<int>();

//    [Range(10, 1000)]
//    public int missileStackMin = 10;
//    [Range(30, 1000)]
//    public int missileStackMax = 30;
//    private int missileSaveStack = 0;
//    private int missileLaunchStack = 0;


//    // Chase Values
//    public float chaseSpeedMin = 0;
//    public float chaseSpeedMax = 0.5f;
//    public float chaseStaticSpeed = 1.0f;
//    private float chaseSpeed = 1.0f;


//    // CoolDown Values
//    public float cooldownTimerCheck = 2.0f;
//    public float cooldownTimer = 0.0f;


//    // Missile Orbit Values
//    private Vector2 bezierStart = Vector2.zero;
//    private Vector2 bezierCenter = Vector2.zero;
//    private Vector2 bezierEnd = Vector2.zero;
//    private Vector2 direction = Vector2.zero;

//    private Transform player;
//    private Vector2 targetPos = Vector2.zero;


//    // LaserNo5 Mode Values
//    public int laserNo5MissileMin = 3;
//    public int laserNo5MissileMax = 5;
//    private float laserNo5Timer = 4.7f;
//    private int laserNo5Missile;
//    private bool laserNo5Ended = false;
//    private float laserNo5TimerAfterLaunched = 0;
//    private float laserNo5TimerAfterLaunchedCheck = 0;

//    // Screen Size (temporary)
//    private float screenHeight;
//    private float screenWidth;


//    //void Start()
//    //{
//    //    optionController = new LaserNo5Option[optionSlotConstPos.childCount];
//    //    for (int i = 0; i < optionController.Length; i++)
//    //    {
//    //        GameObject temp = Instantiate(optionPrefab, this.transform.parent);
//    //        optionController[i] = temp.GetComponent<LaserNo5Option>();
//    //        optionController[i].gameObject.SetActive(false);
//    //    }
//    //}

//    public void InitBossMissileModule(float height, float width, GameObject player)
//    {
//        screenHeight = height;
//        screenWidth = width;
//        bossPatternState = BossPatternState.Shuffle;
//        this.player = player.transform;
//        missileSpeed = PublicValueStorage.Instance.GetAddSpeedRivisionValue();

//        ganjiSpawnPosition = this.transform.position;
//        ganjiSpawnPosition.y += 20.0f;

//        optionController = new LaserNo5Option[optionSlotConstPos.childCount];
//        //Debug.Log("Parent Pos : " + this.transform.parent.position);
//        for (int i = 0; i < optionController.Length; i++)
//        {
//            Debug.Log(i);
//            GameObject temp = Instantiate(optionPrefab, this.transform.parent);
 
//            optionsEa++;
//            optionController[i] = temp.GetComponent<LaserNo5Option>();
//            optionController[i].SetSpeedRivision(missileSpeed);
//            optionController[i].gameObject.SetActive(false);
//        }
//    }

//    // 190315 LifeBalance
//    // This is update() of BOSSMissilePattern, will used by ControllerBOSS.cs
//    public void LaunchMissile()
//    {
//        BossMissile();
//    }


//    private void BossMissile()
//    {
//        if (bossPatternState == BossPatternState.Shuffle)
//        {
//            bossPatternState = BossPatternState.Selected;
//        }
//        if (bossPatternState == BossPatternState.Selected)
//        {

//            bossPatternName = (BossPatternName)RandomPatternSelect();
//            missileSaveStack = Random.Range(missileStackMin, missileStackMax);

//            // If all options were dead, State force change to SpawnOptions
//            if (CheckAllOptionsDead() == true)
//            {
//                bossPatternName = BossPatternName.SpawnOptions;
//            }

//            if (alwaysThisPattern != BossPatternName.StateCount)
//                bossPatternName = alwaysThisPattern;

            
//            if (bossPatternName == BossPatternName.SpawnOptions)
//            {
//                for(int i = 0; i < optionController.Length; i++)
//                {
//                    // If options are dead at least one, change deactive options to active options 
//                    if (optionController[i].gameObject.activeSelf == false)
//                        break;
//                    else
//                    {
//                        // If all options are alive
//                        if (optionController[optionController.Length - 1].gameObject.activeSelf == true)
//                        {
//                            cooldownTimerCheck = cooldownTimer * 0.65f;
//                        }
//                    }
//                }
//                missileSaveStack = 1;
//            }


//            Debug.Log("Selected : " + bossPatternName);
//            //Debug.Log("missile Ea : " + missileEa);
//            //Debug.Log("missile stack : " + missileSaveStack);

//            bossPatternState = BossPatternState.Launch;
//        }
//        if (bossPatternState == BossPatternState.Launch)
//        {
//            ShotMissile();
//        }
//        if (bossPatternState == BossPatternState.Cooldown)
//        {
//            cooldownTimerCheck += Time.deltaTime;

//            if (cooldownTimer <= cooldownTimerCheck)
//            {
//                bossPatternState = BossPatternState.Shuffle;
//                cooldownTimerCheck = 0;
//            }
//        }
//    }

//    // 190318 LifeBalance
//    // Launch Missile method
//    private void ShotMissile()
//    {
//        if (missileLaunchStack >= missileSaveStack)
//        {
//            Debug.Log("Shuffle");
//            bossPatternState = BossPatternState.Cooldown;
//            missileLaunchStack = 0;
//            return;
//        }
//        shotTimerCheck += Time.deltaTime;
//        if (shotTimerCheck >= shotTimer)
//        {
//            Vector2 spawnPosition = this.transform.position;
//            bool chaseMode = false;
//            chaseSpeed = chaseStaticSpeed;

//            switch (bossPatternName)
//            {
//                case BossPatternName.ReflectLaser:
//                    {
//                        // All options are dead
//                        // BOSS activate own Laser
//                        if (CheckAllOptionsDead() == true)
//                        {

//                        }
//                        else
//                        {
//                            int launchIndex = GetLaunchOptionNumber(optionActiveIndexList);
//                            optionController[launchIndex].SetLaserPattern(0);
//                        }
//                        missileLaunchStack = missileSaveStack;
//                        break;
//                    }
//                case BossPatternName.RapidLaser:
//                    {
//                        // All options are dead
//                        // Fix GetLaunchOptionNumber method to Line 259's algorithm
//                        if (CheckAllOptionsDead() == true)
//                        {
//                            missileLaunchStack = missileSaveStack;
//                        }
//                        else
//                        {
//                            int launchIndex = GetLaunchOptionNumber(optionActiveIndexList);
//                            optionController[launchIndex].SetLaserPattern(1);
//                        }
//                        break;
//                    }
//                case BossPatternName.LaserNo5:
//                    {
//                        Debug.Log("Project LaserNo5 is launched . . . . . . .");

//                        if (laserNo5Ended == true)
//                        {
//                            laserNo5TimerAfterLaunchedCheck += Time.deltaTime;
//                            if (laserNo5TimerAfterLaunchedCheck >= laserNo5TimerAfterLaunched)
//                            {
//                                laserNo5TimerAfterLaunchedCheck = 0;
//                                missileLaunchStack = missileSaveStack;
//                                laserNo5Ended = false;
//                                break;
//                            }
//                        }
//                        // laserNo5Ended == false
//                        else
//                        {

//                            int realLaser = -1;

//                            optionActiveIndexList = RefreshAliveOptionList(optionActiveIndexList);

//                            laserNo5Missile = Random.Range(laserNo5MissileMin, laserNo5MissileMax);
//                            laserNo5Missile -= 1;

//                            if (optionActiveIndexList.Count == 0)
//                            {
//                                break;
//                            }
//                            else if (optionActiveIndexList.Count == 1)
//                            {
//                                for (int i = 0; i < laserNo5Missile; i++)
//                                {
//                                    optionController[0].SetLaserPattern(2, laserNo5Timer);
//                                    laserNo5TimerAfterLaunched += laserNo5Timer;
//                                }
//                                laserNo5Ended = true;
//                            }
//                            else
//                            {

//                                for (int missileLoop = 0; missileLoop < laserNo5Missile; missileLoop++)
//                                {
//                                    realLaser = Random.Range(0, optionActiveIndexList.Count);
//                                    for (int i = 0; i < optionActiveIndexList.Count; i++)
//                                    {
//                                        if (i == realLaser) continue;

//                                        optionController[i].SetLaserPattern(2, laserNo5Timer);
//                                    }
//                                    optionController[realLaser].SetLaserPattern(2, laserNo5Timer);
//                                    laserNo5TimerAfterLaunched += laserNo5Timer;
//                                }
//                                laserNo5Ended = true;
//                            }
//                        }
//                        break;
//                    }
//                case BossPatternName.DamageChance:
//                    {
//                        break;
//                    }
//                case BossPatternName.SpawnOptions:
//                    {
//                        direction = opCurves.SeekDirection(spawnPosition, player.position);
//                        chaseMode = true;

//                        for (int i = 0; i < optionController.Length; i++)
//                        {

//                            if (optionController[i].gameObject.activeSelf == false)
//                            {
//                                optionController[i].gameObject.SetActive(true);
//                            }

//                            // Normal Move Mode
//                            //optionController[i].InitOption(this.transform.position, optionSlotConstPos.GetChild(i), player);

//                            // Ganji Move Mode
//                            //Debug.Log("Ganji Pos : " + ganjiSpawnPosition);
//                            optionController[i].InitOption(ganjiSpawnPosition, optionSlotConstPos.GetChild(i), player);
//                        }
//                        break;
//                    }
//                default:
//                    Debug.Log("BossShotMissile Module Error!");
//                    break;
//            }

//            // Missile display on Hierarchy Root

//            shotTimerCheck = 0;
//            missileLaunchStack++;
//        }
//    }

//    public void SetScreenSize(float height, float width)
//    {
//        screenHeight = height;
//        screenWidth = width;
//    }

//    public int RandomPatternSelect()
//    {
//        return Random.Range(0, (int)BossPatternName.StateCount);
//    }

//    private List<int> RefreshAliveOptionList(List<int> indexList)
//    {
//        indexList.Clear();

//        for (int i = 0; i < optionController.Length; i++)
//        {
//            if (optionController[i].gameObject.activeInHierarchy == true)
//            {
//                indexList.Add(i);
//            }
//        }
//        return indexList;
//    }

//    private int GetLaunchOptionNumber(List<int> indexList)
//    {
//        int res = 0;

//        indexList = RefreshAliveOptionList(indexList);
//        // new method
//        res = Random.Range(0, indexList.Count);

//        return res;

//        // old method
//        // index "res" optionCotroller is dead
//        //while (true)
//        //{
//        //    res = Random.Range(0, optionController.Length);

//        //    if (optionController[res].gameObject.activeSelf == false)
//        //        continue;
//        //    else
//        //        return res;
//        //}
//    }


//    public void CheckAllOptionsDestroyed()
//    {
//        bool isAllDestroyed = CheckAllOptionsDead();

//        if (isAllDestroyed == true)
//        {
//            parent.GetDamagedByAllOptionDestroyed();
//        }
//    }

//    // Check all Options dead
//    private bool CheckAllOptionsDead()
//    {
//        optionActiveIndexList = RefreshAliveOptionList(optionActiveIndexList);

//        // new method
//        // All options are dead
//        if (optionActiveIndexList.Count == 0)
//        {
//            return true;
//        }
//        // At least alive one
//        else
//        {
//            return false;
//        }


//        // old method
//        //for (int i = 0; i < optionController.Length; i++)
//        //{
//        //    // If options are dead at least one, change deactive options to active options 
//        //    if (optionController[i].gameObject.activeSelf == true)
//        //    {
//        //        return false;
//        //    }
//        //    else
//        //    {
//        //        // If all options are dead
//        //        if (optionController[optionController.Length - 1].gameObject.activeSelf == false)
//        //        {
//        //            return true;
//        //        }
//        //    }
//        //}

//        return false;
//    }
//}

