using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class hudScript : MonoBehaviour
{
    [SerializeField] private FloatVariable playerHealth;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private GameObject gameover;

    // Update is called once per frame
    float t = 0;
    void Update()
    {
        scrollbar.size = playerHealth.value / 1000f;

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

}
