using UnityEngine;
using UnityEngine.UI;
using TMPro;
using IrishFarmSim;

public class ListAnimal : MonoBehaviour
{
    [SerializeField] private Sprite[] Icons;  // Sprite icons cho từng loại animal (Cow, Chicken, Sheep, etc.)
    [SerializeField] private GameObject animalTemplate;  // Template item từ Unity Inspector

    void Start()
    {
        // Template phải được gán từ Unity Inspector
        if (animalTemplate == null)
        {
            Debug.LogError("ListAnimal: animalTemplate is not assigned! Please drag the template prefab/GameObject into the Inspector.");
            return;
        }
        
        // Debug: In cấu trúc template
        Debug.Log($"ListAnimal: Template structure:");
        Debug.Log($"  - Template children count: {animalTemplate.transform.childCount}");
        for (int i = 0; i < animalTemplate.transform.childCount; i++)
        {
            Transform child = animalTemplate.transform.GetChild(i);
            Image img = child.GetComponent<Image>();
            TextMeshProUGUI tmpText = child.GetComponent<TextMeshProUGUI>();
            Text legacyText = child.GetComponent<Text>();
            Debug.Log($"  - Child({i}): {child.name} | Image: {(img != null)}, TMP: {(tmpText != null)}, Text: {(legacyText != null)}");
        }
        
        // Ẩn template
        animalTemplate.SetActive(false);
        Debug.Log("ListAnimal: Template ready");
    }

    /// <summary>
    /// Tạo template item từ code - KHÔNG DÙNG NỮA
    /// </summary>
    private void CreateTemplateFromCode()
    {
        // Removed - use template from Inspector instead
    }

    /// <summary>
    /// Generic function hiển thị danh sách vật dựa trên animal type
    /// Dùng chung cho tất cả loại vật (Cow, Chicken, Pig, etc.)
    /// </summary>
    public void ShowAnimalListByType(string animalType)
    {
        Debug.Log($"ShowAnimalListByType() called - Displaying {animalType} list");
        
        // Lấy danh sách vật từ GameController dựa trên type
        var animalList = GetAnimalListByType(animalType);

        if (animalList == null || animalList.Count == 0)
        {
            Debug.LogWarning($"Player doesn't have any {animalType}!");
            return;
        }

        // Đếm số lượng vật
        int animalCount = animalList.Count;
        Debug.Log($"Found {animalCount} {animalType}(s)");

        // Xóa tất cả item cũ (ngoại trừ template nếu có)
        foreach (Transform child in transform)
        {
            if (child.gameObject != animalTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        // Loop qua từng con vật
        for (int i = 0; i < animalCount; i++)
        {
            // Lấy con vật từ danh sách
            object animal = animalList[i];
            
            // Instantiate template
            GameObject g = Instantiate(animalTemplate, transform);
            g.SetActive(true);
            g.name = $"Animal_{i}_{GetAnimalName(animal)}";
            
            Debug.Log($"[{i}] Created: {g.name}");

            // Điền icon vào GetChild(1)
            if (g.transform.childCount > 1)
            {
                Image iconImage = g.transform.GetChild(1).GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = GetAnimalIcon(animalType);
                    Debug.Log($"[{i}] ✓ Icon set");
                }
            }

            // Điền name vào GetChild(2)
            if (g.transform.childCount > 2)
            {
                TextMeshProUGUI nameText = g.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                if (nameText != null)
                {
                    string animalName = GetAnimalName(animal);
                    nameText.text = animalName;
                    Debug.Log($"[{i}] ✓ Name set: {animalName}");
                }
                else
                {
                    Debug.LogWarning($"[{i}] ✗ GetChild(2) không có TextMeshProUGUI component");
                }
            }
            else
            {
                Debug.LogWarning($"[{i}] ✗ GetChild(2) không tồn tại (childCount: {g.transform.childCount})");
            }

            // Thêm onClick listener
            Button animalButton = g.GetComponent<Button>();
            if (animalButton != null)
            {
                int index = i;
                animalButton.onClick.AddListener(() => OnAnimalSelected(animalType, index, animal));
            }
        }
        
        Debug.Log($"Successfully created {animalCount} {animalType} items");
    }

    /// <summary>
    /// Lấy danh sách vật từ GameController dựa trên type
    /// </summary>
    private System.Collections.Generic.List<object> GetAnimalListByType(string animalType)
    {
        switch (animalType.ToLower())
        {
            case "cow":
                var cowList = GameController.Instance().cows;
                Debug.Log($"GetAnimalListByType(Cow): Found {cowList.Count} cows");
                for (int i = 0; i < cowList.Count; i++)
                {
                    Debug.Log($"  - Cow {i}: {cowList[i].name} (Breed: {cowList[i].breed})");
                }
                return cowList.ConvertAll(x => (object)x);
            // TODO: Thêm các loại vật khác khi có
            // case "chicken":
            //     return GameController.Instance().chickens.ConvertAll(x => (object)x);
            // case "pig":
            //     return GameController.Instance().pigs.ConvertAll(x => (object)x);
            default:
                Debug.LogWarning($"Unknown animal type: {animalType}");
                return null;
        }
    }

    /// <summary>
    /// Lấy tên từ animal object
    /// </summary>
    private string GetAnimalName(object animal)
    {
        if (animal is Cow cow)
            return cow.name;
        // TODO: Thêm các loại vật khác
        // if (animal is Chicken chicken)
        //     return chicken.name;
        return "Unknown";
    }

    /// <summary>
    /// Lấy icon dựa trên animal type (không phải breed)
    /// Mỗi loại vật (Cow, Chicken, Pig) có một icon riêng
    /// </summary>
    private Sprite GetAnimalIcon(string animalType)
    {
        // Nếu có Icons array, lấy từ đó
        if (Icons != null && Icons.Length > 0)
        {
            switch (animalType.ToLower())
            {
                case "cow":
                    return Icons.Length > 0 ? Icons[0] : null;
                case "chicken":
                    return Icons.Length > 1 ? Icons[1] : null;
                case "pig":
                    return Icons.Length > 2 ? Icons[2] : null;
                case "sheep":
                    return Icons.Length > 3 ? Icons[3] : null;
                case "horse":
                    return Icons.Length > 4 ? Icons[4] : null;
                case "goat":
                    return Icons.Length > 5 ? Icons[5] : null;
                default:
                    return Icons.Length > 0 ? Icons[0] : null;
            }
        }
        
        // Nếu không có Icons array, trả về null (sẽ không gán sprite)
        Debug.LogWarning($"No Icons array assigned for {animalType}");
        return null;
    }

    /// <summary>
    /// Callback khi bấm vào một item trong danh sách
    /// </summary>
    private void OnAnimalSelected(string animalType, int index, object animal)
    {
        Debug.Log($"Selected {animalType}: {GetAnimalName(animal)} (Index: {index})");
        // TODO: Thêm logic xử lý khi chọn một con vật
        // Ví dụ: Hiển thị thông tin chi tiết, mở menu bán/cho ăn, etc.
    }
}
