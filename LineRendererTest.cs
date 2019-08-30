using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererTest : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public SpriteRenderer startPoint;
    public SpriteRenderer endPoint;
    public BoxCollider2D LineCollider;

    private float lineWidthStart = 1.0f;
    private float lineWidthEnd = 1.0f;

    private Vector2 playerposition = new Vector2(0, -4.1f);
    private Vector2 direction = Vector2.down;

    OPCurves opCurves;
    
    void Start()
    {
        lineWidthStart = startPoint.bounds.extents.x * 2.0f;
        lineWidthEnd = endPoint.bounds.extents.x * 2.0f;
        lineRenderer.startWidth = lineWidthStart;
        lineRenderer.endWidth = lineWidthEnd;
        LineCollider.size = new Vector2(lineWidthStart, startPoint.bounds.extents.y * 2.0f);

        opCurves = this.GetComponent<OPCurves>();

        InitLaserVector();
    }
    // Update is called once per frame
    void Update()
    {
        ActivateLaser();
    }


    private void InitLaserVector()
    {
        float eu = Quaternion.FromToRotation(Vector3.up, startPoint.transform.position - endPoint.transform.position).eulerAngles.z;

        startPoint.transform.rotation = Quaternion.Euler(0, 0, eu);
        endPoint.transform.rotation = Quaternion.Euler(0, 0, eu);
    }
    private void ActivateLaser()
    {
        float eu =  Quaternion.FromToRotation(Vector3.up, startPoint.transform.position - endPoint.transform.position).eulerAngles.z;

        LineCollider.transform.rotation = Quaternion.Euler(0, 0, eu-90.0f);
        float size = Vector2.Distance(startPoint.transform.position, endPoint.transform.position);
        LineCollider.size = new Vector2(size + LineCollider.size.y, LineCollider.size.y);

        LineCollider.transform.position = (startPoint.transform.position + endPoint.transform.position) * 0.5f;
        startPoint.transform.rotation = Quaternion.Euler(0, 0, eu);
        endPoint.transform.rotation = Quaternion.Euler(0, 0, eu);

        lineRenderer.SetPosition(0, startPoint.transform.position);
        lineRenderer.SetPosition(1, endPoint.transform.position);
    }
}
