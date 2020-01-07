using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBossModule : MonoBehaviour
{
    public delegate void ModuleHandler(GameObject target);


    [Header("Boss Prefabs")]
    [Header("1 : LineFall")]
    [Header("0 : ControllerBoss")]
    [SerializeField]
    private GameObject[] bossPrefabs;


    public int alwaysThisBoss = 0;
    public void CreateBoss(float screenWidth, float screenHeight, GameObject player, float bossHp, System.Action<GameObject> BossDie)
    {
        int selectBoss = Random.Range(0, bossPrefabs.Length);

        if (alwaysThisBoss > bossPrefabs.Length - 1)
        {

        }
        else
        {
            selectBoss = alwaysThisBoss;
            //Debug.Log("Boss");
        }

        switch (selectBoss)
        {
            case 0: // Normal Boss (ControllerBoss.cs)
                GameObject tempBossOne = Instantiate(bossPrefabs[selectBoss], new Vector3(0, screenHeight * 0.75f, 0), Quaternion.Euler(0, 0, 0));
                ControllerBOSS controllerBOSS;

                PublicValueStorage.Instance.SetBossHp(bossHp);

                controllerBOSS = tempBossOne.GetComponent<ControllerBOSS>();

                controllerBOSS.bossMissileModule.InitBossMissileModule(screenHeight, screenWidth, player.transform.position);
                controllerBOSS.SetBossData(bossHp);
                controllerBOSS.SetCallback(BossDie.Invoke);

                SoundManager.Instance.BgmSpeaker
                    (SoundManager.BGM.Boss, SoundManager.State.Play, 
                     controllerBOSS.bgmClip);
                break;

            case 1: // LineFall (ControllerLineFall.cs)
                GameObject tempBossTwo = Instantiate(bossPrefabs[selectBoss], new Vector3(0, screenHeight * 0.75f, 0), Quaternion.Euler(0, 0, 0));
                ControllerLineFall controllerLineFall;

                PublicValueStorage.Instance.SetBossHp(bossHp);

                controllerLineFall = tempBossTwo.GetComponent<ControllerLineFall>();

                controllerLineFall.bossMissileModule.InitBossMissileModule(screenHeight, screenWidth, player);
                controllerLineFall.SetBossData(bossHp);
                controllerLineFall.SetCallback(BossDie.Invoke);

                SoundManager.Instance.BgmSpeaker
                    (SoundManager.BGM.Boss, SoundManager.State.Play,
                    controllerLineFall.bgmClip);
                break;

            case 2: // Rope Ladder Illuminator (ControllerRLI.cs)
                GameObject tempBossThree = Instantiate(bossPrefabs[selectBoss], new Vector3(0, screenHeight * 0.75f, 0), Quaternion.Euler(0, 0, 0));
                ControllerRLI controllerRLI;

                PublicValueStorage.Instance.SetBossHp(bossHp);

                controllerRLI = tempBossThree.GetComponent<ControllerRLI>();

                controllerRLI.bossMissileModule.InitBossMissileModule(screenHeight, screenWidth, player);
                controllerRLI.SetBossData(bossHp);
                controllerRLI.SetCallback(BossDie.Invoke);

                SoundManager.Instance.BgmSpeaker
                    (SoundManager.BGM.Boss, SoundManager.State.Play,
                    controllerRLI.bgmClip);
                break;

            case 3:
                GameObject tempBossFour = Instantiate(bossPrefabs[selectBoss], new Vector3(0, screenHeight * 0.75f, 0), Quaternion.Euler(0, 0, 0));
                tempBossFour.SetActive(true);
                PublicValueStorage.Instance.SetBossHp(bossHp);
                break;

            default:
                Debug.LogError("Failed Create Boss");
                break;
        }

    }
}
