using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum old_collectabletype // each type of collectable object 
{
    key, ammo, point, health
}
[System.Serializable]
public class old_CollectableObject : MonoBehaviour
{
   
    // Start is called before the first frame update
    bool dead =false;
    public old_collectabletype type;
    public int KeyID;
    public int ammoquantity;
    public int pointquantity;
    public int lifequantity;
   
    private void death()//inactive then call destroyer 
    {
        dead = true;
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")// if player walk on the object call the appropriate function according to the type
        {
            //Debug.LogError("ObjectCollideplayer : "+type);
            if (dead != true)
            {
                
                switch(type)
                {
                    case old_collectabletype.key:
                        other.gameObject.GetComponentInParent<old_Player>().GetKey(KeyID);// add key id to the player
                        break;
                    case old_collectabletype.ammo:
                        other.gameObject.GetComponentInParent<old_Player>().GetAmmo(ammoquantity);// add ammo to the player
                        break;
                    case old_collectabletype.point:
                        other.gameObject.GetComponentInParent<old_Player>().GetPoint(pointquantity);// add points to the player
                        break;
                    case old_collectabletype.health:
                        other.gameObject.GetComponentInParent<old_Player>().AddLife(lifequantity);// add life to the player
                        break;
                }

                death();
            }
            
        }
    }
}
