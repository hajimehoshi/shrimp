using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shrimp.Models
{
    public interface ICommand
    {
        void Do();
        void Undo();
    }

    public class Command : ICommand
    {
        public void Do()
        {
            this.OnDone(EventArgs.Empty);
        }

        public void Undo()
        {
            this.OnUndone(EventArgs.Empty);
        }

        public event EventHandler Doing;
        public event EventHandler Undoing;

        protected virtual void OnDone(EventArgs e)
        {
            if (this.Doing != null) { this.Doing(this, e); }
        }

        protected virtual void OnUndone(EventArgs e)
        {
            if (this.Undoing != null) { this.Undoing(this, e); }
        }
    }
}
