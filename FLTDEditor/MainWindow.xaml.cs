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

using FLTD_lib;

namespace FLTDEditor
{
    class FileIODialogHelper
    {
        public static FileIODialogHelper instance;
        public static String filePath;
        public void fileOpen() {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "*.fltd|*.fltd";
            if (dialog.ShowDialog() == true)
            {
                filePath = dialog.FileName;
                ((MainWindow)Application.Current.MainWindow).Update();
            }
        }
        public void fileSaveAs() {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "*.fltd|*.fltd";
            if (dialog.ShowDialog() == true)
            {
                filePath = dialog.FileName;
                fileSave();
            }
        }
        public void fileSave() { 
        
        }
        public void fileClose() {
            Application.Current.Shutdown();
        }
    }
    class ShortcutCmd : ICommand
    {
        private Action<Object> excute;
        private Func<Object, bool> canExcute;
        public ShortcutCmd(Action<Object> excute) : this(excute, o => true) { }
        public ShortcutCmd(Action<Object> excute, Func<Object, bool> canExcute)
        {
            this.excute = excute;
            this.canExcute = canExcute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Excute(object parameter)
        {
            Excute(parameter);
        }
        public event EventHandler CanExcuteChenged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
        public static void RaiseCanExcuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public void Execute(object parameter)
        {
            if (excute == null)
                return;

            excute(parameter);
        }
    }
    class MainWindowViewModel
    {
        public ShortcutCmd openFileCmd { get; private set; }
        public ShortcutCmd saveFileCmd { get; private set; }
        public ShortcutCmd saveAsFileCmd { get; private set; }
        public ShortcutCmd closeFileCmd { get; private set; }

        public MainWindowViewModel()
        {
            openFileCmd = new ShortcutCmd(fileOpenCmdBody);
            saveFileCmd = new ShortcutCmd(fileSaveCmdBody);
            saveAsFileCmd = new ShortcutCmd(fileSaveAsCmdBody);
            closeFileCmd = new ShortcutCmd(fileSaveAsCmdBody);
        }
        private void fileOpenCmdBody(object sender)
        {
            FileIODialogHelper.instance.fileOpen();
        }
        private void fileSaveCmdBody(object sender)
        {
            FileIODialogHelper.instance.fileSave();
        }
        private void fileSaveAsCmdBody(object sender)
        {
            FileIODialogHelper.instance.fileSaveAs();
        }
        private void fileCloseBody(object sender)
        {
            FileIODialogHelper.instance.fileClose();
        }
    }

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public FltdNGS st;
        public MainWindow()
        {
            InitializeComponent();
            FileIODialogHelper.instance = new FileIODialogHelper();
            DataContext = new MainWindowViewModel();
        }

        public void Update() {
            if (FileIODialogHelper.filePath != null)
            {
                st = new FltdNGS();
                st.loadFile(FileIODialogHelper.filePath);
                string[] str = st.GetAssignList();
                for (int i=0;i<str.Length;i++)
                assignList.Items.Add(str[i]);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void assignList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            rootNodeList.Items.Clear();
            constraintList.Items.Clear();

            if (assignList.Items.IsEmpty == true)
                return;

            string[] rootNodeName = st.GetRootMode(assignList.SelectedIndex);
            for (int i = 0; i < rootNodeName.Length; i++)
                rootNodeList.Items.Add(rootNodeName[i]);
            string[] constraintName = st.GetConstraintList(assignList.SelectedIndex);
            for (int i = 0; i < constraintName.Length; i++)
                constraintList.Items.Add(constraintName[i]);
        }

        private void constraintList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (constraintList.Items.IsEmpty == true)
                return;

        }
    }
}
