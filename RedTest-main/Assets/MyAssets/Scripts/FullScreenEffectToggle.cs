using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullscreenEffectToggle : MonoBehaviour
{
    public ScriptableRendererFeature fullscreenEffect;

    public void EnableFullscreenEffect()
    {
        if (fullscreenEffect != null)
            fullscreenEffect.SetActive(true);
    }

    public void DisableFullscreenEffect()
    {
        if (fullscreenEffect != null)
            fullscreenEffect.SetActive(false);
    }
}
