using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderRoute : MonoBehaviour
{
    //public Texture texture = null;
    //public Shader shader = null;
    public Material LadderMaterial = null;

    public GameObject[] positions;
    public float[] size;
    public GameObject rotateBody;

    // Route Animation Values
    public Vector3 start, end;
    public bool moveStart = false;
    public float moveSpeed = 1.0f;
    private float moveSpeedProcess = 0;

    public Vector3[] verticles;
    public GameObject[] transforms;

    private Mesh mesh;

    public int[] triangles;
    public int[] trianglesRight;
    public int[] trianglesLeft;

    void OnEnable()
    {
        mesh = new Mesh();

        transforms = new GameObject[positions.Length * 2];

        GameObject dummy;
        for (int i = 0, j = 0; i < (positions.Length * 2); i++)
        {
            j = i / 2;

            dummy = new GameObject(positions[j].name);
            dummy.transform.parent = positions[j].transform;
            transforms[i] = dummy;
        }

        verticles = new Vector3[transforms.Length];


        for(int i = 0; i < verticles.Length; i++)
        {
            verticles[i] = (Vector3)transforms[i].transform.position;
        }

        triangles = new int[] {0,1,2,
                                   0, 2, 3};
        trianglesLeft = new int[] { 0, 1, 2, 0, 2, 3 };
        trianglesRight = new int[] { 1, 0, 2, 2, 0, 3 };

        Vector2[] uvs = new Vector2[] { new Vector2(0f, 1f),
                                        new Vector2(1f, 1f),
                                        new Vector2(1f, 0f),
                                        new Vector2(0f, 0f)};

        for (int i = 0, j = 0, k = -1; i < verticles.Length; i++)
        {
            // Left
            if (i % 2 == 0)
            {
                transforms[i].transform.localPosition = new Vector3(size[j] * k, 0, 0);
                k *= -1;
            }
            // Right
            else
            {
                transforms[i].transform.localPosition = new Vector3(size[j] * k, 0, 0);
                j++;
            }
        }


        mesh.vertices = verticles;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;

        //Material material = new Material(Shader.Find("Standard"));
        //Material material = new Material(shader);
        //material.SetTexture("_MainTex", texture);
        GetComponent<MeshRenderer>().material = LadderMaterial;

        positions[0].GetComponent<BoxCollider2D>().size = new Vector2(size[0], size[0]);
        positions[1].GetComponent<BoxCollider2D>().size = new Vector2(size[1], size[1]);

        this.GetComponent<MeshRenderer>().sortingLayerName = "sortingLayer";
        this.GetComponent<MeshRenderer>().sortingOrder = 2;
    }

    // Update is called once per frame
    void Update()
    {
        MoveRoute();


        // Set Vertex order is
        // 1.Left, 2.Right
        // 4.Left, 3.Right
        for (int i = 0; i < transforms.Length; i++)
        {
            verticles[i] = transforms[i].transform.position - this.transform.position;
        }


        mesh.vertices = verticles;
        //mesh.triangles = triangles;

    }

    private void MoveRoute()
    {
        if (moveStart == true)
        {
            if (moveSpeedProcess >= 1.0)
            {
                moveStart = false;
                moveSpeedProcess = 0;
            }
            else
            {
                moveSpeedProcess += Time.deltaTime * moveSpeed;
                positions[1].transform.position =
                    Vector3.Lerp(start, end, moveSpeedProcess);
            }
        }
    }

    public void SetMoveRoute(bool enable)
    {
        if (moveStart == true)
        {
            //Debug.Log("Now moving . . .");
            return;
        }

        moveStart = enable;
    }

    public void SetTopPosition(Vector3 position)
    {
        start = position;   
        //positions[0].transform.position = position;
    }

    public void SetBottomPosition(Vector3 position)
    {
        end = position;
        //positions[1].transform.position = position;
    }

    public void SetPosition(Vector3 top, Vector3 bottom, bool isRightToLeft)
    {

        if (positions.Length <= 1)
        {
            //Debug.LogError("Ladder Route Index is lower than 2");
            return;
        }


        // REMEMBER Top To Bottom
        // Right to Left

        if (isRightToLeft == true)
        {
            rotateBody.transform.rotation = Quaternion.Euler(0, 0, 90f);
            mesh.triangles = trianglesRight;
        }
        // Left To Right
        else
        {
            rotateBody.transform.rotation = Quaternion.Euler(0, 0, -90f);
            mesh.triangles = trianglesRight;
        }

        start = top;
        end = bottom;
        positions[0].transform.position =
            positions[1].transform.position = top;
        //positions[0].transform.position = top;
        //positions[1].transform.position = bottom;

        moveStart = true;
    }

    public Vector2[] GetStartEndPositions()
    {
        Vector2[] value = {positions[0].transform.position,
                           positions[1].transform.position};
        return value;
    }

    public System.Action GetOnDestroyRoute()
    {
        return DestroyRoute;
    }

    public void DestroyRoute()
    {
        Destroy(this.gameObject);
    }
}
