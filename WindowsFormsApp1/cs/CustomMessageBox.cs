using System;
using System.Drawing;
using System.Windows.Forms;

namespace CustomMessageBoxNamespace
{
    public class CustomMessageBox
    {
        public static void Show(string message, Form ownerForm)
        {
            Form customMessageBoxForm = new Form();
            customMessageBoxForm.FormBorderStyle = FormBorderStyle.None;
            customMessageBoxForm.BackColor = Color.FromArgb(0xFF, 0x29, 0x2A, 0x2D); // 设置弹窗背景颜色
            customMessageBoxForm.StartPosition = FormStartPosition.CenterParent;
            customMessageBoxForm.Size = new Size(300, 150);

            Panel customTitleBar = new Panel();
            customTitleBar.BackColor = Color.FromArgb(0xFF, 0x29, 0x2A, 0x2D); // 自定义标题栏的背景颜色
            customTitleBar.Dock = DockStyle.Top;
            customTitleBar.Height = SystemInformation.CaptionHeight;

            Label titleLabel = new Label();
            titleLabel.Text = "提示";
            titleLabel.ForeColor = Color.White; // 标题文本颜色
            titleLabel.Font = new Font("Arial", 10, FontStyle.Bold);
            titleLabel.Dock = DockStyle.Left;
            titleLabel.Padding = new Padding(10, 0, 0, 0);

            Button closeButton = new Button();
            closeButton.Text = "关闭";
            closeButton.ForeColor = Color.White; // 按钮文本颜色
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.FlatAppearance.BorderSize = 0; // 设置边框大小为0，即不显示边框
            closeButton.Dock = DockStyle.Right;
            closeButton.Click += (sender, e) => customMessageBoxForm.Close();

            customTitleBar.Controls.Add(titleLabel);
            customTitleBar.Controls.Add(closeButton);

            customMessageBoxForm.Controls.Add(customTitleBar);

            TextBox textBoxMessage = new TextBox();
            textBoxMessage.Text = message;
            textBoxMessage.Enabled = false; // 禁用文本框，移除焦点光标
            textBoxMessage.Multiline = true;
            textBoxMessage.BackColor = Color.FromArgb(0xFF, 0x29, 0x2A, 0x2D); // 设置背景颜色
            textBoxMessage.ForeColor = Color.White; // 设置文字颜色为白色
            textBoxMessage.BorderStyle = BorderStyle.None;
            textBoxMessage.TextAlign = HorizontalAlignment.Center; // 水平居中
            textBoxMessage.AutoSize = true; // 自适应文本大小
            textBoxMessage.Font = new Font("Arial", 12, FontStyle.Regular); // 设置字体样式和大小
            textBoxMessage.Location = new Point((customMessageBoxForm.Width - textBoxMessage.Width) / 2, (customMessageBoxForm.Height - textBoxMessage.Height) / 2); // 居中位置

            customMessageBoxForm.Controls.Add(textBoxMessage);
            customMessageBoxForm.ShowDialog(ownerForm);
        }
    }
}
