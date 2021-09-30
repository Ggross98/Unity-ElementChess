using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawController : SingletonMonoBehaviour<DrawController>
{
    [SerializeField]
    private Button drawButton, endDrawButton;

    private int leftDraw;

    private Dictionary<ChessData.Type, int> elements = new Dictionary<ChessData.Type, int>() {

        { ChessData.Type.WATER, 0 },
        { ChessData.Type.FIRE, 0 },
        { ChessData.Type.WIND, 0 },
        { ChessData.Type.GROUND, 0 },

        { ChessData.Type.EMPTY, Config.MAX_STEP},

    };

    public void StartDraw()
    {
        leftDraw = Config.MAX_DRAW;

        elements[ChessData.Type.WATER] = 0;
        elements[ChessData.Type.FIRE] = 0;
        elements[ChessData.Type.WIND] = 0;
        elements[ChessData.Type.GROUND] = 0;
        elements[ChessData.Type.EMPTY] = Config.MAX_STEP;

        MovementController.Instance.RenewCount(elements);

        drawButton.interactable = true;
        endDrawButton.interactable = true;

    }

    public void DrawChess()
    {
        if (leftDraw <= 0) return;

        leftDraw--;

        int i = Random.Range(0, 4);
        ChessData.Type key = ChessData.Type.EMPTY;

        switch (i)
        {
            case 0:
                key = ChessData.Type.WATER;
                break;
            case 1:
                key = ChessData.Type.FIRE;
                break;
            case 2:
                key = ChessData.Type.WIND;
                break;
            default:
                key = ChessData.Type.GROUND;
                break;
        }

        elements[key]++;
        elements[ChessData.Type.EMPTY]--;

        MovementController.Instance.RenewCount(elements);

        if (leftDraw <= 0)
        {
            EndDraw();
            
        }
            


    }

    public void Use(ChessData.Type tp)
    {
        if (elements[tp] <= 0) return;

        elements[tp]--;

        MovementController.Instance.RenewCount(elements);
        MovementController.Instance.RenewDisableAndSelect();



    }

    public void EndDraw()
    {
        drawButton.interactable = false;
        endDrawButton.interactable = false;
        MovementController.Instance.SetActing(true);
        //MovementController.Instance.RenewDisable();
    }

    public void EndDrawAndStartMove()
    {
        EndDraw();
        MovementController.Instance.SetActing(true);
    }


}
