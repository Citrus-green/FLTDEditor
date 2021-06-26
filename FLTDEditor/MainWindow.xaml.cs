using System;
using System.Text.RegularExpressions;
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
                    MessageBox.Show($"Can't read file : {filePath}", "Error");
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
            if (st != null)
            {
                if (st.SaveFile(filePath, false) == false)
                {
                    MessageBox.Show($"Can't write file : {filePath}", "Error");

                }
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
                assignList.SelectedIndex = 0;
                assignTab.IsEnabled = true;
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
            foreach (string nodeName in  rootNodeName)
                rootNodeList.Items.Add(nodeName);
            string[] constraintName = FileIODialogHelper.st.GetConstraintList(assignList.SelectedIndex);
            foreach (string constraint in constraintName)
                constraintList.Items.Add(constraint);

            constraintList.SelectedIndex = 0;
        }

        private void constraintList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            constrateBones.Items.Clear();
            if (constraintList.Items.IsEmpty == true)
                return;

            string[] str = FileIODialogHelper.st.GetConstrateBones(assignList.SelectedIndex,constraintList.SelectedIndex);
            foreach(string name in str)
            {
                constrateBones.Items.Add(name);
            }
            constrateBones.SelectedIndex = 0;
        }
        private void constrateBones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            float[] f = FileIODialogHelper.st.GetConstrateParam(assignList.SelectedIndex, constraintList.SelectedIndex, constrateBones.SelectedIndex);
            switch (FileIODialogHelper.st.GetConstrateFormat(assignList.SelectedIndex, constraintList.SelectedIndex))
            {
                case 7:
                    direct.IsEnabled = false;
                    range.Text = f[0].ToString();
                    posX.Text = f[1].ToString();
                    posY.Text = f[2].ToString();
                    posZ.Text = f[3].ToString();
                    break;
            }
        
        }
        private void PreventTextInput(object sender, TextCompositionEventArgs e)
        {
            if (((TextBox)sender).Text.Length == 0 && e.Text == "-")
            {
                return;
            }

            Regex regex = new Regex("[^0-9.]");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void Param_LostFrocus(object sender, Object e) {
            if (((TextBox)sender).Text.Length == 0)
                ((TextBox)sender).Text = "0";

            float[] f;
            switch (FileIODialogHelper.st.GetConstrateFormat(assignList.SelectedIndex, constraintList.SelectedIndex))
            {
                case 7:
                    f = new float[4];
                    f[0]= Convert.ToSingle(range.Text);
                    f[1] = Convert.ToSingle(posX.Text);
                    f[2] = Convert.ToSingle(posY.Text);
                    f[3] = Convert.ToSingle(posZ.Text);
                    FileIODialogHelper.st.SetConstrateParam(assignList.SelectedIndex, constraintList.SelectedIndex, constrateBones.SelectedIndex, f);
                    break;
            }
        }

        private void PriventInput(object sender, KeyEventArgs e)
        {
            if (readOnly.IsChecked == true)
            {
                e.Handled = true;
            }
        }
    }
}
