using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Singleton;

    private Player player = null;//本地玩家

    [SerializeField]
    private TextMeshProUGUI bulletsText;//注册用户的子弹文本框
    [SerializeField]
    private GameObject bulletsObject;

    private WeaponManager weaponManager;//获取到目前的子弹数量


    [SerializeField]
    private Transform healthBarFill;
    [SerializeField]
    private GameObject healthBarObject;

    private void Awake()
    {
        Singleton = this;
    }

    public void setPlayer(Player localPlayer)
    {
        player = localPlayer;//设置为本地玩家
        weaponManager = player.GetComponent<WeaponManager>();//获取
        bulletsObject.SetActive(true);
        healthBarObject.SetActive(true);

    }

    private void Update()//每一帧获取当前子弹数量
    {
        if (player == null) return;

        var currentWeapon = weaponManager.GetCurrentWeapon();
        if(currentWeapon.isReloading)
        {
            bulletsText.text = "Reloading...";
        }
        else
        {
            bulletsText.text = "Bullets:" + currentWeapon.bullets + "/" + currentWeapon.maxBullets;
        }

        healthBarFill.localScale = new Vector3(player.GetHealthy() / 100f, 1f, 1f);
    }
}
