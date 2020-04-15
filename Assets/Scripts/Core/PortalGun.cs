using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : MonoBehaviour
{

    Camera playerCam;

    public Portal bluePortal;
    public Portal orangePortal;

    public float offsetFromWall = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = Camera.main;
    }

    // Gets the first collider a ray hits, ignoring a given collider
    private RaycastHit GetFirstHit(Ray ray, Collider ignore = null)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray);

        RaycastHit firstHit = new RaycastHit();
        bool collided = false;
        for (int i = 0; i < hits.Length; i++)
        {
            if ((!collided || hits[i].distance < firstHit.distance) && !hits[i].collider.Equals(ignore))
            {
                firstHit = hits[i];
                collided = true;
            }
        }

        return firstHit;
    }

    private void ShootPortal(Portal portal, bool blue)
    {
        // Find where the portal shot hits
        RaycastHit firstHit = GetFirstHit(new Ray(playerCam.transform.position, playerCam.transform.forward), portal.GetComponent<Collider>());

        if (!firstHit.Equals(new RaycastHit()) && firstHit.collider.GetComponent<PortalSurface>() != null)
        {
            Quaternion oldRotation = portal.transform.rotation;
            Vector3 oldPosition = portal.transform.position;

            // Move the portal to where the ray hit
            if (blue)
            {
                portal.transform.rotation = Quaternion.LookRotation(firstHit.normal, playerCam.transform.right);
            }
            else
            {
                portal.transform.rotation = Quaternion.LookRotation(-firstHit.normal, -playerCam.transform.right);
            }
            portal.transform.position = firstHit.point + offsetFromWall * firstHit.normal - portal.transform.up * portal.GetComponent<BoxCollider>().size.y / 2;

            // Find where the portal collides in all four directions
            float portalWidth = portal.GetComponent<BoxCollider>().size.y;
            float portalHeight = portal.GetComponent<BoxCollider>().size.x;
            Vector3 portalCenter = portal.transform.position + portal.transform.up * portalWidth / 2;
            Vector3 portalUp = portal.transform.right;
            Vector3 portalRight = portal.transform.up;

            RaycastHit top = GetFirstHit(new Ray(portalCenter, portalUp), portal.GetComponent<Collider>());
            RaycastHit bottom = GetFirstHit(new Ray(portalCenter, -portalUp), portal.GetComponent<Collider>());
            RaycastHit right = GetFirstHit(new Ray(portalCenter, portalRight), portal.GetComponent<Collider>());
            RaycastHit left = GetFirstHit(new Ray(portalCenter, -portalRight), portal.GetComponent<Collider>());

            // Shift the portal so it isn't colliding with anything
            Vector3 verticalShift = Vector3.zero;
            Vector3 horizontalShift = Vector3.zero;
            bool notFit = false;
            if (!bottom.Equals(new RaycastHit()))
            {
                if (bottom.distance < portalHeight / 2)
                {
                    verticalShift = portalUp * (portalHeight / 2 - bottom.distance);
                }
            }
            if (!top.Equals(new RaycastHit()))
            {
                if (top.distance < portalHeight / 2)
                {
                    if (verticalShift == Vector3.zero)
                    {
                        verticalShift = -portalUp * (portalHeight / 2 - top.distance);
                    }
                    else
                    {
                        notFit = true;
                    }
                }
            }
            if (!left.Equals(new RaycastHit()))
            {
                if (left.distance < portalWidth / 2)
                {
                    horizontalShift = portalRight * (portalWidth / 2 - left.distance);
                }
            }
            if (!right.Equals(new RaycastHit()))
            {
                if (right.distance < portalWidth / 2)
                {
                    if (horizontalShift == Vector3.zero)
                    {
                        horizontalShift = -portalRight * (portalWidth / 2 - right.distance);
                    }
                    else
                    {
                        notFit = true;
                    }
                }
            }

            if (notFit)
            {
                portal.transform.position = oldPosition;
                portal.transform.rotation = oldRotation;
            }
            else
            {
                portal.transform.position += verticalShift + horizontalShift;
                portal.embeddedWall = firstHit.collider;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootPortal(bluePortal, true);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ShootPortal(orangePortal, false);
        }
    }
}
