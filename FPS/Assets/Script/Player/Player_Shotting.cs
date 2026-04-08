using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player_Shotting : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";//记录玩家标签

    private WeaponManager weaponManager;//获得当前武器
    private Player_Weapon currentWeapon;

    private float shootCoolDownTime = 0f;//距离上一次开枪距离的时间
    private int autoShootCount = 0;//当前一共连开多少枪

    [SerializeField]
    private LayerMask mask;

    private Camera cam;
    private Player_contraller player_Contraller;
    enum HitEffiectMaterial
    {
        Metal,
        Stone,
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();//返回一个摄像机，如果有多个则会返回多个摄像机
        //因为在子元素中所以用InChildren
        weaponManager = GetComponent<WeaponManager>();
        player_Contraller = GetComponent<Player_contraller>();
    }

    // Update is called once per frame
    void Update()
    {
        shootCoolDownTime += Time.deltaTime;//统计两发子弹间隔时间

        if (!IsLocalPlayer) return;//只有本地玩家可以修改

        currentWeapon = weaponManager.GetCurrentWeapon();//获取当前武器

        if(Input.GetKeyDown(KeyCode.R))//手动装弹
        {
            weaponManager.reload(currentWeapon);
            return;
        }

        if (currentWeapon.shootRate <= 0)//单发
        {
            if (Input.GetButtonDown("Fire1") && shootCoolDownTime >= currentWeapon.shootCoolDownTime)//如果点击左键则发射&&大于等于间隔时间
            {
                autoShootCount = 0;
                Shot();
                shootCoolDownTime = 0f;//充值冷却时间
            }
        }
        else//连发
        {
            if(Input.GetButtonDown("Fire1"))//按下鼠标的时刻开始循环
            {
                autoShootCount = 0;
                InvokeRepeating("Shot",0f,1f/currentWeapon.shootRate);//周期性触发("函数名",开始时间,循环频率)
            }
            else if(Input.GetButtonUp("Fire1") || Input.GetKeyDown(KeyCode.Q)) //切枪或结束射击
            {
                autoShootCount = 0;
                CancelInvoke("Shot");//结束
            }
        }
    }

    public void StopShooting()
    {
        CancelInvoke("Shot");//结束
    }

    private void OnHit(Vector3 pos,Vector3 normal, HitEffiectMaterial material)//击中点特效
    {
        GameObject hitEffectPerfab;
        if(material == HitEffiectMaterial.Metal)
        {
            hitEffectPerfab = weaponManager.GetCurrentGraphic().metalHitEffectPerfeb;
        }
        else
        {
            hitEffectPerfab = weaponManager.GetCurrentGraphic().stoneHitEffectPerfab;
        }

        GameObject hitEffectObject = Instantiate(hitEffectPerfab, pos, Quaternion.LookRotation(normal));//看的方向
        ParticleSystem particleSystem = hitEffectObject.GetComponent<ParticleSystem>();
        particleSystem.Emit(1);
        particleSystem.Play();
        Destroy(hitEffectObject, 1f);//1s后删除
    }
    [ClientRpc]
    private void OnHitClientRpc(Vector3 pos, Vector3 normal, HitEffiectMaterial material)
    {
        OnHit(pos, normal, material);
    }

    [ServerRpc]
    private void OnHitServerRpc(Vector3 pos, Vector3 normal, HitEffiectMaterial material)
    {
        if(!IsHost)
        {
            OnHit(pos, normal, material);
        }
        OnHitClientRpc(pos, normal, material);
    }

    private void OnShoot(float recoilForce)//每次设计相关的逻辑包括特效声音等
    {
        //动态播放一次特效
        weaponManager.GetCurrentGraphic().muzzleFlash.Play();
        weaponManager.GetCurrentAudioSource().Play();//音效

        if(IsLocalPlayer)//只有本地玩家收到后坐力作用
        {
            player_Contraller.AddRecoilForce(recoilForce);
        }
    }

    [ClientRpc]
    private void OnShootClientRpc(float recoilForce)
    {
        OnShoot(recoilForce);//本地
    }

    [ServerRpc]
    private void OnShootServerRpc(float recoilForce)
    {
        if (!IsHost)
        {
            OnShoot(recoilForce);//本地
        }
        OnShootClientRpc(recoilForce);//客户端 实则是个循环
    }

    private void Shot()
    {
        if(currentWeapon.bullets <= 0 || currentWeapon.isReloading) return;

        currentWeapon.bullets--;

        if(currentWeapon .bullets<=0)
        {
            weaponManager.reload(currentWeapon);
        }

        autoShootCount++;
        float recoilForce = currentWeapon.recoilForce;

        if(autoShootCount <=3 && currentWeapon.gunname!= "M110")//狙一直有后坐力
        {
            recoilForce *= 0.2f;
        }
        OnShootServerRpc(recoilForce);//不管有没有击中  一定有声音和画面

        RaycastHit hit;//击中物体
        //Unity自带的一个类，原理是返回第一个集中的带碰撞检测的物体，函数得到的是bool
        if(Physics.Raycast(cam.transform.position,cam.transform.forward,out hit,currentWeapon.range,mask))
        {
            if(hit.collider.tag == PLAYER_TAG)
            {
                ShotServerRpc(hit.collider.name, currentWeapon.damage);//传递玩家的姓名和武器的攻击力
                OnHitServerRpc(hit.point, hit.normal, HitEffiectMaterial.Metal);
            }
            else
            {
                OnHitServerRpc(hit.point, hit.normal, HitEffiectMaterial.Stone);
            }
        }
    }

    [ServerRpc]//一定要以ServerRpc为后缀&要将MonoBehavior换成NetworkBehaviour
    private void ShotServerRpc(string name,int damage)//被击中物体的名称
    {
        Player player = GameManager.Singleton.GetPlayer(name);
        player.TakeDamage(damage);
    }
}
