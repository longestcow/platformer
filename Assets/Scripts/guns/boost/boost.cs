using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boost : MonoBehaviour
{
    //guns
    public grappler grap;
    public Shotty shot;
    public bool boosting;
    //self
    public GameObject head,player,ghost,ghosT;
    Rigidbody2D playerRB;
    public List<int> layers = new List<int>();
    SpriteRenderer playerSprite;
    gameState state;
    public int range = 2;
    //mouse
    Vector3 rot;
    Vector2  mousePos,dashingDir;
    float rotZ;
    void Start()
    {
        state=GameObject.Find("gameState").GetComponent<gameState>();
        head=GameObject.Find("head");
        player=GameObject.Find("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        playerSprite = player.GetComponentInChildren<SpriteRenderer>();
        this.enabled=false; 
    }

    // Update is called once per frame
    void Update()
    {
        if(boosting)
            return;
        moveToMouse();

        if(state.boost < 1){
            if(!state.currentguns.Contains(shot.gameObject) && !state.currentguns.Contains(grap.gameObject))
                print("no more guns"); // error sound
            else {
                if(state.currentguns.Contains(shot.gameObject))
                    shot.enabled=true;
                else if(state.currentguns.Contains(grap.gameObject))
                    grap.enabled=true;
            } 
            this.enabled=false;
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            if(!state.currentguns.Contains(shot.gameObject) && !state.currentguns.Contains(grap.gameObject))
                print("no more guns"); // error sound
            else {
                if(state.currentguns.Contains(shot.gameObject))
                    shot.enabled=true;
                else if(state.currentguns.Contains(grap.gameObject))
                    grap.enabled=true;
                this.enabled=false;
            } 
        }

        if(Input.GetKeyDown(KeyCode.Mouse0)) {
            dash();
        }





    }

   private void dash(){
        playerRB.velocity=Vector2.zero;
        playerRB.gravityScale = 0;
        playerRB.velocity+= (Vector2)(-(mousePos-playerRB.position).normalized * 10);
        StartCoroutine(endBoost());
    }
    
    IEnumerator endBoost() {
        for(int i = 0; i<3; i++) {

            ghosT=Instantiate(ghost, player.transform.position, player.transform.rotation);
            ghosT.transform.localScale = playerSprite.transform.localScale+new Vector3(playerSprite.transform.localScale.x/3f,playerSprite.transform.localScale.y/3f,0);
            Color the; ColorUtility.TryParseHtmlString("#0094CF", out the);
            the.a = state.mapfunc(96, 0, 255, 0, 1);
            ghosT.GetComponent<SpriteRenderer>().color = the;
            yield return new WaitForSeconds(0.5f/3);
        }
        playerRB.velocity=Vector2.zero;
        playerRB.gravityScale = 0.5f;
        StartCoroutine(resetGravity());
        boosting=false;
        if(state.boost < 30)
            state.boost-=1;
    }

    private void OnEnable() {
        GetComponentInChildren<SpriteRenderer>().enabled=true;
    }
    private void OnDisable() {
        GetComponentInChildren<SpriteRenderer>().enabled=false;
    }

    IEnumerator resetGravity(){
        yield return new WaitForSeconds(0.6f);
        playerRB.gravityScale=1;
    }

    void moveToMouse(){
        mousePos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rot=(Vector3)mousePos-transform.position;
        rotZ = Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,rotZ);
    }

}
