using TarodevController;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(MaskHandler))]
[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CombatController))]
public class PlayerController : MonoBehaviour, Damageable<float>
{
    public InputSystem_Actions playerInput;
    private FrameInput _frameInput;
    private SmartSwitch jumpSwtich;
    private SmartSwitch dashSwitch;
    private CharacterMovement controller;
    private CombatController combatController;

    private void Awake()
    {
        controller = GetComponent<CharacterMovement>();
        combatController = GetComponent<CombatController>();

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
        playerInput.Player.Powerup.started += combatController.HeavyAttack;
        playerInput.Player.Powerup.canceled += combatController.HeavyAttack;


    }
    private void OnDisable()
    {
        playerInput.Player.Disable();
        playerInput.Player.Attack.performed -= combatController.Attack;
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
    void Update()
    {
        GatherInput();
    }

    public void OnDamage(float log)
    {
        //throw new System.NotImplementedException();
    }

    public bool IsDead()
    {
        //throw new System.NotImplementedException();
        return false;
    }
}
