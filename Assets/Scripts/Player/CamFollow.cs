using TarodevController;
using Unity.VisualScripting;
using UnityEngine;

public class CamFollow : MonoBehaviour
{

    public GameObject target;
    public CharacterMovement movement;
    public float horizontalSnapingStrangth = 5f;
    public float verticalSnapingStrangth = 10f;

    private Vector3 offset;
    private bool _isTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isTarget = target != null;
        if(_isTarget)
        {
            offset = Vector3.forward * (transform.position.z - target.transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _isTarget = target != null;
        if (_isTarget)
        {
            Vector3 dir = movement.direction.x > 0f ? Vector3.right : Vector3.left;
            float x = Mathf.Lerp(transform.position.x, target.transform.position.x, Time.deltaTime * horizontalSnapingStrangth) + movement.direction.x * 0.2f;
            float y = 0;

            if (movement._frameVelocity.y < 0f)
            {
                y = Mathf.Lerp(transform.position.y, target.transform.position.y - 0.2f, Time.deltaTime * horizontalSnapingStrangth);
            }
            else
            {
                y = Mathf.Lerp(transform.position.y, target.transform.position.y, Time.deltaTime * verticalSnapingStrangth);
            }

                float z = offset.z;
            transform.position = new Vector3()
            { 
                x = x,
                y = y,
                z = offset.z
            };
            //transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, Time.deltaTime * horizontalSnapingStrangth);
            //transform.position = target.transform.position + offset;
        }
    }
}
