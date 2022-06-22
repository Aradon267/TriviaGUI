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
    /// Interaction logic for AddQuestion.xaml
    /// </summary>
    public partial class AddQuestion : Window
    {
        private Menu menuPage;
        TcpClient client;
        IPEndPoint serverEndPoint;
        NetworkStream clientStream;
        LoginRequest log;
        private Helper help = new Helper();
        public AddQuestion(TcpClient _client, IPEndPoint _serverEndPoint, NetworkStream _clientStream, LoginRequest req)
        {
            this.log = req;
            this.Closed += JoinClosed;
            this.client = _client;
            this.serverEndPoint = _serverEndPoint;
            this.clientStream = _clientStream;
            InitializeComponent();
        }
        public void JoinClosed(object sender, EventArgs e)
        {
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            AddQuestionRequest req = new AddQuestionRequest();
            req.question = this.textBoxQuest.Text;
            req.correct = this.textBoxCorr.Text;
            req.ans1 = this.textBoxans1.Text;
            req.ans2 = this.textBoxans2.Text;
            req.ans3 = this.textBoxAns3.Text;

            var ms = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(AddQuestionRequest));
            ser.WriteObject(ms, req);
            byte[] json = ms.ToArray();
            ms.Close();

            byte[] request = help.serializeMsg((int)RequestCode.AddQuestionCode, json);
            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

            byte[] respBytes = help.getFullMsg(this.clientStream);
            var resp = new AddQuestionResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as AddQuestionResponse;
            msResp.Close();
            if (resp.status == (int)ResponseCode.success)
            {
                MessageBox.Show("Question created!");
                if (menuPage == null)
                {
                    menuPage = new Menu(this.client, this.serverEndPoint, this.clientStream, this.log);
                }
                menuPage.Show();
                this.Hide();
            }
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
