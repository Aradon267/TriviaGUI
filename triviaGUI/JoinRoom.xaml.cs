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
    /// Interaction logic for JoinRoom.xaml
    /// </summary>
    public partial class JoinRoom : Window
    {
        public Thread threading = null;
        private WaitingRoom waitPage;
        private Menu menuPage;
        private Helper help = new Helper();
        List<RoomData> roomsList;
        TcpClient client;
        IPEndPoint serverEndPoint;
        NetworkStream clientStream;
        LoginRequest log;
        public JoinRoom(TcpClient _client, IPEndPoint _serverEndPoint, NetworkStream _clientStream, LoginRequest req)
        {
            this.log = req;
            this.Closed += JoinClosed;
            this.client = _client;
            this.serverEndPoint = _serverEndPoint;
            this.clientStream = _clientStream;
            InitializeComponent();
        }
        private void Window_Loaded(object sender, EventArgs e)
        {
            byte[] json = new byte[0];
            byte[] request = help.serializeMsg((int)RequestCode.GetRoomsRequestCode, json);

            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

            byte[] respBytes = help.getFullMsg(this.clientStream);
            GetRoomsResponse resp = new GetRoomsResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as GetRoomsResponse;
            msResp.Close();
            this.roomsList = resp.rooms;
            if(this.roomsList!=null)
            {
                foreach (RoomData room in this.roomsList)
                {
                    //this.listBox1.Items.Add(new ListBoxItem
                    //{
                    //    Tag = (int)room.id,
                    //    Content = room.name
                    //});
                    if (!room.isActive)
                    {
                        this.listBox1.Items.Add(new ListBoxItem
                        {
                            Tag = (int)room.id,
                            Content = room.name
                        });
                    }
                }
            }
            threading = new Thread(new ThreadStart(this.loadRooms));
            threading.Start();
        }
        public void JoinClosed(object sender, EventArgs e)
        {
            try
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
                threading.Abort();
                Application.Current.Shutdown();
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (menuPage == null)
            {
                menuPage = new Menu(this.client, this.serverEndPoint, this.clientStream, this.log);
            }
            threading.Abort();
            menuPage.Show();
            this.Hide();
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object item = this.listBox1.SelectedItem;
            ListBoxItem selected = listBox1.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
            int id = (int)selected.Tag;
            foreach(RoomData room in this.roomsList)
            {
                if(room.id == id)
                {
                    this.btnJoin.Visibility = Visibility.Visible;
                    this.nameBox.Text = room.name;
                    this.timeBox.Text = room.timePerQuestion.ToString();
                    this.maxBox.Text = room.maxPlayers.ToString();
                    this.countBox.Text = room.questionsCount.ToString();
                }
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            JoinRoomRequest req = new JoinRoomRequest();
            object item = this.listBox1.SelectedItem;
            ListBoxItem selected = listBox1.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
            int id = (int)selected.Tag;
            req.room_id = id;

            var ms = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(JoinRoomRequest));
            ser.WriteObject(ms, req);
            byte[] json = ms.ToArray();
            ms.Close();

            byte[] request = help.serializeMsg((int)RequestCode.JoinRoomRequestCode, json);
            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

            byte[] respBytes = help.getFullMsg(this.clientStream);
            var resp = new JoinRoomResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as JoinRoomResponse;
            msResp.Close();

            if(resp.status == (int)ResponseCode.success)
            {
                threading.Abort();
                if (waitPage == null)
                {
                    waitPage = new WaitingRoom(this.client, this.serverEndPoint, this.clientStream, resp, this.log);
                }
                waitPage.Show();
                this.Hide();
            }
            else if(resp.status == (int)ResponseCode.roomFull)
            {
                MessageBox.Show("The room you tried to enter is full!");
            }
            else if (resp.status == (int)ResponseCode.gameStarted)
            {
                MessageBox.Show("Game already started");
            }
        }
        private void loadRooms()
        {
            while(true)
            {
                int found = 0;
                byte[] json = new byte[0];
                byte[] request = help.serializeMsg((int)RequestCode.GetRoomsRequestCode, json);

                clientStream.Write(request, 0, request.Length);
                clientStream.Flush();

                byte[] respBytes = help.getFullMsg(this.clientStream);
                GetRoomsResponse resp = new GetRoomsResponse();
                var msResp = new MemoryStream(respBytes);
                var serResp = new DataContractJsonSerializer(resp.GetType());
                resp = serResp.ReadObject(msResp) as GetRoomsResponse;
                msResp.Close();
                if(resp.rooms!=null)
                {
                    if(this.roomsList==null)
                    {
                        this.roomsList = resp.rooms;
                        Dispatcher.Invoke((Action)(() => this.listBox1.Items.Clear()));
                        if (this.roomsList != null)
                        {
                            foreach (RoomData room in this.roomsList)
                            {
                                //Dispatcher.Invoke((Action)(() => this.listBox1.Items.Add(new ListBoxItem
                                //{
                                //    Tag = (int)room.id,
                                //    Content = room.name
                                //})));
                                if (!room.isActive)
                                {
                                    Dispatcher.Invoke((Action)(() => this.listBox1.Items.Add(new ListBoxItem
                                    {
                                        Tag = (int)room.id,
                                        Content = room.name
                                    })));
                                }
                            }
                        }
                    }
                    else
                    {
                        this.roomsList = resp.rooms;
                        //Dispatcher.Invoke((Action)(() => this.listBox1.Items.Clear()));
                        if (this.roomsList != null)
                        {
                            foreach (RoomData room in this.roomsList)
                            {
                                //Dispatcher.Invoke((Action)(() =>
                                //{
                                //    foreach (ListBoxItem item in this.listBox1.Items)
                                //    {
                                //        if ((int)item.Tag == room.id)
                                //        {
                                //            found = 1;
                                //            break;
                                //        }
                                //    }
                                //}));
                                if (!room.isActive)
                                {
                                    Dispatcher.Invoke((Action)(() =>
                                    {
                                        foreach (ListBoxItem item in this.listBox1.Items)
                                        {
                                            if ((int)item.Tag == room.id)
                                            {
                                                found = 1;
                                                break;
                                            }
                                        }
                                    }));
                                }

                                if (found == 0)
                                {
                                    //Dispatcher.Invoke((Action)(() => this.listBox1.Items.Add(new ListBoxItem
                                    //{
                                    //    Tag = (int)room.id,
                                    //    Content = room.name
                                    //})));
                                    if (!room.isActive)
                                    {
                                        Dispatcher.Invoke((Action)(() => this.listBox1.Items.Add(new ListBoxItem
                                        {
                                            Tag = (int)room.id,
                                            Content = room.name
                                        })));
                                    }
                                }
                                found = 0;
                            }
                        }
                    }
                }
                else
                {
                    Dispatcher.Invoke((Action)(() => this.listBox1.Items.Clear()));
                }
                int milliseconds = 3000;
                Thread.Sleep(milliseconds);
            }
        }
    }
}
