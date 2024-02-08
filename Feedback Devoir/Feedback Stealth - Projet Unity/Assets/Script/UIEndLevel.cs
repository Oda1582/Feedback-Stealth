using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIEndLevel : MonoBehaviour
{
    public TMP_Text EndScoreText, PointText, KeysCollectedText, EnemyKilledText, DetectedText, UIState;
    public TextMeshProUGUI StateTextColor;
    public Player refplayer;
    public GameObject UIEndLevelGO;
    [SerializeField] public TMP_Text _UIChrono;
    [SerializeField] float minutes;
    private float _chrono;

    // Start is called before the first frame update
    void Start()
    {
        UIEndLevelGO.SetActive(false);

        // Initialisation du chronometre
        _UIChrono.text = "00:00";

        UIState.text = "Level Completed !";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEndScore()
    {
        EndScoreText.text = refplayer.endScore.ToString();
    }

    public void SetPoint()
    {
        PointText.text = refplayer.pointplayer + " / " + refplayer.diviser;
    }

    public void SetCollectedKeys()
    {
        KeysCollectedText.text = refplayer.keys.Count + " X 10";
    }

    public void SetEnemyKilledText()
    {
        EnemyKilledText.text = refplayer.countEnnemy + " X 5";
    }

    public void SetDetected()
    {
        DetectedText.text = refplayer.countDetected + " X 5";
    }

    public void ActivateUI()
    {
        UIEndLevelGO.SetActive(true);
    }

    public void SetState(string State)
    {
        UIState.text = State;
    }


    void FixedUpdate() 
    {
        // Attributs 
        float secondes; 
        float centiemes;

        // Incrémentation du chronomètre
        _chrono += Time.fixedDeltaTime;

        // Arrondissement a 2 chiffres apres la virgule
        _chrono = (Mathf.Floor(_chrono * 100f))/100;

        // Formatage 
        secondes = Mathf.Floor(_chrono);
        centiemes = Mathf.Floor((_chrono - secondes) * 100f);
        secondes.ToString();
        centiemes.ToString();
        string niceTime = string.Format("{0:00}:{1:00},{2:00}", minutes, secondes, centiemes);
      
        // Affichage dans l'UI (et gestion du "0")
        _UIChrono.text = niceTime;

        // Renitialisation du chronometre
        if (_chrono > 60f)
        {
            _chrono = 0;
            minutes ++;
        }
    }
}
