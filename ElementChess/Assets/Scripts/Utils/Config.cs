using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Config
{
    public const int ROW = 11, COLUMN = 11;

    public const int MAX_DRAW = 4, MAX_STEP = 5;

    /// <summary>
    /// ui上一格的长宽
    /// </summary>
    public const int TILE_SIZE = 70;

    /// <summary>
    /// 格子的默认颜色
    /// </summary>
    public static Color TILE_COLOR_DEFAULT = new Color(205f / 255f, 197f / 255f, 191f / 255f);

    /// <summary>
    /// 格子的默认颜色
    /// </summary>
    public static Color TILE_COLOR_HIGHLIGHTED = new Color(255f / 255f, 239f / 255f, 219f / 255f);

    /// <summary>
    /// 火元素蔓延格子的提示颜色
    /// </summary>
    public static Color TILE_COLOR_FIRE = new Color(1,69f/255f, 0);

    /// <summary>
    /// 水元素流动格子的提示颜色
    /// </summary>
    public static Color TILE_COLOR_WATER = new Color(30f/255f,144f/255f,1);

    /// <summary>
    /// 选中水元素的颜色
    /// </summary>
    public static Color TILE_COLOR_STREAM = new Color(0,0,200f/255f);


    /// <summary>
    /// 可以飞行到的格子的提示颜色
    /// </summary>
    public static Color TILE_COLOR_FLY = new Color(1,1,20f/255f);

    /// <summary>
    /// 可以直接移动到的格子的提示颜色
    /// </summary>
    public static Color TILE_COLOR_MOVE = new Color(150f/255f,1,150f/255f);

}
