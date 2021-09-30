using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : SingletonMonoBehaviour<InfoUI>
{
    [SerializeField]
    private Text stateText, turnText, winText;

    public void ShowState(GameController.GameState state)
    {
        switch (state)
        {
            case GameController.GameState.Over:

                stateText.text = "游戏结束";

                break;
            case GameController.GameState.Playing:

                stateText.text = "游戏中";

                break;
            case GameController.GameState.Wait:
                stateText.text = "等待中";
                break;
        }
    }

    public void ShowTurn(int turn)
    {
        turnText.text = "玩家" + (turn + 1) + "的回合";
    }



    public void ShowWinner(bool b)
    {
        winText.gameObject.SetActive(b);
        if (b)
        {
            winText.text = "玩家" + (TurnController.Instance.CurrentPlayer()+1) + "获胜！";
        }
    }
}
