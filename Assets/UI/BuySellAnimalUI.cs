using UnityEngine;
using System.Collections;

namespace IrishFarmSim
{
	public class BuySellAnimalUI : MonoBehaviour
	{
		public GameObject buySellAnimalUI;
		public GameObject bidUI;
		public GameObject sellUI;

		void OnTriggerEnter(Collider other)
		{
			// Hiển thị UI mua bán khi player vào khu vực
			if (other.gameObject.name.Equals("Player"))
			{
				buySellAnimalUI.SetActive(true);
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.gameObject.name.Equals("Player"))
			{
				// Ẩn tất cả UI khi player rời khỏi khu vực
				buySellAnimalUI.SetActive(false);
				bidUI.SetActive(false);
				sellUI.SetActive(false);
			}
		}

		public void onPressBuy()
		{
			// Hiển thị UI đấu giá
			buySellAnimalUI.SetActive(false);
			bidUI.SetActive(true);
		}

		public void onPressSell()
		{
			// Hiển thị UI bán vật
			buySellAnimalUI.SetActive(false);
			sellUI.SetActive(true);

			Debug.Log("onPressSell() called - Trying to show animal list");

			// Tự động hiển thị danh sách Cow khi sellUI được activate
			// Tìm ListAnimal trên sellUI hoặc con của nó
			ListAnimal listAnimal = sellUI.GetComponent<ListAnimal>();
			if (listAnimal == null)
			{
				// Nếu không tìm thấy, tìm ở con của sellUI
				listAnimal = sellUI.GetComponentInChildren<ListAnimal>();
			}

			if (listAnimal != null)
			{
				Debug.Log("ListAnimal component found!");
				listAnimal.ShowAnimalListByType("Cow");
			}
			else
			{
				Debug.LogError("ListAnimal component NOT found on sellUI or its children! Make sure to attach ListAnimal script.");
			}
		}
	}
}
