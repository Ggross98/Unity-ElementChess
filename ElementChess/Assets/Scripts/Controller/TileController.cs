using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : SingletonMonoBehaviour<TileController>
{
    //鼠标经过的格子
    private TileObject selectedTile;

    //储存格子信息
    private TileObject[,] tileArray;
    private List<TileObject> tileList;

    //棋盘的行列数目
    [HideInInspector]
    public int row, column;
        
    //格子的尺寸    
    private int size;

    //格子预制体
    [SerializeField]
    private GameObject tilePrefab;

    //格子共同父对象
    [SerializeField]
    private Transform tileParent;

    private TileColorController colorController;

    //是否生成了所有格子
    public bool prepared = false;

    //玩家棋子所在格子
    private List<TileObject> playerTileList;
    //private TileObject playerTile;

    //水元素流动
    //是否已经安置一个水元素并开始流动
    private bool streaming = false;
    //是否已经选择一条水流
    private bool streamSelected = false;
    //可选择的所有水流
    private List<List<TileObject>> streamList;
    //已经选中的水流
    private int selectedStream;
    //要流动的格数
    private int flowCount, flowStep;


    /// <summary>
    /// 根据行列数生成格子。若已有格子，则会清除已有的所有棋子
    /// </summary>
    /// <param name="r"></param>
    /// <param name="c"></param>
    /// <param name="siz"></param>
    public void GenerateMap(int r, int c, int siz)
    {
        //清楚所有格子对象
        ClearAll();

        row = r;
        column = c;
        size = siz;

        tileArray = new TileObject[column, row];
        tileList = new List<TileObject>();


        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row; j++)
            {
                TileObject tile = Instantiate(tilePrefab, tileParent).GetComponent<TileObject>();
                tile.gameObject.name = "tile" + i + ", " + j;
                tile.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * size + size / 2, -(j * size + size / 2));

                tile.SetColor(Config.TILE_COLOR_DEFAULT);

                SetTile(i, j, tile);
            }
        }

        colorController = new TileColorController();

        

        
    }

    /// <summary>
    /// 回合开始时，所有格子均设置成未飞行过
    /// </summary>
    public void RenewTiles()
    {
        foreach(var tile in tileList)
        {
            tile.SetFlied(false);
        }
    }

    public void GeneratePlayer()
    {
        playerTileList = new List<TileObject>();

        CreatePlayerCharacter(5, 9, "Reimu");
        CreatePlayerCharacter(5, 1, "Marisa");

        SelectPlayer(0);

        prepared = true;
    }


    /// <summary>
    /// 临时：创建玩家棋子的方法
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    private void CreatePlayerCharacter(int _x, int _y, string name)
    {

        int x = _x, y = _y;

        if(x<0 || y<0 || x >= column || y >= row)
        {
            x = 1;
            y = 1;
        }

        playerTileList.Add(tileArray[x, y]);
        
        PlaceChess(tileArray[x, y], ChessController.Instance.CreateCharacterChessObject(name, tileArray[x, y].transform));
    }

    public void SelectPlayer(int index)
    {
        if (index < 0 || index >= playerTileList.Count) return;

        //playerTile = playerTileList[index];

        colorController.ClearAllColor();
    }

    /// <summary>
    /// 若无法飞行也无法移动至任意格子，则判定棋子死亡
    /// </summary>
    /// <returns></returns>
    public bool CheckDeath()
    {
        return FlyAccessibleTiles(playerTileList[TurnController.Instance.CurrentPlayer()]).Count == 0 && 
            MoveAccessibleTiles(playerTileList[TurnController.Instance.CurrentPlayer()]).Count == 0;
    }

    /// <summary>
    /// 将一个格子上的所有棋子移动到另一个格子上
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="dest"></param>
    public void MoveChess(TileObject tile, TileObject dest, bool _cover = true)
    {
        List<ChessObject> list = RemoveAllChess(tile, false);

        if (_cover) RemoveAllChess(dest);

        foreach(var chess in list)
        {
            PlaceChess(dest, chess);
        }
    }

    public void MovePlayer(TileObject dest)
    {
        MoveChess(playerTileList[TurnController.Instance.CurrentPlayer()], dest);

        playerTileList[TurnController.Instance.CurrentPlayer()] = dest;
    }

    public void FlyPlayer(TileObject dest)
    {
        var tile = playerTileList[TurnController.Instance.CurrentPlayer()];

        int dx = dest.GetX() - tile.GetX();
        if (dx > 0) dx = 1;
        if (dx < 0) dx = -1;

        int dy = dest.GetY() - tile.GetY();
        if (dy > 0) dy = 1;
        if (dy < 0) dy = -1;

        var next = NextTile(tile, dx, dy);
        while(next != dest)
        {
            if(next.GetChessCount()>0 && next.GetChessType() == ChessData.Type.WIND)
            {
                next.SetFlied(true);
            }

            next = NextTile(next, dx, dy);
        }

        MovePlayer(dest);
    }



    /// <summary>
    /// 角色棋子能否移动至某一点处
    /// </summary>
    /// <param name="start"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public bool CanMoveTo(TileObject start, TileObject dest)
    {


        return MoveAccessibleTiles(start).Contains(dest) || FlyAccessibleTiles(start).Contains(dest);
    }

    /// <summary>
    /// 可以移动一步走到的所有格子
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    private List<TileObject> MoveAccessibleTiles(TileObject start)
    {
        var list = new List<TileObject>();

        for(int dx = -1; dx <= 1; dx++)
        {
            for(int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                TileObject next = NextTile(start, dx, dy);

                if (next == null || next.GetChessCount() > 0) continue;


                //斜方向：要考虑山脉阻挡
                if (dx * dy != 0)
                {
                    if(NextTile(start, 0, dy)!= null && NextTile(start, 0, dy).IsMountain()&&
                            NextTile(start, dx, 0) != null && NextTile(start, dx, 0).IsMountain())
                    {
                        continue;
                    }
                }

                list.Add(next);
            }
        }

       


        return list;
    }

    /// <summary>
    /// 可以一步飞到的所有格子
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    private List<TileObject> FlyAccessibleTiles(TileObject start)
    {
        var list = new List<TileObject>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                TileObject next = NextTile(start, dx, dy);

                if (next == null || next.GetChessType()!= ChessData.Type.WIND || next.IsFlied()) continue;

                int count = 0, step = next.GetChessCount();

                while(count < step)
                {
                    count++;
                    next = NextTile(next, dx, dy);

                    if (next == null) break;

                    //飞到风元素上时，增加飞行距离
                    if(next.GetChessType() == ChessData.Type.WIND)
                    {
                        step += next.GetChessCount();
                    }
                }

                //落脚点不是空格，或超出地图，不能飞行
                if (next == null || next.GetChessCount() != 0 || count != step) continue;


                //斜方向：要考虑山脉阻挡
                if (dx * dy != 0)
                {
                    if (NextTile(start, 0, dy) != null && NextTile(start, 0, dy).IsMountain() &&
                            NextTile(start, dx, 0) != null && NextTile(start, dx, 0).IsMountain())
                    {
                        continue;
                    }
                }

                list.Add(next);
            }
        }

        return list;
    }

    public void ClearChess()
    {
        if (tileList == null) return;

        foreach (TileObject obj in tileList)
        {
            TileController.Instance.RemoveAllChess(obj);
        }
    }

    public void ClearAll()
    {
        if (tileList == null) return;

        foreach (TileObject obj in tileList)
        {
            Destroy(obj.gameObject);
        }

        tileList.Clear();

    }

    public TileObject GetTile(int x, int y)
    {
        if (tileArray == null) return null;

        if (x < 0 || x <= column || y < 0 || y >= column) return null;

        return tileArray[x, y];

    }

    public void DestroyTile(TileObject tile)
    {
        int x = tile.GetX();
        int y = tile.GetY();

        DestroyTile(x, y);
    }

    public void DestroyTile(int x, int y)
    {
        TileObject t = GetTile(x, y);

        if (t == null) return;

        tileArray[x, y] = null;

        tileList.Remove(t);

        Destroy(t.gameObject);

    }

    public void SetTile(int x, int y, TileObject tile)
    {
        DestroyTile(x, y);

        tileArray[x, y] = tile;

        tile.SetPosition(x, y);

        tileList.Add(tile);
    }

    public TileObject GetSelectedTile()
    {
        return selectedTile;
    }

    public void SetSelectedTile(TileObject tile)
    {
        selectedTile = tile;
    }
    
    /// <summary>
    /// 在某个格子中放置棋子
    /// </summary>
    /// <param name="tile">目标格子</param>
    /// <param name="chess">目标棋子</param>
    /// <param name="_cover">是否覆盖原有棋子。若已经放有棋子并选择false，则会堆叠棋子至最上层</param>
    public void PlaceChess(TileObject tile, ChessObject chess, bool _cover = false)
    {

        if (_cover)
        {
            RemoveAllChess(tile);
        }

        tile.Place(chess);
        
    }

    /// <summary>
    /// 火元素规则：向正交方向扩展
    /// </summary>
    /// <param name="tile"></param>
    private void FireExpand(TileObject tile)
    {

        foreach(TileObject obj in GetFireExpandTiles(tile))
        {
            PlaceChess(obj, ChessController.Instance.CreateChessObject(ChessData.Type.FIRE, obj.transform), CanReplace(obj, ChessData.Type.FIRE));
        }

        /*
        TileObject next;

        for(int dx = -1; dx <= 1; dx ++)
        {
            for(int dy = -1; dy <= 1; dy ++)
            {

                if (dx * dy != 0 || (dx == 0 && dy == 0)) continue;

                next = NextTile(tile, dx, dy);

                int count = 0;
                while(next != null && next.GetChessType()== ChessData.Type.FIRE)
                {
                    count++;
                    next = NextTile(next, dx, dy);
                }

                if (next == null || count < 1) continue;

                if(CanPlace(next, ChessData.Type.FIRE) || CanReplace(next, ChessData.Type.FIRE))
                {
                    PlaceChess(next, ChessController.Instance.CreateChessObject(ChessData.Type.FIRE, next.transform), CanReplace(next, ChessData.Type.FIRE));
                }

            }
        }
        */

    }

    private List<TileObject> GetFireExpandTiles(TileObject tile)
    {
        var list = new List<TileObject>();

        TileObject next;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {

                if (dx * dy != 0 || (dx == 0 && dy == 0)) continue;

                next = NextTile(tile, dx, dy);

                int count = 0;
                while (next != null && next.GetChessType() == ChessData.Type.FIRE)
                {
                    count++;
                    next = NextTile(next, dx, dy);
                }

                if (next == null || count < 1) continue;

                if (CanPlace(next, ChessData.Type.FIRE) || CanReplace(next, ChessData.Type.FIRE))
                {
                    list.Add(next);
                    //PlaceChess(next, ChessController.Instance.CreateChessObject(ChessData.Type.FIRE, next.transform), CanReplace(next, ChessData.Type.FIRE));
                }

            }
        }

        return list;
    }

    /// <summary>
    /// 土元素规则：向八个方向形成山脉
    /// </summary>
    /// <param name="tile"></param>
    private void MountainExpand(TileObject tile)
    {
        List<TileObject> open = new List<TileObject>(), close = new List<TileObject>();
        close.Add(tile);

        open.AddRange(AdjacentTiles(tile));

        while(open.Count > 0)
        {
            //下次循环中遍历的格子
            List<TileObject> nextOpen = new List<TileObject>();


            foreach (TileObject t in open)
            {

                //若有地元素，则附加山脉属性
                if (t.GetChessType() == ChessData.Type.GROUND)
                {
                    t.SetMountain(true);
                }

                //将该格子周围八格放入下次遍历的列表
                var temp = AdjacentTiles(t);
                var toRemove = new List<TileObject>();

                //如果该格子已被访问过，或即将被访问，则去除
                foreach(TileObject _t in temp)
                {
                    if(open.Contains(_t) || close.Contains(_t) || nextOpen.Contains(_t))
                    {
                        toRemove.Add(_t);
                    }

                    //open.Add(_t);
                }

                foreach(TileObject r in toRemove)
                {
                    if (temp.Contains(r))
                        temp.Remove(r);
                }
                nextOpen.AddRange(temp);


            }

            close.AddRange(open);
            open.Clear();

            open.AddRange(nextOpen);





        }
    }

    /// <summary>
    /// 以一个水元素为中心，搜索周围可以流动的水流。若没有，则无法流动
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private List<List<TileObject>> FindStream(TileObject tile)
    {

        var list = new List<List<TileObject>>();

        for(int dx = -1; dx <=1; dx++)
        {
            for(int dy = -1; dy <= 1; dy++)
            {
                //只考虑正交方向
                if ((dx == 0 && dy == 0) || dx * dy != 0)
                {
                    continue;
                }


                var stream = new List<TileObject>();
                stream.Add(tile);

                var next = NextTile(tile, dx, dy);

                while(next!= null && next.GetChessType() == ChessData.Type.WATER)
                {
                    stream.Add(next);

                    next = NextTile(next, dx, dy);
                }
                
                if (stream.Count > 1)
                {
                    list.Add(stream);
                }
            }
        }

        return list;
    }

    private List<TileObject> Flow(List<TileObject> stream, TileObject dest)
    {
        MoveChess(stream[0], dest);

        for (int i = 1; i < stream.Count; i++)
        {
            MoveChess(stream[i], stream[i - 1]);
        }

        var newStream = new List<TileObject>();

        newStream.Add(dest);

        for (int i = 1; i < stream.Count; i++)
        {
            newStream.Add(stream[i-1]);
        }

        return newStream;
    }

    private List<TileObject> FlowableTiles(TileObject tile)
    {
        var list = new List<TileObject>();

        foreach(var t in ConnectedTiles(tile))
        {
            if(CanPlace(t, ChessData.Type.WATER) || CanReplace(t, ChessData.Type.WATER))
            {
                list.Add(t);
            }
        }

        return list;
    }


    /// <summary>
    /// 获得目标周围8个方向的格子
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    private List<TileObject> AdjacentTiles(TileObject tile)
    {
        List<TileObject> list = new List<TileObject>();

        for(int dx = -1; dx <= 1; dx++)
        {
            for(int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                TileObject next = NextTile(tile, dx, dy);

                if(next != null)
                {
                    list.Add(next);
                }
            }
        }

        return list;
    }

    private List<TileObject> ConnectedTiles(TileObject tile)
    {
        List<TileObject> list = new List<TileObject>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                if (dx * dy != 0) continue;

                TileObject next = NextTile(tile, dx, dy);

                if (next != null)
                {
                    list.Add(next);
                }
            }
        }

        return list;
    }

    public ChessObject RemoveChess(TileObject tile, bool _destroy = true)
    {
        ChessObject chess = tile.Remove();

        if (_destroy)
        {
            Destroy(chess.gameObject);
            return null;
        }
        else
        {
            return chess;
        }
    }

    public List<ChessObject> RemoveAllChess(TileObject tile, bool _destroy = true)
    {
        List<ChessObject> list = tile.RemoveAll();

        if (list != null && _destroy)
        {
            foreach(ChessObject chess in list) {
                Destroy(chess.gameObject);
            }
            return null;
        }
        else
        {
            return list;
        }
    }

    public bool CanReplace(TileObject tile, ChessData.Type tp)
    {

        
        switch (tp)
        {
            case ChessData.Type.FIRE:

                if (tile.GetChessType() == ChessData.Type.WIND) return true;
                else return false;

            case ChessData.Type.WATER:

                if (tile.GetChessType() == ChessData.Type.FIRE) return true;
                else return false;

            case ChessData.Type.GROUND:
                if (tile.GetChessType() == ChessData.Type.WATER)
                    return true;
                else
                    return false;

            case ChessData.Type.WIND:
                if (tile.GetChessType() == ChessData.Type.GROUND && !tile.IsMountain())
                    return true;
                else return false;
        }

        return false;
    }

    public bool CanPlace(TileObject tile, ChessData.Type tp)
    {
        if (tile.GetChessCount() == 0) return true;
        if (tile.GetChessCount() > 3) return false;

        switch (tp)
        {

            case ChessData.Type.GROUND:
                if (tile.GetChessType() == ChessData.Type.GROUND && tile.GetChessCount() == 1)
                    return true;
                else
                    return false;

            case ChessData.Type.WIND:
                if (tile.GetChessType() == ChessData.Type.WIND && tile.GetChessCount() <=3)
                    return true;
                else return false;
        }

        return false;
    }


    public void EnterTile(TileObject tile)
    {

        if (!MovementController.Instance.IsActing()) return;

        //如果正在流动水元素的过程中
        if (streaming)
        {
            
            if (FlowableTiles(streamList[selectedStream][0]).Contains(tile))
            {

                if(!streamSelected)
                {
                    streamSelected = true;

                    flowStep = streamList[selectedStream].Count;
                    flowCount = 0;
                }


                streamList[selectedStream] = Flow(streamList[selectedStream], tile);

                flowCount++;

                colorController.ClearAllColor();

                colorController.ShowColor(streamList[selectedStream], Config.TILE_COLOR_STREAM);
                colorController.ShowColor(FlowableTiles(streamList[selectedStream][0]), Config.TILE_COLOR_WATER);

                if(flowCount == flowStep || FlowableTiles(streamList[selectedStream][0]).Count == 0)
                {
                    streaming = false;

                    colorController.ClearAllColor();
                }

            }


        }
        else
        {
            ChessData.Type tp = MovementController.Instance.selectedType;

            if (tp == ChessData.Type.EMPTY) return;

            //若无法放置当前元素，跳过显示阶段
            if (tile.GetChessCount() != 0 && !CanPlace(tile, tp) && !CanReplace(tile, tp))
                return;



            colorController.ShowColor(tile, Config.TILE_COLOR_HIGHLIGHTED);

            switch (tp)
            {
                case ChessData.Type.FIRE:

                    colorController.ShowColor(GetFireExpandTiles(tile), Config.TILE_COLOR_FIRE);

                    break;

                case ChessData.Type.WATER:

                    //显示可选的所有水流
                    streamList = FindStream(tile);

                    if (streamList.Count > 0)
                    {
                        foreach (var stream in streamList)
                        {
                            foreach (var t in stream)
                            {
                                if (t != tile)
                                {
                                    colorController.ShowColor(t, Config.TILE_COLOR_STREAM);
                                }
                            }
                        }
                    }

                    break;


            }
        }



        
    }

    public void ExitTile(TileObject tile)
    {
        if (streaming) return;

        var tp = MovementController.Instance.selectedType;

        if (tp != ChessData.Type.EMPTY)
        {
            colorController.ClearAllColor();
        }
        
    }


    /// <summary>
    /// 单击某个格子时进行操作
    /// </summary>
    /// <param name="tile"></param>
    public void ClickTile(TileObject tile)
    {

        if (!MovementController.Instance.IsActing()) return;


        ChessData.Type tp = MovementController.Instance.selectedType;

        if (streaming)
            return;

        //选中的棋子类型符合要求
        if (tp == ChessData.Type.FIRE || tp == ChessData.Type.WATER || tp == ChessData.Type.WIND || tp == ChessData.Type.GROUND)
        {

            //符合放置或替换条件
            if (!CanPlace(tile, tp) && !CanReplace(tile, tp))

                return;

            else {

                DrawController.Instance.Use(tp);

                //放置或替换
                PlaceChess(tile, ChessController.Instance.CreateChessObject(tp, tile.transform), CanReplace(tile, tp));
                colorController.ClearAllColor();

                //执行元素的特定规则
                switch (tp)
                {
                    case ChessData.Type.FIRE:

                        FireExpand(tile);
                        break;

                    case ChessData.Type.GROUND:

                        if(tile.GetChessCount() > 1)
                        {
                            tile.SetMountain(true);
                            MountainExpand(tile);
                        }
                        else if (tile.GetChessCount() == 1)
                        {
                            foreach (TileObject obj in AdjacentTiles(tile))
                            {
                                if (obj.IsMountain())
                                {
                                    tile.SetMountain(true);
                                    MountainExpand(tile);
                                    break;
                                }

                            }
                        }
                        break;

                    case ChessData.Type.WATER:

                        streamList = FindStream(tile);
                        if (streamList.Count == 0) break;

                        streaming = true;
                        streamSelected = false;

                        selectedStream = 0;

                        colorController.ShowColor(streamList[0], Config.TILE_COLOR_STREAM);

                        var show = FlowableTiles(streamList[selectedStream][0]);

                        if (show.Count > 0)
                        {
                            colorController.ShowColor(show, Config.TILE_COLOR_WATER);
                        }
                        //如果没有位置流，直接结束放置
                        else
                        {
                            streaming = false;
                        }
                        break;
                }

            }

        }
        //移动
        else if (tp == ChessData.Type.EMPTY)
        {
            var player = playerTileList[TurnController.Instance.CurrentPlayer()];

            if (CanMoveTo(player, tile))
            {

                if (MoveAccessibleTiles(player).Contains(tile))
                {
                    MovePlayer(tile);
                    DrawController.Instance.Use(tp);
                }
                else
                {
                    FlyPlayer(tile);
                }

                ShowPlayerMove();

                
            }
        }

    }

    private TileObject NextTile(TileObject t, int dx, int dy)
    {

        int x = t.GetX() + dx;
        int y = t.GetY() + dy;

        if (x < 0 || y < 0 || x >= column || y >= row) return null;
        else return tileArray[x, y];
        
    }

    
    private void Update()
    {
        if (prepared && GameController.Instance.IsPlaying() && MovementController.Instance.IsActing())
        {
            
            //水流时，监听鼠标滚轮
            if(streaming && !streamSelected)
            {
                var dv = Input.GetAxis("Mouse ScrollWheel");

                bool changed = false;
                
                if(dv > 0)
                {
                    changed = true;

                    selectedStream++;
                    if (selectedStream >= streamList.Count) selectedStream = 0;

                }else if(dv < 0)
                {
                    changed = true;

                    selectedStream--;
                    if (selectedStream < 0) selectedStream = streamList.Count -1;
                }

                if (changed)
                {

                    Debug.Log("stream changed");

                    colorController.ClearAllColor();

                    colorController.ShowColor(streamList[selectedStream], Config.TILE_COLOR_STREAM);
                    colorController.ShowColor(FlowableTiles(streamList[selectedStream][0]), Config.TILE_COLOR_WATER);

                }
            }

            if(MovementController.Instance.selectedType == ChessData.Type.EMPTY)
            {
                
                ShowPlayerMove();
                
            }


        }
    }

    public void SwitchColor()
    {
        colorController.ClearAllColor();

        
    }

    private void ShowPlayerMove()
    {
        colorController.ClearAllColor();

        colorController.ShowColor(FlyAccessibleTiles(playerTileList[TurnController.Instance.CurrentPlayer()]), Config.TILE_COLOR_FLY);
        colorController.ShowColor(MoveAccessibleTiles(playerTileList[TurnController.Instance.CurrentPlayer()]), Config.TILE_COLOR_MOVE);
    }
    

}
