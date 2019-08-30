using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoResizeRect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GridLayoutGroup grid = this.gameObject.GetComponent<GridLayoutGroup>();
        float child = this.transform.childCount;
        //Debug.Log("Child : " + child);
        Vector2 spacing = grid.spacing;
        Vector2 cellsize = grid.cellSize;
        this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (cellsize.y + spacing.y) * child);
    }
}
