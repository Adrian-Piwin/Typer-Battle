using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UIHealth : NetworkBehaviour
{
    public bool isAssigned;

    [SerializeField]
    private float speed;

    public void TakeDamage(float percent)
    {
        StartCoroutine(ScaleToTargetCoroutine(new Vector2(percent, transform.localScale.y), speed));
    }

    private IEnumerator ScaleToTargetCoroutine(Vector2 targetScale, float duration)
    {
        Vector2 startScale = transform.localScale;
        float timer = 0.0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            //smoother step algorithm
            t = t * t * t * (t * (6f * t - 15f) + 10f);
            transform.localScale = Vector2.Lerp(startScale, targetScale, t);
            yield return null;
        }

        yield return null;
    }
}
