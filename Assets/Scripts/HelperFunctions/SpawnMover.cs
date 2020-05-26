using UnityEngine;
using System.Collections;

public class SpawnMover : MonoBehaviour
{
    public GameObject[] SpawnPointsMovers;
    public GameObject[] MoverObjects;
    public int MaxNumMov;
    public float StartWait;
    public float SpawnWait;
    public float SpawnWaveWait;
    private const string LayerName = "Background";
    private const int SortingOrder = 1;


    void Start()
    {
        StartCoroutine(WaitAtStart());
        StartCoroutine(SpawnMovers());
    }


    void Update()
    {
        
    }

    IEnumerator SpawnMovers()
    {
        while (true)
        {
            int curMovers = GameObject.FindGameObjectsWithTag("Movers").Length;
            for (int i = curMovers; i < Random.Range(0, MaxNumMov); ++i)
            {
                GameObject hazard = MoverObjects[Random.Range(0, MoverObjects.Length)];
                Vector3 spawnPosition = SpawnPointsMovers[Random.Range(0, SpawnPointsMovers.Length)].transform.position;
                Quaternion spawnRotation = Quaternion.identity;
                if (spawnPosition.x > 0)
                {
                    spawnRotation.y = 180;
                }
                GameObject tmpMover = Instantiate(hazard, spawnPosition, spawnRotation);
                SpriteRenderer tmpSprite = tmpMover.GetComponent<SpriteRenderer>();
                tmpSprite.sortingOrder = SortingOrder;
                tmpSprite.sortingLayerName = LayerName;
                yield return new WaitForSeconds(SpawnWait);
            }
            yield return new WaitForSeconds(SpawnWaveWait);
        }
    }

    IEnumerator WaitAtStart()
    {
        yield return new WaitForSeconds(StartWait);
    }

}
