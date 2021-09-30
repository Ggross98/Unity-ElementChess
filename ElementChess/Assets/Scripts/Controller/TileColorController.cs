using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileColorController
{
    List<TileObject> coloredList;


    public TileColorController()
    {
        coloredList = new List<TileObject>();
    }

    public void ShowColor(TileObject tile, Color color)
    {
        tile.SetColor(color);
        coloredList.Add(tile);
    }

    public void ShowColor(List<TileObject> tiles, Color color)
    {
        foreach(var tile in tiles)
        {
            ShowColor(tile, color);
        }
    }

    public void ClearAllColor()
    {
        foreach(var tile in coloredList)
        {
            tile.SetColor(Config.TILE_COLOR_DEFAULT);
        }

        coloredList.Clear();
    }

}
