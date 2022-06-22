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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        private Register regPage;
        private Menu menuPage;
        TcpClient client;
        IPEndPoint serverEndPoint;
        NetworkStream clientStream;
        private Helper help = new Helper();
        public Login(TcpClient _client, IPEndPoint _serverEndPoint, NetworkStream _clientStream)
        {
            this.Closed += JoinClosed;
            this.client = _client;
            this.serverEndPoint = _serverEndPoint;
            this.clientStream = _clientStream;
            InitializeComponent();
        }
        public Login()
        {
            this.Closed += JoinClosed;
            this.client = new TcpClient();
            this.serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8826);
            this.client.Connect(this.serverEndPoint);
            this.clientStream = this.client.GetStream();
            InitializeComponent();
        }
        public void JoinClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            LoginRequest req = new LoginRequest();
            req.username = textBoxName.Text;
            req.password = passwordBox.Password;

            var ms = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(LoginRequest));
            ser.WriteObject(ms, req);
            byte[] json = ms.ToArray();
            ms.Close();

            byte[] request = help.serializeMsg((int)RequestCode.LoginCode, json);
            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

            
            byte[] respBytes = help.getFullMsg(this.clientStream);
            var resp = new LoginResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as LoginResponse;
            msResp.Close();
            if (resp.status==(int)ResponseCode.success)
            {
                if (menuPage == null)
                {
                    menuPage = new Menu(this.client,this.serverEndPoint,this.clientStream, req);
                }
                menuPage.Show();
                this.Hide();
            }
            else
            {
                if(resp.status==(int)ResponseCode.alreadyConnected)
                {
                    MessageBox.Show("The user is already connected from another place!");
                }
                else if(resp.status == (int)ResponseCode.invalidName)
                {
                    MessageBox.Show("The username wasn't found in our database!");
                }
                else if (resp.status == (int)ResponseCode.wrongPassword)
                {
                    MessageBox.Show("Wrong password!");
                }

            }
        }
        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            if(regPage == null)
            {
                regPage = new Register(this.client, this.serverEndPoint, this.clientStream);
            }
            regPage.Show();
            this.Hide();
        }
    }
}
