using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWavesRoom : Room
{
    [Header("Enemy Waves Room")]
    [SerializeField] Enemy[] enemyTypesToSpawn;
    int currentWaveNumber = 0;
    [SerializeField] int waveAmount = 4;
    [SerializeField] int[] enemiesPerWave;
    [SerializeField] float pauseBetweenWaves = 3f;
    float wavePauseDT = 0f;
    bool pausedBetweenWaves = false;
    [SerializeField] float[] enemySpawnTimes_MIN;
    [SerializeField] float[] enemySpawnTimes_MAX;
    float enemySpawnDT = 0f;
    float enemySpawnTime;
    bool enemySpawningActive = false;
    [SerializeField] Brazier[] braziersToLight;
    bool waveChallengeActive = false;
    bool waveChallengeFinished = false;
    [SerializeField] BoxCollider2D challengeStartTrigger;
    int enemiesSpawnedThisWave = 0;
    int enemiesDefeatedThisWave = 0;
    List<Enemy> currentEnemies;
    [SerializeField] GameObject enemySpawnPositions;
    [Header("Spawnable Items")]
    [SerializeField] GameObject[] spawnableItemsPerRound;
    [SerializeField] float[] dropPercentPerRound;
    [SerializeField] AudioClip waveFinishedSound;
    // [SerializeField] AudioClip 

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        currentEnemies = new List<Enemy>();   
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateWavePause();
        UpdateEnemySpawnTimer();
        UpdateEnemyDefeatedCheck();
    }

    protected virtual void UpdateWavePause(){
        if(!pausedBetweenWaves || !waveChallengeActive){
            return;
        }

        wavePauseDT += Time.deltaTime;

        if(wavePauseDT >= pauseBetweenWaves){
            pausedBetweenWaves = false;
            StartNextWave();
        }
    }

    protected virtual void UpdateEnemySpawnTimer(){
        if(!enemySpawningActive){
            return;
        }

        enemySpawnDT += Time.deltaTime;

        if(enemySpawnDT >= enemySpawnTime){
            SpawnNextEnemy();
            enemySpawnDT = 0f;
        }
    }

    protected virtual void UpdateEnemyDefeatedCheck(){
        if(!waveChallengeActive || waveChallengeFinished || pausedBetweenWaves || enemiesSpawnedThisWave < enemiesPerWave[currentWaveNumber]){
            return;
        }

        int enemiesDefeatedCount = 0;

        for(int i = 0; i < currentEnemies.Count; i++){
            if(!currentEnemies[i].Defeated){
                return;
            }

            enemiesDefeatedCount++;
        }

        if(enemiesDefeatedCount >= enemiesPerWave[currentWaveNumber]){
            EndCurrentWave();
        }
    }

    protected virtual void StartNextWave(){
        Debug.Log("Starting next wave");
        braziersToLight[currentWaveNumber].SetOn(true);

        enemiesSpawnedThisWave = 0;
        enemiesDefeatedThisWave = 0;
        
        enemySpawningActive = true;
        enemySpawnDT = 0f;
        enemySpawnTime = Random.Range(enemySpawnTimes_MIN[currentWaveNumber], enemySpawnTimes_MAX[currentWaveNumber]);
    }

    protected virtual void EndCurrentWave(){
        Debug.Log("Ending current wave");
        foreach(Enemy enemy in currentEnemies){
            GameObject.Destroy(enemy.gameObject);
        }
        currentEnemies.Clear();

        currentWaveNumber++;

        if(currentWaveNumber >= waveAmount){
            waveChallengeFinished = true;
            UnlockObjects();
            return;
        }

        GameMaster.Instance.audioSource.PlayOneShot(waveFinishedSound);
        pausedBetweenWaves = true;
        wavePauseDT = 0f;
    }

    protected virtual void SpawnNextEnemy(){
        Debug.Log("Spawning Next Enemy");
        Enemy newEnemy = GameObject.Instantiate(enemyTypesToSpawn[Random.Range(0, enemyTypesToSpawn.Length)]);
        newEnemy.transform.position = enemySpawnPositions.transform.GetChild(Random.Range(0, enemySpawnPositions.transform.childCount)).transform.position;
        newEnemy.SetSpawnItems(new GameObject[]{spawnableItemsPerRound[currentWaveNumber]}, dropPercentPerRound[currentWaveNumber]);
        newEnemy.SetEngagement(true, GameMaster.Instance.Player, -1);

        currentEnemies.Add(newEnemy);

        enemiesSpawnedThisWave++;

        if(enemiesSpawnedThisWave >= enemiesPerWave[currentWaveNumber]){
            enemySpawningActive = false;
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player"){
            if(waveChallengeActive){
                return;
            }

            currentWaveNumber = 0;
            waveChallengeActive = true;
            StartNextWave();
        }
    }
}
