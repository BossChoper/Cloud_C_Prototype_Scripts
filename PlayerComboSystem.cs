using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerComboSystem : MonoBehaviour
{
    // Define input buttons (mapped to Unity Input keys)
    private const string BUTTON_X = "X"; // We'll map this to a key
    private const string BUTTON_A = "A"; // We'll map this to another key

    // Combo timing window
    public float comboWindow = 0.5f; // Time in seconds between inputs to continue a combo
    private float comboTimer = 0f; // Timer to track time since last input

    // Current combo string being built
    private List<string> currentCombo = new List<string>();

    // Defined combos and their actions
    private Dictionary<string, System.Action> comboActions;

    // Flag to track if a combo is in progress
    private bool isComboActive = false;

    void Start()
    {
        // Initialize combo dictionary with predefined combos
        comboActions = new Dictionary<string, System.Action>
        {
            // "X, X, A" combo
            { "X,X,A", () => Debug.Log("Performed Triple Slash Combo! (X, X, A) - High damage combo.") },
            // "X, A" combo
            { "X,A", () => Debug.Log("Performed Quick Slash Kick! (X, A) - Fast attack.") },
            // "A, A" combo
            { "A,A", () => Debug.Log("Performed Double Kick! (A, A) - Staggering attack.") }
        };

        Debug.Log("Combo System Initialized. Try combos like X,X,A or X,A!");
    }

    void Update()
    {
        // Update combo timer if a combo is in progress
        if (isComboActive)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }

        // Detect inputs
        if (Input.GetKeyDown(KeyCode.X)) // Map "X" key to BUTTON_X
        {
            AddInput(BUTTON_X);
        }
        else if (Input.GetKeyDown(KeyCode.A)) // Map "A" key to BUTTON_A
        {
            AddInput(BUTTON_A);
        }
    }

    private void AddInput(string input)
    {
        // Add the input to the current combo
        currentCombo.Add(input);
        Debug.Log($"Input added: {input}, Current Combo: {string.Join(",", currentCombo)}");

        // Start or refresh the combo timer
        comboTimer = comboWindow;
        isComboActive = true;

        // Check if the current combo matches any defined combo
        CheckForCombo();
    }

    private void CheckForCombo()
    {
        // Convert current combo list to a string for comparison
        string comboString = string.Join(",", currentCombo);

        // Check if the current combo matches any defined combo exactly
        if (comboActions.ContainsKey(comboString))
        {
            ExecuteCombo(comboString);
            ResetCombo();
            return;
        }

        // Check if the current combo is a prefix of any defined combo
        bool isPrefix = false;
        foreach (var definedCombo in comboActions.Keys)
        {
            if (definedCombo.StartsWith(comboString) && definedCombo.Length > comboString.Length)
            {
                isPrefix = true;
                break;
            }
        }

        // If the current combo isn't a prefix of any defined combo and doesn't match exactly, reset
        if (!isPrefix && currentCombo.Count > 0)
        {
            Debug.Log($"No matching combo for {comboString}, resetting combo.");
            ResetCombo();
        }
    }

    private void ExecuteCombo(string combo)
    {
        if (comboActions.TryGetValue(combo, out System.Action action))
        {
            action.Invoke();
        }
    }

    private void ResetCombo()
    {
        currentCombo.Clear();
        comboTimer = 0f;
        isComboActive = false;
        Debug.Log("Combo reset.");
    }

    // Optional: Visualize combo state in Inspector for debugging
    void OnGUI()
    {
        GUILayout.Label($"Current Combo: {string.Join(",", currentCombo)}");
        GUILayout.Label($"Combo Timer: {comboTimer:F2}s");
    }
}