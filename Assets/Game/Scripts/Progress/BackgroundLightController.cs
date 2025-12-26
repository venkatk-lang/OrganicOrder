using UnityEngine;

public class BackgroundLightController : MonoBehaviour
{
    public SpriteRenderer background;
    public Color startColor = Color.white;
    public Color endColor = new Color(0.6f, 0.6f, 0.6f);

    public TrialTimer timer;
    public float maxFadeTime = 12f;

    void Update()
    {
        float t = timer.NormalizedTime(maxFadeTime);
        background.color = Color.Lerp(startColor, endColor, t);
    }

    public void ResetLight()
    {
        background.color = startColor;
    }
}
