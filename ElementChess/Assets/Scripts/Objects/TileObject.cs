using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TileObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    private GameObject obj;

    private List<ChessObject> chessList;

    [SerializeField]
    private Transform chessParent;

    private Vector2Int pos;


    //游戏中属性
    private bool mountain = false, flied = false;

    public void SetMountain(bool _m)
    {
        mountain = _m;
    }

    public bool IsMountain()
    {
        return mountain;
    }

    public void SetFlied(bool _f)
    {
        flied = _f;
    }

    public bool IsFlied()
    {
        return flied;
    }


    private void Awake()
    {
        chessList = new List<ChessObject>();
    }

    public int GetChessCount()
    {
        return chessList.Count;
    }

    public ChessData.Type GetChessType()
    {
        if (chessList.Count > 0)
        {
            return chessList[chessList.Count - 1].data.type;
        }
        else
        {
            return ChessData.Type.EMPTY;
        }
    }
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        TileController.Instance.EnterTile(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TileController.Instance.ExitTile(this);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        TileController.Instance.ClickTile(this);
    }


    #region 棋子放置与移除
    /// <summary>
    /// 设置格子背景颜色
    /// </summary>
    /// <param name="c"></param>
    public void SetColor(Color c)
    {
        obj.GetComponent<Image>().color = c;
    }

    /// <summary>
    /// 放置一枚棋子到对应格子
    /// </summary>
    public void Place(ChessObject chess)
    {
        //Debug.Log(chess);

        chess.transform.SetParent(chessParent, false);
        chess.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 10*chessList.Count);
        chessList.Add(chess);
    }

    /// <summary>
    /// 移除格子最顶端的一枚棋子
    /// </summary>
    public ChessObject Remove()
    {
        ChessObject chess = chessList[chessList.Count - 1];

        chessList.Remove(chess);

        return chess;
        //Destroy(chess.gameObject);
    }

    /// <summary>
    /// 移除格子上所有棋子
    /// </summary>
    public List<ChessObject> RemoveAll()
    {
        if (chessList == null || chessList.Count == 0) return null;

        List<ChessObject> list = new List<ChessObject>();

        while (chessList.Count > 0)
        {
            list.Add(Remove());
        }

        return list;
    }
    #endregion

    #region 坐标操作
    public void SetPosition(Vector2Int p)
    {
        pos = p;
    }

    public void SetPosition(int x, int y)
    {
        SetPosition(new Vector2Int(x, y));
    }

    public void SetX(int x)
    {
        pos.x = x;
    }

    public void SetY(int y)
    {
        pos.y = y;
    }

    public int GetX()
    {
        return pos.x;
    }

    public int GetY()
    {
        return pos.y;
    }


    #endregion


}
