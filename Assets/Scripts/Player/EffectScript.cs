using TarodevController;
using UnityEngine;
using UnityEngine.VFX;

public class EffectScript : MonoBehaviour
{
    [Header("Movement Effects")]
    [SerializeField] private ParticleSystem walkingParticles;
    [SerializeField] private ParticleSystem landingParticles;

    [Header("Combat Effects")]
    [SerializeField] private VisualEffect attackVsualEffectLeft;
    [SerializeField] private VisualEffect attackVsualEffectRight;


    [Header("Powerups Effects")]
    [SerializeField] private VisualEffect DashVisualEffect;
    [SerializeField] private VisualEffect HeavyAttackVisualEffect;


    #region Depndences
    private CharacterMovement characterMovement;
    private CombatController combatcontroller;
    #endregion

    #region flags
    private bool _isCharacterMovement;
    private bool _isCombatController;
    #endregion

    SmartSwitch landingSwitch;

    private void Awake()
    {
        _isCharacterMovement = TryGetComponent<CharacterMovement>(out characterMovement);
        _isCombatController = TryGetComponent<CombatController>(out combatcontroller);


    }

    #region Movement Effects

    private void initMovementEffects()
    {
        if (_isCharacterMovement)
        {
            walkingParticles.Play();
        }
    }
    private void MoventHandler()
    {
        if (_isCharacterMovement)
        {
            Vector3 dir = characterMovement.direction.x > 0f ? Vector3.right : Vector3.left;
            float speed = characterMovement.speed;
            landingSwitch.Update(characterMovement._grounded);

            if(landingSwitch.OnPress())
            {
                landingParticles.Play();
            }

            if (Vector3.Dot(dir, Vector3.right) >0.9f)
            {
                walkingParticles.transform.eulerAngles = new Vector3(0f, 0f, 180f);
            }
            else
            {
                walkingParticles.transform.eulerAngles = new Vector3(0f, 0f, 0f);
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
    }

}
