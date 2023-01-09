using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SwordBehavior : MonoBehaviour
{
    private Tween floatTween;
    // Start is called before the first frame update
    void Start()
    {
        floatTween = transform.DOMoveY(transform.position.y + .1f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
