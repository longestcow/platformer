using UnityEngine;

public class grappler : MonoBehaviour
{
    [Header("Changeable:")]
    public LayerMask grappableLayerMask;
    public float range = 7;
    public float launchSpeed = 0.8f;
    public float breakAfterSeconds = 1f;

    [Header("References:")]
    public grapplingRope grappleRope;
    public Shotty shotty;
    public boost boost;
    public Camera _camera;
    public Transform gunHolder;
    public Transform firePoint;
    public SpriteRenderer hpPrefab;
    SpriteRenderer hitPoint;

    [Header("Physics References:")]
    public SpringJoint2D springJoint2D;
    public Rigidbody2D rb;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;
    
    bool wasGrappling;

    gameState state;
    float time = 0f;

    private void Start()
    {
        state = GameObject.Find("gameState").GetComponent<gameState>();
        grappleRope.enabled = false;
        springJoint2D.enabled = false;

    }
//the point set in down used in key
    private void Update()
    {
        if(!grappleRope.isGrappling && wasGrappling)
            if(state.grappler < 50)
                state.grappler-=1;
        wasGrappling = grappleRope.isGrappling;

        if(state.grappler < 1) {
            if(!state.currentguns.Contains(boost.gameObject) && !state.currentguns.Contains(shotty.gameObject))
                print("no more guns"); // error sound
            else {
                if(state.currentguns.Contains(boost.gameObject))
                    boost.enabled=true;
                else if(state.currentguns.Contains(shotty.gameObject))
                    shotty.enabled=true;
            } 
            this.enabled=false;
        }

        

        if(Input.GetKeyDown(KeyCode.Space)){
            if(!state.currentguns.Contains(boost.gameObject) && !state.currentguns.Contains(shotty.gameObject))
                print("no more guns"); // error sound
            else {
                if(state.currentguns.Contains(boost.gameObject))
                    boost.enabled=true;
                else if(state.currentguns.Contains(shotty.gameObject))
                    shotty.enabled=true;
                this.enabled=false;
            } 
        }



        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            time=Time.time+breakAfterSeconds;
            SetGrapplePoint();

        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            if(Time.time>=time) {
                if(hitPoint!=null)
                    hitPoint.enabled=false;
                grappleRope.enabled = false;
                springJoint2D.enabled = false;
                rb.gravityScale = 1;
            }
            
            if (grappleRope.enabled) 
                moveTo(grapplePoint);

            else {
                moveTo(_camera.ScreenToWorldPoint(Input.mousePosition));
                RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.transform.right.normalized, range, grappableLayerMask);
                Debug.DrawRay(firePoint.position, firePoint.transform.right.normalized * range, Color.blue);
                if(hit.collider!=null) {
                    hitPoint.gameObject.transform.position=hit.point;
                    hitPoint.enabled=true;
                }
                else if(hitPoint!=null)
                    hitPoint.enabled=false;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            hitPoint.enabled=false;
            grappleRope.enabled = false;
            springJoint2D.enabled = false;
            rb.gravityScale = 1;
        }
        else {
            moveTo(_camera.ScreenToWorldPoint(Input.mousePosition));
            RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.transform.right.normalized, range, grappableLayerMask);
            Debug.DrawRay(firePoint.position, firePoint.transform.right.normalized * range, Color.blue);
            if(hitPoint!=null){
                if(hit.collider!=null){
                    hitPoint.gameObject.transform.position=hit.point;
                    hitPoint.enabled=true;
                }
                else
                    hitPoint.enabled=false;
            }
        }
    }
    private void OnEnable() {
        hitPoint=Instantiate(hpPrefab, transform.position, Quaternion.identity);
        GetComponentInChildren<SpriteRenderer>().enabled=true;
    }
    private void OnDisable() {
        foreach(GameObject hitpoints in GameObject.FindGameObjectsWithTag("grapplerHP"))
            Destroy(hitpoints);
        grappleRope.enabled = false;
        if(springJoint2D!=null)
            springJoint2D.enabled = false;
        GetComponentInChildren<SpriteRenderer>().enabled=false;
        
    }

    void moveTo(Vector3 lookPoint)
    {
        Vector2 rot=lookPoint-transform.position;
        float rotZ = Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,rotZ);

    }

    void SetGrapplePoint()
    {
        Vector2 distanceVector = _camera.ScreenToWorldPoint(Input.mousePosition) - gunHolder.position;
        if (Physics2D.Raycast(firePoint.position, distanceVector.normalized))
        {
            RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized);
            if ( Physics2D.Raycast(firePoint.position, distanceVector.normalized, range, grappableLayerMask).collider!=null)
            {
                print(_hit.collider.gameObject.name);
                if (Vector2.Distance(_hit.point, firePoint.position) <= range)
                {
                    grapplePoint = _hit.point;
                    hitPoint.transform.position=grapplePoint;
                    grappleDistanceVector = grapplePoint - (Vector2)gunHolder.position;
                    grappleRope.enabled = true;
                }
            }
        }
    }

    public void Grapple()
    {
            springJoint2D.autoConfigureDistance = false;
            springJoint2D.connectedAnchor = grapplePoint;
            Vector2 distanceVector = firePoint.position - gunHolder.position;

            springJoint2D.distance = distanceVector.magnitude;
            springJoint2D.frequency = launchSpeed;
            springJoint2D.enabled = true;
            

    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, range);
        }
    }

}