// 自定义窗体类，继承自 Form 类
using System.Drawing;
using System.Windows.Forms;
using System;

public class CustomForm : Form
{
    // 重写 CreateParams 方法来自定义窗体的样式
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.Style |= 0x20000; // WS_CAPTION
            cp.ClassStyle |= 0x20000; // CS_DROPSHADOW
            return cp;
        }
    }

    // 重写 WndProc 方法来处理窗体消息
    protected override void WndProc(ref Message m)
    {
        const int WM_NCPAINT = 0x85;

        base.WndProc(ref m);

        if (m.Msg == WM_NCPAINT)
        {
            IntPtr hdc = GetWindowDC(m.HWnd);
            if (hdc != IntPtr.Zero)
            {
                Graphics g = Graphics.FromHdc(hdc);
                Rectangle rect = new Rectangle(0, 0, this.Width, SystemInformation.CaptionHeight);
                g.FillRectangle(new SolidBrush(Color.FromArgb(0xFF, 0x29, 0x2A, 0x2D)), rect);
                g.Dispose();
                ReleaseDC(m.HWnd, hdc);
            }
        }
    }

    // 调用 Windows API 函数来获取窗体设备上下文
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr GetWindowDC(IntPtr hWnd);

    // 调用 Windows API 函数来释放窗体设备上下文
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
}