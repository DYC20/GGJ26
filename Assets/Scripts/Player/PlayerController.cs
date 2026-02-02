using System;
using TarodevController;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(MaskController))]
[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CombatController))]
public class PlayerController : MonoBehaviour, Damageable<DamageLog>
{
    [SerializeField] FloatVariable PlayerHealth;

    private InputSystem_Actions playerInput;
    private FrameInput _frameInput;
    private SmartSwitch jumpSwtich;
    private SmartSwitch dashSwitch;
    private CharacterMovement controller;
    private CombatController combatController;
    private MaskController maskController;

    private void Awake()
    {
        PlayerHealth.value = 1000;
        controller = GetComponent<CharacterMovement>();
        combatController = GetComponent<CombatController>();
        maskController = GetComponent<MaskController>();

        jumpSwtich = new SmartSwitch();
        _frameInput = new FrameInput()
        {
            JumpDown = false,
            JumpHeld = false,
            Move = Vector2.zero
        };
        playerInput = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
        playerInput.Player.Attack.performed += combatController.Attack;
        playerInput.Player.Interact.performed += maskController.CollectMask;

        playerInput.Player.Switch.performed += maskController.ChangeMasks;


        playerInput.Player.Powerup.started += combatController.HeavyAttack;
        playerInput.Player.Powerup.canceled += combatController.HeavyAttack;


    }
    private void OnDisable()
    {
        playerInput.Player.Disable();
        playerInput.Player.Attack.performed -= combatController.Attack;
        playerInput.Player.Interact.started -= maskController.CollectMask;
        playerInput.Player.Switch.started -= maskController.ChangeMasks;

        playerInput.Player.Powerup.started -= combatController.HeavyAttack;
        playerInput.Player.Powerup.canceled -= combatController.HeavyAttack;

    }
    private void GatherInput()
    {
        jumpSwtich.Update(playerInput.Player.Jump.IsPressed());
        dashSwitch.Update(playerInput.Player.Powerup.IsPressed());
        if (jumpSwtich.OnPress())
        {
            Debug.Log("Jumped");
        }
        _frameInput = new FrameInput
        {
            JumpDown = jumpSwtich.OnPress(),
            JumpHeld = jumpSwtich.OnHold(),
            Move = playerInput.Player.Move.ReadValue<Vector2>()
        };

        controller.SetInput(
            playerInput.Player.Move.ReadValue<Vector2>(),
            jumpSwtich.OnPress(),
            jumpSwtich.OnHold(),
            dashSwitch.OnPress()
        );
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        GatherInput();
    }

    public event Action PlayerIsDead;
    public void OnDamage(DamageLog log)
    {
        PlayerHealth.value -= log.damageAmount;

        if (PlayerHealth.value < 0f)
        {
            PlayerIsDead?.Invoke();
            this.gameObject.SetActive(false);
        }
    }

    public bool IsDead()
    {
        //throw new System.NotImplementedException();
        return false;
    }
}
