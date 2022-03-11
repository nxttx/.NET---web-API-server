using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


            
// todo: Test stop() if webserver is listening 
// todo: Test starting two servers
// todo: Statuscodes.

namespace WebApi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MyViewModel _viewModel;
        private WebServer webServer;

        public MainWindow()
        {
            _viewModel = new MyViewModel();
            DataContext = _viewModel;
            
            webServer = new WebServer(InitializeComponent);
            webServer.Start();
            
            webServer.Get("/helloworld", () =>
            {
                return new Response(200, "<HTML><BODY> Hello world!</BODY></HTML>");
            });
            
            webServer.Get("/gebruiker", () =>
            {
                return new Response(200, "<HTML><BODY> <h1>Hoi Gebruiker</h1><h2><3</h2></BODY></HTML>");
            });
        }
        private void ActionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!webServer.GetIsRunning())
            {
                Task.Run(() => webServer.Listen(int.Parse(_viewModel.Port)));
                _viewModel.ActionButton = "Stop Server";
                _viewModel.AddToLog("Server started on :"+ _viewModel.Port);
            }
            else
            {
                webServer.Close();
                _viewModel.ActionButton = "Start Server";
                _viewModel.AddToLog("Server stopped");
            }
        }
    }
}
