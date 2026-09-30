using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float dampTime = 0.12f;
    [SerializeField] private PlayerVisuals playerVisuals;
    [SerializeField] private DayCycle dayCycle;
    [SerializeField] private ShopSlot shopSlot;

    private Vector2 mousePosition;
    private Vector2 targetPosition;
    private Vector2 dampVelocity = Vector2.zero;

    private float eatDuration = 0.2f;
    private float eatTimeRemaining;
    [SerializeField] private float minimumEatCooldown = 0.5f;
    public bool CanUpgradeEatCooldown =>
    eatCooldown > minimumEatCooldown + 0.001f;
    private float eatCooldown = 2.3f;
    private float eatCooldownRemaining;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [SerializeField] private Image[] cooldownFills;

    private void Awake()
    {
        originalColor = spriteRenderer.color;
        targetPosition = transform.position;
    }

    public void OnFollowMouse(InputValue value)
    {
        if (gameManager.IsGameOver)
            return;

        mousePosition = value.Get<Vector2>();
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector2(mousePosition.x, mousePosition.y)
        );

        worldPosition.x = Mathf.Clamp(worldPosition.x, -2f, 2f);
        worldPosition.y = Mathf.Clamp(worldPosition.y, -0.5f, 1.5f);
        targetPosition = worldPosition;
    }

    public void OnEat(InputValue value)
    {
        if (gameManager.IsGameOver)
            return;

        if (dayCycle.IsNight)
        {
            if (value.isPressed)
            {
                shopSlot.TryBuy(transform.position);
            }

            return;
        }

        if (value.isPressed && eatTimeRemaining <= 0f && eatCooldownRemaining <= 0f)
        {
            eatTimeRemaining = eatDuration;
            eatCooldownRemaining = eatDuration + eatCooldown;

            playerVisuals.PlayEatAnimation(eatDuration);
        }
    }

    private void Update()
    {
        // Bei Game Over keine Bewegung und keine weiteren Eingaben verarbeiten.
        if (gameManager.IsGameOver || Time.deltaTime <= 0f)
            return;

        if (eatTimeRemaining > 0f)
            eatTimeRemaining -= Time.deltaTime;

        if (eatCooldownRemaining > 0f)
            eatCooldownRemaining -= Time.deltaTime;

        UpdateEatCooldownUI();

        transform.position = Vector2.SmoothDamp(
            transform.position,
            targetPosition,
            ref dampVelocity,
            dampTime
        );
    }

    private void UpdateEatCooldownUI()
    {
        float totalDuration = eatDuration + eatCooldown;
        float progress = Mathf.Clamp01(
            1f - eatCooldownRemaining / totalDuration
        );

        int filledCount = Mathf.FloorToInt(progress * cooldownFills.Length);

        for (int i = 0; i < cooldownFills.Length; i++)
        {
            cooldownFills[i].enabled = i < filledCount;
        }
    }

    public bool isEating()
    {
        return !gameManager.IsGameOver && eatTimeRemaining > 0f;
    }

    // TomatoProjectile kann weiterhin playerMovement.TakeHit() aufrufen.
    public void TakeHit()
    {
        gameManager.TakeHit();
    }

    public void OnTomatoCaught()
    {
        gameManager.AddTomato();
        playerVisuals.PlayEatAnimation(eatCooldownRemaining, true);
    }

    public void UpgradeEatCooldown()
    {
        eatCooldown = Mathf.Max(minimumEatCooldown, eatCooldown - 0.3f);

        eatCooldownRemaining = Mathf.Min(
            eatCooldownRemaining,
            eatDuration + eatCooldown
        );

        UpdateEatCooldownUI();
    }

}
