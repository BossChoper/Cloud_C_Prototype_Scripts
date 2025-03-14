using UnityEngine;
using TMPro;

public class ComboSystem : MonoBehaviour
{
    public TextMeshProUGUI comboText;
    private int comboCount = 0;
    private string styleRank = "D";
    private float comboTimer = 0f;
    private float comboWindow = 2f;

    void Update()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0) ResetCombo();
        }
    }

    public void AddCombo(int amount)
    {
        comboCount += amount;
        comboTimer = comboWindow;
        UpdateStyleRank();
    }

    private void UpdateStyleRank()
    {
        if (comboCount >= 15) styleRank = "S";
        else if (comboCount >= 10) styleRank = "A";
        else if (comboCount >= 6) styleRank = "B";
        else if (comboCount >= 3) styleRank = "C";
        else styleRank = "D";
        comboText.text = $"Combo: {comboCount} | {styleRank}";
    }

    private void ResetCombo()
    {
        comboCount = 0;
        styleRank = "D";
        comboText.text = "Combo: 0 | D";
        Debug.Log("Combo dropped!");
    }

    public int GetComboCount() => comboCount;
}