using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ennemy : MonoBehaviour
{
    // Start is called before the first frame update
    public int MaxLife = 100;
    public int Life;
    bool dead = false;
    bool seeplayer = false;
    Player Player;
    public ennemystate crtstate = ennemystate.sleeping;
    private float delayStopsearching =5f;
    private float counterSearching = 0f;
    Vector3 BaseRotation;
    private bool hasPlayedSearchingSound = false;
    public UIScript UIRef;

    [Tooltip("frequence de balayage")]
    public float frequency=0.5f;// f<1 augmente la frequence f<1 && f>0 reduit la frequence : frequence de balayge lors de l'etat awake and searching

    //attack management variable
    float cdShoot = 4;
    int dmg = 50;
    float range= 10;
    float lastshoot = 0;

    public LayerMask layertohit;

    float stopaggrodelay = 5;
    bool outofaggro = false;
    float lasttimeseeplayer = 0;

    public AudioManager AudioManager;
    public EnnemyHealthBarScript healthBar;
    public GameObject UIHealthBar;
    public GameObject DetectedUI;
    public GameObject SleepingPS;

    public enum ennemystate //ennemy state and angle of rotation 
    {
        sleeping=0, awake=45, searching=90, attacking=666
    }
    void Start()
    {
        Life = MaxLife;
        BaseRotation = this.transform.localRotation.eulerAngles;//register rotation at the beginning of the level
        healthBar.SetMaxHealth(MaxLife);
        UIHealthBar.SetActive(false);
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
                SleepingPS.SetActive(true);
                break;
            case ennemystate.awake:
                YRot = Mathf.Sin(Time.time * Mathf.PI * frequency) * (float)crtstate;
                this.transform.localRotation = Quaternion.Euler(BaseRotation + YRot * Vector3.up);
                SleepingPS.SetActive(false);
                break;
            case ennemystate.searching:
                YRot = Mathf.Sin(Time.time * Mathf.PI * frequency) * (float)crtstate;
                this.transform.localRotation = Quaternion.Euler(BaseRotation + YRot * Vector3.up);
                SleepingPS.SetActive(false);

                counterSearching -= Time.deltaTime;
                if (counterSearching<=0)
                {
                    PreviousState();
                    hasPlayedSearchingSound = false;
                }
                break;
            case ennemystate.attacking:
                SleepingPS.SetActive(false);
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
                    // UIRef.Hidden();
                    hasPlayedSearchingSound = false;
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
            Debug.DrawRay(this.transform.position, this.transform.forward * range, Color.red, 3f);
            GameObject obj = hit.collider.gameObject;
            Debug.LogError("obj hit : " + obj.name);
            bool isplayer = obj.CompareTag("Player");
            AudioManager.PlayOneShot("Enemy Shooting Sound");
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
                // Debug.Log("Hear the player");
            }

            if (!AudioManager.IsSoundPlaying("Huh Sound") && hasPlayedSearchingSound == false)
            {
            AudioManager.PlayOneShot("Huh Sound");
            hasPlayedSearchingSound = true;
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
            healthBar.SetHealth(Life);
            UIHealthBar.SetActive(true);
            AudioManager.PlayOneShot("Enemy Hurt");

            if (Life <= 0)
            {
                AudioManager.PlayOneShot("Enemy Death");
                // UIRef.Hidden();
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
            AudioManager.Play("Detected Sound");
            DetectedUI.SetActive(true);
            // UIRef.Detected();
            
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
