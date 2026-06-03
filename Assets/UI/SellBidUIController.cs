using UnityEngine;
using UnityEngine.UI;
using TMPro;
using IrishFarmSim;

/// <summary>
/// Controller cho SellBidUI - xử lý timer, currentBid update, camera, và kết quả bid
/// Được gắn vào SellBidUI GameObject để chạy Update() khi UI active
/// </summary>
public class SellBidUIController : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public TextMeshProUGUI currentBid;

    private CameraController cameraControl;
    private Vector3 bidArea = new Vector3(109f, 0f, 137f);

    public GameObject BuySellAnimalUI; 
    public GameObject SellBidUI;

    void Start()
    {
        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
            cameraControl = camObj.GetComponent<CameraController>();

    }

    void Update()
    {
        // Kiểm tra hết giờ bidding khi SellBidUI active
        if (MartBidControl.bidding)
        {
            MartBidControl.currentTimer = (int)(MartBidControl.timeRemaining - (Time.time - MartBidControl.timeOfLastBid));

            // Cập nhật timer display
            if (timer != null)
                timer.text = "Going in: " + Mathf.Max(0, MartBidControl.currentTimer).ToString() + "s";

            // Cập nhật current bid display
            if (currentBid != null)
                currentBid.text = "Current Bid: " + MartBidControl.currentCowBid + " VND";

            // Kiểm tra hết giờ
            if (Time.time > MartBidControl.timeOfLastBid + MartBidControl.timeRemaining)
            {
                StopSelling();
            }
        }
    }

    private void StopSelling()
    {
        Debug.Log("StopBidding: Bidding ended");
        Debug.Log($"Last Bidder: {(MartBidControl.lastBidder != null ? MartBidControl.lastBidder.name : "NULL")}");
        Debug.Log($"Current Bid: {MartBidControl.currentCowBid}");
        Debug.Log($"Starting Price: {MartBidControl.startingPrice}");

        BidScript.EndBiddingRound();
        MartBidControl.bidding = false;
        MartBidControl.isSellMode = false;  // Unset SELL mode
        MartBidControl.timeRemaining = 10;


        // Có người bid - bán con vật
        int sellPrice = MartBidControl.currentCowBid;
        GameController.Instance().player.cash += sellPrice;
        
        // Remove con vật từ danh sách của player (trong trường hợp nó chưa được remove)
        if (MartBidControl.biddingCow != null && GameController.Instance().cows.Contains(MartBidControl.biddingCow))
        {
            GameController.Instance().cows.Remove(MartBidControl.biddingCow);
            Debug.Log($"✓ Removed '{MartBidControl.biddingCow.name}' from player inventory");
        }
        
        MartBidControl.cowsInMart.Remove(MartBidControl.biddingCow);
        Debug.Log($"✓ Sell: Animal sold for {sellPrice} VND");
        Debug.Log($"✓ Player cash now: {GameController.Instance().player.cash}");


        // Cleanup
        if (MartBidControl.biddingCow != null && MartBidControl.biddingCow.cowController != null)
            Destroy(MartBidControl.biddingCow.cowController.gameObject);

        ClearStats();

        // Reset biddingCow reference
        MartBidControl.biddingCow = null;
        MartBidControl.lastBidder = null;

        // Quay camera về player
        if (cameraControl != null)
            cameraControl.FollowPlayer();

        // Ẩn SellBidUI
        SellBidUI.SetActive(false);

        // Hiển thị BuySellAnimalUI
        BuySellAnimalUI.SetActive(true);
    }

    private void ClearStats()
    {
        MartBidControl.currentTimer = 0;
        MartBidControl.currentCowBid = 0;
        MartBidControl.playerBidLast = false;

        if (timer != null) timer.text = "0s";
        if (currentBid != null) currentBid.text = "Current Bid: 0 VND";
    }

    /// <summary>
    /// Quit selling - hủy phiên bid bán con vật
    /// Thêm lại con vật vào list nếu nó bị remove
    /// </summary>
    public void QuitSelling()
    {
        Debug.Log("=== QUIT SELLING ===");
        
        if (!MartBidControl.bidding)
        {
            Debug.LogWarning("Bidding not in progress!");
            return;
        }

        // Unset SELL mode
        MartBidControl.isSellMode = false;
        MartBidControl.bidding = false;
        Debug.Log("Bidding cancelled by player");

        // Dừng các bidders
        BidScript.EndBiddingRound();

        // Kiểm tra con vật có bị remove khỏi list không
        if (MartBidControl.biddingCow != null)
        {
            if (!GameController.Instance().cows.Contains(MartBidControl.biddingCow))
            {
                // Con vật bị remove khi bắt đầu bid, add lại vào list
                GameController.Instance().cows.Add(MartBidControl.biddingCow);
                Debug.Log($"✓ Added '{MartBidControl.biddingCow.name}' back to player inventory");
            }
            else
            {
                Debug.Log($"✓ '{MartBidControl.biddingCow.name}' was already in player inventory");
            }
            
            // Cleanup con vật ở bid area
            if (MartBidControl.biddingCow.cowController != null)
            {
                Destroy(MartBidControl.biddingCow.cowController.gameObject);
            }
            
            // Reset reference
            MartBidControl.biddingCow = null;
            MartBidControl.lastBidder = null;
        }

        ClearStats();

        // Quay camera về player
        if (cameraControl != null)
            cameraControl.FollowPlayer();

        // Ẩn SellBidUI
        SellBidUI.SetActive(false);

        // Hiển thị BuySellAnimalUI
        BuySellAnimalUI.SetActive(true);
    }
}

