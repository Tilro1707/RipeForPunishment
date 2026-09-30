using System;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] hitHeads;
    [SerializeField] private Sprite[] eatingFrames;

    private int currentHitStage;
    private bool isEatAnimationPlaying;
    private float eatAnimationTime;
    private float eatAnimationDuration;
    private bool isChewing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer.sprite = hitHeads[currentHitStage];
    }

    private void Update()
    {
        if (!isEatAnimationPlaying || Time.deltaTime <= 0f)
            return;

        eatAnimationTime += Time.deltaTime;

        if (eatAnimationTime >= eatAnimationDuration)
        {
            isEatAnimationPlaying = false;
            spriteRenderer.sprite = hitHeads[currentHitStage];
            return;
        }

        if (!isChewing)
            return;

        float progress = eatAnimationTime / eatAnimationDuration;
        int chewingFrameCount = eatingFrames.Length - 1;

        int frameIndex = 1 + Mathf.Min(
            Mathf.FloorToInt(progress * chewingFrameCount),
            chewingFrameCount - 1
        );

        spriteRenderer.sprite = eatingFrames[frameIndex];
    }

    public void SetHitStage(int stage)
    {
        currentHitStage = Mathf.Clamp(stage, 0, hitHeads.Length - 1);

        if (!isEatAnimationPlaying)
        {
            spriteRenderer.sprite = hitHeads[currentHitStage];
        }
    }

    public void PlayEatAnimation(float duration, bool hasTomato = false)
    {
        if (eatingFrames == null || eatingFrames.Length == 0 || duration <= 0f)
            return;

        if (hasTomato && eatingFrames.Length < 2)
            return;

        eatAnimationDuration = duration;
        eatAnimationTime = 0f;
        isEatAnimationPlaying = true;
        isChewing = hasTomato;

        spriteRenderer.sprite = eatingFrames[hasTomato ? 1 : 0];
    }
}
