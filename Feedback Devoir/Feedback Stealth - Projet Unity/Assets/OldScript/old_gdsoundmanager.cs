using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class old_gdsoundmanager : MonoBehaviour
{

    public List<old_ennemy> ennemies = new List<old_ennemy>();
    
    public void checkSound( float range)// call when player emit a sound test all ennemys in range
    {
        for (int i=0;i<ennemies.Count;i++)
        {
            if(ennemies[i]!= null)
            {
                float distance = Vector3.Distance(ennemies[i].transform.position, this.gameObject.transform.position);
                distance = Mathf.Abs(distance);
                if (distance <= range)
                {
                    ennemies[i].TriggerhearPlayer(this.gameObject.GetComponentInParent<old_Player>());
                }
            }
           
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.tag =="ennemy")
        {
            ennemies.Add(other.gameObject.GetComponent<old_ennemy>());

        }
    }
    private void OnTriggerExit(Collider other)
    {
       
        if (other.gameObject.tag == "ennemy")
        {
            ennemies.Remove(other.gameObject.GetComponent<old_ennemy>());

        }
    }
}
