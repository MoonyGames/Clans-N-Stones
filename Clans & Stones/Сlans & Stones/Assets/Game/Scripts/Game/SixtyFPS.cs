using UnityEngine;

public class SixtyFPS : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
