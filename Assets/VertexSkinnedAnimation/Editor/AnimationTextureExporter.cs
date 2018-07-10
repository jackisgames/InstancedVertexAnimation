using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VertexSkinnedAnimation
{
    public class AnimationTextureExporter : EditorWindow
    {
        [MenuItem("Window/AnimationTextureExporter")]
        private static void ShowWindow()
        {
            GetWindow<AnimationTextureExporter>().Show();
        }

        private Mesh m_mesh;
        private Animator m_animator;
        private Texture2D m_resultTexture;

        private void OnEnable()
        {
            if (m_animator == null)
                m_animator = FindObjectOfType<Animator>();
        }
        private void OnGUI()
        {
            GUILayout.Box(m_resultTexture,GUILayout.Height(200));
            m_animator = (Animator)EditorGUILayout.ObjectField("Animator", m_animator, typeof(Animator), true);

            if (GUILayout.Button("Generate"))
            {
                if (m_mesh == null)
                    m_mesh = new Mesh();

                if (m_resultTexture != null)
                {
                    DestroyImmediate(m_resultTexture);
                }
                m_animator.Rebind();

                SkinnedMeshRenderer skinnedMesh = m_animator.GetComponentInChildren<SkinnedMeshRenderer>();

                AnimatorClipInfo[] animInfo=m_animator.GetCurrentAnimatorClipInfo(0);
                AnimationClip clip = animInfo[0].clip;
                int targetFrame = Mathf.RoundToInt(clip.frameRate*clip.length*10);
                int hash = Animator.StringToHash("Sample");

                m_resultTexture = new Texture2D(skinnedMesh.sharedMesh.vertexCount, targetFrame,TextureFormat.RGB24,false);
                
                List<Vector3> verts=new List<Vector3>(skinnedMesh.sharedMesh.vertexCount);

                float scale = 6;
                for (int y = 0; y <= targetFrame; y++)
                {
                    m_animator.Play(hash, 0, (float)y/targetFrame);
                    m_animator.Update(0);
                    skinnedMesh.BakeMesh(m_mesh);
                    m_mesh.GetVertices(verts);
                    for (int x = 0; x < verts.Count; x++)
                    {
                        Vector3 vec = verts[x]/scale;
                        m_resultTexture.SetPixel(x, y, new Color(.5f + vec.x, .5f + vec.y, .5f + vec.z));
                    }
                }
                m_resultTexture.Apply();

                //File.m_resultTexture.EncodeToPNG()
                Debug.Log("num frame: "+targetFrame);
                Debug.Log("num verts: "+ verts.Count);
                File.WriteAllBytes(Path.Combine(Application.dataPath,"Result.png"),m_resultTexture.EncodeToPNG());
                AssetDatabase.Refresh();

            }
        }
    }
}
