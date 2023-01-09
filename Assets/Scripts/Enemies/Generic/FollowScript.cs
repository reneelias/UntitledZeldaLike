using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    public GameObject objectToFollow;
    public float followSpeed;
    [SerializeField] private Rigidbody2D m_Rigidbody2D;
	private Vector3 m_Velocity = Vector3.zero;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    public bool shouldFollow;
    public bool useStartingPositionAsOrigin = true;
    public Vector3 originPosition;
    private bool atOrigin;

    public bool shouldReturnToOrigin;
    public bool hasLOS;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        Follow();
        ReturnToOrigin();
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
