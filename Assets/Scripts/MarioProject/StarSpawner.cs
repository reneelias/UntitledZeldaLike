using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject starPrefab;
    void Start()
    {
        GameObject tempStar;
        float randScale;
        for(int i = 0; i < 50; i++)
        {
            tempStar = Instantiate(starPrefab, new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0), Quaternion.identity);
            randScale = Random.Range(-.08f, -.04f);
            tempStar.transform.localScale += new Vector3(randScale, randScale, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
