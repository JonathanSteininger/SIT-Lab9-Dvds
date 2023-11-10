using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Lab9Dvds
{
    public partial class AddDVDForm : Form
    {
        public AddDVDForm(bool editing, DVD dvd)
        {
            Editing = editing;
            UpdateDVD = dvd;
            InitializeComponent();
        }
        public AddDVDForm() : this(false, null) { }

        private int offset = 30;
        private TextBox nameBox;
        public DVD UpdateDVD;
        public bool Editing = false;
        private PictureBox pictureBox1;
        public DVD CreatedDVD { get; set; }
        private void AddDVDForm_Load(object sender, EventArgs e)
        {
            Setup();
        }

        private void Setup()
        {
            Width = 600;
            Height = 400;
            Controls.Add(CreateLable("MovieName", 0, 0));
            nameBox = CreateField(!Editing ? "Enter Movie Name" : UpdateDVD.Name, 0, 30, true);
            Controls.Add(nameBox);
            Controls.Add(CreateButton("Browse Image", 0,60, SelectImageButtonClicked));
            pictureBox1 = new PictureBox();
            if (Editing) pictureBox1.Image = UpdateDVD.Image;
            pictureBox1.Location = new Point(offset + 100, 60 + offset);
            pictureBox1.Size = new Size(100, 100);
            pictureBox1.Paint += PictureBox1_Paint;
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(pictureBox1);
            pictureBox1.Refresh();
            Controls.Add(CreateButton(!Editing?"Create":"Save", 0, 200, CreateDVDButtonCLicked));
            Controls.Add(CreateButton("Cancel", 130, 200, CancelDVDButtonCLicked));
        }

        private void CancelDVDButtonCLicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CreateDVDButtonCLicked(object sender, EventArgs e)
        {
            if(Editing)
            {
                if(UpdateDVD == null) { DialogResult = DialogResult.Abort; Close(); }
                UpdateDVD.Name = nameBox.Text;
                UpdateDVD.Image = pictureBox1.Image;
                pictureBox1.Refresh();
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
            if ((bool)nameBox.Tag)
            {
                CreatedDVD = new DVD(nameBox.Text, pictureBox1.Image);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                nameBox.BackColor = Color.Red;
                nameBox.Refresh();
            }
        }
        
        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            PictureBox box = sender as PictureBox;
            if (box.Image == null) return;
            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(new Point(), box.Size);
            g.DrawImage(box.Image, rect);
        }

        
        private Button CreateButton(string text, int x, int y, EventHandler OnClickMethod)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(offset + x, offset + y);
            btn.Size = new Size(100, offset);
            btn.Click += OnClickMethod;
            return btn;
        }

        private void SelectImageButtonClicked(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                string filePath = dialog.FileName;
                try
                {
                    pictureBox1.Image = GetImage(filePath);
                    pictureBox1.Refresh();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        private TextBox CreateField(string defaultText, int x, int y, bool required)
        {
            TextBox box = new TextBox();
            box.Text = defaultText;
            box.Tag = !required;
            box.Location = new Point(x + offset, y + offset);
            box.TextChanged += (sender, e) => { (sender as TextBox).Tag = true; (sender as TextBox).BackColor = Color.White; };
            return box;
        }
        private void CreateField(string defaultText, int x, int y) => CreateField(defaultText, x, y, false);


        private Label CreateLable(string text, int x, int y)
        {
            Label lable = new Label();
            lable.Text = text;
            lable.Location = new Point(x + offset, y + offset);
            lable.Size = new Size(200, 30);
            return lable;
        }
        private Image GetImage(string path)
        {
            if (!File.Exists(path)) return null;
            try
            {
                Image img = Image.FromFile(path);
                return img;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
