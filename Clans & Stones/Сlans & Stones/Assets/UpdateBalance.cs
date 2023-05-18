using UnityEngine;
using TMPro;

public class UpdateBalance : MonoBehaviour
{
    public static UpdateBalance Singleton { get; private set; }

    private TextMeshProUGUI text;

    private int currentBalance;

    private void Awake()
    {
        if (Singleton == null)
            Singleton = this;
        else
            Destroy(this);

        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        currentBalance = PlayerPrefs.GetInt("Balance", 0);

        text.text = currentBalance.ToString();
    }

    public void ChangeBalance(int amount)
    {
        currentBalance += amount;

        PlayerPrefs.SetInt("Balance", currentBalance);

        text.text = currentBalance.ToString();
    }
}
