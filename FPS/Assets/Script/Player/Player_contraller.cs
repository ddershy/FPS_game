using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.Netcode;
using UnityEngine;

public class Player_contraller : NetworkBehaviour //改变物理属性
{
    [SerializeField]
    private Rigidbody rb;//一个刚体
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;//默认是0 存放每秒钟移动的距离
    private Vector3 yRotation = Vector3.zero;//控制刚体
    private Vector3 xRotation = Vector3.zero;//旋转摄像机
    private float recoilForce = 0f;//后坐力

    private float cameraRotationTotal = 0f;//统计一共转了多少度
    [SerializeField]
    private float cameraRotationLimit = 85f;//旋转上限

    private Vector3 thurseForce = Vector3.zero;//向上的力

    private float eps = 0.01f;//精度
    private Vector3 lastFramePosition = Vector3.zero;//上一帧的位置
    private Animator animator;

    private float disToGround = 0f;

    private void Start()
    {
        lastFramePosition = transform.position;
        animator = GetComponentInChildren<Animator>();

        disToGround = GetComponent<Collider>().bounds.extents.y;//碰撞检测的物体中心点坐标距离下边界的长度
    }

    public void Move(Vector3 _velocity)//获取用户的速度
    {
        velocity = _velocity;//重新赋值
    }
    public void Rorate(Vector3 _yRotation,Vector3 _xRotaion)
    {
        yRotation = _yRotation;
        xRotation = _xRotaion;
    }
    public void Thurse(Vector3 _thurseForce)//获取这个力
    {
        thurseForce = _thurseForce;
    }

    public void AddRecoilForce(float newRecoilForce)
    {
        recoilForce += newRecoilForce;//后坐力累计
    }

    private void PerformerMovement()//辅助移动函数
    {
        if (velocity != Vector3.zero)//如果不是0 水平移动
        {
            //Time.fixedDeltaTime: 表示FixedUpdate距离上一次使用间隔的时间
            //Time.deltaTime:表示Update距离上一次调用间隔的时间
            rb.MovePosition(rb.position+velocity*Time.fixedDeltaTime);//向着v的方向移动 
        } 
        if(thurseForce != Vector3.zero)//是否有向上的力
        {
            //rb.AddForce(thurseForce) =>给刚体作用一个向上的力，作用时间为Time.fixedDeltaTime
            rb.AddForce(thurseForce);
            thurseForce = Vector3.zero;//每一帧都清0，因为下一帧会更新
        }
    }

    private void PerformRotation()
    {
        if(recoilForce < 0.1)//停止条件
        {
            recoilForce = 0f;
        }
        if (yRotation != Vector3.zero || recoilForce >0)
        {
            rb.transform.Rotate(yRotation + rb.transform.up * Random.Range(-2f * recoilForce, 2f * recoilForce));
        }

        if(xRotation!=Vector3.zero || recoilForce >0)//限制旋转角度
        {
            cameraRotationTotal += xRotation.x - recoilForce;
            cameraRotationTotal = Mathf.Clamp(cameraRotationTotal, -cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(cameraRotationTotal, 0, 0);
        }

        recoilForce *= 0.5f;//实现先快后慢
    }

    private void PerformAnimation()
    {
        Vector3 deltaPosition = transform.position - lastFramePosition;
        lastFramePosition = transform.position;

        float forward = Vector3.Dot(deltaPosition, transform.forward);
        float right = Vector3.Dot(deltaPosition, transform.right);

        int direction = 0;//静止
        if(forward >eps)
        {
            direction = 1;//前进
        }
        else if(forward < -eps)
        {
            if(right > eps)
            {
                direction = 4;//右后
            }
            else if(right < -eps)
            {
                direction = 6;//左后
            }
            else
            {
                direction = 5;//正后
            }
        }
        else if(right > eps)
        {
            direction = 3;
        }
        else if(right < - eps)
        {
            direction = 7;
        }

        //双脚离地为跳跃
        if(!Physics.Raycast(transform.position, -Vector3.up, disToGround + 0.1f))
        {
            direction = 8;
        }

        if(GetComponent<Player>().IsDead())
        {
            direction = -1;
        }
        animator.SetInteger("direction", direction);
    }

    private void FixedUpdate()//如果是作用于物理需要使用`FixedUpdate`更新坐标
    {
        if (IsLocalPlayer)
        {
            PerformerMovement();
            PerformRotation();
        }
        if (IsLocalPlayer)
        {
            PerformAnimation(); //不管是不是本地玩家都需要播放动画
        }
    }

    private void Update()
    {
        if (!IsLocalPlayer)
        {
            PerformAnimation(); //不管是不是本地玩家都需要播放动画
        }
    }
    /*
     FixedUpdate和Update的区别
    1. FixedUpdate:50次/s，相隔约0.02s执行一次，一定保证每秒均匀执行50次
    2. Update: 每秒执行的次数也是定值，但是相隔执行时间不一定相同，是不均匀的
     */

}
