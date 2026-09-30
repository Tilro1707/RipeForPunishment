using UnityEngine;

public class BodySwing : MonoBehaviour
{

    [SerializeField] private HingeJoint2D hinge;
    [SerializeField] private Transform bodyAnchor;
    private void FixedUpdate()
    {
        if (hinge.connectedBody != null)
        {

            hinge.connectedAnchor = hinge.connectedBody.transform.InverseTransformPoint(bodyAnchor.position);
        }
        else
        {
            hinge.connectedAnchor = bodyAnchor.position;
        }
    }
}
