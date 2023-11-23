using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using Rules.Utils;

namespace ui
{
    class DecimalBox : TextBox
    {
        public DecimalBox() : base()
        {
            PreviewTextInput += NumberValidationTextBox;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public double GetNumericValue()
        {
            return MyUtils.MyParseDouble(Text);
        }
    }
}
