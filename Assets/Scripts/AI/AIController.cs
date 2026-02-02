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
                agent.EnableAgnet(false);
                break;

            case States.CHASE:
                agent.EnableAgnet(true);
                agent.SetTarget(target.transform);
                break;

            case States.ATTACK:
                Debug.Log("Attack");
                agent.EnableAgnet(false);
                combatController.Attack();
                break;

        }
    }

    private void CheckEnviorment()
    { 
        foreach(var a in sensors.Enviroment)
        {
            MaskHandler msk;
            if(a.TryGetComponent<MaskHandler>(out msk))
            {
                if (msk.mask != handler.mask)
                {
                    target = msk.gameObject;
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

        switch (states)
        {
            case States.IDLE:
                if(target != null)
                {
                    states = States.CHASE;
                }
                else
                {
                    CheckEnviorment();
                }
                break;

            case States.CHASE:
                if (combatController._RangeAttack)
                {
                    if (!CommonFunctions.IsClose(this.transform.position, target.transform.position, 7f))
                    {
                        states = States.ATTACK;
                    }
                }
                else
                {
                    if (agent._hasTarget && agent._reachedTarget)
                    {
                        states = States.ATTACK;
                    }
                }
                break;

            case States.ATTACK:

                float rg = combatController._RangeAttack ? 7f: 1.2f;
                if (target.activeSelf == false)
                {
                    target = null;
                    states = States.IDLE;
                } else

                if (!CommonFunctions.IsClose(this.transform.position, player.transform.position, rg))
                {
                    states = States.CHASE;
                }
                break;

        }
    }
    // Update is called once per frame

    float t = 0f;
    float t_A = 0f;
    void FixedUpdate()
    {
        /*t += Time.deltaTime;
        if (t > 0.2f)
        {
            checkState();
            t = 0f;
        }*/

        switch (states)
        {

            case States.ATTACK:
                t_A += Time.deltaTime;
                if (t_A > 0.3f)
                {
                    t_A = 0f;
                    combatController.Attack();
                }
                break;

        }

        stSwitch.Update(oldstates == states);
        if(stSwitch.OnRelese())
        {
            SetStateGraph();
        }
        oldstates = states;
    }

    public void OnDamage(DamageLog log)
    {
        //if (log.type != handler.mask || log.type == null)
        //{
        health -= log.damageAmount;
        Vector2 dir = Vector2.Normalize(new Vector2(-log.source.transform.position.x + this.transform.position.x, 0f));
        dir += Vector2.Normalize(Vector2.up * 0.5f);
        movment.NuckRepale(dir, nuckforce);
        target = log.source;
        checkState();
        //}
        if (health < 0f)
        {
            this.gameObject.SetActive(false);
            if (handler.mask != null)
            {
                MaskItem msk = MaskPool.GetInstance(this.transform.position).gameObject.GetComponent<MaskItem>();
                msk.SetMask(handler.mask);
            }
        }
    }

    public bool IsDead()
    {
        throw new System.NotImplementedException();
    }
}
