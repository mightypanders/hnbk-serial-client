﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Serial_Client
{
    public class MainViewModel : ViewModelBase
    {
        private int intervall;
        private bool running = false;
        private bool debug = false;
        private HnbkContext ctx = new HnbkContext();

        private ObservableCollection<Models.Measurement> tempDaten;
        public RelayCommand Closing { get; private set; }
        public RelayCommand StartRead { get; private set; }
        public RelayCommand StopRead { get; private set; }
        public RelayCommand FillTable { get; private set; }
        public RelayCommand GenerateTestValues { get; private set; }
        private void initButtonCommands()
        {
            this.Closing = new RelayCommand(() => { doClosingCommand(); });
            this.StartRead = new RelayCommand(
                () => { doStartReadCommand(); },
                () =>
                {
                    if (Worker != null)
                        return !Worker.Running;
                    else return true;

                });
            this.StopRead = new RelayCommand(
                () => { doStopReadCommand(); },
                () =>
                {
                    if (Worker != null)
                        return Worker.Running;
                    else
                        return false;
                });
            this.FillTable = new RelayCommand(() => { Task.Run(() => doFillTableCommand()); });
            this.GenerateTestValues = new RelayCommand(() => { doGenerateTestValuesCommand(); }, () => { return !Running && Debug; });
        }

        private void doClosingCommand()
        {
            doStopReadCommand();
        }

        public MainViewModel()
        {
#if DEBUG
            Debug = true;
#endif
            Worker = new RWWorker();
            tempDaten = new ObservableCollection<Models.Measurement>();
            initButtonCommands();
            Intervall = 5;
            PortOptions.ListAllComPorts();

            DBUrl = "sven.tech";
            Standort = "Frankenstrasse";
            Raum = "HNN000";
            Rechner = "PC01";

        }

        #region Commands
        private void doFillTableCommand()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                tempDaten.Clear();
                DateTime compare = DateTime.Now.AddDays(-10);
                var output = ctx.Measurements
                    //.Where(x => x.Date >= compare)
                    .ToList();
                output.OrderBy(x => x.Date);
                foreach (var item in output)
                {
                    tempDaten.Add(item);
                }
            });
        }
        private void doGenerateTestValuesCommand()
        {
            Task t = Task.Run(() =>
            {
                TestGenerator.generate(this.ctx);
            });
        }
        private void doStartReadCommand()
        {

            if (Worker == null)
            {
                Worker = new RWWorker();
                Worker.Init();
                Worker.doWork();
            }
            else
            {
                Worker.Init();
                Worker.doWork();
            }
        }
        private void doStopReadCommand()
        {
            if (Worker != null)
                Worker.stopWork();
        }
        #endregion

        #region Daten
        public ObservableCollection<Models.Measurement> TempDaten
        {
            get => tempDaten;
            set
            {
                if (value == tempDaten)
                    return;
                tempDaten = value;
                this.RaisePropertyChanged("TempDaten");
            }
        }
        public RWWorker Worker
        {
            get;
            private set;
        }
        #endregion

        #region Arduino Settings
        public string SelectedPort
        {
            get => PortOptions.PortName;
            set
            {
                if (value == PortOptions.PortName)
                    return;
                PortOptions.PortName = value;
                this.RaisePropertyChanged("SelectedPortName");
            }
        }
        public Handshake SelectedHandshake
        {
            get => PortOptions.Handshake;
            set
            {
                if (value == PortOptions.Handshake)
                    return;
                PortOptions.Handshake = value;
                this.RaisePropertyChanged("SelectedHandshake");
            }
        }
        public StopBits SelectedStopBits
        {
            get => PortOptions.StopBits;
            set
            {
                if (value == PortOptions.StopBits)
                    return;
                PortOptions.StopBits = value;
                this.RaisePropertyChanged("SelectedStopBits");
            }
        }
        public int SelectedBaudRate
        {
            get => PortOptions.BaudRate;
            set
            {
                if (value == PortOptions.BaudRate)
                    return;
                PortOptions.BaudRate = value;
                this.RaisePropertyChanged("SelectedBaudRate");
            }
        }
        public int SelectedDataBit
        {
            get => PortOptions.DataBits;
            set
            {
                if (value == PortOptions.DataBits)
                    return;
                PortOptions.DataBits = value;
                this.RaisePropertyChanged("SelectedDataBits");
            }
        }
        public int Intervall
        {
            get => intervall;
            set
            {
                if (value == intervall)
                    return;
                intervall = value;
                this.RaisePropertyChanged("Intervall");
            }
        }
        #endregion

        #region DB Settings
        public string DBUrl
        {
            get => Settings.DBUrl;
            set
            {
                if (value == Settings.DBUrl)
                    return;
                Settings.DBUrl = value;
                this.RaisePropertyChanged("DBUrl");
            }
        }
        public string Standort
        {
            get => Settings.Standort;
            set
            {
                if (value == Settings.Standort)
                    return;
                Settings.Standort = value;
                this.RaisePropertyChanged("Standort");
            }
        }
        public string Raum
        {
            get => Settings.Raum;
            set
            {
                if (value == Settings.Raum)
                    return;
                Settings.Raum = value;
                this.RaisePropertyChanged("Raum");
            }
        }
        public string Rechner
        {
            get => Settings.Rechner;
            set
            {
                if (value == Settings.Rechner)
                    return;
                Settings.Rechner = value;
                this.RaisePropertyChanged("Raum");
            }
        }
        #endregion

        #region 
        public bool Running
        {
            get
            {
                if (Worker != null)
                    return Worker.Running;
                else
                    return false;
            }
            set
            {
                if (value == running)
                    return;
                running = value;
                this.RaisePropertyChanged("Running");
            }
        }
        public bool Debug
        {
            get => debug;
            set
            {
                if (value == debug)
                    return;
                debug = value;
                this.RaisePropertyChanged("Debug");
            }
        }
        #endregion
    }
}
