using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private TomatoProjectile tomatoPrefab;
    [SerializeField] private float spawnInterval = 0.4f;
    public float SpawnInterval
    {
        get => spawnInterval;
        set => spawnInterval = value;
    }   

    private float spawnTimer;
    private Vector2 spawnPosition;
    private void Update()
    {
        if (tomatoPrefab == null)
            return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            spawnPosition = transform.position;
            spawnPosition.x += Random.Range(-2, 2);
            Instantiate(tomatoPrefab, spawnPosition, Quaternion.identity);
            MusicManager.Instance.PlayThrowSound();

            spawnTimer = Mathf.Max(0.05f, spawnInterval);
        }
    }

}
