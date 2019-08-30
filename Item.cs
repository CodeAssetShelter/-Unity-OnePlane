using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public OPCurves opCurves;

    public float itemSpeed = 1.0f;
    private Vector2 direction = Vector2.zero;

	// Use this for initialization
	void Start () {
        //opCurves = opCurves.GetComponent<OPCurves>();

        //direction = opCurves.SeekDirectionToPlayer(this.gameObject.transform.position, GameManager.Instance.playerPos);
        direction = opCurves.SeekDirection(this.gameObject.transform.position, PublicValueStorage.Instance.GetPlayerPos());
    }

    private void OnEnable()
    {
        GameManager.onPlayerDie += OnPlayerDie;
        GameManager.onDestroyAllEnemy += OnPlayerDie;
    }

    // Update is called once per frame
    void Update () {
        this.transform.Translate(direction * Time.deltaTime * itemSpeed);
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

    private void OnDisable()
    {
        GameManager.onPlayerDie -= OnPlayerDie;
        GameManager.onDestroyAllEnemy -= OnPlayerDie;
    }
}
