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
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;

namespace DemoCS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnWinrt_Click(object sender, EventArgs e)
        {
            // await
            var folder = KnownFolders.DocumentsLibrary;
            var opts = new QueryOptions(CommonFileQuery.OrderByName, new[] { ".txt" });
            var files = await folder.CreateFileQueryWithOptions(opts).GetFilesAsync(0, 10);
            foreach (var file in files) textBox1.Text += $"FILE {Path.GetFileName(file.Path)}\r\n";

            // streams
            using (var reader = new StreamReader(await files.First().OpenStreamForReadAsync()))
            {
                var txt = await reader.ReadToEndAsync();
                textBox1.Text += $"STREAM {txt.Substring(0, 100)}\r\n";
            }

            // xaml
            CornerRadius cr = new CornerRadius(1.5);
            textBox1.Text += $"XAML {cr.BottomLeft} - {cr.BottomRight}\r\n";

            // pictures
            var pics = await KnownFolders.PicturesLibrary.GetFilesAsync(CommonFileQuery.OrderBySearchRank, 0, 1);
            using (var stream = await pics.First().OpenReadAsync())
            {
                var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream);
                textBox1.Text += $"PICTURE {decoder.OrientedPixelWidth} x {decoder.OrientedPixelHeight}\r\n";
            }
        }

        private void btnAppx_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
