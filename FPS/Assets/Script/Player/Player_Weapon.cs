using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]//串行化
public class Player_Weapon 
{
    public string gunname = "M110";
    public int damage = 10;
    public float range = 100f;

    public float shootRate = 0f;//一秒可以发多少子弹，如果<=0说明单发，其余为连发
    public float shootCoolDownTime = 0.75f;//单发模式的冷却时间
    public float recoilForce = 2f;//后坐力

    public int maxBullets = 30;//子弹个数
    public int bullets = 30;
    public float reloadTime = 2f;

    [HideInInspector]//公开变量展示出来
    public bool isReloading = false;

    public GameObject graphics;//引用图像
}
