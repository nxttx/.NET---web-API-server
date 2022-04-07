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
        public static WebServer webServer;

        public MainWindow()
        {
            // webServer = new WebServer(InitializeComponent);
            // webServer.Start();
            webServer = new WebServer();
            
            webServer.Get("/helloworld", (parameters) =>
            {
                return new Response(200, "<HTML><BODY> Hello world!</BODY></HTML>");
            });
            
            webServer.Get("/gebruiker", (parameters) =>
            {
                return new Response(200, "<HTML><BODY> <h1>Hoi Gebruiker</h1><h2>Hoe is het met je vandaag?</h2></BODY></HTML>");
            });
            
            webServer.All("/all", (request) =>
            {
                var stringbuilder = new StringBuilder(500);
                foreach (var parameter in request.Parameters)
                {
                    stringbuilder.Append(parameter + ", ");
                }
                return new Response(201, "<HTML><BODY> <h1>"+request.Request.HttpMethod+"</h1><code>"+stringbuilder+"</code><br><br><code>"+request.Body+"</code></BODY></HTML>");

            });
            
            webServer.Listen(8080);

        }
    }
}
