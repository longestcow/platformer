using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;


public class loadScript : MonoBehaviour
{
    // Start is called before the first frame update
    float time, speed = 0.3f, size;

    Vector3 startScale;
    bool the=true;
    gameState state;
    GameObject crystals;

    void Start()
    {   
        crystals=GameObject.Find("crystals");
        state = GameObject.Find("gameState").GetComponent<gameState>();
        size =  GameObject.Find("cam").GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize*5;
        GameObject.Find("cam").GetComponent<CinemachineVirtualCamera>().m_Follow=null;
        
        gameObject.GetComponent<SpriteRenderer>().color=state.loadColors[state.phase];
        time=Time.time+speed;
        startScale=transform.localScale;
    }

    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, 10)); // to make it visible and move with cam;
        if(the)
            transform.localScale=Vector3.Lerp(startScale, new Vector3(size,size,10), state.mapfunc(time-Time.time, speed, 0, 0, 1));
        else
            transform.localScale=Vector3.Lerp(new Vector3(size,size,10), new Vector3(0,0,10), state.mapfunc(time-Time.time, speed, 0, 0, 1));

        if(transform.localScale==new Vector3(size,size,10)) {
        state.workArounds[state.phase][state.level].GetComponent<SpriteRenderer>().enabled=true;
            foreach (Transform child in crystals.transform) {
                child.gameObject.GetComponent<BoxCollider2D>().enabled=true;
                child.gameObject.GetComponent<SpriteRenderer>().enabled=true;
                child.gameObject.GetComponent<Light2D>().enabled=true;
            }
            GameObject.Find("Player").transform.position = state.spawnPoints[state.phase][state.level].transform.position;
            GameObject.Find("cam").GetComponent<CinemachineVirtualCamera>().m_Follow = GameObject.Find("Player").transform;
            state.player.cam.GetComponent<Cinemachine.CinemachineConfiner>().m_BoundingShape2D = state.confiners[state.phase][state.level].GetComponent<Collider2D>();
            StartCoroutine(wait(0.08f));//replace with load scene
        }
        else if(transform.localScale==new Vector3(0,0,10)) 
            Destroy(gameObject);
    }



    IEnumerator wait(float f){
        yield return new WaitForSeconds(f);
        state.workArounds[state.phase][state.level].GetComponent<SpriteRenderer>().enabled=false;
        time=Time.time+speed;
        transform.Rotate(0,0,45);
        startScale=transform.localScale;
        the=false;
    }
}
