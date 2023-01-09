using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_ItemDropper : MonoBehaviour
{
    [SerializeField] GameObject[] spawnableItemTypes;
    [Range(0, 1f)][SerializeField] float dropPercent = 1f;
    [SerializeField] bool smartDrop = false;
    [Range(.5f, 1f)][SerializeField] float smartDropRate = .75f;
    [Range(0f, 1f)][SerializeField] float smartDropHpRatio = .6f;
    [SerializeField] bool diminishingReturns = false;
    [Range(0f, 1f)][SerializeField] float diminshRate = .25f;
    [Range(0f, .5f)][SerializeField] float diminishBottom = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    protected void DropItem(){
        float randNum = Random.Range(0, 1f);

        if(randNum < dropPercent && spawnableItemTypes.Length > 0){
            GameObject dropItem = Instantiate(spawnableItemTypes[(int)Random.Range(0, spawnableItemTypes.Length)]);

            if(smartDrop && GameMaster.Instance.Player.GetComponent<PlayableCharacter>().HpRatio < smartDropHpRatio){
                float smartRand = Random.Range(0f, 1f);
                if(smartRand < smartDropRate){
                    foreach(GameObject item in spawnableItemTypes){
                        if(item.name == "Heart"){
                            GameObject.Destroy(dropItem);
                            dropItem = Instantiate(item);
                            break;
                        }
                    }
                }
            }

            dropItem.transform.position = transform.position;

            if(dropItem.tag == "PickUp"){
                dropItem.GetComponent<IPickup>().Activate();
                GameMaster.Instance.dungeon.CurrentRoom.AddObjToDesapwn(dropItem);
            }
        }

        if(diminishingReturns){
            dropPercent = Mathf.Max(dropPercent - diminshRate, diminishBottom);       
        }
    }
}
