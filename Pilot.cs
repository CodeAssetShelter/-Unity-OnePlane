using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pilot : MonoBehaviour {

    public GameObject player;
    [HideInInspector]
    public bool isShieldActive = false;

    private ControllerPlayer playerController;
    private int instanceID = 0;

    private void Start()
    {
        playerController = player.GetComponent<ControllerPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Missile":
                //if (isShieldActive == false)
                //{
                //    playerController.ActivatePlayerDie(0);
                //}
                break;

            case "LineFallNormalMissile":
                //if (isShieldActive == false)
                //{
                //    playerController.ActivatePlayerDie(1);
                //}

                break;

            case "LineFallShieldBreaker":
                if (Random.Range(0, 100) < 5) playerController.ActivateGetItem(6);
                break;

            case "LineFallSlowRailGun":
                if (isShieldActive == false)
                {
                    playerController.ActivatePlayerDie(1);
                }
                break;

            case "Item":
                // 문자열 부분 해결하기
                int temp;
                int.TryParse(collision.name.Substring(0, 1), out temp);
                playerController.ActivateGetItem(temp);
                break;

            default:
                //Debug.Log("There is nothing");
                break;
        }
    }
}
