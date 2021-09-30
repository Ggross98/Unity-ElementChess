using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessObject : MonoBehaviour
{
    public ChessData data;

    public GameObject obj;

    public TileObject tile;

    public void SetData(ChessData cd)
    {
        data = cd;

        obj.GetComponent<Image>().sprite = data.sprite;
    }

    public bool IsCharacter()
    {
        if (data == null) return false;
        else return data.IsCharacter();
    }
}
