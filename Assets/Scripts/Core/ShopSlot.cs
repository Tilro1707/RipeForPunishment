using UnityEngine;
using TMPro;
public class ShopSlot : MonoBehaviour
{
    [SerializeField] private Collider2D purchaseArea;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private TextMeshProUGUI priceTXT;

    private int upgradePrice = 2;

    private void Start()
    {
        UpdatePriceUI();
    }

    private void UpdatePriceUI()
    {
        priceTXT.SetText(
            playerMovement.CanUpgradeEatCooldown
                ? upgradePrice + ""
                : " - "
        );
    }

    public void TryBuy(Vector2 playerPosition)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (!purchaseArea.OverlapPoint(playerPosition))
            return;

        if (!playerMovement.CanUpgradeEatCooldown)
            return;

        if (!gameManager.TrySpendTomatoes(upgradePrice))
        {
            return;
        }

        playerMovement.UpgradeEatCooldown();
        upgradePrice++;

        UpdatePriceUI();
        MusicManager.Instance.PlayEatingSound();
    }
}
