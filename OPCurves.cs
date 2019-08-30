using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OPCurves : MonoBehaviour {

    public void Bezier2DLerp(GameObject target,Vector2 startPos, Vector2 centerPos, Vector2 endPos, float speed)
    {
        target.transform.position = 
            (Vector2.Lerp(Vector2.Lerp(startPos, centerPos, speed),
            Vector2.Lerp(centerPos, endPos, speed),
            speed));
    }

    public Vector2 SeekDirection(Vector2 start, Vector2 target)
    {
        Vector2 heading = target - start;
        float distance = heading.magnitude;
        return heading / distance;
    }

    public Vector2 GetHeading(Vector2 start, Vector2 target)
    {        
        return target - start;
    }

    //float eut = Quaternion.FromToRotation(Vector3.right, startPoint.transform.position - endPoint.transform.position).eulerAngles.z;
    //Vector2 temp = opCurves.GetHeading(startPoint.gameObject.transform.position, endPoint.gameObject.transform.position);
    //float z = -Mathf.Atan2(temp.x, temp.y) * Mathf.Rad2Deg;
}
