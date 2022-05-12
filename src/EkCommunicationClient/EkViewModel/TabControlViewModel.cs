using EkCommunicationClient.EkCommands;

using System.Windows;

namespace EkCommunicationClient.EkViewModel
{
    public class TabControlViewModel
    {
        private static readonly TabControlCommand tabControlCommand = new TabControlCommand((arg) =>
        {
            //string context = arg as string;
            //MessageBox.Show("Click " + context);
        });
        public TabControlCommand CloseItemCommand => tabControlCommand;


    }
}