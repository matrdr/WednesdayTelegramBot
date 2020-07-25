using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Telegram.Bot
{
    /// <summary>
    /// Verantwortlich für das Speichern der Abonenten
    /// </summary>
    static class Subscribers
    {
        /// <summary>
        /// Fügt einen neuen Subscriber hinzu
        /// </summary>
        /// <param name="chatId">Chat ID des hinzuzufügenden chats</param>
        public static void AddSubscriber(Telegram.Bot.Types.ChatId chatId)
        {
            chatIDs.Add(chatId);

            new Thread(new ThreadStart(WriteSubscribersToFile)).Start();
        }

        /// <summary>
        /// Entfernt einen Subscriber von der Liste
        /// </summary>
        /// <param name="chatId">Chat ID des zu entfernenden Chats</param>
        public static void RemoveSubscriber(Telegram.Bot.Types.ChatId chatId)
        {
            chatIDs.Remove(chatId);

            new Thread(new ThreadStart(WriteSubscribersToFile)).Start();
        }

        /// <summary>
        /// Gibt alle aktuellen Abonnenten auf der Konsole aus
        /// </summary>
        public static void PrintSubscribers()
        {
            foreach (var chatId in chatIDs)
            {
                Console.WriteLine(chatId);
            }
        }

        /// <summary>
        /// Schreibt alle aktuellen Abonnenten in eine Datei
        /// </summary>
        private static void WriteSubscribersToFile()
        {
            StreamWriter sw = new StreamWriter("subscribers.txt");

            foreach (var chatId in chatIDs)
            {
                sw.WriteLine(chatId);
            }

            sw.Close();
        }

        /// <summary>
        /// Liest alle aktuellen Abonnenten aus einer Datei
        /// </summary>
        public static void ReadSubscribersFromFile()
        {
            StreamReader sr = null;

            try
            {
                sr = new StreamReader("subscribers.txt");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Created new Subscriber File");
                StreamWriter sw = new StreamWriter("subscribers.txt");
                sw.Close();

                sr = new StreamReader("subscribers.txt");
            }

            string line = "";

            while ((line = sr.ReadLine()) != null)
            {
                chatIDs.Add(long.Parse(line));
            }

            sr.Close();
        }

        public static void RemoveAll()
        {
            chatIDs.Clear();

            WriteSubscribersToFile();
        }

        /// <summary>
        /// Enthält die ChatID jedes Abonennten ein Mal
        /// </summary>
        public static SortedSet<Telegram.Bot.Types.ChatId> chatIDs { get; } = new SortedSet<Telegram.Bot.Types.ChatId>(new ChatIdComparer());
    }

    /// <summary>
    /// Vergleich zweier ChatIDs
    /// </summary>
    class ChatIdComparer : IComparer<Telegram.Bot.Types.ChatId>
    {
        public int Compare(Telegram.Bot.Types.ChatId x, Telegram.Bot.Types.ChatId y)
        {
            return x.Identifier.CompareTo(y.Identifier);
        }
    }
}
