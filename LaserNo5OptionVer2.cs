using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserNo5OptionVer2 : MonoBehaviour
{
    public float shotDelay = 0;
    public float shotTimer = -1;
    public int myIndex = -1;
    private ControllerLaserNo5Ver2 myParent;
    private MissileModuleLaserNo5Ver2 myParentModule;
    private new Rigidbody2D rigidbody2D;
    private BossStatusModule parentHpBar;
    private StatusModule myStatus;
    private float rivisionValue;

    private void Start()
    {
        rivisionValue = PublicValueStorage.Instance.GetAddSpeedRivisionValue();
        parentHpBar = this.transform.parent.GetComponent<BossStatusModule>();
        myParent = this.transform.parent.GetComponent<ControllerLaserNo5Ver2>();
        myParentModule = myParent.GetMissileModule();
        myStatus = this.GetComponent<StatusModule>();
        rigidbody2D = this.GetComponent<Rigidbody2D>();
        rigidbody2D.simulated = false;

        float gap = this.transform.GetComponent<SpriteRenderer>().bounds.extents.x;

        this.transform.GetChild(0).Translate(0, -gap * 0.25f, 0, Space.Self);
    }

    public Vector3 GetLaserGunPos()
    {
        return this.transform.GetChild(0).position;
    }

    public void SetOnSimulated()
    {
        rigidbody2D.simulated = true;
    }

    public void StartShotTimer(float shotDelay)
    {
        this.shotDelay = shotDelay;
        shotTimer = 0;
        StartCoroutine(ShotTimer());
    }

    public void Reset()
    {
        shotTimer = -1;
        StopAllCoroutines();
    }

    IEnumerator ShotTimer()
    {
        while (true)
        {
            if(shotTimer >= shotDelay)
            {
                StopCoroutine(ShotTimer());
                yield break;
            }
            shotTimer += Time.deltaTime;
            yield return null;
        }
    }

    private void OnDisable()
    {
        if (this.transform.parent == null) return;
        if (PublicValueStorage.Instance == null) return;
        if (parentHpBar != null && myStatus.hpBar.value <= 0)
        {
            myParentModule.OptionExplosion(myIndex);
            parentHpBar.DamageToBoss(25 * rivisionValue);
            PublicValueStorage.Instance.AddMissileScore();
            this.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        //Debug.Log("Disable!");
    }
}
