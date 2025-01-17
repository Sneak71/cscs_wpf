﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WpfControlsLibrary
{
    public class DateEditer : DatePicker
    {
        string textBeforeChange;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var textBox = this.Template.FindName("PART_TextBox", this) as UIElement;
            if (textBox != null)
            {
                var dptb = textBox as DatePickerTextBox;
                
                dptb.PreviewTextInput += dptb_PreviewTextInput;

                dptb.PreviewKeyDown += dptb_PreviewKeyDown;
                dptb.SelectionChanged += dptb_SelectionChanged;

                dptb.LostFocus += Dptb_LostFocus;

                dptb.Loaded += Dptb_Loaded;

                dptb.Focus();
            }
        }

        
        public static readonly DependencyProperty DisplaySizeProperty = DependencyProperty.Register("DisplaySize", typeof(int), typeof(DateEditer));
        public int DisplaySize
        {
            get
            {
                return (int)base.GetValue(DisplaySizeProperty);
            }
            set
            {
                base.SetValue(DisplaySizeProperty, value);
            }
        }

        private void Dptb_Loaded(object sender, RoutedEventArgs e)
        {
            var dptb = (e.Source as DatePickerTextBox);

            dptb.SelectionChanged -= dptb_SelectionChanged;

            if (string.IsNullOrEmpty(dptb.Text))
            {
                if (DisplaySize == 8)
                    dptb.Text = "00/00/00";
                else if (DisplaySize == 10)
                    dptb.Text = "00/00/0000";

                dptb.SelectionStart = 0;
                dptb.SelectionLength = 1;
            }
            else
            {
                dptb.SelectAll();
            }
            
            dptb.SelectionChanged += dptb_SelectionChanged;
        }

        private void dptb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var dptb = (e.Source as DatePickerTextBox);

            dptb.SelectionChanged -= dptb_SelectionChanged;

            int[] allowedPositions = { 0, 1, 3, 4, 6, 7, 8, 9};
            if (!allowedPositions.Contains(dptb.SelectionStart))
            {
                    dptb.SelectionStart++;
            }
            
            if ((dptb.SelectionStart == 8 && DisplaySize == 8) || (dptb.SelectionStart == 10 && DisplaySize == 10))
                dptb.SelectionStart--;

            dptb.SelectionLength = 1;
            
            dptb.SelectionChanged += dptb_SelectionChanged;
        }
        private void dptb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var dptb = (e.Source as DatePickerTextBox);

            if (e.Key == System.Windows.Input.Key.Delete || e.Key == System.Windows.Input.Key.Back)
            {
                
                if (dptb.SelectionLength == DisplaySize)
                {
                    var date111 = new DateTime(1, 1, 1);
                    dptb.Text = date111.ToString(GetPattern());
                }
                else
                {
                    var index = dptb.SelectionStart;
                    var textArray = dptb.Text.ToArray();
                   textArray[index] = '0';
                    dptb.Text = new string(textArray);
                    dptb.SelectionStart = index;
                }
                e.Handled = true;
            }
            else if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                if(DateTime.TryParseExact(dptb.Text, GetPattern(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    result = result.AddDays(1);
                    dptb.Text = result.ToString(GetPattern());
                }
            }
            else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                if (DateTime.TryParseExact(dptb.Text, GetPattern(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                {
                    result = result.AddDays(-1);
                    dptb.Text = result.ToString(GetPattern());
                }
            }

            if (dptb.SelectionLength != 1)
                dptb.SelectionLength = 1;

            textBeforeChange = dptb.Text;

            if (e.Key == System.Windows.Input.Key.Left || e.Key == System.Windows.Input.Key.Up || e.Key == System.Windows.Input.Key.Back)
            {
                e.Handled = true;
                if (dptb.SelectionStart == 0)
                    return;
                if (dptb.SelectionStart == 3 || dptb.SelectionStart == 6)
                    dptb.SelectionStart = dptb.SelectionStart - 2;
                else
                    dptb.SelectionStart--;
            }
        }
        private void dptb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(e.Text, out int x))
            {
                e.Handled = true;
            }
        }

        private void Dptb_LostFocus(object sender, RoutedEventArgs e)
        {
            var dptb = (e.Source as DatePickerTextBox);

            var text = dptb.Text;

            if (!DateTime.TryParseExact(text, GetPattern(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime res))
            {
                var selStart = dptb.SelectionStart;
                dptb.Text = textBeforeChange;
                var newPosition = selStart - 1;
                dptb.SelectionStart = (newPosition < 0) ? 0 : newPosition;
            }
        }

        private string GetPattern()
        {
            if (DisplaySize == 8)
            {
                return "dd/MM/yy";
            }
            else if (DisplaySize == 10)
            {
                return "dd/MM/yyyy";
            }
            else return "";
        }

    }
}
