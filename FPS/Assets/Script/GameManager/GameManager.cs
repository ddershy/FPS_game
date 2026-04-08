using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    private Dictionary<string,Player> players = new Dictionary<string,Player>();//字典在每一个端口都有

    [SerializeField]
    public MatchingSettings matchingSettings;//创建需要保存的信息


    private void Awake()
    { 
        Singleton = this;
    }

    public void RegisterPlayer(string name,Player player)//将玩家加入游戏对象
    {
        player.transform.name = name;
        players.Add(name, player);
    }

    public void UnRegisterPlayer(string name)//删除
    {
        players.Remove(name);
    }

    public Player GetPlayer(string name)//通过名字返回玩家
    {
        return players[name];
    }


    /*private void OnGUI()//每一帧至少调用一次
    {
        GUILayout.BeginArea(new Rect(200f, 200f, 200f, 400f));
        GUILayout.BeginVertical();
        GUI.color = Color.red;
        foreach (string name in players.Keys)//遍历字典里所有的东西 相当于for auto t:arr
        {
            Player player = GetPlayer(name);
            GUILayout.Label(name + "-" + player.GetHealthy());
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }*/
}
