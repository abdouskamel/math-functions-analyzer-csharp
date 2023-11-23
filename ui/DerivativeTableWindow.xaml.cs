using Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace ui
{
    /// <summary>
    /// Interaction logic for DerivativeTableWindow.xaml
    /// </summary>
    public partial class DerivativeTableWindow : Window
    {
        class DataGridItem
        {
            public int DerNum { get; set; }
            public string TexFormula { get; set; }
        }

        FunctionEntity TheFunction, LastDerivative;

        List<DataGridItem> ItemsHeap;
        ObservableCollection<DataGridItem> DerivativesBinding;

        public DerivativeTableWindow(FunctionEntity Func)
        {
            InitializeComponent();

            TheFunction = LastDerivative = Func;

            ItemsHeap = new List<DataGridItem>();
            DerivativesBinding = new ObservableCollection<DataGridItem>();

            ItemsHeap.Add(new DataGridItem { DerNum = 1, TexFormula = TheFunction.GetTexFormula() });
            DerivativesBinding.Add(ItemsHeap.Last());

            Binding binding = new Binding { Source = DerivativesBinding };
            DerivativeGrid.SetBinding(DataContextProperty, binding);
        }

        private void DerCount_LostFocus(object sender, RoutedEventArgs e)
        {
            int NewCount = DerCount.GetNumericValue();

            if (NewCount == DerivativesBinding.Count)
                return;

            else if(NewCount > DerivativesBinding.Count)
            {
                int i;
                if(NewCount > ItemsHeap.Count)
                {
                    for(i = ItemsHeap.Count; i < NewCount; ++i)
                    {
                        LastDerivative = LastDerivative.GetDerivative();
                        ItemsHeap.Add(new DataGridItem { DerNum = i + 1, TexFormula = LastDerivative.GetTexFormula() });
                    }
                }

                for(i = DerivativesBinding.Count; i < NewCount; ++i)
                {
                    DerivativesBinding.Add(ItemsHeap[i]);
                }
            }

            else
            {
                for(int i = DerivativesBinding.Count; i > NewCount; --i)
                {
                    DerivativesBinding.RemoveAt(DerivativesBinding.Count - 1);
                }
            }
        }
    }
}
