using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence_Brazier : MonoBehaviour
{
    [SerializeField] Brazier parentBrazier;
    [SerializeField] GameObject frontFence;
    [SerializeField] GameObject backFence;
    [SerializeField] GameObject leftFence;
    [SerializeField] GameObject rightFence;
    // Start is called before the first frame update
    void Start()
    {
        SetFenceLayerOrders();
    }

    void SetFenceLayerOrders(){
        foreach(Transform fenceTransform in frontFence.transform){
            GameObject fenceObj = fenceTransform.gameObject;

            fenceObj.GetComponent<SpriteRenderer>().sortingLayerID = parentBrazier.gameObject.GetComponent<SpriteRenderer>().sortingLayerID;
            fenceObj.GetComponent<SpriteRenderer>().sortingOrder = parentBrazier.gameObject.GetComponent<SpriteRenderer>().sortingOrder + 2;
        }

        foreach(Transform fenceTransform in backFence.transform){
            GameObject fenceObj = fenceTransform.gameObject;

            fenceObj.GetComponent<SpriteRenderer>().sortingLayerID = parentBrazier.gameObject.GetComponent<SpriteRenderer>().sortingLayerID;
            fenceObj.GetComponent<SpriteRenderer>().sortingOrder = parentBrazier.gameObject.GetComponent<SpriteRenderer>().sortingOrder - 2;
        }

        int orderModifier = 1;
        foreach(Transform fenceTransform in leftFence.transform){
            GameObject fenceObj = fenceTransform.gameObject;

            fenceObj.GetComponent<SpriteRenderer>().sortingLayerID = parentBrazier.gameObject.GetComponent<SpriteRenderer>().sortingLayerID;
            fenceObj.GetComponent<SpriteRenderer>().sortingOrder = parentBrazier.gameObject.GetComponent<SpriteRenderer>().sortingOrder + orderModifier;
            orderModifier--;
        }

        orderModifier = 1;
        foreach(Transform fenceTransform in rightFence.transform){
            GameObject fenceObj = fenceTransform.gameObject;

            fenceObj.GetComponent<SpriteRenderer>().sortingLayerID = parentBrazier.gameObject.GetComponent<SpriteRenderer>().sortingLayerID;
            fenceObj.GetComponent<SpriteRenderer>().sortingOrder = parentBrazier.gameObject.GetComponent<SpriteRenderer>().sortingOrder + orderModifier;
            orderModifier--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
