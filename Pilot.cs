using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pilot : MonoBehaviour {

    public GameObject player;
    private ControllerPlayer playerController;

    private void Start()
    {
        playerController = player.GetComponent<ControllerPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Recognized : " + collision.tag);
        switch (collision.tag)
        {
            case "Missile":
                playerController.ActivatePlayerDie();
                break;

            case "LineFallNormalMissile":
                playerController.ActivatePlayerDie(1);
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
