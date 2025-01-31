using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 _originalPosition; //Posição original da camera
    public static CameraShake _instance;

    private void Awake()
    {
        _instance = this;
        _originalPosition = transform.localPosition;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = _originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _originalPosition;
    }

    public static void StartShake(float duration, float magnitude)
    {
        if (_instance != null)
            _instance.StartCoroutine(_instance.Shake(duration, magnitude));
    }
}
