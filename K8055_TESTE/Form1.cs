using System;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;


namespace K8055_TESTE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static class K8055
        {
            [DllImport("K8055D.dll")]
            public static extern int OpenDevice(int CardAdress);

            [DllImport("K8055D.dll")]
            public static extern void CloseDevice();

            [DllImport("K8055D.dll")]
            public static extern int ReadAnalogChannel(int Channel);

            [DllImport("K8055D.dll")]
            public static extern void ReadAllAnalog(ref int Data1, ref int Data2);

            [DllImport("K8055D.dll")]
            public static extern void ClearAnalogChannel(int Channel);

            [DllImport("K8055D.dll")]
            public static extern void ClearAllAnalog();

            [DllImport("K8055D.dll")]
            public static extern void OutputAnalogChannel(int Channel, int Data);

            [DllImport("K8055D.dll")]
            public static extern void OutputAllAnalog(int Data1, int Data2);

            [DllImport("K8055D.dll")]
            public static extern void SetAnalogChannel(int Channel);

            [DllImport("K8055D.dll")]
            public static extern void SetAllAnalog();

            [DllImport("K8055D.dll")]
            public static extern void ClearAllDigital();

            [DllImport("K8055D.dll")]
            public static extern void ClearDigitalChannel(int Channel);

            [DllImport("K8055D.dll")]
            public static extern void SetAllDigital();

            [DllImport("K8055D.dll")]
            public static extern void SetDigitalChannel(int Channel);

            [DllImport("K8055D.dll")]
            public static extern void WriteAllDigital(int Data);

            [DllImport("K8055D.dll")]
            public static extern bool ReadDigitalChannel(int Channel);

            [DllImport("K8055D.dll")]
            public static extern int ReadAllDigital();

            [DllImport("K8055D.dll")]
            public static extern int ReadCounter(int CounterNr);

            [DllImport("K8055D.dll")]
            public static extern void ResetCounter(int CounterNr);

            [DllImport("K8055D.dll")]
            public static extern void SetCounterDebounceTime(int CounterNr, int DebounceTime);
        }

        private bool isInMaintenanceMode = false;
        private bool isOpenForCars = false;
        private bool isOpenForPedestrians = false;
        private bool isWarning = false;
        private int timer = 10000;

        private const int ARED = 1;
        private const int AYELLOW = 2;
        private const int AGREEN = 3;

        private const int BRED = 4;
        private const int BYELLOW = 5;
        private const int BGREEN = 6;

        private const int DRED = 7;
        private const int DGREEN = 8;

        private void Form1_Load(object sender, EventArgs e)
        {
            Task task1 = new Task(delegate
            {
                while (true)
                {
                    K8055.ClearAllDigital();
                    if (isInMaintenanceMode)
                    {
                        System.Threading.Thread.Sleep(500);
                        K8055.SetDigitalChannel(AYELLOW);
                        K8055.SetDigitalChannel(BYELLOW);
                        System.Threading.Thread.Sleep(500);
                        continue;
                    }
                    K8055.SetDigitalChannel(ARED);
                    K8055.SetDigitalChannel(BRED);
                    if (isOpenForCars)
                    {
                        K8055.ClearDigitalChannel(BRED);
                        K8055.SetDigitalChannel(isWarning ? BYELLOW : BGREEN);
                    }
                    else if (isOpenForPedestrians)
                    {
                        K8055.ClearDigitalChannel(ARED);
                        K8055.SetDigitalChannel(isWarning ? AYELLOW : AGREEN);
                    }
                    System.Threading.Thread.Sleep(500);
                }
            });
            task1.Start();
            Task task2 = new Task(delegate
            {
                while (true)
                {
                    isOpenForCars = false;
                    isOpenForPedestrians = false;
                    isWarning = false;
                    if (isInMaintenanceMode)
                    {
                        continue;
                    }

                    isOpenForCars = true;
                    System.Threading.Thread.Sleep((timer / 10) * 9);
                    isWarning = true;
                    System.Threading.Thread.Sleep((timer / 10) * 1);
                    isWarning = false;
                    isOpenForCars = false;
                    System.Threading.Thread.Sleep(4000);

                    isOpenForPedestrians = true;
                    System.Threading.Thread.Sleep(((timer / 10) * 9) / 2);
                    isWarning = true;
                    System.Threading.Thread.Sleep(((timer / 10) * 1) / 2);
                    isWarning = false;
                    isOpenForPedestrians = false;
                    System.Threading.Thread.Sleep(4000);
                }
            });
            task2.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            K8055.OpenDevice(0); //Open communication with K8055 that has the device address 0
        }

        private void button2_Click(object sender, EventArgs e)
        {
            K8055.CloseDevice(); //Closes communication with the K8055
        }

        private void button3_Click(object sender, EventArgs e)
        {
            K8055.SetDigitalChannel(1); //Sets digital output channel 1 to 'ON'
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(K8055.ReadDigitalChannel(1).ToString());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            isInMaintenanceMode = !isInMaintenanceMode;
        }
    }


}
