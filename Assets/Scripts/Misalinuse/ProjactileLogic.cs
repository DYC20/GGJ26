using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjactileLogic : MonoBehaviour
{
    enum Tragectory
    { 
        FLAT
    }
    [Header("Stats")]
    [SerializeField] private bool _splashDamage = false;
    [SerializeField] private float speed = 1;
    [SerializeField] private float damage = 5;
    [SerializeField] private float LifeTime = 4f;
    [SerializeField] private GameObject retical;

    [Header("SplashDamage (Only used if splash Damage = true)")]
    [SerializeField] private float splashRadius = 2f;
    [SerializeField] private LayerMask ScanLayerMask;
    [SerializeField] private float splashDamage = 5;

    [Header("Events")]
    public UnityEvent Fired;
    public UnityEvent Hit;
    public UnityEvent Landed;
    public ObjectPool ImpactPool;

    private bool fire;
    private Vector3 start, control, end;
    private GameObject source;
    private float t = 0f;
    private float flyTime = 0f;

    public float Speed { get {return speed; } private set {speed = value;}}
    public float Damage { get { return damage; } set { damage = value; } }

    private DamageLog log;

    public void OnEnable()
    {
        fire = false;
        source = null;
        t = 0f;
        flyTime = 0f;
}

    public void SetTarget(Vector3 target, GameObject source)
    {
        this.source = source;
        start = this.transform.position;
        end = target;
        end.z = 0f;
        control = Vector3.Normalize(end - start);
        flyTime = LifeTime;

        if(retical) retical.transform.position = end;
        fire = true;
        Fired.Invoke();
    }

    private void FixedUpdate()
    {
        if(fire)
        {
            // Calculate the new progress value over time
            t += Time.deltaTime / flyTime;
            Vector3 newPosition;
            Vector3 pd;

                    start += control * speed * Time.deltaTime;
                    newPosition = start;

            // Move the object to the new position
            transform.position = newPosition;
            if (t > 1f)
            {
                KillProjectile();
            }
        }
    }
    private void OnDisable()
    {
        if (retical) retical.SetActive(false);
        Landed.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != source)
        {
            if (other.gameObject.tag == "Ground")
            {
                KillProjectile();
                return;
            }
            Damageable<float> dmg = other.gameObject.GetComponent<Damageable<float>>();
            if (dmg != null && other.gameObject != source)
            {
                dmg.OnDamage(damage);
                Hit.Invoke();
                
            }
            KillProjectile();
            return;
        }
    }

    private void SplashDamage(Collider ignore)
    {
        if (_splashDamage)
        {
            Collider[] count = Physics.OverlapSphere(transform.position, splashRadius, ScanLayerMask);
            foreach (var d in count)
            {
                float disNormalized = Vector3.Distance(d.transform.position, this.transform.position)/ splashRadius;
                Damageable<DamageLog> dmg = d.GetComponent<Damageable<DamageLog>>();
                if (dmg != null && d.gameObject != source)
                {
                    log.damageAmount = (damage - splashDamage) * disNormalized + splashDamage;
                    log.source = source;
                    dmg.OnDamage(log);
                }
            }
        }
    }

    void KillProjectile(Collider passthrou = null)
    {
        this.gameObject.SetActive(false);
        //Destroy(this.gameObject);
        if(ImpactPool) ImpactPool.GetInstance(transform.position);
    }
}
