using System;
using UnityEngine;
using UnityEngine.UI;

public class MaskHandler : MonoBehaviour
{
    public Mask mask
    {
        get
        {
            return currentMask;
        }
        set
        {
            ClearMask();
            EquipMask(value);
        }
    }
    [SerializeField] private Mask currentMask;

    public event Action maskChanged;
    
    public void Awake()
    {
        if (currentMask != null)
        {
            EquipMask(currentMask);
        }
    }

    // ================= PUBLIC API =================

    public void EquipMask(Mask newMask)
    {
        if (currentMask != null)
            currentMask.Deactivate(gameObject);

        maskChanged?.Invoke();
        currentMask = newMask;

        if (currentMask != null)
            currentMask.Activate(gameObject);
    }

    public void ClearMask()
    {
        if (currentMask == null)
            return;

        currentMask.Deactivate(gameObject);
        currentMask = null;
    }

    // ================= INFO RELAY =================

    public bool HasMask()
    {
        return currentMask != null;
    }

    public Color GetMaskColor()
    {
        return currentMask != null ? currentMask.Color : Color.white;
    }

    public Sprite GetMaskSprite()
    {
        return currentMask != null ? currentMask.Sprite : null;
    }

    public Mask GetMask()
    {
        return currentMask;
    }
}
