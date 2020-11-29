using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace BotTest
{
    class Program
    {
        static ITelegramBotClient botClient;

        static void Main()
        {
            botClient = new TelegramBotClient(/* INSERT YOUR BOT-TOKEN HERE */);
            Subscribers.ReadSubscribersFromFile();

            var wednesdayInvestigator = new WednesdayInvestigator();

            wednesdayInvestigator.ItIsWednesdayEvent += Bot_OnWednesday;
            wednesdayInvestigator.BeginInvestigation();

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();

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
                        ChatId id = Console.ReadLine();
                        Console.WriteLine("Message: ");
                        string text = Console.ReadLine();
                        botClient.SendTextMessageAsync(id, text);
                        break;
                    case 3U:
                        previousSubscriberOperation.Wait();
                        previousSubscriberOperation = Subscribers.RemoveAll();
                        break;
                    case 4U:
                        Console.WriteLine(WednesdayInvestigator.WhenIsWednesdayMyDude());
                        break;
                }

            } while (input != 0U);

            previousSubscriberOperation.Wait();
            wednesdayInvestigator.EndInvestigation();
        }

        static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                // Es handelt sich um eine Textnachricht
                Console.WriteLine($"Received a text message in chat " +
                    $"{e.Message.Chat.Id}. The Message is {e.Message.Text}");

                if (e.Message.Text.StartsWith("/"))
                {
                    // Es wird ein Kommando erwartet
                    switch (e.Message.Text)
                    {
                        case "/start":
                            previousSubscriberOperation.Wait();
                            previousSubscriberOperation = 
                                Subscribers.AddSubscriber(e.Message.Chat.Id);
                            break;
                        case "/unsubscribe":
                            previousSubscriberOperation.Wait();
                            previousSubscriberOperation =
                                Subscribers.RemoveSubscriber(e.Message.Chat.Id);
                            break;
                        case "/whensdaymydude":
                            botClient
                                .SendTextMessageAsync(
                                    e.Message.Chat, 
                                    WednesdayInvestigator
                                        .WhenIsWednesdayMyDude()
                                        .ToString())
                                .Wait();
                            break;
                    }
                }
                else
                {
                    botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      text: "You said:\n" + e.Message.Text
                    ).Wait();
                }
            }
        }

        static void Bot_OnWednesday()
        {
            var sendTasks = new List<Task<Message>>();
            foreach (ChatId id in Subscribers.ChatIDs)
            {
                sendTasks.Add(
                    botClient.SendTextMessageAsync(id, "IT IS WEDNESDAY, MY DUDE!")
                );
            }

            Task.WaitAll(sendTasks.ToArray());
        }

        private static Task previousSubscriberOperation = Task.CompletedTask;
    }
}
