using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : SingletonMonoBehaviour<TurnController>
{

    private int maxPlayer = 2;

    private int currentPlayer = 0;

    

    public int CurrentPlayer()
    {
        return currentPlayer;
    }

    public void StartTurn()
    {
        InfoUI.Instance.ShowTurn(currentPlayer);

        MovementController.Instance.SetActing(false);
        DrawController.Instance.StartDraw();
        TileController.Instance.RenewTiles();
    }
    
    public void EndTurn()
    {
        /*
        //检测有没有把自己堵死
        if (TileController.Instance.CheckDeath())
        {
            GameController.Instance.EndGame();
        }
        */


        MovementController.Instance.SetActing(false);
        DrawController.Instance.EndDraw();

        currentPlayer++;
        if(currentPlayer >= maxPlayer)
        {
            currentPlayer = 0;
        }

        TileController.Instance.SelectPlayer(currentPlayer);


        //检测有没有把对手堵死
        if (!TileController.Instance.CheckDeath())
        {
            StartTurn();
        }
        else
        {
            currentPlayer++;
            if (currentPlayer >= maxPlayer)
            {
                currentPlayer = 0;
            }

            GameController.Instance.EndGame();
        }
    }


}
