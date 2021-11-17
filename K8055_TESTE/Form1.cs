using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

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

        private bool isInMaintenanceMode = true;
        private bool isWarning = false;
        private bool yinyang = false;
        private bool D1 = false;
        private int timer = 10000;
        private int state = 0;

        private const int S11 = 1;
        private const int S12 = 2;
        private const int S21 = 3;
        private const int S22 = 4;
        private const int S3 = 5;
        private const int P1 = 6;
        private const int P2 = 7;
        private const int P3 = 8;
        private const bool P1Pressed = false;
        private const bool P2Pressed = false;
        private const bool P3Pressed = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            K8055.OpenDevice(0);
            
            Task task1 = new Task(delegate
            {
                int[] allLights = new int[] { S11, S12, S21, S22, S3, P1, P2, P3 };
                while (true)
                {
                    state1.BackColor = Color.Transparent;
                    state2.BackColor = Color.Transparent;
                    state3.BackColor = Color.Transparent;
                    maintanceModePanel.BackColor = isInMaintenanceMode ? Color.White : Color.Transparent;
                    foreach (int light in allLights)
                    {
                        K8055.SetDigitalChannel(light);
                    }
                    if (state == 1)
                    {
                        state1.BackColor = Color.White;
                        K8055.ClearDigitalChannel(S12);
                        K8055.ClearDigitalChannel(S21);
                        if (yinyang)
                        {
                            K8055.SetDigitalChannel(S11);
                            K8055.SetDigitalChannel(P3);
                        }
                        else
                        {
                            K8055.ClearDigitalChannel(S11);
                            K8055.ClearDigitalChannel(P3);
                            if (isWarning)
                            {
                                K8055.SetDigitalChannel(S12);
                                K8055.SetDigitalChannel(S21);
                            }
                        }
                    }
                    else if (state == 2)
                    {
                        state2.BackColor = Color.White;
                        K8055.ClearDigitalChannel(S21);
                        if (yinyang)
                        {
                            K8055.SetDigitalChannel(S22);
                            K8055.SetDigitalChannel(P3);
                        }
                        else
                        {
                            K8055.ClearDigitalChannel(S22);
                            K8055.ClearDigitalChannel(P3);
                            if (isWarning)
                            {
                                K8055.SetDigitalChannel(S21);
                            }
                        }
                    }
                    else if (state == 3)
                    {
                        state3.BackColor = Color.White;
                        K8055.ClearDigitalChannel(S11);
                        K8055.ClearDigitalChannel(S3);
                        if (isWarning && !yinyang)
                        {
                            K8055.SetDigitalChannel(S11);
                            K8055.SetDigitalChannel(S3);
                        }
                    }
                    System.Threading.Thread.Sleep(333);
                    if (isInMaintenanceMode)
                    {
                        K8055.ClearAllDigital();
                        System.Threading.Thread.Sleep(333);
                    }
                    yinyang = !yinyang;
                }
            });
            task1.Start();
            Task task2 = new Task(delegate
            {
                while (true)
                {
                    isWarning = false;
                    if (isInMaintenanceMode)
                    {
                        state = 0;
                        continue;
                    }
                    state++;
                    if ((state == 3 && !D1) || state > 3)
                    {
                        state = 1;
                        D1 = false;
                    }
                    for (int i = 0; i < 9; i++)
                    {
                        if (isInMaintenanceMode)
                        {
                            i = 8;
                        }
                        System.Threading.Thread.Sleep(timer / 10);
                    }
                    isWarning = true;
                    System.Threading.Thread.Sleep(timer / 10);
                    isWarning = false;
                }
            });
            task2.Start();
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


        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            K8055.OpenDevice(0);
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            isInMaintenanceMode = !isInMaintenanceMode;
        }

        private void p1_Click(object sender, EventArgs e)
        {
            // should turn off
            // s1.1, s1.2, s2.1, s3
            MessageBox.Show("On: P1 \n Off: s1.1, s1.2, s2.1, s3 ");
        }

        private void p2_Click(object sender, EventArgs e)
        {
            // should turn off
            // s3, s2.1, s2.2, s1.2
            MessageBox.Show("On: p1 \n Off: s3, s2.1, s2.2, s1.2");
        }

        private void p3_Click(object sender, EventArgs e)
        {
        // should turn off
        // s3, s2.2, s1.
        MessageBox.Show("On P3 \n Off: s3, s2.2, s1.1");
        }

        private void d1_Click(object sender, EventArgs e)
        {
            D1 = true;
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }
    }


}
