using System;
using System.Collections.Generic;

public class AstarPathFinding
{
    //マップ要素
    private const char START = 'S';//スタート
    private const char END = 'E';//ゴール
    private const char SPACE = '.';//スペース
    private const char WALL = 'W';//壁
    private const char VISITED = '-';//探索済み
    private const char ON_PATH = '@';//

    //baseElementからcharへ変更
    public static char[,] MAP = null;
    //マップサイズ上限
    public static Point MAX_PNT = null;
    //スタート
    public static Point START_PNT = null;
    //ゴール
    public static Point END_PNT = null;
    //todo

    /// <summary>
    /// マンハッタン距離
    /// </summary>
    /// <param name="pnt">現在評価される点</param>
    /// <returns>評価されているH</returns>
    private static double HManhattanDistance(Point pnt)
    {
        return Math.Abs(pnt.x - END_PNT.x) + Math.Abs(pnt.y - END_PNT.y);
    }

    /// <summary>
    /// ユークリッド距離
    /// </summary>
    /// <param name="pnt">現在評価される点</param>
    /// <returns>評価されているH</returns>
    private static double HEuclidianDistance(Point pnt)
    {
        return Math.Sqrt(Math.Pow(pnt.x - END_PNT.x,2)) + Math.Sqrt(Math.Pow(pnt.y - END_PNT.y,2));//` √a^2+b^2
    }

    /// <summary>
    /// ユークリッド距離の^2
    /// </summary>
    /// <param name="pnt">現在評価される点</param>
    /// <returns>評価されているH</returns>
    private static double HPowEuclidianDistance(Point pnt)
    {
        return Math.Pow(pnt.x - END_PNT.x,2) + Math.Pow(pnt.y - END_PNT.y,2);
    }
    
    //H関数
    private static double HFun(Point pnt)
    {
        return HManhattanDistance(pnt);
    }

    private void Search()
    {
        List<PointData> openList = new List<PointData>();
        //8方向
        int[,] directs = { {1,0},{ 0,1},{-1,0},{0,-1},{1,1},{1,-1},{-1,1},{-1,-1} };
        //スタートをopenListに入れる
        openList.Add(new PointData(START_PNT, 0, 0, null));
        // 
        PointData endData = null;
        //ステップ　2　
        for(bool finish = false; !finish && openList.Count > 0;)　//　C*,E
        {
            openList.Sort((x, y) => { return x.F().CompareTo(y.F()); });
            PointData data = openList[0];//a
            openList.RemoveAt(0);//b
            Point point = data.point;
            if(MAP[point.x,point.y] == SPACE)
            {
                MAP[point.x, point.y] = VISITED;
            }
            for (int i = 0; i < directs.GetLength(0); i++)
            {
                Point newPoint = new Point(point.x+directs[i,0],point.y + directs[i,1]);
                if (newPoint.x >= 0 && newPoint.x < MAX_PNT.x && newPoint.y >= 0 && newPoint.y < MAX_PNT.y) 
                {
                    char e = MAP[newPoint.x, newPoint.y];
                    if(e == END)
                    {
                        endData = data;
                        finish = true;
                        break;
                    }
                    if(e != SPACE)
                    {
                        continue;
                    }
                    //Gの値チェックと更新
                    PointData tempData = openList.Find((x)=> { return x.point.Equals(newPoint); });
                    //d*a,b
                    if(tempData != null)
                    {
                        float goffset;
                        if (Math.Abs(directs[i, 0]) + Math.Abs(directs[i, 1]) > 1)
                        {
                            goffset = 1.4f;
                        }
                        else
                        {
                            goffset = 1.0f;      
                        }
                        if(tempData.g > data.g + goffset)
                        {
                            tempData.g = data.g + goffset;
                            tempData.parent = data;
                        }
                    }
                    //openListになければ、入れる
                    else
                    {
                        float goffset;
                        if (Math.Abs(directs[i, 0]) + Math.Abs(directs[i, 1]) > 1)
                        {
                            goffset = 1.4f;
                        }
                        else
                        {
                            goffset = 1.0f;
                        }
                        double h = HFun(newPoint);
                        PointData newData = new PointData(newPoint, data.g + goffset, h, data);
                        openList.Add(newData);
                    }
                }
            }
        }
        //ルートを決める
        for (PointData pathData = endData;pathData != null;)
        {
            Point point = pathData.point;
            if (MAP[point.x,point.y]== VISITED)
            {
                MAP[point.x, point.y] = ON_PATH;
            }
            pathData = pathData.parent;
        }
    }
}

