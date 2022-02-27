using System;
using System.Threading;
using System.Threading.Tasks;

namespace BotTest
{
    class WednesdayInvestigator
    {
        private const DayOfWeek DayOfInterest = DayOfWeek.Wednesday;

        /// <summary>
        /// Startet einen Thread, welcher auf jeden Wednesday das
        /// ItIsWednesdayEvent auslöst
        /// </summary>
        public void BeginInvestigation()
        {
            if (wednesdayInvestigationTask == null)
            {
                tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;
                wednesdayInvestigationTask =
                    Task.Run(WednesdayInvestigationThread, token);
            }
        }

        /// <summary>
        /// Stoppt den internen Wednesday-Investigations-Thread. Kehrt zurück,
        /// sobald der Thread terminiert ist.
        /// </summary>
        public void EndInvestigation()
        {
            tokenSource.Cancel();
            wednesdayInvestigationTask.Wait();
            tokenSource.Dispose();
            wednesdayInvestigationTask = null;
        }

        /// <summary>
        /// Berechnet die Zeitdauer bis zum nächsten Wednesday
        /// </summary>
        /// <returns>
        /// Die Zeit bis zum nächsten Wednesday. Ist es bereits Wednesday, so
        /// ist die Zeitspanne 0
        /// </returns>
        public static TimeSpan WhenIsWednesdayMyDude()
        {
            var now = DateTime.Now;

            if (DateTime.Now.DayOfWeek == DayOfInterest)
            {
                // Es ist Wednesday, es wird eine TimeSpan von 0 zurückgegeben
                return new TimeSpan();
            }
            else
            {
                // Es ist nicht Wednesday => Berechne Zeit bis Wednesday

                var wednesdayTime = now;

                // Berechnen wie viele Tage es noch bis Wednesday sind
                if (now.DayOfWeek < DayOfInterest)
                {
                    // Wochentag ist Sonntag - Dienstag
                    wednesdayTime = wednesdayTime
                        .AddDays(DayOfInterest - now.DayOfWeek);
                }
                else
                {
                    // Wochentag ist Donnerstag - Samstag
                    wednesdayTime = wednesdayTime
                        .AddDays(DayOfInterest + 7 - now.DayOfWeek);
                }

                // Sicherstellen, dass Zieldatum wirklich Wednesday ist
                System.Diagnostics.Debug.Assert(
                    wednesdayTime.DayOfWeek == DayOfInterest,
                    "Date is not a Wednesday :("
                );

                // Zeit berechnen, die am Zieldatum bereits seit 0:00 vergangen
                // ist
                var timeIntoWednesday = new TimeSpan(
                    0,
                    wednesdayTime.Hour,
                    wednesdayTime.Minute,
                    wednesdayTime.Second,
                    wednesdayTime.Millisecond
                );

                // Diese Zeit abziehen, um Mitternacht des gewünschten Wednesday
                // zu erlangen
                wednesdayTime = wednesdayTime.Subtract(timeIntoWednesday);

                // Die Zeitspanne zwischen jetzt und Wednesday zurückgeben
                return wednesdayTime - now;
            }
        }

        /// <summary>
        /// Löst das ItIsWednesdayEvent aus, wenn Wendesday ist.
        /// Schläft anschließend bis zum nächsten Wednesday.
        /// </summary>
        private void WednesdayInvestigationThread()
        {
            while (!token.IsCancellationRequested)
            {
                token.WaitHandle.WaitOne(WhenIsWednesdayMyDude());

                if (!token.IsCancellationRequested)
                {
                    System.Diagnostics.Debug.Assert(
                        DateTime.Now.DayOfWeek == DayOfInterest,
                        "For some odd reason, it is not Wednesday, my dude!"
                    );

                    Console.WriteLine("IT IS WEDNESDAY, MY DUDE!");

                    ItIsWednesdayEvent();

                    token.WaitHandle.WaitOne(TimeSpan.FromDays(1.0));
                }
            }
        }

        public delegate void ItIsWednesdayHandler();
        public event ItIsWednesdayHandler ItIsWednesdayEvent;

        private Task wednesdayInvestigationTask;
        private CancellationTokenSource tokenSource;
        private CancellationToken token;
    }
}
