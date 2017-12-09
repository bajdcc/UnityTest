using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    public int point_count = 10; // 个数
    public int min_adj_num = 3; // 最小连接数
    public float min_dist = 2f; // 方块间最小距离

    List<GameObject> cubes = new List<GameObject>();
    List<GameObject> lines = new List<GameObject>();
    List<GameObject> txts = new List<GameObject>();
    List<Vector3> pts = new List<Vector3>(); // 点坐标
    int[,] adj; // 邻接矩阵
    string info = "距离信息";

    // Use this for initialization
    void Start () {
        direction = Quaternion.FromToRotation(new Vector3(0, 1, 0), Normal);
    }

    Vector3 Normal = new Vector3(0, 1, 0);
    Quaternion direction;
    // Update is called once per frame
    void Update () {
        foreach (var o in txts)
        {
            o.transform.rotation = Camera.main.transform.rotation * direction;
        }
    }
    void OnGUI()
    {
        GUI.Box(new Rect(20, 20, 200, 140), "最小生成树");

        GUI.Box(new Rect(20, 170, 200, 30 * point_count + 10), "信息");

        GUI.Label(new Rect(30, 190, 190, 30 * point_count), info);

        if (GUI.Button(new Rect(30, 50, 80, 40), "初始化"))
        {
            init();
        }

        if (GUI.Button(new Rect(120, 50, 80, 40), "生成"))
        {
            if (cubes.Count == 0)
            {
                init();
            }
            build_mst();
        }

        if (GUI.Button(new Rect(30, 100, 80, 40), "退出"))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    void init()
    {
        if (cubes.Count != 0)
        {
            foreach (var o in cubes)//清除物体
            {
                Destroy(o);
            }
            cubes.Clear();
        }
        if (lines.Count != 0)
        {
            foreach (var o in lines)//清除物体
            {
                Destroy(o);
            }
            lines.Clear();
        }
        if (txts.Count != 0)
        {
            foreach (var o in txts)//清除物体
            {
                Destroy(o);
            }
            txts.Clear();
        }

        pts.Clear();
        adj = new int[point_count, point_count];

        for (int i = 0; i < point_count; i++)
        {
            // 随机位置
            while (true)
            {
                var pos = new Vector3(Random.Range(-6f, 6f), Random.Range(1f, 6f), Random.Range(-6f, 6f));
                int j;
                for (j = 0; j < i; j++) //判断是否重叠
                {
                    if (Vector3.Distance(pts[j], pos) < min_dist)
                    {
                        break;
                    }
                }
                if (j == i)//没有重叠
                {
                    pts.Add(pos);
                    break;
                }
            }
        }
        for (int i = 0; i < point_count - 1; i++)
        {
            var cnt = 0;
            for (int j = i + 1; j < point_count; j++)
            {
                if (i == j) continue;
                if (Random.Range(0.0f, 1.0f) > 0.7f)
                {
                    cnt++;
                    adj[i, j] = 1; // 相连
                    adj[j, i] = 1;
                }
                else
                {
                    adj[i, j] = 0;
                    adj[j, i] = 0;
                }
            }
            for (int j = cnt; j < min_adj_num;)
            {
                var id = (int)(System.Math.Floor(Random.Range(0.0f, 1.0f * point_count - 0.01f)));
                if (adj[i, id] != 1)
                {
                    adj[i, id] = 1;
                    adj[id, i] = 1;
                    j++;
                }
            }
        }

        for (int i = 0; i < point_count; i++)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Cube#" + i;
            cubes.Add(cube);
            var text = new GameObject("Text");
            txts.Add(text);
            text.name = "Text#" + i;
            var txtmesh = text.AddComponent<TextMesh>();
            txtmesh.text = "" + i;
            txtmesh.fontSize = 100;
            txtmesh.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            txtmesh.color = Color.white;
            cube.transform.localScale = new Vector3(1f, 1f, 1f);//cube的大小，自己设
            cube.transform.position = pts[i];
            text.transform.SetParent(cube.transform, false);
            var material = new Material(Shader.Find("Legacy Shaders/Diffuse")); // 注意坑，不是所有shader默认都打包的，在Graphics Settings里面
            material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            cube.GetComponent<MeshRenderer>().material = material;
        }
        for (int i = 0; i < point_count - 1; i++)
        {
            for (int j = i + 1; j < point_count; j++)
            {
                if (adj[i, j] == 1)
                {
                    //画线
                    var obj = new GameObject("Line");
                    obj.name = "Line#" + i + "_" + j;
                    lines.Add(obj);
                    var line = obj.AddComponent<LineRenderer>();
                    //只有设置了材质 setColor才有作用
                    line.material = new Material(Shader.Find("Particles/Additive"));
                    line.positionCount = 2;//设置两点
                    line.startColor = Color.yellow;
                    line.endColor = Color.red; //设置直线颜色
                    line.startWidth = 0.01f;
                    line.endWidth = 0.01f;//设置直线宽度

                    //设置指示线的起点和终点
                    line.SetPosition(0, pts[i]);
                    line.SetPosition(1, pts[j]);
                }
            }
        }
    }

    void build_mst()
    {
        float[,] dist = new float[point_count, point_count]; // 点与点距离
        for (int i = 0; i < point_count; i++)
        {
            for (int j = i; j < point_count; j++)
            {
                if (i == j) dist[i, j] = float.MaxValue;
                else dist[i, j] = dist[j, i] = (adj[i, j] == 1) ? Vector3.Distance(pts[i], pts[j]) : float.MaxValue;
            }
        }

        var lowcost = new float[point_count]; //记录Vnew中每个点到V中邻接点的最短边
        var addvnew = new bool[point_count]; //标记某点是否加入Vnew
        var adjecent = new int[point_count]; //记录V中与Vnew最邻近的点

        var start = 0;

        for (int i = 0; i < point_count; i++)
        {
            lowcost[i] = -1;
            addvnew[i] = false;//将所有点至于Vnew之外
            adjecent[i] = start;
        }

        for (int i = 1; i < point_count; i++)//顶点是从1开始
        {
            lowcost[i] = dist[start, i];
        }

        addvnew[start] = true;
        adjecent[start] = start;
        var sumweight = 0f;

        if (lines.Count != 0)
        {
            foreach (var o in lines)//清除物体
            {
                Destroy(o);
            }
            lines.Clear();
        }

        var infos = new List<string>();

        for (int i = 0; i < point_count - 1; i++)
        {
            var min = float.MaxValue;
            var v = -1;
            for (int j = 0; j < point_count; j++)
            {
                if (!addvnew[j] && lowcost[j] < min)//在Vnew之外寻找最短路径
                {
                    min = lowcost[j];
                    v = j;
                }
            }
            if (v != -1)
            {
                Debug.Log(string.Format("Distance({3}): {0} -> {1} = {2}\n", adjecent[v], v, lowcost[v], i));
                infos.Add(string.Format("{3}. {0}-{1}={2}\n", adjecent[v], v, lowcost[v], i + 1));
                addvnew[v] = true; //将v加Vnew中

                {
                    var src = adjecent[v];
                    var dst = v;

                    //画线
                    var obj = new GameObject("Line");
                    obj.name = "Line#" + src + "_" + dst;
                    lines.Add(obj);
                    var line = obj.AddComponent<LineRenderer>();
                    //只有设置了材质 setColor才有作用
                    line.material = new Material(Shader.Find("Particles/Additive"));
                    line.positionCount = 2;//设置两点
                    line.startColor = Color.blue;
                    line.endColor = Color.cyan; //设置直线颜色
                    line.startWidth = 0.05f;
                    line.endWidth = 0.05f;//设置直线宽度

                    //设置指示线的起点和终点
                    line.SetPosition(0, pts[src]);
                    line.SetPosition(1, pts[dst]);
                }

                sumweight += lowcost[v];//计算路径长度之和
                for (int j = 0; j < point_count; j++)//更新路径
                {
                    if (!addvnew[j] && dist[v, j] < lowcost[j])
                    {
                        lowcost[j] = dist[v, j];//此时v点加入Vnew 需要更新lowcost
                        adjecent[j] = v;
                    }
                }
            }
        }

        info = string.Join("\r\n", infos.ToArray());
    }
}
