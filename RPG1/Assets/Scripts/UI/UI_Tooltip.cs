using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Tooltip : MonoBehaviour
{
    [SerializeField] private float xLimit = Screen.width / 2;
    [SerializeField] private float yLimit = Screen.height / 2;

    [SerializeField] private float xOffset = Screen.width / 8;
    [SerializeField] private float yOffset = Screen.height / 8;

    public virtual void AdjustPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        float newXOffset = 0;
        float newYOffset = 0;

        if (mousePosition.x > xLimit)
            newXOffset = -xOffset;
        else
            newXOffset = xOffset;
        if (mousePosition.y > yLimit)
            newYOffset = -yOffset;
        else
            newYOffset = yOffset;
        transform.position = new Vector2(mousePosition.x + newXOffset, mousePosition.y + newYOffset);
    }


}
