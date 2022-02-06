using UnityEngine.UI;
using System;
using UnityEngine;
/*
 changes
    20.02.19 basic

 */
namespace Mkey
{
    public enum MessageAnswer { Yes, Cancel, No, None }
    public class WarningMessController : PopUpsController
    {
        [SerializeField]
        private Text caption;
        [SerializeField]
        private Text message;
        [SerializeField]
        private Button yesButton;
        [SerializeField]
        private Button noButton;
        [SerializeField]
        private Button cancelButton;

        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchGUIController MGui { get { return MatchGUIController.Instance; } }

        private MessageAnswer answer = MessageAnswer.None;
        public MessageAnswer Answer
        {
            get { return answer; }
        }

        public void Cancel_Click()
        {
            answer = MessageAnswer.Cancel;
            CloseWindow();
        }

        public void Open_4chest_Click()
        {
            if (MPlayer.Fortune > 0)
                MGui.ShowFortune(null);
            else
                MGui.ShowNonFortune(null);
            CloseWindow();
        }
        public void Cancel_4chest_Click()
        {
            MPlayer.statChest = 0;
            answer = MessageAnswer.Cancel;
            CloseWindow();
        }

        public void Yes_Click()
        {
            answer = MessageAnswer.Yes;
            #if UNITY_EDITOR
                 Debug.Log("PEREXOD HA CAUT Google Play");
#elif UNITY_ANDROID
                 Application.OpenURL("market://details?id=com.WhiteSarcazm.ForestMatch3");
#elif UNITY_IPHONE
                 Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif
            MPlayer.RateMove(); 
            
            CloseWindow();
        }

        public void YesCheckNew_Click()
        {
            answer = MessageAnswer.Yes;
#if UNITY_EDITOR
            Debug.Log("PEREXOD HA CAUT Google Play");
#elif UNITY_ANDROID
                 Application.OpenURL("market://details?id=com.WhiteSarcazm.ForestMatch3");
#elif UNITY_IPHONE
                 Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif
            MPlayer.RateMove();

            CloseWindow();
            SceneLoader.Instance.LoadScene(0);
        }

        public void No_Click()
        {
            answer = MessageAnswer.No;
            CloseWindow();
        }

        public void NoCheckNew_Click()
        {
            answer = MessageAnswer.No;
            CloseWindow();
            SceneLoader.Instance.LoadScene(0);
        }

        public void Show_Fortune()
        {
            MGui.ShowFortune(null);
        }

        public string Caption
        {
            get { if (caption) return caption.text; else return string.Empty; }
            set { if (caption) caption.text = value; }
        }

        public string Message
        {
            get { if (message) return message.text; else return string.Empty; }
            set { if (message) message.text = value; }
        }

        internal void SetMessage(string caption, string message, bool yesButtonActive, bool cancelButtonActive, bool noButtonActive)
        {
            Caption = caption;
            Message = message;
            yesButton.gameObject.SetActive(yesButtonActive);
            cancelButton.gameObject.SetActive(cancelButtonActive);
            noButton.gameObject.SetActive(noButtonActive);
        }
    }
}