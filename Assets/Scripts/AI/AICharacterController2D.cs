using Pathfinding;
using TarodevController;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class AICharacterController2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private float activateDistance = 50f;
    [SerializeField] private float pathUpdateSeconds = 0.5f;

    [Header("Movement")]
    [SerializeField] private float nextWaypointDistance = 1.2f;
    [SerializeField] private float jumpNodeHeightRequirement = 0.8f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;

    private CharacterMovement controller;
    private Seeker seeker;
    private Rigidbody2D rb;
    private Collider2D col;

    private Path path;
    private int currentWaypoint;

    private bool grounded;
    private float lastJumpTime;
    private const float JumpCooldown = 0.2f;

    private void Awake()
    {
        controller = GetComponent<CharacterMovement>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(UpdatePath), 0f, pathUpdateSeconds);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(UpdatePath));
    }

    #region Public API (AI as a Service)

    public bool _hasTarget { get; private set; }
    public bool _isEnabled { get; private set; }
    public bool _reachedTarget { get; private set; }
    
    public void SetTarget(Transform target)
    {
        this.target = target;
        _hasTarget = this.target != null;
        _reachedTarget = false;
        
        if (_hasTarget && _isEnabled)
        {
            UpdatePath();
        }
    }

    public void ClearTarget()
    {
        _hasTarget = false;
        _reachedTarget = false;
        this.target = null;
        path = null;
        currentWaypoint = 0;
        controller.SetInput(Vector2.zero, false, false);
    }

    public void EnableAgent(bool enable)
    {
        this._isEnabled = enable;
        if (!enable)
        {
            controller.SetInput(Vector2.zero, false, false);
        }
    }

    #endregion

    #region Agnet Logic
    private void FixedUpdate()
    {
        if (!_isEnabled)
        {
            controller.SetInput(Vector2.zero, false, false);
            return;
        }

        if (!TargetInRange())
        {
            controller.SetInput(Vector2.zero, false, false);
            _reachedTarget = true;
            return;
        }

        FollowPath();
    }

    private void UpdatePath()
    {
        if (!TargetInRange() || !seeker.IsDone()) return;
        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    private void FollowPath()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            _reachedTarget = true;
            controller.SetInput(Vector2.zero, false, false);
            return;
        }

        _reachedTarget = false;

        CheckGrounded();

        Vector2 toWaypoint = (Vector2)path.vectorPath[currentWaypoint] - rb.position;

        float moveX = Mathf.Sign(toWaypoint.x);
        bool wantsJump =
            grounded &&
            Time.time > lastJumpTime + JumpCooldown &&
            toWaypoint.y > jumpNodeHeightRequirement;

        controller.SetInput(
            new Vector2(moveX, 0),
            wantsJump,
            wantsJump
        );

        if (Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
            currentWaypoint++;
    }

    private void CheckGrounded()
    {
        Vector2 origin = (Vector2)col.bounds.center +
                         Vector2.down * (col.bounds.extents.y + 0.02f);

        grounded = Physics2D.Raycast(
            origin,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
    }

    private bool TargetInRange()
    {
        if (target == null)
        {
            _hasTarget = false;
            return false;
        }
        
        bool inRange = Vector2.Distance(rb.position, target.position) <= activateDistance;
        _hasTarget = inRange;
        return inRange;
    }

    private void OnPathComplete(Path p)
    {
        if (p.error) return;
        path = p;
        currentWaypoint = 0;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (path == null) return;
        Gizmos.color = Color.red;
        for (int i = currentWaypoint; i < path.vectorPath.Count; i++)
        {
            Gizmos.DrawSphere(path.vectorPath[i], 0.1f);
        }
    }

#endif
    #endregion
}
