using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum collectabletype // each type of collectable object 
{
    key, ammo, point, health
}
[System.Serializable]
public class CollectableObject : MonoBehaviour
{
   
    // Start is called before the first frame update
    bool dead =false;
    public collectabletype type;
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
                    case collectabletype.key:
                        other.gameObject.GetComponentInParent<Player>().GetKey(KeyID);// add key id to the player
                        break;
                    case collectabletype.ammo:
                        other.gameObject.GetComponentInParent<Player>().GetAmmo(ammoquantity);// add ammo to the player
                        break;
                    case collectabletype.point:
                        other.gameObject.GetComponentInParent<Player>().GetPoint(pointquantity);// add points to the player
                        break;
                    case collectabletype.health:
                        other.gameObject.GetComponentInParent<Player>().AddLife(lifequantity);// add life to the player
                        break;
                }

                death();
            }
            
        }
    }
}
