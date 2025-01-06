using UnityEngine;

public class CustomerSpawner : Singleton<CustomerSpawner>
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private float spawnIntervalMin = 5f;
    [SerializeField] private float spawnIntervalMax = 10f;
    [SerializeField] private bool canSpawn = false;

    private float spawnTimer = 0f;
    private float spawnInterval;

    private void Update()
    {
        if(!canSpawn) return;

        if(spawnTimer <= 0f)
        {
            Instantiate(customerPrefab, transform.position, Quaternion.identity);
            spawnInterval = Random.Range(spawnIntervalMin, spawnIntervalMax);
            spawnTimer = spawnInterval;
        }

        spawnTimer -= Time.deltaTime;
    }

    public void ChangeCanSpawn(bool cond)
    {
        canSpawn = cond;
    }
}
