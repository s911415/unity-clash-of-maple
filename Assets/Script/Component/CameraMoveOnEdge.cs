using NTUT.CSIE.GameDev.Game;
using NTUT.CSIE.GameDev.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NTUT.CSIE.GameDev.Component
{

    /// <summary>   A camera move on edge. </summary>
    /// From: https://answers.unity.com/questions/275436/move-the-camera-then-the-cursor-is-at-the-screen-e.html
    ///
    public class CameraMoveOnEdge : MonoBehaviour
    {
        public int boundary = 50;
        public int speed = 5;
        public GameObject referenceObject = null;

        private int _screenWidth;
        private int _screenHeight;
        private Camera _camera;


        private void Start()
        {
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            _camera = gameObject.GetComponent<Camera>();

            if (_camera == null)
            {
                throw new System.Exception("You cannot attach this script on non-camera object.");
            }
        }

        private void Update()
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Vector3 orgPos = new Vector3(pos.x, pos.y, pos.z);
            bool anyChange = false;

            if (!SpecialKeyController.GetKeyState(SpecialKeyController.VK_SCROLL))
            {
                if (Input.mousePosition.x > _screenWidth - boundary)
                {
                    pos.x += speed * Time.deltaTime; // move on +X axis
                    anyChange = true;
                }

                if (Input.mousePosition.x < 0 + boundary)
                {
                    pos.x -= speed * Time.deltaTime; // move on -X axis
                    anyChange = true;
                }

                if (Input.mousePosition.y > _screenHeight - boundary)
                {
                    pos.z += speed * Time.deltaTime; // move on +Z axis
                    anyChange = true;
                }

                if (Input.mousePosition.y < 0 + boundary)
                {
                    pos.z -= speed * Time.deltaTime; // move on -Z axis
                    anyChange = true;
                }
            }

            if (anyChange)
            {
                transform.position = pos;

                if (referenceObject != null)
                {
                    Vector3 refPos = _camera.WorldToViewportPoint(referenceObject.transform.position);
                    var refSize = _camera.WorldToViewportPoint(referenceObject.GetComponent<Collider>().bounds.size);

                    if (refPos.x > 0) pos.x = orgPos.x;

                    if (refSize.x < 1) pos.x = orgPos.x;

                    if (refPos.y > 0) pos.z = orgPos.z;

                    if (refSize.y < 1) pos.z = orgPos.z;

                    transform.position = pos;
                }
            }
        }

        private void OnGUI()
        {
            GUI.Box(new Rect((Screen.width / 2) - 140, 5, 280, 25), "Mouse Position = " + Input.mousePosition);
            GUI.Box(new Rect((Screen.width / 2) - 70, Screen.height - 30, 140, 25), "Mouse X = " + Input.mousePosition.x);
            GUI.Box(new Rect(5, (Screen.height / 2) - 12, 140, 25), "Mouse Y = " + Input.mousePosition.y);
        }
        private void OnMouseEnter()
        {
            Debug.Log("Mouse Enter");
        }

        private void OnMouseExit()
        {
            Debug.Log("Mouse Exit");
        }
    }
}