using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostDie : MonoBehaviour
{
    SpriteRenderer rend;
    bool the = false;
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        StartCoroutine(wait(0.1f));
    }
    void Update() {
        if(the){
            Color temp = rend.color;
            temp.a -= Time.deltaTime * 2;
            rend.color = temp;
            if(temp.a <= 0) 
                Destroy(gameObject);
        }
    }

    IEnumerator wait(float f){
        yield return new WaitForSeconds(f);
        the=true;
    }
}
