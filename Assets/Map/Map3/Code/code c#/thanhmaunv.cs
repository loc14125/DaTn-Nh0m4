using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class thanhmaunv : MonoBehaviour
{
    public Image fillBar;
    public TextMeshProUGUI valuaText;

    public void UpdateBar(int currentValua, int maxValua)
    {
        fillBar.fillAmount = (float)currentValua / (float)maxValua;
        valuaText.text = currentValua.ToString() + "/ " + maxValua.ToString();
    }
}
