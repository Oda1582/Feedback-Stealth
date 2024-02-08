using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endleveldetector : MonoBehaviour
{
    public AudioManager AudioManager;
    public UIEndLevel UIEndLevelref;
    public Player refplayer;
    
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)//call end of the level
    {
        //Debug.Log(""+other.gameObject.name);
        if (other.gameObject.name == "Player")
        {
           // Debug.Log("" + other.gameObject.GetComponent<Player>());
            other.gameObject.GetComponent<Player>().EndRun();
            AudioManager.PlayOneShot("End Run");
            UIEndLevelref.ActivateUI();
            UIEndLevelref.UIState.text = "Level Completed !";
            Time.timeScale = 0f;
           //fin du niveau
        }
    }
}
