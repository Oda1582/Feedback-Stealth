using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ennemyvision : MonoBehaviour
{
    // detecte when the player is currently visiblez by the player with the use of a trigger box
    ennemy ennemy;
    void Start()
    {
        ennemy = this.GetComponentInParent<ennemy>();
        this.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)// player visible
    {
        if(other.gameObject.tag=="Player")
        {
            //Debug.LogError("Other : " + other.gameObject.name);
            ennemy.Triggerseeplayer(other.transform.gameObject.GetComponent<Player>(), true);
        }
    }
    private void OnTriggerExit(Collider other)// player out of vision
    {
        if (other.gameObject.tag == "Player")
        {
            ennemy.Triggerseeplayer(other.transform.gameObject.GetComponent<Player>(), false);
        }
    }
}
