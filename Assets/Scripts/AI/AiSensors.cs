using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AiSensors : MonoBehaviour
{
    // this comparer sort a list of colliders by there tags.
    // and then sort them by proximity to the reletive transform if both poses the same tag.
    public struct CompareByTag : IComparer<Collider2D>
    {
        public Transform reletive;
        public int Compare(Collider2D x, Collider2D y)
        {
            //If both collider are null then they are equal 
            if (x == null && y == null)
            {
                return 0;
            }
            //If one collider not equal to null then it is grater the the other 
            if (x == null && y != null)
            {
                return 1;
            }

            if (x != null && y == null)
            {
                return -1;
            }

            //if both exsist them compere there tags.
            int result = (int)(string.Compare(x.tag, y.tag));
            //if tags are the same then return the collider that is the closes to the reletive transform.
            if(result == 0)
            {
                return (int)(Vector3.Magnitude(x.gameObject.transform.position - reletive.position) - Vector3.Magnitude(y.gameObject.transform.position - reletive.position));
            }
            else
            {
                //else return the collider with higher numeric tag.
                return result;
            }

        }
    }

    [Header("Scaner")]
    [SerializeField] private float ScanRate = 30f;
    private float scantime;
    private float scanTimer;
    private static float delaytime = 0;
    public LayerMask ScanLayerMask;

    [Header("FieldOfView")]
    public float heightOffset = 1.0f;
    public float detectionRadius = 10;
    public bool useHeightDifference = true;

    [Range(0.0f, 360.0f)]
    public float detectionAngle = 270;
    public float maxHeightDifference = 1.0f;
    public string DetectTag;
    public LayerMask viewBlockerLayerMask;

    private CompareByTag bs;
    private Collider2D call;

    public event Action OnScan;

    [SerializeField] private Collider2D[] locEnviroment = new Collider2D[10];
    public Collider2D[] Enviroment
    {
        get
        {
            return locEnviroment;
        }
        private set
        {
            locEnviroment = value;
        }
    }

    private Transform detector;

    private void Awake()
    {
        bs.reletive = transform;
        call = gameObject.GetComponent<Collider2D>();
        scantime = 1.0f / ScanRate;
        delaytime += Time.deltaTime;
        if(delaytime > scantime)
        {
            delaytime = 0f;
        }
        scanTimer = delaytime;
    }

    public bool InView(Collider2D cal)
    {
        if (cal != null)
            return CommonFunctions.InView(cal.transform, transform, detectionAngle, detectionRadius, heightOffset, maxHeightDifference, viewBlockerLayerMask, useHeightDifference);
        else
            return false;
    }

    public bool IsClose(Collider2D col, float distance)
    {
        if (col != null)
        {
            // This is so the distance will be calculated from the closes point to the object and not from the center
            // Because bigger models won't calc the distance as they should because it will include the distance from the object center and not from the model edges
            Vector3 closestPoint = col.ClosestPoint(transform.position);
            return CommonFunctions.IsClose(closestPoint, transform.position, distance, heightOffset);
        }
        else
            return false;
    }

    private void scan()
    {
        if (locEnviroment != null)
        {
            scanTimer += Time.deltaTime;
            if (scanTimer > scantime)
            {
                Vector2 pos = CommonFunctions.get2DPosition(transform.position + Vector3.up * heightOffset);

                locEnviroment = Physics2D.OverlapCircleAll(pos, detectionRadius, ScanLayerMask);

                for(int i = 0; i < locEnviroment.Length - 1; i++)
                {
                    if (locEnviroment[i] == call) locEnviroment[i] = null;
                }
                int count = locEnviroment.Length;
                Array.Sort(locEnviroment, bs);
                count = Mathf.Clamp(count, 0, locEnviroment.Length - 1);
                locEnviroment[count] = null;
                scanTimer = 0f;
                OnScan.Invoke();
            }
            
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position + Vector3.up * heightOffset, detectionRadius);
        //Gizmos.DrawFrustum(transform.position + Vector3.up * heightOffset, detectionAngle, detectionRadius, 1, 1);
    }

    private void FixedUpdate()
    {
        scan();
    }

    //public bool View(Transform target)
    //{
    //    detector = transform;
    //    Vector3 eyePos = detector.position + Vector3.up * heightOffset;
    //    Vector3 toPlayer = target.position - eyePos;
    //    Vector3 toPlayerTop = target.position + Vector3.up * 1.5f - eyePos;
    //
    //    if (useHeightDifference && Mathf.Abs(toPlayer.y + heightOffset) > maxHeightDifference)
    //    { //if the target is too high or too low no need to try to reach it, just abandon pursuit
    //        return false;
    //    }
    //
    //    Vector3 toPlayerFlat = toPlayer;
    //    toPlayerFlat.y = 0;
    //
    //    if (toPlayerFlat.sqrMagnitude <= detectionRadius * detectionRadius)
    //    {
    //        if (Vector3.Dot(toPlayerFlat.normalized, detector.forward) >
    //            Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad))
    //        {
    //
    //            bool canSee = false;
    //
    //            Debug.DrawRay(eyePos, toPlayer, Color.blue);
    //            Debug.DrawRay(eyePos, toPlayerTop, Color.blue);
    //
    //            canSee |= !Physics.Raycast(eyePos, toPlayer.normalized, detectionRadius,
    //                viewBlockerLayerMask, QueryTriggerInteraction.Ignore);
    //
    //            canSee |= !Physics.Raycast(eyePos, toPlayerTop.normalized, toPlayerTop.magnitude,
    //                viewBlockerLayerMask, QueryTriggerInteraction.Ignore);
    //
    //            if (canSee)
    //                return true;
    //        }
    //    }
    //
    //    return false;
    //}
}
