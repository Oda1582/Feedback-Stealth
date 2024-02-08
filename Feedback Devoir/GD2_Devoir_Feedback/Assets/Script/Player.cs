using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    public bool aim = false;// is player  button to aim pressed  
    public bool crouch = false;// is player  button to crouch pressed 
    public bool run = false;// is player button to run pressed 
    //bool reload = false;// is player try to reload 
    public bool reloading = false;// is player currently reloading
    public bool jumpstarted = false;// is player just press jump button 
    bool dead = false;// is player dead 
    public bool end = false;
    /* float endurance;
     float maxendurance = 100;*/
    public int noise = 0; // 0 crouch, 5 walk, 15 run


    public int Maxplayerlife = 150; // player life
    public int playerlife; // player life
    Camera camera; // camera of the player
    public List<weapon> PlayerWeapons = new List<weapon>();//all weapon currently possessed by the player
    private List<GameObject> objweapon = new List<GameObject>();// all 3D object to of each weapon possessed by the player
    public weapon crtweapon; // current weapon equipt
    public int crtIndexweapon=0; // index of the current weapon equipt
    //Rigidbody rb; // rigidbody of the player
    CharacterController cc; // character controller of the player
    Vector2 direction = new Vector2(); // input of movement 
    
    public movementspeeds speed = movementspeeds.nomove; // state of speed of the player
    public int ammoplayer = 100; // current quantity of ammo possessed by the player
    public int pointplayer; // current points possessed by the player
    float Chrono; //  timer of the level 
    public int countEnnemy; // number of ennemys killed by the player
    public int countDetected = 0;
    public LayerMask ennemylayer; // specific layer dedicated to enemy
    public LayerMask lvllayer;

    public List<int> keys = new List<int>();// all key(id) possessed by the player
    Vector2 mousepos;//mouse position
    //Manage rotation and movement direction
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
    public bool grounded = true; // is player on the grounded
    Vector3 velocity; // manage movement in 3D
    public GameObject weaponplace;//empty object to place the 3D model of the weapon /child of camera

    float BaseFov = 60; // base value of the FOV of the cam 
    float AimFov = 40;// value of the FOV cam when the player aim
    public gdsoundmanager soundcheck; //object that manage the detection of ennemys

    public UIScript UIRef;
    public AudioManager AudioManager;
    public GameObject MuzzleEffect;
    GameObject InstantiatedSilencer, InstantiatedRifle;
    public HealthBarScript healthBar;
    public Vector3 finalSpeed;
    public bool IsShooting = false;
    bool Jumped = true;
    public DoorUIScript DoorUIScriptDoor13, DoorUIScriptDoor14;
    public float endScore;
    public int diviser;
    public UIEndLevel UIEndLevelref;
    public bool InputBlock = false;
    public CrouchTutorialScript CrouchScriptRef;

    // all OnName function are here to receive input from input unity system and call function and/or modify variable 
    public void OnMove( InputAction.CallbackContext context )
    {
        if (!InputBlock)
        {
        Vector2 move = context.ReadValue<Vector2>();
        direction = move;
        }
        
    }
    public void OnMousePosition(InputAction.CallbackContext context)
    {
        if (!InputBlock)
        {
        mousepos = context.ReadValue<Vector2>();
        //Debug.Log("mousepos : " + mousepos);
        }
    }
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (!InputBlock)
        {
        if (context.started)
        {
           shoot = true;
           float Rangesound = crtweapon.shootWeapon(camera, this, ennemylayer, lvllayer, MuzzleEffect, InstantiatedSilencer, InstantiatedRifle);
           if(Rangesound > 0)
           {
                if (crtweapon.Name == "silencieux")
                {
                    int newRange = Mathf.RoundToInt(Rangesound);
                    healthBar.NoiseBarFiller(newRange+1);
                    soundcheck.checkSound(Rangesound);
                } else if (crtweapon.Name == "fusil")
                {
                healthBar.NoiseBarFiller(Mathf.RoundToInt(Rangesound));
                soundcheck.checkSound(Rangesound);   
                }
           }
        }
        else if(context.canceled){
            shoot = false;
        }
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (!InputBlock)
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
        // Debug.Log("aim : " + aim);
        }
    }
    public void OnReload(InputAction.CallbackContext context)
    {
        if (!InputBlock)
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
                    healthBar.ReloadFill.enabled = true;
                    healthBar.ReloadFill.fillAmount = 0;
                    StartCoroutine(healthBar.Reloading(1, crtweapon.ReloadTime));
                    // UIRef.DeactivateReloadUI();
                }
            }         
        }
        }
    }
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (!InputBlock)
        {
        if (context.started)
        {
            crouch = true;
            UIRef.DeactivateCrouchUI();
            
            if (CrouchScriptRef != null)
            {
            CrouchScriptRef.CheckCrouch();
            }
        }
        else if (context.canceled)
        {
            crouch = false;
            UIRef.DeactivateCrouchUI();
            if (CrouchScriptRef != null)
            {
            CrouchScriptRef.CheckCrouch();
            }         
        }
        // Debug.Log("crouch : " + crouch);
        }
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (!InputBlock)
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
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!InputBlock)
        {
        if (context.started)
        {
            Interact();
        }
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!InputBlock)
        {
        if (context.started)
        {
            jumpstarted = true;
            Jumped = false;
          // Debug.LogError("jump : " + jumpstarted);
           
           // jump();
        }
        }
    }
    public void OnChangeweapon(InputAction.CallbackContext context)
    {
        if (!InputBlock)
        {
       // Debug.Log(" Molette values : " + context);//.ReadValue<int>());
        if (context.started)
        {
           
            changeweapon((int)Mathf.Sign(context.ReadValue<float>()));
            UIRef.SetAmmo();
        }
        }

    }
    public void OnChangeMap()
    {
        SceneManager.LoadScene("OldScene");
    }

    // -------------- end input function  ---------------------------  //
    void Start()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        camera = this.gameObject.GetComponentInChildren<Camera>();
        //rb = this.gameObject.GetComponent<Rigidbody>();
        cc = this.gameObject.GetComponent<CharacterController>();
        Chrono = Time.time;
        countEnnemy = 0;
        Initweapons();
        playerlife = 100;
        healthBar.SetMaxHealth(Maxplayerlife);
        healthBar.SetHealth(playerlife);
        
    }
    void Initweapons()// create weapon model according to the data contain in weapon => mask all the model => make visible the current weapon hold by the player
    {
        Vector3 ScaleSilencer = new Vector3(2f, 2f, 2f);
        Vector3 ScaleRifle = new Vector3(0.5f, 0.5f, 0.5f);

        for (int i = 0; i < PlayerWeapons.Count;i++)
        {
           GameObject asset = PlayerWeapons[i].asset;
           GameObject instance = Instantiate(asset,weaponplace.transform);
           instance.transform.localPosition = Vector3.zero;

            if (PlayerWeapons[i].Name == "silencieux")
            {
                // Set the local scale of the instantiated object
                instance.transform.localScale = ScaleSilencer;

                // Store the reference to the instantiated weapon object
                InstantiatedSilencer = instance;
            }
            if (PlayerWeapons[i].Name == "fusil")
            {
                // Set the local scale of the instantiated object
                instance.transform.localScale = ScaleRifle;

                // Store the reference to the instantiated weapon object
                InstantiatedRifle = instance;
            } 

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
            // Debug.Log(noise);
            // healthBar.NoiseBarFiller(200);
        }
        if(crouch && finalSpeed.magnitude > 0 && IsShooting == false && Jumped == true)
            {
                noise = 1;
                healthBar.NoiseBarFiller(noise);
            }
            else if(run && finalSpeed.magnitude > 0 && IsShooting == false && Jumped == true)
            {
                noise = 14;
                healthBar.NoiseBarFiller(noise);
            }
            else if (finalSpeed.magnitude > 0 && IsShooting == false && Jumped == true)
            {
                noise = 5;
                healthBar.NoiseBarFiller(noise);

            } else if(finalSpeed.magnitude == 0 && IsShooting == false && Jumped == true)
            {
                healthBar.NoiseBarFiller(1);
            } 
            // else if (jumpstarted == false && refplayer.grounded == false)
            // {

            // }
        
        if (crtweapon.Currentammo <= 0)
        {
            UIRef.ActivateReloadUI();
        } else if (crtweapon.Currentammo > 0)
        {
            UIRef.DeactivateReloadUI();
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
    private void Movement()//manage movement of the player
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
                // healthBar.SetNoise(0);
                // healthBar.NoiseBarFiller(noise+1);
            }
            else if(run)
            {
                speed = movementspeeds.runspeed;
                noise = 15;
                // healthBar.SetNoise(15);
                // healthBar.NoiseBarFiller(noise);
            }
            else
            {
                speed = movementspeeds.walkspeed;
                noise = 5;
                // healthBar.SetNoise(5);
                // healthBar.NoiseBarFiller(noise+1);
            }

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

        finalSpeed = forwardSpeed + straffSpeed;// + JumpS;
        cc.Move(finalSpeed * Time.deltaTime);
        //check velocity for walking sound

        if (jumpstarted && grounded)//manage gravity when player jump
        {
            velocity.y = Mathf.Sqrt(JumpSpeed * -2f * gravity);
            if (!AudioManager.IsSoundPlaying("Jumping"))
            {
            AudioManager.Play("Jumping");
            }
        }
        else
        {
            jumpstarted = false;
            if (Jumped == false && grounded)
            {
                AudioManager.PlayOneShot("Landing Sound");                
                Jumped = true;
            }
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime); // move the player 

        // Debug.Log(finalSpeed);

    }
    private void Interact()// when the player use the interact input determine if any action is possible 
    {
        if(camera!= null)
        {
            // basé sur ça faire un box collider pour la porte pour faire un raycast qui en fonction de si je regarde la porte, active un HUD press E
            Ray ray = new Ray();
            ray.direction = camera.transform.forward;
            RaycastHit hit;

            if(Physics.Raycast(camera.transform.position, camera.transform.forward ,out hit, 15)) //cast ray from the center of the camera
            {
               GameObject obj = hit.collider.gameObject;
                if (obj.tag=="door") // if the object is a door try to open it with the player's keys
                {
                    //faire apparaitre hud press e 

                   bool dooropen = obj.GetComponent<door>().tryOpenDoor(keys);

                //    Debug.Log(dooropen);

                   if (dooropen == false)
                   {
                        DoorUIScriptDoor13.CantOpen("It's closed, find the key.");
                        DoorUIScriptDoor14.CantOpen("It's closed, find the key.");
                   }
                }            
            }
        }
    }
 
    public void GetKey(int keyID)// add key to the player
    {
        keys.Add(keyID);
        if(keyID == 13)
        {
            UIRef.ActivateKey13();
        } else if (keyID == 14)
        {
            UIRef.ActivateKey14();
        }
    }
    public void GetAmmo(int nammo)// add ammo to the player
    {
        ammoplayer += nammo;
        UIRef.SetAmmo();
        UIRef.AmmoAnimStart();

    }
    public void GetPoint(int points)// add points to the player
    {
        pointplayer += points;
        UIRef.PointAnimStart();
    }
    public void addCountennemy() //increment every time an ennemy is killed by the player
    {
        countEnnemy++;
    }
    public void TakeDamage(int damage) //  inflict dmg to the player
    {
        Debug.Log("dmg taken : " + damage);
        playerlife -= damage;
        Debug.Log("Lifeplayer: " + playerlife);
        healthBar.SetHealth(playerlife);
        AudioManager.PlayOneShot("Player Hurt");

        if (playerlife<=0 && !dead)
        {
            Debug.Log("shoulddie: ");
            healthBar.SetHealth(playerlife);
            AudioManager.PlayOneShot("Player Death");
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
        weaponvisible(false, crtIndexweapon); //make precedent weapon invisible
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
        UIRef.SetWeapon(crtIndexweapon);
    }
    public void Death()//explicit
    {
        dead = true;
        InputBlock = true;
        Chrono = Time.time - Chrono;
        // Debug.LogError("Chrono : " + Chrono);
        diviser = (int)Chrono / 30;
        if (diviser<1)
        {
            diviser = 1;
        }
        endScore = (pointplayer/diviser) + ( keys.Count*10) - (countEnnemy*5)-(countDetected * 5);
        Debug.LogError("Mort : " + endScore);

        UIEndLevelref.ActivateUI();
        UIEndLevelref.SetEndScore();
        UIEndLevelref.SetPoint();
        UIEndLevelref.SetCollectedKeys();
        UIEndLevelref.SetEnemyKilledText();
        UIEndLevelref.SetDetected();
        UIEndLevelref.UIState.text = "You're Dead";
        UIEndLevelref.StateTextColor.color = new Color(1, 0, 0, 1);
        Time.timeScale = 0f;
    }
    public void AddLife(int life)//player gain life
    {
        playerlife += life;
        healthBar.SetHealth(playerlife);
        UIRef.HealthAnimStart();
    }
    public void EndRun()// player arrived at the end of the level calcul the points of the player
    {
        InputBlock = true;
        Chrono = Time.time - Chrono;
        // Debug.LogError("Chrono : " + Chrono);
        diviser = (int)Chrono / 30;
        if (diviser<1)
        {
            diviser = 1;
        }
        endScore = (pointplayer/diviser) + ( keys.Count*10) - (countEnnemy*5)-(countDetected * 5);
        Debug.LogError("Fin du lvl : " + endScore);

        UIEndLevelref.SetEndScore();
        UIEndLevelref.SetPoint();
        UIEndLevelref.SetCollectedKeys();
        UIEndLevelref.SetEnemyKilledText();
        UIEndLevelref.SetDetected();
        // Debug.Log(Chrono);

        end = true;
    }
    IEnumerator waitforreload()//reload the weapon  after a certain amount of time
    {
        if (crtweapon.Name == "silencieux")
        {
            crtweapon.AudioManager.PlayOneShot("Silencer Reload");
        } 
        else if (crtweapon.Name == "fusil")
        {
            crtweapon.AudioManager.PlayOneShot("Rifle Reload");
        }
        reloading = true;

        yield return new WaitForSeconds(crtweapon.ReloadTime);
       
        if(ammoplayer>= crtweapon.LoaderCapacity)
        {
            ammoplayer -= crtweapon.LoaderCapacity;
            crtweapon.Currentammo = crtweapon.LoaderCapacity;
            UIRef.SetAmmo();
        }
        else
        {
            crtweapon.Currentammo = ammoplayer;
            ammoplayer = 0;
            UIRef.SetAmmo();
        }
       // Debug.Log("ReloadDOne");
        reloading = false;
    }

    public IEnumerator SoundGunUI()
    {
        IsShooting = true;
        healthBar.NoiseBarFiller(crtweapon.Noise);
        Debug.Log(crtweapon.Noise);
        float waitTime = Time.time - crtweapon.lastshoot;
        waitTime = Mathf.Max(0, (1 / crtweapon.Frequency) - waitTime);
        yield return new WaitForSeconds(waitTime/2);
        Debug.Log("coroutine done");
        IsShooting = false;
        
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
    public float lastshoot = 0;
    public AudioManager AudioManager;
    public Hitmarker refHitmarker;
    public GameObject HitPrefab, BloodPrefab;
    UnityEngine.RaycastHit collisionbehaviour;
    public UIScript UIRef;
    public HealthBarScript healthBar;
    public Player refplayer;


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


    public bool tryreload()//called when player try to reload, reload if condition are ok : enough ammo, loader missing some ammo
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

        private void BulletBehaviour()
        {
            refHitmarker.Start();
            refHitmarker.LaunchHitmarker();
            AudioManager.PlayOneShot("Hitmarker Sound"); 
        }

    public void BulletHitBehaviour()
    {
        Vector3 pos = collisionbehaviour.point;
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, collisionbehaviour.normal);
        
        if (HitPrefab != null)
        {
        var hitVFX = UnityEngine.Object.Instantiate (HitPrefab, pos, rot);

        var psHit = hitVFX.GetComponent<ParticleSystem> ();
        if (psHit != null){
            UnityEngine.Object.Destroy (hitVFX, psHit.main.duration);
        }
        else 
        {
            var psChild = hitVFX.transform.GetChild (0).GetComponent<ParticleSystem> ();
            UnityEngine.Object.Destroy (hitVFX, psChild.main.duration);
        }
        }
    }

    public void BloodBehaviour()
    {
        Vector3 pos = collisionbehaviour.point;
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, collisionbehaviour.normal);
        
        if (BloodPrefab != null)
        {
        var hitBlood = UnityEngine.Object.Instantiate (BloodPrefab, pos, rot);

        UnityEngine.Object.Destroy(hitBlood, 1f);
        
        // else 
        // {
        //     var psChild = hitBlood.transform.GetChild (0).GetComponent<ParticleSystem> ();
        //     UnityEngine.Object.Destroy (hitBlood, psChild.main.duration);
        // }
        }
    }

    public float shootWeapon(Camera cam,Player player,LayerMask layertohit, LayerMask layerlvl, GameObject MuzzleEffect, GameObject InstantiatedSilencer, GameObject InstantiatedRifle)
    {
        bool shootdone = false;
        float RangeTotransmit = 0;
        if (Currentammo > 0 && lastshoot + (1 / Frequency) < Time.time)// shoot done if enough ammo, and cooldown is ok
        {
            Currentammo--;
            UIRef.SetAmmo();
            shootdone = true;
            lastshoot = Time.time;
            if (cam != null)
            {
                refplayer.StartCoroutine(refplayer.SoundGunUI());
                if (Name == "silencieux")
                {
                    AudioManager.PlayOneShot("Sons Arme Silencieux");
                } 
                else if (Name == "fusil")
                {
                    AudioManager.PlayOneShot("Sons Arme Fusil");
                }

                if (MuzzleEffect != null)
                {
                GameObject newMuzzleEffect = UnityEngine.Object.Instantiate(MuzzleEffect);

                if (InstantiatedSilencer != null && Name == "silencieux")
                {
                    Transform muzzleTransformSilencer = InstantiatedSilencer.transform.Find("Muzzle");
                    newMuzzleEffect.transform.SetParent(muzzleTransformSilencer.transform, false);
                    if (muzzleTransformSilencer != null)
                    {
                        newMuzzleEffect.transform.position = muzzleTransformSilencer.position;
                    }
                } 
                else if (InstantiatedRifle != null && Name == "fusil")
                {
                    Transform muzzleTransformRifle = InstantiatedRifle.transform.Find("Muzzle");
                    newMuzzleEffect.transform.SetParent(muzzleTransformRifle.transform, false);
                    if (muzzleTransformRifle != null)
                    {
                        newMuzzleEffect.transform.position = muzzleTransformRifle.position;
                    }
                    
                }
                UnityEngine.Object.Destroy(newMuzzleEffect.gameObject, 1);
                }

                Ray ray = new Ray();
                ray.origin = cam.transform.position;
                ray.direction = cam.transform.forward;
                RaycastHit hit;

                Debug.DrawRay(ray.origin, ray.direction * Range, Color.red, 3f);
                
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Range, layertohit))
                {
                    collisionbehaviour = hit;
                    GameObject obj = hit.collider.gameObject;
                    bool isennemy = obj.CompareTag("ennemy");
                    if (isennemy)
                    {
                        bool ennemydead = obj.GetComponent<ennemy>().TakeDamage(Damage);//return if ennemy died from the shoot
                        BulletBehaviour();
                        BulletHitBehaviour();
                        BloodBehaviour();
                        if(ennemydead)
                        {
                            player.addCountennemy();
                            
                        }
                    }
                } else if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Range, layerlvl))
                {
                    Debug.DrawRay(cam.transform.position, cam.transform.forward * Range, Color.red, 3f);
                    collisionbehaviour = hit;
                    BulletHitBehaviour();
                }
            }
        } else if (Currentammo <= 0 && lastshoot + (1 / Frequency) < Time.time)
        {
            shootdone = true;
            lastshoot = Time.time;
            UIRef.OutOfAmmoAnimStart();
            AudioManager.PlayOneShot("Out of Ammo");
            UIRef.ActivateReloadUI();
        }

        if(shootdone)
        {
            RangeTotransmit = Noise;
        }

        return RangeTotransmit; // return the range of the sound emmited by the weapon

    }

}