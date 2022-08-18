using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Mesh Trail Effect", menuName = "RPG/Abilities/Effects/MeshTrailEffect", order = 0)]
    public class MeshTrailEffect : EffectStategy
    {
        [SerializeField] private float activeTime = 2f;

        [Header("Mesh Related")]
        [SerializeField] float meshRefreshRate = 0.1f;
        [SerializeField] private float meshDestroyDelay = 3f;

        [Header("Shader Related")]
        [SerializeField] Material material;
        [SerializeField] private string shaderVarRef;
        [SerializeField] private float shaderVarRate = 0.1f;
        [SerializeField] private float shaderVarRefreshRate = 0.05f;
    
        //private bool isTrailActive = false;
        private Dictionary<int, SkinnedMeshRenderer[]> _skinnedMeshRenderers = new Dictionary<int, SkinnedMeshRenderer[]>();
        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(ActiveTrail(data, finished, activeTime));
        }
        
        private IEnumerator ActiveTrail(AbilityData data, Action finished, float timeActive)
        {
            while (timeActive > 0)
            {
                timeActive -= meshRefreshRate;
                foreach (GameObject target in data.GetTargets().ToArray())
                {
                    int id = target.GetInstanceID();
                    if (!_skinnedMeshRenderers.ContainsKey(id))
                    {
                        _skinnedMeshRenderers[id] = target.GetComponentsInChildren<SkinnedMeshRenderer>();
                    }

                    for (int i = 0; i < _skinnedMeshRenderers[id].Length; i++)
                    {
                        Transform positionToSpawn = _skinnedMeshRenderers[id][i].transform;
                        // TODO Use Graphics.DrawMesh
                        GameObject gObj = new GameObject();
                        gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);
                        MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                        MeshFilter mf = gObj.AddComponent<MeshFilter>();
                        Mesh mesh = new Mesh();
                        // take a snapshot of the mesh at this exact moment. We are recording where each vertex is.
                        _skinnedMeshRenderers[id][i].BakeMesh(mesh);
                        mf.mesh = mesh;
                        mr.material = material;

                        data.StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                        Destroy(gObj, meshDestroyDelay);
                    }

                    yield return new WaitForSeconds(meshRefreshRate);
                }
            }
            finished();
        }

        private IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate)
        {
            float valueToAnimate = mat.GetFloat(shaderVarRef);
            while (valueToAnimate > goal)
            {
                valueToAnimate -= rate;
                mat.SetFloat(shaderVarRef, valueToAnimate);
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}