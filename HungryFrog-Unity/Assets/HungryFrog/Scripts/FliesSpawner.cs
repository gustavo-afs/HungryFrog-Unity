using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class FliesSpawner : MonoBehaviour
{
    [SerializeField] private GameObject flyPrefab;
    [SerializeField] private float flySpeed;
    [SerializeField] private float radiusSpawnRange = 5f;
    [SerializeField] private float spawnTimeRange = 5f;
    [SerializeField] private float flylifetimeRange = 5f;
    private ObjectPool<GameObject> fliesPool;
    
    void Start()
    {
        fliesPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(flyPrefab),
            actionOnGet: (fly)=> fly.SetActive(true),
            actionOnRelease: (fly) => fly.SetActive(false),
            actionOnDestroy: (fly) => Destroy(fly),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 20
            );
    }

    public void StartSpawning()
    {
        StopAllCoroutines();
        StartCoroutine(Spawner());
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }

    IEnumerator Spawner()
    {
        do
        {
            //Random Timer to spawn the Fly
            float spawnTimer = Random.Range(0f, spawnTimeRange);
            yield return new WaitForSeconds(spawnTimer);
            
            //Random Fly position 
            var positionX = Random.Range(-radiusSpawnRange, radiusSpawnRange);
            var positionY = Random.Range(-radiusSpawnRange, radiusSpawnRange);
            var position = new Vector3(positionX, positionY, transform.position.z);
            var lifetime = Random.Range(2f, flylifetimeRange);
            var fly = fliesPool.Get();
            fly.GetComponent<Fly>().StartFly(position,lifetime, fliesPool);
        } while (true);
    }
}
