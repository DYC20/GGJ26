using System.Collections.Generic;
using TarodevController;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AICharacterController2D))]
[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CombatController))]
public class AIController : MonoBehaviour, Damageable<DamageLog>
{
    public float health = 100f;
    public float nuckforce = 50f;
    private enum States
    {
        IDLE,
        CHASE,
        ATTACK
    }
    [SerializeField] private States states;
    private States oldstates;
    private GameObject player;
    private GameObject target;
    private AICharacterController2D agent;
    private CharacterMovement movment;
    private CombatController combatController;
    private MaskHandler handler;
    private AiSensors sensors;
    [SerializeField] private LayerMask characterLayerMask;
    [SerializeField] private ObjectPool MaskPool;

    [SerializeField] private List<Mask> masks;

    private SmartSwitch stSwitch;
    private float t = 0f;
    private float t_A = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        movment = gameObject.GetComponent<CharacterMovement>();
        combatController = gameObject.GetComponent<CombatController>();
        handler = gameObject.GetComponent<MaskHandler>();
        sensors = GetComponent<AiSensors>();
        player = GameObject.FindGameObjectWithTag("Player");
        agent = gameObject.GetComponent<AICharacterController2D>();
        t = Random.value * 0.2f;
        stSwitch = new SmartSwitch();
        if (sensors) sensors.OnScan += checkState;
    }

    private void OnEnable()
    {
        ChangedMask();
    }

    private void SetStateGraph()
    {
        switch (states)
        {
            case States.IDLE:
                agent.EnableAgent(false);
                agent.ClearTarget();
                break;

            case States.CHASE:
                if (target != null)
                {
                    agent.EnableAgent(true);
                    agent.SetTarget(target.transform);
                }
                else
                {
                    states = States.IDLE;
                }
                break;

            case States.ATTACK:
                Debug.Log("Attack");
                agent.EnableAgent(false);
                if (combatController != null)
                {
                    combatController.Attack();
                }
                break;

        }
    }

    private void CheckEnviorment()
    {
        // First check for player
        if (player != null && sensors != null)
        {
            // Check if player is in view and close enough
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            if (playerCollider != null && sensors.InView(playerCollider))
            {
                target = player;
                return;
            }
        }

        // Then check for other enemies with different masks
        if (sensors != null && sensors.Enviroment.Length > 0)
        {
            foreach (var a in sensors.Enviroment)
            {
                if (a == null) continue;
                
                MaskHandler msk;
                try
                {
                    if (a.TryGetComponent<MaskHandler>(out msk))
                    {
                        if (msk.mask != handler.mask)
                        {
                            target = msk.gameObject;
                            return;
                        }
                    }
                }
                catch
                {
                    // Silently continue if component check fails
                }
            }
        }
    }

    private void ChangedMask()
    {
        if (masks.Count > 0)
        {
            handler.EquipMask(masks[Random.Range(0, masks.Count)]);
        }
    }

    private void checkState()
    {
        // Validate target exists
        if (target != null && !target.activeSelf)
        {
            target = null;
            states = States.IDLE;
            return;
        }

        switch (states)
        {
            case States.IDLE:
                CheckEnviorment();
                if (target != null)
                {
                    states = States.CHASE;
                }
                break;

            case States.CHASE:
                if (target == null)
                {
                    states = States.IDLE;
                    break;
                }

                if (combatController._RangeAttack)
                {
                    // For ranged attacks, attack when close enough
                    if (CommonFunctions.IsClose(this.transform.position, target.transform.position, 7f))
                    {
                        states = States.ATTACK;
                    }
                }
                else
                {
                    // For melee attacks, attack when reached target
                    if (agent._hasTarget && agent._reachedTarget)
                    {
                        states = States.ATTACK;
                    }
                }
                break;

            case States.ATTACK:
                if (target == null)
                {
                    states = States.IDLE;
                    break;
                }

                float attackRange = combatController._RangeAttack ? 7f : 1.2f;
                
                // Check if target is still in attack range
                if (!CommonFunctions.IsClose(this.transform.position, target.transform.position, attackRange))
                {
                    states = States.CHASE;
                }
                break;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // Periodically check state
        t += Time.deltaTime;
        if (t > 0.2f)
        {
            checkState();
            t = 0f;
        }

        // Handle attack timing
        switch (states)
        {
            case States.ATTACK:
                if (target == null || !target.activeSelf)
                {
                    target = null;
                    states = States.IDLE;
                    break;
                }

                t_A += Time.deltaTime;
                if (t_A > 0.3f)
                {
                    t_A = 0f;
                    if (combatController != null)
                    {
                        combatController.Attack();
                    }
                }
                break;
        }

        // Handle state transitions
        stSwitch.Update(oldstates == states);
        if (stSwitch.OnRelese())
        {
            SetStateGraph();
        }
        oldstates = states;
    }

    public void OnDamage(DamageLog log)
    {
        // DamageLog is a struct, so it can't be null; only guard against missing source
        if (log.source == null) return;

        health -= log.damageAmount;
        
        // Apply knockback
        Vector2 dir = Vector2.Normalize(new Vector2(-log.source.transform.position.x + this.transform.position.x, 0f));
        dir += Vector2.Normalize(Vector2.up * 0.5f);
        movment.NuckRepale(dir, nuckforce);
        
        // Set attacker as target
        target = log.source;
        checkState();
        
        // Check if dead
        if (health <= 0f)
        {
            this.gameObject.SetActive(false);
            if (handler != null && handler.mask != null && MaskPool != null)
            {
                GameObject maskInstance = MaskPool.GetInstance(this.transform.position);
                if (maskInstance != null)
                {
                    MaskItem msk = maskInstance.GetComponent<MaskItem>();
                    if (msk != null)
                    {
                        msk.SetMask(handler.mask);
                    }
                }
            }
        }
    }

    public bool IsDead()
    {
        return health <= 0f;
    }
}
