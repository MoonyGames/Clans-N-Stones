using UnityEngine;
using TMPro;

public class UpdateLevelText : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake() => text = GetComponent<TextMeshProUGUI>();

    private void Start() => text.text = "Level " + PlayerPrefs.GetInt("Level", 0).ToString();
}
