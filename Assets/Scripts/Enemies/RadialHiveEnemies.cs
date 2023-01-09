using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialHiveEnemies : ClusterEnemy
{
    [SerializeField] float largestRadius;
    [SerializeField] float smallestRadius;
    // [SerializeField] float velocity;
    [SerializeField] float angleChangeTime;
    float scaledAngleChangTime;
    float angleChangeDT;
    [SerializeField] float radiusTransitionSpeed;
    [SerializeField] float statePauseTime;
    float statePauseDT;
    [SerializeField] bool startLargestRadius = true;
    [SerializeField] float angleDelta;
    // [SerializeField] GameObject enemyPrefab;
    float currentRadius;
    float angleIncrements;
    float[] enemyAngles;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    // Start is called before the first frame update
    float targetRadius;
    bool transitioning = false;
    bool firstInitialize = true;
    bool reviving = false;


    protected override void Start()
    {
        base.Start();
        InitializeEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        DetachDeadChildren();
    }

    void FixedUpdate()
    {
        UpdateRadius();
        UpdateAngles();
        // RotateEnemies();
    }

    void InitializeEnemies(){
        if(generateEnemies && firstInitialize){
            enemies = new Enemy[enemyGenerationAmount];

            Enemy tempEnemy;
            for(int i = 0; i < enemyGenerationAmount; i++){
                tempEnemy = Instantiate(enemyPrefab);
                tempEnemy.transform.SetParent(transform);
                tempEnemy.shouldFollowTarget = false;
                tempEnemy.shouldRevive = shouldEnemiesRevive;
                tempEnemy.shouldResetHealth = shouldEnemiesResetHealth;
                tempEnemy.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                tempEnemy.SetAsHiveEnemy();

                enemies[i] = tempEnemy;
            }

            enemyAngles = new float[enemyGenerationAmount];

            // int j = 0;

            // foreach(Transform child in transform){
            //     enemies[j++] = (Enemy)child.gameObject;
            // }
            firstInitialize = false;
        }

        

        currentRadius = startLargestRadius ? largestRadius : smallestRadius;

        angleIncrements = (2 * Mathf.PI) / enemies.Length;
        Vector3 differenceVector;
        for(var i = 0; i < enemies.Length; i++){
            if(!firstInitialize && shouldEnemiesRevive && enemies[i].transform.parent == null){
                enemies[i].transform.SetParent(transform);
            }
            differenceVector = new Vector3(Mathf.Cos(i * angleIncrements), Mathf.Sin(i * angleIncrements), 0) * currentRadius;
            enemies[i].transform.position = transform.position + differenceVector;
            enemyAngles[i] = i * angleIncrements;
        } 
    }

    /*
    * Will need to use this method if I have to go with a velocity based solution.
    */
    void UpdateAngles(){
        angleChangeDT += Time.deltaTime;
        scaledAngleChangTime = angleChangeTime * currentRadius / largestRadius;

        if(angleChangeDT >= scaledAngleChangTime){
            angleChangeDT = 0;

            RotateEnemies();
        }
    }

    void RotateEnemies(){
        // float radianAngleSpeed = velocity * Mathf.Deg2Rad;
        Vector3 targetPosition;
        Vector3 targetVelocity;
        Vector3 diffVector;

        for(var i = 0; i < enemies.Length; i++){
            if(!enemies[i].Alive){
                continue;
            }
            // enemyAngles[i] += radianAngleSpeed * largestRadius / currentRadius;
            enemyAngles[i] += angleDelta * Mathf.Deg2Rad;
            if(enemyAngles[i] >= 2 * Mathf.PI){
                enemyAngles[i] -=  2 * Mathf.PI;
            }
            targetPosition = transform.position + new Vector3(Mathf.Cos(enemyAngles[i]), Mathf.Sin(enemyAngles[i]), 0) * currentRadius;
            diffVector = targetPosition - enemies[i].transform.position;
            targetVelocity = diffVector.magnitude / angleChangeTime * diffVector.normalized;
            enemies[i].UpdateVelocity(targetVelocity, m_MovementSmoothing);
            // enemies[i].transform.position = targetPosition;
        }
    }

    void UpdateRadius(){
        if(transitioning){
            currentRadius += Mathf.Abs(targetRadius - currentRadius) / (targetRadius - currentRadius) * radiusTransitionSpeed;

            if(Mathf.Abs(targetRadius - currentRadius) <= radiusTransitionSpeed){
                currentRadius = targetRadius;
                transitioning = false;
                statePauseDT = 0f;
            }
        } else {
            statePauseDT += Time.deltaTime;

            if(statePauseDT >= statePauseTime){
                transitioning = true;
                targetRadius = currentRadius == largestRadius ? smallestRadius : largestRadius;
            }
        }
    }

    void DetachDeadChildren(bool roomReset = false){
        if(!roomReset && inactiveRoom){
            return;
        }

        foreach(Enemy enemy in enemies){
            if(!enemy.Alive && enemy.transform.parent != null){
            // if(!enemy.Alive && enemy.gameObject.activeSelf){
                // enemy.gameObject.SetActive(true);
                enemy.transform.parent = null;

                if(++deadEnemyAmount == enemyGenerationAmount){
                    Defeated = true;
                }
            }
        }
    }

    private void OnDrawGizmos(){
		UnityEngine.Gizmos.color = new Color(1f, 0.5f, 0.25f);
        UnityEngine.Gizmos.DrawWireSphere(transform.position, largestRadius);
		UnityEngine.Gizmos.color = new Color(.25f, 0.5f, 1f);
        UnityEngine.Gizmos.DrawWireSphere(transform.position, smallestRadius);
    }

    public override void ResetOnRoomLeave(bool initialRoomSet = false, bool roomResetOverride = false){
        base.ResetOnRoomLeave(initialRoomSet);
        DetachDeadChildren(true);
        angleChangeDT = 0f;
        statePauseDT = 0f;
        transitioning = false;
        if(shouldEnemiesRevive || roomResetOverride){
            deadEnemyAmount = 0;
        }
        InitializeEnemies();
    }

    public override void Revive()
    {
        base.Revive();

        reviving = true;
        angleChangeDT = 0f;
        statePauseDT = 0f;
        transitioning = false;
        deadEnemyAmount = 0;

        currentRadius = startLargestRadius ? largestRadius : smallestRadius;

        angleIncrements = (2 * Mathf.PI) / enemies.Length;
        Vector3 differenceVector;
        for(var i = 0; i < enemies.Length; i++){
            if(enemies[i].transform.parent == null){
                enemies[i].transform.SetParent(transform);
            }
            enemies[i].Revive(false);
            differenceVector = new Vector3(Mathf.Cos(i * angleIncrements), Mathf.Sin(i * angleIncrements), 0) * currentRadius;
            enemies[i].transform.position = transform.position + differenceVector;
            enemyAngles[i] = i * angleIncrements;
        } 
        
        reviving = false;
    }
}
