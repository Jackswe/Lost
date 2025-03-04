using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public CinemachineVirtualCamera cm;

    [Header("相机距离信息")]
    public float defaultCameraLensSize;
    public float targetCameraLensSize;
    public float cameraLensSizeChangeSpeed;

    [Header("相机垂直位置信息")]
    public float defaultCameraYPosition;
    public float targetCameraYPosition;
    public float cameraYPositionChangeSpeed;

    //public float defaultCameraXPosition;
    //public float targetCameraXPositionOffset;
    //public float cameraXPositionChangeSpeed;

    private Player player;
    public CinemachineFramingTransposer ft { get; set; }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ft = cm.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Start()
    {
        player = PlayerManager.instance.player;
    }


    // 如果玩家在可下落平台 则拉远相机距离
    public void CameraMovementOnDownablePlatform()
    {
        if (player.isOnPlatform)
        {
            if (cm.m_Lens.OrthographicSize < targetCameraLensSize)
            {
                cm.m_Lens.OrthographicSize = Mathf.Lerp(cm.m_Lens.OrthographicSize, targetCameraLensSize, cameraLensSizeChangeSpeed * Time.deltaTime);

                if (cm.m_Lens.OrthographicSize >= targetCameraLensSize - 0.01f)
                {
                    cm.m_Lens.OrthographicSize = targetCameraLensSize;
                }
            }

            if (ft.m_ScreenY > targetCameraYPosition)
            {
                ft.m_ScreenY = Mathf.Lerp(ft.m_ScreenY, targetCameraYPosition, cameraYPositionChangeSpeed * Time.deltaTime);

                if (ft.m_ScreenY >= targetCameraYPosition + 0.01f)
                {
                    ft.m_ScreenY = targetCameraYPosition;
                }
            }
        }
        else
        {
            if (player.isNearPit)
            {
                return;
            }

            if (cm.m_Lens.OrthographicSize > defaultCameraLensSize)
            {
                cm.m_Lens.OrthographicSize = Mathf.Lerp(cm.m_Lens.OrthographicSize, defaultCameraLensSize, cameraLensSizeChangeSpeed * Time.deltaTime);

                if (cm.m_Lens.OrthographicSize <= defaultCameraLensSize + 0.01f)
                {
                    cm.m_Lens.OrthographicSize = defaultCameraLensSize;
                }
            }

            if (ft.m_ScreenY < defaultCameraYPosition)
            {
                ft.m_ScreenY = Mathf.Lerp(ft.m_ScreenY, defaultCameraYPosition, cameraYPositionChangeSpeed * Time.deltaTime);

                if (ft.m_ScreenY <= targetCameraYPosition - 0.01f)
                {
                    ft.m_ScreenY = targetCameraYPosition;
                }
            }
        }
    }
}
