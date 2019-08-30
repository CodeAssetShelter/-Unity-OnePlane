using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour {

    // Class

    // 190128 LifeBalance
    // Add class variable itemType
    public enum ItemType
    {
        Failed = 0, GetCredit, ShieldRecovery, PowerBooster, BigShield, ScoreBooster, ShieldFullCharge, Bomb
    }

    [System.Serializable]
    public class ItemOptions
    {
        public string name;
        public ItemType typeEnum;
        public Sprite sprite;
        public float duration;
        public float magnification = 1.0f;
        [Range(0, 1000)]
        public int dropChance;

        public float activeTimer = 0;
        public bool active = false;
    }

    [System.Serializable]
    public class ItemSlotBox
    {
        public bool itemSlotBoxBeSet;
        public GameObject itemSlotBox;
        public ItemButton itemSlotButton;
        public ItemType itemType;
        public int itemOptionNum;
        //public Image itemSprite;

        public ItemSlotBox(GameObject buttonPrefab, GameObject itemSlot, int index)
        {
            itemSlotBoxBeSet = false;
            itemSlotBox = Instantiate(buttonPrefab, itemSlot.gameObject.transform);
            itemSlotBox.name = index.ToString();
            itemSlotButton = itemSlotBox.GetComponent<ItemButton>();
            itemSlotButton.SetButtonNumber(index);
            //itemSprite = itemSlotBox.transform.GetChild(0).GetComponent<Image>();
        }
    }

    [System.Serializable]
    public class enemyAreaBox
    {
        public Vector2 bottomLeft;
        public Vector2 upRight;
    }

    public class ActivateItemList
    {
        public bool ShieldRecovery = false;
        public float ShieldRecoveryTimer = 0;

        public bool PowerBooster = false;
        public float PowerBoosterTimer = 0;

        public bool BigShield = false;
        public float BigShieldTimer = 0;

        public bool ScoreBooster = false;
        public float ScoreBoosterTimer = 0;

        public bool ShieldFullCharge = false;
        public float ShieldFullChargeTimer = 0;

        public bool Bomb = false;
        public float BombTimer = 0;
    }

    // Default Values
    private float timerZero = 0;

    float screenHeight;
    float screenWidth;
    //float[] screenSize = new float[2];

    // EventHandlers
    public delegate void GameHandler();
    public static event GameHandler onPlayerDie;
    public static event GameHandler onDestroyAllEnemy;
    public static event GameHandler onDeadByItemBomb;
    public static event GameHandler onDestroyAllObject;


    public delegate void PVSHandler();
    //public static event PVSHandler onAddScore;
    //public static event PVSHandler onAddEnemyPos;

    public delegate int IntHandler(int index);
    public static event IntHandler onGetBossHp;
    //public static event ButtonHandler UseItemButton;


    public delegate void BossHandler(GameObject target);
    public static event BossHandler onBossDie;







    // Level system
    enum GameState {Lobby = 0, Play, BossSpawn, BossPlay, LevelChange, Respawning, GameOver, GameStateCount}
    GameState gameState;

    private int live = 3; // no use

    public int level = 1;
    public int levelWithEnemyEa = 25;
    public int levelWithDiedEnemy = 0;


    // Position Values
    [Header("Positions & Spawns")]
    public GameObject playerPosition;
    public float playerSpawnAnimSpeed = 2.0f;

    // debug options
    [Header("Debug Switch")]
    public bool isDebug = false;
    public bool debugNotCreateEnemy = false;
    public bool debugNotCreatePlayer = false;
    public ItemType debugAlwaysCreateThisItem;


    // Game Objects
    [Header("Don't Add Hierarchy Objects")]
    public GameObject playerPrefab;
    private GameObject player;
    private Vector2 playerPos;
    public GameObject enemyPrefab;
    public GameObject item;

    [Header("Boss Prefabs")]
    public CreateBossModule createBossModule;


    // Game Balance Values
    // Stage Values
    [Header("Game Balance Values")]
    private int stageLevel = 0;
    private int createEnemies;
    private int createdEnemies = 0;
    public int createEnemiesMin = 1;
    public int createEnemiesMax = 10;
    private int remainEnemies;
    private int credit = 5000;

    [Header("! ! ! BOSS VALUES ! ! !")]
    public float bossHp = 50f;
    private bool isBossAlive = false;


    // 190207 LifeBalance
    // 아 잘놀았다.
    // Enemy spawn area variable
    [Tooltip("Enemy spawn area")]    
    public enemyAreaBox enemySpawnBox;
    [Tooltip("Enemy battle area")]
    public enemyAreaBox enemyBattleBox;
    public Vector2 enemyBoxScaleStart = Vector2.zero;
    public Vector2 enemyBoxScaleEnd = Vector2.zero;

    enum EnemySpawnArea { Left = 0, LeftUp, Up, RightUp, Right, enemySpawnAreaCount }

    [SerializeField]
    private float enemySpawnTimer = 0;

    [Range(0.0f, 500.0f)]
    [Tooltip("Spawn delay")]
    public float enemySpawnTerm = 2.0f;

    [SerializeField]
    private int enemyCurrentSpawned = 0;
    [Range(0, 100)]
    public int enemyMaxSpawn = 10;

    [Header("Set item drop chance value")]
    //public int[] items;

    // This value is used in ControllerEnemy.cs, also.
    public List<GameObject> enemyPos = new List<GameObject>();


    // Game UI Objects
    [Header("Game UI")]
    public Text scoreboard;
    private int score = 0;
    public GameObject gamePlayUI;
    public Text creditboardInGame;
    public Text creditboardInUI;

    // Game Menu UI Objects
    [Header("Menu UI")]
    public GameObject playerSlot;
    public Lobby Lobby;
    public Planes planes;
    public Text bestScoreBoard;
    private int bestScore = 0;


    [Header("Item UI")]
    public Sprite itemUIMask;
    //public Sprite[] itemSprites;
    public ItemOptions[] itemOption;
    public GameObject itemSlot;
    public GameObject buttonPrefab;
    private int itemAllChanceValue = 0;
    //public Sprite[] itemImage;


    [Range(0, 5)]
    public int itemSlotEa = 0;
    public ItemSlotBox[] itemSlotBox;

    //In Game System values for save (IGS) 
    [Header("Score Values")]
    public int scoreMissile;
    public int scoreEnemy;
    private int igsScoreMultifly = 1;
    private ControllerPlayer playerScript;

    [Header("Item Values")]
    public float itemShieldRecovery;



    // Use this for initialization
    void Awake()
    {

        //
        //
        //
        // Set ScreenSize
        //
        //
        //

        Screen.SetResolution(720, 720 * (9 / 16), false);

        //
        //
        //
        //
        // Game Init Area
        //
        //
        //
        //
        //
        //

        player = Instantiate(playerPrefab, playerPrefab.transform.position, Quaternion.Euler(0, 0, 0));
        playerScript = player.GetComponent<ControllerPlayer>();
        playerScript.SetCallbacks(CBPlayerDie, CBGetItem, CBChangeGameStart);
        //playerScript.SetCallback(0, CBPlayerDie);
        //playerScript.SetCallback(1, CBGetItem);
        //player.GetComponent<ControllerPlayer>().SetCallback(0, PlayerDie);
        playerPos = player.transform.position;


        PublicValueStorage.Instance.SetPlayerConstPosition(playerPosition);
        PublicValueStorage.Instance.SetValues(player);


        if (debugNotCreatePlayer == true)
        {
            player.SetActive(false);
        }



        PublicValueStorage.Instance.SetCallbacks(CBMissileScoreAdd, CBAddEnemyPosition, CBUseItem, CBGetRandomEnemyPos, RefreshCreditPVC, CBGetBestScore, CBBossDie);

        onBossDie += CBonBossDie;

        screenHeight = 2 * Camera.main.orthographicSize;
        screenWidth = screenHeight * Camera.main.aspect;
        //screenSize[1] = screenWidth;
        //screenSize[0] = screenHeight;

        PublicValueStorage.Instance.SetScreenSize(screenHeight, screenWidth);


        Debug.Log("height : " + screenHeight);
        Debug.Log("width : " + screenWidth);


        // 190207 LifeBalance
        // Set All Spawn And Battle Area
        enemySpawnBox.bottomLeft.x = screenWidth * enemyBoxScaleStart.x;
        enemySpawnBox.bottomLeft.y = screenHeight * enemyBoxScaleStart.y;
        enemySpawnBox.upRight.x = screenWidth * enemyBoxScaleEnd.x;
        enemySpawnBox.upRight.y = screenHeight * enemyBoxScaleEnd.y;

        enemyBattleBox.bottomLeft.x = screenWidth * enemyBoxScaleStart.x;
        enemyBattleBox.bottomLeft.y = screenHeight * enemyBoxScaleStart.y;
        enemyBattleBox.upRight.x = screenWidth * enemyBoxScaleEnd.x;
        enemyBattleBox.upRight.y = screenHeight * enemyBoxScaleEnd.y;

        ///////////////////////////////////////////
        //////////////////////////////////////////
        ///////// 1월 16일 여기까지 했음 //////////
        ///////////////////////////////////////////
        // notice
        // Delegate Event 로 게임 매니저와 탄, 적들을 묶음
        // Delegate Enete 로 게임 매니저와 플레이어를 묶어 게임 오버 신호 가도록 설정함

        //////////////////////
        // 1월 18일 할일
        // 인스턴스로 만든 경우 - 게이지 작동안함, 콜백 작동함
        // 인스턴스로 안만든 경우 - 게이지 작동함, 콜백 작동안함
        // 해결할 것
        // 1월 19일 해결

        // 190125 LifeBalance
        // Set Item Box
        // 190131
        // Add Class

        itemSlotBox = new ItemSlotBox[itemSlotEa];

        for (int i = 0; i < itemSlotBox.Length; i++)
        {
            itemSlotBox[i] = new ItemSlotBox(buttonPrefab, itemSlot, i);
        }

        foreach(ItemOptions value in itemOption)
        {
            itemAllChanceValue += value.dropChance;
        }

        scoreboard.text = "" + score;

        createEnemies = remainEnemies = Random.Range(createEnemiesMin, createEnemiesMax);

        ChangeGameState(GameState.Lobby);


        //
        //
        //
        //
        // Lobby Area
        //
        //
        //
        //
        //

        gamePlayUI.SetActive(false);
        Lobby.SetCallbacks(CBGameStart, null, CBDeactiveShop, null);

        //player.transform.SetParent(playerSlot.transform);
        player.transform.position = playerSlot.transform.position;

        bestScoreBoard.text = "" + bestScore;
        creditboardInUI.text = "" + credit;

        if (File.Exists(Application.persistentDataPath + "/OnePlane.dat") == true)
        {
            Debug.Log(Application.persistentDataPath);
            SaveData.Instance.LoadUserData();
            credit = SaveData.Instance.LoadCredit();
            bestScore = SaveData.Instance.LoadBestScore();
            bestScoreBoard.text = "" + bestScore;
            creditboardInUI.text = "" + SaveData.Instance.LoadCredit();
            //Debug.Log(SaveData.Instance.LoadCredit());
            //Debug.Log(SaveData.Instance.LoadBestScore());
        }
        else
        {
            Debug.Log("There is no savedata");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (gameState == GameState.Lobby)
        {
            //player.transform.SetParent(playerSlot.transform);
            //ChangeGameState(GameState.Play);
        }
        if (gameState == GameState.Play)
        {

            // Debug State
            if (isDebug == true)
            {
                if (player.gameObject != null)
                {
                    if (debugNotCreateEnemy == false)
                        CreateEnemy();
                    DetectItem();
                }
            }
            // Normal State
            else
            {
                if (player.gameObject != null)
                {
                    CreateEnemy();
                    DetectItem();
                }
            }
        }
        if (gameState == GameState.BossSpawn)
        {
            if (isBossAlive == false)
            {
                CreateBoss();
            }
            ChangeGameState(GameState.BossPlay);
        }
        if (gameState == GameState.BossPlay)
        {
            DetectItem();
        }
        if (gameState == GameState.LevelChange)
        {
            level++;
            Debug.Log("Now Game Level is : lv." + level);
            ChangeGameState(GameState.Play);
            LevelUpData();
        }
    }

    private void LevelUpData()
    {
        levelWithEnemyEa += level * 2;
        levelWithDiedEnemy = 0;
        createEnemiesMin = 1;
        createdEnemies = 0;

        if (enemyMaxSpawn < 30)
            enemyMaxSpawn += level;

        bossHp += (level * 2) + enemyMaxSpawn;
        PublicValueStorage.Instance.SetAddSpeedRivisionValue(((float)level * 0.1f));
    }


    private void DetectItem()
    {
        for(int i=2; i<itemOption.Length; i++)
        {
            if (itemOption[i].active == true)
            {
                // Activating Item...
                if (itemOption[i].duration >= itemOption[i].activeTimer)
                {
                    itemOption[i].activeTimer += Time.deltaTime;
                }
                // Deactivated Item... Timer Out
                else
                {
                    switch (itemOption[i].typeEnum)
                    {
                        case ItemType.ScoreBooster:
                            igsScoreMultifly = 1;
                            break;

                        case ItemType.BigShield:
                            playerScript.OP_BigShield(playerScript.op_shieldOriginalSize);
                            break;

                        case ItemType.ShieldFullCharge:
                            playerScript.OP_ShieldFullCharge(false);
                            playerScript.OP_BigShield(playerScript.op_shieldOriginalSize);
                            break;
                    }
                    itemOption[i].active = false;
                    itemOption[i].activeTimer = 0;
                }
            }
        }
    }

    private void CreateEnemy()
    {
        // 190121 플레이어 포지션을 계속해서 받는다.
        //playerPos = player.transform.position;


        if (Input.GetKeyDown(KeyCode.Q))
        {
            onPlayerDie();
        }

        // 190117 LifeBalance
        // Enemy Spawn
        // 190207 LifeBalance
        // Add enemy random spawn animation, random spawn area. 
        CreateEnemies();
    }

    private void CreateBoss()
    {
        //Debug.Log("CB");
        isBossAlive = true;

        createBossModule.CreateBoss(screenWidth, screenHeight, player, bossHp, CBBossDie);

        //GameObject tempBoss = Instantiate(bossPrefabs[bossIndex], new Vector3(0, screenHeight * 0.75f, 0), Quaternion.Euler(0, 0, 0));
        //ControllerBOSS tempBossScript;

        //PublicValueStorage.Instance.SetBossHp(bossHp);

        //tempBossScript = tempBoss.GetComponent<ControllerBOSS>();

        //tempBossScript.bossMissileModule.InitBossMissileModule(screenHeight, screenWidth, playerPos);
        //tempBossScript.SetBossData(bossHp);
        //tempBossScript.SetCallback(CBBossDie);

        //enemyCurrentSpawned++;
    }



    /////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////
    /////////////////////  LEVEL SYSTEM /////////////////////
    /////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////

    private void DeadEnemyCountUp()
    {
        levelWithDiedEnemy++;        
        // change state to BossSpawn
        if (levelWithDiedEnemy >= levelWithEnemyEa)
        {
            if (isBossAlive == false)
            {
                ChangeGameState(GameState.BossSpawn);
            }
        }
    }

    private void ChangeGameState(GameState state)
    {
        gameState = state;
    }



    /////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////
    /////////////////////   CALLBACKS   /////////////////////
    /////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////

    private void CBPlayerDie(int index)
    {
        //Debug.Log("Player Died");
        onPlayerDie();
        enemyCurrentSpawned = 0;
        ChangeGameState(GameState.GameOver);
        ResetGame();
        //onPlayerDie();
    }


    private void CBDestroyAllEnemy()
    {
        onDestroyAllEnemy();
    }

    private void CBUseItemBomb()
    {
        onDeadByItemBomb();
        DeadEnemyCountUp();
    }

    private void CBDeadByItemBomb(GameObject target)
    {
        
        enemyPos.Remove(target);
        enemyCurrentSpawned--;
        DropItem(target.transform.position);
        ScoreAdd("Bomb");

        Destroy(target);
    }


    private void CBonBossDie(GameObject target)
    {
        CBBossDie(target);

    }
    private void CBBossDie(GameObject target)
    {
        Debug.Log("GM BossDie called by ControllerBoss.cs");
        enemyPos.Remove(target);
        enemyCurrentSpawned = 0;

        isBossAlive = false;

        Destroy(target);

        ChangeGameState(GameState.LevelChange);
    }

    private void CBChangeGameStart(int noUse = 0)
    {
        ChangeGameState(GameState.Play);
    }

    // 190117 LifeBalance
    // When Enemy Die, Drop Item and Add Score
    private void CBEnemyDie(GameObject target)
    {
        Debug.Log("Remain : " + levelWithDiedEnemy + " // Time : " + System.DateTime.Now);
        enemyPos.Remove(target);
        Destroy(target);
        enemyCurrentSpawned--;
        DropItem(target.transform.position);

        DeadEnemyCountUp();

        ScoreAdd("Enemy");
    }


    private void CBGameStart()
    {
        //ChangeGameState(GameState.Play);
        InitGamePlay();
        Lobby.gameObject.SetActive(false);
        gamePlayUI.SetActive(true);
        PublicValueStorage.Instance.SetBossHpBar(GameObject.Find("Boss Shield Gauge").GetComponent<Slider>());
    }


    private void CBMissileScoreAdd(GameObject target = null)
    {
        ScoreAdd("Missile");
    }

    private void CBAddEnemyPosition(GameObject target = null)
    {
        if (target == null) return;

        enemyPos.Add(target);
    }


    private void CBUseItem(int index)
    {
        UseItem(index);
    }

    private Vector2 CBGetRandomEnemyPos(Vector2 direction)
    {
        return GetRandomEnemyPos(direction);
    }

    private int CBGetBestScore(int value = 0)
    {
        return bestScore;
    }


    // 190126 LifeBalance
    // When Player get item, GameManager recognize item type
    private void CBGetItem(int index)
    {
        //Debug.Log("Get Item : " + index);
        for (int i = 0; i < itemOption.Length; i++)
        {
            if (i == index)
            {
                SetItem(i);
                return;
                //Debug.Log("Item " + index);
            }
        }
    }


    private void CBDeactiveShop()
    {
        //Debug.Log(planes.selectedIndex);
        playerScript.SetPlayerSprite(planes.planeSprite[planes.selectedIndex]);
    }




    /////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////
    /////////////////////    METHOD     /////////////////////
    /////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////

    private void CreateEnemies()
    {
        // 190318 LifeBalance
        // No more need to create enemy
        // Switch on Boss create (not completed)
        if (levelWithDiedEnemy >= levelWithEnemyEa || createdEnemies >= levelWithEnemyEa)
        {
            Debug.Log(levelWithDiedEnemy + ">" + levelWithEnemyEa + "||" + createdEnemies + ">=" + levelWithEnemyEa);
            //Debug.Log("Can't Create Enemy!");
            return;
        }
        //if (createdEnemies >= createEnemies)
        //{
        //    return;
        //}

        enemySpawnTimer += Time.deltaTime;
        if (enemySpawnTimer >= enemySpawnTerm && enemyCurrentSpawned <= enemyMaxSpawn)
        {
            if (levelWithDiedEnemy >= levelWithEnemyEa || createdEnemies >= levelWithEnemyEa)
            {
                Debug.Log(levelWithDiedEnemy + ">" + levelWithEnemyEa + "||" + createdEnemies + ">=" + levelWithEnemyEa);
                Debug.Log("Can't Create Enemy in Spawn Code Area!");
                return;
            }

            // 190207 LifeBalance
            // Set spawn area by random
            Vector2 spawnAxis = Vector2.zero;
            int spawnArea = Random.Range(0, (int)EnemySpawnArea.enemySpawnAreaCount);
            switch (spawnArea)
            {
                case (int)EnemySpawnArea.Left:
                    spawnAxis.x = -screenWidth;
                    spawnAxis.y = 0;
                    //Debug.Log(EnemySpawnArea.Left);
                    break;
                case (int)EnemySpawnArea.LeftUp:
                    spawnAxis.x = -screenWidth;
                    spawnAxis.y = screenHeight;
                    //Debug.Log(EnemySpawnArea.LeftUp);
                    break;
                case (int)EnemySpawnArea.Up:
                    spawnAxis.x = 0;
                    spawnAxis.y = screenHeight;
                    //Debug.Log(EnemySpawnArea.Up);
                    break;
                case (int)EnemySpawnArea.RightUp:
                    spawnAxis.x = screenWidth;
                    spawnAxis.y = screenHeight;
                    //Debug.Log(EnemySpawnArea.RightUp);
                    break;
                case (int)EnemySpawnArea.Right:
                    spawnAxis.x = screenWidth;
                    spawnAxis.y = 0;
                    //Debug.Log(EnemySpawnArea.Right);
                    break;

                default:
                    Debug.Log("GameManager EnemySpawn Error");
                    break;
            }

            GameObject temp =
                Instantiate(enemyPrefab,
                new Vector3(Random.Range(enemySpawnBox.bottomLeft.x + spawnAxis.x, enemySpawnBox.upRight.x + spawnAxis.x),
                            Random.Range(enemySpawnBox.bottomLeft.y + spawnAxis.y, enemySpawnBox.upRight.y + spawnAxis.y),
                            0),
                Quaternion.Euler(Vector3.zero));
            var temp2 = temp.GetComponent<ControllerEnemy>();
            temp2.SetSystemValue(spawnArea, enemyBattleBox.bottomLeft, enemyBattleBox.upRight);
            //temp2.SetCallbackEnemyDie(EnemyDie);
            //temp2.SetCallbackDeadByBomb(EnemyDie);
            temp2.SetCallbacks(CBEnemyDie, CBEnemyDie, CBMissileScoreAdd, CBAddEnemyPosition);
            //enemyPos.Add(temp);

            enemyCurrentSpawned++;
            createdEnemies++;
            Debug.Log("enemyCurrentSpawned : " + enemyCurrentSpawned + "  createdEnemies : " + createdEnemies);
            enemySpawnTimer = timerZero;
        }
    }


    // 190128 LifeBalance
    // When this method called by GetItem, SetItem set item to itemSlotBox
    private void SetItem(int itemIndex)
    {

        if (itemOption[itemIndex + 1].typeEnum == ItemType.GetCredit)
        {
            ActivateItem(itemOption[itemIndex + 1]);
            return;
        }
        // Check itemSlot Empty or Set
        for(int i = 0; i < itemSlotBox.Length; i++)
        {
            // itemSlotBox is empty
            // -> Set Item!
            if (itemSlotBox[i].itemSlotBoxBeSet == false)
            {
                //Debug.Log("Set Item Name : " + itemOption[itemIndex + 1].itemTypeEnum);
                //Debug.Log("Set Item Slot : " + i);

                // itemOptionNum have itemOption index infomation on hierarchy
                itemSlotBox[i].itemOptionNum = itemIndex;
                itemSlotBox[i].itemType = itemOption[itemIndex + 1].typeEnum;
                itemSlotBox[i].itemSlotBoxBeSet = true;
                itemSlotBox[i].itemSlotButton.SetButtonImage(itemOption[itemIndex + 1].sprite);
                return;
            }
        }
    }

    // 190131 LifeBalance
    // When Use Item signal from ItemButton.cs
    public void UseItem(int index)
    {
        if (itemSlotBox[index].itemSlotBoxBeSet == false)
        {
            //Debug.Log("Slot : " + index + " is empty");
            return;
        }
        else
        {
            //Debug.Log("Use Item Slot " + index);
            //Debug.Log("Item Name : " + itemOption[itemSlotBox[index].itemOptionNum + 1].itemTypeEnum);
            itemSlotBox[index].itemSlotBoxBeSet = false;
            itemSlotBox[index].itemSlotButton.SetButtonImage(itemUIMask);
            ActivateItem(itemOption[itemSlotBox[index].itemOptionNum + 1]);
        }
    }

    // 190208 LifeBalance
    // Activate Item by UseItem method
    private void ActivateItem(ItemOptions itemOptionInfo)
    {
        //Debug.Log("Activate " + itemOptionIndex);
        //ItemType itemtype = itemOption[itemOptionNum].itemTypeEnum;
        

        switch (itemOptionInfo.typeEnum)
        {
            case ItemType.GetCredit:
                // 크레딧 UI 를 따로 제작할 것
                RefreshCredit((int)itemOptionInfo.duration);
                //Debug.Log(itemOptionInfo.typeEnum + " : " + itemOptionInfo.active);
                break;

            case ItemType.ScoreBooster:
                itemOptionInfo.active = true;
                itemOptionInfo.activeTimer = 0;
                igsScoreMultifly = (int)itemOptionInfo.magnification;
                //Debug.Log(itemOptionInfo.typeEnum + " : " + itemOptionInfo.active);
                break;

            case ItemType.BigShield:
                itemOptionInfo.active = true;
                itemOptionInfo.activeTimer = 0;
                playerScript.OP_BigShield(itemOptionInfo.magnification * playerScript.op_shieldOriginalSize);
                //Debug.Log(itemOptionInfo.typeEnum + " : " + itemOptionInfo.active);
                break;

            case ItemType.ShieldFullCharge:
                itemOptionInfo.active = true;
                itemOptionInfo.activeTimer = 0;
                playerScript.OP_ShieldFullCharge(true);
                playerScript.OP_BigShield(itemOptionInfo.magnification * playerScript.op_shieldOriginalSize);
                //Debug.Log(itemOptionInfo.typeEnum + " : " + itemOptionInfo.active);
                break;

            case ItemType.ShieldRecovery:
                itemOptionInfo.active = true;
                itemOptionInfo.activeTimer = 0;
                playerScript.OP_ShieldRecovery(itemOptionInfo.duration);
                //Debug.Log(itemOptionInfo.typeEnum + " : " + itemOptionInfo.active);
                break;

            case ItemType.Bomb:
                //Debug.Log("YOU ACTIVATE BOMB ! ! !");
                CBUseItemBomb();
                break;

            case ItemType.PowerBooster:
                itemOptionInfo.active = true;
                itemOptionInfo.activeTimer = 0;
                //Debug.Log(itemOptionInfo.typeEnum + " : " + itemOptionInfo.active);
                break;
        }
    }

    // 190122 LifeBalance
    // When game is over, Reset all parameter (will)
    public void ResetGame()
    {
        player = Instantiate(playerPrefab, playerPrefab.transform.position, Quaternion.Euler(0, 0, 0));
        playerScript = player.GetComponent<ControllerPlayer>();
        playerScript.SetCallbacks(CBPlayerDie, CBGetItem, CBChangeGameStart);
        //playerScript.SetCallback(0, CBPlayerDie);
        //playerScript.SetCallback(1, CBGetItem);
        //player.GetComponent<ControllerPlayer>().SetCallback(0, PlayerDie);
        playerPos = player.transform.position;

        Lobby.SetCallBacksTest(CBGameStart);
        player.transform.position = playerSlot.transform.position;

        PublicValueStorage.Instance.SetValues(player);
        PublicValueStorage.Instance.SetAddSpeedRivisionValue(1.0f);

        gamePlayUI.SetActive(false);
        Lobby.gameObject.SetActive(true);

        createdEnemies = 0;

        enemyPos.Clear();

        CompareBestScore();
        creditboardInUI.text = "" + credit;

        score = 0;
        scoreboard.text = "" + score;

        levelWithDiedEnemy = 0;

        SaveData.Instance.SaveUserData();
    }

    private void CompareBestScore()
    {
        Debug.Log(score + " " + bestScore);
        if (score >= bestScore)
        {
            bestScore = score;
        }

        bestScoreBoard.text = "" + bestScore;
    }

    public Vector2 GetRandomEnemyPos(Vector2 direction)
    {
        if (enemyPos.Count == 0)
            return direction;
        return enemyPos[Random.Range(0, enemyPos.Count)].transform.position;
    }

    // 190121 LifeBalance
    // DropItem according to probability (gasha)

    public void DropItem(Vector2 position)
    {
        // 190227 LifeBalance
        // Debug State
        if (isDebug == true)
        {
            for(int i = 0; i<itemOption.Length; i++)
            {
                if(itemOption[i].typeEnum == debugAlwaysCreateThisItem)
                {
                    //Debug.Log("DROP : " + (i - 1).ToString());
                    item.name = (i - 1).ToString();                    
                    item.GetComponent<SpriteRenderer>().sprite = itemOption[i].sprite;
                    Instantiate(item, position, Quaternion.Euler(Vector2.zero));
                    return;
                }
            }
        }


        // 190128 LifeBalance
        // Change Item droptable algorhythm

        //int itemValue = Random.Range(0, itemOption[itemOption.Length - 1].itemDropChance);
        int itemValue = Random.Range(0, itemAllChanceValue);
        int currValue = 0;
        //Debug.Log(itemValue);
        
        
        // Normal state
        for (int i = 0; i < itemOption.Length; i++)
        {
            currValue += itemOption[i].dropChance;
            if (i == 0)
            {
                if (itemValue < currValue)
                {
                    //Debug.Log("Failed");
                    return;
                }
            }
            // 'i' is bigger than 0 && 'i' is smaller than items.Length - 1
            else
            {
                // If itemValue is between current and next
                if (itemValue > itemOption[i - 1].dropChance && itemValue <= currValue)
                {
                    Instantiate(item, position, Quaternion.Euler(Vector2.zero));
                    item.GetComponent<SpriteRenderer>().sprite = itemOption[i].sprite;
                    item.name = (i-1).ToString();
                    //Debug.Log("Create : " + item.name + "//// itemValue : " + itemValue + "//// currValue : " + currValue);
                    return;
                }
            }
        }
    }

    // 190501 LifeBalance
    // When player touched gameplay button, this wiil be actived
    // Pref : Gamemanager.cs

    private void InitGamePlay()
    {
        //player.transform.SetParent(playerPosition.transform);
        //Debug.Log("1");
        playerScript.SetSpawnAnimation(playerPosition.transform.position, playerSpawnAnimSpeed);
        RefreshCredit(0);
        //Debug.Log("2");
    }

    
    // 190507 LifeBalance
    // Refresh Credit value
    private void RefreshCredit(int value)
    {
        credit += value;
        creditboardInGame.text = "" + credit;
    }
    // 190507 LifeBalance
    // Refresh Credit value to PVC
    private int RefreshCreditPVC(int value)
    {
        RefreshCredit(value);
        //Debug.Log("ship Credit : " + credit);
        return credit;
    }


    // 190121 LifeBalace
    // When Enemy or Missile is destroy, Add score to scoreboard
    // Pref : GameManager.cs, MissileForEnemy.cs

    // 190305 LifeBalance
    // Add Bomb type switch in ScoreAdd()

    /// <summary>
    /// Set ScoreAdd(string kind)
    /// </summary>
    /// <param name="kind">
    /// <para>Enemy : When each enemy died</para>
    /// <para>Missile : When each reflect missile</para>
    /// <para>Bomb : When use item type : Bomb </para>
    /// </param>
    public void ScoreAdd(string kind)
    {
        switch(kind)
        {
            case "Enemy":
                score += (scoreEnemy * igsScoreMultifly);
                scoreboard.text = "" + score;
                break;

            case "Missile":
                score += (scoreMissile * igsScoreMultifly);
                scoreboard.text = "" + score;
                break;

            default:
                Debug.Log("There is abnormal param");
                break;
        }
    }

    // 190121 LifeBalace
    // When other script request player's position, return it
    // Pref MissileForEnemy.cs

    public Vector2 GetPlayerPosition()
    {
        return playerPos;
    }

    // 190313 LifeBalance 
    // Get Screen Size (Don't use)

    //public float[] GetScreenSize()
    //{
    //    return screenSize;
    //}
}
