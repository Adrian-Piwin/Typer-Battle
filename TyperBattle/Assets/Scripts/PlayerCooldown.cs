using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCooldown : MonoBehaviour
{

    [SerializeField]
    private Transform cooldownIndicator;

    [SerializeField]
    private float startingIndicatorSize;

    private PlayerCommands playerCommands;

    private void Start()
    {
        playerCommands = GetComponent<PlayerCommands>();
    }

    public void ApplyCooldown(float time) 
    {
        playerCommands.onCooldown = true;
        StartCoroutine(ScaleToTargetCoroutine(new Vector2(0, cooldownIndicator.localScale.y), time));
    }

    private IEnumerator ScaleToTargetCoroutine(Vector2 targetScale, float duration)
    {
        Vector2 startScale = new Vector2(startingIndicatorSize, cooldownIndicator.localScale.y);
        float timer = 0.0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            //smoother step algorithm
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            cooldownIndicator.localScale = Vector2.Lerp(startScale, targetScale, t);
            yield return null;
        }

        yield return null;
        playerCommands.onCooldown = false;
    }
}
