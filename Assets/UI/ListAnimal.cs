using UnityEngine;
using UnityEngine.UI;
using TMPro;
using IrishFarmSim;

public class ListAnimal : MonoBehaviour
{
    [SerializeField] private Sprite[] Icons;  // Sprite icons cho từng loại animal (Cow, Chicken, Sheep, etc.)
    [SerializeField] private GameObject animalTemplate;  // Template item từ Unity Inspector

    public GameObject SellBidUI;
    public GameObject SellUI;
    public GameObject BuySellAnimalUI;  
    public TextMeshProUGUI cash;
    public TextMeshProUGUI age;
    public TextMeshProUGUI weight;
    public TextMeshProUGUI gender;
    public TextMeshProUGUI pregnant;
    public TextMeshProUGUI breed;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI currentBid;

    private bool timerStart;
    public Image healthBar;
    public Image happinessBar;
    private CameraController cameraControl;

    private string cowGender = "Male";
    private string cowPregnant = "No";
    private Vector3 bidArea = new Vector3(109f, 0f, 137f);

    void Start()
    {
        // Template phải được gán từ Unity Inspector
        if (animalTemplate == null)
        {
            Debug.LogError("ListAnimal: animalTemplate is not assigned! Please drag the template prefab/GameObject into the Inspector.");
            return;
        }

        // Check SellUI & SellBidUI
        if (SellUI == null)
            Debug.LogWarning("ListAnimal: SellUI is not assigned!");
        if (SellBidUI == null)
            Debug.LogWarning("ListAnimal: SellBidUI is not assigned!");

        // Get CameraController
        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            cameraControl = camObj.GetComponent<CameraController>();
        else
            Debug.LogWarning("ListAnimal: MainCamera not found!");

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

    void Update()
    {
        // Logic moved to SellBidUIController - nó chạy khi SellBidUI active
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
            
            // Xóa tất cả item cũ để clear list UI
            foreach (Transform child in transform)
            {
                if (child.gameObject != animalTemplate)
                {
                    Destroy(child.gameObject);
                }
            }
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
                animalButton.onClick.AddListener(() => StartSellAnimal(animalType, animal, index));
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
    /// Khởi động quy trình bán con vật (generic cho tất cả loại)
    /// Dựa trên logic từ BidScript.CowBuyStartBid()
    /// </summary>
    private void StartSellAnimal(string animalType, object animal, int index)
    {
        Debug.Log($"StartSellAnimal: {animalType} - {GetAnimalName(animal)}");

        // Set SELL mode để BidScript.Update() không trigger timeout
        MartBidControl.isSellMode = true;

        // Lưu animal info vào MartBidControl
        if (animal is Cow cow)
        {
            MartBidControl.biddingCow = cow;
            GameController.Instance().cowIndex = index;

            // Remove con vật khỏi danh sách của player (để phân biệt: đang bán vs đã bán)
            if (GameController.Instance().cows.Contains(cow))
            {
                GameController.Instance().cows.Remove(cow);
                Debug.Log($"✓ Animal '{cow.name}' removed from player inventory for selling");
                Debug.Log($"  Remaining cows: {GameController.Instance().cows.Count}");
            }
            else
            {
                Debug.LogWarning($"⚠ Animal '{cow.name}' not found in player inventory!");
                Debug.Log($"  Current cows count: {GameController.Instance().cows.Count}");
            }

            // Hiển thị SellBidUI
            if (SellUI != null)
                SellUI.SetActive(false);
            if (SellBidUI != null)
                SellBidUI.SetActive(true);
                

            // Cập nhật thông tin animal ở UI (lấy từ animal được chọn)
            SetHealth(cow.health / 100f);
            SetHappiness(cow.happiness / 10f);

            // Check gender từ animal được chọn
            if (!cow.gender == true)  // Female
            {
                cowGender = "Female";
                if (cow.pregnant == true)
                    cowPregnant = "Yes";
            }
            else  // Male
            {
                cowGender = "Male";
                cow.pregnant = false;
                cowPregnant = "No";
            }

            // Update all text fields từ animal object
            if (cash != null) cash.text = GameController.Instance().player.cash.ToString("N0") + " VND";
            if (age != null) age.text = cow.age.ToString() + " years";
            if (weight != null) weight.text = cow.weight.ToString() + " KG";
            if (gender != null) gender.text = cowGender;
            if (pregnant != null) pregnant.text = cowPregnant;
            if (breed != null) breed.text = cow.breed;
            
            // Set starting price dựa trên animal stats
            int startPrice = GetAnimalPrice(animal);
            MartBidControl.startingPrice = startPrice;
            MartBidControl.currentCowBid = startPrice;
            MartBidControl.timeRemaining = 10;
            
            if (currentBid != null) currentBid.text = "Current Bid: " + MartBidControl.currentCowBid + " VND";
        }
        
        // Spawn vật trực tiếp vào bid area
        Vector3 bidArea = new Vector3(109f, 0f, 137f);
        SpawnAnimalToBidArea(animal, animalType, bidArea);
        
        // Chờ cow di chuyển tới bid area rồi khởi động bidding
        // Gọi coroutine từ SellBidUI vì ListAnimal có thể inactive
        if (SellBidUI != null)
            SellBidUI.GetComponent<MonoBehaviour>().StartCoroutine(WaitForAnimal(animal, bidArea));
        else
            StartCoroutine(WaitForAnimal(animal, bidArea));
    }    /// <summary>
    /// Chờ con vật di chuyển tới bid area
    /// Dựa trên BidScript.WaitForCow()
    /// </summary>
    private System.Collections.IEnumerator WaitForAnimal(object animal, Vector3 bidArea)
    {
        yield return new WaitForSeconds(1f);

        if (animal is Cow cow && cow.cowController != null)
        {
            cow.cowController.MoveTo(bidArea);

            // Chờ cow tới gần bid area
            while (Vector3.Distance(cow.cowController.ReturnPosition(), bidArea) > 2f)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        // Cow đã tới, quay camera nhìn vào ring
        LookAtRing();

        // Khởi động bidding
        MartBidControl.timeOfLastBid = Time.time;
        BidScript.StartNewRound();
        MartBidControl.bidding = true;
    }

    /// <summary>
    /// Spawn con vật trực tiếp tới bid area
    /// Hỗ trợ tất cả loại vật, dựa trên CowMaker.SpawnCow()
    /// </summary>
    private void SpawnAnimalToBidArea(object animal, string animalType, Vector3 bidArea)
    {
        if (animal is Cow cow)
        {
            // Cleanup controller cũ nếu có
            if (cow.cowController != null)
            {
                Destroy(cow.cowController.gameObject);
            }

            // Load prefab từ breed (theo CowMaker pattern)
            GameObject animalPrefab = Resources.Load<GameObject>(cow.breed);

            if (animalPrefab != null)
            {
                // Spawn tại bid area
                Vector3 spawnLocation = bidArea;
                spawnLocation.y = Terrain.activeTerrain.SampleHeight(spawnLocation) + 0.5f;

                GameObject animalObj = Instantiate(animalPrefab, spawnLocation, Quaternion.identity);

                // Set scale dựa trên weight (theo CowMaker pattern)
                float scale = Mathf.Pow(cow.weight, .125f);
                animalObj.transform.localScale = new Vector3(scale, scale, scale);

                // Gán controller (theo CowMaker pattern)
                CowController cowController = animalObj.GetComponent<CowController>();
                if (cowController != null)
                {
                    cow.cowController = cowController;
                    cowController.cow = cow;
                    Debug.Log($"Cow spawned at bid area: {spawnLocation}, Scale: {scale}, Breed: {cow.breed}");
                }
            }
            else
            {
                Debug.LogError($"Cow prefab '{cow.breed}' not found in Resources!");
            }
        }
        // TODO: Thêm các loại vật khác với spawn logic riêng
    }

    /// <summary>
    /// Khởi động phiên đấu giá
    /// </summary>

    private int GetAnimalPrice(object animal)
    {
        if (animal is Cow cow)
        {
            if (cow.age <= 0)
                return (cow.weight + (int)(cow.health + cow.happiness)) * 10;
            return (int)((cow.weight + (cow.health + cow.happiness) / cow.age) * 10);
        }
        // TODO: Thêm các loại vật khác với công thức tính giá riêng
        return 1000; // Giá mặc định
    }

    /// <summary>
    /// Helper methods tương tự BidScript
    /// </summary>
    private void SetHealth(float health)
    {
        if (healthBar != null)
            healthBar.fillAmount = health;
    }

    private void SetHappiness(float happiness)
    {
        if (happinessBar != null)
            happinessBar.fillAmount = happiness;
    }

    /// <summary>
    /// Camera nhìn vào bid ring (tương tự BidScript.LookAtRing())
    /// </summary>
    private void LookAtRing()
    {
        if (cameraControl == null)
        {
            Debug.LogWarning("ListAnimal: CameraController not found! Make sure MainCamera has the tag.");
            return;
        }
        Vector3 height = new Vector3(0, 2, 0);
        Vector3 position = new Vector3(96, 8, 142.31f);
        cameraControl.MoveToLookAt(position, bidArea + height);
    }
}
