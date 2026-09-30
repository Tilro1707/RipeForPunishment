using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject splatPrefab;
    [SerializeField] private RectTransform splatParent;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Spawner spawner;
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private PlayerVisuals playerVisuals;
    [SerializeField] private TextMeshProUGUI tomatoCounterTXT;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject catchPanel;
    [SerializeField] private GameObject endingContent;
    private int tomatoCount;
    private readonly List<Vector2> availableCorners = new List<Vector2>();

    public bool IsGameOver { get; private set; }

    public bool IsEnding { get; private set; }
    private readonly List<GameObject> spawnedSplats = new List<GameObject>();

    private void Awake()
    {
        Time.timeScale = 1f;
        IsGameOver = false;
        gameOverPanel.SetActive(false);
        endingPanel.SetActive(false);
    }

    private void Start()
    {
        // Canvas-Groesse vor dem Berechnen der vier Positionen aktualisieren.
        Canvas.ForceUpdateCanvases();

        float cornerX = splatParent.rect.width / 4f;
        float cornerY = splatParent.rect.height / 4f;

        availableCorners.Add(new Vector2(-cornerX, cornerY));
        availableCorners.Add(new Vector2(cornerX, cornerY));
        availableCorners.Add(new Vector2(-cornerX, -cornerY));
        availableCorners.Add(new Vector2(cornerX, -cornerY));

        UpdateTomatoUI();
    }

    public void TakeHit()
    {
        if (IsGameOver || IsEnding)
            return;

        MusicManager.Instance.PlayPlayerHitSound();

        GameObject newSplat = Instantiate(splatPrefab, splatParent);
        spawnedSplats.Add(newSplat);
        RectTransform splatRect = newSplat.GetComponent<RectTransform>();

        if (availableCorners.Count > 0)
        {
            int randomIndex = Random.Range(0, availableCorners.Count);

            Vector2 cornerPosition = availableCorners[randomIndex];

            splatRect.anchoredPosition = cornerPosition;
            availableCorners.RemoveAt(randomIndex);
            playerVisuals.SetHitStage(4 - availableCorners.Count);

            StartCoroutine(FadeSplat(newSplat, cornerPosition));
        }
        else
        {
            splatRect.anchoredPosition = Vector2.zero;
            playerVisuals.SetHitStage(5);
            GameOver();
        }
    }

    public void GameOver()
    {
        if (IsGameOver || IsEnding)
            return;

        IsGameOver = true;
        MusicManager.Instance.PlayGameOverSound();

        spawner.enabled = false;
        Time.timeScale = 0f;

        gameOverPanel.SetActive(true);
        gameOverPanel.transform.SetAsLastSibling();
    }

    public void WinGame()
    {
        if (IsGameOver || IsEnding)
            return;

        IsEnding = true;
        spawner.enabled = false;
        Time.timeScale = 1f;

        StopAllCoroutines();

        foreach (GameObject splat in spawnedSplats)
        {
            Destroy(splat);
        }

        spawnedSplats.Clear();

        StartCoroutine(EndSequence());
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator FadeSplat(GameObject splat, Vector2 cornerPosition)
    {
        // Nur dieser Ablauf wartet. Das Spiel läuft weiter.
        yield return new WaitForSeconds(5f);

        Image image = splat.GetComponent<Image>();
        Color color = image.color;
        float startAlpha = color.a;

        float elapsedFadeTime = 0f;
        float fadeDuration = 5f;

        while (elapsedFadeTime < fadeDuration)
        {
            elapsedFadeTime += Time.deltaTime;
            float fadeProgress = elapsedFadeTime / fadeDuration;

            color.a = Mathf.Lerp(startAlpha, 0f, fadeProgress);
            image.color = color;

            // Hier pausieren und im nächsten Frame weitermachen.
            yield return null;
        }

        Destroy(splat);
        availableCorners.Add(cornerPosition);
        playerVisuals.SetHitStage(4 - availableCorners.Count);
    }

    private IEnumerator EndSequence()
    {
        tomatoCounterTXT.gameObject.SetActive(false);
        catchPanel.SetActive(false);

        Vector3 startPosition = cameraTransform.position;
        Vector3 targetPosition = new Vector3(
            startPosition.x, -2f, startPosition.z
        );

        float elapsed = 0f;
        float duration = 3f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            cameraTransform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                elapsed / duration
            );

            yield return null;
        }

        cameraTransform.position = targetPosition;

        yield return new WaitForSecondsRealtime(1f);

        endingContent.SetActive(false);

        Image background = endingPanel.GetComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0f);

        endingPanel.SetActive(true);
        endingPanel.transform.SetAsLastSibling();

        float fadeElapsed = 0f;
        float fadeDuration = 1.5f;

        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(fadeElapsed / fadeDuration);

            background.color = new Color(0f, 0f, 0f, alpha);

            yield return null;
        }

        background.color = Color.black;

        // Erst jetzt Bewegung sperren und Inhalt anzeigen.
        IsGameOver = true;
        IsEnding = false;
        endingContent.SetActive(true);
    }

    public void AddTomato()
    {
        if (IsGameOver)
            return;

        tomatoCount++;
        UpdateTomatoUI();
    }

    public bool TrySpendTomatoes(int price)
    {
        if (IsGameOver || IsEnding || price < 0 || tomatoCount < price)
            return false;

        tomatoCount -= price;
        UpdateTomatoUI();
        return true;
    }

    private void UpdateTomatoUI()
    {
        tomatoCounterTXT.SetText("Caught: " + tomatoCount);
    }
}
