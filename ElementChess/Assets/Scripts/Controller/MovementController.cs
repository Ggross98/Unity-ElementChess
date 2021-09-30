using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementController : SingletonMonoBehaviour<MovementController>
{

    List<MovementOptionObject> optionList;

    [SerializeField]
    private GameObject optionPrefab;

    [SerializeField]
    private Transform optionParent;

    public ChessData.Type selectedType = ChessData.Type.EMPTY;
    private MovementOptionObject selectedObject = null;

    private bool acting = false;


    public void Generate()
    {
        if(optionList == null || optionList.Count == 0)
        {
            optionList = new List<MovementOptionObject>();

            CreateMovementOption(ChessData.Type.WATER);
            CreateMovementOption(ChessData.Type.FIRE);
            CreateMovementOption(ChessData.Type.WIND);
            CreateMovementOption(ChessData.Type.GROUND);

            CreateMovementOption(ChessData.Type.EMPTY);
        }

        SetSelectedOption(optionList[4]);

        foreach(var opt in optionList)
        {
            opt.Disable(true);
        }
    }



    public void SetSelectedOption(MovementOptionObject obj)
    {

        if(selectedObject != null) selectedObject.Select(false);

        obj.Select(true);

        selectedObject = obj;
        selectedType = obj.type;

        TileController.Instance.SwitchColor();
    }

    private MovementOptionObject CreateMovementOption(ChessData.Type type)
    {

        MovementOptionObject option = Instantiate(optionPrefab, optionParent).GetComponent<MovementOptionObject>();

        option.type = type;
        optionList.Add(option);

        switch (type)
        {
            case ChessData.Type.EMPTY:
                option.SetIcon(null);
                option.SetName("Step");
                option.SetColor(Color.white, Color.magenta, Color.magenta, Color.gray);
                break;
            case ChessData.Type.FIRE:
                option.SetIcon(Resources.Load<Sprite>("Images/Elements/fire"));
                option.SetName("Fire");

                option.SetColor(Color.white, Color.red, Color.red, Color.gray);
                break;
            case ChessData.Type.WATER:
                option.SetIcon(Resources.Load<Sprite>("Images/Elements/water"));
                option.SetName("Water");

                option.SetColor(Color.white, Color.blue, Color.blue, Color.gray);
                break;
            case ChessData.Type.GROUND:
                option.SetIcon(Resources.Load<Sprite>("Images/Elements/ground"));
                option.SetName("Ground");

                option.SetColor(Color.white, Color.green, Color.green, Color.gray);
                break;
            case ChessData.Type.WIND:
                option.SetIcon(Resources.Load<Sprite>("Images/Elements/wind"));
                option.SetName("Wind");

                option.SetColor(Color.white, Color.yellow, Color.yellow, Color.gray);
                break;
            /*
            case ChessData.Type.EMPTY:
                option.SetIcon(Resources.Load<Sprite>("Images/Elements/fire"));
                option.SetColor(Color.white, Color.red, Color.red, Color.gray);
                break;
            */

        }

        return null;
    }

    public void RenewCount(Dictionary<ChessData.Type, int> dict)
    {
        foreach(var opt in optionList)
        {
            opt.SetCount(dict[opt.type]);

            opt.CheckDisable();
        }
    }

    public void RenewDisableAndSelect()
    {
        foreach (var opt in optionList)
        {
            opt.CheckDisable();
        }

        if (selectedObject.disabled)
        {
            selectedObject.Disable(true);

            bool found = false;

            foreach(var opt in optionList)
            {
                if (!opt.disabled)
                {
                    SetSelectedOption(opt);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                TurnController.Instance.EndTurn();
            }
        }
        else
        {
            selectedObject.Select(true);
        }
    }


    public void SetActing(bool b)
    {
        acting = b;

        

        if (b)
        {
            
            foreach (var opt in optionList)
            {
                opt.CheckDisable();
            }

            foreach(var opt in optionList)
            {
                if (!opt.disabled)
                {
                    SetSelectedOption(opt);
                    break;
                }
            }
        }
        else
        {
            foreach (var opt in optionList)
            {
                opt.Disable(true);
            }
        }

    }

    public bool IsActing()
    {
        return acting;
    }



}
