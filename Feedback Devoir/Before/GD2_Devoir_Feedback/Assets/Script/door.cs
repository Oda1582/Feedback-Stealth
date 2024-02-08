using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour
{
    // Start is called before the first frame update
    public int keyID;
    public bool open = false;
    void Start()
    {       
    }
    void Update()
    {      
    }
    public bool tryOpenDoor(List<int> PlayersKey)//when player try to open the door :  verify if the player have the corresponding key to open the door
    {
        bool dooropen = false;
        if(open != true)
        {
            for (int i=0; i<PlayersKey.Count;i++)
            {
                if(PlayersKey[i]==keyID)
                {
                    Opendoor();
                    dooropen = true;
                    break;
                }
            }
        }

        return dooropen;
    }

    void Opendoor()// make the door inactive(invisilbe, no collision) when the door is open
    {
        this.gameObject.SetActive(false);
    }
}
