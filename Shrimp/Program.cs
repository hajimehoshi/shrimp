using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Shrimp.Presenters;
using Shrimp.Views;

namespace Shrimp
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var mainForm = new MainForm();
            var mainFormPresenter = new MainFormPresenter(mainForm, mainForm.ViewModel);
            mainFormPresenter.Run();
        }
    }
}
