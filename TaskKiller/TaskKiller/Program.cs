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
    public class Program
    {
        public static System.Timers.Timer timer;
        public static int Allowed_Time;
        public static int Interval;
        public static string Name;
        public static DateTime dt { get { return DateTime.Now; } }
        public static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException("Значение аргуементов равно null");
            if (args.Length == 3)
            {
                if (!String.IsNullOrEmpty(args[0]))
                {
                    Name = args[0];
                    if (args[1] == null) throw new ArgumentNullException("Второй аргумент не должен равнятся null");
                    bool is_time = Int32.TryParse(args[1], out Allowed_Time);
                    if (is_time && Allowed_Time >= 0)
                    {
                        if (args[2] == null) throw new ArgumentNullException("Третий аргумент не должен равнятся null");
                        bool is_interval = Int32.TryParse(args[2], out Interval);
                        if (is_interval && Interval > 0)
                        {
                            //инициализируем таймер и запускаем его
                            StartTimer();
                        }
                        else
                            throw new ArgumentOutOfRangeException($"Значение параметра \"{args[2]}\" не является натуральным числом или он меньше или равняется 0.");
                    }
                    else throw new ArgumentOutOfRangeException($"Значение параметра \"{args[1]}\" не является натуральным числом или меньше 0.");
                }
                else throw new ArgumentNullException($"Значение параметра \"{args[0]}\" неверное или пустое.");
            }
            else throw new ArgumentOutOfRangeException("Неправильны параметры, входные параметры: taskkiller <Имя процесса> <Время жизни> <Период>.\nПример: notepad 5 1.");
            //Console.ReadKey();
        }

        public static void StartTimer()
        {
            var interval_minutes = TimeSpan.FromMinutes(Interval);
            timer = new System.Timers.Timer() { Enabled = true, Interval = interval_minutes.TotalMilliseconds };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Console.WriteLine("{0:HH:mm:ss} Таймер запущен для поиска процесса: \"{1}\", проверка каждые {2}.", dt, Name, interval_minutes);
        }

        public static void StopTimer()
        {
            timer.Stop();
            timer.Dispose();
        }

        public static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("{0:HH:mm:ss} Попытка найти процесс \"{1}\"", e.SignalTime, Name);
            KillProcesses();
        }

        public static void KillProcesses()
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
                            throw new Exception(ex.ToString());
                            //Console.WriteLine(ex.Message);
                            //return;
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
