using RedDistrict.FinalCharacterContoller;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace RedDistrict.FinalCharacterController
{
    public class PlayerLocomotionInput : MonoBehaviour, InputSystem_Actions.IPlayerMapActions
    {
        public InputSystem_Actions InputSystem_Actions { get; private set; }
        public Vector2  MovementInput {  get; private set; }
        public Toggle ui;

        private PlayerCombo _playerCombo;
        private SpecialAttack _specialAttack;

        private void OnEnable()
        {
            InputSystem_Actions = new InputSystem_Actions();
            InputSystem_Actions.Enable(); //habilita o input system

            InputSystem_Actions.PlayerMap.Enable(); //habilita o mapeamento de teclas e controles
            InputSystem_Actions.PlayerMap.SetCallbacks(this); //retorno de chamada para esta classe
        }

        private void OnDisable()
        {
            InputSystem_Actions.PlayerMap.Disable(); //desabilita o mapeamento de teclas e controles
            InputSystem_Actions.PlayerMap.RemoveCallbacks(this); //remove o retorno de chamadas da classe quando não usada
        }

        private void Awake()
        {
            //Obtém o componente PlayerCombo
            _playerCombo = GetComponent<PlayerCombo>();
            _specialAttack = GetComponent<SpecialAttack>();
        }

        #region Metodos Para a Interface do Input System
        public void OnAttack(InputAction.CallbackContext context)
        {
            //Verifica se a ação foi iniciada
            if (context.started)
            {
                if (_specialAttack._isSpecialAttacking && _specialAttack._canAttack)
                {
                    //Chama o sistema de combo no script SpecialAttack
                    _specialAttack?.PerformSpecialAttack();
                }
                else
                {
                    //Chama o sistema de combo no script PlayerCombo
                    _playerCombo?.PerformCombo();
                }
            }
        }

        public void OnSpecial(InputAction.CallbackContext context)
        {
            //Verifica se a ação foi iniciada
            if (context.started)
            {
                //Ativa o sistema de Special Attack
                _specialAttack.SpecialMode();
            }
        }

        public void OnFullPower(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _specialAttack.FullPower();
                Debug.Log("FulPower");
            }
        }

        public void OnResume(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ui.isOn = !ui.isOn;
            }
        }
        public void OnMove(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }
        #endregion  


    }
}
