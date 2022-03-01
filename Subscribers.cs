using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Telegram.Bot
{
	/// <summary>
	/// Verantwortlich für das Speichern der Abonnenten
	/// </summary>
	static class Subscribers
	{
		/// <summary>
		/// Fügt einen neuen Subscriber hinzu
		/// </summary>
		/// <param name="chatId">Chat ID des hinzuzufügenden chats</param>
		public static async Task AddSubscriber(Types.ChatId chatId)
		{
			ChatIDs.Add(chatId);

			await WriteSubscribersToFile();
		}

		/// <summary>
		/// Entfernt einen Subscriber von der Liste
		/// </summary>
		/// <param name="chatId">Chat ID des zu entfernenden Chats</param>
		public static async Task RemoveSubscriber(Types.ChatId chatId)
		{
			ChatIDs.Remove(chatId);

			await WriteSubscribersToFile();
		}

		/// <summary>
		/// Gibt alle aktuellen Abonnenten auf der Konsole aus
		/// </summary>
		public static void PrintSubscribers()
		{
			foreach (var chatId in ChatIDs)
			{
				Console.WriteLine(chatId);
			}
		}

		/// <summary>
		/// Schreibt alle aktuellen Abonnenten in eine Datei
		/// </summary>
		private static async Task WriteSubscribersToFile()
		{
			using var sw = new StreamWriter("subscribers.txt");

			foreach (var chatId in ChatIDs)
			{
				await sw.WriteLineAsync(chatId);
			}
		}

		/// <summary>
		/// Liest alle aktuellen Abonnenten aus einer Datei
		/// </summary>
		public static async Task ReadSubscribersFromFile()
		{
			if (!File.Exists("subscribers.txt"))
			{
				return;
			}

			var text = await File.ReadAllLinesAsync("subscribers.txt");
			foreach (var line in text)
			{
				ChatIDs.Add(long.Parse(line));
			}
		}

		public static async Task RemoveAll()
		{
			ChatIDs.Clear();

			await WriteSubscribersToFile();
		}

		/// <summary>
		/// Enthält die ChatID jedes Abonennten ein Mal
		/// </summary>
		public static SortedSet<Types.ChatId> ChatIDs { get; } =
			new SortedSet<Types.ChatId>(new ChatIdComparer());
	}

	/// <summary>
	/// Vergleich zweier ChatIDs
	/// </summary>
	class ChatIdComparer : IComparer<Types.ChatId>
	{
		public int Compare(Types.ChatId x, Types.ChatId y)
		{
			Debug.Assert(x.Identifier.HasValue && y.Identifier.HasValue);
			return x.Identifier.Value.CompareTo(y.Identifier.Value);
		}
	}
}
