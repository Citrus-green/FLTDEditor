using System;
using System.Windows;
using FLTD_lib;

namespace FLTDEditor
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 2)
            {
                if ((string)e.Args[0] == "-dump")
                {
                    FLTD st = new FLTD();
                    st.LoadFile((string)e.Args[1]);
                    st.DumpData((string)e.Args[1] + ".txt");
                }
                System.Windows.Application.Current.Shutdown();
            }
        }
    }

}