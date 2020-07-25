using System;
using System.Threading;
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

            WednesdayInvestigator.ItIsWednesdayEvent += Bot_OnWednesday;
            Thread wednesdayInvestigationThread = WednesdayInvestigator.BeginInvestigation();

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            Thread.CurrentThread.Name = "Main Thread";
            //Console.WriteLine("I am Main Thread " + Thread.CurrentThread.Name);

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();

            // Bot Command Menu
            uint input = 0;

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
                    case 0:
                        break;
                    case 1:
                        Subscribers.PrintSubscribers();
                        break;
                    case 2:
                        Console.Write("ChatID: ");
                        ChatId id = Console.ReadLine();
                        Console.WriteLine("Message: ");
                        string text = Console.ReadLine();
                        botClient.SendTextMessageAsync(id, text);
                        break;
                    case 3:
                        Subscribers.RemoveAll();
                        break;
                    case 4:
                        Console.WriteLine(WednesdayInvestigator.WhenIsWednesdayMyDude());
                        break;
                }

            } while (input != 0);

            wednesdayInvestigationThread.Interrupt();            
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                // Es handelt sich um eine Textnachricht
                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}. The Message is {e.Message.Text}");
                //Console.WriteLine("I am Thread " + Thread.CurrentThread.Name);

                if (e.Message.Text.StartsWith("/"))
                {
                    // Es wird ein Kommando erwartet
                    switch (e.Message.Text)
                    {
                        case "/start":
                            Subscribers.AddSubscriber(e.Message.Chat.Id);
                            break;
                        case "/unsubscribe":
                            Subscribers.RemoveSubscriber(e.Message.Chat.Id);
                            break;
                        case "/whensdaymydude":
                            await botClient.SendTextMessageAsync(e.Message.Chat, WednesdayInvestigator.WhenIsWednesdayMyDude().ToString());
                            break;
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                      chatId: e.Message.Chat,
                      text: "You said:\n" + e.Message.Text
                    );
                }
            }
        }

        static async void Bot_OnWednesday()
        {
            foreach (ChatId id in Subscribers.chatIDs)
            {
                await botClient.SendTextMessageAsync(id, "IT IS WEDNESDAY, MY DUDE!");
            }
        }
    }
}