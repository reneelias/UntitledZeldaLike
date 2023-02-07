using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    public GameObject objectToFollow;
    public float followSpeed = 5f;
    [SerializeField] private Rigidbody2D m_Rigidbody2D;
	private Vector3 m_Velocity = Vector3.zero;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    public bool shouldFollow;
    public bool useStartingPositionAsOrigin = true;
    public Vector3 originPosition;
    private bool atOrigin;
    public bool shouldReturnToOrigin;
    public bool hasLOS;
    [Header("Idle Movement")]
    [SerializeField] bool shouldHaveIdleMovement = false;
    [SerializeField] float idleMovementRange = 2f;
    float idleMovementAngle = 0f;
    [SerializeField] float idleMovementPauseTimeMin = 1f;
    [SerializeField] float idleMovementPauseTimeMax = 3f;
    float idleMovementPauseTime;
    float idleMovementPauseDT;
    [SerializeField] float idleMovementSpeed = 2f;
    Vector3 idleTargetPosition;
    Vector3 idleVelocity;
    bool movingToNewPosition = false;

    private void Awake()
    {
		// m_Rigidbody2D = GetComponent<Rigidbody2D>();
        atOrigin = true;
        hasLOS = true;
    }
    protected virtual void Start()
    {
        if(useStartingPositionAsOrigin){
            originPosition = transform.position;
        }

        idleMovementPauseTime = Random.Range(idleMovementPauseTimeMin, idleMovementPauseTimeMax);
        idleMovementPauseDT = 0f;
        idleVelocity = Vector3.zero;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateIdleMovement();
    }

    protected virtual void FixedUpdate()
    {
        Follow();
        ReturnToOrigin();
    }

    protected virtual void UpdateIdleMovement(){
        if(!shouldHaveIdleMovement || !atOrigin){
            return;
        }

        if(!movingToNewPosition){
            idleMovementPauseDT += Time.deltaTime;
        }

        if(idleMovementPauseDT >= idleMovementPauseTime && !movingToNewPosition){
            idleMovementAngle = Random.Range(0f, Mathf.PI * 2f);
            idleTargetPosition = originPosition + new Vector3(Mathf.Cos(idleMovementAngle) * Random.Range(0f, idleMovementRange), Mathf.Sin(idleMovementAngle) * Random.Range(0f, idleMovementRange));
            movingToNewPosition = true;
            Vector3 differenceVector3D = idleTargetPosition - transform.position;
            idleVelocity = differenceVector3D.normalized * idleMovementSpeed;
        }

        if(movingToNewPosition){
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, idleVelocity, ref m_Velocity, m_MovementSmoothing);
        }

        if(movingToNewPosition && (transform.position - idleTargetPosition).magnitude <= .05f){
            movingToNewPosition = false;
            m_Rigidbody2D.velocity = Vector2.zero;
            idleMovementPauseDT = 0f;
            idleMovementPauseTime = Random.Range(idleMovementPauseTimeMin, idleMovementPauseTimeMax);
        }
    }

    void Follow(){
        if(!shouldFollow){
            if(m_Rigidbody2D){
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, Vector2.zero, ref m_Velocity, m_MovementSmoothing);
            }
            
            return;
        }
        Vector3 differenceVector3D = objectToFollow.transform.position - transform.position;
        
        UpdateVelocity(differenceVector3D);
        atOrigin = false;
    }

    void ReturnToOrigin()
    {
        if(shouldFollow || !shouldReturnToOrigin || atOrigin){
            return;
        }
                
        Vector3 differenceVector3D = originPosition - transform.position;

        UpdateVelocity(differenceVector3D);
    }

    protected virtual void UpdateVelocity(Vector3 diffVector)
    {
        if(diffVector == null){
            return;
        }
        if(diffVector.magnitude <= .1f && shouldReturnToOrigin)
        {
            atOrigin = true;
        }

        if(m_Rigidbody2D){
            Vector2 targetVelocity = diffVector.normalized * followSpeed;
		    m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        } else {
            Vector3 targetVelocity = diffVector.normalized * followSpeed;
            transform.position += targetVelocity;
        }
    }
}
