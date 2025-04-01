using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using Whisper.Utils;
using Whisper;
using TMPro;
using UnityEngine.UI;
using Utils;
using Enum;

namespace Managers 
{ 
    /// <summary>
    /// Manages the voice command controllers
    /// </summary>
    public class VoiceCommandsManager : MonoBehaviour
    {
        #region fields
        private static VoiceCommandsManager instance;

        private string recordText = "Click to speak commands";
        private string stopRecordingText = "Stop recording";
        private string listeningText = "Listening...";
        private string processingText = "Processing...";
        private string commandSpeechText = "COMMAND: ";

        private ButtonMenu buttonMenu;

        // handle speech Recognizion
        [SerializeField]
        private WhisperManager whisper;
        [SerializeField]
        public MicrophoneRecord microphoneRecord;

        [Header("UI that handles Speech")]
        [SerializeField]
        private Button button;
        [SerializeField]
        private TMP_Text buttonText;
        [SerializeField]
        private TMP_Text text;
        [SerializeField]
        private WhisperStream stream;

        #endregion

        #region properties

        /// <summary>
        /// Property to access the Singleton instance of VoiceCommandsManager
        /// </summary>
        public static VoiceCommandsManager Instance
        {
            get { return instance; }
        }

        #endregion

        #region private methods

        private void Awake()
        {
            // Set the current instance as the singleton or destroy case duplicated
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }

            buttonMenu = button.GetComponent<ButtonMenu>();

            if (buttonMenu == null)
            {
                Debug.LogError("Button Menu not found in speech button");
            }
        }

       
        // Start is called before the first frame update
        private async void Start()
        {
            // Check if permission is granted
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                // Request permission
                Permission.RequestUserPermission(Permission.Microphone);
            }

            // set whisper and listners 
            stream = await whisper.CreateStream(microphoneRecord);
            stream.OnResultUpdated += OnResult;
            stream.OnSegmentUpdated += OnSegmentUpdated;
            stream.OnSegmentFinished += OnSegmentFinished;
            stream.OnStreamFinished += OnFinished;

            microphoneRecord.OnRecordStop += OnRecordStop;
            button.onClick.AddListener(OnButtonPressed);
        }

        /// <summary>
        /// Handles when record button is clicked
        /// </summary>
        private void OnButtonPressed()
        {
            // click in button 
            buttonMenu.ClickButton();

            // TODO: after 3 seconds, stop recording

            if (!microphoneRecord.IsRecording)
            {
                text.text = "";
                stream.StartStream();
                microphoneRecord.StartRecord();
            }
            else
            {
                microphoneRecord.StopRecord();
            }
                

            buttonText.text = microphoneRecord.IsRecording ? stopRecordingText : recordText;
        }

        private void OnRecordStop(AudioChunk recordedAudio)
        {
            buttonText.text = recordText;
        }

        /// <summary>
        /// Handle the result of speech recognition 
        /// </summary>
        /// <param name="result"></param>
        private void OnResult(string result)
        {
            // clean the result text
            text.text = commandSpeechText + " " + result;
            string resultClean = result.Replace("!","").Replace("?","").Replace(".","").ToLower();

            InteractionVoice? interactionVoiceRecognized = null; 

            // handle result to command
            if (resultClean.Contains("rotate"))
            {
                interactionVoiceRecognized = InteractionVoice.Rotate;
            } else if (resultClean.Contains("scale"))
            {
                interactionVoiceRecognized = InteractionVoice.Scale;
            } else if (resultClean.Contains("forward"))
            {
                interactionVoiceRecognized = InteractionVoice.Forward;
            }

            // communicate result if any command recognized
            if (interactionVoiceRecognized.HasValue)
            {
                ARInteractionsManager.Instance.InteractWithObject(interactionVoiceRecognized);
            }
            
        }

        private void OnSegmentUpdated(WhisperResult segment)
        {
            
        }

        private void OnSegmentFinished(WhisperResult segment)
        {

        }

        private void OnFinished(string finalResult)
        {

        }

        #endregion
    }
}
