using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Mkey
{
    public class ContiVideoTimer : MonoBehaviour
    {
        private GlobalTimer gTimer;
        private string videoIncTimerName = "VideoInc";
        [Tooltip("Time span to life increase, minutes")]
        [SerializeField]
        private int videoIncTime = 30;
        [Tooltip("Increase lives if count less than value")]
        [SerializeField]
        private uint incIfLessThan = 1;
        [Tooltip("Calc global time (between games)")]
        [SerializeField]
        private bool calcGlobalTime = true;

        private VideoPurchaser VPurch { get { return VideoPurchaser.Instance; } }

        public static ContiVideoTimer Instance;

        #region properties
        public bool IsWork { get; private set; }
        public float RestMinutes { get; private set; }
        public float RestSeconds { get; private set; }
        public float RestDays { get; private set; }
        public float RestHours { get; private set; }
        #endregion properties

        #region events
        public Action<int, int, int, float> TickRestDaysHourMinSecEvent;
        public UnityEvent TimePassedEvent;
        #endregion events

        #region regular
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            // set life handlers

            calcGlobalTime = true;

            VPurch.VideoRewardComplete += ChangeVideoHandler;

            if ((VPurch?.countVideoConti < incIfLessThan) && !IsWork)
            {
                CreateNewTimerAndStart();
            }
            calcGlobalTime = false;
        }

        void OnDestroy()
        {
            VPurch.VideoRewardComplete -= ChangeVideoHandler;
        }

        void Update()
        {
            if (IsWork)
                gTimer.Update();
        }
        #endregion regular

        #region timerhandlers
        private void TickRestDaysHourMinSecHandler(int d, int h, int m, float s)
        {
            RestDays = d;
            RestHours = h;
            RestMinutes = m;
            RestSeconds = s;
            TickRestDaysHourMinSecEvent?.Invoke(d, h, m, s);
        }

        private void TimePassed()
        {
            if (VPurch.countVideoConti < incIfLessThan)
            {
                VPurch.VideoCountAdd(1);
            }
            TimePassedEvent?.Invoke();
            gTimer.Restart();
        }
        #endregion timerhandlers

        #region player video handlers
        private void ChangeVideoHandler(int count)
        {
            if (count >= incIfLessThan && IsWork)
            {
                RestDays = 0;
                RestHours = 0;
                RestMinutes = 0;
                RestSeconds = 0;
                IsWork = false;
            }
            else if (count < incIfLessThan && !IsWork)
            {
                CreateNewTimerAndStart();
            }
        }

        #endregion player video handlers

        private void CreateNewTimerAndStart()
        {
            //  calcGlobalTime = (MPlayer?.Life < incIfLessThan);   

            //true 
            gTimer = new GlobalTimer(videoIncTimerName, 0, 0, videoIncTime, 0, !calcGlobalTime);
            gTimer.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecHandler;
            gTimer.TimePassedEvent += TimePassed;
            IsWork = true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ContiVideoTimer))]
    public class VideoTimerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (EditorApplication.isPlaying)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Set 0 live"))
                {
                    MatchPlayer.Instance?.SetLifesCount(0);
                }
                if (GUILayout.Button("Inc life"))
                {
                    MatchPlayer.Instance?.AddLifes(1);
                }
                if (GUILayout.Button("Dec life"))
                {
                    MatchPlayer.Instance?.AddLifes(-1);
                }
            }
            else
            {
                GUILayout.Label("Goto play mode for test");
            }
        }
    }
#endif
}

