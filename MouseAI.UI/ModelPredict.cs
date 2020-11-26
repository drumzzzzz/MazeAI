using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MouseAI.ML;

namespace MouseAI.UI
{
    public partial class ModelPredict : Form
    {
        public ModelPredict(Color backgroundcolor)
        {
            InitializeComponent();
            lvwSegments.BackColor = backgroundcolor;
        }

        public void SetImages(ImageDatas ids)
        {
            MemoryStream ms;
            lvwSegments.Items.Clear();
            ImageList imglist = new ImageList
            {
                ImageSize = new Size(100, 100)
            };

            for (int i = 0; i < ids.Count; i++)
            {
                ms = new MemoryStream(ids[i].Data);
                imglist.Images.Add((Bitmap)Image.FromStream(ms));
                lvwSegments.Items.Add(new ListViewItem
                {
                    Text = ids[i].Index.ToString(),
                    ImageIndex = i
                });
            }
            lvwSegments.LargeImageList = imglist;
        }
    }
}
