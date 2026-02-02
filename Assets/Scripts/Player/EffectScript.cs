using TarodevController;
using UnityEngine;
using UnityEngine.VFX;

public class EffectScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;

    [Header("Movement Effects")]
    [SerializeField] private ParticleSystem walkingParticles;
    [SerializeField] private ParticleSystem landingParticles;

    [Header("Combat Effects")]
    [SerializeField] private VisualEffect attackVsualEffectLeft;
    [SerializeField] private VisualEffect attackVsualEffectRight;


    [Header("Powerups Effects")]
    [SerializeField] private VisualEffect DashVisualEffect;

    [SerializeField] private VisualEffect HeavyattackChargeVsualEffect;
    [SerializeField] private VisualEffect HeavyattackChargeHoldVsualEffect;

    [SerializeField] private VisualEffect HeavyattackVsualEffectLeft;
    [SerializeField] private VisualEffect HeavyattackVsualEffectRight;


    #region Depndences
    private CharacterMovement characterMovement;
    private CombatController combatcontroller;
    private MaskHandler maskhandler;
    private Animator animator;
    #endregion

    #region flags
    private bool _isCharacterMovement;
    private bool _isCombatController;
    private bool _isMaskHandler;
    private bool _isAnimator;
    #endregion

    SmartSwitch landingSwitch;

    private void Awake()
    {
        _isCharacterMovement = TryGetComponent<CharacterMovement>(out characterMovement);
        _isCombatController = TryGetComponent<CombatController>(out combatcontroller);
        _isMaskHandler = TryGetComponent<MaskHandler>(out maskhandler);
        animator = GetComponentInChildren<Animator>();
        _isAnimator = animator != null;

        if(_isMaskHandler) maskhandler.maskChanged += OnChageSprite;

    }

    #region Movement Effects

    public void OnChageSprite()
    {
        if (maskhandler.mask != null)
        {
            sprite.color = maskhandler.mask.Color;
        }
    }    

    private void initMovementEffects()
    {
        if (_isCharacterMovement)
        {
            walkingParticles.Play();
        }
    }

    SmartSwitch runswitch;
    private void MoventHandler()
    {
        if (_isCharacterMovement)
        {
            Vector3 dir = characterMovement.direction.x > 0f ? Vector3.right : Vector3.left;
            float speed = characterMovement.speed;
            landingSwitch.Update(characterMovement._grounded);


            if(characterMovement._grounded)
            {
                walkingParticles.Play();
            }else
            {
                walkingParticles.Stop();
            }

            if(landingSwitch.OnPress())
            {
                landingParticles.Play();
            }

            if (_isAnimator)
            {
                sprite.flipX = characterMovement.direction.x <= 0f;
                if (characterMovement._grounded)
                {
                    runswitch.Update(Mathf.Abs(characterMovement._rb.linearVelocity.magnitude) > 0.1f);
                    if (runswitch.OnPress())
                    {
                        animator.Play("Run");
                    }
                    if (runswitch.OnRelese())
                    {
                        animator.Play("Idle");
                    }
                }
                else
                {
                    //in the air
                    if(characterMovement._rb.linearVelocity.y > 0f)
                    {
                        animator.Play("Jump");
                    }
                    else
                    {
                        animator.Play("Falling");
                    }

                }
            }
        }

        
    }
    #endregion

    #region Combat Effects

    private void initCombatEffects()
    {
        if (_isCombatController)
        {
            combatcontroller.OnRegularAttack += () => {
                if (animator) animator.Play("Attack");
                Vector3 dir = characterMovement.direction.x > 0f ? Vector3.right : Vector3.left;
                if (Vector3.Dot(dir, Vector3.right) > 0.9f)
                {
                    if(attackVsualEffectRight.gameObject.activeSelf == false) attackVsualEffectRight.gameObject.SetActive(true);
                    attackVsualEffectRight.Play();
                }
                else
                {
                    if (attackVsualEffectLeft.gameObject.activeSelf == false) attackVsualEffectLeft.gameObject.SetActive(true);
                    attackVsualEffectLeft.Play();
                }
            };
        }
    }
    private void CombatEffectsHandler()
    {
        if (_isCombatController)
        {
            
        }
    }
    #endregion

    #region Powerups

    private void initPowerupEffects()
    {
        if (_isCombatController)
        {
            characterMovement.OnDash += () => {
                Vector3 dir = characterMovement.direction.x > 0f ? Vector3.right : Vector3.left;
                if (DashVisualEffect.gameObject.activeSelf == false) DashVisualEffect.gameObject.SetActive(true);
                if (Vector3.Dot(dir, Vector3.right) > 0.9f)
                {
                    DashVisualEffect.gameObject.transform.localScale = new Vector3(1f,1f,1f);
                    DashVisualEffect.Play();
                }
                else
                {
                    DashVisualEffect.gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
                    DashVisualEffect.Play();
                }
            };

            combatcontroller.OnHeavyAttackCharge += () =>
            {
                if (HeavyattackChargeVsualEffect.gameObject.activeSelf == false) HeavyattackChargeVsualEffect.gameObject.SetActive(true);
                HeavyattackChargeVsualEffect.Play();
            };
            combatcontroller.OnHeavyAttackRelease += (a) =>
            {
                if (a == true)
                {
                    if(animator) animator.Play("Attack");
                    Vector3 dir = characterMovement.direction.x > 0f ? Vector3.right : Vector3.left;
                    if (Vector3.Dot(dir, Vector3.right) > 0.9f)
                    {
                        if (HeavyattackVsualEffectRight.gameObject.activeSelf == false) HeavyattackVsualEffectRight.gameObject.SetActive(true);
                        HeavyattackVsualEffectRight.Play();
                    }
                    else
                    {
                        if (HeavyattackVsualEffectLeft.gameObject.activeSelf == false) HeavyattackVsualEffectLeft.gameObject.SetActive(true);
                        HeavyattackVsualEffectLeft.Play();
                    }
                }
            };
        }
    }

    float t_H = 0;
    private void PowerupHandler()
    {
        if (_isCombatController)
        {
            if (combatcontroller.isChargingHeavy)
            {
                t_H += Time.deltaTime;
                if (t_H > 0.2f)
                {
                    if (HeavyattackChargeHoldVsualEffect.gameObject.activeSelf == false) HeavyattackChargeHoldVsualEffect.gameObject.SetActive(true);
                    HeavyattackChargeHoldVsualEffect.Play();
                    t_H = 0f;
                }
            }
        }
    }

    #endregion
    private void Start()
    {
        initMovementEffects();
        initCombatEffects();
        initPowerupEffects();
    }

    private void Update()
    {
        MoventHandler();
        CombatEffectsHandler();
        PowerupHandler();
    }

}
