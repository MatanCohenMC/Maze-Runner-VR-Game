using UnityEngine;
using System.Collections;

public class VelocityEstimator : MonoBehaviour
{
    [Tooltip("How many frames to average over for computing velocity")]
    [SerializeField] private int m_VelocityAverageFrames = 5;
    [Tooltip("How many frames to average over for computing angular velocity")]
    [SerializeField] private int m_AngularVelocityAverageFrames = 11;
    [SerializeField] private bool m_EstimateOnAwake = true;

    private Coroutine m_Routine;
    private int m_SampleCount;
    private Vector3[] m_VelocitySamples;
    private Vector3[] m_AngularVelocitySamples;
    
    private void Awake()
    {
        m_VelocitySamples = new Vector3[m_VelocityAverageFrames];
        m_AngularVelocitySamples = new Vector3[m_AngularVelocityAverageFrames];

        if (m_EstimateOnAwake)
        {
            BeginEstimatingVelocity();
        }
    }

    public void BeginEstimatingVelocity()
    {
        FinishEstimatingVelocity();
        m_Routine = StartCoroutine(estimateVelocityCoroutine());
    }

    public void FinishEstimatingVelocity()
    {
        if (m_Routine != null)
        {
            StopCoroutine(m_Routine);
            m_Routine = null;
        }
    }

    public Vector3 GetVelocityEstimate()
    {
        // Compute average velocity
        Vector3 velocity = Vector3.zero;
        int velocitySampleCount = Mathf.Min(m_SampleCount, m_VelocitySamples.Length);
        if (velocitySampleCount != 0)
        {
            for (int i = 0; i < velocitySampleCount; i++)
            {
                velocity += m_VelocitySamples[i];
            }
            velocity *= (1.0f / velocitySampleCount);
        }

        return velocity;
    }

    public Vector3 GetAngularVelocityEstimate()
    {
        // Compute average angular velocity
        Vector3 angularVelocity = Vector3.zero;
        int angularVelocitySampleCount = Mathf.Min(m_SampleCount, m_AngularVelocitySamples.Length);
        if (angularVelocitySampleCount != 0)
        {
            for (int i = 0; i < angularVelocitySampleCount; i++)
            {
                angularVelocity += m_AngularVelocitySamples[i];
            }
            angularVelocity *= (1.0f / angularVelocitySampleCount);
        }

        return angularVelocity;
    }

    public Vector3 GetAccelerationEstimate()
    {
        Vector3 average = Vector3.zero;

        for (int i = 2 + m_SampleCount - m_VelocitySamples.Length; i < m_SampleCount; i++)
        {
            if (i < 2)
                continue;

            int first = i - 2;
            int second = i - 1;

            Vector3 v1 = m_VelocitySamples[first % m_VelocitySamples.Length];
            Vector3 v2 = m_VelocitySamples[second % m_VelocitySamples.Length];
            average += v2 - v1;
        }
        average *= (1.0f / Time.deltaTime);
        return average;
    }

    private IEnumerator estimateVelocityCoroutine()
    {
        Vector3 previousPosition = transform.position;
        Quaternion previousRotation = transform.rotation;

        m_SampleCount = 0;

        while (true)
        {
            yield return new WaitForEndOfFrame();

            float velocityFactor = 1.0f / Time.deltaTime;

            int v = m_SampleCount % m_VelocitySamples.Length;
            int w = m_SampleCount % m_AngularVelocitySamples.Length;
            m_SampleCount++;

            // Estimate linear velocity
            m_VelocitySamples[v] = velocityFactor * (transform.position - previousPosition);

            // Estimate angular velocity
            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(previousRotation);

            float theta = 2.0f * Mathf.Acos(Mathf.Clamp(deltaRotation.w, -1.0f, 1.0f));
            if (theta > Mathf.PI)
            {
                theta -= 2.0f * Mathf.PI;
            }

            Vector3 angularVelocity = new Vector3(deltaRotation.x, deltaRotation.y, deltaRotation.z);
            if (angularVelocity.sqrMagnitude > 0.0f)
            {
                angularVelocity = theta * velocityFactor * angularVelocity.normalized;
            }

            m_AngularVelocitySamples[w] = angularVelocity;

            previousPosition = transform.position;
            previousRotation = transform.rotation;
        }
    }
}
