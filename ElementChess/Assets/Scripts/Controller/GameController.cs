using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : SingletonMonoBehaviour<GameController>
{
    public enum GameState { Wait, Playing, Over };

    private GameState state = GameState.Wait;

    //public Map map;

    private void Start()
    {
        StartGame();
    }


    public bool IsPlaying()
    {
        return state == GameState.Playing;
    }

    public void StartGame()
    {

        TileController.Instance.GenerateMap(Config.ROW, Config.COLUMN, Config.TILE_SIZE);

        TileController.Instance.GeneratePlayer();

        MovementController.Instance.Generate();

        TurnController.Instance.StartTurn();

        state = GameState.Playing;

        InfoUI.Instance.ShowState(state);
        InfoUI.Instance.ShowWinner(false);

    }

    public void EndGame()
    {
        state = GameState.Over;

        InfoUI.Instance.ShowState(state);
        InfoUI.Instance.ShowWinner(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }


}
