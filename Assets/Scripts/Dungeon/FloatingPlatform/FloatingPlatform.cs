using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloatingPlatform : MonoBehaviour
{
    [SerializeField] Vector3[] pathNodes;
    [SerializeField] float speed = 3f;
    [SerializeField] bool useTimedTransitions = true;
    [SerializeField] float transitionTime = 2f;
    [SerializeField] bool pauseAtNodes = true;
    [SerializeField] float pauseTime = 1f;
    float pauseDT = 0f;
    bool pausing = true;
    [Tooltip("When true, will move floating platform from the last node in array to the very first one, instead of going backwards.")]
    [SerializeField] bool circularNodeTraversal = false;
    [SerializeField] int currentNode = 0;
    int nextNode;
    public Vector3 Velocity{
        get;
        protected set;
    }
    [SerializeField] GameObject rigidbodyObject;
    Rigidbody2D rigidbody;
    [SerializeField] bool positiveDirection = true;
    Tween movementTween;
    [SerializeField] float wireCubeSize = 4;

    void Awake(){
        transform.position = pathNodes[currentNode];
        rigidbody = rigidbodyObject.GetComponent<Rigidbody2D>();
        nextNode = currentNode;
        // SetNextNode();
    }

    // Start is called before the first frame update
    void Start()
    {
            
    }

    void SetNextNode(){
        if(circularNodeTraversal){
            nextNode++;
            if(nextNode >= pathNodes.Length){
                nextNode = 0;
            } 
        } else {
            if(positiveDirection){
                nextNode++;

                if(nextNode >= pathNodes.Length){
                    nextNode = pathNodes.Length - 2;
                    positiveDirection = false;
                }
            } else {
                nextNode--;

                if(nextNode < 0){
                    nextNode = 1;
                    positiveDirection = true;
                }
            }
        }
        
        // Velocity = (pathNodes[nextNode] - transform.position).normalized * speed;
        float transitionDuration = speed * (pathNodes[nextNode] - transform.position).magnitude;
        if(useTimedTransitions){
            transitionDuration = transitionTime;
        }

        movementTween = transform.DOMove(pathNodes[nextNode], transitionTime)
            .OnComplete(()=>{
                if(pauseAtNodes){
                    rigidbody.velocity = Vector2.zero;
                    pausing = true;
                    Velocity = Vector3.zero;
                } else {
                    SetNextNode();
                }
            })
            .SetEase(Ease.Linear)
            .SetUpdate(UpdateType.Fixed);

        Velocity = (pathNodes[nextNode] - transform.position) / transitionDuration;
    }

    // Update is called once per frame
    void Update()
    {
        // UpdateNodeProximityCheck();
    }

    void FixedUpdate()
    {
        // UpdatePosition();
        UpdatePause();
    }

    void UpdatePosition(){
        if(pausing){
            return;
        }
        
        transform.position += Velocity;
    }

    void UpdateNodeProximityCheck(){
        if(pausing){
            return;
        }

        if((transform.position - pathNodes[nextNode]).magnitude <= .2f){
            transform.position = pathNodes[nextNode];

            if(pauseAtNodes){
                rigidbody.velocity = Vector2.zero;
                pausing = true;
                Velocity = Vector3.zero;
            } else {
                SetNextNode();
            }
        }
    }

    void UpdatePause(){
        if(!pausing){
            return;
        }

        Velocity = Vector3.zero;
        pauseDT += Time.deltaTime;
        if(pauseDT >= pauseTime){
            pausing = false;
            pauseDT = 0f;
            SetNextNode();
        }
    }

    public void PlayerEnteredPlatform(CharacterControls characterControls){
        characterControls.EnterFloatingPlatform(this);
    }

    public void PlayerExitedPlatform(CharacterControls characterControls){
        characterControls.ExitFloatingPlatform(this);
    }


    void OnDrawGizmos(){
        if(pathNodes == null || pathNodes.Length <= 1){
            return;
        }

        UnityEngine.Gizmos.color = Color.cyan;

        for(int i = 0; i < pathNodes.Length; i++){
            if(circularNodeTraversal){
                if(i == pathNodes.Length - 1 && pathNodes.Length > 2){
                    UnityEngine.Gizmos.DrawLine(pathNodes[i], pathNodes[0]);
                } else {
                    UnityEngine.Gizmos.DrawLine(pathNodes[i], pathNodes[i + 1]);
                }

                UnityEngine.Gizmos.DrawWireCube(pathNodes[i], new Vector3(wireCubeSize, wireCubeSize, 0));
            } else {
                if(i < pathNodes.Length - 1){
                    UnityEngine.Gizmos.DrawLine(pathNodes[i], pathNodes[i + 1]);
                }

                UnityEngine.Gizmos.DrawWireCube(pathNodes[i], new Vector3(wireCubeSize, wireCubeSize, 0));
            }
        }
    }
}
