using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorUIScript : MonoBehaviour
{
    public GameObject DoorUI;
    public TMP_Text DoorTextUI;
    public string TexteUI;

    // Start is called before the first frame update
    void Start()
    {
        DoorUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateUI(string TexteUI)
    {
        DoorUI.SetActive(true);
        DoorTextUI.text = TexteUI;
    }

    public void DeactivateUI()
    {
        DoorUI.SetActive(false);
    }

    public void CantOpen(string TexteUI)
    {
        DoorTextUI.text = TexteUI;        
    }
}
