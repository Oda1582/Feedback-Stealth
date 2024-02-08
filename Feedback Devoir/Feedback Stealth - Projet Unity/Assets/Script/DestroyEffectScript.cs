using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffectScript : MonoBehaviour
{
    public ParticleSystem particule;

    // Start is called before the first frame update
    void Start()
    {
        particule = GetComponent<ParticleSystem>();
        PlayP();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayP()
    {
        particule.Play();
        //Debug.Log("Test");
    }
}
