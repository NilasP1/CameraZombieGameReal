using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import the TextMeshPro namespace
using UnityEngine.UI;

public class BulletManagerScript : MonoBehaviour
{
    public camerascript cameraScript;  // Reference to your existing camerascript
    public TextMeshProUGUI bulletText1;  // Text UI for bullet count
    public TextMeshProUGUI bulletText2;  // Text UI for bullet count
    public TextMeshProUGUI bulletText3;  // Text UI for bullet count

    private bool isPulsing = false; // Track if the pulsing effect is active
    private float pulseTimer = 0f;  // Timer for pulsing effect

    private void Start()
    {
        // Make sure to get the Camerascript reference if not already set
        if (cameraScript == null)
        {
            cameraScript = FindObjectOfType<camerascript>(); // Get Camerascript if it's in the scene
        }

        // Update bullet count UI on start
        UpdateBulletText();
    }

    private void Update()
    {
        // If the Camerascript is available, update the bullet count display
        if (cameraScript != null)
        {
            UpdateBulletText();

            // If no ammo left, start pulsing effect
            if (cameraScript.GetTotalBulletsLeft() <= 0)
            {
                PulseText();
            }
            else
            {
                // Stop pulsing effect if ammo is available
                if (isPulsing)
                {
                    isPulsing = false;
                    SetTextOpacity(1f); // Set to full opacity
                }
            }
        }
    }

    // Update the bullet count displayed in the UI
    void UpdateBulletText()
    {
        string bulletDisplay = "Bullets left: " + cameraScript.GetTotalBulletsLeft();

        // Check if there are no bullets left
        if (cameraScript.GetTotalBulletsLeft() <= 0)
        {
            bulletDisplay = "No Ammo Left"; // Display "No Ammo Left"
            SetTextColor(Color.red); // Set text to red
        }
        else
        {
            SetTextColor(Color.white); // Set text to white
        }

        // Update all bullet text elements
        if (bulletText1 != null) bulletText1.text = bulletDisplay;
        if (bulletText2 != null) bulletText2.text = bulletDisplay;
        if (bulletText3 != null) bulletText3.text = bulletDisplay;
    }

    // Function to change the text color
    void SetTextColor(Color color)
    {
        if (bulletText1 != null) bulletText1.color = color;
        if (bulletText2 != null) bulletText2.color = color;
        if (bulletText3 != null) bulletText3.color = color;
    }

    // Pulsing effect for the text opacity
    void PulseText()
    {
        if (!isPulsing)
        {
            isPulsing = true;
            pulseTimer = 0f;
        }

        pulseTimer += Time.deltaTime * 2f; // Increase pulse speed

        // Calculate opacity based on sine wave
        float opacity = Mathf.PingPong(pulseTimer, 1f) * 0.75f + 0.25f; // Oscillates between 0.25 and 1

        SetTextOpacity(opacity); // Set the opacity for the text
    }

    // Set the text opacity (clamps between 0.25 and 1)
    void SetTextOpacity(float opacity)
    {
        opacity = Mathf.Clamp(opacity, 0.25f, 1f); // Ensure opacity doesn't drop below 0.25

        Color currentColor = bulletText1.color;
        currentColor.a = opacity; // Set the alpha channel for transparency
        if (bulletText1 != null) bulletText1.color = currentColor;
        if (bulletText2 != null) bulletText2.color = currentColor;
        if (bulletText3 != null) bulletText3.color = currentColor;
    }
}
