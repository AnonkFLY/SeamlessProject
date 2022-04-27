using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreView : MonoBehaviour
{
    [SerializeField] Color selfColor;
    public int score;
    private TMP_Text[] texts;
    private Transform _trans;
    private void Awake()
    {
        texts = GetComponentsInChildren<TMP_Text>();
        _trans = transform;
    }
    public void UpdateData(byte valueType, int value)
    {
        texts[valueType].text = value.ToString();
        if (valueType == 3)
            score = value;
    }
    public void ResetValue()
    {
        for (int i = 1; i < texts.Length; i++)
        {
            texts[i].text = "0";
        }
    }
    public void SetPlayerName(string name)
    {
        texts[0].text = name;
    }
    public void SetSelf()
    {
        GetComponent<Image>().color = selfColor;
    }
    public void SetUp()
    {
        _trans.SetAsFirstSibling();
    }
}
