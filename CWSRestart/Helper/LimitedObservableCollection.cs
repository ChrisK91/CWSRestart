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
        private uint maxCapacity = 200;
        public uint MaxCapacity
        {
            get
            {
                return maxCapacity;
            }
            set
            {
                if (maxCapacity != value && MaxCapacity > 0)
                {
                    maxCapacity = value;
                    trimCollection();
                }
            }
        }

        protected override void InsertItem(int index, T item)
        {
            index = index - trimCollection();

            base.InsertItem(index, item);
        }

        private int trimCollection()
        {
            int removed = 0;

            while (this.Count >= MaxCapacity)
            {
                this.RemoveAt(0);
                removed++;
            }

            return removed;
        }
    }
}
