using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_input : MonoBehaviour
{
    [SerializeField] //可以在unity中调试
    private float speed = 3f;//调试用速度
    [SerializeField]
    private float lookSensativity = 3.2f;//鼠标灵敏度
    [SerializeField]
    Player_contraller contraller;
    [SerializeField]
    private float thurseForce = 20f;//推力

    private float disToGround = 0f;//记录人物中心点到脚的距离，用于判断是否能继续跳跃

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;//鼠标锁定，鼠标消失
        disToGround = GetComponent<Collider>().bounds.extents.y;//碰撞检测的物体中心点坐标距离下边界的长度
    }

    // Update is called once per frame
    void Update()
    {
        float xMov = Input.GetAxisRaw("Horizontal");//横向距离
        float yMov = Input.GetAxisRaw("Vertical");//纵向

        /*Vector3 包含三维的向量
         * transform.right * xMov 向量×距离
         * .normalized;//把向量的模长统一成1
        */
        Vector3 velocity = (transform.right * xMov + transform.forward * yMov).normalized * speed;
        contraller.Move(velocity);


        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");

        Vector3 yRotation = new Vector3(0f,xMouse,0f) * lookSensativity;
        Vector3 xRotation = new Vector3(-yMouse, 0f, 0f) * lookSensativity;
        contraller.Rorate(yRotation,xRotation);

        if (Input.GetButton("Jump")) //获取的是一直按住空格键
        {
            if(Physics.Raycast(transform.position,-Vector3.up,disToGround + 0.1f))//如果碰撞检测成功，可以跳跃
            {
                Vector3 force = Vector3.up * thurseForce;
                contraller.Thurse(force);
            }
        }

    }
}
