using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace RedDistrict.FinalCharacterController
{
    public class SpecialAttack : MonoBehaviour
    {
        [Header("Special Settings")]
        [SerializeField] private float _SpecialResetTime = 2f; // Valor padrão
        [SerializeField] private GameObject _specialCamera;
        private int _currentComboStep = 0;
        private float _lastSpecialTime = 0f;
        public bool _canAttack = false; //variavel para atacar após a animação de special mode
        public bool _isSpecialAttacking = false;
        public bool _isComboAttacking = false;

        [Header("Cooldown Settings")]
        [SerializeField] private float _specialCooldown = 0.2f;

        private Hit _hit;
        private AudioSource _audioSource;
        private Animator _playerAnimator;
        private Coroutine _drainCoroutine;
        private Material[][] _originalMaterials; //Materiais originais
        private FullscreenEffectToggle _fullscreenEffectToggle;

        [Header("Hit Settings")]
        [SerializeField] private GameObject _hitCollider;

        [Header("Special Bar Settings")]
        [SerializeField] private float _specialDrainDuration = 15f; //Tempo para esgotar a barra
        [SerializeField] private Image _specialBarImage;

        [Header("Special Effect Settings")]
        [SerializeField] private SkinnedMeshRenderer[] _affectedMeshes; //Objetos com outline
        [SerializeField] private Material _specialMaterial; //Material do outline
        [SerializeField] private GameObject _particlesSpecial; //Particulas do modo Special
        [SerializeField] private GameObject _particlesAura; //Particulas de aura do modo Special
        [SerializeField] private AudioClip _specialSound; //Efeito para entrar no modo especial

        private readonly string[] _specialComboTriggers =
        {
            "Special_Combo_01", "Special_Combo_02", "Special_Combo_03",
            "Special_Combo_04", "Special_Combo_05", "Special_Combo_06",
            "Special_Combo_07", "Special_Combo_08", "Special_Combo_09",
            "Special_Combo_10"
        };

        private void Awake()
        {
            _hit = GetComponentInChildren<Hit>();
            _playerAnimator = GetComponent<Animator>();
            _audioSource = FindFirstObjectByType<AudioSource>();
            _fullscreenEffectToggle = GetComponent<FullscreenEffectToggle>();

            //Salva os materiais originais
            _originalMaterials = new Material[_affectedMeshes.Length][];
            for (int i = 0; i < _affectedMeshes.Length; i++)
            {
                _originalMaterials[i] = _affectedMeshes[i].materials;
            }
        }

        public void SpecialMode()
        {
            if (!_isSpecialAttacking && _hit.specialFullPlayed)
            {
                _isSpecialAttacking = true;
                _playerAnimator.SetTrigger("SpecialAttack");
                _specialCamera.SetActive(true);
            }
        }

        private IEnumerator DrainSpecialBar()
        {
            float startValue = _hit.specialBar;
            float elapsedTime = 0f;

            while (elapsedTime < _specialDrainDuration)
            {
                if (!_isSpecialAttacking) yield break; //Para se o especial for cancelado

                _hit.specialBar = Mathf.Lerp(startValue, 0, elapsedTime / _specialDrainDuration);
                UpdateSpecialBarUI(); //Atualiza a barra na UI
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _hit.specialBar = 0; //Garante que a barra chegue a zero
            _isSpecialAttacking = false; //Desativa o especial quando a barra acabar
            RemoveSpecialMaterial(); //Remove o material especial quando o especial termina
            _particlesSpecial.SetActive(false); //Desativa as particulas do especial
            _particlesAura.SetActive(false); //Desativa as particulas de aura
            _canAttack = false; //Desativa o _canAttack para a animação poder ativa-lo novamete
            _hit.specialFull.gameObject.SetActive(false); //Desativa o efeito da barra de special
            _fullscreenEffectToggle.DisableFullscreenEffect(); //Desabilita o efeito fullscreen
            _hit.specialFullPlayed = false; //Garante que o special não é ativo quando a barra zerar

        }

        private void UpdateSpecialBarUI()
        {
            if (_specialBarImage != null)
            {
                _specialBarImage.fillAmount = _hit.specialBar / 100f; //Normaliza o valor entre 0 e 1
            }
        }

        public void ApplySpecialMaterial()
        {
            for (int i = 0; i < _affectedMeshes.Length; i++)
            {
                Material[] newMaterials = new Material[_originalMaterials[i].Length + 1];

                //Copia os materiais originais
                for (int j = 0; j < _originalMaterials[i].Length; j++)
                {
                    newMaterials[j] = _originalMaterials[i][j];
                }

                //Adiciona o material especial na posição 1
                newMaterials[1] = _specialMaterial;
                _affectedMeshes[i].materials = newMaterials;
            }

            CameraShake.StartShake(.05f, .07f);

            //Ativa as particulas
            _particlesAura.SetActive(true);

            //Inicia a drenagem da barra de especial
            if (_drainCoroutine != null) StopCoroutine(_drainCoroutine);
            _drainCoroutine = StartCoroutine(DrainSpecialBar());

            _audioSource.PlayOneShot(_specialSound);
        }

        private void RemoveSpecialMaterial()
        {
            for (int i = 0; i < _affectedMeshes.Length; i++)
            {
                _affectedMeshes[i].materials = _originalMaterials[i]; //Restaura os materiais originais
            }
        }

        public void PerformSpecialAttack()
        {
            if (Time.time - _lastSpecialTime < _specialCooldown) return; //Cooldown ativo

            _lastSpecialTime = Time.time;

            ResetTriggers();

            if (_currentComboStep < _specialComboTriggers.Length)
            {
                _playerAnimator.SetTrigger(_specialComboTriggers[_currentComboStep]);
                _currentComboStep++;
                _isComboAttacking = true;

                CancelInvoke(nameof(ResetSpecialCombo));
                Invoke(nameof(ResetSpecialCombo), _SpecialResetTime);
            }
            else
            {
                ResetSpecialCombo();
            }
        }

        public void ResetSpecialCombo()
        {
            ResetTriggers();
            _playerAnimator.SetBool("isAttacking", false);
            _currentComboStep = 0;
            _isComboAttacking = false;
        }

        private void ResetTriggers()
        {
            foreach (string trigger in _specialComboTriggers)
            {
                _playerAnimator.ResetTrigger(trigger);
            }
        }

        public void AtivarHitCollider()
        {
            _hitCollider.GetComponent<Collider>().enabled = true;
        }

        //desativa a hit box
        public void DesativarHitCollider()
        {
            _hitCollider.GetComponent<Collider>().enabled = false;
        }

        //método para ativar o som e particula do special no inicio da animação
        public void SpecialSound()
        {
            _audioSource.PlayOneShot(_hit.specialFullSound);
            _particlesSpecial.SetActive(true);

            //Ativar Slow Motion
            _hit.StartCoroutine(_hit.ApplySlowMotion(.5f, .5f));
        }

        //Método para ativar o _canAttack
        public void CanAttack()
        {
            _canAttack = true; //player pode atacar
            _hit.specialFullPlayed = false; //garante que a barra de special não está mais full
            _specialCamera.SetActive(false); //ativa a camera de special
        }

        public void FullPower() //Metodo para encher a barra de especial
        {
            _hit.specialBar = +100f;
            if (_hit.specialBar >= 100f && !_hit.specialFullPlayed)
            {
                _hit.specialFullPlayed = true; //Evita tocar mais de uma vez
                if (_hit.specialFullSound != null)
                {
                    _hit.specialFull.gameObject.SetActive(true);
                    _audioSource.PlayOneShot(_hit.specialFullSound);
                }
                Debug.Log("ESPECIAL PRONTO!");
            }
            UpdateSpecialBarUI();
        }
    }
}