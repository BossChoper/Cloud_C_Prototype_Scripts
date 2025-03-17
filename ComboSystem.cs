using UnityEngine;
using TMPro;

public class ComboSystem : MonoBehaviour
{
    public TextMeshProUGUI comboText;
    public int comboCount { get; private set; } = 0;
    public string styleRank { get; private set; } = "D";
    
    private float comboTimer = 0f;
    private float comboWindow = 2f;
    private bool lastWasSpace = false;

    void Update()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0) ResetCombo();
        }
    }

    public void AddCombo(int amount, bool resetTimer = true)
    {
        comboCount += amount;
        if (resetTimer) comboTimer = comboWindow;
        UpdateStyleRank();
    }

    public void UpdateCombo(bool isSpace)
    {
        if (isSpace != lastWasSpace) comboCount += 2;
        else comboCount++;
        lastWasSpace = isSpace;
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
        UpdateComboText();
    }

    private void UpdateComboText()
    {
        if (comboText != null)
        {
            comboText.text = $"Combo: {comboCount} | {styleRank}";
        }
    }

    public void ResetCombo()
    {
        comboCount = 0;
        styleRank = "D";
        UpdateComboText();
        Debug.Log("Combo dropped!");
    }
}
