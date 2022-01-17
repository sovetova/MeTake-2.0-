using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public GameObject ResultObject;
    public PlayerController PC;
    public RoadGenerator RG;

    public Text PointsTxt,
                CoinsTxt;
    float Points;

    public bool CanPlay = false;

    public float MoveSpeed;

    public int Coins = 0;


    public void StartGame ()
    {
        ResultObject.SetActive(false);
        RG.StartGame();
        CanPlay = true;
        PC.ac.SetTrigger("Respawn");

        Points = 0;
    }

    private void Update()
    {
        if(CanPlay)
            Points += Time.deltaTime * 3;

        PointsTxt.text = ((int)Points).ToString();
    }
    public void ShowResult()
    {
        ResultObject.SetActive(true);
    }

    public void AddCoins(int number)
    {
        Coins += number;
        CoinsTxt.text = Coins.ToString();
    }
}
