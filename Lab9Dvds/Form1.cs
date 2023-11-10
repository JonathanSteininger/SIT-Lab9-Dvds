using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab9Dvds
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private List<DVD> _dvds;
        private DB _db;
        private void Form1_Load(object sender, EventArgs e)
        {
            Setup();
        }

        PictureBox pb;

        
        private void Setup()
        {
            _db = new DB(@"Data Source=./Lab9.db;Version=3;");
            _dvds = _db.LoadAllDVDs();
            Width = Screen.PrimaryScreen.Bounds.Width/5*4;
            Height = Screen.PrimaryScreen.Bounds.Height/5*4;

            MakeControlsContainer();
            FillList();
            listContainer.Controls.Add(list);
            listContainer.Controls.Add(CreateButton("New DVD", 30, Height/2 + 60, NewDVDButtonClicked));
            listContainer.Controls.Add(CreateButton("Refresh", 200 + 30, Height/2 + 60, (sender, e) => { RefreshData();}));
            listContainer.Controls.Add(CreateButton("Delete", 400 + 30, Height/2 + 60, DeleteSelected));
        }

        private void DeleteSelected(object sender, EventArgs e)
        {
            ListView lv = list;
            foreach(ListViewItem item in lv.CheckedItems) {
                _db.RemoveDVD(item.Tag as DVD);
            }
            RefreshData();
        }

        private void RefreshData()
        {
            _dvds = _db.LoadAllDVDs();
            refill();
            list.Refresh();
        }

        private void MakeControlsContainer()
        {
            listContainer = new ContainerControl();
            listContainer.Dock = DockStyle.Fill;
            Controls.Add(listContainer);
        }
        private Button CreateButton(string text, int x, int y, EventHandler OnClickMethod)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(x,y);
            btn.Size = new Size(100, 30);
            btn.Click += OnClickMethod;
            return btn;
        }

        private async void NewDVDButtonClicked(object sender, EventArgs e)
        {
            using (AddDVDForm dvdForm = new AddDVDForm())
            {
                if(dvdForm.ShowDialog() == DialogResult.OK)
                {
                    DVD temp = dvdForm.CreatedDVD;
                    _db.AddDVD(temp);
                    RefreshData();
                    await Task.Delay(500);
                    RefreshData();
                }
            }
        }

        private ListView list;

        private ContainerControl listContainer;

        private void FillList()
        {
            ImageList imageList = new ImageList(); //need to fill this with every image

            ListView listView = new ListView();// need to fill this with every dvds information

            listView.Bounds = new Rectangle(30, 30, Width/3, Height/2);
            listView.View = View.Details;
            listView.DoubleClick += ListView_DoubleClick;
            listView.CheckBoxes = true;
            foreach(DVD dvd in _dvds)
            {
                ListViewItem lv = new ListViewItem(dvd.Name, 0);
                lv.Tag = dvd;
                if(dvd.Image != null) imageList.Images.Add(dvd.Name,dvd.Image);
                lv.ImageKey = dvd.Name;
                lv.SubItems.Add(dvd.ID.ToString());
                listView.Items.Add(lv);
            }

            listView.SmallImageList = imageList;
            listView.LargeImageList = imageList;
            listView.Columns.Add("Movie", 150,HorizontalAlignment.Left);
            listView.Columns.Add("id", -2,HorizontalAlignment.Left);
            list = listView;
        }

        private async void ListView_DoubleClick(object sender, EventArgs e)
        {
            ListView temp = sender as ListView;
            DVD dvd = temp.SelectedItems[0].Tag as DVD;
            using(AddDVDForm dvdForm = new AddDVDForm(true, dvd))
            {
                if(dvdForm.ShowDialog() == DialogResult.OK)
                {
                    _db.UpdatdeDVD(dvd);
                    RefreshData();
                    await Task.Delay(500);
                    RefreshData();
                }
            }
        }

        private void refill()
        {
            ImageList imageList = new ImageList(); //need to fill this with every image

            ListView listView = list;// need to fill this with every dvds information
            listView.Items.Clear();
            listView.Bounds = new Rectangle(30, 30, Width / 3, Height / 2);
            listView.View = View.Details;

            foreach (DVD dvd in _dvds)
            {
                ListViewItem lv = new ListViewItem(dvd.Name, 0);
                lv.Tag = dvd;
                if (dvd.Image != null) imageList.Images.Add(dvd.Name, dvd.Image);
                lv.ImageKey = dvd.Name;

                lv.SubItems.Add(dvd.ID.ToString());
                listView.Items.Add(lv);
            }

            listView.SmallImageList = imageList;
            listView.LargeImageList = imageList;
            list = listView;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            pb.Refresh();
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            PictureBox box = sender as PictureBox;
            g.DrawImage(box.Image, 0, 0,box.Width,box.Height);
        }

        
    }
}
