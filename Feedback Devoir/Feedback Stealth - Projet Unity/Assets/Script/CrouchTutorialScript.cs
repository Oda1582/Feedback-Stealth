using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchTutorialScript : MonoBehaviour
{
    public UIScript UIRef;
    public Player refplayer;
    public bool PassedCrouchTutorial = false;

    public void CheckCrouch()
    {
        if (PassedCrouchTutorial == true && refplayer.crouch)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Call a method in the ParentScript when the trigger event occurs
        UIRef.ActivateCrouchUI();
        PassedCrouchTutorial = true;
        StartCoroutine("IfNotCrouchingCO");
    }


    public IEnumerator IfNotCrouchingCO()
    {
        yield return new WaitForSeconds(5f);
        UIRef.DeactivateCrouchUI();
        PassedCrouchTutorial = true;
        Destroy(this.gameObject);
    }
}
