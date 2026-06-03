using UnityEngine;
using UnityEngine.UI;

public class Transparency : MonoBehaviour
{
    public Image image;
    public float transparency = 0.6f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
         var tempColor = image.color;
         tempColor.a = transparency;
         image.color = tempColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
