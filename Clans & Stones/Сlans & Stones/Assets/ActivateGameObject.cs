using UnityEngine;

public class ActivateGameObject : MonoBehaviour
{
    public void Activate(GameObject gameObject) => gameObject.SetActive(true);
}
