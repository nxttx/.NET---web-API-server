using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

// todo: Test stop() if webserver is listening 
// todo: Test starting two servers
namespace WebApi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MyViewModel _viewModel;
        private WebServer _webServer;

        public MainWindow()
        {
            _viewModel = new MyViewModel();
            DataContext = _viewModel;
            
            _webServer = new WebServer(InitializeComponent);
            // webServer.Start();
            
            _webServer.Get("/helloworld", (parameters) =>
            {
                return new Response(200, "<HTML><BODY> Hello world!</BODY></HTML>");
            });
            
            _webServer.Get("/gebruiker", (parameters) =>
            {
                return new Response(200, "<HTML><BODY> <h1>Hoi Gebruiker</h1><h2>Hoe is het met je vandaag?</h2></BODY></HTML>");
            });
            
            _webServer.All("/all", (request) =>
            {
                var stringbuilder = new StringBuilder(500);
                foreach (var parameter in request.Parameters)
                {
                    stringbuilder.Append(parameter + ", ");
                }
                return new Response(201, "<HTML><BODY> <h1>"+request.Request.HttpMethod+"</h1><code>"+stringbuilder+"</code><br><br><code>"+request.Body+"</code></BODY></HTML>");

            });
            
            _webServer.Listen(8080);

        }
        private void ActionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_webServer.GetIsRunning())
            {
                Task.Run(() => _webServer.Listen(int.Parse(_viewModel.Port)));
                _viewModel.ActionButton = "Stop Server";
                _viewModel.AddToLog("Server started on :"+ _viewModel.Port);
            }
            else
            {
                _webServer.Close();
                _viewModel.ActionButton = "Start Server";
                _viewModel.AddToLog("Server stopped");
            }
        }
    }
}
