using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLine : MonoBehaviour
{
    private float addScoreDelay = 0.5f;
    private float addScoreTimeCheck = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerShield")
        {
            if (addScoreTimeCheck >= addScoreDelay)
            {
                //PublicValueStorage.Instance.AddMissileScore();
                addScoreTimeCheck = 0;
            }
            addScoreTimeCheck += Time.deltaTime;
        }
        if (collision.tag == "Player")
        {
            Destroy(this.transform.parent);
        }
    }
}
