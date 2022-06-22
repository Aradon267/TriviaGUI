using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
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
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        public Thread threading = null;
        private Menu menuPage;
        TcpClient client;
        IPEndPoint serverEndPoint;
        NetworkStream clientStream;
        LoginRequest log;
        DispatcherTimer _timer;
        TimeSpan _time;
        int time;
        int totalQuestions;
        int correctAnswers = 0;
        private Helper help = new Helper();
        public Game(TcpClient _client, IPEndPoint _serverEndPoint, NetworkStream _clientStream, LoginRequest req, GetRoomStateResponse roomState)
        {
            this.time = roomState.answerTimeout;
            this.totalQuestions = roomState.questionCount;
            this.log = req;
            this.client = _client;
            this.serverEndPoint = _serverEndPoint;
            this.clientStream = _clientStream;
            this.Closed += JoinClosed;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            getQuestion();
        }

        public void getQuestion()
        {
            byte[] json = new byte[0];
            byte[] request = help.serializeMsg((int)RequestCode.GetQuestionCode, json);

            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

            byte[] respBytes = help.getFullMsg(this.clientStream);
            GetQuestionResponse resp = JsonSerializer.Deserialize<GetQuestionResponse>(respBytes);
            
            if(resp.status == (int)ResponseCode.noMoreQuestions)
            {
                this.questionText.Text = "Waiting for others to finish";
                this.Answer1Btn.Visibility = Visibility.Hidden;
                this.Answer2Btn.Visibility = Visibility.Hidden;
                this.Answer3Btn.Visibility = Visibility.Hidden;
                this.Answer4Btn.Visibility = Visibility.Hidden;
                this.tbTime.Visibility = Visibility.Hidden;
                this.correctText.Visibility = Visibility.Hidden;
                this.remainingText.Visibility = Visibility.Hidden;
                threading = new Thread(new ThreadStart(this.loadResults));
                threading.Start();
            }
            else
            {
                this.remainingText.Text = "Remaining questions: " + this.totalQuestions.ToString();
                this.totalQuestions--;
                this.questionText.Text = resp.question;
                this.Answer1Btn.Content = resp.answers["0"];
                this.Answer2Btn.Content = resp.answers["1"];
                this.Answer3Btn.Content = resp.answers["2"];
                this.Answer4Btn.Content = resp.answers["3"];

                _time = TimeSpan.FromSeconds(this.time);

                _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                {
                    tbTime.Text = _time.ToString("c");
                    if (_time == TimeSpan.Zero)
                    {
                        _timer.Stop();
                        timer_Tick();
                    }
                    _time = _time.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);

                _timer.Start();

            }
        }

        void loadResults()
        {
            GetGameResultsResponse resp = new GetGameResultsResponse();
            while (true)
            {
                byte[] json = new byte[0];
                byte[] request = help.serializeMsg((int)RequestCode.GetGameResultCode, json);

                clientStream.Write(request, 0, request.Length);
                clientStream.Flush();

                byte[] respBytes = help.getFullMsg(this.clientStream);
                var msResp = new MemoryStream(respBytes);
                var serResp = new DataContractJsonSerializer(resp.GetType());
                resp = serResp.ReadObject(msResp) as GetGameResultsResponse;
                msResp.Close();
                if(resp.status == (int)ResponseCode.gameNotFinished)
                {
                    int milliseconds = 5000;
                    Thread.Sleep(milliseconds);
                }
                else
                {
                    break;
                }
            }
            List<PlayerResults> sortedList = resp.results.OrderByDescending(o=>o.correctAnswerCount).ToList();
            Dispatcher.Invoke((Action)(() => this.listBox2.Visibility = Visibility.Visible));
            Dispatcher.Invoke((Action)(() => this.questionText.Text = "And the results are:"));
            foreach (var result in sortedList)
            {
                Dispatcher.Invoke((Action)(() => this.listBox2.Items.Add(new ListBoxItem
                {
                    Tag = result.username,
                    Content = result.username + " got " + result.correctAnswerCount.ToString() + " correct answers!"
                })));
            }
            threading.Abort();
        }


        void timer_Tick()
        {
            submit(7);
        }

        public void leaveGame()
        {
            byte[] json = new byte[0];
            byte[] request = help.serializeMsg((int)RequestCode.LeaveGameCode, json);

            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();

            byte[] respBytes = help.getFullMsg(this.clientStream);
            LeaveGameResponse resp = new LeaveGameResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as LeaveGameResponse;
            msResp.Close();
        }
        public void JoinClosed(object sender, EventArgs e)
        {
            byte[] json = new byte[0];
            byte[] request = help.serializeMsg(229, json);
            try
            {
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

        private void QuitGame(object sender, RoutedEventArgs e)
        {
            leaveGame();
            if (menuPage == null)
            {
                menuPage = new Menu(this.client, this.serverEndPoint, this.clientStream, this.log);
            }
            menuPage.Show();
            this.Hide();
        }
        public void submit(int ID)
        {
            _timer.Stop();
            SubmitAnswerRequest req = new SubmitAnswerRequest(ID);
            var ms = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(SubmitAnswerRequest));
            ser.WriteObject(ms, req);
            byte[] json = ms.ToArray();
            ms.Close();

            byte[] request = help.serializeMsg((int)RequestCode.SubmitAnswerCode, json);
            clientStream.Write(request, 0, request.Length);
            clientStream.Flush();


            byte[] respBytes = help.getFullMsg(this.clientStream);
            var resp = new SubmitAnswerResponse();
            var msResp = new MemoryStream(respBytes);
            var serResp = new DataContractJsonSerializer(resp.GetType());
            resp = serResp.ReadObject(msResp) as SubmitAnswerResponse;
            msResp.Close();
            if(ID == resp.correctAnswerID)
            {
                this.correctAnswers++;
                this.correctText.Text = "Correct answers: " + this.correctAnswers.ToString();
            }
            if(resp.status == (int)ResponseCode.success)
            {
                getQuestion();
            }
        }
        private void Answer1(object sender, RoutedEventArgs e)
        {
            submit(0);
        }
        private void Answer2(object sender, RoutedEventArgs e)
        {
            submit(1);
        }
        private void Answer3(object sender, RoutedEventArgs e)
        {
            submit(2);
        }
        private void Answer4(object sender, RoutedEventArgs e)
        {
            submit(3);
        }

    }
}
