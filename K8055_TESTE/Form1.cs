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
        private bool yinyang = false;
        private bool D1 = false;
        private int timer = 20000;
        private int state = 0;
        private bool P1Pressed = false;
        private bool P2Pressed = false;
        private bool P3Pressed = false;
        private DateTime P1PressedAt;
        private DateTime P2PressedAt;
        private DateTime P3PressedAt;

        private const int S11 = 1;
        private const int S12 = 2;
        private const int S21 = 3;
        private const int S22 = 4;
        private const int S3 = 5;
        private const int P1 = 6;
        private const int P2 = 7;
        private const int P3 = 8;
        
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
                    if (isInMaintenanceMode)
                    {
                        foreach (int light in allLights)
                        {
                            K8055.SetDigitalChannel(light);
                        }
                    }
                    if (state == 1)
                    {
                        state1.BackColor = Color.White;
                        K8055.ClearDigitalChannel(S12);
                        K8055.ClearDigitalChannel(S21);
                        K8055.SetDigitalChannel(S22);
                        K8055.SetDigitalChannel(S3);
                        K8055.SetDigitalChannel(P1);
                        K8055.SetDigitalChannel(P2);
                        if (yinyang)
                        {
                            K8055.SetDigitalChannel(S11);
                            K8055.SetDigitalChannel(P3);
                        }
                        else
                        {
                            K8055.ClearDigitalChannel(S11);
                            K8055.ClearDigitalChannel(P3);
                        }
                    }
                    else if (state == 2)
                    {
                        state2.BackColor = Color.White;
                        K8055.ClearDigitalChannel(S21);
                        K8055.SetDigitalChannel(S3);
                        K8055.SetDigitalChannel(P1);
                        K8055.SetDigitalChannel(P2);
                        if (yinyang)
                        {
                            K8055.SetDigitalChannel(S22);
                            K8055.SetDigitalChannel(P3);
                        }
                        else
                        {
                            K8055.ClearDigitalChannel(S22);
                            K8055.ClearDigitalChannel(P3);
                        }
                    }
                    else if (state == 3)
                    {
                        state3.BackColor = Color.White;
                        K8055.ClearDigitalChannel(S11);
                        K8055.ClearDigitalChannel(S3);
                        K8055.SetDigitalChannel(S12);
                        K8055.SetDigitalChannel(S21);
                        K8055.SetDigitalChannel(S22);
                        K8055.SetDigitalChannel(P1);
                        K8055.SetDigitalChannel(P2);
                        K8055.SetDigitalChannel(P3);
                    }
                    if (isInMaintenanceMode)
                    {
                        System.Threading.Thread.Sleep(200);
                        K8055.ClearAllDigital();
                    }
                    else
                    {
                        if (P1Pressed)
                        {
                            int timediff = Convert.ToInt32((DateTime.Now - P1PressedAt).TotalMilliseconds);
                            int[] blockedByP1 = new int[] { S11, S12, S21, S3};
                            if (timediff > Convert.ToInt32(timer / 2))
                            {
                                P1Pressed = false;
                                K8055.SetDigitalChannel(P1);
                            }
                            else
                            {
                                foreach (int light in blockedByP1)
                                {
                                    K8055.SetDigitalChannel(light);
                                }
                                K8055.ClearDigitalChannel(P1);
                            }
                        }
                        if (P2Pressed)
                        {
                            int timediff = Convert.ToInt32((DateTime.Now - P2PressedAt).TotalMilliseconds);
                            int[] blockedByP2 = new int[] { S12, S21, S22, S3 };
                            if (timediff > Convert.ToInt32(timer / 2))
                            {
                                P2Pressed = false;
                                K8055.SetDigitalChannel(P2);
                            }
                            else
                            {
                                foreach (int light in blockedByP2)
                                {
                                    K8055.SetDigitalChannel(light);
                                }
                                K8055.ClearDigitalChannel(P2);
                            }
                        }
                        if (P3Pressed)
                        {
                            int timediff = Convert.ToInt32((DateTime.Now - P3PressedAt).TotalMilliseconds);
                            int[] blockedByP3 = new int[] { S11, S22, S3 };
                            if (timediff > Convert.ToInt32(timer / 2))
                            {
                                P3Pressed = false;
                                K8055.SetDigitalChannel(P3);
                            }
                            else
                            {
                                foreach (int light in blockedByP3)
                                {
                                    K8055.SetDigitalChannel(light);
                                }
                                K8055.ClearDigitalChannel(P3);
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(200);
                    yinyang = !yinyang;
                }
            });
            task1.Start();
            Task task2 = new Task(delegate
            {
                while (true)
                {
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
                    System.Threading.Thread.Sleep(timer);
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
            P1Pressed = true;
            P1PressedAt = DateTime.Now;
        }

        private void p2_Click(object sender, EventArgs e)
        {
            P2Pressed = true;
            P2PressedAt = DateTime.Now;
        }

        private void p3_Click(object sender, EventArgs e)
        {
            P3Pressed = true;
            P3PressedAt = DateTime.Now;
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
