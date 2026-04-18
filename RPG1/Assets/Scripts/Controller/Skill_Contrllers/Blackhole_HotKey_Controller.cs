using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Blackhole_HotKey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;

    private Transform myEnemy;
    private Blackhole_Skill_Controller blackHole;

    private KeyCode myHotKey;
    private TextMeshProUGUI myText;

    public void SetupHotKey(KeyCode _myNewHotKey, Transform _myEnemy, Blackhole_Skill_Controller _blackHole)
    {
        sr = GetComponent<SpriteRenderer>();

        myHotKey = _myNewHotKey;
        myText = GetComponentInChildren<TextMeshProUGUI>();
        myText.text = _myNewHotKey.ToString();

        myEnemy = _myEnemy;
        blackHole = _blackHole;
    }

    private void Update()
    {
        if (Input.GetKeyDown(myHotKey))
        {
            blackHole.AddEnemyToList(myEnemy);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }
}
