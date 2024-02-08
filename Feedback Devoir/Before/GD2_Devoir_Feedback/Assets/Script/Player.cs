using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public enum movementspeeds// explicit
{
    nomove =0, crouchspeed = 5, walkspeed = 10, runspeed = 15
}

[System.Serializable]
public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    bool shoot = false;// is player  button to shoot pressed  
    bool aim = false;// is player  button to aim pressed  
    bool crouch = false;// is player  button to crouch pressed 
    bool run = false;// is player button to run pressed 
    //bool reload = false;// is player try to reload 
    bool reloading = false;// is player currently reloading
    bool jumpstarted = false;// is player just press jump button 
    bool dead = false;// is player dead 
    bool end = false;
    /* float endurance;
     float maxendurance = 100;*/
    int noise = 0; // 0 crouch, 5 walk, 15 run


    int playerlife = 100; // player life
    Camera camera; // camera of the player
    public List<weapon> PlayerWeapons = new List<weapon>();//all weapon currently possesed by the player
    private List<GameObject> objweapon = new List<GameObject>();// all 3D object to of each weapon possesed by the player
    public weapon crtweapon; // current weapon equipt
    public int crtIndexweapon=0; // index of the current weapon equipt
    //Rigidbody rb; // rigidbody of the player
    CharacterController cc; // character controller of the player
    Vector2 direction = new Vector2(); // input of movement 
    
    public movementspeeds speed = movementspeeds.nomove; // state of speed of the player
    int ammoplayer = 100; // current quantity of ammo possesed by the player
    int pointplayer; // current points possesed by the player
    float Chrono; //  timer of the level 
    int countEnnemy; // numb er of ennmys killed by the player
    int countDetected = 0;
    public  LayerMask ennemylayer; // specific layer dedicated to ennemys

    public List<int> keys = new List<int>();// all key(id) possesed by the player
    Vector2 mousepos;//mouse position
    //Manage roration and movement direction
    float horizontalspeed = 100f;
    float verticalspeed = 100f;
    float xRotation = 0f;
    float yRotation = 0f;


    public Transform groundCheck; // empty object to verify if player on ground
    public float groundDistance = 0.4f;//distance to cehck if on the ground
    public LayerMask groundMask;//layer of the ground
    float gravity = -10; // gravity value
   // float JumpForce = 10;//
    float JumpSpeed = 1;
    bool grounded = true; // is player on the grounded
    Vector3 velocity; // manage movement in 3D
    public GameObject weaponplace;//empty object to place the 3D model of the weapon /child of camera

    float BaseFov = 60; // base value of the FOV of the cam 
    float AimFov = 40;// value of the FOV cam when the player aim
    public gdsoundmanager soundcheck; //object that manage the detection of ennemys

    // all OnName function are here to receive input from input unity system and call function and/or modify variable 
    public void OnMove( InputAction.CallbackContext context )
    {
        Vector2 move = context.ReadValue<Vector2>();
        direction = move;
        
    }
    public void OnMousePosition(InputAction.CallbackContext context)
    {
        mousepos = context.ReadValue<Vector2>();
        //Debug.Log("mousepos : " + mousepos);
    }
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
           shoot = true;
           float Rangesound = crtweapon.shootWeapon(camera, this, ennemylayer);
           if(Rangesound > 0)
           {
               soundcheck.checkSound(Rangesound);
               
           }
        }
        else if(context.canceled){
            shoot = false;
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            aim = true;
            camera.fieldOfView = AimFov;
        }
        else if (context.canceled)
        {
            aim = false;
            camera.fieldOfView = BaseFov;
        }
        Debug.Log("aim : " + aim);
    }
    public void OnReload(InputAction.CallbackContext context)
    {
        // start reloading if can be
        if (context.started )
        {
            if (reloading != true &&  ammoplayer > 0)
            {
                bool canreload = crtweapon.tryreload();
                //Debug.Log("Reload" + canreload);
                if(canreload)
                {
                    StartCoroutine(waitforreload());
                }
            }         
        }
    }
    public void OnCrouch(InputAction.CallbackContext context)
    {
        
        if (context.started)
        {
            crouch = true;
        }
        else if (context.canceled)
        {
            crouch = false;
        }
        Debug.Log("crouch : " + crouch);
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            run = true;
        }
        else if (context.canceled)
        {
            run = false;
        }
        //Debug.Log("crouch : " + run);
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Interact();
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpstarted = true;
          // Debug.LogError("jump : " + jumpstarted);
           
           // jump();
        }
    }
    public void OnChangeweapon(InputAction.CallbackContext context)
    {
       // Debug.Log(" Molette values : " + context);//.ReadValue<int>());
        if (context.started)
        {
           
            changeweapon((int)Mathf.Sign(context.ReadValue<float>()));
        }

    }

    // -------------- end input function  ---------------------------  //
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        camera = this.gameObject.GetComponentInChildren<Camera>();
        //rb = this.gameObject.GetComponent<Rigidbody>();
        cc = this.gameObject.GetComponent<CharacterController>();
        Chrono = Time.time;
        countEnnemy = 0;
        Initweapons();

    }
    void Initweapons()// create weapon model according to the data contain in weapon => mask all the model => make visible the current weapon hold by the player
    {
        for (int i = 0; i < PlayerWeapons.Count;i++)
        {
           GameObject asset = PlayerWeapons[i].asset;
           GameObject instance = Instantiate(asset,weaponplace.transform);
           instance.transform.localPosition = Vector3.zero;
           objweapon.Add(instance);
           instance.SetActive(false);
        }

        crtweapon = PlayerWeapons[crtIndexweapon];
        weaponvisible(true, crtIndexweapon);
    }
    void weaponvisible(bool isvisible,int index)//make the model of the weapon visible or invisible
    {
        objweapon[index].SetActive(isvisible);
    }

    // Update is called once per frame
    void Update()
    {
        if(!end && !dead)
        {
            Cameramanager();
            Movement();
        }
        
        
    }
    private void Cameramanager()// make the camera move according to the mouse movement
    {
        float mouseX = mousepos.x * horizontalspeed * Time.deltaTime;
        float mouseY = mousepos.y * verticalspeed * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


        camera.transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f); //rotate cam up and down

        this.transform.Rotate(Vector3.up * mouseX);  //rotate player around itself right/left
    }
    private void Movement()//manage movemnt of the player
    {
        if (crouch)//change height of the player if he is crouching
        {
            cc.height = 1;
        }
        else
        {
            cc.height = 2;
        }

        if (direction == Vector2.zero)//modify the player speed according to his position
        {
            speed = movementspeeds.nomove;
        }
        else 
        {
            if(crouch)
            {
                speed = movementspeeds.crouchspeed;
                noise = 0;
            }
            else if(run)
            {
                speed = movementspeeds.runspeed;
                noise = 15;
            }
            else
            {
                speed = movementspeeds.walkspeed;
                noise = 5;

            }
            // test noise
            if(noise>0)
            {
                soundcheck.checkSound(noise);
            }
        }


        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);//check if the player is on the ground
        if (grounded && velocity.y < 0)//apply gravity all the time
        {
            velocity.y = -2f;
        }



        Vector3 forwardSpeed = direction.y * this.gameObject.transform.forward * (float)speed;
        Vector3 straffSpeed = direction.x * this.gameObject.transform.right * (float)speed;
        Vector3 JumpS = Vector3.zero;

        Vector3 finalSpeed = forwardSpeed + straffSpeed;// + JumpS;
        cc.Move(finalSpeed * Time.deltaTime);


        if (jumpstarted && grounded)//manage gravity when player jump
        {
            velocity.y = Mathf.Sqrt(JumpSpeed * -2f * gravity);
        }
        else
        {
            jumpstarted = false;
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime); // move the player 

    }
    private void Interact()// when the player use the interact input determine if any action is possible 
    {
        if(camera!= null)
        {
            Ray ray = new Ray();
            ray.direction = camera.transform.forward;
            RaycastHit hit;

            if(Physics.Raycast(camera.transform.position, camera.transform.forward ,out hit, 15)) //cast ray from the center of the camera
            {
               GameObject obj = hit.collider.gameObject;
                if (obj.tag=="door") // if the object is a door try to opne it with the player's keys
                {
                   bool dooropen = obj.GetComponent<door>().tryOpenDoor(keys);
                }            
            }
        }
    }
 
    public void GetKey(int keyID)// add key to the player
    {
        keys.Add(keyID);
    }
    public void GetAmmo(int nammo)// add ammo to the player
    {
        ammoplayer += nammo;
    }
    public void GetPoint(int points)// add points to the player
    {
        pointplayer += points;
    }
    public void addCountennemy() //increment every time an ennemy is killed by the player
    {
        countEnnemy++;
    }
    public void TakeDamage(int damage) //  inflict dmg to the player
    {
        Debug.LogError("dmg taken : " + damage);
        playerlife -= damage;
        Debug.LogError("Lifeplayer: " + playerlife);
        if (playerlife<=0 && !dead)
        {
            Debug.LogError("shoulddie: ");
            Death();
        }
    }
    public void Detected()
    {
        countDetected++;
    }
    void changeweapon(int lastnext ) //change weapon 
    {
       // Debug.LogError("CUrrentweapon index :" + crtIndexweapon);
        weaponvisible(false, crtIndexweapon); //make precedetn weapon invisible
        crtIndexweapon += lastnext;
        if(crtIndexweapon<0)
        {
            crtIndexweapon = PlayerWeapons.Count - 1;

        }
        else if(crtIndexweapon>= PlayerWeapons.Count)
        {
            crtIndexweapon = 0;
        }
        crtweapon = PlayerWeapons[crtIndexweapon];

        weaponvisible(true, crtIndexweapon);//make new weapon visible

        //Debug.LogError("CUrrentweapon index :" + crtIndexweapon);
    }
    public void Death()//explicit
    {
        dead = true;
    }
    public void AddLife(int life)//player gain life
    {
        playerlife += life;
    }
    public void EndRun()// player arrived at the end of the level calcul the points of the player
    {
        Chrono = Time.time - Chrono;
        Debug.LogError("Chrono : " + Chrono);
        int diviser = (int)Chrono / 30;
        if (diviser<1)
        {
            diviser = 1;
        }
        float endScore = (pointplayer/diviser) + ( keys.Count*10) - (countEnnemy*5)-(countDetected * 5);
        Debug.LogError("Fin du lvl : " + endScore);
        

        end = true;
    }
    IEnumerator waitforreload()//reload the weapon  after a certain amount of time
    {
        reloading = true;
        yield return new WaitForSeconds(crtweapon.ReloadTime);
       
        if(ammoplayer>= crtweapon.LoaderCapacity)
        {
            ammoplayer -= crtweapon.LoaderCapacity;
            crtweapon.Currentammo = crtweapon.LoaderCapacity;
        }
        else
        {
            crtweapon.Currentammo = ammoplayer;
            ammoplayer = 0;
        }
       // Debug.Log("ReloadDOne");
        reloading = false;
    }

   
}

