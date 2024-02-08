using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Ctrl : MonoBehaviour {

    public TMP_Text _vie,_munitions,_clés,_score,_points,_EnnTué,_chrono;
    public TMP_Text _ammoS, _ammoF;
    public GameObject menuFin, menuOver;
    public Image loading;
    
    public void SetLife (int _life) {
        _vie.text = _life.ToString();

    }

    public void SetMunition (int _muni) {
        _munitions.text = _muni.ToString();

    }

    public void SetAmmoS (int _ammo) {
        _ammoS.text = _ammo.ToString();

    }

    public void SetAmmoF(int _ammo) {
        _ammoF.text = _ammo.ToString();

    }

    public void SetCle (int _cle) {
        _clés.text = _cle.ToString();

    }

    public void SetScore (float _point) {
        _score.text = _point.ToString();

    }

    public void SetPoint (int _point) {
        _points.text = _point.ToString();

    }

    public void SetEnnTue (int _Enn) {
        _EnnTué.text = _Enn.ToString();

    }

    public void SetChrono (float _temps) {
        _chrono.text = _temps.ToString();

    }

    public void Recharge(float duration) {

       StartCoroutine(LerpFunction(loading,0,1,duration));
    }

    // Activation des menu hud //

    public void MenuFin () {
        menuFin.SetActive(true);

    }

    public void MenuOver () {
        menuOver.SetActive(true);

    }

    private void Start() {
        loading.fillAmount=0;
        
    }

    // toModify : fillAmont de l'image circulaire à modifier, startValue : valeur de départ, endValue : valeur de fin, duration : temps en seconde
    IEnumerator LerpFunction(Image toModify, float startValue,float endValue, float duration) {

        toModify.gameObject.SetActive(true);  //Active l'image
        float time = 0;
        toModify.fillAmount = startValue;
        while (time < duration)
        {
            toModify.fillAmount = Mathf.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;  //Pause d'une frame 
        }
        toModify.fillAmount = endValue;  //Snap à la valeur de fin
        yield return null;
        toModify.fillAmount = 0; // Reset
        toModify.gameObject.SetActive(false);
        Debug.Log("Reload UI End");
    }

}
