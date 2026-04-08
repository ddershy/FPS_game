using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private int maxHealthy = 100;
    [SerializeField]
    private Behaviour[] componentsToDisable;//记录去世的组件
    private bool[] componentsEnabled;//记录组件初始状态
    private bool colliderEnable;//不是继承自Behavior，需要特判

    private NetworkVariable<int> currentHealthy = new NetworkVariable<int>();
    private NetworkVariable<bool> isDead = new NetworkVariable<bool>();//判断角色是否存活

    public void Setup()
    {
        componentsEnabled = new bool[componentsToDisable.Length];//长度是组件个数
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsEnabled[i] = componentsToDisable[i].enabled;
        }

        Collider collider = GetComponent<Collider>();
        colliderEnable = collider.enabled;

        SetDefault();
    }

    private void SetDefault()//重生
    {
        for (int i = 0; i < componentsToDisable.Length; i++)//恢复
        {
            componentsToDisable[i].enabled = componentsEnabled[i];
        }
        Collider collider = GetComponent<Collider>();
        collider.enabled = colliderEnable;

        if (IsServer)//只在服务器端有效
        {
            currentHealthy.Value = maxHealthy;
            isDead.Value = false;
        }
    }

    public bool IsDead()
    {
        return isDead.Value;
    }
    public void TakeDamage(int damage)//受到伤害,只会在服务器端调用
    {
        if (isDead.Value) return;//死亡不管它
        currentHealthy.Value -= damage;
        if (currentHealthy.Value <= 0)
        {
            currentHealthy.Value = 0;
            isDead.Value = true;

            if (!IsHost)
            {
                DieOnServer();//先在服务器端调用
            }
            DieClientRpc();//每个客户端执行一遍
        }
    }

    public int GetHealthy()
    {
        return currentHealthy.Value;
    }
    
    private IEnumerator Respan()//重生计时
    {
        yield return new WaitForSeconds(GameManager.Singleton.matchingSettings.respanTime);//sleep3s后重生

        SetDefault();
        GetComponentInChildren<Animator>().SetInteger("direction", 0);
        GetComponent<Rigidbody>().useGravity = true;//重力恢复

        if (IsLocalPlayer)//角色移动是靠Client移动的，所以需要判断是本地玩家才可以移动
            transform.position = new Vector3(0f, 20f, 0f);//从天而降
    }

    private void DieOnServer()
    {
        Die();
    }

    [ClientRpc]//只在客户端执行,在Server执行每个Client
    private void DieClientRpc()
    {
        Die();
    }

    private void Die()//取消三个组件和碰撞检测
    {
        GetComponent<Player_Shotting>().StopShooting();

        GetComponentInChildren<Animator>().SetInteger("direction", -1);
        GetComponent<Rigidbody>().useGravity = false;//重力小时
        for (int i = 0; i < componentsToDisable.Length; i++)
            componentsToDisable[i].enabled = false;
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        StartCoroutine(Respan());//调用异步函数，新的线程
    }
}
