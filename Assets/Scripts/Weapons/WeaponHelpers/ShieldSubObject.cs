using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSubObject : MonoBehaviour
{
    [SerializeField] Shield parentShield;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sent when an incoming collider makes contact with this object's
    /// collider (2D physics only).
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        switch(other.gameObject.tag){
            case "Enemy":
            case "EnemyMagicBarrier":
                Enemy enemy;
                if(other.gameObject.tag == "Enemy"){
                    enemy = other.gameObject.GetComponent<Enemy>();
                } else {
                    enemy = other.gameObject.GetComponent<MagicBarrier>().ParentEnemy;
                }

                Vector2 pushBackForce = (transform.position - other.transform.position).normalized * -enemy.ShieldHitKnockForceMagnitude;
                enemy.ApplyForce(pushBackForce);

                parentShield.ShieldHit(enemy.PlayerShieldHitStaminaCost);
                break;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        switch(other.gameObject.tag){
            case "EnemyProjectile":
                parentShield.ShieldHit(5);
                break;
        }
    }
}
