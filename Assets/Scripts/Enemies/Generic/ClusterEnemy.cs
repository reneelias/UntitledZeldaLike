using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterEnemy : MonoBehaviour, IDefeatable
{
    [SerializeField] protected bool generateEnemies;
    [SerializeField] protected Enemy enemyPrefab;
    [SerializeField] protected int enemyGenerationAmount;
    protected Enemy[] enemies;
    protected Vector3 spawnPosition;
    // Start is called before the first frame update
    [SerializeField] protected FollowScript followScript;
    public bool shouldEnemiesRevive = false;
    public bool shouldEnemiesResetHealth = true;
    protected int deadEnemyAmount = 0;
    protected bool inactiveRoom = true;

    public bool Defeated{
        protected set; get;
    } = false;

    protected virtual void Start()
    {
        spawnPosition = transform.position;
        Defeated = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void ActivateOnRoomEnter(){
        gameObject.SetActive(true);

        foreach(Enemy enemy in enemies){
            enemy.gameObject.SetActive(true);
        }

        inactiveRoom = false;
    }

    public virtual void ResetOnRoomLeave(bool initialRoomSet = false, bool roomResetOverride = false){
        transform.position = spawnPosition;

        foreach(Enemy enemy in enemies){
            if(initialRoomSet){
                enemy.gameObject.SetActive(false);
                continue;
            }

            enemy.ResetOnRoomLeave();
        }

        if(shouldEnemiesRevive || roomResetOverride){
            Defeated = false;
        }

        inactiveRoom = !initialRoomSet;
        gameObject.SetActive(false);
    }

    public virtual void Revive(){

    }
}
