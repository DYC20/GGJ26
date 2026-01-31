using UnityEngine;

public class MaskItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Mask maskSource;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetMask(Mask msk)
    {
        maskSource = msk;
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = msk.Sprite;
    }

    public Mask CollectMask()
    {
        this.gameObject.SetActive(false);
        return maskSource;
    }
}
