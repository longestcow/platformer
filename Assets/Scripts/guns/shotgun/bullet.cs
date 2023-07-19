using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public int force = 700;
    public float impactField, impactPower = 6;
    public LayerMask playerLayer;
    public ParticleSystem ps;
    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.right * force);  
        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.name!="Player"){
            explode();
            Destroy(gameObject);
            Instantiate(ps, transform.position, transform.rotation);            
        }
    }
    void explode(){
        Collider2D[] hitObjects =   Physics2D.OverlapCircleAll(transform.position, impactField, playerLayer);
        foreach(Collider2D obj in hitObjects)
            if(obj.gameObject.name=="Player")
                obj.GetComponent<Rigidbody2D>().AddForce((obj.transform.position-transform.position).normalized * impactPower, ForceMode2D.Impulse);
                //rb.velocity=Vector2.up*jumpVelocity;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position, impactField);
    }



}
