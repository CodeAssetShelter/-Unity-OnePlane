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

    public void CreateBoss(float screenWidth, float screenHeight, GameObject player, float bossHp, System.Action<GameObject> BossDie)
    {
        int selectBoss = Random.Range(0, bossPrefabs.Length);
        selectBoss = 1;
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

                break;

            case 1: // LineFall (ControllerLineFall.cs)
                GameObject tempBossTwo = Instantiate(bossPrefabs[selectBoss], new Vector3(0, screenHeight * 0.75f, 0), Quaternion.Euler(0, 0, 0));
                ControllerLineFall controllerLineFall;

                PublicValueStorage.Instance.SetBossHp(bossHp);

                controllerLineFall = tempBossTwo.GetComponent<ControllerLineFall>();

                controllerLineFall.bossMissileModule.InitBossMissileModule(screenHeight, screenWidth, player);
                controllerLineFall.SetBossData(bossHp);
                controllerLineFall.SetCallback(BossDie.Invoke);
                break;

            default:
                Debug.LogError("Failed Create Boss");
                break;
        }

    }
}
