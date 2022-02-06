using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Mkey
{
    public enum modeRot { x, y };
    //[ExecuteInEditMode]
    public class RotateAvatar : MonoBehaviour
    {
        public modeRot mode;

        public RectTransform Group;
        public RectTransform maskRect;
        float groupAngleZ;
        public bool topHalf;
        public bool leftHalf;
        public float acceler = 2.0f;

        private LevelButton activeButton;
        private Canvas mainCanvas;

        

        void Start()
        {
            Group = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (mode == modeRot.y)
            {
                activeButton = MapController.Instance.ActiveButton;
                if (!mainCanvas) mainCanvas = MapController.Instance.parentCanvas;
            }
            

            if (activeButton && mainCanvas)
            {
                if (mode == modeRot.y)
                {
                    Group.transform.position = activeButton.transform.position;
                    //  Group.anchoredPosition = Coordinats.RectTransformToCanvasSpaceCenterCenter(activeButton.GetComponent<RectTransform>(), MapController.Instance.parentCanvas);
                    topHalf = Group.anchoredPosition.y > 0;

                    groupAngleZ = (topHalf) ? Mathf.LerpAngle(groupAngleZ, 180, Time.deltaTime * 2.0f) : Mathf.LerpAngle(groupAngleZ, 0, Time.deltaTime * 2.0f);
                    maskRect.localRotation = Quaternion.Euler(-Group.localRotation.eulerAngles);
                    Group.localRotation = Quaternion.Euler(new Vector3(0, 0, groupAngleZ + 5f * Mathf.Sin(2f * Time.time)));
                }
                else
                {
                    Group.transform.position = activeButton.transform.position;
                    //  Group.anchoredPosition = Coordinats.RectTransformToCanvasSpaceCenterCenter(activeButton.GetComponent<RectTransform>(), MapController.Instance.parentCanvas);
                    leftHalf = Group.anchoredPosition.x < 0;

                    groupAngleZ = (leftHalf) ? Mathf.LerpAngle(groupAngleZ, 90, Time.deltaTime * 2.0f) : Mathf.LerpAngle(groupAngleZ, 0, Time.deltaTime * 2.0f);
                    maskRect.localRotation = Quaternion.Euler(-Group.localRotation.eulerAngles);
                    Group.localRotation = Quaternion.Euler(new Vector3(0, 0, groupAngleZ + 5f * Mathf.Sin(2f * Time.time)));
                }
            }
        }
    }
}
