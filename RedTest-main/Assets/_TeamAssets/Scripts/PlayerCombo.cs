using UnityEngine;

namespace RedDistrict.FinalCharacterContoller
{
    public class PlayerCombo : MonoBehaviour
    {
        [Header("Combo Settings")]
        [SerializeField] private float _comboResetTime = 1f; //Tempo para resetar o combo
        private int _currentComboStep = 0; //Fase atual do combo
        private float _lastAttackTime = 0f; //Tempo do ataque final
        public bool _isAttacking = false;

        [Header("Cooldown Settings")]
        [SerializeField] private float _attackCooldown = 0.5f;

        [Header("Attack Audios")]
        [SerializeField] private AudioSource _audioSource; //Fonte de áudio
        [SerializeField] private AudioClip[] _punch; //audio do golpe

        [Header("Audio Settings")]
        [SerializeField] private AudioClip[] _audioHit; //Audio do hit
        [SerializeField] private AudioClip _criticalHit; //Audio de golpe crítico

        [Header("Hit Settings")]
        [SerializeField] private GameObject _boxCollider;

        private Animator _playerAnimator;

        private void Awake()
        {
            _playerAnimator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (_currentComboStep > 0)
            {
                _playerAnimator.SetBool("isAttacking", true);
            }
            else if (_currentComboStep == 0)
            {
                _isAttacking = false;
                _playerAnimator.SetBool("isAttacking", false);
            }
        }

        //Executação do combo
        public void PerformCombo()
        {            
            //Verifica se está no cooldown
            if (Time.time - _lastAttackTime < _attackCooldown)
                return;

            //Atualiza o tempo do ataque
            _lastAttackTime = Time.time;

            //Reseta os triggers antes de ativar o próximo
            ResetAllTriggers();

            //Executa o combo com base no estado atual
            switch (_currentComboStep)
            {
                case 0:                   
                    if (!_isAttacking) //verifica se esta sendo realizado um ataque para dar o primeiro golpe
                    {
                        _isAttacking = true;
                        _playerAnimator.SetTrigger("Attack_1");
                        
                        _currentComboStep = 1;
                    }
                    break;

                case 1:
                    _playerAnimator.SetTrigger("Attack_2");
                    _currentComboStep = 2;
                    break;

                case 2:
                    _playerAnimator.SetTrigger("Attack_3");
                    _currentComboStep = 0; //Reseta o combo depois do ataque final
                    break;

                default:
                    _currentComboStep = 0;
                    _isAttacking = false;
                    break;
            }

            //Inicia o temporizador para dar reset
            CancelInvoke(nameof(ResetCombo));
            Invoke(nameof(ResetCombo), _comboResetTime);
        }

        //Método para tocar um som aleatório da lista _punch
        public void PlayAttackSound()
        {
            if (_punch.Length == 0 || _audioSource == null) return;

            int randomIndex = Random.Range(0, _punch.Length); //Escolhe um audio aleatório
            _audioSource.PlayOneShot(_punch[randomIndex]); //Reproduz o som escolhido
        }

        //Método para tocar um som aleatório da lista _audioHit
        public void PlayHitSound()
        {
            if (_audioHit.Length == 0 || _audioSource == null) return;

            int randomIndex = Random.Range(0, _audioHit.Length); //Escolhe um audio aleatório
            _audioSource.PlayOneShot(_audioHit[randomIndex]); //Reproduz o som escolhido
        }

        public void PlayCriticalHitSound()
        {
            _audioSource.PlayOneShot(_criticalHit);
        }

        //ativa a hit box
        public void AtivarCollider()
        {
            _boxCollider.GetComponent<Collider>().enabled = true;
        }

        //desativa a hit box
        public void DesativarCollider()
        {
            _boxCollider.GetComponent<Collider>().enabled = false;
        }

        //Metodo para resetar o combo
        public void ResetCombo()
        {
            ResetAllTriggers();
            _playerAnimator.SetBool("isAttacking", false);
            _currentComboStep = 0;
            _isAttacking = false;
        }

        //Metodo para resetar os triggers de animações de ataques
        private void ResetAllTriggers()
        {
            _playerAnimator.ResetTrigger("Attack_1");
            _playerAnimator.ResetTrigger("Attack_2");
            _playerAnimator.ResetTrigger("Attack_3");
        }

    }
}
