using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public GameObject parent;

    private ControllerEnemy parentParam;

    // 190207 LifeBalance
    // for game balance, when enemy appear in camera, Collider is online
    public BoxCollider2D enemyCollider;

    private void OnEnable()
    {
        enemyCollider.enabled = false;

        GameManager.onPlayerDie += OnPlayerDie;
        GameManager.onDestroyAllEnemy += OnPlayerDie;
        //GameManager.onDeadByItemBomb += DeadByItemBomb;
        parentParam = parent.GetComponent<ControllerEnemy>();
    }

    private void OnBecameVisible()
    {
        enemyCollider.enabled = true;
        //GameManager.Instance.enemyPos.Add(parent.gameObject);
        PublicValueStorage.Instance.AddEnemyPos(parent);
        //parentParam.CallbackAddEnemyPos(parent);
        GameManager.onDeadByItemBomb += DeadByItemBomb;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerMissile")
        {
            //enemyCollider.enabled = false;
            //parent.SetActive(false);
            //parentParam.CallbackEnemyDie(parent);
            Destroy(parent);
        }
    }

    public void OnPlayerDie()
    {
        //Debug.Log("Enemy Pilot Destroy to " + this.transform.position);
        Destroy(parent);
    }

    public void DeadByItemBomb()
    {
        //Destroy(parent);
        //Debug.Log("name : " + parent.name);
        //parentParam.CallbackDeadByItemBomb(parent);
        parentParam.CallbackEnemyDie(parent);
    }

    private void OnDisable()
    {
        parentParam.CallbackEnemyDie(parent);
        GameManager.onPlayerDie -= OnPlayerDie;
        GameManager.onDestroyAllEnemy -= OnPlayerDie;
        GameManager.onDeadByItemBomb -= DeadByItemBomb;
    }
}
