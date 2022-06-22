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
using System.Threading;

namespace triviaGUI
{
    /// <summary>
    /// Interaction logic for WaitingRoom.xaml
    /// </summary>
    public partial class WaitingRoom : Window
    {
        private Menu menuPage;
        private Game gamePage;
        public Thread threading = null;
        bool isAdmin = false;
        TcpClient client;
        IPEndPoint serverEndPoint;
        NetworkStream clientStream;
        LoginRequest log;
        List<string> players;
        GetRoomStateResponse roomState;
        private Helper help = new Helper();
        public Mutex Mutex;
        int room_id;
        public WaitingRoom(TcpClient _client, IPEndPoint _serverEndPoint, NetworkStream _clientStream, CreateRoomResponse resp, LoginRequest req)
        {
            this.log = req;
            this.isAdmin = true;
            this.Closed += JoinClosed;
            this.room_id = resp.room_id;
            this.client = _client;
            this.serverEndPoint = _serverEndPoint;
            this.clientStream = _clientStream;
            InitializeComponent();
        }
        public WaitingRoom(TcpClient _client, IPEndPoint _serverEndPoint, NetworkStream _clientStream, JoinRoomResponse resp, LoginRequest req)
        {
            this.log = req;
            this.Closed += JoinClosed;
            this.room_id = resp.room_id;
            this.client = _client;
            this.serverEndPoint = _serverEndPoint;
            this.clientStream = _clientStream;
            InitializeComponent();
        }
        private void Window_Loaded(object sender, EventArgs e)
        {
            if(isAdmin)
            {
                this.closeBtn.Visibility = Visibility.Visible;
                this.startBtn.Visibility = Visibility.Visible;
            }
            else
            {
                this.leaveBtn.Visibility = Visibility.Visible;
            }
            

            byte[] json = new byte[0];
            byte[] request = help.serializeMsg((int)RequestCode.GetRoomStateCode, json);

            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

          

            byte[] respBytes = help.getFullMsg(this.clientStream);
            var resp = new GetRoomStateResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as GetRoomStateResponse;
            msResp.Close();
            this.roomState = resp;
            this.players = resp.players;
            foreach (string name in this.players)
            {
                this.playersList.Items.Add(name);
            }
            threading = new Thread(new ThreadStart(this.loadPlayers));
            threading.Start();
        }

        public void closeRoomAfterLogout()
        {
            byte[] json = new byte[0];
            byte[] request = help.serializeMsg((int)RequestCode.CloseRoomCode, json);

            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

            byte[] respBytes = help.getFullMsg(this.clientStream);
            CloseRoomResponse resp = new CloseRoomResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as CloseRoomResponse;
            msResp.Close();
        }
        public void leaveRoomAfterLogout()
        {
            byte[] json = new byte[0];
            byte[] request = help.serializeMsg((int)RequestCode.LeaveRoomCode, json);

            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

            byte[] respBytes = help.getFullMsg(this.clientStream);
            LeaveRoomResponse resp = new LeaveRoomResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as LeaveRoomResponse;
            msResp.Close();
        }
        public void JoinClosed(object sender, EventArgs e)
        {
            threading.Abort();
            byte[] json = new byte[0];
            byte[] request = help.serializeMsg(229, json);
            try
            {
                if (isAdmin)
                {
                    closeRoomAfterLogout();
                }
                else
                {
                    leaveRoomAfterLogout();
                }
                clientStream.Write(request, 0, request.Length);
                clientStream.Flush();

                byte[] respBytes = help.getFullMsg(this.clientStream);
                LogoutResponse resp = new LogoutResponse();
                var msResp = new MemoryStream(respBytes);
                var serResp = new DataContractJsonSerializer(resp.GetType());
                resp = serResp.ReadObject(msResp) as LogoutResponse;
                msResp.Close();
            }
            catch (Exception ex)
            {

            }
            Application.Current.Shutdown();
        }
        private void loadPlayers()
        {
            while(true)
            {
                byte[] json = new byte[0];
                byte[] request = help.serializeMsg((int)RequestCode.GetRoomStateCode, json);

                clientStream.Write(request, 0, request.Length);
                clientStream.Flush();

                

                byte[] respBytes = help.getFullMsg(this.clientStream);
                var resp = new GetRoomStateResponse();
                var msResp = new MemoryStream(respBytes);
                var serResp = new DataContractJsonSerializer(resp.GetType());
                resp = serResp.ReadObject(msResp) as GetRoomStateResponse;
                msResp.Close();
                if (!resp.hasGameBegun)
                {
                    if (resp.status == (int)ResponseCode.success)
                    {
                        this.players = resp.players;
                        Dispatcher.Invoke((Action)(() => this.playersList.Items.Clear()));
                        foreach (string name in this.players)
                        {
                            Dispatcher.Invoke((Action)(() => this.playersList.Items.Add(name)));
                        }
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() => {
                            if (menuPage == null)
                            {
                                menuPage = new Menu(this.client, this.serverEndPoint, this.clientStream, this.log);
                            }
                            menuPage.Show();
                            this.Hide();
                        });
                        threading.Abort();
                    }
                }
                else
                {
                    this.Dispatcher.Invoke(() => {
                        if (gamePage == null)
                        {
                            gamePage = new Game(this.client, this.serverEndPoint, this.clientStream, this.log, this.roomState);
                        }
                        gamePage.Show();
                        this.Hide();
                    });
                    threading.Abort();
                    break;
                }
                int milliseconds = 3000;
                Thread.Sleep(milliseconds);
            }
            
        }


        private void leaveBtn_Click(object sender, RoutedEventArgs e)
        {
            byte[] json = new byte[0];
            byte[] request = help.serializeMsg((int)RequestCode.LeaveRoomCode, json);

            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

            byte[] respBytes = help.getFullMsg(this.clientStream);
            LeaveRoomResponse resp = new LeaveRoomResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as LeaveRoomResponse;
            msResp.Close();
            if(resp.status == (int)ResponseCode.success)
            {
                threading.Abort();
                if (menuPage == null)
                {
                    menuPage = new Menu(this.client, this.serverEndPoint, this.clientStream, this.log);
                }
                menuPage.Show();
                this.Hide();
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            threading.Abort();
            byte[] json = new byte[0];
            byte[] request = help.serializeMsg((int)RequestCode.CloseRoomCode, json);

            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

            byte[] respBytes = help.getFullMsg(this.clientStream);
            CloseRoomResponse resp = new CloseRoomResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as CloseRoomResponse;
            msResp.Close();
            if (resp.status == (int)ResponseCode.success)
            {
                if (menuPage == null)
                {
                    menuPage = new Menu(this.client, this.serverEndPoint, this.clientStream, this.log);
                }
                menuPage.Show();
                this.Hide();
            }
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            threading.Abort();
            byte[] json = new byte[0];
            byte[] request = help.serializeMsg((int)RequestCode.StartGameCode, json);

            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

            byte[] respBytes = help.getFullMsg(this.clientStream);
            StartGameResponse resp = new StartGameResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as StartGameResponse;
            msResp.Close();
            if (resp.status == (int)ResponseCode.success)
            {
                this.Dispatcher.Invoke(() => {
                    if (gamePage == null)
                    {
                        gamePage = new Game(this.client, this.serverEndPoint, this.clientStream, this.log, this.roomState);
                    }
                    gamePage.Show();
                    this.Hide();
                });
                threading.Abort();
            }
        }
    }
}
