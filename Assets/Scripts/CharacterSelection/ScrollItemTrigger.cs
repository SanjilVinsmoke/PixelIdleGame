using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollItemTrigger : MonoBehaviour, IPointerClickHandler
{
    private static GameObject selectedChild = null;  // Track selected child
    [SerializeField] private Button selectButton;    // Reference to the select button in your game manager

    public void OnPointerClick(PointerEventData eventData)
    {
        // Get parent (Content) and disable Outline for all children
        Transform content = transform.parent;
        foreach (Transform child in content)
        {
            Outline outline = child.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false; // Disable outline on other children
            }
        }

        // Enable Outline for the clicked object
        Outline clickedOutline = gameObject.GetComponent<Outline>();
        if (clickedOutline != null)
        {
            clickedOutline.enabled = true;
        }

        // Store the selected child
        selectedChild = gameObject;

        // Enable the select button
        if (selectButton != null)
        {
            selectButton.interactable = true;  // Enable button when a child is selected
            selectButton.gameObject.SetActive(true);
        }

        Debug.Log("Clicked on: " + gameObject.name);
    }

    // Get the selected child data
    public static GameObject GetSelectedChild()
    {
        return selectedChild;
    }

    // Set the select button dynamically from CharacterSelectionController
    public void SetSelectButton(Button button)
    {
        selectButton = button;
    }
}