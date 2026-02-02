using UnityEngine;
using UnityEngine.Events;
using static Unity.VisualScripting.Member;

public class StartBroller : MonoBehaviour
{
    [SerializeField] UnityEvent BrollerStartedEvent;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            BrollerStartedEvent.Invoke();


        }
    }
}
