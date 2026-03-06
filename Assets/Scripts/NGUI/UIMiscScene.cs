using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace IrishFarmSim
{
	public class UIMiscScene : MonoBehaviour 
	{
		public bool loadAchievements;

		public void OnClick()
		{
			if(loadAchievements)
			{
				OpenA();
			}
			else
			{
				OpenL();
			}
		}
        public void OpenA()
        {
            Debug.Log("Achievements not implemented");
        }

        public void OpenL()
        {
            Debug.Log("Leaderboard not implemented");
        }
    }
}