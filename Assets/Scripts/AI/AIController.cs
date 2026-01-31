using Mono.Cecil;
using TarodevController;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AICharacterController2D))]
[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CombatController))]
public class AIController : MonoBehaviour, Damageable<float>
{
    public float health = 100f;
    private enum States
    {
        IDLE,
        CHASE,
        ATTACK
    }
    [SerializeField] private States states;
    private States oldstates;
    private GameObject player;
    private AICharacterController2D agent;
    private CharacterMovement movment;
    private CombatController combatController;
    private MaskHandler handler;
    [SerializeField] private LayerMask characterLayerMask;
    [SerializeField] private ObjectPool MaskPool;

    private SmartSwitch stSwitch;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        movment = gameObject.GetComponent<CharacterMovement>();
        combatController = gameObject.GetComponent<CombatController>();
        handler = gameObject.GetComponent<MaskHandler>();
        player = GameObject.FindGameObjectWithTag("Player");
        agent = gameObject.GetComponent<AICharacterController2D>();
        t = Random.value * 0.2f;

        handler.maskChanged += ChangedMask;
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
                agent.SetTarget(player.transform);
                break;

            case States.ATTACK:
                Debug.Log("Attack");
                agent.EnableAgnet(false);
                combatController.Attack();
                break;

        }
    }

    private void ChangedMask()
    {

    }

    private void checkState()
    {

        switch (states)
        {
            case States.IDLE:
                if (CommonFunctions.IsClose(this.transform.position, player.transform.position,30f))
                {
                    states = States.CHASE;
                }
                break;

            case States.CHASE:
                if (combatController._RangeAttack)
                {
                    if (!CommonFunctions.IsClose(this.transform.position, player.transform.position, 7f))
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
    void Update()
    {
        t += Time.deltaTime;
        if (t > 0.2f)
        {
            checkState();
            t = 0f;
        }

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

    public void OnDamage(float log)
    {
        health -= log;

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
