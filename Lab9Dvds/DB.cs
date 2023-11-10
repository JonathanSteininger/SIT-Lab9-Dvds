using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Lab9Dvds
{
    internal class DB
    {

        SQLiteConnection conn;
        private string _connectionString;
        public DB(string ConnectionString)
        {
            _connectionString = ConnectionString;
            try
            {
                conn = new SQLiteConnection(ConnectionString);
            }catch (Exception ex)
            {
                throw new Exception("Error Connecting to server, check connection string or server.", ex);
            }
        }

        public void Reconnect()
        {
            try
            {
                if(conn != null) conn.Close();
                conn = new SQLiteConnection(_connectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DVD> LoadAllDVDs()
        {
            try
            {
                conn.Close();
                conn.Open();
                List<DVD> list = new List<DVD>();
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM DVDs", conn))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string base64Image = reader.GetString(2);
                        Image image = (name != "NULL") ? ImageDecode(base64Image): null;
                        list.Add(new DVD(id, name, image));
                    }
                }
                conn.Close();
                return list;
            }catch(Exception ex)
            {
                conn.Close();
                throw ex;
            }
        }

        private Image ImageDecode(string base64Image)
        {
            if(base64Image == "NULL") return null;
            byte[] bytes = Convert.FromBase64String(base64Image);
            MemoryStream ms = new MemoryStream(bytes);//needs to stay open in order to save images later
            return Image.FromStream(ms);
        }

        private string ImageEncode(Image image)
        {
            if (image == null) return "NULL";
            MemoryStream ms = new MemoryStream();
            image.Save(ms, image.RawFormat);
            byte[] bytes = ms.ToArray();
            return Convert.ToBase64String(bytes);
        }

        public void AddDVD(DVD dvd)
        {
            try
            {
                conn.Open();
                string EncodedImage = ImageEncode(dvd.Image);
                string command = $"Insert Into DVDs(id, name, image) Values (NULL,'{dvd.Name}','{EncodedImage}');";
                using (SQLiteCommand cmd = new SQLiteCommand(command, conn)) {
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                return;
            }
            catch (Exception ex)
            {
                conn.Close();
                throw ex;
            }
        }
        public void UpdatdeDVD(DVD dvd)
        {
            try
            {
                conn.Open();
                string EncodedImage = ImageEncode(dvd.Image);
                string command = $"Update DVDs Set name = '{dvd.Name}', image = '{EncodedImage}' Where 'DVDs'.'id'={dvd.ID};";
                using (SQLiteCommand cmd = new SQLiteCommand(command, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
                return;
            }
            catch (Exception ex)
            {
                conn.Close();
                throw ex;
            }
        }
        public void RemoveDVD(int id)
        {
            try
            {
                conn.Open();
                string command = $"Delete from DVDs Where id = {id};";
                using (SQLiteCommand cmd = new SQLiteCommand(command, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                return;
            }
            catch (Exception ex)
            {
                conn.Close();
                throw ex;
            }
        }
        public void RemoveDVD(DVD dvd) => RemoveDVD(dvd.ID);
        
    }
}
