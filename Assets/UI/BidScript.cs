using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace IrishFarmSim
{
    public class BidScript : MartInit
    {
        private GameObject scrollCowList;
        private CameraController cameraControl;
        public TextMeshProUGUI cash;
        public TextMeshProUGUI age;
        public TextMeshProUGUI weight;
        public TextMeshProUGUI gender;
        public TextMeshProUGUI pregnant;
        public TextMeshProUGUI breed;
        public TextMeshProUGUI timer;
        public TextMeshProUGUI currentBid;

        public GameObject BeforeBidUI;
        public GameObject BidUI;
        public GameObject BuySellAnimalUI;

        // Private local variables
        private string cowGender = "Male";
        private string cowPregnant = "No";
        private bool timerStart;
        private bool timerChecked = true;
        private bool cowInRing;
        private bool isLoading;
        public Image healthBar;
        public Image happinessBar;

        // Camera control variables
        private Vector3 bidArea = new Vector3(109f, 0f, 137f);
        private Vector3 cameraPosition;
        public GameObject bidWon;
        public GameObject bidLost;
        private bool bidResult;

        void Start()
        {
            try
            {
                InitUIControl();
                InitBidControl();
            }
            catch (System.Exception e) // Dùng Exception chung để bắt mọi lỗi
            {
                Debug.LogError("Error during Initialization: " + e.Message);
            }
        }

        void Update()
        {
            // Kiểm tra hết giờ bidding
            if (MartBidControl.bidding)
            {
                MartBidControl.currentTimer = (int)(MartBidControl.timeRemaining - (Time.time - MartBidControl.timeOfLastBid));

                // Cập nhật timer display
                if (timer != null)
                    timer.text = "Going in: " + Mathf.Max(0, MartBidControl.currentTimer).ToString() + "s";

                // Cập nhật current bid display
                UpdateCurrentBidDisplay();

                // Kiểm tra hết giờ
                if (Time.time > MartBidControl.timeOfLastBid + MartBidControl.timeRemaining)
                {
                    StopBidding();
                }
            }
        }

        private void InitUIControl()
        {
            GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
            if (camObj != null)
                cameraControl = camObj.GetComponent<CameraController>();
        }

        private void InitBidControl()
        {
            MartBidControl.cowList = GameObject.Find("CowSelectList");
            if (MartBidControl.cowList != null)
                MartBidControl.cowList.SetActive(false);

            // KHÔNG ghi đè bidderList - nó được spawn từ MartInit.Start()
            // MartBidControl.bidderList đã được khởi tạo bởi MartInit
            if (MartBidControl.bidderList == null)
                MartBidControl.bidderList = new List<Bidder>();
            
            MartBidControl.timeRemaining = 10;
            MartBidControl.biddingCow = new Cow();
        }

        private void SetHealth(float health) => healthBar.fillAmount = health;
        private void SetHappiness(float happiness) => happinessBar.fillAmount = happiness;

        public void CowBuyStartBid()
        {
            if (MartBidControl.cowsInMart.Count == 0)
            {
                Debug.LogWarning("No cows available in mart!");
                return;
            }

            GameController.Instance().cowIndex = Random.Range(0, MartBidControl.cowsInMart.Count);
            MartBidControl.biddingCow = MartBidControl.cowsInMart[GameController.Instance().cowIndex];

            LookAtRing();
            BeforeBidUI.SetActive(false);
            BidUI.SetActive(true);

            SetHealth(MartBidControl.biddingCow.health / 100f);
            SetHappiness(MartBidControl.biddingCow.happiness / 10f);

            // Match MartUI logic: check gender
            if (!MartBidControl.biddingCow.gender == true)  // Female
            {
                cowGender = "Female";
                if (MartBidControl.biddingCow.pregnant == true)
                    cowPregnant = "Yes";
            }
            else  // Male
            {
                cowGender = "Male";
                MartBidControl.biddingCow.pregnant = false;
                cowPregnant = "No";
            }

            // Update all text fields
            cash.text = GameController.Instance().player.cash.ToString("N0") + " VND";
            age.text = MartBidControl.biddingCow.age.ToString() + " years";
            weight.text = MartBidControl.biddingCow.weight.ToString() + " KG";
            gender.text = cowGender;
            pregnant.text = cowPregnant;
            breed.text = MartBidControl.biddingCow.breed;
            currentBid.text = "Current Bid: " + MartBidControl.currentCowBid + " VND";

            // Start Bid logic - match MartUI
            if (!cowInRing && MartBidControl.cowsInMart.Count > 0)
            {
                GameController.Instance().cowIndex = Random.Range(0, MartBidControl.cowsInMart.Count);
                MartBidControl.biddingCow = MartBidControl.cowsInMart[GameController.Instance().cowIndex];
                StartBidding();
            }
        }

        public void BidOn() // Gắn vào Button Bid
        {
            // Match MartUI logic exactly
            if (!MartBidControl.playerBidLast && cowInRing)
            {
                if (GameController.Instance().player.cash >= MartBidControl.currentCowBid + 500)
                {
                    BidOnCow(null, MartBidControl.currentCowBid + 500);
                    MartBidControl.playerBidLast = true;
                    LookAtRing();
                }
            }
        }

        public static void BidOnCow(Bidder bidder, float bid)
        {
            if (bid >= MartBidControl.currentCowBid)
            {
                // Tìm CameraController hiệu quả hơn thay vì tìm mỗi lần bid (nhưng giữ nguyên cấu trúc static của bạn)
                GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
                CameraController cam = camObj != null ? camObj.GetComponent<CameraController>() : null;

                if (bidder != null && cam != null)
                {
                    MartBidControl.playerBidLast = false;
                    cam.LookAt(bidder.gameObject.transform.position);
                }

                MartBidControl.currentCowBid = (int)bid;
                MartBidControl.lastBidder = bidder;

                EndBiddingRound();
                StartNewRound();
            }
        }

        private void UpdateCurrentBidDisplay()
        {
            if (currentBid != null)
                currentBid.text = "Current Bid: " + MartBidControl.currentCowBid + " VND";
        }
        // Press button x
        public void Quit()
        {
            if (!cowInRing)
            {
                BidUI.SetActive(false);
                BeforeBidUI.SetActive(false);

                if (cameraControl != null)
                    cameraControl.FollowPlayer();
                else
                    Debug.LogWarning("cameraControl is null in Quit()");
            }
            else
            {
                Debug.LogWarning("Cannot quit while cow is in ring (bidding in progress)");
            }
        }

        private void StartBidding()
        {
            cowInRing = true;
            MartBidControl.startingPrice = SetCowPrice(MartBidControl.biddingCow);
            MartBidControl.currentCowBid = MartBidControl.startingPrice;
            cameraControl.WatchTarget(MartBidControl.biddingCow.cowController.gameObject);
            StartCoroutine(WaitForCow());
        }

        private void StopBidding()
        {
            EndBiddingRound();
            MartBidControl.bidding = false;
            MartBidControl.timeRemaining = 10;

            if (GameController.Instance().cows.Contains(MartBidControl.biddingCow))
            {
                GameController.Instance().player.cash += MartBidControl.currentCowBid;
                GameController.Instance().cows.Remove(MartBidControl.biddingCow);
            }
            else
            {
                if (MartBidControl.playerBidLast)
                {
                    GameController.Instance().player.cash -= MartBidControl.currentCowBid;
                    GameController.Instance().cows.Add(MartBidControl.biddingCow);
                    MartBidControl.cowsInMart.Remove(MartBidControl.biddingCow);
                    bidWon.SetActive(true);
                }
                else
                {
                    MartBidControl.cowsInMart.Remove(MartBidControl.biddingCow);
                    bidLost.SetActive(true);
                }
                StartCoroutine(CloseResultUI());
            }

            // Cleanup
            if (MartBidControl.biddingCow.cowController != null)
                Destroy(MartBidControl.biddingCow.cowController.gameObject);

            ClearStats();
            BeforeBidUI.SetActive(true);
            BidUI.SetActive(false);
            
            if (cameraControl != null)
                cameraControl.FollowPlayer();
        }

        private void ClearStats()
        {
            MartBidControl.currentTimer = 0;
            MartBidControl.currentCowBid = 0;
            cowInRing = false;
            MartBidControl.playerBidLast = false;

            SetHealth(0);
            SetHappiness(0);
            if (timer != null) timer.text = "0s";
        }

        private IEnumerator CloseResultUI()
        {
            yield return new WaitForSeconds(3f);
            bidWon.SetActive(false);
            bidLost.SetActive(false);
        }

        private IEnumerator WaitForCow()
        {
            yield return new WaitForSeconds(1f);
            MartBidControl.biddingCow.cowController.MoveTo(bidArea);

            while (Vector3.Distance(MartBidControl.biddingCow.cowController.ReturnPosition(), bidArea) > 2f)
            {
                yield return new WaitForSeconds(0.5f);
            }

            MartBidControl.timeOfLastBid = Time.time;
            StartNewRound();
            MartBidControl.bidding = true;
        }

        private int SetCowPrice(Cow cow)
        {
            if (cow.age <= 0) return (cow.weight + (int)(cow.health + cow.happiness)) * 10;
            return (int)((cow.weight + (cow.health + cow.happiness) / cow.age) * 10);
        }

        public static void StartNewRound()
        {
            MartBidControl.timeOfLastBid = Time.time;
            if (MartBidControl.timeRemaining > 3) MartBidControl.timeRemaining--;

            foreach (Bidder bidder in MartBidControl.bidderList)
                if (bidder != MartBidControl.lastBidder)
                    bidder.ConsiderBidding(MartBidControl.biddingCow, MartBidControl.currentCowBid);
        }

        public static void EndBiddingRound()
        {
            foreach (Bidder bidder in MartBidControl.bidderList)
                bidder.StopBidding();
        }

        private void LookAtRing()
        {
            if (cameraControl == null)
            {
                Debug.LogWarning("CameraController not found! Make sure MainCamera has the tag.");
                return;
            }
            Vector3 height = new Vector3(0, 2, 0);
            Vector3 position = new Vector3(96, 8, 142.31f);
            cameraControl.MoveToLookAt(position, bidArea + height);
        }
        // Đã xóa dấu ngoặc thừa ở đây

        private IEnumerator TimerDec(int seconds)
        {
            if (timerStart)
            {
                MartBidControl.currentTimer -= seconds;
                timerChecked = true;
            }

            yield return new WaitForSeconds(seconds);

            timerChecked = false;
        }

        private IEnumerator WaitForSec(int seconds)
        {
            yield return new WaitForSeconds(seconds);

            bidResult = false;
        }

        private IEnumerator WaitFor(int level)
        {
            yield return new WaitForSeconds(1.0f);

            if (level == 10)
            {
                Application.Quit();
            }
            else
            {
                GameController.Instance().ResetMenus();

                if (level == 3)
                    GameController.Instance().loadPlayer = true;

                SceneManager.LoadScene(level);
            }
        }
    }
}