using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class EmoticonSelection : MonoBehaviour
{
    public GameObject bulbFX;
    public GameObject stunnedStarsFX;
    public GameObject questionFX;
    public GameObject exclamationFX;

    public InputActionReference leftPrimaryAction;
    public InputActionReference rightPrimaryAction;
    public InputActionReference leftSecondaryAction;
    public InputActionReference rightSecondaryAction;

    private void OnEnable()
    {
        leftPrimaryAction.action.Enable();
        rightPrimaryAction.action.Enable();
        leftSecondaryAction.action.Enable();
        rightSecondaryAction.action.Enable();
    }

    private void OnDisable()
    {
        leftPrimaryAction.action.Disable();
        rightPrimaryAction.action.Disable();
        leftSecondaryAction.action.Disable();
        rightSecondaryAction.action.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        CheckButtonState(leftPrimaryAction, bulbFX);
        CheckButtonState(rightPrimaryAction, stunnedStarsFX);
        CheckButtonState(leftSecondaryAction, questionFX);
        CheckButtonState(rightSecondaryAction, exclamationFX);
    }

    void CheckButtonState(InputActionReference action, GameObject fxObject)
    {
        if (action != null)
        {
            bool buttonPressed = action.action.ReadValue<float>() > 0.5f;

            if (buttonPressed)
            {
                StartCoroutine(EmoteOn(fxObject));
            }
        }
    }

    IEnumerator EmoteOn(GameObject fxObject)
    {
        fxObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        fxObject.SetActive(false);
    }
}
