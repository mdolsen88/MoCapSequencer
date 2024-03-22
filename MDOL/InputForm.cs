using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoCapSequencer.MDOL
{
    public static class InputForm
    {
        public static int Choice(string Title, params string[] Candidates)
        {
            int space = 10;
            int itemHeight = 25;
            int itemWidth = 200;

            Form frm = new Form()
            {
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                Text = Title,
                Width = 16 + (space + itemWidth + space),
            };
            int y = space;
            int iClicked = -1;
            for (int i = 0; i < Candidates.Length; i++)
            {
                Button button = new Button()
                {
                    Text = Candidates[i],
                    Tag = i,
                    Bounds = new Rectangle(space, y, itemWidth, itemHeight),
                    TextAlign = ContentAlignment.MiddleLeft,
                };
                y += itemHeight + space;
                button.Click += (s, e) =>
                {
                    iClicked = (int)button.Tag;
                    frm.Close();
                };
                frm.Controls.Add(button);
            }
            frm.Height = y + 39;
            frm.ShowDialog();
            return iClicked;
        }
        public static bool Input(string Title, params KeyValuePair<string, Control>[] LabelAndControl)
        {
            int space = 10;
            int itemHeight = 13;
            int itemWidth = 200;

            Form frm = new Form()
            {
                StartPosition = FormStartPosition.CenterParent,
                Text = Title,
                Width = 16 + (space + itemWidth + space),
            };
            int y = space;
            for (int i = 0; i < LabelAndControl.Length; i++)
            {
                if (LabelAndControl[i].Value != null)
                {
                    frm.Controls.Add(new Label()
                    {
                        Text = LabelAndControl[i].Key,
                        Bounds = new Rectangle(space, y, itemWidth, itemHeight),
                        TextAlign = ContentAlignment.MiddleLeft,
                    });
                    y += itemHeight + space;
                    LabelAndControl[i].Value.Bounds = new Rectangle(space, y, itemWidth, itemHeight); ; y += itemHeight + space;
                    frm.Controls.Add(LabelAndControl[i].Value);
                }
                else
                    y += 2 * (itemHeight + space) + space; // 2 Controls (Label+Control) + extra space to next item
            }

            Button ok = new Button()
            {
                Bounds = new Rectangle(space + itemWidth / 2, y, itemWidth / 2, itemHeight * 2),
                Text = "OK",
            };
            y += itemHeight * 2 + space;
            frm.Height = y + 39;
            frm.Controls.Add(ok);
            bool OkClicked = false;
            ok.Click += (s, e) =>
            {
                OkClicked = true;
                frm.Close();
            };
            frm.ShowDialog();
            return OkClicked;
        }
        public static string InputText(string defMessage = "", bool Multiline = false)
        {
            Form frm = new Form()
            {
                StartPosition = FormStartPosition.CenterParent
            };
            frm.Size = new Size(300, Multiline ? 300 : 120);
            frm.FormBorderStyle = Multiline ? FormBorderStyle.Sizable : FormBorderStyle.FixedToolWindow;
            TextBox txtMessage = new TextBox()
            {
                Size = new Size(260, Multiline ? 202 : 20),
                Location = new Point(12, 12),
                Multiline = Multiline,
                Text = defMessage,
            };
            Button cmdOK = new Button()
            {
                Size = new Size(58, 29),
                Location = new Point(214, Multiline ? 220 : 38),
                Tag = false,
                Text = "OK"
            };
            cmdOK.Click += (s, e) =>
            {
                cmdOK.Tag = true;
                frm.Close();
            };
            frm.Resize += (s, e) =>
            {
                txtMessage.Width = frm.Width - 40;
                txtMessage.Height = frm.Height - 92;
                cmdOK.Location = new Point(frm.Width - 86, frm.Height - 80);
            };
            frm.Controls.Add(txtMessage);
            frm.Controls.Add(cmdOK);
            if (!Multiline)
                txtMessage.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        cmdOK.Tag = true;
                        frm.Close();
                    }
                };
            frm.ShowDialog();
            bool click = (bool)cmdOK.Tag;
            return click ? txtMessage.Text : null;
        }
    }
}
