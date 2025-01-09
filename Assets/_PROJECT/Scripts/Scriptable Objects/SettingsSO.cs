using UnityEngine;

[CreateAssetMenu(fileName = "SettingsSO", menuName = "Scriptable Objects/SettingsSO")]
public class SettingsSO : ScriptableObject
{
    public bool IsSet;
    public float MouseSensitivity;
    public int QualityIndex;
    public int ResolutionIndex;
    public bool IsFullscreen;
    public float Brightness;
}
