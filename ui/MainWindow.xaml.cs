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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Rules;
using Rules.Utils;
using System.Drawing;
using fmath.controls;
using System.Windows.Interop;
using System.IO;

namespace ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string DefaultExpression = "sin(x) + 2x";

        FunctionEntity TheFunction, TheDerivative;
        FuncGraphicWindow GraphicWindow;

        public MainWindow()
        {
            MathMLFormulaControl.setFolderUrlForFonts("fonts");
            MathMLFormulaControl.setFolderUrlForGlyphs("glyphs");

            InitializeComponent();
            InitTheWindow();
        }

        private void InitTheWindow()
        {
            AnalyzeNewExp(DefaultExpression);
            ExpTextBox.Document = new FlowDocument(new Paragraph(new Run(DefaultExpression)));

            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "asset1.png", UriKind.RelativeOrAbsolute);
            ShowCalculTable.Source = BitmapFrame.Create(uri);

            uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "asset2.png", UriKind.RelativeOrAbsolute);
            ShowFuncGraphic.Source = BitmapFrame.Create(uri);
        }

        private void AnalyzeNewExp(string exp)
        {
            TheFunction = new FunctionEntity(exp);
            TheDerivative = TheFunction.GetDerivative();

            ErrorBlock.Text = string.Empty;
            InputBox.Clear();
            OutputBox.Clear();

            TexFormulaToImageSource converter = new TexFormulaToImageSource();

            UserTexFormula.Source = (ImageSource)converter.Convert(TheFunction.GetTexFormula(), typeof(ImageSource), null, null);
            DerivativeFormula.Source = (ImageSource)converter.Convert(TheDerivative.GetTexFormula(), typeof(ImageSource), null, null);

            ProgramStatusMsg.Content = "L'expression a été analysée.";
        }   

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AnalyzeNewExp(RichTextBoxText(ExpTextBox));
            }

            catch(TokensException _e)
            {
                ErrorBlock.Text = _e.Message;
                if(_e.ErrorPos.Begin != -1)
                {
                    TextPointer textpoint = ExpTextBox.Document.ContentStart,
                                ErrBeg = MyGetPositionAtOffset(textpoint, _e.ErrorPos.Begin),
                                ErrEnd = MyGetPositionAtOffset(textpoint, _e.ErrorPos.End);

                    TextRange ErrRange = new TextRange(ErrBeg, ErrEnd);
                    ErrRange.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));
                    ErrRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ExpTextBox.Document = new FlowDocument();
        }

        private void CalculateOutputButton_Click(object sender, RoutedEventArgs e)
        {
            if (InputBox.Text == string.Empty)
                return;

            if(!MyUtils.IsANumber(InputBox.Text))
            {
                MessageBox.Show("Veuillez entrer un nombre valide.", "Erreur critique", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                double output = TheFunction.GetOutput(InputBox.GetNumericValue());
                OutputBox.Text = output.ToString();
            }

            catch(OutputCalculException _e)
            {
                MessageBox.Show(_e.Message, "Erreur critique", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowCalculTable_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CalculTableWindow ShowOutputsWindow = new CalculTableWindow(TheFunction);
            ShowOutputsWindow.Show();
        }

        private void ShowDerivativesTable_Click(object sender, RoutedEventArgs e)
        {
            DerivativeTableWindow ShowDerivWindow = new DerivativeTableWindow(TheDerivative);
            ShowDerivWindow.Show();
        }

        private void ShowFuncGraphic_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (GraphicWindow != null)
                GraphicWindow.Close();

            GraphicWindow = new FuncGraphicWindow();
            GraphicWindow.Show();
        }

        // ---------------------------------------------------------------------------------------------------------- //
        public static string RichTextBoxText(RichTextBox textbox)
        {
            return new TextRange(textbox.Document.ContentStart, textbox.Document.ContentEnd).Text;
        }

        public static TextPointer MyGetPositionAtOffset(TextPointer start, int offset)
        {
            TextPointer ret = start;
            TextPointerContext context;
            int i = 0;

            while(i < offset && ret != null)
            {
                context = ret.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.Text || context == TextPointerContext.None)
                    i++;

                if (ret.GetPositionAtOffset(1, LogicalDirection.Forward) == null)
                    return ret;

                ret = ret.GetPositionAtOffset(1, LogicalDirection.Forward);
            }

            return ret;
        }
    }
}
