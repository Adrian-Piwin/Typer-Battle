using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCooldown : NetworkBehaviour
{

    [SerializeField]
    private Transform cooldownIndicator;

    [SerializeField]
    private float startingIndicatorSize;

    private PlayerCommands playerCommands;
    private IEnumerator cooldownCoroutine;

    private void Start()
    {
        playerCommands = GetComponent<PlayerCommands>();
    }

    public void ApplyCooldown(float time) 
    {
        // Stop prev cooldown if any
        if (cooldownCoroutine != null)
            StopCoroutine(cooldownCoroutine);

        // Go on cooldown
        playerCommands.onCooldown = true;
        cooldownCoroutine = ScaleToTargetCoroutine(new Vector2(0, cooldownIndicator.localScale.y), time);
        StartCoroutine(cooldownCoroutine);
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
