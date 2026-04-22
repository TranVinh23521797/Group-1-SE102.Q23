using UnityEngine;
using System.Collections;

namespace IrishFarmSim
{
	public class NewSupplies : MonoBehaviour
	{
		public GameObject newSuppliesUI;
        void FixedUpdate () 
		{
			transform.Rotate (0, 2, 0);
		}

		void OnTriggerEnter(Collider other) 
		{
			if (other.gameObject.name.Equals ("Player"))
			{
				newSuppliesUI.SetActive(true);
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.gameObject.name.Equals ("Player"))
			{
				newSuppliesUI.SetActive(false);
			}
		}
	}
}