using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MaskController : MonoBehaviour
{
    [SerializeField] public Mask carry;
    [SerializeField] public Mask resurve;
    [SerializeField] private ObjectPool MaskPool;
    [SerializeField] private LayerMask InteractLayers;

    public event Action<bool> changeMask;
    public event Action collectMask;

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
                
                return;
            }

            carry = maskHandler.mask;
            maskHandler.mask = resurve;

            resurve = carry;
            carry = null;
        }
    }

    public void CollectMask(InputAction.CallbackContext ctx)
    {
        Debug.Log("Collecting");
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
                Debug.Log("Collected: " + hit.name);
                if(resurve != null)
                {
                    MaskItem msk = MaskPool.GetInstance(this.transform.position).gameObject.GetComponent<MaskItem>();
                    msk.SetMask(resurve);
                }
                resurve = dmg.CollectMask();
                collectMask?.Invoke();
                return;
            }
        }
    }
}
