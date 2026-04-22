using IrishFarmSim;
using UnityEngine;

public class AddMoney : MonoBehaviour
{
    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.name.Equals ("Player"))
        {
            GameController.Instance().player.cash += 5000;
        }
    }
}
