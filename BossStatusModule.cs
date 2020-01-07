using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossStatusModule : MonoBehaviour
{
    public enum MODE { Normal = 0, LineFall, RLI, LaserNo5, StateCount }
    public MODE statusMode;

    public Slider BossHpBar;
    public Animator bossExplosion;

    private ColorBlock bossHpBarColor = ColorBlock.defaultColorBlock;

    private CircleCollider2D circleCollider2D;
    private SpriteRenderer spriteRenderer;

    private Vector3 hpBarAddPos;


    private void OnDestroyAllObject()
    {
        Destroy(this.gameObject);
    }
    // Start is called before the first frame update
    void Awake()
    {
        GameManager.onPlayerDie += OnPlayerDie;

        circleCollider2D = this.GetComponent<CircleCollider2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();

        float temp = spriteRenderer.bounds.extents.x * (1 / this.transform.localScale.x);
        circleCollider2D.radius = temp * 0.3f;

        Vector3 tempPos = this.transform.position;
        hpBarAddPos = Vector3.zero;
        hpBarAddPos.y = spriteRenderer.bounds.extents.x * 0.5f;
        tempPos.y += temp * 0.5f;


        BossHpBar = UICanvas.Instance.GetBossHpBar();
        BossHpBar.gameObject.SetActive(true);
        BossHpBar.maxValue = PublicValueStorage.Instance.GetBossHp();
        BossHpBar.value = BossHpBar.maxValue;
    }

    private void OnEnable()
    {
        GameManager.onDestroyAllObject += OnDestroyAllObject;

        float temp = spriteRenderer.bounds.extents.x * (1 / this.transform.localScale.x);
        circleCollider2D.radius = temp * 0.3f;

        Vector3 tempPos = this.transform.position;
        hpBarAddPos = Vector3.zero;
        hpBarAddPos.y = spriteRenderer.bounds.extents.x * 0.5f;
        tempPos.y += temp * 0.5f;

        BossHpBar.gameObject.SetActive(true);
        Transform parent = BossHpBar.transform.parent;

        BossHpBar.transform.SetParent(UICanvas.Instance.transform);
        BossHpBar.maxValue = 300 * PublicValueStorage.Instance.GetAddSpeedRivisionValue();
        BossHpBar.value = BossHpBar.maxValue;
    }

    private void OnBecameVisible()
    {
        GameManager.onDeadByItemBomb += GetDamagedByBomb;
        //GameManager.Instance.enemyPos.Add(this.gameObject);
        
        if (statusMode != MODE.LaserNo5)
            PublicValueStorage.Instance.AddEnemyPos(this.gameObject);
    }

    private void OnPlayerDie()
    {
        
    }

    private void GetDamagedByBomb()
    {
        float damage = BossHpBar.maxValue * 0.1f;
        DamageToBoss(damage);
    }

    public void DamageToBoss(float value)
    {
        switch (statusMode)
        {
            case MODE.Normal:
            case MODE.LineFall:
            case MODE.RLI:
                break;

            case MODE.LaserNo5:
                if (BossHpBar.value <= 0)
                {
                    OnBossDie();
                    break;
                }
                BossHpBar.value -= value;
                SetBossHpBarColor();
                break;
        }
        //Debug.Log("HP : " + BossHpBar.value + " // DAMAGE : " + value);
    }

    public void OnBossDie()
    {
        bossExplosion.gameObject.SetActive(true);
        bossExplosion.GetComponent<BossExplosion>().ExplosionSoundPlay();
        StartCoroutine(BossDie());
        //onBossDie(this.gameObject);
    }
    bool bossDie = false;
    IEnumerator BossDie()
    {
        while (true)
        {
            //Debug.Log("ss : " + bossExplosion.GetCurrentAnimatorStateInfo(0).normalizedTime);
            if (bossExplosion.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                if (bossDie == false)
                {
                    BossHpBar.gameObject.SetActive(false);
                    //Destroy(this.transform.gameObject);
                    PublicValueStorage.Instance.GetPlayerComponent().RestoreShield();
                    PublicValueStorage.Instance.BossDie(this.gameObject);
                    bossDie = true;
                }
                yield break;
            }
            yield return null;
        }
    }

    private void SetBossHpBarColor()
    {
        if (BossHpBar == null) return;

        bossHpBarColor.disabledColor = Color.Lerp(Color.red, Color.green, BossHpBar.value / BossHpBar.maxValue);
        BossHpBar.colors = bossHpBarColor;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        string target = collision.tag;
        if (target.Contains("Reflect") || target.Contains("Player"))
        {
            //Debug.Log(collision.tag + " is touched");
            if (BossHpBar.value <= 0)
            {
                if (this.tag == "LaserNo5")
                {
                    DeactiveOption();
                }
                else
                {
                    if (statusMode == MODE.LaserNo5)
                    {
                        //Debug.Log("New Laser No5 is Dead");
                    }
                    Destroy(this.gameObject);
                }
                return;
            }
            BossHpBar.value--;
        }
    }

    private void DeactiveOption()
    {
        this.transform.Translate(0, 100, 0, Space.World);
        BossHpBar.transform.position = this.transform.position;
        if (this.transform.parent != null)
        {
            this.gameObject.SetActive(false);
            //Debug.Log("Deactive option");
        }
        BossHpBar.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.onDestroyAllObject -= OnDestroyAllObject;
        GameManager.onPlayerDie -= OnPlayerDie;
        GameManager.onDeadByItemBomb -= GetDamagedByBomb;
    }

    private void OnDisable()
    {
        if (BossHpBar == null) return;

        BossHpBar.value = BossHpBar.maxValue;
        BossHpBar.gameObject.SetActive(false);
    }
}
