using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    bool keyUsed = false;
    [SerializeField] Door parentDoor;
    [SerializeField] bool isBossDoor = false;
    bool nonKeyLock = false;
    void Start()
    {
         Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.Find("WizardBoxCollider").GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNonKeyLock(bool nonKeyLock = true){
        this.nonKeyLock = nonKeyLock;
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Player" && !keyUsed && !nonKeyLock){
            if(isBossDoor && GameMaster.Instance.dungeon.MasterKeyAcquired){
                GameMaster.Instance.dungeon.BossDoorUnlocked();
                parentDoor.UnlockDoor();
                keyUsed = true;
                return;
            }
            if(!isBossDoor && GameMaster.Instance.dungeon.KeyCount > 0){
                GameMaster.Instance.dungeon.ModifyKeyCount(-1);
                parentDoor.UnlockDoor();
            }

        }
    }
}
