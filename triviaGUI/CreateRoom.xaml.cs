using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net.Sockets;
using System.Net;

namespace triviaGUI
{
    /// <summary>
    /// Interaction logic for CreateRoom.xaml
    /// </summary>
    public partial class CreateRoom : Window
    {
        private Menu menuPage;
        private WaitingRoom waitPage;
        TcpClient client;
        IPEndPoint serverEndPoint;
        NetworkStream clientStream;
        LoginRequest log;
        private Helper help = new Helper();
        public CreateRoom(TcpClient _client, IPEndPoint _serverEndPoint, NetworkStream _clientStream, LoginRequest req)
        {
            this.Closed += JoinClosed;
            this.client = _client;
            this.serverEndPoint = _serverEndPoint;
            this.clientStream = _clientStream;
            this.log = req;
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            CreateRoomRequest req = new CreateRoomRequest();
            req.room_name = this.textBoxName.Text;
            req.max_users = int.Parse(this.textBoxPlayers.Text);
            req.question_count = int.Parse(this.textBoxQuestions.Text);
            req.answer_timeout = int.Parse(this.textBoxTime.Text);

            var ms = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(CreateRoomRequest));
            ser.WriteObject(ms, req);
            byte[] json = ms.ToArray();
            ms.Close();

            byte[] request = help.serializeMsg((int)RequestCode.CreateRoomRequestCode, json);
            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

            byte[] respBytes = help.getFullMsg(this.clientStream);
            var resp = new CreateRoomResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as CreateRoomResponse;
            msResp.Close();
            if (resp.status == (int)ResponseCode.success)
            {
                if (waitPage == null)
                {
                    waitPage = new WaitingRoom(this.client, this.serverEndPoint, this.clientStream, resp, this.log);
                }
                waitPage.Show();
                this.Hide();
            }
        }
        public void JoinClosed(object sender, EventArgs e)
        {
            try { 
                byte[] json = new byte[0];
                byte[] request = help.serializeMsg(229, json);

                clientStream.Write(request, 0, request.Length);
                clientStream.Flush();

                byte[] respBytes = help.getFullMsg(this.clientStream);
                LogoutResponse resp = new LogoutResponse();
                var msResp = new MemoryStream(respBytes);
                var serResp = new DataContractJsonSerializer(resp.GetType());
                resp = serResp.ReadObject(msResp) as LogoutResponse;
                msResp.Close();
                Application.Current.Shutdown();
            }
            catch { }
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (menuPage == null)
            {
                menuPage = new Menu(this.client, this.serverEndPoint, this.clientStream, this.log);
            }
            menuPage.Show();
            this.Hide();
        }
    }
}
