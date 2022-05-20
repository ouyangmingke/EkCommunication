
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace EkCommunicationClient.EkCommands
{
    public class TabControlCommand : ICommand
    {
        private readonly Action<object> m_execute;
        /// <summary>
        /// 添加自定义回调函数
        /// </summary>
        /// <param name="execute"></param>
        public TabControlCommand(Action<object> execute)
        {
            this.m_execute = execute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
            throw new NotImplementedException();
        }

        public void CloseTabCommand()
        {
        }

        public void Execute(object? parameter)
        {
            if (parameter is TabItem tab)
            {
                if (tab.TemplatedParent is TabControl tabControl)
                {
                    tabControl.Items.Remove(tab);
                }
                if (tab.Parent is TabControl tabControl1)
                {
                    tabControl1.Items.Remove(tab);
                }
            }
            this.m_execute(parameter);
            //throw new NotImplementedException();
        }
    }
}
