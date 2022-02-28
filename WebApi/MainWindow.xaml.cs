using System.Windows;

namespace WebApi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WebServer webServer = new WebServer();
            // todo: Test stop() if webserver is listening 
            webServer.Stop();
            webServer.Get("/helloworld", () =>
            {
                return new Response(200, "<HTML><BODY> Hello world!</BODY></HTML>");
            });
            
            webServer.Get("/nadine", () =>
            {
                return new Response(200, "<HTML><BODY> <h1>Hoi Nadine</h1><h2><3</h2></BODY></HTML>");
            });

            webServer.Listen(8080);
        }
    }
}
