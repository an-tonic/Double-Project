
using UnityEngine;

public class TeleportationManager : MonoBehaviour
{
    public Transform xrOrigin;
    public Transform TP_Line;
    public LayerMask teleportLayer;

    public void TeleportPlayer()
    {
        
        Ray ray = new Ray(TP_Line.position, TP_Line.up);
        

        if (Physics.Raycast(ray, out RaycastHit hitInfo, TP_Line.localScale.y, teleportLayer))
        {
            
            if (hitInfo.collider.CompareTag("TeleportSurface"))
            {
                
                xrOrigin.position = hitInfo.point;
                
            }
        }
    }
}
