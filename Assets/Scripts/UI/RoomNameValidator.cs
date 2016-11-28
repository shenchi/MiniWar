using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomNameValidator : MonoBehaviour
{
    public InputField input;

    // Use this for initialization
    void Start()
    {
        input.onValidateInput += Validate;
    }

    char Validate(string input, int charIndex, char addedChar)
    {
        if (char.IsLetterOrDigit(addedChar) || char.IsWhiteSpace(addedChar))
            return addedChar;
        return '\0';
    }
}
