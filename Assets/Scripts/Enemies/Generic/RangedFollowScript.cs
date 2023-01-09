using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedFollowScript : FollowScript
{
    [SerializeField] float separationDistance = 3f;

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void UpdateVelocity(Vector3 diffVector)
    {
        diffVector = -diffVector;
        diffVector = diffVector.normalized * separationDistance - diffVector;
        if(diffVector.magnitude <= .05f){
            diffVector = Vector3.zero;
        }

        base.UpdateVelocity(diffVector);
    }

     private void OnDrawGizmos(){
		UnityEngine.Gizmos.color = new Color(1f, 0.5f, 0.25f);
        UnityEngine.Gizmos.DrawWireSphere(transform.position, separationDistance);
    }
}
