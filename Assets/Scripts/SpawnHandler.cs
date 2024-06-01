using UnityEngine;

public class SpawnHandler : MonoBehaviour
{
    [SerializeField] private GameObject m_PrefabObject;
    [SerializeField] private Vector3 m_SpawnPointOffset = Vector3.zero;
    [SerializeField] private float m_Scale = 1f;

    public void Spawn()
    {
        Vector3 spawnerPosition = transform.position;
        GameObject spawnedObject = Instantiate(m_PrefabObject, spawnerPosition + m_SpawnPointOffset, Quaternion.identity);

        spawnedObject.transform.localScale = new Vector3(m_Scale, m_Scale, m_Scale); // change its local scale in x y z format
    }
}