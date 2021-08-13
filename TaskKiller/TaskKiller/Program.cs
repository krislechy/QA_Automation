using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace TaskKiller
{
    class Program
    {
        static System.Timers.Timer timer;
        static int Allowed_Time;
        static int Interval;
        static string Name;
        static DateTime dt { get { return DateTime.Now; } }
        static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                if (!String.IsNullOrEmpty(args[0]))
                {
                    Name = args[0];
                    bool is_time = Int32.TryParse(args[1], out Allowed_Time);
                    if (is_time && Allowed_Time >= 0)
                    {
                        bool is_interval = Int32.TryParse(args[2], out Interval);
                        if (is_interval && Interval > 0)
                        {
                            StartTimer();
                        }
                        else Console.WriteLine("Значение параметра \"{0}\" не является натуральным числом или он меньше или равняется 0.", args[2]);
                    }
                    else Console.WriteLine("Значение параметра \"{0}\" не является натуральным числом или меньше 0.", args[1]);
                }
                else Console.WriteLine("Значение параметра \"{0}\" неверное.", args[0]);
            }
            else Console.WriteLine("Неправильны параметры, входные параметры: taskkiller <Имя процесса> <Время жизни> <Период>.\nПример: notepad 5 1.");
            Console.ReadKey();
        }
        private static void StartTimer()
        {
            var interval_minutes = TimeSpan.FromMinutes(Interval);
            timer = new System.Timers.Timer() { Enabled = true, Interval = interval_minutes.TotalMilliseconds };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Console.WriteLine("{0:HH:mm:ss} Таймер запущен для поиска процесса: \"{1}\", проверка каждые {2}.", dt, Name, interval_minutes);
        }

        private static void StopTimer()
        {
            timer.Stop();
            timer.Dispose();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("{0:HH:mm:ss} Попытка найти процесс \"{1}\"", e.SignalTime, Name);
            KillProcesses();
        }

        private static void KillProcesses()
        {
            var proc = Process.GetProcesses();
            bool IsFound = false;
            foreach (Process s in proc)
            {
                if (s.ProcessName.Equals(Name, StringComparison.OrdinalIgnoreCase))
                {
                    DateTime start_dt = s.StartTime;
                    var different = (DateTime.Now - start_dt);
                    if (different.TotalMinutes > Allowed_Time)
                    {
                        try
                        {
                            s.Kill();
                            Console.WriteLine("{0:HH:mm:ss} ИД процесса: {1} с именем: \"{2}\" был уничтожен.", dt, s.Id, Name);
                            IsFound = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            return;
                        }
                    }
                    else
                    {
                        IsFound = true;
                        var elapsed = TimeSpan.FromMinutes(Allowed_Time) - different;
                        Console.WriteLine("{0:HH:mm:ss} Процесс с ИД: {1} и именем: {2} работает уже: {3} осталось: {4}", dt, s.Id, Name, different, elapsed);
                    }
                }
            }
            if (!IsFound)
                Console.WriteLine("{0:HH:mm:ss} Процесс с именем \"{1}\" не найден.", dt, Name);
        }
    }
}
