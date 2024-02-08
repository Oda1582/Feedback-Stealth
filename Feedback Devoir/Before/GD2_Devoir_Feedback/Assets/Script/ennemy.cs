using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ennemy : MonoBehaviour
{
    // Start is called before the first frame update
    public int Life = 100; 
    bool dead = false;
    bool seeplayer = false;
    Player Player;
    public ennemystate crtstate = ennemystate.sleeping;
    private float delayStopsearching =5f;
    private float counterSearching = 0f;
    Vector3 BaseRotation;
    [Tooltip("frequence de balayage")]
    public float frequency=0.5f;// f<1 augmente la frequence f<1 && f>0 reduit la frequence : frequence de balayge lors de l'�tat awake and searching

    //attack management variable
    float cdShoot = 4;
    int dmg = 50;
    float range= 10;
    float lastshoot = 0;

    public LayerMask layertohit;

    float stopaggrodelay = 5;
    bool outofaggro = false;
    float lasttimeseeplayer = 0;

    public enum ennemystate //ennemy state and angle of rotation 
    {
        sleeping=0, awake=45, searching=90, attacking=666
    }
    void Start()
    {
        BaseRotation = this.transform.localRotation.eulerAngles;//register rotation at the beginning of the level
    }

    // Update is called once per frame
    void Update()
    {
        manageState();
    }

    private void manageState()
    {
        float YRot;
        switch (crtstate)
        {
            case ennemystate.sleeping:
                break;
            case ennemystate.awake:
                YRot = Mathf.Sin(Time.time * Mathf.PI * frequency) * (float)crtstate;
                this.transform.localRotation = Quaternion.Euler(BaseRotation + YRot * Vector3.up);
                break;
            case ennemystate.searching:
                YRot = Mathf.Sin(Time.time * Mathf.PI * frequency) * (float)crtstate;
                this.transform.localRotation = Quaternion.Euler(BaseRotation + YRot * Vector3.up);

                counterSearching -= Time.deltaTime;
                if (counterSearching<=0)
                {
                    PreviousState();
                }
                break;
            case ennemystate.attacking:
                if (lastshoot + cdShoot <= Time.time)
                {
                    shoot();
                }
                else
                {
                    if (Player != null)
                    {
                        this.transform.LookAt(Player.gameObject.transform);
                    }
                }

                if (outofaggro == true && lasttimeseeplayer + stopaggrodelay <= Time.time)
                {
                    PreviousState();
                }
                break;

        }
    }
    private void shoot()
    {
        //shoot
        lastshoot = Time.time;
        Ray ray = new Ray();
        ray.direction = this.transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, range, layertohit))
        {
            
            GameObject obj = hit.collider.gameObject;
            Debug.LogError("obj hit : " + obj.name);
            bool isplayer = obj.CompareTag("Player");
            if (isplayer)
            {
                obj.GetComponent<Player>().TakeDamage(dmg);//return if ennemy died from the shoot                       
            }

        }
    }
    public void TriggerhearPlayer(Player player) // hear the player, change the ennemy state // if player is in state "attacking" do nothing
    {
        if(crtstate != ennemystate.attacking)
        {

            Player = player;
            NextState();
            if (crtstate == ennemystate.attacking)
            {
                ChangeState(ennemystate.searching);
            }
        }
       
    }
    private void death()//explicit
    {
        dead = true;
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
    public bool TakeDamage (int damage)// inflict dmg to the ennemy if the ennemy die from dmg call the function to increment the number of ennemy killed by the player
    {
       // Debug.Log("DamageTaken");
        bool deadbyshoot = false;
        if(dead != true)
        {
            Life -= damage;
            if (Life <= 0)
            {
                deadbyshoot = true;
                death();
            }
        }
        return deadbyshoot;
    }

    public void Triggerseeplayer(Player player,bool enter)//change state of the ennemy if the player cross the trigger box of detection
    {
        if(enter)
        {
            //Debug.LogError("Attack playerr : "+ player);
            seeplayer = true;
            Player = player;
            ChangeState(ennemystate.attacking);
            outofaggro = false;
            player.Detected();
            
        }
        else 
        { 
            seeplayer = false;
            outofaggro = true;
            lasttimeseeplayer = Time.time;
        }
        
    }

    void ChangeState(ennemystate newstate)
    {

        crtstate = newstate;
        isNewlysearching();

    }
    void NextState()
    {
        switch(crtstate)
        {
            case ennemystate.sleeping:
                crtstate = ennemystate.awake;
                break;
            case ennemystate.awake:
                crtstate = ennemystate.searching;
                break;
            case ennemystate.searching:
                crtstate = ennemystate.attacking;
                break;
            case ennemystate.attacking:
                //do nothing
                break;
        }

        isNewlysearching();
    }

    void isNewlysearching ()
    {
        if (crtstate == ennemystate.searching)
        {
            counterSearching = delayStopsearching;
        }
    }
    void PreviousState()
    {
        switch (crtstate)
        {
            case ennemystate.sleeping:
                //do nothing
                break;
            case ennemystate.awake:
                crtstate = ennemystate.sleeping;
                break;
            case ennemystate.searching:
                crtstate = ennemystate.awake;
                break;
            case ennemystate.attacking:
                crtstate = ennemystate.searching;
                break;
        }

        isNewlysearching();
    }
}
