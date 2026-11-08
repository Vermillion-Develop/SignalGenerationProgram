using System;
using System.Collections.Generic;
using System.Text;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;

namespace TestNUnit_QA
{
    internal class MainPage
    {
        private readonly Window _window;
        private readonly UIA3Automation _automation;
        private readonly Application _app;


        public MainPage(Window window, Application application, UIA3Automation automato)
        {
            _window= window;
            _automation = automato;
            _app = application;
        }
        public Window? CloseMessageBox()
        {
            Thread.Sleep(300);

            Window? msgBox = _window.FindFirstChild(f => f.ByControlType(ControlType.Window))?.AsWindow();
            msgBox?.Focus();
            Keyboard.Type(VirtualKeyShort.ENTER);
            return msgBox;
        }

        public Window? GetAllWindows()
        {
            Window? win = _app.GetAllTopLevelWindows(_automation).FirstOrDefault(f => f.Title != _window.Title);
            return win;
        }
        
        public void ClickButton(string automat)
        {
            Button? selectedButton = _window.FindFirstDescendant(f => f.ByAutomationId(automat))?.AsButton();
            selectedButton?.Click();
            Thread.Sleep(3000);
        }

        public TextBox? TextBoxElement(string automat, string znachenie)
        {
            TextBox? selectedText = _window.FindFirstDescendant(f => f.ByAutomationId(automat))?.AsTextBox();
            selectedText?.Enter(znachenie);
            return selectedText;
        }
        public TextBox? TextBoxElement(string automat)
        {
            TextBox? selectedText = _window.FindFirstDescendant(f => f.ByAutomationId(automat))?.AsTextBox();
            return selectedText;
        }

        public string ComboBoxElement(string automat, string znachenie)
        {
            ComboBox? combo = _window.FindFirstDescendant(f => f.ByAutomationId(automat))?.AsComboBox();
            combo?.Select(znachenie);
            switch (znachenie)
            {
                case "Синусоида": return "sinus";

                case "Меандр": return "meander";

                case "Треугольный": return "triangular";

                case "Пилообразный": return "sawtooth";

                default: return "null";
            }
        }

        public DataGridView? DataGridElement(string automat)
        {
            DataGridView? datagrd = _window.FindFirstDescendant(c => c.ByAutomationId(automat))?.AsDataGridView();
            return datagrd;
        }
    }
}
