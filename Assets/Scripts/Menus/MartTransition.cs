using UnityEngine;
using System.Collections;

namespace IrishFarmSim
{
	public class MartTransition : MonoBehaviour
	{
		public GameObject martTransitionUI;
		void FixedUpdate() 
		{
			transform.Rotate(0, 2, 0);
		}

		void OnTriggerEnter(Collider other) 
		{
			if (other.gameObject.name.Equals("Player"))
			{
				martTransitionUI.SetActive(true);
			}
		}
		
		void OnTriggerExit(Collider other)
		{
			if (other.gameObject.name.Equals("Player"))
			{
				martTransitionUI.SetActive(false);
			}
		}
	}
}