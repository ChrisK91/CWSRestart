using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWSRestart.Helper
{
    public class LimitedObservableCollection<T> : ObservableCollection<T>
    {
        public UInt32 MaxCapacity = 200;

        protected override void InsertItem(int index, T item)
        {
            while(this.Count >= MaxCapacity){
                this.RemoveAt(0);
                index--;
            }

            base.InsertItem(index, item);
        }
    }
}
