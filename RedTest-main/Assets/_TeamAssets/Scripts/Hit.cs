using RedDistrict.FinalCharacterContoller;
using RedDistrict.FinalCharacterController;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;


public class Hit : MonoBehaviour
{
    public PlayerCombo _playerCombo;

    [Header("Hit Effects")]
    [SerializeField] private GameObject _hitPrefab;
    [SerializeField] private GameObject _criticalHitPrefab;
    [SerializeField] private GameObject _textHit;
    [SerializeField] private GameObject _textCriticalHit;

    [Header("Camera Shake")]
    public float _shakeMagnitudeX = 0.15f;
    public float _shakeMagnitudeY = 0.1f;

    [Header("Slow Motion")]
    public float _duration = 0.2f;
    public float _slowAmount = 0.5f;

    [Header("Special Bar")]
    public float specialBar = 0f;
    public float specialIncreaseNormal = 10f;
    public float specialIncreaseCritical = 20f;
    public AudioClip specialFullSound;
    public GameObject specialFull;

    public bool specialFullPlayed = false;
    private UIManager _uiManager;
    private AudioSource audioSource;
    SpecialAttack _specialAttack;

    private void Awake()
    {
        _playerCombo = GetComponentInParent<PlayerCombo>();
        _uiManager = FindAnyObjectByType<UIManager>();
        audioSource = FindFirstObjectByType<AudioSource>();
        _specialAttack = FindFirstObjectByType<SpecialAttack>();
    }

    private void OnTriggerEnter(Collider enemy)
    {
        if (enemy.CompareTag("Enemy"))
        {
            Debug.Log("Colidiu com o ninimigo");
            _playerCombo.PlayHitSound();

            //Determina se é um golpe crítico
            bool isCritical = Random.value <= 0.1f; //10% de chance

            if (!isCritical && !_specialAttack._isSpecialAttacking)
            {
                HitEffect(enemy); //instancia o efeito de Hit
                CameraShake.StartShake(_shakeMagnitudeX, _shakeMagnitudeY); //Ativa o efeito de camera shake
            }

            if (isCritical && !_specialAttack._isSpecialAttacking)
            {
                Debug.Log("GOLPE CRÍTICO!");
                CameraShake.StartShake(_shakeMagnitudeX * 2, _shakeMagnitudeY * 2); //Shake mais forte para críticos
                PlayCriticalEffect(enemy); //Partícula ou efeito visual

                //Ativar Slow Motion
                StartCoroutine(ApplySlowMotion(_duration, _slowAmount));

                //Tocar o som de golpe critico
                _playerCombo.PlayCriticalHitSound();
            }

            if (_specialAttack._isSpecialAttacking)
            {
                Debug.Log("GOLPE CRÍTICO!");
                CameraShake.StartShake(_shakeMagnitudeX * 1.5f, _shakeMagnitudeY * 1.5f); //Shake mais forte para críticos
                PlayCriticalEffect(enemy); //Partícula ou efeito visual

                //Ativar Slow Motion
                StartCoroutine(ApplySlowMotion(_duration * 0.3f, _slowAmount * 0.3f));

                //Tocar o som de golpe critico
                _playerCombo.PlayCriticalHitSound();
            }

            //Atualizar barra de especial
            IncreaseSpecialBar(isCritical);
        }
    }

    private void HitEffect(Collider enemy)
    {
        if (_hitPrefab != null)
        {
            Vector3 hitPosition = enemy.ClosestPoint(transform.position); //Encontra o ponto mais próximo da colisão

            //Instancia os prefabs e guarda em variaveis
            GameObject hitInstance = Instantiate(_hitPrefab, hitPosition, Quaternion.identity);
            GameObject textInstance = Instantiate(_textHit, hitPosition, Quaternion.identity);

            //destrois as instancias após 1 segundo
            Destroy(hitInstance, 1f);
            Destroy(textInstance, 1f);
        }
    }

    private void PlayCriticalEffect(Collider enemy)
    {
        if (_criticalHitPrefab != null)
        {
            Vector3 hitPosition = enemy.ClosestPoint(transform.position);

            //Instancia os prefabs
            GameObject criticalInstance = Instantiate(_criticalHitPrefab, hitPosition, Quaternion.identity);
            GameObject textCriticalInstance = Instantiate(_textCriticalHit, hitPosition, Quaternion.identity);

            //Destroi as instancias após  segundo
            Destroy(criticalInstance, 1f);
            Destroy(textCriticalInstance, 1f);
        }
    }

    public IEnumerator ApplySlowMotion(float duration, float slowAmount)
    {
        Time.timeScale = slowAmount; //Diminui a velocidade do tempo
        Time.fixedDeltaTime = 0.02f * Time.timeScale; //Ajusta a física ao novo timeScale

        yield return new WaitForSecondsRealtime(duration); //Espera o tempo real

        Time.timeScale = 1f; //Restaura o tempo ao normal
        Time.fixedDeltaTime = 0.02f; //Reseta a física ao normal
    }

    public void IncreaseSpecialBar(bool isCritical)
    {
        float increaseAmount = isCritical ? specialIncreaseCritical : specialIncreaseNormal;

        //não aumentar mais que 100
        if (specialBar >= 100f && !_specialAttack._isSpecialAttacking) return;

        specialBar += increaseAmount;
        specialBar = Mathf.Clamp(specialBar, 0f, 100f); //Garante que não passe de 100%

        //Atualizar UI
        _uiManager?.UpdateSpecialBar(specialBar / 100f);

        //Se a barra ficou cheia, tocar som de feedback
        if (specialBar >= 100f && !specialFullPlayed)
        {
            specialFullPlayed = true; //Evita tocar mais de uma vez
            if (specialFullSound != null)
            {
                specialFull.gameObject.SetActive(true);
                audioSource.PlayOneShot(specialFullSound);
            }
            Debug.Log("ESPECIAL PRONTO!");
        }
    }
}