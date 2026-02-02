using UnityEngine;

public abstract class Mask : ScriptableObject
{
    [Header("Visuals")]
    [SerializeField] private Color maskColor = Color.white;
    [SerializeField] private Sprite maskSprite;

    // ================= INFO =================
    public Color Color => maskColor;
    public Sprite Sprite => maskSprite;

    public string Instructions;

    // ================= POWERUP =================
    public abstract void Activate(GameObject owner);
    public abstract void Deactivate(GameObject owner);
}

