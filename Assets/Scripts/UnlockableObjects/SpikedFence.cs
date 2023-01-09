using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikedFence : MonoBehaviour, IUnlocklableObject
{
    [SerializeField] bool unlocked = false;
    [SerializeField] AudioClip unlockingSound;
    // Start is called before the first frame update
    void Start()
    {
        if(unlocked){
            Unlock();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Unlock(){
        GameMaster.Instance.audioSource.PlayOneShot(unlockingSound);
        gameObject.SetActive(false);
    }
}
