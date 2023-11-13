using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voice_Bot.Dialogs
{
	[Serializable]
	[LuisModel("modelId", "subsKey")]
	public class ICTLuisDialog : LuisDialog<object>
	{


		[LuisIntent("")]
		public async Task None(IDialogContext context, LuisResult result)
		{
			await context.PostAsync("I'm sorry. I didn't understand you.");
			context.Wait(MessageReceived);
		}

		/*[LuisIntent("RentCar")]
		public async Task Rent(IDialogContext context, LuisResult result)
		{
			
			var entities = new List<EntityRecommendation>(result.Entities);
			foreach (var entity in result.Entities)
			{
				switch (entity.Type)
				{
					case PickLocationEntityType:
						entities.Add(new EntityRecommendation(type: nameof(RentForm.PickLocation)) { Entity = entity.Entity });
						break;
					case PickDateEntityType:
						EntityRecommendation pickTime;
						result.TryFindEntity(PickTimeEntityType, out pickTime);
						var pickDateAndTime = entity.Entity + " " + pickTime?.Entity;
						if (!string.IsNullOrWhiteSpace(pickDateAndTime))
							entities.Add(new EntityRecommendation(type: nameof(RentForm.PickDateAndTime)) { Entity = pickDateAndTime });
						break;
					default:
						break;
				}
			}

			var rentForm = new FormDialog<RentForm>(new RentForm(), RentForm.BuildForm, FormOptions.PromptInStart, entities);
			context.Call(rentForm, RentComplete);
		}

		private async Task RentComplete(IDialogContext context, IAwaitable<RentForm> result)
		{
			try
			{
				var form = await result;

				await context.PostAsync($"Your reservation is confirmed");

				context.Wait(MessageReceived);
			}
			catch (Exception e)
			{
				string reply;
				if (e.InnerException == null)
				{
					reply = $"You quit --maybe you can finish next time!";
				}
				else
				{
					reply = "Sorry, I've had a short circuit.  Please try again.";
				}
				await context.PostAsync(reply);
			}
		}*/

		[LuisIntent("About CCE")]
		public async Task AboutCCE(IDialogContext context, LuisResult result) {
			await context.PostAsync("About CCE.");
			context.Wait(MessageReceived);
		}

		[LuisIntent("About CSE")]
		public async Task AboutCSE(IDialogContext context, LuisResult result)
		{
			await context.PostAsync("About CSE.");
			context.Wait(MessageReceived);
		}

		[LuisIntent("About IT")]
		public async Task AboutIT(IDialogContext context, LuisResult result)
		{
			await context.PostAsync("About IT.");
			context.Wait(MessageReceived);
		}

		[LuisIntent("Collaboration")]
		public async Task Collaboration(IDialogContext context, LuisResult result)
		{
			await context.PostAsync("Collaboration.");
			context.Wait(MessageReceived);
		}

		[LuisIntent("Combocard")]
		public async Task Combocard(IDialogContext context, LuisResult result)
		{
			await context.PostAsync("Combocard.");
			context.Wait(MessageReceived);
		}

		[LuisIntent("Companies")]
		public async Task Companies(IDialogContext context, LuisResult result)
		{
			await context.PostAsync("Companies.");
			context.Wait(MessageReceived);
		}

		[LuisIntent("Cutoff")]
		public async Task Cutoff(IDialogContext context, LuisResult result)
		{
			await context.PostAsync("Cutoff.");
			context.Wait(MessageReceived);
		}

		[LuisIntent("Extracurricular")]
		public async Task Extracurricular(IDialogContext context, LuisResult result)
		{
			await context.PostAsync("Extracurricular.");
			context.Wait(MessageReceived);
		}

		[LuisIntent("Faculty")]
		public async Task Faculty(IDialogContext context, LuisResult result)
		{
			await context.PostAsync("Faculty.");
			context.Wait(MessageReceived);
		}

		[LuisIntent("Food")]
		public async Task Food(IDialogContext context, LuisResult result)
		{
			await context.PostAsync("Food.");
			context.Wait(MessageReceived);
		}


	}
}