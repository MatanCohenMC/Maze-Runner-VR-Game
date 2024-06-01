using UnityEngine;
using EzySlice;
using UnityEngine.XR.Interaction.Toolkit;

public class SliceObject : MonoBehaviour
{
    [SerializeField] private Transform[] m_StartSlicePoints;
    [SerializeField] private Transform[] m_EndSlicePoints;
    [SerializeField] private LayerMask m_SliceableLayer;
    [SerializeField] private string m_SliceableLayerName;
    [SerializeField] private VelocityEstimator m_VelocityEstimator;
    [SerializeField] private Material m_CrossSectionMaterial;
    [SerializeField] private float m_CutForce = 10;

    private void FixedUpdate()
    {
        for (int i = 0; i < m_StartSlicePoints.Length; i++)
        {
            bool hasHit = Physics.Linecast(m_StartSlicePoints[i].position, m_EndSlicePoints[i].position, out RaycastHit hit, m_SliceableLayer);

            if (hasHit)
            {
                GameObject target = hit.transform.gameObject;
                Slice(target, m_StartSlicePoints[i], m_EndSlicePoints[i]);
            }
        }
    }

    public void Slice(GameObject i_Target, Transform i_StartSlicePoint, Transform i_EndSlicePoint)
    {
        Vector3 velocity = m_VelocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(i_EndSlicePoint.position - i_StartSlicePoint.position, velocity);

        planeNormal.Normalize();

        SlicedHull hull = i_Target.Slice(i_EndSlicePoint.position, planeNormal);

        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(i_Target, m_CrossSectionMaterial);
            setupSlicedComponent(upperHull);
            GameObject lowerHull = hull.CreateLowerHull(i_Target, m_CrossSectionMaterial);
            setupSlicedComponent(lowerHull);
            Destroy(i_Target);
        }
    }

    private void setupSlicedComponent(GameObject i_SlicedObject)
    {
        Rigidbody rb = i_SlicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = i_SlicedObject.AddComponent<MeshCollider>();

        i_SlicedObject.AddComponent<XRGrabInteractable>();
        collider.convex = true;
        i_SlicedObject.layer = LayerMask.NameToLayer(m_SliceableLayerName);
        rb.AddExplosionForce(m_CutForce, i_SlicedObject.transform.position, 1);
    }
}