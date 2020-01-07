using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusModule : MonoBehaviour
{
    public Slider hpBar;

    private CircleCollider2D circleCollider2D;
    private SpriteRenderer spriteRenderer;

    private Vector3 hpBarAddPos;

    // Start is called before the first frame update
    void Awake()
    {
        circleCollider2D = this.GetComponent<CircleCollider2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();

        float temp = spriteRenderer.bounds.extents.x * (1 / this.transform.localScale.x);
        circleCollider2D.radius = temp * 0.3f;

        Vector3 tempPos = this.transform.position;
        hpBarAddPos = Vector3.zero;
        hpBarAddPos.y = spriteRenderer.bounds.extents.x * 0.5f;
        tempPos.y += temp * 0.5f;

        Transform parent = hpBar.transform.parent;
        hpBar = Instantiate(hpBar, tempPos, Quaternion.Euler(Vector3.zero), UICanvas.Instance.transform);

        hpBar.transform.SetParent(UICanvas.Instance.transform);
        hpBar.maxValue = 100 * PublicValueStorage.Instance.GetAddSpeedRivisionValue();
        hpBar.value = hpBar.maxValue;
        StartCoroutine(RefreshHpBarPosition());
    }

    private void OnEnable()
    {
        float temp = spriteRenderer.bounds.extents.x * (1 / this.transform.localScale.x);
        circleCollider2D.radius = temp * 0.3f;

        Vector3 tempPos = this.transform.position;
        hpBarAddPos = Vector3.zero;
        hpBarAddPos.y = spriteRenderer.bounds.extents.x * 0.5f;
        tempPos.y += temp * 0.5f;

        hpBar.gameObject.SetActive(true);
        Transform parent = hpBar.transform.parent;

        hpBar.transform.SetParent(UICanvas.Instance.transform);
        hpBar.maxValue = 100 * PublicValueStorage.Instance.GetAddSpeedRivisionValue();
        hpBar.value = hpBar.maxValue;
        StartCoroutine(RefreshHpBarPosition());
    }

    public IEnumerator RefreshHpBarPosition()
    {
        while (true)
        {
            yield return null;
            hpBar.transform.position = this.transform.position + hpBarAddPos;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        string target = collision.tag;
        if (target.Contains("Reflect") || target.Contains("Player"))
        {
            //Debug.Log(collision.tag + " is touched");
            if (hpBar.value <= 0)
            {
                if (this.tag == "LaserNo5")
                {
                    DeactiveOption();
                }
                else
                {
                    Destroy(this.gameObject);
                }
                return;
            }
            hpBar.value--;
        }
    }

    private void DeactiveOption()
    {
        this.transform.Translate(0, 100, 0, Space.World);
        hpBar.transform.position = this.transform.position;
        if (this.transform.parent != null)
        {
            this.gameObject.SetActive(false);
            //Debug.Log("Deactive option");
        }
        hpBar.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (hpBar == null) return;
        Destroy(hpBar.gameObject);
    }
}
