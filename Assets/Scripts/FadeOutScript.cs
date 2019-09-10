using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutScript : MonoBehaviour
{
    CanvasGroup uiElement;

    public void FadeIn()
    {
        uiElement = GetComponent<CanvasGroup>();
        StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 1, .5f));
    }

    public void FadeOut()
    {
        uiElement = GetComponent<CanvasGroup>();
        StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 0, .5f));
    }

    public IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 0.5f)
    {
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            cg.alpha = currentValue;

            if (percentageComplete >= 1) break;

            yield return new WaitForFixedUpdate();
        }
    }
}
