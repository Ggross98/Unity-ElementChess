using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessData
{
    public enum Type { EMPTY, FIRE, WATER, GROUND, WIND, CHARACTER };
    public Type type;

    public Sprite sprite;

    public ChessData(Type tp, Sprite img)
    {
        this.type = tp;
        this.sprite = img;
    }

    public bool IsCharacter()
    {
        return type == Type.CHARACTER;
    }


    public static ChessData Chess_Fire = new ChessData(Type.FIRE, Resources.Load<Sprite>("Images/Elements/fire"));

    public static ChessData Chess_Water = new ChessData(Type.WATER, Resources.Load<Sprite>("Images/Elements/water"));

    public static ChessData Chess_Ground = new ChessData(Type.GROUND, Resources.Load<Sprite>("Images/Elements/ground"));

    public static ChessData Chess_Wind = new ChessData(Type.WIND, Resources.Load<Sprite>("Images/Elements/wind"));

    public static ChessData Character_Reimu = new ChessData(Type.CHARACTER, Resources.LoadAll<Sprite>("Images/Characters/reimu")[9]);

    public static ChessData Character_Marisa = new ChessData(Type.CHARACTER, Resources.LoadAll<Sprite>("Images/Characters/marisa")[9]);

    public static Dictionary<Type, ChessData> ChessDataDict = new Dictionary<Type, ChessData>() {

        { Type.FIRE, Chess_Fire },
        { Type.WATER, Chess_Water },
        { Type.GROUND, Chess_Ground },
        { Type.WIND, Chess_Wind },

    };

    public static Dictionary<string, ChessData> CharacterChessDataDict = new Dictionary<string, ChessData>(){

        { "Reimu", Character_Reimu },
        { "Marisa", Character_Marisa },

    };
    
}
