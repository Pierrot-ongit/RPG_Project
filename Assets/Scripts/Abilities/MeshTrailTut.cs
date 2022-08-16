using System.Collections;
using UnityEngine;

public class MeshTrailTut : MonoBehaviour
{
    [SerializeField] private float activeTime = 2f;

    [Header("Mesh Related")]
    [SerializeField] float meshRefreshRate = 0.1f;
    [SerializeField] private float meshDestroyDelay = 3f;
    //[SerializeField] private Transform positionToSpawn; Bad Idea because NavmeshAgent.
    
    [Header("Shader Related")]
    [SerializeField] Material material;
    [SerializeField] private string shaderVarRef;
    [SerializeField] private float shaderVarRate = 0.1f;
    [SerializeField] private float shaderVarRefreshRate = 0.05f;
    
    private bool isTrailActive;
    private SkinnedMeshRenderer[] _skinnedMeshRenderers;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActiveTrail(activeTime));
        }
        
    }

    private IEnumerator ActiveTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;
            if (_skinnedMeshRenderers == null)
            {
                _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }
             

            for (int i = 0; i < _skinnedMeshRenderers.Length; i++)
            {
                Transform positionToSpawn = _skinnedMeshRenderers[i].transform;
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);
                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();
                Mesh mesh = new Mesh();
                _skinnedMeshRenderers[i].BakeMesh(mesh); // take a snapshot of the mesh at this exact moment. We are recording where each vertex is.
                mf.mesh = mesh;
                mr.material = material;

                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));
                
                Destroy(gObj, meshDestroyDelay);
            }
            
            yield return new WaitForSeconds(meshRefreshRate);
        }
        isTrailActive = false;
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
