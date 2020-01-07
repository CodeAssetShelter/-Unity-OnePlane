using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// 
/// 
///  Planet GameObjects are must supervis object pooling
/// 
/// 
/// 
/// </summary>
public class MapScrollPlanets : MonoBehaviour
{
    public class PlanetInfo
    {
        public enum State { Idle = 0, Move, End }
        public State state;

        public GameObject planetObject;
        public SpriteRenderer planetSpriteRenderer;
        public float spawnTimer;
        public float spawnTimerProcess = 0;
        public float planetSpeed;

        public PlanetInfo(GameObject original, Sprite planetSprite, Vector3 defaultPosition, Transform parent)
        {
            planetObject = Instantiate(original, defaultPosition, Quaternion.identity, parent);
            planetSpriteRenderer = planetObject.GetComponent<SpriteRenderer>();
            planetSpriteRenderer.sprite = planetSprite;
            planetObject.transform.Rotate(0, 0, Random.Range(0, 360f));
            state = State.Idle;
        }

        public void MoveDown()
        {
            planetObject.transform.Translate(Vector2.down * planetSpeed, Space.World);
        }

        public bool IsEndOfWaitTime()
        {
            if (spawnTimerProcess >= spawnTimer)
            {
                state = State.Move;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetPlanet(Vector3 position, float moveSpeed)
        {
            float scale = Random.Range(0.1f, 0.8f);

            planetObject.transform.position = position;
            planetObject.transform.localScale = new Vector3(scale, scale, 1);
            this.planetSpeed = moveSpeed;
            planetObject.SetActive(true);
        }

        public void Reset(float spawnTimer, float moveSpeed)
        {
            planetObject.SetActive(false);
            spawnTimerProcess = 0;
            this.spawnTimer = spawnTimer;
            this.planetSpeed = moveSpeed;
            planetObject.transform.Rotate(0, 0, Random.Range(0, 360f));
            this.state = State.Idle;
        }
    }

    public GameObject planetPrefab;
    public Sprite[] planetSprites;
    public PlanetInfo[] planetInfo;

    private Vector2 screenSize;
    private Vector3 defaultPlanetPosition = Vector3.zero;
    private float planetStartYPos, planetEndYPos;

    public float moveSpeed;
    [Range(0.01f, 0.03f)]
    public float moveSpeedMax;
    readonly float moveSpeedMin = 0.01f;

    public float spawnTimer;
    [Range(30,240)]
    public float spawnTimerMax;
    readonly float spawnTimerMin = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        screenSize = PublicValueStorage.Instance.GetScreenSize();
        //Debug.Log("Size : " + screenSize);
        planetInfo = new PlanetInfo[planetSprites.Length];

        defaultPlanetPosition.y += screenSize.y;
        defaultPlanetPosition.z = planetPrefab.transform.position.z;
        for(int i = 0; i < planetSprites.Length; i++)
        {
            Vector3 spawnPos = defaultPlanetPosition;
            spawnPos.x = Random.Range(screenSize.x * -0.5f, screenSize.x * 0.5f);
            spawnPos.z = planetPrefab.transform.position.z;

            planetInfo[i] = new PlanetInfo(planetPrefab, planetSprites[i], spawnPos, this.transform);
            planetInfo[i].planetSpeed = Random.Range(moveSpeedMin, moveSpeedMax);
            planetInfo[i].spawnTimer = Random.Range(10, spawnTimerMax);

            planetInfo[i].planetObject.SetActive(false);

            //Debug.Log(planetInfo[i].planetSpriteRenderer.sprite.name + " 's Timer : " + planetInfo[i].spawnTimer);
        }

        StartCoroutine(CoroutineMovePlanets());
    }

    IEnumerator CoroutineMovePlanets()
    {
        while (true)
        {
            for (int i = 0; i < planetInfo.Length; i++)
            {
                if (planetInfo[i].state == PlanetInfo.State.Idle)
                {
                    planetInfo[i].spawnTimerProcess += Time.deltaTime;
                    //Debug.Log(planetInfo[i].planetSpriteRenderer.sprite.name + " 's Timer : " + planetInfo[i].spawnTimer);
                    //Debug.Log(planetInfo[i].planetSpriteRenderer.sprite.name + " 's Timer Process : " + 
                    //    planetInfo[i].spawnTimerProcess);
                    if (planetInfo[i].IsEndOfWaitTime() == true)
                    {
                        Vector3 spawnPos = defaultPlanetPosition;
                        spawnPos.x = Random.Range(screenSize.x * -0.5f, screenSize.x * 0.5f);
                        planetInfo[i].SetPlanet(spawnPos, Random.Range(moveSpeedMin, moveSpeedMax));
                    }
                }
                if (planetInfo[i].state == PlanetInfo.State.Move)
                {
                    planetInfo[i].MoveDown();

                    if (planetInfo[i].planetObject.transform.position.y <= -defaultPlanetPosition.y)
                    {
                        planetInfo[i].state = PlanetInfo.State.End;
                    }
                }
                if (planetInfo[i].state == PlanetInfo.State.End)
                {
                    planetInfo[i].Reset
                        (Random.Range(spawnTimerMin, spawnTimerMax), 
                        Random.Range(moveSpeedMin, moveSpeedMax));
                }
            }
            yield return null;
        }
    }
}
