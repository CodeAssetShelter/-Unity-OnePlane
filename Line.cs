using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public LineRenderer line;
    public Transform start;
    public Transform end;

    void Update()
    {
        line.SetPosition(0, start.position);
        line.SetPosition(1, end.position);
    }
}
