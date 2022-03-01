using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotTest
{
	class Program
	{
		static TelegramBotClient botClient;

		async static Task Main()
		{
			botClient = new TelegramBotClient(
				// TODO: Insert Bot token
				"INSERT BOT TOKEN HERE!"
			);
			await Subscribers.ReadSubscribersFromFile();

			var wednesdayInvestigator = new WednesdayInvestigator();

			wednesdayInvestigator.ItIsWednesdayEvent += Bot_OnWednesday;
			wednesdayInvestigator.BeginInvestigation();

			var me = await botClient.GetMeAsync();
			Console.WriteLine(
				$"Hello, World! I am user {me.Id} " +
				$"and my name is {me.FirstName}."
			);

			var receiverOptions = new ReceiverOptions
			{
				// receive all update types
				AllowedUpdates = { }
			};

			using var cts = new CancellationTokenSource();
			botClient.StartReceiving(
				HandleUpdateAsync,
				HandleError,
				receiverOptions,
				cts.Token);

			// Bot Command Menu
			uint input = 0U;
			do
			{
				Console.WriteLine("0...Exit");
				Console.WriteLine("1...Show Subscribers");
				Console.WriteLine("2...Send Message");
				Console.WriteLine("3...Remove All Subscribers");
				Console.WriteLine("4...Time until Wednesday");
				input = uint.Parse(Console.ReadLine());

				switch (input)
				{
					case 0U:
						break;
					case 1U:
						Subscribers.PrintSubscribers();
						break;
					case 2U:
						Console.Write("ChatID: ");
						var id = Console.ReadLine();
						Console.WriteLine("Message: ");
						var text = Console.ReadLine();
						await botClient.SendTextMessageAsync(id, text);
						break;
					case 3U:
						previousSubscriberOperation.Wait();
						previousSubscriberOperation = Subscribers.RemoveAll();
						break;
					case 4U:
						Console.WriteLine(
							WednesdayInvestigator.WhenIsWednesdayMyDude()
						);
						break;
				}

			} while (input != 0U);

			cts.Cancel();
			await previousSubscriberOperation;
			wednesdayInvestigator.EndInvestigation();
		}

		private static async Task HandleUpdateAsync(
			ITelegramBotClient client,
			Update update,
			CancellationToken cancellationToken)
		{
			if (update.Type == UpdateType.Message &&
				update.Message!.Type == MessageType.Text)
			{
				var chatId = update.Message.Chat.Id;
				var messageText = update.Message.Text;

				Console.WriteLine($"Received text message: {messageText}");
				await HandleTextMessage(messageText, chatId);
			}
		}

		private static async Task HandleTextMessage(string messageText, long chatId)
		{
			// Es handelt sich um eine Textnachricht
			if (messageText.StartsWith("/"))
			{
				// Es wird ein Kommando erwartet
				switch (messageText)
				{
					case "/start":
					case "/subscribe":
						await previousSubscriberOperation;
						previousSubscriberOperation =
							Subscribers.AddSubscriber(chatId);
						break;
					case "/unsubscribe":
						await previousSubscriberOperation;
						previousSubscriberOperation =
							Subscribers.RemoveSubscriber(chatId);
						break;
					case "/whensdaymydude":
						await HandleWednesdayRequest(chatId);
						break;
				}
			}
			else
			{
				await botClient.SendTextMessageAsync(
				  chatId,
				  $"It is {messageText}, my dude!"
				);
			}
		}

		private static async Task HandleWednesdayRequest(long chatId)
		{
			var time = WednesdayInvestigator.WhenIsWednesdayMyDude();

			if (time == TimeSpan.Zero)
			{
				await botClient.SendTextMessageAsync(
					chatId, "IT IS WEDNESDAY, MY DUDE!");
			}
			else
			{
				var message = $"{time:%d} days, {time:%h} hours, " +
				              $"{time:%m} minutes and {time:%s} seconds";

				await botClient.SendTextMessageAsync(
					chatId,
					$"There are still {message} until Wednesday!");
			}
		}

		private static Task HandleError(
			ITelegramBotClient botClient,
			Exception exception,
			CancellationToken cancellation)
		{
			var ErrorMessage = exception switch
			{
				ApiRequestException apiRequestException
					=> $"Telegram API Error:\n" +
					   $"[{apiRequestException.ErrorCode}]" +
					   $"\n{apiRequestException.Message}",
				_ => exception.ToString()
			};

			Console.Error.WriteLine(ErrorMessage);
			return Task.CompletedTask;
		}

		private static void Bot_OnWednesday()
		{
			var sendTasks = new List<Task<Message>>();
			foreach (var id in Subscribers.ChatIDs)
			{
				sendTasks.Add(
					botClient.SendTextMessageAsync(
						id,
						"IT IS WEDNESDAY, MY DUDE!"
					)
				);
			}

			Task.WaitAll(sendTasks.ToArray());
		}

		private static Task previousSubscriberOperation = Task.CompletedTask;
	}
}
