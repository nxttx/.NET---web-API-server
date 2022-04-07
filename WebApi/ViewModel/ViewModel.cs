// INotifyPropertyChanged notifies the View of property changes, so that Bindings are updated.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using WebApi;
using WebApi.Commands;

sealed class MyViewModel : INotifyPropertyChanged
{
    private String _actionButtonValue = "StartServer";
    private String _portValue = "8080";
    private ObservableCollection<String> _logValue =new();
    public string ActionButton
    {
        get { return _actionButtonValue; } 
        set { if(_actionButtonValue != value) { _actionButtonValue = value; OnPropertyChange("ActionButton"); } }
    }
    public string Port
    {
        get { return _portValue; }
        set
        {
            int ignoreMe;
            if (_portValue != value && (int.TryParse(value, out ignoreMe)) || value == "")
            {
                _portValue = value;
                OnPropertyChange("Port");
            }
        }
    }
    
    public ObservableCollection<String> Log
    {
        get { return _logValue; } 
        set { _logValue = value; OnPropertyChange("Log"); }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    public IDelegateCommand StartStopCommand { protected set; get; }
    
    public MyViewModel()
    {
        StartStopCommand = new DelegateCommand(ExecuteStartStop);
    }
    
    public void AddToLog(String value)
    {
        _logValue.Insert(0, value);
        OnPropertyChange("Log");
    }
    public void ExecuteStartStop(object parameter)
    {
        if (!(MainWindow.webServer.GetIsRunning()))
        {
            Task.Run(() => MainWindow.webServer.Listen(int.Parse(Port)));
            ActionButton = "Stop Server";
            AddToLog("Server started on :"+ Port);
        }
        else
        {
            MainWindow.webServer.Close();
            ActionButton = "Start Server";
            AddToLog("Server stopped");
        }
    }

    protected void OnPropertyChange(string propertyName) {
        if(PropertyChanged != null) {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}