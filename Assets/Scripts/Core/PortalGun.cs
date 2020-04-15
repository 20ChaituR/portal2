using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : MonoBehaviour
{

    Camera playerCam;

    public Portal bluePortal;
    public Portal orangePortal;

    public float offsetFromWall = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit[] hits = Physics.RaycastAll(new Ray(playerCam.transform.position, playerCam.transform.forward));

            RaycastHit firstHit = new RaycastHit();
            bool collided = false;
            for (int i = 0; i < hits.Length; i++)
            {
                if ((!collided || hits[i].distance < firstHit.distance) && !hits[i].collider.Equals(bluePortal.GetComponent<Collider>()))
                {
                    firstHit = hits[i];
                    collided = true;
                }
            }

            if (collided && firstHit.collider.GetComponent<PortalSurface>() != null)
            {
                bluePortal.transform.position = firstHit.point + offsetFromWall * firstHit.normal;
                bluePortal.transform.rotation = Quaternion.LookRotation(firstHit.normal, playerCam.transform.right);
                bluePortal.embeddedWall = firstHit.collider;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RaycastHit[] hits = Physics.RaycastAll(new Ray(playerCam.transform.position, playerCam.transform.forward));

            RaycastHit firstHit = new RaycastHit();
            bool collided = false;
            for (int i = 0; i < hits.Length; i++)
            {
                if ((!collided || hits[i].distance < firstHit.distance) && !hits[i].collider.Equals(orangePortal.GetComponent<Collider>()))
                {
                    firstHit = hits[i];
                    collided = true;
                }
            }

            if (collided && firstHit.collider.GetComponent<PortalSurface>() != null)
            {
                orangePortal.transform.position = firstHit.point + offsetFromWall * firstHit.normal;
                orangePortal.transform.rotation = Quaternion.LookRotation(-firstHit.normal, -playerCam.transform.right);
                orangePortal.embeddedWall = firstHit.collider;
            }
        }
    }
}
