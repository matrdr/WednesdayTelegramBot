# WednesdayTelegramBot
Telegram bot that informs his dudes that it is Wednesday

Written in C# .NET Core using the Telegram.Bot API (https://github.com/TelegramBots/telegram.bot).
Should therefore also work with Linux.

When run and supplied with a bot token, users can subscribe to the bot using the ´/start` command.
They will then receive a message every time it turns Wednesday, informing them about the fact that
it is in fact Wednesday.

Running it on a PC is not really satisfying because every time the computer falls asleep, the internal
timer that sleeps until the next Wednesday is getting out of sync as there is no polling for Wednesdays 
involved for maximum power efficiency :D.

It also has never been tested much (e.g. more than two subscribers) as I didn't want to annoy my 
colleagues with fake Wednesday allerts for testing purpouses.

Feel free to enjoy this bot and from now on never miss a Wednesday again! 
