using UnityEngine;

public class ArmAim : MonoBehaviour
{
    public Transform player; // Parent/player som armen er child af

    void Update()
    {
        if (player == null || Camera.main == null) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // Konverter musens position til playerens lokale rum
        // Så følger aim automatisk player flip
        Vector3 localMouse = player.InverseTransformPoint(mouseWorld);

        // Retning fra armens lokale position til musen i playerens lokale rum
        Vector3 localDirection = localMouse - transform.localPosition;
        localDirection.z = 0f;

        float angle = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;

        // 180 graders aim foran karakteren
        angle = Mathf.Clamp(angle, -90f, 90f);

        // VIGTIGT: localRotation, ikke world rotation
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}