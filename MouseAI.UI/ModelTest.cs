using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseAI.ML;

namespace MouseAI.UI
{
    public partial class ModelTest : Form
    {
        private const int COLUMNS = 10;
        private const int ROWS = 10;
        private const int MARGIN = 5;
        private const int SCALE = 2;
        private const int CONTROLS_HEIGHT = 200;

        public ModelTest(int width, int height)
        {
            InitializeComponent();

            width *= SCALE;
            height *= SCALE;

            pnlImages.Width = COLUMNS * (width + (MARGIN * SCALE));
            pnlImages.Height = ROWS * (height + (MARGIN * SCALE));
            Width = pnlImages.Width + (MARGIN * SCALE);
            Height = CONTROLS_HEIGHT + pnlImages.Height + (MARGIN * SCALE);

            int index = 0;
            for (int x = 0; x < COLUMNS + 1; x++)
            {
                for (int y = 0; y < ROWS + 1; y++)
                {
                    PictureBox pbx = new PictureBox
                    {
                        Width = width,
                        Height = height,
                        Location = new Point(x * width + (MARGIN * SCALE), y * height + (MARGIN * SCALE)),
                        Visible = false,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        BorderStyle = BorderStyle.FixedSingle,
                    };
                    pnlImages.Controls.Add(pbx);
                    pbx.Name = (index++).ToString();
                    pbx.MouseHover += pbx_MouseHover;
                }
            }
        }

        private void pbx_MouseHover(object sender, EventArgs e)
        {
            PictureBox pbx = (PictureBox)sender;

            if (pbx == null || !pbx.Visible)
                return;

            txtInfo.Text = pbx.Text;
            // Console.WriteLine("Pbx {0} Clicked", pbx.Name);
        }

        public void SetImages(ImageDatas ids)
        {
            MemoryStream ms;
            int index = 0;
            PictureBox pbx;
            foreach (Control c in pnlImages.Controls)
            {
                if (c is PictureBox)
                {
                    pbx = (PictureBox)c;
                    if (index >= ids.Count)
                    {
                        ((PictureBox)c).Visible = false;
                    }
                    else
                    {
                        ms = new MemoryStream(ids[index].Data);
                        pbx.Image = (Bitmap)Image.FromStream(ms);
                        pbx.Visible = true;
                        pbx.SizeMode = PictureBoxSizeMode.StretchImage;
                        pbx.Text = string.Format("{0} [{1}] ",ids[index].Index, ids[index].Label);
                        index++;
                    }
                }
            }
        }
    }
}
