using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTest : MonoBehaviour {
    public GameObject player;

    private float distance;
    private Vector2 heading, direction2;
    // Use this for initialization
    void Start () {
        heading = player.transform.position - this.transform.position;
        distance = heading.magnitude;
        direction2 = heading / distance;
    }
	
	// Update is called once per frame
	void Update () {
        heading = player.transform.position - this.transform.position;
        distance = heading.magnitude;
        direction2 = heading / distance;

        this.transform.Translate(direction2 * Time.deltaTime);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        Debug.Log(collision.name);
        this.gameObject.SetActive(false);
    }
}
