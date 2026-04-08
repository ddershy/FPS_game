using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerName;
    [SerializeField]
    private Transform playerHealth;
    [SerializeField]
    private Transform infoUI;//获取信息

    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        playerName.text = transform.name;
        playerHealth.localScale = new Vector3(player.GetHealthy() / 100f, 1f, 1f);

        var camera = Camera.main;//每一帧 信息朝向摄像头
        infoUI.LookAt(transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.up);
        infoUI.Rotate(new Vector3(0f, 180f, 0f));//手动翻转180°
    }
}
