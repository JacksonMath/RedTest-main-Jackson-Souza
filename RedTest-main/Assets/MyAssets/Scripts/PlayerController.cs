using RedDistrict.FinalCharacterController;
using UnityEngine;

namespace RedDistrict.FinalCharacterContoller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;

        public float runAcceleration = 0.25f;
        public float runSpeed = 4f;
        public float drag = 0.1f;

        private PlayerLocomotionInput _playerLocomotionInput;
        private Animator _playerAnimator;
        private PlayerCombo _playerCombo;

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerAnimator = GetComponentInChildren<Animator>();
            _playerCombo = GetComponent<PlayerCombo>();
        }

        private void Update()
        {
            if (_playerCombo != null && !_playerCombo._isAttacking)
            {
                Vector3 movementDirection = Vector3.right * _playerLocomotionInput.MovementInput.x +
                                            Vector3.forward * _playerLocomotionInput.MovementInput.y;

                Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
                Vector3 newVelocity = (_characterController.velocity + movementDelta);

                // Adiciona o drag ao player
                Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
                newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
                newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

                // Move o personagem
                _characterController.Move(newVelocity * Time.deltaTime);

                // Converte a velocidade global para a velocidade no referencial local do player
                Vector3 localVelocity = transform.InverseTransformDirection(newVelocity);

                // Atualiza os valores no Animator
                _playerAnimator.SetFloat("Speed_X", localVelocity.x);
                _playerAnimator.SetFloat("Speed_Z", localVelocity.z);
            }
        }
    }
}
