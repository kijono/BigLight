using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public CinemachineVirtualCamera vCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        noise = vCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(shakeTime > 0){
            shakeTime -= Time.deltaTime;
            if(shakeTime <= 0){
                noise.enabled = false;
            }
        }
    }

    private float shakeTime = 0f;
    public void SetShake(float time){
        shakeTime = time;
        noise.enabled = true;
    }
}
