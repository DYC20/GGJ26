using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class hudScript : MonoBehaviour
{
    [SerializeField] private FloatVariable playerHealth;
    [SerializeField] private float Starthealth;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private GameObject gameover;

    [SerializeField] private Image ResuveMask;
    [SerializeField] private Image CurrentMask;
    [SerializeField] private TMP_Text Current_Mask_Text;
    [SerializeField] private TMP_Text Resurve_Mask_Text;
   
    [SerializeField] private MaskHandler playerHandler;
    [SerializeField] private MaskController playerMaskController;

    [SerializeField] private GameObject pickupMassage;
    [SerializeField] private TMP_Text Instructions;


    private void Awake()
    {
        Starthealth = playerHealth.value;
    }
    private void OnEnable()
    {
        playerMaskController.collectMask += UpdateMaskView;
        playerMaskController.OnVisableMask += (a) => { pickupMassage.SetActive(a); };
    }

    private void OnDisable()
    {
        playerMaskController.collectMask -= UpdateMaskView;
    }
    // Update is called once per frame
    float t = 0;
    void Update()
    {
        scrollbar.size = playerHealth.value / Starthealth;

        if(playerHealth.value <= 0f)
        {
            t += Time.deltaTime;
            gameover.SetActive(true);
            if(t > 4f)
            {
                SceneManager.LoadScene(0);
            }

        }
    }

    private void UpdateMaskView()
    {
        if (playerMaskController.resurve)
        {
            ResuveMask.sprite = playerMaskController.resurve.Sprite;
            Resurve_Mask_Text.text = playerMaskController.resurve.name + " Resuved";
        }
        if (playerHandler.mask)
        {
            CurrentMask.sprite = playerHandler.mask.Sprite;
            Current_Mask_Text.text = playerHandler.mask.name + " Equiped";
            Instructions.text = playerHandler.mask.Instructions;
        }
    }

}
