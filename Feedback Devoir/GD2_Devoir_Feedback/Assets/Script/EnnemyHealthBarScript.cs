using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnnemyHealthBarScript : MonoBehaviour
{
    public Slider sliderHealth;

    public void SetMaxHealth(int health)
    {
        sliderHealth.maxValue = health;
        sliderHealth.value = health;
    }

    public void SetHealth(int health)
    {
        sliderHealth.value = health;
    }
}
