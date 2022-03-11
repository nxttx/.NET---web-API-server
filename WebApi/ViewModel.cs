// INotifyPropertyChanged notifies the View of property changes, so that Bindings are updated.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

sealed class MyViewModel : INotifyPropertyChanged
{
    private String ActionButtonValue = "StartServer";
    private String PortValue = "8080";
    private ObservableCollection<String> LogValue =new();
    
    public string ActionButton
    {
        get { return ActionButtonValue; } 
        set {
            if(ActionButtonValue != value) {
                ActionButtonValue = value;
                OnPropertyChange("ActionButton");
            }
        }
    }

    public string Port
    {
        get { return PortValue; }
        set
        {
            int ignoreMe;
            if (PortValue != value && (int.TryParse(value, out ignoreMe)))
            {
                PortValue = value;
                OnPropertyChange("Port");
            }
        }
    }
    
    public ObservableCollection<String> Log
    {
        get { return LogValue; } 
        set
        {
            LogValue = value;
            OnPropertyChange("Log");
        }
    }

    public void AddToLog(String value)
    {
        LogValue.Insert(0, value);
        OnPropertyChange("Log");
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChange(string propertyName) {
        if(PropertyChanged != null) {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}