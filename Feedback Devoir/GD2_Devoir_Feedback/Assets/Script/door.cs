using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour
{
    // Start is called before the first frame update
    public int keyID;
    public bool open = false;
    public AudioManager AudioManager;
    public bool SoundPlayed = false;
    public int CheckforSoundClosed; 
    public bool IsOpened = false;
    public UIScript UIRef;

    void Start()
    {       
    }
    void Update()
    {      
    }
    public bool tryOpenDoor(List<int> PlayersKey)//when player try to open the door : verify if the player have the corresponding key to open the door
    {
        bool dooropen = false;
        if(open != true)
        {
            for (int i=0; i<PlayersKey.Count;i++)
            {
                if(PlayersKey[i]==keyID)
                {
                    IsOpened = true;
                    Opendoor();
                    dooropen = true;
                    UIRef.DeactivateKey();
                    //play door open here
                    break;
                } else if (PlayersKey[i]!=keyID)
                {
                    if (SoundPlayed == false)
                    {
                    Debug.Log("Closed");
                    SoundPlayed = true;
                    }
                } 
            }
        }
        if (!AudioManager.IsSoundPlaying("Door Closed") && IsOpened == false)
        {        
        AudioManager.PlayOneShot("Door Closed");
        }

        return dooropen;
    }

    void Opendoor()// make the door inactive(invisilbe, no collision) when the door is open
    {
        this.gameObject.SetActive(false);
        AudioManager.PlayOneShot("Door Opening");
    }

}
