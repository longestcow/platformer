using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunCount : MonoBehaviour
{
    
    public int gunNum;
    int count = 0,statenum;
    gameState state;
    GameObject gun;
    Player player;
    SpriteRenderer sprite;
    Vector3 pos;
    GameObject parent;
    void Start()
    {
        parent = GameObject.Find("counts");
        gun = GameObject.Find(gunNum==0?"shotty":gunNum==1?"grappler":gunNum==2?"boost":"shotty");
        player = GameObject.Find("Player").GetComponent<Player>();
        sprite = GetComponent<SpriteRenderer>();
        state = GameObject.Find("gameState").GetComponent<gameState>();
    }
    // Update is called once per frame
    void Update()
    {

        statenum = (gunNum==0)?state.shotgun:(gunNum==1)?state.grappler:(gunNum==2)?state.boost:(gunNum==3)?player.jNums:(gunNum==4)?player.dNums:state.shotgun;
        if(gunNum<3){
            if(state.currentguns.Contains(gun) && !sprite.enabled) {
                pos = new Vector3(387, 142, 0);
                foreach(Transform child in parent.transform){
                    if(child!=transform)
                        if(child.GetComponent<SpriteRenderer>().enabled){
                            pos-=new Vector3(23, 0, 0);
                            print(pos);
                        }
                }
                transform.localPosition = pos;
                sprite.enabled=true;
            }
            else if(!state.currentguns.Contains(gun) && sprite.enabled)
                sprite.enabled=false;
        }
        else {
            if(statenum > 0 && !sprite.enabled){
                pos = new Vector3(387, 142, 0);
                foreach(Transform child in parent.transform){
                    if(child!=transform)
                        if(child.GetComponent<SpriteRenderer>().enabled){
                            pos-=new Vector3(23, 0, 0);
                            print(pos);
                        }
                }
                transform.localPosition = pos;
                sprite.enabled=true;
            }
            else if(statenum <= 0 && sprite.enabled)
                sprite.enabled=false;
        }
            
        if(count != statenum){
            count=statenum;
            if(count < 40) {
                gameObject.transform.GetChild(gameObject.transform.childCount-1).GetComponent<SpriteRenderer>().enabled = false;              
                for(int i = 0; i<gameObject.transform.childCount-1; i++)
                    gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = (i<count);
            }
            else {
                for(int i = 0; i<gameObject.transform.childCount-1; i++)
                    gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
                gameObject.transform.GetChild(gameObject.transform.childCount-1).GetComponent<SpriteRenderer>().enabled = true;              
            }

        }
    }
}
