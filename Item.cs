using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public OPCurves opCurves;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public float itemSpeed = 1.5f;
    public Rigidbody2D rigid2D;
    private Vector2 direction = Vector2.zero;

    private GameObject player;
    private 
	// Use this for initialization
	void Start () {
        //opCurves = opCurves.GetComponent<OPCurves>();
        player = PublicValueStorage.Instance.GetPlayer();
        //direction = opCurves.SeekDirectionToPlayer(this.gameObject.transform.position, GameManager.Instance.playerPos);
    }

    private void OnDestroyAllObject()
    {
        Destroy(this.gameObject);
    }
    private void OnEnable()
    {
        GameManager.onDestroyAllObject += OnDestroyAllObject;
        GameManager.onPlayerDie += OnPlayerDie;
        GameManager.onDestroyAllEnemy += OnPlayerDie;
    }

    // Update is called once per frame
    void Update () {
        if (player != null)
        {
            direction = opCurves.SeekDirection(this.gameObject.transform.position, player.transform.position);
            this.transform.Translate(direction * Time.deltaTime * itemSpeed);
        }
        else
        {
            this.transform.Translate(direction * Time.deltaTime * itemSpeed);
        }
    }

    public void ItemAddForce(Vector2 vector)
    {
        rigid2D.AddForce(vector, ForceMode2D.Impulse);
        //StartCoroutine(ExitAddForce());
    }
    //float exitProcess = 0;
    //IEnumerator ExitAddForce()
    //{
    //    while (true)
    //    {
    //        if (1.0 <= exitProcess)
    //        {
    //            rigid2D.AddForce(Vector2.zero, ForceMode2D.Force);
    //            yield break;
    //        }
    //        yield return null;
    //    }
    //}


    public void ActiveCoinAnimation(bool active)
    {
        animator.SetBool("CoinAnim", active);
        //animator.Play("Coin");
    }

    private void OnPlayerDie()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Player":
            case "WasteBasket":
                Destroy(this.gameObject);
                break;

        }

    }

    private void OnDestroy()
    {
        GameManager.onDestroyAllObject -= OnDestroyAllObject;
        GameManager.onPlayerDie -= OnPlayerDie;
        GameManager.onDestroyAllEnemy -= OnPlayerDie;
    }
}
