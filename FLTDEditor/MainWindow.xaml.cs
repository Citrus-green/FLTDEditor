using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using FLTD_lib;

namespace FLTDEditor
{
    internal class FileIODialogHelper
    {

        public static FLTD st;
        public static FileIODialogHelper instance;
        private String filePath;
        public void fileOpen() {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "*.fltd|*.fltd";
            if (dialog.ShowDialog() == true)
            {
                filePath = dialog.FileName;

                st = new FLTD();
                if (st.LoadFile(filePath) == false)
                {
                    MessageBox.Show($"Can't open file : {filePath}", "Error");
                    st = null;
                }
                else
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
            if (st.SaveFile(filePath, false) == false)
            {
                MessageBox.Show($"Can't open file : {filePath}", "Error");

            }
        }
        public void fileClose() {
            Application.Current.Shutdown();
        }
        public void Dump() {
            if (st != null)
                st.DumpData(filePath + ".txt");
        }
    }
    internal class ShortcutCmd : ICommand
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
        public MainWindow()
        {
            InitializeComponent();
            FileIODialogHelper.instance = new FileIODialogHelper();
            DataContext = new MainWindowViewModel();
        }

        public void Update() {
            if (FileIODialogHelper.st != null)
            {
                assignList.Items.Clear();
                rootNodeList.Items.Clear();
                constraintList.Items.Clear();
                string[] str = FileIODialogHelper.st.GetAssignList();
                for (int i=0;i<str.Length;i++)
                assignList.Items.Add(str[i]);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Dump_data_click(object sender, RoutedEventArgs e)
        {
            FileIODialogHelper.instance.Dump();
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

            string[] rootNodeName = FileIODialogHelper.st.GetRootNode(assignList.SelectedIndex);
            for (int i = 0; i < rootNodeName.Length; i++)
                rootNodeList.Items.Add(rootNodeName[i]);
            string[] constraintName = FileIODialogHelper.st.GetConstraintList(assignList.SelectedIndex);
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
