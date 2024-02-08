using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    // Start is called before the first frame update
    float speed=125;
    int damage= 10;
    bool bActivebullet = false;
    bool bDestroyed = false;
    
    Rigidbody rb;
    GameObject Emmiter =null;
    public Hitmarker refHitmarker;
    public Player refPlayer;
    public AudioManager AudioManager;
    public GameObject HitPrefab;
    Collision collisionbehaviour;

    void Start()
    {

    }
    public void ActiveBullet(Vector3 direction,GameObject emmiter,int dmg)
    {
        Emmiter = emmiter;
        damage = dmg;
        bActivebullet = true;
        this.transform.forward = direction;
        rb = this.GetComponent<Rigidbody>();
        if (bActivebullet)
        {
            rb.velocity = this.transform.forward * speed;
        }

    }
//     public void ActiveChargedBullet(Vector3 direction, GameObject emmiter, int dmg, float spd,float size)
//     {
//         bActivebullet = true;
//         Emmiter = emmiter;
//         this.transform.forward = direction;
//         damage = dmg;
//         speed = 100 * spd;
//         if(speed<75)
//         {
//             speed = 75;
//         }
//         if(size <1)
//         {
//             size = 1;
//         }

//         size = 3;
//         this.gameObject.transform.localScale = Vector3.one * size;     

//         rb = this.GetComponent<Rigidbody>();
//         if (bActivebullet)
//         {
//             rb.velocity = this.transform.forward * speed;
//         }
//     }

    public void Destroybullet()
    {
        if(bActivebullet )
        {
            bDestroyed = true;
            bActivebullet = false;
            rb.velocity = Vector3.zero;
            if (this.GetComponent<MeshRenderer>() == null)
            {
                //Debug.Log("No mesh renderer");
            } 
            else 
            {
                this.GetComponent<MeshRenderer>().enabled = false;
                //Debug.Log("yes mesh renderer");
            }

            Destroy(this.gameObject);
            this.gameObject.SetActive(false);
        }       

    }
//     // Update is called once per frame
    void Update()
    {
        if(bActivebullet && bDestroyed ==false)
        {
            //if (Vector3.Distance(this.transform.position, Emmiter.transform.position) > 1000) { Destroybullet(); }
        }
    }

//     // Action de la balle en fonction de sa collision
    private void OnCollisionEnter(Collision collision)
    {
        collisionbehaviour = collision; 
        if (collision.gameObject.GetComponentInParent<ennemy>())
        {
            if (Emmiter != null && Emmiter.GetComponentInParent<Player>()!=null)
            {
                //Debug.Log("Emmiter" + Emmiter);
                Debug.Log("Touché ennemi");
                collision.gameObject.GetComponentInParent<ennemy>().TakeDamage(damage);
                BulletBehaviour();
                BulletHitBehaviour();
                AudioManager.PlayOneShot("Hit Ennemy Sound");
                Destroybullet();
            }
            else if (Emmiter != null && Emmiter.GetComponent<ennemy>() != null)
            {
                // do nothing, affiche le nom de qui a raté
                //Debug.Log("Emmiter" + Emmiter.GetComponent<Ennemy>());
                //Debug.Log("L'ennemi a tiré");
            }
            else
            {
                Debug.Log("Emmiter" );
                collision.gameObject.GetComponentInParent<ennemy>().TakeDamage(damage);
                //Impossible d'arriver
                //Debug.Log("Un ennemi a tué un ennemi");
                AudioManager.PlayOneShot("Hit Ennemy Sound");
                Destroybullet();
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && Emmiter != null && Emmiter.GetComponentInParent<Player>()!=null)
        {
            BulletBehaviour();
            BulletHitBehaviour();
            AudioManager.PlayOneShot("Hit Wall Sound");
            //Debug.Log("Tu as touché obstacle");
            Destroybullet();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            AudioManager.PlayOneShot("Hit Wall Sound");
            //Debug.Log("Ils ont touché un obstacle");
            Destroybullet();
        }
        else if (collision.gameObject.GetComponentInParent<Player>())
        {
            collision.gameObject.GetComponentInParent<Player>().TakeDamage(damage);
            //Debug.Log("Tu as été touché");
            AudioManager.PlayOneShot("Hit Ennemy Sound");
            Destroybullet();
        }
        else
        {
            //Debug.Log("Vous ou un ennemi a tiré sur un mur");
            if (Emmiter != null && Emmiter.GetComponentInParent<Player>()!=null)
            {
                BulletHitBehaviour(); 
            }
            AudioManager.PlayOneShot("Hit Wall Sound");
            Destroybullet();
        }
    }

    private void BulletBehaviour()
    {
        refHitmarker.Start();
        refHitmarker.LaunchHitmarker();
        AudioManager.PlayOneShot("Hitmarker Sound");
    }

    public void BulletHitBehaviour()
    {
        ContactPoint contact = collisionbehaviour.contacts [0];
        Quaternion rot = Quaternion.FromToRotation (Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        if (HitPrefab != null)
        {
        var hitVFX = Instantiate (HitPrefab, pos, rot);

        var psHit = hitVFX.GetComponent<ParticleSystem> ();
        if (psHit != null){
            Destroy (hitVFX, psHit.main.duration);
        }
        else 
        {
            var psChild = hitVFX.transform.GetChild (0).GetComponent<ParticleSystem> ();
            Destroy (hitVFX, psChild.main.duration);
        }
        }
    }
}
