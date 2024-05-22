using CustomMessageBoxNamespace;
using System;
using System.Configuration;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.cs;

namespace WindowsFormsApp1
{
    public partial class frmMain : Form
    {

        private Point downPoint;

        private string previousValue = "";
        private string currentValue = "0";
        private string operatorValue = "";
        private double result = 0;
        private bool isOperation = false;
        private bool finishFlag = false;
        private const int maxLengthBeforeScaling = 11; // 定义开始缩小字体的字符数量
        private const float minFontSize = 8; // 最小字体大小
        private const float baseFontSize = 32; // 初始字体大小
        private const int maxLengthAllowed = 16; // 最大允许输入字符数
        private double memoryValue = 0; // 内存中的值

        private ContextMenuStrip contextMenuStrip;
        private bool isFormVisible = false;
        private bool miniSizeTipMenuItemChecked;
        Color displayColor = ColorTranslator.FromHtml("#202020");
        Color frmCloseBackColor = ColorTranslator.FromHtml("#C42B1C");
        Color mouseHoverColor = ColorTranslator.FromHtml("#333739");
        Color mouseLeaveColor = ColorTranslator.FromHtml("#202020");
        Color buttonNumberBackColor = ColorTranslator.FromHtml("#383D3E");
        Color buttonOperatorBackColor = ColorTranslator.FromHtml("#323232");
        Color buttonEqualsBackColor = ColorTranslator.FromHtml("#DA8ABD");
        // 创建 HideCursorRichTextBox 控件实例
        HideCursorRichTextBox richTextDIsplay = new HideCursorRichTextBox();
        HideCursorRichTextBox richTextDIsplayTop = new HideCursorRichTextBox();

        public frmMain()
        {
            LoadAppConfig();
            InitializeComponent();
            InitializeNotifyIcon();
            InitializeButtonStyles();
            this.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this, true, null);
            richTextBoxInit(richTextDIsplay);
            richTextBoxInit(richTextDIsplayTop);
            this.Controls.Add(richTextDIsplay);
            this.Controls.Add(richTextDIsplayTop);

            // 初始化窗体事件处理
            this.VisibleChanged += frmMain_VisibleChanged;
            this.SizeChanged += frmMain_SizeChanged;
        }

        private void frmMain_VisibleChanged(object sender, EventArgs e)
        {
            // 更新窗体可见状态
            isFormVisible = this.Visible && this.WindowState != FormWindowState.Minimized;
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            // 更新窗体可见状态
            isFormVisible = this.Visible && this.WindowState != FormWindowState.Minimized;
        }

        private void InitializeNotifyIcon()
        {
            // 设置托盘图标的文本
            notifyIconMain.Text = "计算器";

            // 初始化ContextMenuStrip
            contextMenuStrip = new ContextMenuStrip();

            // 自定义菜单项外观
            ToolStripMenuItem openMenuItem = new ToolStripMenuItem("打开计算器");
            openMenuItem.Font = new Font("Segoe UI", 10);
            openMenuItem.BackColor = Color.FromArgb(0xFF, 0x29, 0x2A, 0x2D); // 设置背景色为指定的 ARGB 颜色值
            openMenuItem.ForeColor = Color.White;
            openMenuItem.Click += OpenMenuItem_Click;

            ToolStripMenuItem miniSizeTipMenuItem = new ToolStripMenuItem("最小化提示");
            miniSizeTipMenuItem.Font = new Font("Segoe UI", 10);
            miniSizeTipMenuItem.BackColor = Color.FromArgb(0xFF, 0x29, 0x2A, 0x2D); // 设置背景色为指定的 ARGB 颜色值
            miniSizeTipMenuItem.ForeColor = Color.White;
            miniSizeTipMenuItem.Checked = miniSizeTipMenuItemChecked;
            miniSizeTipMenuItem.Click += miniSizeTipMenuItem_click;


            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("退出");
            exitMenuItem.Font = new Font("Segoe UI", 10);
            exitMenuItem.BackColor = Color.FromArgb(0xFF, 0x29, 0x2A, 0x2D); // 设置背景色为指定的 ARGB 颜色值
            exitMenuItem.ForeColor = Color.White;
            exitMenuItem.Click += ExitMenuItem_Click;

            contextMenuStrip.Items.Add(openMenuItem);
            contextMenuStrip.Items.Add(miniSizeTipMenuItem);
            contextMenuStrip.Items.Add(exitMenuItem);

            // 将ContextMenuStrip关联到NotifyIcon
            notifyIconMain.ContextMenuStrip = contextMenuStrip;

            // 设置ContextMenuStrip的属性以使其更美观
            contextMenuStrip.Renderer = new ToolStripProfessionalRenderer(new DarkColorTable());
            contextMenuStrip.Font = new Font("Segoe UI", 10);
            contextMenuStrip.BackColor = Color.White;
            contextMenuStrip.ForeColor = Color.Black;
        }

