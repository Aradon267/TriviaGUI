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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private Login log;
        private Helper help = new Helper();
        TcpClient client;
        IPEndPoint serverEndPoint;
        NetworkStream clientStream;
        public Register(TcpClient _client, IPEndPoint _serverEndPoint, NetworkStream _clientStream)
        {
            this.Closed += JoinClosed;
            this.client = _client;
            this.serverEndPoint = _serverEndPoint;
            this.clientStream = _clientStream;
            InitializeComponent();
        }
        public void JoinClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            SignupRequest req = new SignupRequest();
            req.username = this.textBoxName.Text;
            req.password = this.passwordBox.Password;
            req.email = this.textBoxEmail.Text;
            req.phone = this.textBoxPhone.Text;
            req.addr = this.textBoxAddr.Text;

            var ms = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(SignupRequest));
            ser.WriteObject(ms, req);
            byte[] json = ms.ToArray();
            ms.Close();

            byte[] request = help.serializeMsg((int)RequestCode.SignupCode, json);
            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

            byte[] respBytes = help.getFullMsg(this.clientStream);
            var resp = new SignupResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as SignupResponse;
            msResp.Close();
            if (resp.status == (int)ResponseCode.success)
            {
                if (log == null)
                {
                    log = new Login();
                }
                log.Show();
                this.Hide();
            }
            else
            {
                if (resp.status == (int)ResponseCode.badAddr)
                {
                    MessageBox.Show("A bad addr was entered! Make sure that the addr is in the following form:\n [street] [apt] [city]\n strret can have only letters, apt can only have numbers and city can only contain letters");
                }
                else if (resp.status == (int)ResponseCode.badEmail)
                {
                    MessageBox.Show("A bad email was entered! Make sure that the email is in the following forn:\n [start]@[end]\n start can have letters and numbers and end can have the domain names seperated with a dot");
                }
                else if (resp.status == (int)ResponseCode.badPassword)
                {
                    MessageBox.Show("A bad password was entered! Make sure the password is 8 characters long, contains a number, a capitalized and uncapitalized letter and one special character(!@#$^&*)");
                }
                else if (resp.status == (int)ResponseCode.badPhone)
                {
                    MessageBox.Show("A bad phone number was entered! Make sure the number is in the following form:\n [prefix]-[number]\n prefix can have 2 or 3 numbers and must start with 0!");
                }
                else if (resp.status == (int)ResponseCode.nameExsits)
                {
                    MessageBox.Show("A user with the same name already exists!");
                }
            }
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if(log==null)
            {
                log = new Login();
            }
            log.Show();
            this.Hide();
        }
    }
}
