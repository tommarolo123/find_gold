using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }

    }
    [Header("キャラクター")]
    public GameObject player;

    [Header("プレハブ")]
    [Tooltip("背景プレハブ")]
    public GameObject bgElement;
    [Tooltip("borderPrefab、順番は：\n上、下、左、右、右上、左下、右下")]
    public GameObject[] borderEnements;
    public GameObject baseElement;
    public GameObject flagElement;
    public GameObject errorElement;

    [Header("特效")]
    public GameObject smokeEffect;
    public GameObject UncoverEffect;
    public GameObject goldEffect;

    [Header("素材")]
    public Sprite[] coverTilesSprites;
    public Sprite[] trapsSprites;
    public Sprite[] numberSprites;
    public Sprite[] toolSprites;
    public Sprite[] goldSprites;
    public Sprite[] bigwallSprites;
    public Sprite[] smallwallSprites;
    public Sprite[] enemySprites;
    public Sprite exitSprite;
    public Sprite doorSprite;
    [Header("マップ設定")]
    public int w;
    public int h;
    public float minTrapProbability;
    public float maxTrapProbability;
    public float uncoProbability;
    public int standAreaW;
    public int obstacleAreaW;

    [HideInInspector]
    public int obstacleAreaNum;

    //マップelement
    public BaseElement[,] mapArray;

    private void Awake()
    {
        _instance = this;
        mapArray = new BaseElement[w, h];
        obstacleAreaNum = (w - (standAreaW + 3)) / obstacleAreaW;
    }

    private void Start()
    {
        CreateMap();
        ResetCamera();
        InitMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetTarget();
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            player.transform.GetChild(0).transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0))+new Vector3(0,0,10);
        }
        if (Input.GetMouseButton(0))
        {
            player.transform.GetChild(0).transform.position += new Vector3(-Input.GetAxis("Mouse X"), 0, 0);
        }
    }
    /// <summary>
    /// 虚拟摄像机与玩家位置同步
    /// </summary>
    private void ResetTarget()
    {
        player.transform.GetChild(0).transform.localPosition = Vector3.zero;
    }

    private void CreateMap()
    {
        Transform bgHolder = GameObject.Find("ElementsHolder/Background").transform;
        Transform elementHolder = GameObject.Find("ElementsHolder").transform;
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                Instantiate(bgElement, new Vector3(i, j, 0), Quaternion.identity, bgHolder);
                mapArray[i, j] = Instantiate(baseElement, new Vector3(i, j, 0), Quaternion.identity, bgHolder).GetComponent<BaseElement>();
            }
        }
        for (int i = 0; i < w; i++)
        {
            Instantiate(borderEnements[0], new Vector3(i, h + 0.25f, 0), Quaternion.identity, bgHolder);
            Instantiate(borderEnements[1], new Vector3(i, -1.25f, 0), Quaternion.identity, bgHolder);
        }
        for (int i = 0; i < h; i++)
        {
            Instantiate(borderEnements[2], new Vector3(-1.25f, i, 0), Quaternion.identity, bgHolder);
            Instantiate(borderEnements[3], new Vector3(w + 0.25f, i, 0), Quaternion.identity, bgHolder);
        }
        Instantiate(borderEnements[4], new Vector3(-1.25f, h + 0.25f, 0), Quaternion.identity, bgHolder);
        Instantiate(borderEnements[5], new Vector3(w + 0.25f, h + 0.25f, 0), Quaternion.identity, bgHolder);
        Instantiate(borderEnements[6], new Vector3(-1.25f, -1.25f, 0), Quaternion.identity, bgHolder);
        Instantiate(borderEnements[7], new Vector3(w + 0.25f, -1.25f, 0), Quaternion.identity, bgHolder);
    }

    private void ResetCamera()
    {
        //Camera.main.orthographicSize = (h + 3) / 2f;
        //Camera.main.transform.position = new Vector3((w - 1) / 2f, (h - 1) / 2f, -10);
        CinemachineVirtualCamera vCam = GameObject.Find("VCam").GetComponent<CinemachineVirtualCamera>();
        vCam.m_Lens.OrthographicSize = (h + 3) / 2f;
        CinemachineFramingTransposer ft = vCam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFramingTransposer;
        ft.m_DeadZoneHeight = (h * 100) / (300 + h * 100f);
        ft.m_DeadZoneWidth = (h * 100) / (300 + h * 100f) / 9 * 16 / h;
        GetComponent<PolygonCollider2D>().SetPath(0,
            new Vector2[]{
                new Vector2(-2f,-2f),
                new Vector2(-2f,h+1f),
                new Vector2(w+1f,h+1f),
                new Vector2(w+1f,-2f),

            });
    }

    private void InitMap()
    {
        List<int> availableIndex = new List<int>();
        for (int i = 0; i < w * h; i++)
        {
            availableIndex.Add(i);
        }
        int standAreaY = Random.Range(1, h - 1);
        GenerateExit(availableIndex);
        GenerateObstacleArea(availableIndex);
        GenerateTool(availableIndex);
        GenerateGold(availableIndex);
        GenerateTrap(standAreaY,availableIndex);
        GenerateNumber(availableIndex);
        GenerateStandArea(standAreaY);
    }

    /// <summary>
    /// 出口を生成
    /// </summary>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void GenerateExit(List<int> availableIndex)
    {
        float x = w - 1.5f;
        float y = Random.Range(1, h) - 0.5f;
        BaseElement exit = SetElement(GetIndex((int)(x + 0.5), (int)(y - 0.5)), ElementContent.Exit);
        exit.transform.position = new Vector3(x, y, 0);
        Destroy(exit.GetComponent<BoxCollider2D>());
        exit.gameObject.AddComponent<BoxCollider2D>();
        availableIndex.Remove(GetIndex((int)(x + 0.5), (int)(y - 0.5)));
        availableIndex.Remove(GetIndex((int)(x + 0.5), (int)(y + 0.5)));
        availableIndex.Remove(GetIndex((int)(x - 0.5), (int)(y - 0.5)));
        availableIndex.Remove(GetIndex((int)(x - 0.5), (int)(y + 0.5)));
        Destroy(mapArray[(int)(x + 0.5), (int)(y + 0.5)].gameObject);
        Destroy(mapArray[(int)(x - 0.5), (int)(y - 0.5)].gameObject);
        Destroy(mapArray[(int)(x - 0.5), (int)(y + 0.5)].gameObject);
        mapArray[(int)(x + 0.5), (int)(y + 0.5)] = exit;
        mapArray[(int)(x - 0.5), (int)(y - 0.5)] = exit;
        mapArray[(int)(x - 0.5), (int)(y + 0.5)] = exit;

    }

    /// <summary>
    /// 障害物を生成
    /// </summary>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void GenerateObstacleArea(List<int> availableIndex)
    {
        for (int i = 0; i < obstacleAreaNum; i++)
        {
            if (Random.value < 0.5f)
            {
                CreateCloseArea(i, availableIndex);
            }
            else
            {
                CreateRandomWall(i, availableIndex);
            }
        }
    }

    /// <summary>
    /// 闭合区域信息结构体
    /// </summary>
    private struct CloseAreaInfo
    {
        public int x, y, sx, ex, sy, ey;
        public int doorType;
        public Vector2 doorPos;
        public int tx, ty;
        public ToolElement t;
        public int gx, gy;
        public GoldElement g;
        public int innerCount, goldNum;

    }
    /// <summary>
    /// 生成闭合区域信息
    /// </summary>
    /// <param name="type">闭合区域类型，0:边界闭合1:自闭合</param>
    /// <param name="nowArea">闭合区域的索引值</param>
    /// <param name="info">要生成的闭合区域信息结构体</param>
    private void CreateCloseAreaInfo(int type, int nowArea, ref CloseAreaInfo info)
    {
        switch (type)
        {
            case 0:
                info.x = Random.Range(3, obstacleAreaW - 2);
                info.y = Random.Range(3, h - 3);
                info.sx = standAreaW + nowArea * obstacleAreaW + 1;
                info.ex = info.sx + info.x;
                info.doorType = Random.Range(4, 8);
                break;
            case 1:

                info.x = Random.Range(3, obstacleAreaW - 2);
                info.y = Random.Range(3, info.x + 1);
                info.sx = standAreaW + nowArea * obstacleAreaW + 1;
                info.ex = info.sx + info.x;
                info.sy = Random.Range(3, h - info.y - 1);
                info.ey = info.sy + info.y;
                info.doorType = (int)ElementContent.BigWall;

                break;
        }
    }

    /// <summary>
    /// 生成开启闭合障碍物区域所需要道具
    /// </summary>
    /// <param name="info"></param>
    /// <param name="availableIndex"></param>
    private void CreateCloseAreaTool(CloseAreaInfo info, List<int> availableIndex)
    {
        info.tx = Random.Range(0, info.sx);
        info.ty = Random.Range(0, h);
        for (; !availableIndex.Contains(GetIndex(info.tx, info.ty));)
        {
            info.tx = Random.Range(0, info.sx);
            info.ty = Random.Range(0, h);
        }
        availableIndex.Remove(GetIndex(info.tx, info.ty));
        info.t = (ToolElement)SetElement(GetIndex(info.tx, info.ty), ElementContent.Tool);
        info.t.toolType = (ToolType)info.doorType;
        if (info.t.isHide == false)
        {
            info.t.ConfirmSprite();
        }
    }

    /// <summary>
    /// 生成封闭障碍物区域
    /// </summary>
    /// <param name="nowArea">当前障碍物区域的索引值</param>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void CreateCloseArea(int nowArea, List<int> availableIndex)
    {
        int shape = Random.Range(0, 2);
        CloseAreaInfo info = new CloseAreaInfo();
        switch (shape)
        {
            //0和地图闭合
            case 0:
                CreateCloseAreaInfo(0, nowArea, ref info);
                int dir = Random.Range(0, 4);
                switch (dir)
                {
                    case 0:
                        info.doorPos = Random.value < 0.5 ? new Vector2(Random.Range(info.sx + 1, info.ex), info.y) : new Vector2(Random.value < 0.5 ? info.sx : info.ex, Random.Range(info.y, h));
                        CreateULShapeAreaDoor(info, availableIndex);
                        for (int i = h - 1; i > info.y; i--)
                        {
                            if (availableIndex.Contains(GetIndex(info.sx, i)))
                            {
                                availableIndex.Remove(GetIndex(info.sx, i));
                                SetElement(GetIndex(info.sx, i), ElementContent.BigWall);
                            }
                        }
                        for (int i = info.sx; i < info.ex; i++)
                        {
                            if (availableIndex.Contains(GetIndex(i, info.y)))
                            {
                                availableIndex.Remove(GetIndex(i, info.y));
                                SetElement(GetIndex(i, info.y), ElementContent.BigWall);
                            }
                        }

                        for (int i = h - 1; i >= info.y; i--)
                        {
                            if (availableIndex.Contains(GetIndex(info.ex, i)))
                            {
                                availableIndex.Remove(GetIndex(info.ex, i));
                                SetElement(GetIndex(info.ex, i), ElementContent.BigWall);
                            }
                        }
                        info.sy = info.y;
                        info.ey = h - 1;
                        info.y = h - 1 - info.y;
                        CreateCloseAreaRewards(info, availableIndex);

                        break;
                    case 1:
                        info.doorPos = Random.value < 0.5 ? new Vector2(Random.Range(info.sx + 1, info.ex), info.y) : new Vector2(Random.value < 0.5 ? info.sx : info.ex, Random.Range(0, info.y));
                        CreateULShapeAreaDoor(info, availableIndex);
                        for (int i = 0; i < info.y; i++)
                        {
                            if (availableIndex.Contains(GetIndex(info.sx, i)))
                            {
                                availableIndex.Remove(GetIndex(info.sx, i));
                                SetElement(GetIndex(info.sx, i), ElementContent.BigWall);
                            }
                        }
                        for (int i = info.sx; i < info.ex; i++)
                        {
                            if (availableIndex.Contains(GetIndex(i, info.y)))
                            {
                                availableIndex.Remove(GetIndex(i, info.y));
                                SetElement(GetIndex(i, info.y), ElementContent.BigWall);
                            }
                        }

                        for (int i = 0; i <= info.y; i++)
                        {
                            if (availableIndex.Contains(GetIndex(info.ex, i)))
                            {
                                availableIndex.Remove(GetIndex(info.ex, i));
                                SetElement(GetIndex(info.ex, i), ElementContent.BigWall);
                            }
                        }
                        info.sy = 0;
                        info.ey = info.y;
                        info.y = h - 1 - info.y;
                        CreateCloseAreaRewards(info, availableIndex);

                        break;
                    case 2:
                        info.doorPos = Random.value < 0.5 ? new Vector2(Random.Range(info.sx + 1, info.ex), info.y) : new Vector2(info.sx, Random.Range(info.y, h));
                        CreateULShapeAreaDoor(info, availableIndex);
                        for (int i = h - 1; i > info.y; i--)
                        {
                            if (availableIndex.Contains(GetIndex(info.sx, i)))
                            {
                                availableIndex.Remove(GetIndex(info.sx, i));
                                SetElement(GetIndex(info.sx, i), ElementContent.BigWall);
                            }
                        }
                        for (int i = info.sx; i < info.ex; i++)
                        {
                            if (availableIndex.Contains(GetIndex(i, info.y)))
                            {
                                availableIndex.Remove(GetIndex(i, info.y));
                                SetElement(GetIndex(i, info.y), ElementContent.BigWall);
                            }
                        }

                        for (int i = 0; i <= info.y; i++)
                        {
                            if (availableIndex.Contains(GetIndex(info.ex, i)))
                            {
                                availableIndex.Remove(GetIndex(info.ex, i));
                                SetElement(GetIndex(info.ex, i), ElementContent.BigWall);
                            }
                        }

                        break;
                    case 3:
                        info.doorPos = Random.value < 0.5 ? new Vector2(Random.Range(info.sx + 1, info.ex), info.y) : new Vector2(Random.value < 0.5 ? info.sx : info.ex, Random.Range(0, info.y));
                        CreateULShapeAreaDoor(info, availableIndex);
                        for (int i = 0; i < info.y; i++)
                        {
                            if (availableIndex.Contains(GetIndex(info.sx, i)))
                            {
                                availableIndex.Remove(GetIndex(info.sx, i));
                                SetElement(GetIndex(info.sx, i), ElementContent.BigWall);
                            }
                        }
                        for (int i = info.sx; i < info.ex; i++)
                        {
                            if (availableIndex.Contains(GetIndex(i, info.y)))
                            {
                                availableIndex.Remove(GetIndex(i, info.y));
                                SetElement(GetIndex(i, info.y), ElementContent.BigWall);
                            }
                        }

                        for (int i = h - 1; i > info.y; i--)
                        {
                            if (availableIndex.Contains(GetIndex(info.ex, i)))
                            {
                                availableIndex.Remove(GetIndex(info.ex, i));
                                SetElement(GetIndex(info.ex, i), ElementContent.BigWall);
                            }
                        }

                        break;
                }
                CreateCloseAreaTool(info, availableIndex);
                break;
            //1和障碍物闭合
            case 1:
                CreateCloseAreaInfo(1, nowArea, ref info);
                for (int i = info.sx; i <= info.ex; i++)
                {
                    if (availableIndex.Contains(GetIndex(i, info.sy)))
                    {
                        availableIndex.Remove(GetIndex(i, info.sy));
                        SetElement(GetIndex(i, info.sy), ElementContent.BigWall);
                    }

                    if (availableIndex.Contains(GetIndex(i, info.ey)))
                    {
                        availableIndex.Remove(GetIndex(i, info.ey));
                        SetElement(GetIndex(i, info.ey), ElementContent.BigWall);
                    }

                }

                for (int i = info.sy + 1; i <= info.ey; i++)
                {
                    if (availableIndex.Contains(GetIndex(info.sx, i)))
                    {
                        availableIndex.Remove(GetIndex(info.sx, i));
                        SetElement(GetIndex(info.sx, i), ElementContent.BigWall);
                    }

                    if (availableIndex.Contains(GetIndex(info.ex, i)))
                    {
                        availableIndex.Remove(GetIndex(info.ex, i));
                        SetElement(GetIndex(info.ex, i), ElementContent.BigWall);
                    }

                }
                CreateCloseAreaTool(info, availableIndex);
                CreateCloseAreaRewards(info, availableIndex);
                break;
        }
    }

    /// <summary>
    /// 生成U或L闭合障碍物区域的门
    /// </summary>
    /// <param name="info"></param>
    /// <param name="availableIndex"></param>
    private void CreateULShapeAreaDoor(CloseAreaInfo info, List<int> availableIndex)
    {
        availableIndex.Remove(GetIndex((int)info.doorPos.x, (int)info.doorPos.y));
        SetElement(GetIndex((int)info.doorPos.x, (int)info.doorPos.y), (ElementContent)info.doorType);
    }

    /// <summary>
    /// 生成闭合障碍物区域内的奖励物品
    /// </summary>
    /// <param name="info"></param>
    /// <param name="availableIndex"></param>
    private void CreateCloseAreaRewards(CloseAreaInfo info, List<int> availableIndex)
    {
        info.innerCount = info.x * info.y;
        info.goldNum = Random.Range(1, Random.value < 0.5f ? info.innerCount + 1 : info.innerCount / 2);
        for (int i = 0; i < info.goldNum; i++)
        {
            info.gy = i / info.x;
            info.gx = i - info.gy * info.x;
            info.gx = info.sx + info.gx + 1;
            info.gy = info.sy + info.gy + 1;
            if (availableIndex.Contains(GetIndex(info.gx, info.gy)))
            {
                availableIndex.Remove(GetIndex(info.gx, info.gy));
                info.g = (GoldElement)SetElement(GetIndex(info.gx, info.gy), ElementContent.Gold);
                info.g.goldType = (GoldType)Random.Range(0, 7);
                if (info.g.isHide == false)
                {
                    info.g.ConfirmSprite();
                }
            }
        }
    }

    /// <summary>
    /// 生成随机离散障碍物
    /// </summary>
    /// <param name="nowArea">当前障碍物区域的索引值</param>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void CreateRandomWall(int nowArea, List<int> availableIndex)
    {
        for (int i = 0; i < 5; i++)
        {
            int sx = standAreaW + nowArea * obstacleAreaW + 1;
            int ex = sx + obstacleAreaW;
            int wx = Random.Range(sx, ex);
            int wy = Random.Range(0, h);
            for (; !availableIndex.Contains(GetIndex(wx, wy));)
            {
                wx = Random.Range(sx, ex);
                wy = Random.Range(0, h);
            }
            availableIndex.Remove(GetIndex(wx, wy));
            SetElement(GetIndex(wx, wy), Random.value < 0.5f ? ElementContent.SmallWall : ElementContent.BigWall);
        }
    }

    /// <summary>
    /// 道具を生成
    /// </summary>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void GenerateTool(List<int> availableIndex)
    {
        float trapProbability = Random.Range(minTrapProbability, maxTrapProbability);
        int trapNum = (int)(availableIndex.Count * trapProbability);
        for (int i = 0; i < 3; i++)
        {
            int tempIndex = availableIndex[Random.Range(0, availableIndex.Count)];  
            availableIndex.Remove(tempIndex);
            ToolElement t = (ToolElement)SetElement(tempIndex, ElementContent.Tool);
            t.toolType = (ToolType)Random.Range(0,9);
            if (t.isHide == false)
            {
                t.ConfirmSprite();
            }
        }
    }

    /// <summary>
    /// ゴールドを生成
    /// </summary>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void GenerateGold(List<int> availableIndex)
    {
      
        for (int i = 0; i < obstacleAreaNum*3; i++)
        {
            int tempIndex = availableIndex[Random.Range(0, availableIndex.Count)];
            availableIndex.Remove(tempIndex);
            GoldElement g = (GoldElement)SetElement(tempIndex, ElementContent.Gold);
            g.goldType = (GoldType)Random.Range(0, 7);
            if (g.isHide == false)
            {
                g.ConfirmSprite();
            }
        }
    }

    /// <summary>
    /// トラップを生成
    /// </summary>
    /// <param name="standAreaY">站立区的Y</param>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void GenerateTrap(int standAreaY, List<int> availableIndex)
    {
        float trapProbability = Random.Range(minTrapProbability, maxTrapProbability);
        int trapNum = (int)(availableIndex.Count * trapProbability);
        for (int i = 0; i < trapNum; i++)
        {
            int tempIndex = availableIndex[Random.Range(0, availableIndex.Count)];
            int x, y;
            GetPosition(tempIndex, out x, out y);
            if (x > 0 && x < standAreaW && y >= standAreaY - 1 && y <= standAreaY + 1) continue;
            {

            }
            availableIndex.Remove(tempIndex);
            SetElement(tempIndex, ElementContent.Trap);
        }
    }

    /// <summary>
    /// 数字を生成
    /// </summary>
    /// <param name="availableIndex">尚未初始化的地图元素的索引值列表</param>
    private void GenerateNumber(List<int> availableIndex)
    {
        foreach (int i in availableIndex)
        {
            SetElement(i, ElementContent.Number);
        }
        availableIndex.Clear();
    }

    private void GenerateStandArea(int y)
    {
        for (int i = 0; i < standAreaW; i++)
        {
            for (int j = y-1; j <= y+1; j++)
            {
                ((SinleCoveredElement)mapArray[i, j]).UnconveredElementSingle();
            }
        }
        player.transform.position = new Vector3(1, y, 0);
    }

    private BaseElement SetElement(int index, ElementContent content)
    {
        int x, y;
        GetPosition(index, out x, out y);
        GameObject tempgo = mapArray[x, y].gameObject;
        Destroy(tempgo.GetComponent<BaseElement>());
        switch (content)
        {
            case ElementContent.Number:
                mapArray[x, y] = tempgo.AddComponent<NumberElement>();
                break;
            case ElementContent.Trap:
                mapArray[x, y] = tempgo.AddComponent<TrapElement>();
                break;
            case ElementContent.Tool:
                mapArray[x, y] = tempgo.AddComponent<ToolElement>();
                break;
            case ElementContent.Gold:
                mapArray[x, y] = tempgo.AddComponent<GoldElement>();
                break;
            case ElementContent.Enemy:
                mapArray[x, y] = tempgo.AddComponent<EnemyElement>();
                break;
            case ElementContent.Door:
                mapArray[x, y] = tempgo.AddComponent<DoorElement>();
                break;
            case ElementContent.BigWall:
                mapArray[x, y] = tempgo.AddComponent<BigWallElement>();
                break;
            case ElementContent.SmallWall:
                mapArray[x, y] = tempgo.AddComponent<SmallWallElement>();
                break;
            case ElementContent.Exit:
                mapArray[x, y] = tempgo.AddComponent<ExitElement>();
                break;

        }
        return mapArray[x, y];
    }

    private void GetPosition(int index, out int x, out int y)
    {
        y = index / w;
        x = index - y * w;
    }
    private int GetIndex(int x, int y)
    {
        return w * y + x;
    }

    /// <summary>
    /// 计算指定位置元素的八领域陷阱个数
    /// </summary>
    /// <param name="x">元素所在位置的x</param>
    /// <param name="y">元素所在位置的y</param>
    /// <returns></returns>
    public int CountAdjacentTraps(int x, int y)
    {
        int count = 0;
        if (IsSameContent(x, y + 1, ElementContent.Trap)) count++;
        if (IsSameContent(x, y - 1, ElementContent.Trap)) count++;
        if (IsSameContent(x - 1, y, ElementContent.Trap)) count++;
        if (IsSameContent(x + 1, y, ElementContent.Trap)) count++;
        if (IsSameContent(x - 1, y + 1, ElementContent.Trap)) count++;
        if (IsSameContent(x + 1, y + 1, ElementContent.Trap)) count++;
        if (IsSameContent(x - 1, y - 1, ElementContent.Trap)) count++;
        if (IsSameContent(x + 1, y - 1, ElementContent.Trap)) count++;
        return count;

    }

    public bool IsSameContent(int x, int y, ElementContent content)
    {
        if (x >= 0 && x < w && y >= 0 && y < h)
        {
            return mapArray[x, y].elementContent == content;
        }
        return false;
    }
    /// <summary>
    ///泛洪算法翻开方法
    /// </summary>
    /// <param name="x">开始位置</param>
    /// <param name="y">开始位置</param>
    /// <param name="visited">访问表</param>
    public void FloodFillElement(int x, int y, bool[,] visited)
    {
        //检测XY边界
        //是否访问过
        //翻不翻开怎么翻开
        //将自己标记为访问过
        //让邻居一起做
        if (x >= 0 && x < w && y >= 0 && y < h)
        {
            if (visited[x, y]) return;
            if (mapArray[x, y].elementType != ElementType.CantCovered)
            {
                ((SinleCoveredElement)mapArray[x, y]).UnconveredElementSingle();
            }
            if (CountAdjacentTraps(x, y) > 0) return;
            if (mapArray[x, y].elementType == ElementType.CantCovered) return;
            visited[x, y] = true;
            FloodFillElement(x - 1, y, visited);
            FloodFillElement(x + 1, y, visited);
            FloodFillElement(x, y - 1, visited);
            FloodFillElement(x, y + 1, visited);
            FloodFillElement(x - 1, y - 1, visited);
            FloodFillElement(x + 1, y + 1, visited);
            FloodFillElement(x + 1, y - 1, visited);
            FloodFillElement(x - 1, y + 1, visited);
        }
    }

    public void UncoveredAdjacentElement(int x, int y)
    {
        int marked = 0;
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < w && j >= 0 && j < h)
                {
                    if (mapArray[i, j].elementState == ElementState.Marked) marked++;
                    if (mapArray[i, j].elementState == ElementState.Uncovered && mapArray[i, j].elementContent == ElementContent.Trap) marked++;
                    Debug.Log("markCount" + marked);
                }
            }
        }
        if (CountAdjacentTraps(x, y) == marked)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i >= 0 && i < w && j >= 0 && j < h)
                    {
                        if (mapArray[i, j].elementState != ElementState.Marked)
                        { mapArray[i, j].OnPlayerStand(); }

                    }
                }
            }
        }
    }
    /// <summary>
    /// 翻开所有地图陷阱
    /// </summary>
    public void DisplayAppTraps()
    {
        foreach (BaseElement element in mapArray)
        {
            if (element.elementContent == ElementContent.Trap)
            {
                ((TrapElement)element).UnconveredElementSingle();
            }
            if (element.elementContent != ElementContent.Trap && element.elementState == ElementState.Marked)
            {
                Instantiate(errorElement, element.transform);
            }
        }
    }
}