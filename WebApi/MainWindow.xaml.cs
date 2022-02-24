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
