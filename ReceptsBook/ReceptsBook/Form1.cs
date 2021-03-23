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
using System.Text.Json;
using System.Web.Helpers;

namespace ReceptsBook{
    public partial class Form1 : Form
    {
        private static Book _book;
        private static Recept current_recept;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string File_text = "";
            if (File.Exists("recipes.json"))
            {
                File_text = File.ReadAllText("recipes.json", Encoding.GetEncoding(1251));
            
                dynamic json0 = Json.Decode(File_text);

                _book = new Book();

                foreach (dynamic jsonItem in json0.recipes)
                {
                    current_recept = new Recept(jsonItem.uuid,
                                                jsonItem.name,
                                                jsonItem.images,
                                                jsonItem.lastUpdated,
                                                jsonItem.description,
                                                jsonItem.instructions,
                                                jsonItem.difficulty);
                
                    _book.AddRecept(current_recept);
                }

                for(int i=0;i< _book.ind; i++)
                {
                    listBox1.Items.Add(_book[i].name);
                }

                listBox1.SelectedIndex = 0;

                get_current_recept();
                presentation_recept();
            }
            else
                textBox1.Text = "File recipes.json don't exsist";
        }

        private Bitmap load_image_url(string url)
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = request.GetResponse();
            System.IO.Stream respStream = resp.GetResponseStream();
            Bitmap bmp = new Bitmap(respStream);
            return bmp;
        }
        private void add_text_tb1(string text)
        {
            textBox1.Text += text+ Environment.NewLine;
        }
        private void get_current_recept()
        {
            int ind = listBox1.SelectedIndex;
            current_recept = new Recept(_book[ind].uuid,
                                        _book[ind].name,
                                        _book[ind]._images,
                                        _book[ind].lastUpdated,
                                        _book[ind].description,
                                        _book[ind].instructions,
                                        _book[ind].difficulty);
        }
        private void presentation_recept()
        {
            add_text_tb1("<uuid>");
            add_text_tb1(current_recept.uuid.ToString());

            add_text_tb1("<name>");
            add_text_tb1(current_recept.name.ToString());

            add_text_tb1("<images>");
            add_text_tb1(current_recept._images.ToString());

            add_text_tb1("<lastUpdated>");
            add_text_tb1(current_recept.lastUpdated.ToString());

            add_text_tb1("<description>");
            add_text_tb1(current_recept.description.ToString());

            add_text_tb1("<instructions>");
            add_text_tb1(current_recept.instructions.ToString());

            add_text_tb1("<difficulty>");
            add_text_tb1(current_recept.difficulty.ToString());

            label1.Text = current_recept._images_cou.ToString()+"/" + current_recept._images_len.ToString();
            pictureBox1.Image = load_image_url(current_recept._images[current_recept._images_cou-1]);


        }
        private void b_next_Click(object sender, EventArgs e)
        {
            current_recept._images_cou += 1;
            if (current_recept._images_cou > current_recept._images_len)
                current_recept._images_cou = 1;
            label1.Text = current_recept._images_cou.ToString() + "/" + current_recept._images_len.ToString();
            pictureBox1.Image = load_image_url(current_recept._images[current_recept._images_cou-1]);
        }
        private void b_prev_Click(object sender, EventArgs e)
        {
            current_recept._images_cou -= 1;
            if (current_recept._images_cou < 1)
                current_recept._images_cou = current_recept._images_len;
            label1.Text = current_recept._images_cou.ToString() + "/" + current_recept._images_len.ToString();
            pictureBox1.Image = load_image_url(current_recept._images[current_recept._images_cou-1]);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                textBox1.Text = "";
                get_current_recept();
                presentation_recept();
            }
        }

    }

    public class Recept
    {
        private string _uuid;
        private string _name;
        public dynamic _images;
        public int _images_len;
        public int _images_cou;
        private int _lastUpdated;
        private string _description;
        private string _instructions;
        private int _difficulty;

        public string uuid => _uuid;
        public string name => _name;
        public int lastUpdated => _lastUpdated;
        public string description => _description;
        public string instructions => _instructions;
        public int difficulty => _difficulty;

        public Recept(string uuid = "",
                        string name = "",
                        dynamic images = null,
                        int lastUpdated = 0,
                        string description = "",
                        string instructions = "",
                        int difficulty = 0)
        {
            if (uuid == null)
                uuid = "";
            if (name == null)
                name = "";
            if (images == null)
            {
                images = new string[1];
                images[0] = "";
            } 
            if (lastUpdated == null)
                lastUpdated = 0;
            if (description == null)
                description = "";
            if (instructions == null)
                instructions = "";
            if (difficulty == null)
                difficulty = 0;

             
            int ind = 1;
            foreach(var item in images)
            {
                ind += 1;
            }

            _uuid = uuid;
            _name = name;
            _images = images;
            _images_len = ind-1;
            _images_cou = 1;
            _lastUpdated = lastUpdated;
            _description = description;
            _instructions = instructions;
            _difficulty = difficulty;
        }


    }

    public class Book
    {

        private List<Recept> _Book;
        private int _ind;
        public int ind => _ind;

        public Recept this[int index] => _Book[index];

        public Recept this[string name] => _Book.FirstOrDefault(g => g.name.Equals(name));

        public Book()
        {
            _Book = new List<Recept>();
            _ind = 0;
        }

        public void AddRecept(Recept Recept)
        {
            _Book.Add(Recept);
            _ind += 1;
        }

        public void SortByName()
        {
            _Book = _Book.OrderBy(g => g.name).ToList();
        }

    }
}
