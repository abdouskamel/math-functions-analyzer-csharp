using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Rules.Utils;

namespace ui
{
    class IntegerBox : TextBox
    {
        public IntegerBox() : base()
        {
            PreviewTextInput += NumberValidationTextBox;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public int GetNumericValue()
        {
            return Int32.Parse(Text);
        }
    }
}
