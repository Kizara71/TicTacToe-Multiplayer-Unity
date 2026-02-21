using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
public class GridPositionSensor : MonoBehaviour , IPointerDownHandler
{
    [SerializeField] private int x;
    [SerializeField] private int y;
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down on Grid Position Sensor at position: " + x + ", " + y   );
    }
    
    
    // does not work idk why
    
    // private void OnMouseDown()
    // {
    //     Debug.Log("Clicked !");
    // }
}