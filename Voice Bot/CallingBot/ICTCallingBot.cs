using Microsoft.Bot.Builder.Calling;
using Microsoft.Bot.Builder.Calling.Events;
using Microsoft.Bot.Builder.Calling.ObjectModel.Contracts;
using Microsoft.Bot.Builder.Calling.ObjectModel.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voice_Bot
{
    public class ICTCallingBot : ICallingBot
    {
        public ICallingBotService CallingBotService
        {
            get; private set;
        }

        public ICTCallingBot(ICallingBotService callingBotService)
        {
            if (callingBotService == null)
                throw new ArgumentNullException(nameof(callingBotService));

            CallingBotService = callingBotService;
			

			if (callingBotService == null)
				throw new ArgumentNullException(nameof(callingBotService));

			CallingBotService = callingBotService;

			CallingBotService.OnIncomingCallReceived += OnIncomingCallReceived;
			CallingBotService.OnPlayPromptCompleted += OnPlayPromptCompleted;
			CallingBotService.OnRecordCompleted += OnRecordCompleted;
			CallingBotService.OnHangupCompleted += OnHangupCompleted;


		}

		private List<string> response = new List<string>();

		int silenceTimes = 0;

		bool sttFailed = false;

		private Task OnIncomingCallReceived(IncomingCallEvent incomingCallEvent)
        {
            var id = Guid.NewGuid().ToString();
            incomingCallEvent.ResultingWorkflow.Actions = new List<ActionBase>
                {
                    new Answer { OperationId = id },
                    GetPromptForText("Welcome to ICT call bot!\n" +
									 "How can we help?")
				};

            return Task.FromResult(true);
        }

        private Task OnPlayPromptCompleted(PlayPromptOutcomeEvent playPromptOutcomeEvent)
        {
            playPromptOutcomeEvent.ResultingWorkflow.Actions = new List<ActionBase>
            {
                CreateIvrOptions("Press 1 to record your voice, Press 2 to hangup.", 2, false)
            };
            return Task.FromResult(true);
        }

        private Task OnRecognizeCompleted(RecognizeOutcomeEvent recognizeOutcomeEvent)
        {
            switch (recognizeOutcomeEvent.RecognizeOutcome.ChoiceOutcome.ChoiceName)
            {
                case "1":
                    var id = Guid.NewGuid().ToString();

                    var prompt = GetPromptForText("Record your message!");
                    var record = new Record
                    {
                        OperationId = id,
                        PlayPrompt = prompt,
                        MaxDurationInSeconds = 10,
                        InitialSilenceTimeoutInSeconds = 5,
                        MaxSilenceTimeoutInSeconds = 2,
                        PlayBeep = true,
                        StopTones = new List<char> { '#' }
                    };
                    recognizeOutcomeEvent.ResultingWorkflow.Actions = new List<ActionBase> { record };
                    break;
                case "2":
                    recognizeOutcomeEvent.ResultingWorkflow.Actions = new List<ActionBase>
                    {
                        GetPromptForText("Goodbye!"),
                        new Hangup { OperationId = Guid.NewGuid().ToString() }
                    };
                    break;
                default:
                    recognizeOutcomeEvent.ResultingWorkflow.Actions = new List<ActionBase>
                    {
                        CreateIvrOptions("Press 1 to record your voice, Press 2 to hangup.", 2, false)
                    };
                    break;
            }
            return Task.FromResult(true);
        }

        private async Task OnRecordCompleted(RecordOutcomeEvent recordOutcomeEvent)
        {
			if (recordOutcomeEvent.RecordOutcome.Outcome == Outcome.Success)
			{
				var record = await recordOutcomeEvent.RecordedContent;
				BingSpeech bs =
					new BingSpeech(recordOutcomeEvent.ConversationResult, t => response.Add(t), s => sttFailed = s);
				bs.CreateDataRecoClient();
				bs.SendAudioHelper(record);
				recordOutcomeEvent.ResultingWorkflow.Actions =
					new List<ActionBase>
					{
						GetSilencePrompt()
					};
			}
			else
			{
				if (silenceTimes > 1)
				{
					recordOutcomeEvent.ResultingWorkflow.Actions =
						new List<ActionBase>
						{
					GetPromptForText("Thank you for calling"),
					new Hangup()
					{
						OperationId = Guid.NewGuid().ToString()
					}
						};
					recordOutcomeEvent.ResultingWorkflow.Links = null;
					silenceTimes = 0;
				}
				else
				{
					silenceTimes++;
					recordOutcomeEvent.ResultingWorkflow.Actions =
						new List<ActionBase>
						{
					GetRecordForText("I didn't catch that, would you kinly repeat?")
						};
				}
			}
		}

        private Task OnHangupCompleted(HangupOutcomeEvent hangupOutcomeEvent)
        {
            hangupOutcomeEvent.ResultingWorkflow = null;
            return Task.FromResult(true);
        }

        private static Recognize CreateIvrOptions(string textToBeRead, int numberOfOptions, bool includeBack)
        {
            if (numberOfOptions > 9)
                throw new Exception("too many options specified");

            var id = Guid.NewGuid().ToString();
            var choices = new List<RecognitionOption>();
            for (int i = 1; i <= numberOfOptions; i++)
            {
                choices.Add(new RecognitionOption { Name = Convert.ToString(i), DtmfVariation = (char)('0' + i) });
            }
            if (includeBack)
                choices.Add(new RecognitionOption { Name = "#", DtmfVariation = '#' });
            var recognize = new Recognize
            {
                OperationId = id,
                PlayPrompt = GetPromptForText(textToBeRead),
                BargeInAllowed = true,
                Choices = choices
            };

            return recognize;
        }

        private static PlayPrompt GetPromptForText(string text)
        {
            var prompt = new Prompt { Value = text, Voice = VoiceGender.Male };
            return new PlayPrompt { OperationId = Guid.NewGuid().ToString(), Prompts = new List<Prompt> { prompt } };
        }

		private static PlayPrompt GetPromptForText(List<string> text)
		{
			var prompts = new List<Prompt>();
			foreach (var txt in text)
			{
				if (!string.IsNullOrEmpty(txt))
					prompts.Add(new Prompt { Value = txt, Voice = VoiceGender.Female });
			}
			if (prompts.Count == 0)
				return GetSilencePrompt(1000);
			return new PlayPrompt { OperationId = Guid.NewGuid().ToString(), Prompts = prompts };
		}

		private static PlayPrompt GetSilencePrompt(uint silenceLengthInMilliseconds = 3000)
		{
			var prompt = new Prompt { Value = string.Empty, Voice = VoiceGender.Female, SilenceLengthInMilliseconds = silenceLengthInMilliseconds };
			return new PlayPrompt { OperationId = Guid.NewGuid().ToString(), Prompts = new List<Prompt> { prompt } };
		}

		private ActionBase GetRecordForText(string promptText)
		{
			PlayPrompt prompt;
			if (string.IsNullOrEmpty(promptText))
				prompt = null;
			else
				prompt = GetPromptForText(promptText);
			var id = Guid.NewGuid().ToString();
			return new Record()
			{
				OperationId = id,
				PlayPrompt = prompt,
				MaxDurationInSeconds = 10,
				InitialSilenceTimeoutInSeconds = 5,
				MaxSilenceTimeoutInSeconds = 2,
				PlayBeep = false,
				RecordingFormat = RecordingFormat.Wav,
				StopTones = new List<char> { '#' }
			};
		}
	}
}