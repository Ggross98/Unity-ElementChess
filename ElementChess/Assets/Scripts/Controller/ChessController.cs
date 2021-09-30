using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessController : SingletonMonoBehaviour<ChessController>
{
    //[SerializeField]
    //private Transform chessParent;

    [SerializeField]
    private GameObject chessPrefab, characterChessPrefab;
    
    private ChessObject CreateChessObject(ChessData cd, Transform parent)
    {
        ChessObject chess = Instantiate(chessPrefab, parent).GetComponent<ChessObject>();

        if(chess!= null)
        {
            chess.SetData(cd);

            return chess;
        }
        else
        {
            return null;
        }

    }
    
    public ChessObject CreateChessObject(ChessData.Type ct, Transform parent)
    {
        if (ct == ChessData.Type.CHARACTER || ct == ChessData.Type.EMPTY) return null;

        else return CreateChessObject(ChessData.ChessDataDict[ct], parent);
    }

    public CharacterChessObject CreateCharacterChessObject(string name, Transform parent)
    {
        var character = Instantiate(characterChessPrefab, parent).GetComponent<CharacterChessObject>();


        if(character!= null)
        {
            character.SetData(ChessData.CharacterChessDataDict[name]);
            return character;
        }
        else
        {
            return null;
        }

    }

}
