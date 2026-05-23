using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NumberSystemCalculator
{
    /// <summary>
    /// Main application window. Renders a modern dark-themed multi-base
    /// calculator with real-time conversion between BIN/OCT/DEC/HEX,
    /// arithmetic and bitwise operations, and a calculation history panel.
    /// </summary>
    public partial class Form1 : Form
    {
        // ---- Color palette (modern dark theme) ----
        private static readonly Color BgColor       = Color.FromArgb(30, 30, 46);
        private static readonly Color PanelColor    = Color.FromArgb(40, 42, 60);
        private static readonly Color PanelColor2   = Color.FromArgb(49, 50, 68);
        private static readonly Color HeaderColor   = Color.FromArgb(24, 24, 37);
        private static readonly Color AccentColor   = Color.FromArgb(124, 58, 237);  // purple
        private static readonly Color AccentColor2  = Color.FromArgb(59, 130, 246);  // blue
        private static readonly Color TextColor     = Color.FromArgb(226, 232, 240);
        private static readonly Color TextMuted     = Color.FromArgb(148, 163, 184);
        private static readonly Color DigitColor    = Color.FromArgb(68, 71, 90);
        private static readonly Color HexDigitColor = Color.FromArgb(94, 71, 130);
        private static readonly Color OpColor       = Color.FromArgb(82, 90, 130);
        private static readonly Color ErrorColor    = Color.FromArgb(239, 68, 68);
        private static readonly Color OkColor       = Color.FromArgb(34, 197, 94);

        private readonly CalculatorEngine _engine = new CalculatorEngine();
        private int _currentBase = 10;

        // ---- Controls ----
        private TextBox _binField, _octField, _decField, _hexField;
        private Label _binLabel, _octLabel, _decLabel, _hexLabel;
        private TextBox _mainDisplay;
        private ListBox _historyList;
        private Label _statusLabel;
        private Label _baseHintLabel;
        private Button _btnBin, _btnOct, _btnDec, _btnHex;
        private readonly List<Button> _hexLetterButtons = new List<Button>();
        private readonly List<Button> _digitButtons = new List<Button>();
        private bool _suppressFieldEvents;
        private bool _justComputed;

        /// <summary>Initializes the form and builds the entire UI tree.</summary>
        public Form1()
        {
            InitializeComponent();
            BuildUi();
            this.KeyDown += Form1_KeyDown;
            this.KeyPress += Form1_KeyPress;
            UpdateBaseHighlight();
            SetMainDisplay("0");
            UpdateAllFieldsFromDecimal(0);
            SetStatus($"Текущая система: {BaseName(_currentBase)}", false);
        }

        // ============================================================
        //  UI CONSTRUCTION
        // ============================================================
        private void BuildUi()
        {
            this.BackColor = BgColor;
            this.ForeColor = TextColor;
            this.Font = new Font("Segoe UI", 9.5f);

            // ---- Header ----
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = HeaderColor
            };
            var iconBox = new Label
            {
                Text = "☰",
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = AccentColor,
                AutoSize = false,
                Size = new Size(50, 50),
                Location = new Point(15, 5),
                TextAlign = ContentAlignment.MiddleCenter
            };
            var title = new Label
            {
                Text = "Калькулятор систем счисления",
                Font = new Font("Segoe UI Semibold", 15f, FontStyle.Bold),
                ForeColor = TextColor,
                AutoSize = false,
                Location = new Point(65, 12),
                Size = new Size(600, 36),
                TextAlign = ContentAlignment.MiddleLeft
            };
            header.Controls.Add(iconBox);
            header.Controls.Add(title);
            this.Controls.Add(header);

            // ---- Status bar ----
            var statusBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 28,
                BackColor = HeaderColor
            };
            _statusLabel = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = TextMuted,
                Font = new Font("Segoe UI", 9f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                Text = "Готов."
            };
            statusBar.Controls.Add(_statusLabel);
            this.Controls.Add(statusBar);

            // ---- Right history panel ----
            var historyPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 290,
                BackColor = PanelColor,
                Padding = new Padding(12)
            };
            var historyTitle = new Label
            {
                Text = "История",
                Dock = DockStyle.Top,
                Height = 30,
                ForeColor = AccentColor,
                Font = new Font("Segoe UI Semibold", 11f, FontStyle.Bold)
            };
            _historyList = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = PanelColor2,
                ForeColor = TextColor,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 10f),
                IntegralHeight = false,
                ItemHeight = 22
            };
            var btnClearHist = MakeButton("Очистить историю", OpColor, 0, 0, 264, 32);
            btnClearHist.Dock = DockStyle.Bottom;
            btnClearHist.Click += (s, e) =>
            {
                _engine.ClearHistory();
                RefreshHistory();
                SetStatus("История очищена.", false);
            };
            historyPanel.Controls.Add(_historyList);
            historyPanel.Controls.Add(btnClearHist);
            historyPanel.Controls.Add(historyTitle);
            this.Controls.Add(historyPanel);

            // ---- Main center panel ----
            var center = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BgColor,
                Padding = new Padding(15)
            };
            this.Controls.Add(center);

            // -- Conversion fields panel --
            var convPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = PanelColor,
                Padding = new Padding(10)
            };

            (_binLabel, _binField) = AddConversionRow(convPanel, "BIN", 10);
            (_octLabel, _octField) = AddConversionRow(convPanel, "OCT", 42);
            (_decLabel, _decField) = AddConversionRow(convPanel, "DEC", 74);
            (_hexLabel, _hexField) = AddConversionRow(convPanel, "HEX", 106);

            _binField.TextChanged += (s, e) => OnFieldEdited(_binField, 2);
            _octField.TextChanged += (s, e) => OnFieldEdited(_octField, 8);
            _decField.TextChanged += (s, e) => OnFieldEdited(_decField, 10);
            _hexField.TextChanged += (s, e) => OnFieldEdited(_hexField, 16);

            center.Controls.Add(convPanel);

            // -- Base selector --
            var basePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = BgColor,
                Padding = new Padding(0, 8, 0, 4)
            };
            _btnBin = MakeBaseButton("BIN", 2);
            _btnOct = MakeBaseButton("OCT", 8);
            _btnDec = MakeBaseButton("DEC", 10);
            _btnHex = MakeBaseButton("HEX", 16);
            var baseFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = BgColor,
                WrapContents = false
            };
            baseFlow.Controls.Add(_btnBin);
            baseFlow.Controls.Add(_btnOct);
            baseFlow.Controls.Add(_btnDec);
            baseFlow.Controls.Add(_btnHex);
            _baseHintLabel = new Label
            {
                Text = "  Активная система: DEC",
                ForeColor = TextMuted,
                AutoSize = false,
                Dock = DockStyle.Right,
                Width = 220,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Italic)
            };
            basePanel.Controls.Add(_baseHintLabel);
            basePanel.Controls.Add(baseFlow);
            center.Controls.Add(basePanel);

            // -- Main display --
            var displayPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = PanelColor2,
                Padding = new Padding(12, 8, 12, 8)
            };
            _mainDisplay = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = PanelColor2,
                ForeColor = TextColor,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 26f, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Right,
                Text = "0"
            };
            _mainDisplay.KeyPress += MainDisplay_KeyPress;
            _mainDisplay.TextChanged += MainDisplay_TextChanged;
            displayPanel.Controls.Add(_mainDisplay);
            center.Controls.Add(displayPanel);

            // -- Buttons grid --
            var buttonsArea = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BgColor,
                Padding = new Padding(0, 10, 0, 0)
            };
            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = BgColor,
                ColumnCount = 7,
                RowCount = 5
            };
            for (int i = 0; i < 7; i++)
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 7));
            for (int i = 0; i < 5; i++)
                grid.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));

            // Layout:
            // Row 0: C  ←   AND  OR   XOR  NOT  =
            // Row 1: D  E   F    /    *    -    +
            // Row 2: A  B   C    <<   >>   ±
            // Row 3: 7  8   9
            // Row 4: 4  5   6
            // Better: arrange compactly.
            // We'll use 7 cols x 5 rows:
            grid.Controls.Add(MakeOpButton("C", (s, e) => OnClear()),               0, 0);
            grid.Controls.Add(MakeOpButton("←", (s, e) => OnBackspace()),           1, 0);
            grid.Controls.Add(MakeOpButton("AND", (s, e) => OnOperation(Operation.And)), 2, 0);
            grid.Controls.Add(MakeOpButton("OR",  (s, e) => OnOperation(Operation.Or)),  3, 0);
            grid.Controls.Add(MakeOpButton("XOR", (s, e) => OnOperation(Operation.Xor)), 4, 0);
            grid.Controls.Add(MakeOpButton("NOT", (s, e) => OnNot()),               5, 0);
            grid.Controls.Add(MakeAccentButton("=", (s, e) => OnEquals()),          6, 0);

            grid.Controls.Add(MakeHexButton("D"),                                   0, 1);
            grid.Controls.Add(MakeHexButton("E"),                                   1, 1);
            grid.Controls.Add(MakeHexButton("F"),                                   2, 1);
            grid.Controls.Add(MakeOpButton("/",  (s, e) => OnOperation(Operation.Divide)),   3, 1);
            grid.Controls.Add(MakeOpButton("*",  (s, e) => OnOperation(Operation.Multiply)), 4, 1);
            grid.Controls.Add(MakeOpButton("<<", (s, e) => OnOperation(Operation.ShiftLeft)),5, 1);
            grid.Controls.Add(MakeOpButton(">>", (s, e) => OnOperation(Operation.ShiftRight)),6,1);

            grid.Controls.Add(MakeHexButton("A"),                                   0, 2);
            grid.Controls.Add(MakeHexButton("B"),                                   1, 2);
            grid.Controls.Add(MakeHexButton("C"),                                   2, 2);
            grid.Controls.Add(MakeDigit("7"),                                       3, 2);
            grid.Controls.Add(MakeDigit("8"),                                       4, 2);
            grid.Controls.Add(MakeDigit("9"),                                       5, 2);
            grid.Controls.Add(MakeOpButton("-", (s, e) => OnOperation(Operation.Subtract)), 6, 2);

            grid.Controls.Add(MakeOpButton("±", (s, e) => OnToggleSign()),          0, 3);
            grid.Controls.Add(MakeDigit("0"),                                       1, 3);
            grid.Controls.Add(MakeDigit("1"),                                       2, 3);
            grid.Controls.Add(MakeDigit("4"),                                       3, 3);
            grid.Controls.Add(MakeDigit("5"),                                       4, 3);
            grid.Controls.Add(MakeDigit("6"),                                       5, 3);
            grid.Controls.Add(MakeOpButton("+", (s, e) => OnOperation(Operation.Add)),      6, 3);

            // Row 4 — fill with remaining digits
            grid.Controls.Add(MakeDigit("2"),                                       2, 4);
            grid.Controls.Add(MakeDigit("3"),                                       3, 4);

            buttonsArea.Controls.Add(grid);
            center.Controls.Add(buttonsArea);

            // Order matters for docking — re-add center last so it stretches inside.
            // (Already added above; Z-order handled by docking.)
        }

        private (Label, TextBox) AddConversionRow(Panel parent, string label, int y)
        {
            var lbl = new Label
            {
                Text = label,
                Location = new Point(8, y + 4),
                Size = new Size(50, 22),
                ForeColor = AccentColor2,
                Font = new Font("Segoe UI Semibold", 10f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            var tb = new TextBox
            {
                Location = new Point(65, y),
                Size = new Size(580, 22),
                BackColor = PanelColor2,
                ForeColor = TextColor,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 11f),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            parent.Controls.Add(lbl);
            parent.Controls.Add(tb);
            return (lbl, tb);
        }

        // ============================================================
        //  Factory helpers for styled buttons
        // ============================================================
        private Button MakeButton(string text, Color back, int x, int y, int w, int h)
        {
            var b = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = back,
                ForeColor = TextColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(back, 0.15f);
            b.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(back, 0.1f);
            return b;
        }

        private Button MakeBaseButton(string label, int baseValue)
        {
            var b = MakeButton(label, PanelColor, 0, 0, 80, 36);
            b.Margin = new Padding(4, 4, 4, 4);
            b.Tag = baseValue;
            b.Click += (s, e) => SetCurrentBase(baseValue);
            return b;
        }

        private Button MakeDigit(string digit)
        {
            var b = MakeButton(digit, DigitColor, 0, 0, 0, 0);
            b.Dock = DockStyle.Fill;
            b.Margin = new Padding(3);
            b.Font = new Font("Segoe UI Semibold", 13f, FontStyle.Bold);
            b.Tag = digit;
            b.Click += (s, e) => OnDigit(digit);
            _digitButtons.Add(b);
            return b;
        }

        private Button MakeHexButton(string letter)
        {
            var b = MakeButton(letter, HexDigitColor, 0, 0, 0, 0);
            b.Dock = DockStyle.Fill;
            b.Margin = new Padding(3);
            b.Font = new Font("Segoe UI Semibold", 13f, FontStyle.Bold);
            b.Tag = letter;
            b.Click += (s, e) => OnDigit(letter);
            _hexLetterButtons.Add(b);
            return b;
        }

        private Button MakeOpButton(string text, EventHandler handler)
        {
            var b = MakeButton(text, OpColor, 0, 0, 0, 0);
            b.Dock = DockStyle.Fill;
            b.Margin = new Padding(3);
            b.Font = new Font("Segoe UI Semibold", 12f, FontStyle.Bold);
            b.Click += handler;
            return b;
        }

        private Button MakeAccentButton(string text, EventHandler handler)
        {
            var b = MakeButton(text, AccentColor, 0, 0, 0, 0);
            b.Dock = DockStyle.Fill;
            b.Margin = new Padding(3);
            b.Font = new Font("Segoe UI Semibold", 14f, FontStyle.Bold);
            b.Click += handler;
            return b;
        }

        // ============================================================
        //  Event handlers / behaviors
        // ============================================================

        private void SetCurrentBase(int newBase)
        {
            if (_currentBase == newBase) return;

            // Preserve the current value across base change.
            long current = TryParseCurrent(out bool ok);
            _currentBase = newBase;
            UpdateBaseHighlight();
            if (ok)
                SetMainDisplay(NumberConverter.FromDecimal(current, newBase));
            else
                SetMainDisplay("0");
            SetStatus($"Текущая система: {BaseName(newBase)}", false);
        }

        private void UpdateBaseHighlight()
        {
            foreach (var btn in new[] { _btnBin, _btnOct, _btnDec, _btnHex })
            {
                int b = (int)btn.Tag;
                bool active = b == _currentBase;
                btn.BackColor = active ? AccentColor : PanelColor;
                btn.ForeColor = active ? Color.White : TextColor;
                btn.FlatAppearance.MouseOverBackColor =
                    active ? ControlPaint.Light(AccentColor, 0.15f)
                           : ControlPaint.Light(PanelColor, 0.15f);
            }
            _baseHintLabel.Text = "Активная система: " + BaseName(_currentBase) + "  ";

            // Enable/disable hex letters and out-of-range digits.
            foreach (var btn in _hexLetterButtons)
                btn.Enabled = _currentBase == 16;
            foreach (var btn in _digitButtons)
            {
                string d = (string)btn.Tag;
                int v = int.Parse(d);
                btn.Enabled = v < _currentBase;
                btn.ForeColor = btn.Enabled ? TextColor : Color.FromArgb(85, 88, 110);
            }
        }

        private void OnDigit(string digit)
        {
            if (!NumberConverter.IsValidDigit(digit[0], _currentBase))
            {
                SetStatus($"Цифра '{digit}' недопустима для {BaseName(_currentBase)}.", true);
                return;
            }
            string cur = _mainDisplay.Text;
            if (_justComputed)
            {
                cur = "0";
                _justComputed = false;
            }
            if (cur == "0" || cur == "")
                SetMainDisplay(digit);
            else if (cur == "-0")
                SetMainDisplay("-" + digit);
            else
                SetMainDisplay(cur + digit);
        }

        private void OnBackspace()
        {
            string cur = _mainDisplay.Text;
            if (cur.Length <= 1 || (cur.Length == 2 && cur.StartsWith("-")))
                SetMainDisplay("0");
            else
                SetMainDisplay(cur.Substring(0, cur.Length - 1));
        }

        private void OnClear()
        {
            _engine.ClearAll();
            _justComputed = false;
            SetMainDisplay("0");
            SetStatus("Сброс.", false);
        }

        private void OnToggleSign()
        {
            string cur = _mainDisplay.Text;
            if (string.IsNullOrEmpty(cur) || cur == "0") return;
            SetMainDisplay(cur.StartsWith("-") ? cur.Substring(1) : "-" + cur);
        }

        private void OnOperation(Operation op)
        {
            try
            {
                long val = TryParseCurrent(out bool ok);
                if (!ok) return;
                _engine.PushOperation(val, op);
                _justComputed = true;
                SetStatus($"Операция: {CalculatorEngine.OperationSymbol(op)}", false);
            }
            catch (Exception ex)
            {
                SetStatus("Ошибка: " + ex.Message, true);
            }
        }

        private void OnEquals()
        {
            try
            {
                long val = TryParseCurrent(out bool ok);
                if (!ok) return;
                long result = _engine.Equals(val, _currentBase);
                SetMainDisplay(NumberConverter.FromDecimal(result, _currentBase));
                _justComputed = true;
                RefreshHistory();
                SetStatus("Готово.", false);
            }
            catch (DivideByZeroException ex)
            {
                SetStatus("Ошибка: " + ex.Message, true);
            }
            catch (OverflowException)
            {
                SetStatus("Ошибка: переполнение результата.", true);
            }
            catch (Exception ex)
            {
                SetStatus("Ошибка: " + ex.Message, true);
            }
        }

        private void OnNot()
        {
            try
            {
                long val = TryParseCurrent(out bool ok);
                if (!ok) return;
                long result = _engine.ApplyNot(val, _currentBase);
                SetMainDisplay(NumberConverter.FromDecimal(result, _currentBase));
                _justComputed = true;
                RefreshHistory();
            }
            catch (Exception ex)
            {
                SetStatus("Ошибка: " + ex.Message, true);
            }
        }

        // ---- Main display & conversion synchronisation ----
        private void SetMainDisplay(string text)
        {
            _suppressFieldEvents = true;
            _mainDisplay.Text = text;
            _mainDisplay.SelectionStart = text.Length;
            _suppressFieldEvents = false;

            // Recompute conversion fields.
            try
            {
                long dec = NumberConverter.ToDecimal(text, _currentBase);
                UpdateAllFieldsFromDecimal(dec);
            }
            catch
            {
                _binField.Text = _octField.Text = _decField.Text = _hexField.Text = "—";
            }
        }

        private void UpdateAllFieldsFromDecimal(long dec)
        {
            _suppressFieldEvents = true;
            try
            {
                _binField.Text = NumberConverter.FromDecimal(dec, 2);
                _octField.Text = NumberConverter.FromDecimal(dec, 8);
                _decField.Text = NumberConverter.FromDecimal(dec, 10);
                _hexField.Text = NumberConverter.FromDecimal(dec, 16);
            }
            finally
            {
                _suppressFieldEvents = false;
            }
        }

        private void OnFieldEdited(TextBox tb, int baseValue)
        {
            if (_suppressFieldEvents) return;
            if (!tb.Focused) return;
            try
            {
                long dec = NumberConverter.ToDecimal(tb.Text, baseValue);
                _suppressFieldEvents = true;
                _mainDisplay.Text = NumberConverter.FromDecimal(dec, _currentBase);
                _suppressFieldEvents = false;
                // Update *other* fields, but not the one being edited.
                _suppressFieldEvents = true;
                if (baseValue != 2)  _binField.Text = NumberConverter.FromDecimal(dec, 2);
                if (baseValue != 8)  _octField.Text = NumberConverter.FromDecimal(dec, 8);
                if (baseValue != 10) _decField.Text = NumberConverter.FromDecimal(dec, 10);
                if (baseValue != 16) _hexField.Text = NumberConverter.FromDecimal(dec, 16);
                _suppressFieldEvents = false;
                SetStatus($"Ввод в {BaseName(baseValue)}.", false);
            }
            catch (Exception ex)
            {
                SetStatus("Ошибка ввода: " + ex.Message, true);
            }
        }

        private void MainDisplay_TextChanged(object sender, EventArgs e)
        {
            if (_suppressFieldEvents) return;
            try
            {
                long dec = NumberConverter.ToDecimal(_mainDisplay.Text, _currentBase);
                UpdateAllFieldsFromDecimal(dec);
            }
            catch
            {
                // ignore partial input
            }
        }

        private void MainDisplay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Enter) return;
            if (e.KeyChar == '-')
            {
                e.Handled = true;
                OnToggleSign();
                return;
            }
            if (!NumberConverter.IsValidDigit(e.KeyChar, _currentBase))
                e.Handled = true;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Avoid double-handling when focus is in a TextBox that handles keys itself.
            if (this.ActiveControl is TextBox) return;

            char c = char.ToUpperInvariant(e.KeyChar);
            if (NumberConverter.IsValidDigit(c, _currentBase))
            {
                OnDigit(c.ToString());
                e.Handled = true;
                return;
            }
            switch (c)
            {
                case '+': OnOperation(Operation.Add); e.Handled = true; break;
                case '-': OnOperation(Operation.Subtract); e.Handled = true; break;
                case '*': OnOperation(Operation.Multiply); e.Handled = true; break;
                case '/': OnOperation(Operation.Divide); e.Handled = true; break;
                case '=': OnEquals(); e.Handled = true; break;
                case '&': OnOperation(Operation.And); e.Handled = true; break;
                case '|': OnOperation(Operation.Or); e.Handled = true; break;
                case '^': OnOperation(Operation.Xor); e.Handled = true; break;
                case '~': OnNot(); e.Handled = true; break;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnEquals();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Back && !(this.ActiveControl is TextBox))
            {
                OnBackspace();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                OnClear();
                e.Handled = true;
            }
        }

        // ---- Utilities ----
        private long TryParseCurrent(out bool ok)
        {
            try
            {
                long v = NumberConverter.ToDecimal(_mainDisplay.Text, _currentBase);
                ok = true;
                return v;
            }
            catch (Exception ex)
            {
                SetStatus("Ошибка: " + ex.Message, true);
                ok = false;
                return 0;
            }
        }

        private void RefreshHistory()
        {
            _historyList.BeginUpdate();
            _historyList.Items.Clear();
            foreach (var entry in _engine.History)
                _historyList.Items.Add(entry);
            _historyList.EndUpdate();
        }

        private void SetStatus(string text, bool isError)
        {
            _statusLabel.Text = "  " + text;
            _statusLabel.ForeColor = isError ? ErrorColor : OkColor;
        }

        private static string BaseName(int b)
        {
            switch (b)
            {
                case 2: return "BIN (двоичная)";
                case 8: return "OCT (восьмеричная)";
                case 10: return "DEC (десятичная)";
                case 16: return "HEX (шестнадцатеричная)";
                default: return b.ToString();
            }
        }
    }
}
