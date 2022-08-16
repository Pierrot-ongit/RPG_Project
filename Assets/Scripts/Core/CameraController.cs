using UnityEngine;
using Cinemachine;
using RPG.Control;

namespace RPG.Core
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] GameObject freeLookCamera;
        CinemachineFreeLook donkey;
        PlayerController playerControllerScript;

        private void Awake()
        {
            donkey = freeLookCamera.GetComponent<CinemachineFreeLook>();
            playerControllerScript = GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (playerControllerScript.isDraggingUI) return;

                // use the following line for mouse control of zoom instead of mouse wheel
                // be sure to change Input Axis Name on the Y axis to Mouse Y
                //donkey.m_YAxis.m_MaxSpeed = 10;
                donkey.m_XAxis.m_MaxSpeed = 500;
            }
            if (Input.GetMouseButtonUp(1))
            {
                // use the following line for mouse control of zoom instead of mouse wheel
                // be sure to change Input Axis Name on the Y axis to Mouse Y
                //donkey.m_YAxis.m_MaxSpeed = 0;
                donkey.m_XAxis.m_MaxSpeed = 0;
            }

            // wheel zoom //
            // comment out if using mouse control for  zoom
            /*
            if (Input.mouseScrollDelta.y != 0)
            {
                Debug.Log(Input.mouseScrollDelta);
                donkey.m_YAxis.m_MaxSpeed = 10;
            }
            */
        }
    }
}