        private void miniSizeTipMenuItem_click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                menuItem.Checked = !menuItem.Checked; // 切换选中状态
                miniSizeTipMenuItemChecked = menuItem.Checked;
            }
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {

            if (!isFormVisible)
            {
                // 显示窗体并带有动画效果
                Animation.AnimateWindow(this.Handle, 200, Animation.AW_SLIDE | Animation.AW_VER_NEGATIVE | Animation.AW_ACTIVATE);
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate(); // 确保窗体被激活
            }
            else
            {
                // 隐藏窗体并带有动画效果
                Animation.AnimateWindow(this.Handle, 200, Animation.AW_SLIDE | Animation.AW_VER_NEGATIVE | Animation.AW_ACTIVATE | 0x00010000);
                this.Hide();
            }
            // 更新窗体可见状态
            isFormVisible = !isFormVisible;

        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            // 处理退出菜单项的点击事件
            closeApplication();
        }


        // 在初始化代码中设置按钮的样式
        private void InitializeButtonStyles()
        {
            AllButtonDefaultStyle();
            NoneBolderButtonStyle(btnMiniSize);
            NoneBolderButtonStyle(btnExit, frmCloseBackColor, Color.Transparent);
            NoneBolderButtonStyle(btnMP);
            NoneBolderButtonStyle(btnMD);
            NoneBolderButtonStyle(btnMC);
            NoneBolderButtonStyle(btnMV);
            NoneBolderButtonStyle(btnMS);
            NoneBolderButtonStyle(btnMR);
        }

        private void AllButtonDefaultStyle()
        {
            foreach (Control ctrl in Controls)
            {
                if (ctrl is Button)
                {
                    Button button = (Button)ctrl;
                    button.Cursor = Cursors.Hand; // 设置鼠标指针样式
                }
            }
        }

        private void NoneBolderButtonStyle(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0; // 设置边框大小为0，即不显示边框
            button.BackColor = Color.Transparent; // 根据需要设置背景色
            button.MouseEnter += (sender, e) =>
            {
                button.BackColor = mouseHoverColor; // 鼠标经过时保持透明背景
            };
            button.MouseLeave += (sender, e) =>
            {
                button.BackColor = Color.Transparent; // 鼠标离开时保持透明背景
            };
        }

        private void NoneBolderButtonStyle(Button button, Color enterColor, Color LeaveColor)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.Transparent; // 根据需要设置背景色
            button.MouseEnter += (sender, e) =>
            {
                button.BackColor = enterColor; // 鼠标经过时保持透明背景
            };
            button.MouseLeave += (sender, e) =>
            {
                button.BackColor = LeaveColor; // 鼠标离开时保持透明背景
            };
        }

        private void richTextBoxInit(HideCursorRichTextBox richTextBox)
        {
            // 设置控件的位置和大小

            richTextBox.BackColor = displayColor;
            richTextBox.ForeColor = Color.White;
            richTextBox.BorderStyle = BorderStyle.None;
            richTextBox.Font = new Font(this.Font.FontFamily, 20, FontStyle.Bold);
            richTextBox.ScrollBars = RichTextBoxScrollBars.None;
            richTextBox.Multiline = false;
            richTextBox.RightToLeft = RightToLeft.Yes;

            richTextBox.ReadOnly = true;
            richTextBoxSetPoint();
            // 绑定 TextChanged 事件
            richTextBox.TextChanged += new EventHandler(richTextBox_TextChanged);
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            // 临时移除事件处理程序以防止递归调用
            richTextDIsplay.TextChanged -= richTextBox_TextChanged;

            // 处理文本格式化
            FormatText();

            // 恢复事件处理程序
            richTextDIsplay.TextChanged += richTextBox_TextChanged;
        }

        private void FormatText()
        {
            string text = richTextDIsplay.Text.Replace(",", ""); // 移除现有的逗号
            if (text.Length > maxLengthAllowed)
            {
                // 如果文本长度（不包括逗号）超过 maxLengthAllowed，截取前 maxLengthAllowed 个字符
                text = text.Substring(0, maxLengthAllowed);
                richTextDIsplay.Text = text;
                richTextDIsplay.SelectionStart = text.Length;
            }

            if (decimal.TryParse(text, out decimal number))
            {
                // 如果解析成功，检查是否为整数或小数
                if (number == Math.Truncate(number))
                {
                    richTextDIsplay.Text = string.Format("{0:n0}", number); // 格式化整数，每隔三位添加逗号
                }
                else
                {
                    richTextDIsplay.Text = number.ToString(); // 不是整数，直接显示小数
                }
                richTextDIsplay.SelectionStart = richTextDIsplay.Text.Length; // 将光标移动到文本末尾
            }
            else
            {
                // 解析失败，显示错误信息或进行其他处理

                richTextDIsplay.SelectionStart = richTextDIsplay.Text.Length;
            }


            // 如果文本长度没有超过 maxLengthAllowed，则调整字体大小
            if (text.Length <= maxLengthAllowed)
            {
                AdjustFontSize(richTextDIsplay); // 调整字体大小
            }
        }

        private void AdjustFontSize(RichTextBox richTextBox)
        {
            int textLength = richTextBox.Text.Length;
            float newFontSize;

            if (textLength > maxLengthBeforeScaling)
            {
                // 根据文本长度计算字体大小的降低速率
                float reductionRate = (textLength - maxLengthBeforeScaling) * 1.9f; // 根据需要调整这个因子

                newFontSize = baseFontSize - reductionRate;
                if (newFontSize < minFontSize)
                {
                    newFontSize = minFontSize;
                }
            }
            else
            {
                newFontSize = baseFontSize;
            }

            richTextBox.Font = new Font(richTextBox.Font.FontFamily, newFontSize, FontStyle.Bold);
        }




        private void richTextBoxSetPoint()
        {
            richTextDIsplayTop.Location = new Point(1, 89);
            richTextDIsplayTop.Size = new Size(280, 39);
            richTextDIsplayTop.Font = new Font(this.Font.FontFamily, 12);
            richTextDIsplayTop.ForeColor = ColorTranslator.FromHtml("#8A8A8A");
            richTextDIsplay.Location = new Point(1, 128);
            richTextDIsplay.Size = new Size(280, 39);
            richTextDIsplay.Font = new Font(this.Font.FontFamily, 32, FontStyle.Bold);
        }

        private void paneTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                downPoint = new Point(e.X, e.Y);
            }
        }

        private void paneTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - downPoint.X,
                    this.Location.Y + e.Y - downPoint.Y);
            }
        }

        private void btn0_Click(object sender, EventArgs e)
        {
            btnNum_Click(sender, e);
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            btnNum_Click(sender, e);
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            btnNum_Click(sender, e);
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            btnNum_Click(sender, e);
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            btnNum_Click(sender, e);
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            btnNum_Click(sender, e);
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            btnNum_Click(sender, e);
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            btnOperator_Click(sender, e);
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            richTextDIsplay.Text = "";
            richTextDIsplayTop.Text = "";
            previousValue = "";
            currentValue = "";
        }

        private void btnMiniSize_Click(object sender, EventArgs e)
        {
            Animation.AnimateWindow(this.Handle, 500, Animation.AW_SLIDE | Animation.AW_VER_POSITIVE | Animation.AW_ACTIVATE | 0x00010000);
            // 取消最小化操作，隐藏窗体到托盘
            this.Hide();
            notifyIconMain.Visible = true;
            // 显示托盘消息
            if (miniSizeTipMenuItemChecked)
            {
                notifyIconMain.ShowBalloonTip(2000, "提示", "计算器已最小化到托盘", ToolTipIcon.Info);
            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            closeApplication();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Animation.AnimateWindow(this.Handle, 200, Animation.AW_CENTER);//窗体加载动画                                                           
            richTextDIsplay.Text = currentValue;
            // 确保 paneTitle 是最上层控件
            this.Controls.SetChildIndex(this.paneTitle, 0);
            // 初始化窗体可见状态
            isFormVisible = this.Visible && this.WindowState != FormWindowState.Minimized;
        }

        private void LoadAppConfig()
        {
            // 读取配置
            miniSizeTipMenuItemChecked = Convert.ToBoolean(ConfigurationManager.AppSettings["MiniSizeTipMenuItemChecked"]);
        }

        private void btnPoint_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentValue) && !currentValue.Contains("."))
            {
                currentValue += ".";
                richTextDIsplay.Text = currentValue;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            // 检查 RichTextBox 中是否有文本
            if (richTextDIsplay.TextLength > 0)
            {
                richTextDIsplay.ReadOnly = false;
                // 将光标移动到文本末尾
                richTextDIsplay.SelectionStart = richTextDIsplay.TextLength - 1;
                // 将选定文本的长度设置为 1，即最后一个字符
                richTextDIsplay.SelectionLength = 1;
                // 删除选定文本（即最后一个字符）
                richTextDIsplay.SelectedText = "";
                richTextDIsplay.ReadOnly = true;
                currentValue = richTextDIsplay.Text;
            }
        }

        private void closeApplication()
        {
            SaveAppConfig();
            Animation.AnimateWindow(this.Handle, 500, Animation.AW_SLIDE | Animation.AW_HOR_POSITIVE | Animation.AW_ACTIVATE | 0x00010000);
            Application.Exit();
        }


        private void SaveAppConfig()
        {
            // 修改配置
            UpdateAppSettings("MiniSizeTipMenuItemChecked", miniSizeTipMenuItemChecked.ToString());
        }

        private void UpdateAppSettings(string key, string value)
        {
            // 打开配置文件
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            // 修改设置
            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }

            // 保存并刷新配置文件
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                closeApplication();
            }
            string keyValue = "";
            if (e.Shift && e.KeyCode == Keys.D8)
            {
                keyValue = "Mul";
            }
            else if (e.Shift && e.KeyCode == Keys.OemQuestion)
            {
                keyValue = "Divi";
            }
            else if (e.Shift && e.KeyCode == Keys.Oemplus)
            {
                keyValue = "Plus";
            }
            else if (e.Shift && e.KeyCode == Keys.OemMinus)
            {
                keyValue = "Sub";
            }
            else if (e.Shift && e.KeyCode == Keys.D5)
            {
                keyValue = "Mod";
            }
            // 判断按下的键是否为数字键或者小数点
            else if (IsNumericKey(e.KeyCode) || e.KeyCode == Keys.Decimal)
            {
                if (e.KeyCode == Keys.Decimal)
                {
                    keyValue = "Point";
                }
                else
                {
                    keyValue = GetNumericValue(e.KeyCode) + "";
                }
            }
            // 判断按下的键是否为加减乘除操作符
            else if (e.KeyCode == Keys.Add)
            {
                keyValue = "Plus";
            }
            else if (e.KeyCode == Keys.Subtract)
            {
                keyValue = "Sub";
            }
            else if (e.KeyCode == Keys.Multiply)
            {
                keyValue = "Mul";
            }
            else if (e.KeyCode == Keys.Divide)
            {
                keyValue = "Divi";
            }
            // 判断按下的键是否为退格键
            else if (e.KeyCode == Keys.Back)
            {
                keyValue = "Remove";
            }
            // 判断按下的键是否为删除键
            else if (e.KeyCode == Keys.Delete)
            {
                keyValue = "Clear";
            }
            PerformClick(keyValue);
            e.Handled = true; // 标记事件已经处理
        }


        private async void PerformClick(string keyValue)
        {
            // 在窗体中查找对应的数字按钮，并触发其点击事件
            Control[] controls = this.Controls.Find("btn" + keyValue, true);
            if (controls.Length > 0 && controls[0] is Button)
            {
                Button button = (Button)controls[0];
                button.PerformClick();

                // 添加按钮点击的动画效果
                button.BackColor = mouseHoverColor;

                // 添加按下时的动画效果，如缩放或移动等
                button.Size = new Size(button.Width - 2, button.Height - 2); // 示例中简单地减小按钮大小

                // 等待一段时间后恢复按钮状态
                await Task.Delay(100);

                // 恢复按钮状态，模拟释放时的动画效果
                button.Size = new Size(button.Width + 2, button.Height + 2); // 示例中简单地增加按钮大小
                Color backColor;
                if (keyValue.Equals("Equals"))
                {
                    backColor = buttonEqualsBackColor;
                }
                else if (IsButtonNumber(keyValue))
                {
                    backColor = buttonNumberBackColor;
                }
                else
                {
                    backColor = buttonOperatorBackColor;
                }
                button.BackColor = backColor;
            }
        }

        private bool IsButtonNumber(string keyValue)
        {
            if (keyValue.Equals("0") || keyValue.Equals("1") || keyValue.Equals("2") || keyValue.Equals("3") || keyValue.Equals("4") ||
                keyValue.Equals("5") || keyValue.Equals("6") || keyValue.Equals("7") || keyValue.Equals("8") || keyValue.Equals("9") ||
                keyValue.Equals("Point") || keyValue.Equals("PN"))
            {
                return true;
            }
            return false;
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                // 判断按下的键是否为回车键
                string keyValue = "Equals";
                PerformClick(keyValue);
                return true;
            }
            return false;
        }


        private bool IsNumericKey(Keys keyCode)
        {
            return (keyCode >= Keys.D0 && keyCode <= Keys.D9) || keyCode == Keys.NumPad0 || keyCode == Keys.NumPad1 ||
                   keyCode == Keys.NumPad2 || keyCode == Keys.NumPad3 || keyCode == Keys.NumPad4 || keyCode == Keys.NumPad5 ||
                   keyCode == Keys.NumPad6 || keyCode == Keys.NumPad7 || keyCode == Keys.NumPad8 || keyCode == Keys.NumPad9;
        }

        private int GetNumericValue(Keys keyCode)
        {
            if (keyCode >= Keys.D0 && keyCode <= Keys.D9)
            {
                return keyCode - Keys.D0;
            }
            else if (keyCode >= Keys.NumPad0 && keyCode <= Keys.NumPad9)
            {
                return keyCode - Keys.NumPad0;
            }
            return -1; // 如果不是数字键，则返回负数或者其他标志值
        }

        private void btnSub_Click(object sender, EventArgs e)
        {
            btnOperator_Click(sender, e);
        }

        private void btnMul_Click(object sender, EventArgs e)
        {
            btnOperator_Click(sender, e);
        }

        private void btnDivi_Click(object sender, EventArgs e)
        {
            btnOperator_Click(sender, e);
        }

        private void btnMod_Click(object sender, EventArgs e)
        {
            btnOperator_Click(sender, e);
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            btnNum_Click(sender, e);
        }

        private void btn8_Click_1(object sender, EventArgs e)
        {
            btnNum_Click(sender, e);
        }

        private void btn9_Click_1(object sender, EventArgs e)
        {
            btnNum_Click(sender, e);
        }

        private void btnEquals_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentValue))
            {
                if (!string.IsNullOrEmpty(previousValue))
                {
                    richTextDIsplayTop.Text = $"{previousValue} {operatorValue} {currentValue}" + " ="; // 更新顶部显示
                    Calcualte(); // 如果之前已经有一个操作数，则计算之前的结果
                    richTextDIsplay.Text = previousValue;
                    currentValue = previousValue;
                }
                else
                {
                    richTextDIsplayTop.Text = currentValue + "=";
                }
                finishFlag = true;
            }
        }

        private void btnNum_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender; // 获取触发事件的按钮
            string buttonText = button.Text; // 获取按钮上的文本（数字）

            if (finishFlag)
            {
                ResetCalculator(buttonText);
            }
            else
            {
                ProcessInput(buttonText);
            }

            richTextDIsplay.Text = currentValue; // 更新显示
        }

        private void ResetCalculator(string buttonText)
        {
            previousValue = "";
            currentValue = buttonText; // 开始新的一次输入
            finishFlag = false;
            isOperation = false;
        }

        private void ProcessInput(string buttonText)
        {
            if (isOperation)
            {
                currentValue = buttonText; // 如果之前按了操作符，则开始新的一次输入
                isOperation = false;
            }
            else
            {
                if (currentValue == "0")
                {
                    currentValue = buttonText; // 如果当前值是0，则替换它
                }
                else
                {
                    currentValue += buttonText; // 否则在当前值后追加新的数字
                }
            }
        }
        private void btnOperator_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender; // 获取触发事件的按钮
            string buttonText = button.Text; // 获取按钮上的文本（操作符）

            finishFlag = false;
            if (!string.IsNullOrEmpty(currentValue))
            {
                if (!string.IsNullOrEmpty(previousValue))
                {
                    Calcualte(); // 如果之前已经有一个操作数，则计算之前的结果
                    richTextDIsplayTop.Text = $"{previousValue} {buttonText}"; // 更新顶部显示
                }
                else
                {
                    // 如果没有之前的操作数，则将当前值作为之前的操作数
                    previousValue = currentValue;
                    richTextDIsplayTop.Text = $"{previousValue} {buttonText}"; // 更新顶部显示
                }
                currentValue = "";
                isOperation = true; // 标记当前正在进行操作符输入
            }
            else
            {
                if (!string.IsNullOrEmpty(previousValue))
                {
                    richTextDIsplayTop.Text = $"{previousValue} {buttonText}"; // 更新顶部显示
                }
            }
            operatorValue = buttonText; // 更新操作符

        }
        private void Calcualte()
        {
            isOperation = false;

            if (string.IsNullOrEmpty(previousValue) || string.IsNullOrEmpty(currentValue))
            {
                return;
            }

            double num1 = double.Parse(previousValue);
            double num2 = double.Parse(currentValue);

            switch (operatorValue)
            {
                case "+":
                    result = num1 + num2;
                    break;
                case "-":
                    result = num1 - num2;
                    break;
                case "×":
                    result = num1 * num2;
                    break;
                case "÷":
                    if (num2 != 0)
                        result = num1 / num2;
                    else
                    {
                        MessageBox.Show("除数不能为0！");
                        return;
                    }
                    break;
                case "%":
                    if (num2 != 0)
                        result = num1 % num2;
                    else
                    {
                        MessageBox.Show("取余数不能为0！");
                        return;
                    }
                    break;
            }
            previousValue = result.ToString("0.###############");
            currentValue = "";
            operatorValue = "";
        }
        private void pictureBoxHistory_MouseHover(object sender, EventArgs e)
        {
            pictureBoxHistory.BackColor = mouseHoverColor;
        }

        private void pictureBoxHistory_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxHistory.BackColor = mouseLeaveColor;
        }

        private void pictureBoxMenu_MouseHover(object sender, EventArgs e)
        {
            pictureBoxMenu.BackColor = mouseHoverColor;
        }

        private void pictureBoxMenu_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxMenu.BackColor = mouseLeaveColor;
        }

        private void btnMiniSize_MouseLeave(object sender, EventArgs e)
        {
            btnMiniSize.BackColor = mouseLeaveColor;
        }

        private void btnMiniSize_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 显示窗体
                this.Show();
                // 添加动画效果
                Animation.AnimateWindow(this.Handle, 200, Animation.AW_SLIDE | Animation.AW_VER_NEGATIVE);
            }
        }

        private void notifyIconMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!isFormVisible)
                {
                    // 显示窗体并带有动画效果
                    Animation.AnimateWindow(this.Handle, 200, Animation.AW_SLIDE | Animation.AW_VER_NEGATIVE | Animation.AW_ACTIVATE);
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                    this.Activate(); // 确保窗体被激活
                }
                else
                {
                    // 隐藏窗体并带有动画效果
                    Animation.AnimateWindow(this.Handle, 200, Animation.AW_SLIDE | Animation.AW_VER_NEGATIVE | Animation.AW_ACTIVATE | 0x00010000);
                    this.Hide();
                }
                // 更新窗体可见状态
                isFormVisible = !isFormVisible;
            }
        }

        private void pictureBoxIcon_MouseDown(object sender, MouseEventArgs e)
        {
            // 将鼠标按下事件转发给 paneTitle 的鼠标按下事件
            paneTitle_MouseDown(sender, e);
        }

        private void pictureBoxIcon_MouseMove(object sender, MouseEventArgs e)
        {
            // 将鼠标移动事件转发给 paneTitle 的鼠标移动事件
            paneTitle_MouseMove(sender, e);
        }

        private void labelTit_MouseDown(object sender, MouseEventArgs e)
        {
            // 将鼠标按下事件转发给 paneTitle 的鼠标按下事件
            paneTitle_MouseDown(sender, e);
        }

        private void labelTit_MouseMove(object sender, MouseEventArgs e)
        {
            // 将鼠标移动事件转发给 paneTitle 的鼠标移动事件
            paneTitle_MouseMove(sender, e);
        }

        private void btnPN_Click(object sender, EventArgs e)
        {
            double number = double.Parse(currentValue) * (-1);
            currentValue = number.ToString();
            richTextDIsplay.Text = currentValue;
        }

        private void btnSqr_Click(object sender, EventArgs e)
        {
            double number = double.Parse(currentValue);
            // 计算数字的平方
            double square = Math.Pow(number, 2);
            currentValue = number.ToString();
            richTextDIsplay.Text = currentValue;
        }

        private void btnSqrt_Click(object sender, EventArgs e)
        {
            // 将字符串转换为 double 类型的数字
            double number = double.Parse(currentValue);

            // 计算数字的平方根
            double squareRoot = Math.Sqrt(number);
            currentValue = number.ToString();
            richTextDIsplay.Text = currentValue;
        }

        private void btnReciprocal_Click(object sender, EventArgs e)
        {
            // 将字符串转换为 double 类型的数字
            double x = double.Parse(currentValue);

            // 计算 x 的分之一
            double number = Math.Pow(x, -1);
            currentValue = number.ToString();
            richTextDIsplay.Text = currentValue;
        }

        private void btnMP_Click(object sender, EventArgs e)
        {
            // 获取当前显示的数字
            double currentValue;
            if (double.TryParse(richTextDIsplay.Text, out currentValue))
            {
                // 将当前数字加到内存中
                memoryValue += currentValue;
            }
        }

        private void btnMD_Click(object sender, EventArgs e)
        {
            // 获取当前显示的数字
            double currentValue;
            if (double.TryParse(richTextDIsplay.Text, out currentValue))
            {
                // 将当前数字从内存中减去
                memoryValue -= currentValue;
            }
        }

        private void btnMR_Click(object sender, EventArgs e)
        {
            // 将内存中的值显示在文本框中
            richTextDIsplay.Text = memoryValue.ToString();
            currentValue = memoryValue.ToString();
        }

        private void btnMC_Click(object sender, EventArgs e)
        {
            // 清除内存中的值
            memoryValue = 0;
        }

        private void btnMS_Click(object sender, EventArgs e)
        {
            // 获取当前显示的数字
            double currentValue;
            if (double.TryParse(richTextDIsplay.Text, out currentValue))
            {
                // 将当前数字存储到内存中
                memoryValue = currentValue;
            }
        }

        private void btnMV_Click(object sender, EventArgs e)
        {
            TBDTip();
        }

        private void TBDTip()
        {
            CustomMessageBox.Show("待开发", this);
        }

        private void pictureBoxHistory_Click(object sender, EventArgs e)
        {
            TBDTip();
        }

        private void pictureBoxMenu_Click(object sender, EventArgs e)
        {
            TBDTip();
        }
    }
}