[System.Serializable]
public class weapon 
{
    [System.Serializable]
    public enum WeaponType
    {
        manual, semiauto, auto
    }

    public string Name;//name of the weapon
    public int LoaderCapacity;//maximum munition of the loader
    public int Currentammo;// current ammo in the loader
    public float Frequency;//shoot frequency of the weapon
    public int Damage;
    public int Noise;//range of the sound emmited by the weapon
    public float ReloadTime;
    public WeaponType Type;
    public float Range;
    public GameObject asset;
    float lastshoot = 0;


    public weapon(string name, int loadercapacity, int currentammo, float frequency, int damage, int noise, float reloadtime, WeaponType type, float range)//constructpr
    {
        Name = name;
        LoaderCapacity = loadercapacity;
        Currentammo = currentammo;
        Frequency = frequency;
        Damage = damage;
        Noise = noise;
        ReloadTime = reloadtime;
        Type = type;
        Range = range;
    }


    public bool tryreload()//called when player try to reload, reload if condiiton are ok : enough ammo, loader missing some ammo
    {
        bool reloaddone = false;
        //Debug.Log("trying to reload");
        if (Currentammo < LoaderCapacity )
        {
            //Debug.Log("can reload");
            reloaddone = true;
        }
        return reloaddone;
    }

    public float shootWeapon(Camera cam,Player player,LayerMask layertohit)
    {
        bool shootdone = false;
        float RangeTotransmit = 0;
        if (Currentammo > 0 && lastshoot + (1 / Frequency) < Time.time)// shoot done if enough ammo, and cooldown is ok
        {
            Currentammo--;
            shootdone = true;
            lastshoot = Time.time;
            if (cam != null)
            {
                Ray ray = new Ray();
                ray.direction = cam.transform.forward;
                RaycastHit hit;
                
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Range, layertohit))
                {
                    GameObject obj = hit.collider.gameObject;
                    bool isennemy = obj.CompareTag("ennemy");
                    if (isennemy)
                    {
                        bool ennemydead = obj.GetComponent<ennemy>().TakeDamage(Damage);//return if ennemy died from the shoot
                        if(ennemydead)
                        {
                            player.addCountennemy();
                        }
                    }
                }
            }
        }
        if(shootdone)
        {
            RangeTotransmit = Noise;
        }

        return RangeTotransmit; // return the range of the sound emmited by the weapon

    }

}