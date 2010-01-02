using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Shrimp.Models;
using Shrimp.Presenters;
using Shrimp.Views;

namespace Shrimp
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var viewModel = new ViewModel();
            var mainForm = new MainForm(viewModel);
            var mainFormPresenter = new MainFormPresenter(mainForm, viewModel);
            mainFormPresenter.Run();
        }
    }
}
