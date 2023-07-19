using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


using TMPro;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    bool isGrounded, wasGrounded, isDashing, canDash;
    GameObject shotgunCrystal;
    public float moveSpeed = 7, camSpeed = 0.8f;
    [Header("Jump:")] 
    public float fullMult = 4f;
    public float lowMult = 5f;
    public float jumpVelocity = 6.3f;
    public float fallOff = 3;

    [Header("Dash:")] 
    public float dashSpeed = 20;
    public float  dashTime = 0.12f;
    public float dashCooldown = 0.5f;
        
    [Header("References:")] 
    public LayerMask groundLayer;
    public GameObject shotgun, grappler, boost, ghost;
    public Animator animator;
    public ParticleSystem landPS;
    public GameObject load;
    public TMP_Text dialogue;
    [HideInInspector] public Cinemachine.CinemachineVirtualCamera cam;
    Cinemachine.CinemachineBasicMultiChannelPerlin noise;
    
    float coyoteTime = 0.1f, coyoteCount, bufferTime = 2f, bufferCount, facingDirection = 1, dTime, dCooldown, dCount, crystalNumber, tempFull, tempLow;
    [HideInInspector] public int dNums,jNums;


    Vector2 dashingDir;
    List<Coroutine> corsList = new List<Coroutine>(), dialogues = new List<Coroutine>();
    ParticleSystem temp_landps;
    gameState state;

    // Start is called before the first frame update
    void Start()
    {
        tempFull=fullMult; tempLow = lowMult;
        shotgunCrystal = GameObject.Find("oneShotgun");
        state = GameObject.Find("gameState").GetComponent<gameState>();
        cam=GameObject.Find("cam").GetComponent<Cinemachine.CinemachineVirtualCamera>();
        noise = cam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin> ();
        rb = GetComponent<Rigidbody2D>(); 
        transform.position=state.spawnPoints[state.phase][state.level].transform.position;
        Noise(0,0);
    }

    // Update is called once per frame
    void Update()
    {


        cam.m_Lens.OrthographicSize = Mathf.Clamp(cam.m_Lens.OrthographicSize-Input.GetAxis("Mouse ScrollWheel")*camSpeed, 0.3f,8);
        facingDirection = (Input.GetAxisRaw("Horizontal") == 0)? facingDirection:Input.GetAxisRaw("Horizontal");
        gameObject.transform.eulerAngles = (Input.GetAxis("Horizontal")>0.2)?new Vector3(0,0,-7):(Input.GetAxis("Horizontal")<-0.2)?new Vector3(0,0,7):new Vector3(0,0,0); //no rotation 

        //position guns
        shotgun.transform.position = transform.position;
        grappler.transform.position = transform.position;
        boost.transform.position = transform.position;

        //position dialogue box
        dialogue.transform.position = transform.position + new Vector3(0,1,0);

        
        if(Input.GetKeyDown(KeyCode.R))
            death();

        if(isDashing) {
            if(dTime-Time.time <= 0) {
                isDashing=false;
                rb.velocity=Vector2.zero;
                dashingDir=Vector2.zero;
                rb.gravityScale=1;
            }
            else 
                return;
        }
        if(boost.GetComponent<boost>().boosting)
            return;
        
        isGrounded=IsGrounded();

        if(isGrounded) //coyote
            coyoteCount = coyoteTime;
        else
            coyoteCount-=Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.W)) { //buffer
            bufferCount=bufferTime;
        }
        else
            bufferCount-=Time.deltaTime;

        if(!(Input.GetKeyUp(KeyCode.D) && Input.GetKeyUp(KeyCode.A))) //movement
            rb.velocity = (new Vector2((Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))?0:Input.GetAxis("Horizontal") * moveSpeed, rb.velocity.y));
    

        if((bufferCount>0f && coyoteCount>0f)) {//jump start
            temp_landps = Instantiate(landPS, GameObject.Find("Player/anchor").transform.position, transform.rotation);

            rb.velocity=Vector2.up*jumpVelocity;
            bufferCount=0f;
        }
        else if(bufferCount>0f && jNums>0){
            temp_landps = Instantiate(landPS, GameObject.Find("Player/anchor").transform.position, transform.rotation);
            var main = temp_landps.main;
            var emission = temp_landps.emission;
            emission.rateOverTime=30;
            main.startColor = new Color(state.mapfunc(233,0,255,0,1),state.mapfunc(116,0,255,0,1),0);
            if(jNums<40) jNums-=1;
            rb.velocity=Vector2.up*(jumpVelocity*1.1f);
            bufferCount=0f;
            
        }

        if(isGrounded && !wasGrounded) {
            Instantiate(landPS, GameObject.Find("Player/anchor").transform.position, transform.rotation);
            animator.SetTrigger("landing");
        }

        if(rb.velocity.y < fallOff) { //jump end big
            rb.velocity += (Vector2.up * Physics2D.gravity.y * ((grappler.GetComponent<grappler>().enabled)?fullMult-1:fullMult)  * Time.deltaTime);
        }
        else if(rb.velocity.y > 0 && !Input.GetKey(KeyCode.W)) { //jump end small
             rb.velocity += Vector2.up * Physics2D.gravity.y * ((grappler.GetComponent<grappler>().enabled)?lowMult-1:lowMult) * Time.deltaTime;
             coyoteCount=0f;
        }
        
        //dash
        if(!isDashing && dCooldown-Time.time<=0 && dCount>0 &&dNums>0)
            canDash=true;
        else
            canDash=false;
        if(Input.GetKey(KeyCode.LeftShift) && canDash){
            dash(Input.GetAxisRaw("Horizontal"));
        }
        wasGrounded=isGrounded;
       
}
    bool IsGrounded(){
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.4f, groundLayer);
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position + new Vector3(0.11f,0f), Vector2.down, 0.4f, groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + new Vector3(-0.11f,0f), Vector2.down, 0.4f, groundLayer);
        if(hit.collider!=null || hit1.collider!=null || hit2.collider!=null){
            dCount=(dCount==0)?1:dCount;
            return true;
        }
        return false;
    }

    private void dash(float xAxis){
        rb.velocity=Vector2.zero;
        rb.gravityScale=0;
        StartCoroutine(camShake(0.2f));
        isDashing=true;
        dashingDir = (xAxis==0)?new Vector2(facingDirection, 0):new Vector2(xAxis, 0);
        StartCoroutine(dashTrail(3));
        rb.velocity+=dashingDir.normalized*dashSpeed;
        dTime = Time.time + dashTime;
        dCooldown = Time.time + dashCooldown;
        dCount-=1;
        if(dNums < 50)
            dNums-=1;
    }
    
    IEnumerator dashTrail(int j){
        for(int i = 0; i<j; i++) {
            Instantiate(ghost, transform.position, transform.rotation).transform.localScale = GetComponentInChildren<SpriteRenderer>().transform.localScale+new Vector3(GetComponentInChildren<SpriteRenderer>().transform.localScale.x/3f,GetComponentInChildren<SpriteRenderer>().transform.localScale.y/3f,0);
            yield return new WaitForSeconds(dashTime/j);
        }
        isDashing=false;
        foreach(Coroutine cor in corsList) 
            StopCoroutine(cor);
        corsList.Add(StartCoroutine(floatForDash()));
    }
    public IEnumerator floatForDash(){  
        fullMult = 0; lowMult = 0;
        yield return new WaitForSeconds(0.15f);
        fullMult = tempFull; lowMult = tempLow;
        rb.gravityScale=1;
    }

    public IEnumerator camShake(float f){
        Noise(1.3f,10);
        yield return new WaitForSeconds(f);
        Noise(0,0);
        
    }

    public void Noise(float amplitudeGain, float frequencyGain) {
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;    
    }

    

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.layer==9) 
            death();
         if(other.gameObject.layer==11) 
            finish();
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 10) {
            if(other.gameObject.name.Contains("big")) {
                FindObjectOfType<RippleEffect>().refractionStrength=0.4f;
                FindObjectOfType<RippleEffect>().reflectionStrength=0.4f;
                crystalNumber=50;
            }
            else{
                FindObjectOfType<RippleEffect>().refractionStrength=0.2f;
                FindObjectOfType<RippleEffect>().reflectionStrength=0.25f;
                crystalNumber=1;
            }
            FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

            if(other.gameObject.name.Contains("Shotgun")) {
                state.shotgun+=(int)crystalNumber;
                shotgun.GetComponent<Shotty>().enabled=true;
                grappler.GetComponent<grappler>().enabled=false;
                boost.GetComponent<boost>().enabled=false;
            }
            else if(other.gameObject.name.Contains("Grappler")) {
                state.grappler+=(int)crystalNumber;
                shotgun.GetComponent<Shotty>().enabled=false;
                grappler.GetComponent<grappler>().enabled=true;
                boost.GetComponent<boost>().enabled=false;
            }
            else if(other.gameObject.name.Contains("Boost")){
                state.boost+=(int)crystalNumber; 
                shotgun.GetComponent<Shotty>().enabled=false;
                grappler.GetComponent<grappler>().enabled=false;
                boost.GetComponent<boost>().enabled=true;
            }
            else if(other.gameObject.name.Contains("Dash")){
                dNums+=(int)crystalNumber; 
                dCooldown=Time.time;
            }
            else if(other.gameObject.name.Contains("Jump")){
                jNums+=(int)crystalNumber; 

            }
            dCount=(dCount==0)?1:dCount;
            other.enabled=false;
            other.gameObject.GetComponent<SpriteRenderer>().enabled=false;
            other.gameObject.GetComponent<Light2D>().enabled=false;
        }

    }

    void death() {
        GameObject.Find("rope").GetComponent<grapplingRope>().enabled=false;
        clearDialogue();
        transform.position=new Vector3(-20,20,0);
        Quaternion the = Quaternion.Euler(0,0,Random.Range(40, 80));
        Instantiate(load, new Vector3(0,0,10), the, GameObject.Find("cam").transform);
        //tries-=1
        state.reset();
    }


    void finish() {
        state.nextLevel();
    }


    public void dialogueChange(string s, float f = 5) {
        clearDialogue();
        //list.add()
        dialogues.Add(StartCoroutine(changeText(s, f)));
    }

    IEnumerator changeText(string s, float f = 5f){
        
        for(int i = 0; i<=s.Length; i++) {
            yield return new WaitForSeconds(0.03f);
            dialogue.text = s.Substring(0,i);
        }
        yield return new WaitForSeconds(f);
        clearDialogue();
    }

    public void clearDialogue(){
        foreach(Coroutine dia in dialogues){
            StopCoroutine(dia);
        }
        dialogue.text="";
    }


}
