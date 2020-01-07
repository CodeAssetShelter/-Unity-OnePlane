using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public GameObject parent;
    public Animator enemyExplosion;
    private ControllerEnemy parentParam;

    // 190207 LifeBalance
    // for game balance, when enemy appear in camera, Collider is online
    public BoxCollider2D enemyCollider;

    private void OnEnable()
    {
        parentParam = parent.GetComponent<ControllerEnemy>();
        enemyCollider.enabled = false;

        GameManager.onPlayerDie += OnPlayerDie;
        GameManager.onDestroyAllEnemy += OnPlayerDie;
        //GameManager.onDeadByItemBomb += DeadByItemBomb;
    }

    //private void OnBecameVisible()
    //{
    //    enemyCollider.enabled = true;
    //    //GameManager.Instance.enemyPos.Add(parent.gameObject);
    //    PublicValueStorage.Instance.AddEnemyPos(parent);
    //    //parentParam.CallbackAddEnemyPos(parent);
    //    GameManager.onDeadByItemBomb += DeadByItemBomb;
    //}

    public void SetEventAuto()
    {
        enemyCollider.enabled = true;
        //GameManager.Instance.enemyPos.Add(parent.gameObject);
        PublicValueStorage.Instance.AddEnemyPos(parent);
        //parentParam.CallbackAddEnemyPos(parent);
        GameManager.onDeadByItemBomb += DeadByItemBomb;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Enemy : " + collision.name);
        if (collision.tag == "PlayerMissile")
        {
            Instantiate(enemyExplosion.gameObject, this.transform.position, Quaternion.identity).gameObject.SetActive(true);
            parentParam.CallbackEnemyDie(parent);
            Destroy(parent);
        }
    }
    // 190605 LifeBalance
    public void OnPlayerDie()
    {
        //Debug.Log("Enemy Pilot Destroy to " + this.transform.position);
        Instantiate(enemyExplosion.gameObject, this.transform.position, Quaternion.identity).gameObject.SetActive(true);
        Destroy(parent);
    }

    public void DeadByItemBomb()
    {
        //Destroy(parent);
        //Debug.Log("name : " + parent.name);
        //parentParam.CallbackDeadByItemBomb(parent);
        Instantiate(enemyExplosion.gameObject, this.transform.position, Quaternion.identity).gameObject.SetActive(true);
        parentParam.CallbackEnemyDie(parent);
    }

    private void OnDisable()
    {
        GameManager.onPlayerDie -= OnPlayerDie;
        GameManager.onDestroyAllEnemy -= OnPlayerDie;
        GameManager.onDeadByItemBomb -= DeadByItemBomb;
    }
}
