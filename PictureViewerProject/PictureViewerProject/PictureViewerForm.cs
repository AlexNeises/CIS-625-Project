using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using edu.ksu.cis.masaaki;
using System.Threading.Tasks;

namespace PictureViewer
{
    public partial class PictureViewerForm : Form
    {
        public PictureViewerForm()
        {
            InitializeComponent();
        }

        private string mFolderSelected;
        private string mFileSelected;

        private void PictureViewerForm_Load(object sender, EventArgs e)
        {
            this.FoldersTreeView.Nodes.Clear();

            // Root node of the TreeView is the My Pictures folder on the file system
            TreeNode RootNode = new TreeNode("My Pictures");
            RootNode.Tag = Environment.GetFolderPath
                (Environment.SpecialFolder.MyPictures);

            this.FoldersTreeView.Nodes.Add(RootNode);

            // Fill up then expand the TreeView control
            FindDirectories(RootNode);
            FoldersTreeView.Nodes[0].Expand();
        }

        private void FindDirectories(TreeNode DirNode)
        {

            System.IO.DirectoryInfo Dir =
                new System.IO.DirectoryInfo(DirNode.Tag.ToString());

            System.IO.DirectoryInfo[] DirItems;
            DirItems = Dir.GetDirectories();

            foreach (System.IO.DirectoryInfo DirItem in DirItems)
            {
                TreeNode NewNode = new TreeNode(DirItem.Name);
                DirNode.Nodes.Add(NewNode);
                NewNode.Tag = DirItem.FullName;

                FindDirectories(NewNode);
            }
        }

        private void FindFiles()
        {

            System.IO.DirectoryInfo Dir =
                new System.IO.DirectoryInfo(mFolderSelected);
            System.IO.FileInfo[] Pictures =
                Dir.GetFiles("*.jpg");

            FilesListView.Items.Clear();
            ThumbnailsImageList.Images.Clear();

            this.toolStripProgressBar1.Maximum
                = Pictures.Length;
            this.toolStripProgressBar1.Value = 0;
            Bitmap Thumbnail;
            foreach (System.IO.FileInfo File in Pictures)
            {
                ListViewItem NewListViewItem =
                    new ListViewItem(File.Name);
                NewListViewItem.Tag = File.FullName;
                Thumbnail = new Bitmap(Image.FromFile(File.FullName), new Size(128, 96));
                ThumbnailsImageList.Images.Add(Thumbnail);
                NewListViewItem.ImageIndex =
                    this.toolStripProgressBar1.Value;

                this.FilesListView.Items.Add(NewListViewItem);
                this.toolStripProgressBar1.Value += 1;
            }
            this.toolStripProgressBar1.Value = 0;

        }

        static Bitmap emptyBitmap = null;
        private static Bitmap EmptyBitmap // 
        {
            get
            {
                //This bitmap will act as a temporary place holder
                //Only create it once
                if (emptyBitmap == null)
                {
                    emptyBitmap = new Bitmap(128, 96, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    for (int i = 0; i <= emptyBitmap.Width - 1; i++)
                    {
                        for (int j = 0; j <= emptyBitmap.Height - 1; j++)
                        {
                            emptyBitmap.SetPixel(i, j, Color.Gray);
                        }
                    }
                }
                return emptyBitmap;
            }
        }

        private void FoldersTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            mFolderSelected = (string)e.Node.Tag;
            FindFiles();
        }

        private void FilesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            mFileSelected = this.FilesListView.FocusedItem.Tag.ToString();
            this.PicturePanel.Invalidate();
        }

        private void PicturePanel_Paint(object sender, PaintEventArgs e)
        {
            if (mFileSelected != null)
            {
                DrawPicture(mFileSelected, this.PicturePanel.Height,
                    this.PicturePanel.Width, e.Graphics);
            }
        }

        private void DrawPicture(string pictureFile, double maxHeight, double maxWidth, System.Drawing.Graphics graphics)
        {

            Bitmap picture = new Bitmap(pictureFile);

            // Scale the image so if it is bigger than the
            // Panel its size is maximized but not distorted
            double scale;
            double picHeight;
            double picWidth;
            picHeight = picture.Height;
            picWidth = picture.Width;
            if (picHeight > maxHeight)
            {
                scale = maxHeight / picture.Height;
                picHeight = maxHeight;
                picWidth = picWidth * scale;
            }
            if (picWidth > maxWidth)
            {
                scale = maxWidth / picWidth;
                picWidth = maxWidth;
                picHeight = picHeight * scale;
            }

            // After the image is scaled center it in the panel
            int Top;
            int Left;
            Top = (int)((maxHeight - picHeight) / 2.0);
            Left = (int)((maxWidth - picWidth) / 2.0);

            // Fill the panel with a white background
            graphics.FillRectangle(Brushes.Ivory, new Rectangle(0, 0, this.PicturePanel.Width, this.PicturePanel.Height));

            // Put the image centered in the panel
            graphics.DrawImage(picture, new Rectangle((Left), (Top), (int)picWidth, (int)picHeight));
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void miCancel_Click(object sender, EventArgs e)
        {
            // the following code is to explain how to use ListDialog
            // These lines should be replaced
            UnitedNations un = new UnitedNations();
            List<Nation> nationsList = un.MakeList();

            ListDialog ld = new ListDialog();
            object[] displayItems = nationsList.ToArray();
            ld.AddDisplayItems(displayItems);
            ld.AddDisplayItems(new Nation("Canada", 120000000));  // because of "param", you can keep adding individual objects
            ld.AddDisplayItems(new Nation("Australia", 23398147), new Nation("Austria", 8414638));
            ld.ShowDialog();
            ld.ClearDisplayItems();   // you can clear the listDialog and start over
            ld.AddDisplayItems("You can clear the listDialog and start over");
            ld.AddDisplayItems("Sure, you can display a dstring", "this way");
            ld.AddDisplayItems(displayItems);
            ld.ShowDialog();
        }
    }
}