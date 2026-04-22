using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace IrishFarmSim
{
public class MartTransitionScript : MartInit
{
    public void ManiMenu()
    {
        GameController.Instance().Save();
        StartCoroutine(WaitFor(0));
    }

    public void Farm()
    {
        GameController.Instance().Save();
        StartCoroutine(WaitFor(3));
    }
    private IEnumerator WaitFor(int level) 
		{
			yield return new WaitForSeconds(1.0f);
			
			if (level == 10) 
			{
				Application.Quit ();	
			}
			else 
			{
				GameController.Instance().ResetMenus();
				
				if(level == 3)
					GameController.Instance().loadPlayer = true;

                SceneManager.LoadScene(level);
            }
		}
}
}