﻿using UnityEngine;
using UnityEngine.UI;

namespace Mkey {
    public class SettingsWindowController : PopUpsController
    {
        [SerializeField]
        private Toggle easyToggle;
        [SerializeField]
        private Toggle hardToggle;
        [SerializeField]
        private Image musikOff;
        [SerializeField]
        private Image soundOff;
        [SerializeField]
        private Button facebookConnectButton;
        [SerializeField]
        private Text facebookButtonText;
        [SerializeField]
        private Text musikOnOff;
        [SerializeField]
        private VolumeSlider volumeSlider;

        [Space(8)]
        [SerializeField]
        private string ANDROID_RATE_URL;
        [SerializeField]
        private string IOS_RATE_URL;

        #region initial
        private float volume;
        private bool musikOn;
        private bool soundOn;
        private bool facebookConnected;
        private HardMode hMode;
        #endregion initial

        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchGUIController MGui { get { return MatchGUIController.Instance; } }
        private MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }
        private FBholder FB { get { return FBholder.Instance; } } 

        #region regular
        private void Start()
        {
            hMode = MPlayer.Hardmode;
            volume = MSound.Volume;
            musikOn = MSound.MusicOn;
            soundOn = MSound.SoundOn;
            facebookConnected = FBholder.IsLogined;

           if(easyToggle) easyToggle.onValueChanged.AddListener((value) =>
            {
                MSound.SoundPlayClick(0, null);
                if (value) { MPlayer.SetHardMode(HardMode.Easy); }
                else { MPlayer.SetHardMode(HardMode.Hard); }
            });
            FBholder.LoginEvent += FacebooLoginHandler;
            FBholder.LogoutEvent += FacebooLogoutHandler;

            if(facebookConnectButton) facebookConnectButton.onClick.RemoveListener(FacebooLoginLogout);
            if (facebookConnectButton) facebookConnectButton.onClick.AddListener(FacebooLoginLogout);
            RefreshWindow();
        }

        private void OnDestroy()
        {
            FBholder.LoginEvent -= FacebooLoginHandler;
            FBholder.LogoutEvent -= FacebooLogoutHandler;
        }
        #endregion regular

        public override void RefreshWindow()
        {
            RefreshSound();
            RefreshHardMode();
            RefreshFacebook();
            base.RefreshWindow();
        }

        public void Save_Click()
        {
            CloseWindow();
        }

        public void Cancel_Click()
        {
            CloseWindow();

            // return initial states
            MPlayer.SetHardMode(hMode);
            MSound.SetVolume(volume);
            MSound.SetMusic(musikOn);
            MSound.SetSound(soundOn);

            if (facebookConnected != FBholder.IsLogined)
            {
                if (facebookConnected) FBholder.Instance.FBlogin();
                else
                {
                    FBholder.Instance.FBlogOut();
                }
            }
        }

        private void RefreshSound()
        {
            if (volumeSlider)volumeSlider.SetVolume(MSound.Volume);
            if (musikOff) musikOff.gameObject.SetActive(!MSound.MusicOn);
            if (soundOff) soundOff.gameObject.SetActive(!MSound.SoundOn);
          //  if (musikOnOff)musikOnOff.text = (!MSound.MusicOn) ? "music" : "music";
        }

        private void RefreshFacebook()
        {
            if (facebookButtonText) facebookButtonText.text = (!FBholder.IsLogined) ? "CONNECT" : "DISCONNECT";
        }

        public void FacebooLoginLogout()
        {
            if (FBholder.IsLogined)
            {
                FBholder.Instance.FBlogOut();
            }
            else
            {
                FBholder.Instance.FBlogin();
            }
        }

        public void FacebooLoginHandler(bool logined, string message)
        {
            if (facebookButtonText) facebookButtonText.text = (!logined) ? "CONNECT" : "DISCONNECT";
            if (logined) MPlayer.AddFbCoins();
        }

        public void FacebooLogoutHandler()
        {
            if (facebookButtonText) facebookButtonText.text = (!FBholder.IsLogined) ? "CONNECT" : "DISCONNECT";
        }

        private void RefreshHardMode()
        {
            if(hardToggle)   hardToggle.isOn = (MPlayer.Hardmode == HardMode.Hard);
            if(easyToggle)  easyToggle.isOn = (MPlayer.Hardmode != HardMode.Hard);
        }

        public void MusikButtonClick()
        {
            MSound.SetMusic(!MSound.MusicOn);
            RefreshSound();
        }

        public void SoundButtonClick()
        {
            MSound.SetSound(!MSound.SoundOn);
            RefreshSound();
        }

        public void FolumeButton_Click(bool plus)
        {
            float currVolume = MatchSoundMaster.Instance.Volume;
            currVolume = (plus) ? currVolume + 0.1f : currVolume - 0.1f;
            currVolume = Mathf.Clamp(currVolume, 0.0f, 1.0f);
            MSound.SetVolume(currVolume);
            if(volumeSlider)  volumeSlider.SetVolume(currVolume);
        }

        public void RateUs_Click()
        {
#if UNITY_ANDROID
            if (!string.IsNullOrEmpty(ANDROID_RATE_URL)) Application.OpenURL(ANDROID_RATE_URL);
#elif UNITY_IOS
            if (!string.IsNullOrEmpty(IOS_RATE_URL)) Application.OpenURL(IOS_RATE_URL);
#else
            if (!string.IsNullOrEmpty(ANDROID_RATE_URL)) Application.OpenURL(ANDROID_RATE_URL);
#endif
        }

        public void Profile_Click()
        {
            MGui.ShowProfile();
        }

    }
}
