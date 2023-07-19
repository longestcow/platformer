using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotty : MonoBehaviour
{
    float time;

    [Header("Self:")]
    public GameObject bullet, head;
    public int cooldown = 0;
    [Header("Guns:")]
    public grappler grapplerScript;
    public boost boost;

    gameState state;
    int count;

    //mouse
    Vector3 mousePos,rot; 
    float rotZ;
    // Start is called before the first frame update
    void Start()
    {   
        state = GameObject.Find("gameState").GetComponent<gameState>();
        this.enabled=false;
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if(state.shotgun < 1) {
            if(!state.currentguns.Contains(boost.gameObject) && !state.currentguns.Contains(grapplerScript.gameObject))
                print("no more guns"); // error sound
            else {
                if(state.currentguns.Contains(grapplerScript.gameObject))
                    grapplerScript.enabled=true;
                else if(state.currentguns.Contains(boost.gameObject))
                    boost.enabled=true;
            } 
            this.enabled=false;
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            if(!state.currentguns.Contains(boost.gameObject) && !state.currentguns.Contains(grapplerScript.gameObject))
                print("no more guns"); // error sound
            else {
                if(state.currentguns.Contains(grapplerScript.gameObject))
                    grapplerScript.enabled=true;
                else if(state.currentguns.Contains(boost.gameObject))
                    boost.enabled=true;
                this.enabled=false;
            } 
        }

        mousePos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rot=mousePos-transform.position;
        rotZ = Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,rotZ);

        if(Input.GetMouseButtonDown(0) && time-Time.time<=0){
            time=Time.time+cooldown;
            shoot();
        }

    }

    private void OnEnable() {
        GetComponentInChildren<SpriteRenderer>().enabled=true;
    }
    private void OnDisable() {
        GetComponentInChildren<SpriteRenderer>().enabled=false;
    }
    void shoot(){
        Instantiate(bullet, head.GetComponent<Transform>().position, transform.rotation);
        if(state.shotgun < 50) 
            state.shotgun-=1;
    }




}
