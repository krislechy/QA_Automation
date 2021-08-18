using NUnit.Framework;
using TaskKiller;
using System;

namespace Test_TaskKiller
{
    public class Test_TaskKiller
    {
        string test_name;
        int test_Allowed_Time;
        int test_Interval;
        string[] args;
        [SetUp]
        public void Setup()
        {
            test_name = "2";
            test_Allowed_Time = 5;
            test_Interval = 1;
            args = new string[] { test_name, test_Allowed_Time.ToString(), test_Interval.ToString() };

            Program.Main(args);
        }
        [Test]
        public void TestVariable_Args()
        {
            Assert.Throws<ArgumentNullException>(() => Program.Main(null));
            Assert.Throws<ArgumentNullException>(() => Program.Main(new string[] { null,null,null}));
            Assert.Throws<ArgumentNullException>(() => Program.Main(new string[] { "1",null,null}));
            Assert.Throws<ArgumentNullException>(() => Program.Main(new string[] { null,"1",null}));
            Assert.Throws<ArgumentNullException>(() => Program.Main(new string[] { null,"-1",null}));
            Assert.Throws<ArgumentNullException>(() => Program.Main(new string[] { null,null,"0"}));
            Assert.Throws<ArgumentNullException>(() => Program.Main(new string[] { null,null,"2"}));
            Assert.Throws<ArgumentOutOfRangeException>(() => Program.Main(new string[] { "notepad","1","1", "gfhdf" }));
            Assert.Throws<ArgumentNullException>(() => Program.Main(new string[] { null,"5","1"}));
            Assert.Throws<ArgumentOutOfRangeException>(() => Program.Main(new string[] { "notepad","-5","1"}));
            Assert.Throws<ArgumentOutOfRangeException>(() => Program.Main(new string[] { "notepad","5","0"}));
        }
        [Test]
        public void TestVariable_Allowed_Time()
        {
            Assert.IsNotNull(Program.Interval);
            Assert.IsTrue(Program.Allowed_Time >= 0);
            Assert.AreEqual(test_Allowed_Time,Program.Allowed_Time);
        }
        [Test]
        public void TestVariable_timer()
        {
            Assert.IsNotNull(Program.timer, "������ �� ���������������");
            Assert.IsTrue(Program.timer.Enabled, "������ �� �������");
        }
        [Test]
        public void TestVariable_Interval()
        {
            Assert.IsNotNull(Program.Interval, "�������� ������� ��������� ������������ �������� null!");
            Assert.IsTrue(Program.Interval > 0, "�������� ������� ������ ��� ����� 0");
            Assert.AreEqual(test_Interval, Program.Interval, "�������� ��������� �� ������������� ���������");
        }
        [Test]
        public void TestVariable_Name()
        {
            Assert.IsNotNull(test_name, "������������ �������� ��������� ������������ �������� null!");
            Assert.IsNotEmpty(test_name, "������������ �������� ��������� ������������ �������� null!");
            Assert.AreEqual(test_name, Program.Name, "��� �������� �� ������������� ���������");
        }
        [Test]
        public void TestVariable_DateTime()
        {
            Assert.IsNotNull(Program.dt, "���������� ���� � ������� ��������� ������������ �������� null!");
            Assert.AreEqual(Program.dt,DateTime.Now,"�������� ������� ���������� �� ���������");
        }
    }
}