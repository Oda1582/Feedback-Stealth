using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider sliderHealth;
    public Slider sliderNoise;
    public Gradient HealthGradient;
    public Gradient NoiseGradient;
    public Image HealthFill;
    public Image NoiseFill;
    public Image ReloadFill;
    public Image[] NoisePoints;
    public Player refplayer;


    public void Update()
    {
        // if (refplayer.run == true)
        // {
        //     if (refplayer.finalSpeed.magnitude > 0)
        //     {
        //         noise = 15;
        //         healthBar.NoiseBarFiller(noise);
        //     }

        // } 
        // else if (refplayer.crouch == true)
        // {
        //     if (refplayer.finalSpeed.magnitude > 0)
        //     {
        //         noise = 0;
        //         healthBar.NoiseBarFiller(noise);
        //     }
        // } 
        // else 
        // {
        //     if (refplayer.finalSpeed.magnitude > 0)
        //     {

        //     }
        // }

    }

    public void SetMaxHealth(int health)
    {
        sliderHealth.maxValue = health;
        sliderHealth.value = health;

        HealthFill.color = HealthGradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        sliderHealth.value = health;

        HealthFill.color = HealthGradient.Evaluate(sliderHealth.normalizedValue);
    }

    public void SetMaxNoise(int Noise)
    {
        sliderNoise.maxValue = Noise;
        sliderNoise.value = Noise;

        NoiseFill.color = NoiseGradient.Evaluate(1f);
    }

    public void SetNoise(int Noise)
    {
        sliderNoise.value = Noise;

        NoiseFill.color = NoiseGradient.Evaluate(sliderNoise.normalizedValue);
    }

    public IEnumerator Reloading(float End, float ReloadDuration)
    {
        float Time = 0;
        float Start = ReloadFill.fillAmount;

        while (Time < ReloadDuration)
        {
            ReloadFill.fillAmount = Mathf.Lerp(Start, End, Time/ReloadDuration);
            Time += UnityEngine.Time.deltaTime;
            yield return null;
        }
        ReloadFill.fillAmount = End;
        ReloadFill.enabled = false;
    }

    // public IEnumerator SoundGunUI()
    // {
    //     NoiseBarFiller(refplayer.crtweapon.Noise);
    //     yield return new WaitForSeconds(refplayer.crtweapon.lastshoot + (1 / refplayer.crtweapon.Frequency));
        
    // }

    bool DisplayHealthPoint(int _noise, int pointNumber)
    {
        return ((pointNumber * 1) >= _noise);
    }

    public void NoiseBarFiller(int Noise)
    {
        // NoiseBar.fillAmount = Mathf.Lerp(NoiseBar.fillAmount, (Noise / MaxNoise), lerpSpeed);
        // ringHealthBar.fillAmount = Mathf.Lerp(NoiseBar.fillAmount, (Noise / MaxNoise), lerpSpeed);

        for (int i = 0; i < NoisePoints.Length; i++)
        {
            NoisePoints[i].enabled = !DisplayHealthPoint(Noise, i);
            // Debug.Log(!DisplayHealthPoint(refplayer.noise, i));
            // Debug.Log(refplayer.noise);
        }
    }
}
