using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] private Button selectButton;
    [SerializeField] private Transform content;  // Reference to Content of ScrollView
    [SerializeField] private Transform player;  // Reference to Content of ScrollView

    private void Start()
    {
        
        // Make sure button is disabled initially
        if (selectButton != null)
        {
            selectButton.interactable = false;
            selectButton.gameObject.SetActive(false);
            selectButton.onClick.AddListener(OnSelectButtonClick);  // Add listener for select button click
        }

        // Add ScrollItemTrigger to all children of the Content
        foreach (Transform child in content)
        {
            if (child.GetComponent<ScrollItemTrigger>() == null)
            {
                ScrollItemTrigger trigger = child.gameObject.AddComponent<ScrollItemTrigger>();
                trigger.SetSelectButton(selectButton);
            }
        }
        //Init SaveSystem
        SaveSystem.Init();
        Load();
    }

    private void OnSelectButtonClick()
    {
        // Get the selected child
        GameObject selectedChild = ScrollItemTrigger.GetSelectedChild();

        if (selectedChild != null)
        {
            // Send the selected child’s data (you can customize this part)
            Debug.Log("Selected: " + selectedChild.name);
            Save(selectedChild.name);
            // You can pass this data to other scripts or UI as needed
        }
        else
        {
            Debug.LogWarning("No child selected!");
        }
    }
    //Save Function
    public void Save(string prefabName)
    {
        SaveObject saveObject = new SaveObject
        {
            prefabPath  = "PlayerSkins/" + prefabName,
            prefabName = prefabName
        };
        string json = JsonUtility.ToJson(saveObject);
        SaveSystem.Save(json);
        
        Load();
    }
    //Load Save Preference
    private void Load()
    {
        string saveString = SaveSystem.Load();
        if (saveString != null)
        {
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);
            LoadPrefab(saveObject.prefabPath);
        }
        else
        {
            //LoadPrefab("PlayerSkins/Male.prefab");
            Debug.Log("No Save File");
        }
    }
            
    //load/change player prefab
    public void LoadPrefab(string prefabPath)
    {
                
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab != null)
        {
            // Remove all current children
            foreach (Transform child in player)
            {
                Destroy(child.gameObject);
            }
            // Instantiate prefab and set it as a child of the current GameObject (this.gameObject)
            GameObject newPrefabInstance = Instantiate(prefab, player.transform); // Use this.transform to make it a child
            //newPrefabInstance.transform.localPosition = Vector3.zero; // Optionally reset the position to (0, 0, 0)

            Debug.Log("Loaded and Instantiated: " + prefabPath);
        }
        else
        {
            Debug.LogError("Prefab not found: " + prefabPath);
        }
    }
}