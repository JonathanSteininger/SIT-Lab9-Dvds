using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lab9Dvds
{
    public class DVD
    {
        private int _id;
        private string _name;
        private Image _image;

        public int ID { get { return _id; } }
        public string Name { get { return _name;} set { _name = value; } }
        public Image Image { get { return _image; } set { _image = value; } }

        public DVD(int id, string name, Image image)
        {
            _id = id;
            _name = name;
            _image = image;
        }

        public DVD(string name, Image image) : this(-1, name, image) { }

        
    }
}
