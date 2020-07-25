using System;
using System.Threading;

namespace BotTest
{
    class WednesdayInvestigator
    {
        const DayOfWeek DayOfInterest = DayOfWeek.Wednesday;

        /// <summary>
        /// Startet einen Thread, welcher auf jeden Wenesday das ItIsWednesdayEvent auslöst
        /// </summary>
        /// <returns>Gibt Referenz auf den gestarteten Thread zurück, damit dieser vom Nutzer beendet werden kann</returns>
        public static Thread BeginInvestigation()
        {
            Thread wit = new Thread(new ThreadStart(WednesdayInvestigationThread));
            wit.Start();

            return wit;
        }

        /// <summary>
        /// Berechnet die Zeitdauer bis zum nächsten Wednesday
        /// </summary>
        /// <returns>Die Zeit bis zum nächsten Wednesday. Ist es bereits Wednesday, so ist die Zeitspanne 0</returns>
        public static TimeSpan WhenIsWednesdayMyDude()
        {
            DateTime now = DateTime.Now;

            if (DateTime.Now.DayOfWeek == DayOfInterest)
            {
                // Es ist Wednesday, es wird eine TimeSpan von 0 zurückgegeben
                return new TimeSpan();
            }
            else
            {
                // Es ist nicht Wednesday => Berechne Zeit bis Wednesday

                DateTime wednesdayTime = now;

                // Berechnen wie viele Tage es noch bis Wednesday sind
                if (now.DayOfWeek < DayOfInterest)
                {
                    // Wochentag ist Sonntag - Dienstag
                    wednesdayTime = wednesdayTime.AddDays(DayOfInterest - now.DayOfWeek);
                }
                else
                {
                    // Wochentag ist Donnerstag - Samstag
                    wednesdayTime = wednesdayTime.AddDays(DayOfInterest + 7 - now.DayOfWeek);
                }

                // Sicherstellen, dass Zieldatum wirklich Wednesday ist
                System.Diagnostics.Debug.Assert(wednesdayTime.DayOfWeek == DayOfInterest, "Date is not a Wednesday :(");

                // Zeit berechnen, die am Zieldatum bereits seit 0:00 vergangen ist
                TimeSpan timeIntoWednesday = new TimeSpan(0, wednesdayTime.Hour, wednesdayTime.Minute, wednesdayTime.Second, wednesdayTime.Millisecond);

                // Diese Zeit abziehen, um Mitternacht des gewünschten Wednesday zu erlangen
                wednesdayTime = wednesdayTime.Subtract(timeIntoWednesday);

                // Die Zeitspanne zwischen jetzt und Wednesday zurückgeben
                return wednesdayTime - now/* /* Debug-Time *\/ - new TimeSpan(1,57,0) */;
            }
        }

        /// <summary>
        /// Löst das ItIsWednesdayEvent aus, wenn Wendesday ist. Schläft anschließend bis zum nächsten Wednesday
        /// </summary>
        private static void WednesdayInvestigationThread()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(WhenIsWednesdayMyDude());
                    System.Diagnostics.Debug.Assert(DateTime.Now.DayOfWeek == DayOfInterest, "For some odd reason, it is not Wednesday, my dude!");
                    Console.WriteLine("IT IS WEDNESDAY, MY DUDE!");

                    ItIsWednesdayEvent();
                    Thread.Sleep(new TimeSpan(1, 0, 0, 0));
                }
            }
            catch (ThreadInterruptedException tiex)
            {
                Console.WriteLine(tiex.Message);
            }
        }

        public delegate void ItIsWednesdayHandler();
        public static event ItIsWednesdayHandler ItIsWednesdayEvent;
    }
}
