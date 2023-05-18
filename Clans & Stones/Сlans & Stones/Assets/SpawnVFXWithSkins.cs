using UnityEngine;

public class SpawnVFXWithSkins : MonoBehaviour
{
    [SerializeField] private Transform _VFXLeftPosition, _VFXRightPosition;

    [SerializeField] private GameObject[] _VFXSkins;

    private void Start()
    {
        GameObject leftVFX = Instantiate(_VFXSkins[PlayerPrefs.GetInt("CurrentSkinIndex", 0)], _VFXLeftPosition.position, _VFXLeftPosition.rotation);
        GameObject rightVFX = Instantiate(_VFXSkins[PlayerPrefs.GetInt("CurrentSkinIndex", 0)], _VFXRightPosition.position, _VFXRightPosition.rotation);

        leftVFX.SetActive(true);
        rightVFX.SetActive(true);
    }
}
