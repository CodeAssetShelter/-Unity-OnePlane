using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnd : MonoBehaviour
{
    public new BoxCollider2D collider;
    public new SpriteRenderer renderer;

    private void OnEnable()
    {
        collider.size = new Vector2(renderer.bounds.extents.x * 2.0f, renderer.bounds.extents.y * 2.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.tag == "LaserNo5")
        {
            if (collision.tag == "Player")
            {
                PublicValueStorage.Instance.GetPlayerComponent().ActivatePlayerDie();
            }
            if(collision.tag == "PlayerShield")
            {
                Destroy(this.transform.parent.gameObject);
            }
        }
        if ( collision.tag == "WasteBasket")
        {
            Destroy(this.transform.parent.gameObject);
        }
        
    }
}
