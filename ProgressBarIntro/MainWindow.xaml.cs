using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ProgressBarIntro
{
    public enum WorkType { Run1, Run2, Unknown }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string sApplicationName = "Progress Bar Class";
        int iMin = 0;
        int iMax = 100;
        BackgroundWorker worker;
        WorkType workType = WorkType.Unknown;

        public MainWindow()
        {
            InitializeComponent();
            RefreshTitle();
            prgProgressBar.Minimum = iMin;
            prgProgressBar.Maximum = iMax;
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            //set a class variable so that our worker knows which type of action to do in the worker
            workType = WorkType.Run1;
            CreateBackgroundWorker();
            RefreshTitle();
            EnableButtons(false);
        }

        private void btnRun2_Click(object sender, RoutedEventArgs e)
        {
            //set a class variable so that our worker knows which type of action to do in the worker
            workType = WorkType.Run2;
            prgProgressBar.IsIndeterminate = true;
            CreateBackgroundWorker();
            RefreshTitle();
            EnableButtons(false);
        }

        private void CreateBackgroundWorker()
        {
            //create the background worker and activate it
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Run1()
        {
            //run iterative
            for (int i = iMin; i <= iMax; i++)
            {
                worker.ReportProgress(i);
                Thread.Sleep(10);
            }
        }

        private void Run2()
        {
            //wait for 3 seconds
            Thread.Sleep(3000);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshTitle();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //run worker depending on what has been set in our class variable
            switch (workType)
            {
                case WorkType.Run1:
                    Run1();
                    break;
                case WorkType.Run2:
                    Run2();
                    break;
                default:
                    break;
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //update UI thread's progress bar
            prgProgressBar.Value = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //activated this event on completion of the background worker
            MessageBox.Show("Done!");
            prgProgressBar.Value = 0;
            prgProgressBar.IsIndeterminate = false;
            worker.Dispose();
            worker = null;
            RefreshTitle();
            EnableButtons(true);
        }

        private void RefreshTitle()
        {
            //add some info for the title
            this.Title = this.sApplicationName + " - Build Date "
                + DataAccess.GetBuildDate().ToString("M/d/yyyy h:mm tt")
                + " - " + Process.GetCurrentProcess().Threads.Count.ToString() + " threads running";
        }

        private void EnableButtons(bool bEnable)
        {
            //enables/disabled buttons based on input
            btnRun1.IsEnabled = bEnable;
            btnRun2.IsEnabled = bEnable;
            btnRefresh.IsEnabled = bEnable;
        }
    }
}
