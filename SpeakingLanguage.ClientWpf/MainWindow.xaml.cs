using LiteNetLib.Utils;
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

namespace SpeakingLanguage.ClientWpf
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private NetDataWriter _dataWriter = new NetDataWriter();

        public MainWindow()
        {
            InitializeComponent();
            Bootstrap.Start();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            var words = txtCommand.Text.Split(' ');
            for (int i = 0; i != words.Length; i++)
            {

            }
        }

        private void CmbPacket_Initialized(object sender, EventArgs e)
        {
            for (var code = Protocol.Code.Packet.None; code != Protocol.Code.Packet.__MAX__; code++)
            {
                cmbPacket.Items.Add(code);
            }
        }

        private void CmbPacket_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var code = (Protocol.Code.Packet)cmbPacket.SelectedItem;
        }
    }
}
