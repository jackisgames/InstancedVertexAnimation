using System.Threading;
using UnityEngine;

namespace VertexSkinnedAnimation
{
    public class InstancedRender : MonoBehaviour
    {
        [SerializeField]
        private Mesh m_mesh;

        [SerializeField]
        private Material m_material;

        private const int AgentCount = 1000;
        private readonly Matrix4x4[] m_matrices=new Matrix4x4[AgentCount];
        private void Start()
        {
            float spawnRadius = 30;
            for (int i = 0; i < AgentCount; ++i)
            {
                float positionAngle = Random.value * Mathf.PI * 2;
                float distance = 3f + Random.value * spawnRadius;
                m_matrices[i].SetTRS(new Vector3(Mathf.Cos(positionAngle) *distance, 0, Mathf.Sin(positionAngle) * distance), Quaternion.Euler(-90,Random.value*360,0), Vector3.one*Random.Range(.9f,1.1f));
            };
        }
        // Update is called once per frame
        private void Update ()
        {
            Graphics.DrawMeshInstanced(m_mesh, 0, m_material, m_matrices);
        }
    }
}
