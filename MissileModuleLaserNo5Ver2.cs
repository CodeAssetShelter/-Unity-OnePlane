using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileModuleLaserNo5Ver2 : MonoBehaviour
{
    enum Pattern { Idle = 0, SpawnOption, NormalLaser, CircleLaser, LaserNo5, CoolDown, StateCount, ReflectLaser, PatternStop}
    Pattern pattern;

    // Debug
    [Header("- Debug Triggers")]
    public bool debugMode = false;
    [SerializeField]
    private Pattern constPattern = Pattern.Idle;


    // Player 
    private ControllerPlayer player = null;
    private SpriteRenderer playerSpriteRenderer = null;
    
    // Prefabs
    [Header("- Prefabs")]
    public LaserVer2 laserPrefab;
    public LaserNo5OptionVer2 optionPrefab;

    [Header("- Audio")]
    public AudioClip soundSpawnOptions;
    public AudioClip soundCircleLaser;
    public AudioClip soundNormalLaser;
    public AudioClip soundLaserNo5;
    private AudioClip soundSendClip;
    private SoundManager.Speaker soundSpeaker = SoundManager.Speaker.Center;

    // OptionsValue
    private List<LaserNo5OptionVer2> options;
    public int optionEa = 4;
    public Animator missileExplosion;

    // Triggers
    private bool activateModule = false;


    // CoolDown
    [Header("- CoolDown")]
    public float cooldownDelay = 5.0f;
    [SerializeField]
    private float cooldownTimer = 0;


    // Spawn Positions
    [SerializeField]
    private Vector3 spawnOptionPosition;

    [SerializeField]
    private List<Vector3> optionPositions = new List<Vector3>();

    // Update Method for ControllerLaserNo5Ver2.cs
    public void RunModule()
    {
        if (activateModule == true)
        {
            LaunchMissile();
        }
        else
        {
        }
    }

    public void InitModule(Vector3 spawnStart)
    {
        pattern = Pattern.Idle;

 
        player = PublicValueStorage.Instance.GetPlayerComponent();
        if (player != null)
            playerSpriteRenderer = player.transform.GetChild(0).GetComponent<SpriteRenderer>();

        spawnOptionPosition = spawnStart;
        activateModule = true;

        if (optionEa >= 4) optionEa = 4;

        options = new List<LaserNo5OptionVer2>();
        deadOptionIndex = new List<int>();

        Vector3 center = this.transform.position;
        float gap = PublicValueStorage.Instance.GetScreenSize().x * 0.2f;
        float startPositionX = center.x + (gap * (-1.5f));

        for (int i = 0; i < optionEa; i++)
        {
            LaserNo5OptionVer2 temp = Instantiate(optionPrefab, spawnOptionPosition, Quaternion.Euler(Vector3.zero), this.transform);
            temp.gameObject.SetActive(false);
            options.Add(temp);

            temp.myIndex = i;
            if ( i == 0 || i == optionEa - 1)
            {
                optionPositions.Add(new Vector3(startPositionX, center.y, center.z));
            }
            else
            {
                optionPositions.Add(new Vector3(startPositionX, center.y - (gap * 1.2f), center.z));
            }
            startPositionX += gap;
        }
        optionPositions.Add(new Vector3(gap, 0, 0));

        PublicValueStorage.Instance.SetBossTransform(this.transform);
    }

    private void LaunchMissile()
    {
        switch (pattern)
        {
            case Pattern.Idle:
                PatternShuffle();
                break;

            case Pattern.SpawnOption:
                CreateOptions();
                break;

            case Pattern.NormalLaser:
                LaunchLaser();
                break;

            case Pattern.CircleLaser:
                LaunchLaser();
                break;

            case Pattern.ReflectLaser:
                break;

            case Pattern.LaserNo5:
                LaunchLaser();
                break;

            case Pattern.CoolDown:
                break;

            case Pattern.PatternStop:
                //pattern = Pattern.CoolDown;
                break;
            default:
                //Debug.Log("out of pattern index : " + pattern);
                break;
        }
    }

    //
    // Pattern Methods
    //
    int deadCount;
    int callCount = 0;
    private void PatternShuffle()
    {
        deadCount = CheckHowManyOptionsDead();

        if (debugMode == false)
        {

            if (deadCount == 0)
            {
                pattern = (Pattern)Random.Range((int)Pattern.NormalLaser, (int)Pattern.StateCount - 1);
            }
            else
            {
                pattern = (Pattern)Random.Range(1, (int)Pattern.StateCount - 1);
            }
        }
        else
        {
            // When debugMode is true, this method set next behavior.
            if (deadCount == 0)
            {
                // Make LaserNo5
                pattern = constPattern + 3;      
            }
            else
            {
                pattern = constPattern;
            }
        }

        //Debug.Log("Now Pattern : " + pattern);
        DefineTriggers();        
    }

    private int attackPrevOptionNumber = -1;
    private void DefineTriggers()
    {
        switch (pattern)
        {
            case Pattern.SpawnOption:
                //int deadCount = CheckHowManyOptionsDead();
                if (deadCount == optionEa)
                {
                    //Debug.Log("DEAD COUNT 1 : " + deadCount);    
                    SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundSpawnOptions);
                }
                else if (deadCount >= 1)
                {
                    //Debug.Log("DEAD COUNT 2 : " + deadCount);
                    SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundSpawnOptions);
                }
                else
                {
                    pattern = Pattern.Idle;
                    //Debug.Log("DEAD COUNT 3 : " + deadCount);
                }
                break;

            case Pattern.NormalLaser:
            case Pattern.CircleLaser:
            case Pattern.ReflectLaser:
            case Pattern.LaserNo5:
                attackOptionNumber = PickOption();
                if (attackOptionNumber == -1)
                {
                    pattern = Pattern.Idle;
                    //Debug.Log("option num (-1) : " + attackOptionNumber);
                }
                else { }
                    //Debug.Log("option num (not -1) : " + attackOptionNumber);
                break;

            default:
                //Debug.Log("out of pattern index");
                break;
        }
    }


    [Header("- CreateOptions()")]
    public float spawnSpeed = 3.0f;
    bool animationMode = false;
    List<int> deadOptionIndex;
    float spawnProcess = 0;

    private void CreateOptions()
    {
        if (animationMode == false)
        {
            deadOptionIndex = new List<int>();
            for (int i = 0; i < options.Count; i++)
            {
                // Option was dead
                if (options[i].gameObject.activeSelf == false)
                {
                    deadOptionIndex.Add(i);
                    options[i].gameObject.SetActive(true);

                    //Debug.Log("Dead option number : " + i);
                }
            }

            if (deadOptionIndex.Count != 0)
            {
                animationMode = true;
                //Debug.Log("Switch");
            }
            else // GO NEXT PATTERN
            {
                pattern = Pattern.Idle;           
                //Debug.Log("It is END OF PATTERN 2");
            }
        }
        else
        {
            spawnProcess += spawnSpeed * Time.deltaTime;
            // Revive dead options
            for (int i = 0; i < deadOptionIndex.Count; i++)
            {
                options[deadOptionIndex[i]].transform.position =
                    Vector3.Lerp(spawnOptionPosition, optionPositions[deadOptionIndex[i]], spawnProcess);
            }

            if (spawnProcess >= 1.0f)
            {
                foreach (LaserNo5OptionVer2 option in options)
                {
                    option.SetOnSimulated();
                }
                spawnProcess = 0;
                //pattern = Pattern.Idle;

                animationMode = false;
                //Debug.Log("It is END OF PATTERN");
            }
        }
    }


    // 함수 구현 끝나면 위로 옮긴다
    [Header("- Option MoveSpeed value")]
    public float rotateSpeedToPlayer = 2.0f;
    private float rotateProcessToPlayer = 0;
    [Header("- Picked Option values")]
    int attackOptionNumber = -1;


    private Quaternion []angle;
    private void LaunchLaser()
    {
        if (player == null) return;

        if (rotateProcessToPlayer >= 1.0f)
        {
            attackPrevOptionNumber = attackOptionNumber;
            rotateProcessToPlayer = 0;
            ShotLaser(pattern);
            //Debug.Log("SHOOT LASER VER 2");
        }
        else
        {

            if (rotateProcessToPlayer <= 0)
            {
                angle = new Quaternion[options.Count];
                if (pattern == Pattern.LaserNo5)
                {
                    for (int i = 0; i < options.Count; i++)
                    {
                        angle[i] = 
                            Quaternion.FromToRotation
                            (Vector3.up, 
                            options[i].transform.position - player.transform.position);
                    }
                }
                else
                {
                    angle[0] = Quaternion.FromToRotation(Vector3.up, options[attackOptionNumber].transform.position - player.transform.position);
                }
            }

            rotateProcessToPlayer += rotateSpeedToPlayer * Time.deltaTime;
            
            // 회전식
            if (pattern == Pattern.LaserNo5)
            {
                for (int i = 0; i < options.Count; i++)
                {
                    if (options[i].transform.eulerAngles != angle[i].eulerAngles)
                    {
                        options[i].transform.rotation =
                            Quaternion.Lerp(Quaternion.Euler(Vector3.zero),
                            angle[i], rotateProcessToPlayer);
                    }
                }
            }
            else
            {
                if (options[attackOptionNumber].transform.eulerAngles == angle[0].eulerAngles)
                {
                    rotateProcessToPlayer = 1.0f;
                }
                else
                {
                    options[attackOptionNumber].transform.rotation =
                        Quaternion.Lerp(Quaternion.Euler(Vector3.zero),
                        angle[0], rotateProcessToPlayer);
                }
            }
        }
    }

    [SerializeField]
    private float LaserNo5ShotEa = 30;
    [SerializeField]
    private float LaserNo5ShotFired = 0;
    private void ShotLaser(Pattern type)
    {
        if (type <= Pattern.SpawnOption || type >= Pattern.CoolDown)
        {
            //Debug.LogError(type + " is not valid to " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            return;
        }

        LaserVer2 newLaser;
        switch (type)
        {
            case Pattern.NormalLaser:
                newLaser = Instantiate(laserPrefab, options[attackOptionNumber].GetLaserGunPos(), Quaternion.Euler(Vector3.zero));
                newLaser.gameObject.SetActive(true);
                newLaser.SetLaserInfo(LaserVer2.LaserType.NormalLaser);


                SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundNormalLaser);

                pattern = Pattern.PatternStop;
                StartCoroutine(ActiveCoolDown());
                break;

            case Pattern.CircleLaser:
                //Debug.Log("Make CircleLaser!");
                newLaser = Instantiate(laserPrefab, options[attackOptionNumber].GetLaserGunPos(), Quaternion.Euler(Vector3.zero));
                newLaser.gameObject.SetActive(true);
                newLaser.SetLaserInfo(LaserVer2.LaserType.CircleLaser);


                SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundCircleLaser);

                pattern = Pattern.PatternStop;
                StartCoroutine(ActiveCoolDown());
                break;

            case Pattern.ReflectLaser:
                break;

            case Pattern.LaserNo5:
                LaserNo5ShotEa *= PublicValueStorage.Instance.GetAddSpeedRivisionValue();
                StartCoroutine(LaunchLaserNo5Attack());
                pattern = Pattern.PatternStop;
                break;

            default:
                //Debug.LogError(type + " is out of index in " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                pattern = Pattern.PatternStop;
                break;
        }
    }

    private float LaserNo5ShotTimer = 0;
    private float LaserNo5Delay = 0.2f;
    IEnumerator LaunchLaserNo5Attack()
    {
        while (true)
        {
            LaserNo5ShotTimer += Time.deltaTime;

            if (LaserNo5Delay <= LaserNo5ShotTimer)
            {
                attackOptionNumber = PickOption();

                if (attackOptionNumber == -1 || LaserNo5ShotFired >= LaserNo5ShotEa)
                {
                    //Debug.Log("LaserNo5 End : " + LaserNo5ShotFired);

                    LaserNo5ShotFired = 0;
                    LaserNo5ShotTimer = 0;
                    pattern = Pattern.PatternStop;

                    foreach (LaserNo5OptionVer2 option in options)
                    {
                        option.Reset();
                    }

                    StopCoroutine(LaunchLaserNo5Attack());
                    StartCoroutine(ActiveCoolDown());
                    yield break;
                }

                // 그냥한글로할랜다 귀찮다
                // 옵션의 상태가 초기화(-1) 이거나 샷이 끝난 상태 (3>) 일때
                // 새로운 레이저 발사
                if (options[attackOptionNumber].shotTimer == -1 || options[attackOptionNumber].shotTimer > LaserNo5Delay)
                {

                    LaserVer2 newLaser;

                    newLaser = Instantiate(laserPrefab, options[attackOptionNumber].GetLaserGunPos(), Quaternion.Euler(Vector3.zero));
                    newLaser.gameObject.SetActive(true);

                    if (newLaser != null && playerSpriteRenderer != null)
                    {
                        LaserNo5Delay =
                            newLaser.SetLaserInfo(LaserVer2.LaserType.LaserNo5, 5,
                                                  playerSpriteRenderer.bounds.extents.x);

                        if (LaserNo5Delay != -1)
                        {


                            options[attackOptionNumber].StartShotTimer(LaserNo5Delay);
                            LaserNo5ShotTimer = 0;
                            LaserNo5ShotFired++;

                            SoundManager.Instance.ShortSpeaker(SoundManager.Speaker.Center, soundLaserNo5);
                        }
                    }
                    else
                    {
                        Destroy(newLaser.gameObject);
                    }
                }
            }            
            yield return null;
        }
    }

    IEnumerator ActiveCoolDown()
    {
        while(true)
        {
            if (cooldownTimer >= cooldownDelay)
            {
                cooldownTimer = 0;
                pattern = Pattern.Idle;
                StopCoroutine(ActiveCoolDown());
                yield break;
            }
            cooldownTimer += Time.deltaTime;
            yield return null;
        }
    }


    private int PickOption()
    {
        RefreshOptionState();
        List<int> AliveOptions = new List<int>();

        //for (int i = 0; i < deadOptionIndex.Count; i++)
        //{
        //    Debug.Log("deadoptions : " + deadOptionIndex[i]);
        //}

        for (int i = 0; i < options.Count; i++)
        {
            // If option state was alive
            // Remember, When add options, It must not be dead state.
            if (deadOptionIndex.Exists(index => index == i) == false)
            {
                AliveOptions.Add(i);
                //Debug.Log("insert : " + AliveOptions[AliveOptions.Count - 1]);
            }
        }
        
        //for(int i = 0; i < AliveOptions.Count; i++)
        //{
        //    Debug.Log("options : " + AliveOptions[i]);
        //}

        if (AliveOptions.Count == 0)
        {
            return -1;
        }
        else
        {
            return AliveOptions[Random.Range(0, AliveOptions.Count)];
        }
    }

    /// <summary>
    /// true = All options are alive
    /// </summary>
    /// <returns></returns>
    private void RefreshOptionState()
    {
        deadOptionIndex = new List<int>();
        for (int i = 0; i < options.Count; i++)
        {
            // Option was dead
            if (options[i].gameObject.activeSelf == false)
            {
                // When prev attacker option is dead, attackPrevOptionNumber set null 
                if (i == attackPrevOptionNumber)
                    attackPrevOptionNumber = -1;

                deadOptionIndex.Add(i);
                //options[i].gameObject.SetActive(true);

                //Debug.Log("Dead option number : " + i);
            }
        }
    }

    // 디버그 후 정식 사용할지 차일 결정
    private int CheckHowManyOptionsDead()
    {
        //    // All options are alive
        //    if (deadOptionIndex.Count == 0) return true;
        //    else return false;

        int count = 0;
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i].gameObject.activeSelf == false)
                count++;
        }
        //Debug.Log("COUNT : " + (count));

        callCount++;
        //Debug.Log("CallCount : " + callCount);
        return count;
    }

    public void OptionExplosion(int optionIndex)
    {
        if (this.gameObject != null)
            Instantiate(missileExplosion.gameObject, optionPositions[optionIndex], Quaternion.identity).gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (PublicValueStorage.Instance == null) return;

        PublicValueStorage.Instance.SetBossTransform(null);
    }
}
