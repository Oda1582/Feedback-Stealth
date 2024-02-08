using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIScript : MonoBehaviour
{
    public Player refplayer;
    public GameObject HealthAnchor, AmmoAnchor, PointAnchor; 
    public TMP_Text _UIHealthAnimText, _UIAmmoAnimText, _UIPointAnimText;
    public GameObject _UIHealthAnim, _UIAmmoAnim, _UIPointAnim; 
    public Image ImageArme, ImagePlayerState, ImageKey;
    public Sprite ArmeSilencieux, ArmeFusil, WalkingSprite, RunningSprite, CrouchingSprite, JumpingSprite, Key13, Key14;
    public AudioManager AudioManager;
    public Coroutine coroutine;
    public bool IsStartedWalking = false;
    public bool IsStartedRunning = false;
    public bool IsStartedCrouching = false;
    public TMP_Text AmmoText, AmmoPlayerText;
    public Animator OutOfAmmoAnim;
    public GameObject HiddenEye, DetectedEye;
    public GameObject ImageKeyGO;
    public GameObject ReloadUI, CrouchUI;

    // Start is called before the first frame update
    public void Start()
    {
   
    }

    // Update is called once per frame
    public void Update()
    {
        if (refplayer.run == true)
        {
            ImagePlayerState.sprite = RunningSprite;

            if (refplayer.finalSpeed.magnitude > 0)
            {
            AudioManager.DisableSound("Walking");
            AudioManager.EnableSound("Running");
            AudioManager.DisableSound("Crouching");
            }

            if (IsStartedRunning == false && refplayer.finalSpeed.magnitude > 0)
            {
                IsStartedWalking = false;
                IsStartedCrouching = false;
                StopCoroutine(CrouchingSound());
                StopCoroutine(WalkingSound());
                coroutine = StartCoroutine(RunningSound());
            }

        } 
        else if (refplayer.crouch == true)
        {
            ImagePlayerState.sprite = CrouchingSprite;

            if (refplayer.finalSpeed.magnitude > 0)
            {
            AudioManager.DisableSound("Walking");
            AudioManager.DisableSound("Running");
            AudioManager.EnableSound("Crouching");

            if (IsStartedCrouching == false && refplayer.finalSpeed.magnitude > 0)
            {
                IsStartedWalking = false;
                IsStartedRunning = false;
                StopCoroutine(RunningSound());
                StopCoroutine(WalkingSound());
                coroutine = StartCoroutine(CrouchingSound());
            }
            }      
        } 
        else if (refplayer.jumpstarted == false && refplayer.grounded == false)
        {
            ImagePlayerState.sprite = JumpingSprite;
            AudioManager.DisableSound("Walking");
            AudioManager.DisableSound("Running");
            AudioManager.DisableSound("Crouching");

        } else 
        {

            ImagePlayerState.sprite = WalkingSprite;
            
            if (refplayer.finalSpeed.magnitude > 0)
            {
                AudioManager.EnableSound("Walking");
                AudioManager.DisableSound("Running");
                AudioManager.DisableSound("Crouching");

                if (IsStartedWalking == false && refplayer.finalSpeed.magnitude > 0)
                {
                    StopCoroutine(RunningSound());
                    StopCoroutine(CrouchingSound());
                    IsStartedRunning = false;
                    IsStartedCrouching = false;
                    coroutine = StartCoroutine(WalkingSound());
                }
            }
            else
            {
                AudioManager.DisableSound("Walking");
            }
        }
    }

    public void ActivateKey13()
    {
        ImageKeyGO.SetActive(true);
        ImageKey.sprite = Key13;
    }

    public void ActivateKey14()
    {
        ImageKeyGO.SetActive(true);
        ImageKey.sprite = Key14;
    }

    public void DeactivateKey()
    {
        ImageKeyGO.SetActive(false);
    }

    public void HealthAnimStart()
    {
        GameObject newHealthAnim = Instantiate(_UIHealthAnim);
        TMP_Text newHealthAnimText = newHealthAnim.GetComponent<TMPro.TextMeshProUGUI>();
        newHealthAnim.transform.SetParent(HealthAnchor.transform, false);
        newHealthAnim.transform.position = HealthAnchor.transform.position;
        newHealthAnim.SetActive(true);
    }

    public void AmmoAnimStart()
    {
        GameObject newAmmoAnim = Instantiate(_UIAmmoAnim);
        TMP_Text newAmmoAnimText = newAmmoAnim.GetComponent<TMPro.TextMeshProUGUI>();
        newAmmoAnim.transform.SetParent(AmmoAnchor.transform, false);
        newAmmoAnim.transform.position = AmmoAnchor.transform.position;
        newAmmoAnim.SetActive(true);
    }

    public void PointAnimStart()
    {
        GameObject newPointAnim = Instantiate(_UIPointAnim);
        TMP_Text newPointAnimText = newPointAnim.GetComponent<TMPro.TextMeshProUGUI>();
        newPointAnim.transform.SetParent(PointAnchor.transform, false);
        newPointAnim.transform.position = PointAnchor.transform.position;
        newPointAnim.SetActive(true);
    }

    public void SetWeapon(int index) {

        if(index == 0) {
            ImageArme.sprite = ArmeSilencieux;
            AudioManager.Play("Change Weapon Sound");
        } else {
            ImageArme.sprite = ArmeFusil;
            AudioManager.Play("Change Weapon Sound");
        }
    }

    public void SetAmmo()
    {
        AmmoText.text = refplayer.crtweapon.Currentammo + " / " + refplayer.crtweapon.LoaderCapacity;
        AmmoPlayerText.text = refplayer.ammoplayer.ToString();

    }

    public void OutOfAmmoAnimStart()
    {
        OutOfAmmoAnim.SetTrigger("Out of Ammo");
    }

    public void Detected()
    {
        HiddenEye.SetActive(false);
        DetectedEye.SetActive(true);
    }

    public void Hidden()
    {
        HiddenEye.SetActive(true);
        DetectedEye.SetActive(false);
    }

    IEnumerator WalkingSound()
    {
        IsStartedWalking = true;
        if (refplayer.finalSpeed.magnitude > 0 && refplayer.grounded == true)
        {
        AudioManager.PlayOneShot("Walking");
        }

        yield return new WaitForSeconds(0.4f);

        IsStartedWalking = false;
    }

    IEnumerator RunningSound()
    {
        IsStartedRunning = true;
        if (refplayer.finalSpeed.magnitude > 0 && refplayer.grounded == true)
        {
        AudioManager.PlayOneShot("Running");
        }

        yield return new WaitForSeconds(0.2f);

        IsStartedRunning = false;
    }

    IEnumerator CrouchingSound()
    {
        IsStartedCrouching = true;
        if (refplayer.finalSpeed.magnitude > 0 && refplayer.grounded == true)
        {
        AudioManager.PlayOneShot("Crouching");
        }

        yield return new WaitForSeconds(0.6f);

        IsStartedCrouching = false;
    }

    public void RetryButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ActivateReloadUI()
    {
        ReloadUI.SetActive(true);
    }

    public void DeactivateReloadUI()
    {
        ReloadUI.SetActive(false);
    }

    public void ActivateCrouchUI()
    {
        CrouchUI.SetActive(true);
    }

    public void DeactivateCrouchUI()
    {
        CrouchUI.SetActive(false);
    }
}
