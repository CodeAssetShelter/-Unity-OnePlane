using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOSS : MonoBehaviour {

    //public delegate void BOSSHandler();
    //public static event BOSSHandler onGetDamaged;

    System.Action onGetDamaged;

    public GameObject parent;

    private ControllerLineFall parentParam;

    // 190423 LifeBalance
    // It is BOSS.cs' collision box
    public CircleCollider2D BossCollider;

    private void OnEnable()
    {
        BossCollider.enabled = false;
        //GameManager.onDeadByItemBomb += DeadByItemBomb;
        //parentParam = parent.GetComponent<ControllerLineFall>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerMissile")
        {
            //parentParam.GetDamaged();
            //Destroy(parent);
            onGetDamaged();
        }
    }

    public void SetMethodGetDamaged(System.Action getDamaged)
    {
        onGetDamaged = getDamaged;
    }

    public void SetColliderSwitch(bool enable)
    {
        BossCollider.enabled = enable;
    }
}
