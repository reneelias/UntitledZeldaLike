using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stair_Main : MonoBehaviour
{
    bool containsCharacter = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player"){
            containsCharacter = true;
            other.gameObject.GetComponent<CharacterControls>().StaircaseInteraction(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player"){
            containsCharacter = false;
            other.gameObject.GetComponent<CharacterControls>().StaircaseInteraction(false);
        }
    }
}
