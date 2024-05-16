using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class HideCursorRichTextBox : RichTextBox
{
    private const int GWL_STYLE = -16;
    private const int ES_NOHIDESEL = 0x0010; // 防止文本选择时隐藏光标
    private const int ES_READONLY = 0x0800; // 只读模式

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x20;
            return cp;
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x7 || m.Msg == 0x201 || m.Msg == 0x202 || m.Msg == 0x203 || m.Msg == 0x204 || m.Msg == 0x205 || m.Msg == 0x206 || m.Msg == 0x0100 || m.Msg == 0x0101)
        {
            return;
        }
        base.WndProc(ref m);
    }
}
