using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private Player_Weapon primaryWeapon;//存主武器

    [SerializeField]
    private Player_Weapon secondaryWeapon;

    [SerializeField]//添加权限
    private GameObject weaponHolder;

    private Player_Weapon currentWeapon;//存当前武器
    private WeaponGraphic currentGraphic;//当前武器特效
    private AudioSource currentAudioSource;//当前音效

    // Start is called before the first frame update
    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    public void EquipWeapon(Player_Weapon weapon)
    {
        if(weaponHolder.transform.childCount >0)//检查换武器之前有没有别的武器
        {
            DestroyImmediate(weaponHolder.transform.GetChild(0).gameObject);//如果有装备的武器就把之前的武器删掉
        }
        currentWeapon = weapon;

        GameObject weaponObject = Instantiate(currentWeapon.graphics,weaponHolder.transform.position,weaponHolder.transform.rotation);//实例化一个对象
        weaponObject.transform.SetParent(weaponHolder.transform);//生成并挂上

        currentGraphic = weaponObject.GetComponent<WeaponGraphic>();//获得
        currentAudioSource = weaponObject.GetComponent<AudioSource>();

        if(IsLocalPlayer)
        {
            currentAudioSource.spatialBlend = 0f;//如果是自己开枪就在中心，敌人才感知方位
        }
    }

    public Player_Weapon GetCurrentWeapon()//获取武器接口
    {
        return currentWeapon;
    }

    public AudioSource GetCurrentAudioSource()
    {
        return currentAudioSource;
    }

    public WeaponGraphic GetCurrentGraphic()//获取当前特效
    {
        return currentGraphic;
    }
    private void ToggleWeapon()//交换武器
    {
        if (currentWeapon == primaryWeapon)//如果是主武器
        {
            EquipWeapon(secondaryWeapon);
        }
        else
        {
            EquipWeapon(primaryWeapon); 
        }
    }

    [ClientRpc]
    private void ToggleWeaponClientRpc()
    {
        ToggleWeapon();//每个客户端切好枪
    }

    [ServerRpc]
    private void ToggleWeaponServerRpc()
    {
        if(!IsHost)
        {
            ToggleWeapon();//自己且好枪
        }
        ToggleWeaponClientRpc();//调用每个客户端的窗口
    }

    // Update is called once per frame
    void Update()//切换武器
    {
        if (IsLocalPlayer)//每个窗口只能操作本地玩家
        {
            if (Input.GetKeyDown(KeyCode.Q))//如果点击了'Q'
            {
                ToggleWeaponServerRpc();//收到输入
            }
        }
    }

    public void reload(Player_Weapon player_Weapon)//换弹
    {
        if (player_Weapon.isReloading) return;//正在装弹就退出
        player_Weapon.isReloading = true;//开始装

        StartCoroutine(ReloadCoroutine(player_Weapon));
    }

    private IEnumerator ReloadCoroutine(Player_Weapon player_Weapon)
    {
        yield return new WaitForSeconds(player_Weapon.reloadTime);

        player_Weapon.bullets = player_Weapon.maxBullets;
        player_Weapon.isReloading = false;//结束
    }
}
