using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Newtonsoft.Json;

namespace Predict.Model
{
    public class CONNECT
    {
        private TcpClient Client;
        private NetworkStream Stream;

        private string IP = "10.10.21.119";
        private int Port = 3421;

        public CONNECT() { }

        private int ConnectServer()
        {
            try
            {
                Client = new TcpClient(IP, Port);
                Stream = Client.GetStream();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }

            return 1;
        }

        private void DisconnectServer()
        {
            Stream.Close();
            Client.Close();
        }

        public int LoginCheck(string EnterID, string EnterPW)
        {
            if (ConnectServer() == 0)
            {
                return 0;
            }

            Data Send = new Data()
            {
                Type = (int)TYPE.LOGIN,
                ID = EnterID
            };

            string SendJson = JsonConvert.SerializeObject(Send, Newtonsoft.Json.Formatting.Indented);
            byte[] SendByte = Encoding.UTF8.GetBytes(SendJson);
            Stream.Write(SendByte, 0, SendByte.Length);

            byte[] Buffer = new byte[2048];
            int ReadByte = Stream.Read(Buffer, 0, Buffer.Length);
            string ReadJson = Encoding.UTF8.GetString(Buffer, 0, ReadByte);

            try
            {
                Data Result = JsonConvert.DeserializeObject<Data>(ReadJson);

                if (Result.Type == (int)TYPE.SUCCEED)
                {
                    //MessageBox.Show(Result.NO, "Success");
                    //MessageBox.Show(Result.NAME, "Success");
                    //MessageBox.Show(Result.PW, "Success");

                    if(EnterPW == Result.PW)
                    {
                        Login.data.ID = EnterID;
                        Login.data.PW = EnterPW;
                        Login.data.NO = Result.NO;
                        Login.data.NAME = Result.NAME;
                        MessageBox.Show("로그인 성공", "Success");
                    }
                    else
                    {
                        Result.Type = (int)TYPE.FAIL;
                        MessageBox.Show("로그인 실패", "Fail");
                    }
                }
                else
                {
                    MessageBox.Show("로그인 실패", "Fail");
                }

                DisconnectServer();

                return Result.Type;
            }
            catch (JsonException jsonEx)
            {
                DisconnectServer();

                MessageBox.Show($"JSON Exception: {jsonEx.Message}");

                return (int)TYPE.FAIL;
            }
        }

        public int JoinUser(string EnterName, string EnterID, string EnterPW)
        {
            if (ConnectServer() == 0)
            {
                return 0;
            }

            Data Send = new Data()
            {
                Type = (int)TYPE.JOIN,
                NAME = EnterName,
                ID = EnterID,
                PW = EnterPW
            };

            string SendJson = JsonConvert.SerializeObject(Send, Newtonsoft.Json.Formatting.Indented);
            byte[] SendByte = Encoding.UTF8.GetBytes(SendJson);
            Stream.Write(SendByte, 0, SendByte.Length);

            byte[] Buffer = new byte[2048];
            int ReadByte = Stream.Read(Buffer, 0, Buffer.Length);
            string ReadJson = Encoding.UTF8.GetString(Buffer, 0, ReadByte);

            try
            {
                Data Result = JsonConvert.DeserializeObject<Data>(ReadJson);

                if (Result.Type == (int)TYPE.SUCCEED)
                {
                    //MessageBox.Show(Result.NAME, "Success");
                    //MessageBox.Show(Result.ID, "Success");
                    //MessageBox.Show(Result.PW, "Success");
                    MessageBox.Show("회원가입 성공", "Success");
                }
                else if (Result.Type == (int)TYPE.DUPLICATE)
                {
                    MessageBox.Show("아이디 중복, 아이디를 변경해주세요.", "Duplicate");
                }
                else
                {
                    MessageBox.Show("회원가입 실패", "Fail");
                }

                DisconnectServer();

                return Result.Type;
            }
            catch (JsonException jsonEx)
            {
                DisconnectServer();

                MessageBox.Show($"JSON Exception: {jsonEx.Message}");

                return (int)TYPE.FAIL;
            }
        }

        public int GrapRead()
        {
            if (ConnectServer() == 0)
            {
                return 0;
            }

            Data Send = new Data()
            {
                Type = (int)TYPE.SHOWGRAPH
            };

            string SendJson = JsonConvert.SerializeObject(Send, Newtonsoft.Json.Formatting.Indented);
            byte[] SendByte = Encoding.UTF8.GetBytes(SendJson);
            Stream.Write(SendByte, 0, SendByte.Length);

            byte[] Buffer = new byte[2048];
            int ReadByte = Stream.Read(Buffer, 0, Buffer.Length);
            string ReadJson = Encoding.UTF8.GetString(Buffer, 0, ReadByte);

            try
            {
                ChartData Result = JsonConvert.DeserializeObject<ChartData>(ReadJson);

                if (Result.Type == (int)TYPE.SUCCEED)
                {
                    Chart.chartdata.SexList = Result.SexList;
                    Chart.chartdata.RiskList = Result.RiskList;
                }
                else
                {
                    MessageBox.Show("그래프 실패", "Fail");
                }

                DisconnectServer();

                return Result.Type;
            }
            catch (JsonException jsonEx)
            {
                DisconnectServer();

                MessageBox.Show($"JSON Exception: {jsonEx.Message}");

                return (int)TYPE.FAIL;
            }
        }

        public int SendInputData(List<int> list)
        {
            if (ConnectServer() == 0)
            {
                return 0;
            }

            InformationData Send = new InformationData()
            {
                Type = (int)TYPE.SENDINPUTDATA,
                ID = Login.data.ID,
                Age = list[0],
                Gender = list[1],
                Ap_hi = list[2],
                Ap_lo = list[3],
                Smoke = list[4],
                Alco = list[5],
                Active = list[6],
                Bmi = list[7],
                Bp = list[8]
            };

            string SendJson = JsonConvert.SerializeObject(Send, Newtonsoft.Json.Formatting.Indented);
            byte[] SendByte = Encoding.UTF8.GetBytes(SendJson);
            Stream.Write(SendByte, 0, SendByte.Length);

            byte[] Buffer = new byte[2048];
            int ReadByte = Stream.Read(Buffer, 0, Buffer.Length);
            string ReadJson = Encoding.UTF8.GetString(Buffer, 0, ReadByte);

            try
            {
                Data Result = JsonConvert.DeserializeObject<Data>(ReadJson);

                if (Result.Type == (int)TYPE.SUCCEED)
                {
                    Login.data.Cardio = Result.Cardio;
                }
                else
                {
                    MessageBox.Show("조회 실패", "Fail");
                }

                DisconnectServer();

                return Result.Type;
            }
            catch (JsonException jsonEx)
            {
                DisconnectServer();

                MessageBox.Show($"JSON Exception: {jsonEx.Message}");

                return (int)TYPE.FAIL;
            }
        }
    }
}
