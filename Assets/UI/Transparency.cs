using UnityEngine;
using UnityEngine.UI;

public class Transparency : MonoBehaviour
{
    public Image image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
         var tempColor = image.color;
         tempColor.a = 0.6f;
         image.color = tempColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
