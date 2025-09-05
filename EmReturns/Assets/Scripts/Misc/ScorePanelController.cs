using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScorePanelController : MonoBehaviour
{
    public RectTransform selectorRt;
    public TMP_Text lettersText;

    private Vector2 selectorRtInitiaLPosition;
    private int selectorRtIndex = 0;
    private char[] initials = new char[3] { 'A', 'A', 'A' };

    public string Initials
    {
        get { return new string(initials); }
    }

    // Start is called before the first frame update
    void Start()
    {
        selectorRtInitiaLPosition = selectorRt.anchoredPosition;
        UpdateDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (Gamepad.current != null)
        {
            //if(Gamepad.current.crossButton.)
        }
        else if (Keyboard.current != null) 
        {
            if (Keyboard.current.upArrowKey.wasPressedThisFrame) 
            {
                ChangeLetter(-1);
            }
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                ChangeLetter(1);
            }
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                ChangeSelectorPosition(-1);
            }
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                ChangeSelectorPosition(1);
            }
        }
    }

    void ChangeSelectorPosition(int direction)
    {
        selectorRtIndex += direction;
        if (selectorRtIndex < 0) selectorRtIndex = 2;
        if (selectorRtIndex > 2) selectorRtIndex = 0;
        selectorRt.anchoredPosition = selectorRtInitiaLPosition + new Vector2(selectorRtIndex * 60, 0);
        UpdateDisplay();
    }

    void ChangeLetter(int direction)
    {
        initials[selectorRtIndex] = (char) (initials[selectorRtIndex] + direction);
        if (initials[selectorRtIndex] > 'Z') initials[selectorRtIndex] = 'A';
        if (initials[selectorRtIndex] < 'A') initials[selectorRtIndex] = 'Z';
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        // Opcional: poner corchetes en la letra seleccionada
        string result = "";
        for (int i = 0; i < initials.Length; i++)
        {
            if (i == selectorRtIndex)
                result += "[" + initials[i] + "]";
            else
                result += " " + initials[i] + " ";
        }
        lettersText.text = result;
    }
}
