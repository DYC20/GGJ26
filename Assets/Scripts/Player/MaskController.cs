using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MaskController : MonoBehaviour
{
    [SerializeField] public MaskItem visableMask;

    [SerializeField] public Mask carry;
    [SerializeField] public Mask resurve;
    [SerializeField] private ObjectPool MaskPool;
    [SerializeField] private LayerMask InteractLayers;

    public event Action<bool> changeMask;
    public event Action collectMask;
    public event Action<bool> OnVisableMask;

    SmartSwitch vMask;

    private MaskHandler maskHandler;

    public void Awake()
    {
        maskHandler = GetComponent<MaskHandler>(); 
    }

    public void ChangeMasks(InputAction.CallbackContext ctx)
    {
        if (maskHandler != null)
        {
            if (resurve == null)
            {
                Debug.Log("No Mask");
                return;
            }

            carry = maskHandler.mask;
            maskHandler.EquipMask(resurve);

            resurve = carry;
            carry = null;
            collectMask?.Invoke();
        }
    }

    public void CollectMask(InputAction.CallbackContext ctx)
    {
        if (visableMask != null)
        {
            Debug.Log("Collected: " + visableMask.name);

            if (maskHandler.mask == null)
            {
                maskHandler.EquipMask(visableMask.CollectMask());
            }
            else
            {
                if (resurve != null)
                {
                    MaskItem msk = MaskPool.GetInstance(this.transform.position).gameObject.GetComponent<MaskItem>();
                    msk.SetMask(resurve);
                }
                resurve = visableMask.CollectMask();
            }
            collectMask?.Invoke();
        }
    }
    private float t_R = 0;
    private void Update()
    {
        vMask.Update(visableMask != null);

        if(vMask.OnEvent())
        {
            OnVisableMask?.Invoke(vMask.OnHold());
        }


        t_R += Time.deltaTime;
        if(t_R > 0.1f)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                this.transform.position + Vector3.down,
                1.2f,
                InteractLayers
            );

            foreach (Collider2D hit in hits)
            {
                Debug.Log("Found: " + hit.name);
                MaskItem dmg = hit.gameObject.GetComponent<MaskItem>();
                if (dmg != null && hit.gameObject != this)
                {
                    visableMask = dmg;
                    t_R = 0;
                    return;
                }
            }
            visableMask = null;
            t_R = 0;
        }
    }
}
