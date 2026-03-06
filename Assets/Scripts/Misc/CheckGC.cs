using UnityEngine;

namespace IrishFarmSim
{
	public class CheckGC : MonoBehaviour {

		void Start()
		{
			CheckGCExists("GameController");
		}
		
		void Awake()
		{
			CheckGCExists("GameController");
		}

		private void CheckGCExists(string gcName)
		{
			// Checks if the game game controller exists
			if(GameObject.Find(gcName) == null)
			{
				GameObject cowGameObject = Instantiate(Resources.Load(gcName) as GameObject);
				cowGameObject.name = gcName;
			}
		}
	}
}