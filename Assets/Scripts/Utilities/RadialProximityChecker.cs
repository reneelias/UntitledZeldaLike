using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialProximityChecker : MonoBehaviour
{
    CircleCollider2D circleCollider2D;
    [SerializeField] GameObject[] objectTypesToCheck;
    [SerializeField] GameObject parentObject;
    public List<Collider2D> CollidersToDodge{
        protected set; get;
    }
    // Start is called before the first frame update
    void Start()
    {
        CollidersToDodge = new List<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset(){
        CollidersToDodge.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CollidersToDodge.Clear();

        for(var i = 0; i < objectTypesToCheck.Length; i++){
            if(other.tag == objectTypesToCheck[i].tag){
                Vector2 otherVelocity = other.attachedRigidbody.velocity;
                Vector2 differenceVector = parentObject.transform.position - other.transform.position;
                float dotProduct = Vector2.Dot(otherVelocity.normalized, differenceVector.normalized);

                if(dotProduct > .9f && dotProduct <= 1f){
                    CollidersToDodge.Add(other);
                }
            }
        }        
    }
}
