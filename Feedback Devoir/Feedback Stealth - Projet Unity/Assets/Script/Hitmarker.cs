using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitmarker : MonoBehaviour
{
    public GameObject hitmarker;

    // Start is called before the first frame update
    public void Start()
    {
      // HitmarkerAudio = GetComponent<AudioSource>();
      hitmarker.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   // Lance le hitmarker
     public void LaunchHitmarker(){
                //Debug.Log("gogogaga");
                HitActive();
                Invoke("HitDisable", 0.1f);
            }

    // Activate Hitmarker
    public void HitActive(){
        hitmarker.SetActive(true);
        //Debug.Log("Activé");
    }

    // Disable Hitmarker
     public void HitDisable(){
        hitmarker.SetActive(false);
        //Debug.Log("Désactivé");
    }

}
