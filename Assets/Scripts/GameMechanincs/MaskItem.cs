using UnityEngine;

public class MaskItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public Mask maskSource;
    [SerializeField] private bool StopDelete = false;
    private SpriteRenderer spriteRenderer;
    

    private float t = 7f;
    private void Awake()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        t = 7f;
        if(maskSource != null)
        {
            SetMask(maskSource);
        }
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

    private void Update()
    {
        if (!StopDelete)
        {
            t -= Time.deltaTime;
            if (t < 0f)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
