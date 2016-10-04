using System;
using System.Drawing;
using System.Windows.Forms;

internal class DevPromtDialog : Form
{
    private RichTextBox _textBox;

    public DevPromtDialog()
    {
        this.Text = "devPrompt for CloudBall";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.AutoScaleBaseSize = new Size(5, 13);
        this.ClientSize = new Size(400, 600);
        this.MaximizeBox = false;
        this._textBox = new RichTextBox();
        this._textBox.Multiline = true;
        this._textBox.SetBounds(0, 0, 400, 600);
        this._textBox.ScrollBars = RichTextBoxScrollBars.Vertical;
        this.Controls.Add((Control)this._textBox);
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.ClientSize = new Size(284, 262);
        this.Name = "devPrompt";
        this.ResumeLayout(false);
    }

    public void ShowPromt()
    {
        this.SetVisibleCore(true);
    }

    public void HidePromt()
    {
        this.Hide();
    }

    public void ClearPromt()
    {
        this._textBox.Clear();
    }

    public bool IsVisible()
    {
        return this.Visible;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason != CloseReason.UserClosing)
            return;
        e.Cancel = true;
        this.HidePromt();
        base.OnFormClosing(e);
    }

    public void AppendText(string text)
    {
        if (this._textBox.TextLength > 0)
            this._textBox.AppendText(Environment.NewLine + text);
        else
            this._textBox.AppendText(text);
        this._textBox.SelectionStart = this._textBox.Text.Length;
        this._textBox.ScrollToCaret();
    }
}
