using System;
using TarodevController;
using Unity.Mathematics;
using Unity.XR.Oculus.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.Universal;
using static Unity.VisualScripting.Member;

public class CombatController : MonoBehaviour
{
    private CharacterMovement movenet;
    [SerializeField] private LayerMask characterLayerMask;
    [SerializeField] public bool _RangeAttack = false;
    [SerializeField] public bool _HeavyAttack = false;

    private ProjactileLogic plg;
    [Header("Projectle Logic")]
    public ObjectPool amunition;

    public Action OnRegularAttack;
    public Action OnHeavyAttackCharge;
    public Action<bool> OnHeavyAttackRelease;

    public ObjectPool Amunition
    {
        set
        {
            amunition = value;
            plg = Amunition.prefabe.GetComponent<ProjactileLogic>();
        }
        get
        {
            return amunition;
        }
    }

    [Header("Heavy Attack")]
    [SerializeField] private float heavyChargeTime = 1.2f;
    [SerializeField] private float heavyCooldown = 2.5f;
    [SerializeField] private float heavyDamage = 35f;
    [SerializeField] private float heavyRange = 1.8f;

    private float heavyChargeStart;
    private float lastHeavyAttackTime = -999f;
    public bool isChargingHeavy { get; private set; }



    void Awake()
    {
        movenet = GetComponent<CharacterMovement>();
        OnHeavyAttackCharge = () => { };
        OnHeavyAttackRelease = (a) => { };
    }
    private void OnDisable()
    {
        isChargingHeavy = false;
    }

    #region Attack Interface
    public void Attack(InputAction.CallbackContext ctx)
    {
        Attack();
    }
    public void Attack()
    {
        if (_RangeAttack) RangeAttack();
        else RegularAttack();
    }

    public void HeavyAttack(InputAction.CallbackContext ctx)
    {
        if (!_HeavyAttack)
        {
            return;
        }
        // Button pressed
        if (ctx.started)
        {
            StartHeavyCharge();
        }

        // Button released
        if (ctx.canceled)
        {
            ReleaseHeavyAttack();
        }
    }
    #endregion
    #region RegularAttack
    private void RegularAttack()
    {
        Debug.Log("Attack");
        OnRegularAttack.Invoke();
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            this.transform.position + (movenet.direction.x > 0f ? Vector3.right : Vector3.left),
            1f,
            characterLayerMask
        );

        movenet.NuckRepale(movenet.direction.x > 0f ? Vector3.right : Vector3.left, 8f);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("Hit: " + hit.name);
            Damageable<float> dmg = hit.gameObject.GetComponent<Damageable<float>>();
            if (dmg != null && hit.gameObject != this)
            {
                dmg.OnDamage(10f);
            }
        }

    }
    #endregion
    #region RangedAttack
    private void RangeAttack()
    {
        GameObject bullet = Amunition.GetInstance(this.transform.position);
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseScreenPos3 = new Vector3(
            mouseScreenPos.x,
            mouseScreenPos.y,
            Mathf.Abs(Camera.main.transform.position.z)
        );

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos3);
        ProjactileLogic plg = bullet.GetComponent<ProjactileLogic>();
        plg.Damage = this.plg.Damage;
        plg.SetTarget(mouseWorldPos, this.gameObject);
    }
    #endregion
    #region HeavyAttack
    private void StartHeavyCharge()
    {
        if (Time.time < lastHeavyAttackTime + heavyCooldown)
            return;

        isChargingHeavy = true;
        heavyChargeStart = Time.time;

        Debug.Log("Heavy attack charging...");
        OnHeavyAttackCharge.Invoke();
    }

    private void ReleaseHeavyAttack()
    {
        if (!isChargingHeavy)
            return;

        isChargingHeavy = false;

        float chargeTime = Time.time - heavyChargeStart;

        if (chargeTime < heavyChargeTime)
        {
            Debug.Log("Heavy attack canceled (not charged)");
            OnHeavyAttackRelease.Invoke(false);
            return;
        }

        ExecuteHeavyAttack();
    }
    private void ExecuteHeavyAttack()
    {
        lastHeavyAttackTime = Time.time;

        Debug.Log("HEAVY ATTACK!");
        OnHeavyAttackRelease.Invoke(true);
        movenet.NuckRepale(movenet.direction.x > 0f ? Vector3.right : Vector3.left, 12f);
        Vector3 offset =
            movenet.direction.x > 0f ? Vector3.right : Vector3.left;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position + offset,
            heavyRange,
            characterLayerMask
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Damageable<float> dmg = hit.GetComponent<Damageable<float>>();
            if (dmg != null)
            {
                dmg.OnDamage(heavyDamage);
            }
        }
    }

    #endregion

}