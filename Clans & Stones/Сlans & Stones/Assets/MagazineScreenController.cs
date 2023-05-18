using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MagazineScreenController : MonoBehaviour
{
    [SerializeField] private GameObject[] _skinsGameObjects;

    [SerializeField] private TextMeshProUGUI _coinsText, _selectOrBuyText;

    [SerializeField] private Button _selectOrBuyButton;

    [SerializeField] private int[] skinCostInfo;

    private int currentIndex = 0, currentBalance = 0;

    private bool[] skinBuyedInfo;

    private void Start()
    {
        skinBuyedInfo = new bool[_skinsGameObjects.Length];

        GetSkinBuyedInfo();

        skinBuyedInfo[0] = true;

        currentIndex = PlayerPrefs.GetInt("CurrentSkinIndex", 0);
        currentBalance = PlayerPrefs.GetInt("Balance", 0);

        _coinsText.text = currentBalance.ToString();

        _skinsGameObjects[currentIndex].SetActive(true);
    }

    public void Arrow(int indexAmount)
    {
        _skinsGameObjects[currentIndex].SetActive(false);

        currentIndex += indexAmount;

        if (currentIndex > _skinsGameObjects.Length - 1)
            currentIndex = 0;
        else if (currentIndex < 0)
            currentIndex = _skinsGameObjects.Length - 1;

        _skinsGameObjects[currentIndex].SetActive(true);

        ChangeSelectOrBuyButtonState();
    }

    public void SelectOrBuyButton()
    {
        switch (_selectOrBuyText.text)
        {
            case "Buy":
                currentBalance -= skinCostInfo[currentIndex];
                _coinsText.text = currentBalance.ToString();

                skinBuyedInfo[currentIndex] = true;

                Debug.Log(currentBalance);

                SaveSkinBuyedInfo();

                break;

            case "Select":
                PlayerPrefs.SetInt("CurrentSkinIndex", currentIndex);
                _selectOrBuyButton.interactable = false;

                break;
        }

        ChangeSelectOrBuyButtonState();
    }

    private void ChangeSelectOrBuyButtonState()
    {
        Debug.Log(skinBuyedInfo[currentIndex]);

        if (!skinBuyedInfo[currentIndex] && currentBalance >= skinCostInfo[currentIndex])
        {
            _selectOrBuyText.text = "Buy";

            _selectOrBuyButton.interactable = true;
        }
        else if(!skinBuyedInfo[currentIndex] && currentBalance < skinCostInfo[currentIndex])
        {
            _selectOrBuyText.text = "Buy";

            _selectOrBuyButton.interactable = false;
        }
        else if (skinBuyedInfo[currentIndex] && currentIndex != PlayerPrefs.GetInt("CurrentSkinIndex", 0))
        {
            _selectOrBuyText.text = "Select";

            _selectOrBuyButton.interactable = true;
        }
        else if (skinBuyedInfo[currentIndex] && currentIndex == PlayerPrefs.GetInt("CurrentSkinIndex", 0))
        {
            _selectOrBuyText.text = "Select";

            _selectOrBuyButton.interactable = false;
        }
    }

    private void SaveSkinBuyedInfo()
    {
        string skin;

        for (int i = 0; i < _skinsGameObjects.Length; i++)
        {
            skin = string.Format("SkinBuyed{0}", i);

            PlayerPrefs.SetInt(skin, Convert.ToInt32(skinBuyedInfo[i]));
        }
    }

    private void GetSkinBuyedInfo()
    {
        string skin;

        for (int i = 0; i < _skinsGameObjects.Length; i++)
        {
            skin = string.Format("SkinBuyed{0}", i);

            skinBuyedInfo[i] = Convert.ToBoolean(PlayerPrefs.GetInt(skin, 0));
        }
    }

    private void ChangeBalance(int amount)
    {
        currentBalance += amount;

        PlayerPrefs.SetInt("Balance", currentBalance);
    }

    public void BackButton()
    {
        for (int i = 0; i < _skinsGameObjects.Length; i++)
            _skinsGameObjects[i].SetActive(false);

        gameObject.SetActive(false);
    }
}
