using System;
using Unity.Cinemachine;
using UnityEngine;

public enum ECameraType
{
    Main,
    POSMachine,
    Computer
}

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CinemachineCamera posMachineCamera;
    [SerializeField] private CinemachineCamera computerCamera;
    [SerializeField] private GameObject reticleCanvasGO;

    public static event Action OnCameraChangedToMain;
    public static event Action OnCameraChangedFromMain;

    public void PrioritizeCamera(ECameraType type)
    {
        if(type == ECameraType.Main)
        {
            posMachineCamera.Priority = 10;
            computerCamera.Priority = 10;
            mainCamera.Priority = 11;

            reticleCanvasGO.SetActive(true);

            OnCameraChangedToMain?.Invoke();
        }
        if(type == ECameraType.POSMachine)
        {
            mainCamera.Priority = 10;
            computerCamera.Priority = 10;
            posMachineCamera.Priority = 11;

            reticleCanvasGO.SetActive(false);

            OnCameraChangedFromMain?.Invoke();
        }
        if(type == ECameraType.Computer)
        {
            mainCamera.Priority = 10;
            posMachineCamera.Priority = 10;
            computerCamera.Priority = 11;

            reticleCanvasGO.SetActive(false);

            OnCameraChangedFromMain?.Invoke();
        }
    }

    public void PrioritizeCamera(ECameraType cameraType, GameObject cameraGO)
    {
        switch(cameraType)
        {
            case ECameraType.Main:
                break;
            case ECameraType.POSMachine:
                posMachineCamera = cameraGO.GetComponent<CinemachineCamera>();
                break;
            case ECameraType.Computer:
                computerCamera = cameraGO.GetComponent<CinemachineCamera>();
                break;
        }

        PrioritizeCamera(cameraType);
    }
}
