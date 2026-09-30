using UnityEngine;
using UnityEngine.SceneManagement;

public class TomatoProjectile : MonoBehaviour
{
    private float elapsedFlightTime;
    private float flightDuration = 1.5f;

    private Vector2 startPosition;
    private Vector2 targetPosition;

    private Vector3 startScale;
    private Vector3 endScale = new Vector3(0.3f, 0.3f, 1f);

    private GameObject player;
    private PlayerMovement playerMovement;
    private Collider2D playerColider;

    [SerializeField] private GameObject shadowPrefab;
    private Transform timingRing;
    private Vector3 ringStartScale;
    private Vector3 ringEndScale;
    private GameObject shadowInstance;

    [SerializeField] private GameObject tomatoSplatPrefab;
    private Collider2D wheelCollider;

    private void Start()
    {
        wheelCollider = GameObject.FindGameObjectWithTag("DayWheel").GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerColider = player.GetComponent<Collider2D>();
        playerMovement = player.GetComponent<PlayerMovement>();

        startPosition = transform.position;
        startScale = transform.localScale;
        elapsedFlightTime = 0f;

        Vector2 playerPosition = player.transform.position;

        switch (Random.Range(0, 3))
        {
            case 0: // Direkt auf die aktuelle Spielerposition
                targetPosition = playerPosition;
                break;

            case 1: // In die Nähe des Spielers
                targetPosition = playerPosition + Random.insideUnitCircle * 0.75f;
                break;

            case 2: // Zufällig ins Spielfeld
                targetPosition = new Vector2(
                    Random.Range(-2f, 2f),
                    Random.Range(-0.5f, 1.5f)
                );
                break;
        }

        targetPosition.x = Mathf.Clamp(targetPosition.x, -2f, 2f);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -0.5f, 1.5f);

        shadowInstance = Instantiate(
            shadowPrefab,
            targetPosition,
            Quaternion.identity
        );

        timingRing = shadowInstance.transform.Find("TimingRing");
        ringStartScale = timingRing.localScale;
        ringEndScale = shadowInstance.transform.Find("TargetCircle").localScale;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedFlightTime += Time.deltaTime;
        float flightProgress = elapsedFlightTime / flightDuration;
        transform.position = Vector2.Lerp(startPosition, targetPosition, flightProgress);
        transform.localScale = Vector3.Lerp(startScale, endScale, flightProgress);
        timingRing.localScale = Vector3.Lerp(ringStartScale,ringEndScale,flightProgress);
        if (flightProgress >= 1)
        {
            if (playerColider.OverlapPoint(targetPosition))
            {
                if (playerMovement.isEating()){
                    MusicManager.Instance.PlayEatingSound();
                    playerMovement.OnTomatoCaught();
                }
                else
                {
                    playerMovement.TakeHit();
                }
            }
            else
            {
                GameObject splat = Instantiate(
                tomatoSplatPrefab,
                targetPosition,
                Quaternion.identity
                );

                MusicManager.Instance.PlayBackgroundHitSound();

                if (wheelCollider.OverlapPoint(targetPosition))
                {
                    splat.transform.SetParent(wheelCollider.transform, true);
                }

                Destroy(splat, 20f);
            }
            Die();
        } 
    }

    private void Die()
    {
        Destroy(shadowInstance);
        Destroy(gameObject);     
    }
}
