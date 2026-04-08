using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player_Setup : NetworkBehaviour
{
    [SerializeField]
    private Behaviour[] componentToDisable;//这里拖放的是自己的组件，所以需要在组件右侧拖拽

    private Camera sceneCamera;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsLocalPlayer)//非本地玩家
        {
            SetLayerMaskForAllChildren(transform, LayerMask.NameToLayer("Remote Player"));
            DisableComponent();
        }
        else
        {
            PlayerUI.Singleton.setPlayer(GetComponent<Player>());///取出本地玩家
            SetLayerMaskForAllChildren(transform, LayerMask.NameToLayer("Player"));
            closeSceneCamera();
        }

        string name = "Player" + GetComponent<NetworkObject>().NetworkObjectId.ToString();//获取姓名
        Player player = GetComponent<Player>();//角色对象

        player.Setup();

        //这一步在写字典，所以名字需要靠自己找
        GameManager.Singleton.RegisterPlayer(name, player);//使用Singleton模式可以不用将函数修改成static
        //一旦加入就修改
    }

    private void SetLayerMaskForAllChildren(Transform transform,LayerMask layerMask)
    {
        transform.gameObject.layer = layerMask;
        for(int i = 0; i < transform.childCount;i++)
        {
            SetLayerMaskForAllChildren(transform.GetChild(i), layerMask);//递归修改所有的
        }
    }

    private void DisableComponent()
    {
        for (int i = 0; i < componentToDisable.Length; i++)
        {
            componentToDisable[i].enabled = false;//组件禁用
        }
    }
    private void closeSceneCamera()
    {
        sceneCamera = Camera.main;
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(false);//非活跃态
        }
    }

    public override void OnNetworkDespawn()//消失
    {
        base.OnNetworkDespawn();
        if (sceneCamera != null)
            sceneCamera.gameObject.SetActive(true);
        GameManager.Singleton.UnRegisterPlayer(transform.name);//这里只需要传名字是因为在注册的时候已经赋值，这里直接调用字典
    }
}
