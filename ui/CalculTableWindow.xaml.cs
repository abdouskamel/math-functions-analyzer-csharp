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
using System.Windows.Shapes;
using Rules;
using System.Collections.ObjectModel;
using Rules.Utils;

namespace ui
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
   
    public partial class CalculTableWindow : Window
    {
        FunctionEntity TheFunction;
        ObservableCollection<Point> PutCouples;
        double LastInterval;

        public CalculTableWindow(FunctionEntity TheFunction)
        {
            this.TheFunction = TheFunction;
            InitializeComponent();

            NbCouplesBox_LostFocus(null, null);
            CouplesTable.SetBinding(DataContextProperty, new Binding { Source = PutCouples });

            LastInterval = MyUtils.MyParseDouble(IntervalBox.Text);
        }

        private void NbCouplesBox_LostFocus(object sender, RoutedEventArgs e)
        {
            int NbCouples = NbCouplesBox.GetNumericValue();
            if (PutCouples == null)
                PutCouples = new ObservableCollection<Point>();

            if (NbCouples == PutCouples.Count)
                return;

            else if(NbCouples > PutCouples.Count)
            {
                double interval = IntervalBox.GetNumericValue(),
                       x = 0d, tmp;

                if (PutCouples.Count != 0)
                    x = PutCouples.Last().X + interval;

                for (int i = PutCouples.Count; i < NbCouples; ++i)
                {
                    try
                    {
                        tmp = TheFunction.GetOutput(x);
                    }

                    catch(OutputCalculException _e)
                    {
                        if (_e.ErrorType == CalculErrorType.POSITIVE_OVERFLOW)
                            tmp = double.PositiveInfinity;

                        else if (_e.ErrorType == CalculErrorType.NEGATIVE_OVERFLOW)
                            tmp = double.NegativeInfinity;

                        else
                            tmp = double.NaN;
                    }
                    
                    PutCouples.Add(new Point { X = x, Y = tmp });
                    x += interval;
                }
            }

            else
            {
                int i = PutCouples.Count - NbCouples;
                while(i > 0)
                {
                    PutCouples.RemoveAt(PutCouples.Count - 1);
                    --i;
                }
            }
        }

        private void IntervalBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IntervalBox.Text == string.Empty)
                return;

            if(!MyUtils.IsANumber(IntervalBox.Text))
            {
                MessageBox.Show("Veuillez entrer un nombre valide.", "Erreur critique", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double interval = MyUtils.MyParseDouble(IntervalBox.Text);
            if(interval != LastInterval)
            {
                PutCouples.Clear();
                NbCouplesBox_LostFocus(sender, e);
                LastInterval = interval;
            }
        }
    }
}
