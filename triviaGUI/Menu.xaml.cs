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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        private AddQuestion addPage;
        private Login logPage;
        private CreateRoom createPage;
        private JoinRoom joinPage;
        TcpClient client;
        IPEndPoint serverEndPoint;
        NetworkStream clientStream;
        LoginRequest log;
        private Helper help = new Helper();
        public Menu(TcpClient _client, IPEndPoint _serverEndPoint, NetworkStream _clientStream, LoginRequest req)
        {
            this.Closed += JoinClosed;
            this.log = req;
            this.client = _client;
            this.serverEndPoint = _serverEndPoint;
            this.clientStream = _clientStream;
            InitializeComponent();
        }
        public Menu()
        {
            InitializeComponent();
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
            catch
            {

            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (createPage == null)
            {
                createPage = new CreateRoom(this.client, this.serverEndPoint, this.clientStream, this.log);
            }
            createPage.Show();
            this.Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
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
            if(resp.status==(int)ResponseCode.success)
            {
                if(logPage == null)
                {
                    logPage = new Login();
                }
                logPage.Show();
                this.Hide();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (joinPage == null)
            {
                joinPage = new JoinRoom(this.client, this.serverEndPoint, this.clientStream, this.log);
            }
            joinPage.Show();
            this.Hide();
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            this.nameText.Text = this.log.username;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (addPage == null)
            {
                addPage = new AddQuestion(this.client, this.serverEndPoint, this.clientStream, this.log);
            }
            addPage.Show();
            this.Hide();
        }
    }
}
