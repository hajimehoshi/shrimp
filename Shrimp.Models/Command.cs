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

        public event EventHandler Done;
        public event EventHandler Undone;

        protected virtual void OnDone(EventArgs e)
        {
            if (this.Done != null) { this.Done(this, e); }
        }

        protected virtual void OnUndone(EventArgs e)
        {
            if (this.Undone != null) { this.Undone(this, e); }
        }
    }
}
