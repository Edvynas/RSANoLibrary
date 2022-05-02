using System.Numerics;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace RSA_algoritmas
{
    
    public partial class Form1 : Form
    {
        string source = @"Data source=D:\Programavimas\Sharpas\RSA algoritmas\RSA algoritmas\duombaze.db;Version=3;";

        public int p, q;
        public BigInteger N, E, D, F;
        public Form1()
        {
            InitializeComponent();
            panel1.BringToFront();
        }

        public static bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }

        private void button6_Click(object sender, EventArgs e)
        {

                p = Convert.ToInt32(textBox5.Text);
                q = Convert.ToInt32(textBox2.Text);

            if (IsPrime(p) == true && IsPrime(q) == true)
            {

                Keys();

            }
            else
                MessageBox.Show("Vienas arba abu skaiciai nera pirminiai");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox5.Text == "" && textBox2.Text == "")
            {
                decryptnopq();
            }
            else
            {


                textBox3.Text = Decrypt(textBox1.Text);
                textBox4.Text = Encrypt(textBox3.Text);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection(source);

            string query = "insert into uzsifruoti values (NULL,'" + textBox3.Text + "','" + textBox6.Text + "','" + N + "')";
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(query, conn);
            cmd = new SQLiteCommand(query, conn);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Inserted sucessfully");
            conn.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            panel2.BringToFront();

            SQLiteConnection conn = new SQLiteConnection(source);
            conn.Open();
            string query = "select id, encrypted, publickey from uzsifruoti";
            SQLiteCommand cmd = new SQLiteCommand(query, conn);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, "uzsifruoti");
            dataGridView1.DataSource = ds.Tables["uzsifruoti"].DefaultView;

            query = "select id from uzsifruoti";
            cmd = new SQLiteCommand(query, conn);
            SQLiteDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
                comboBox1.Items.Add(Convert.ToString(dr[0]));
            dr.Close();

        }

        private void button7_Click(object sender, EventArgs e)
        {
            string pasirinko = "";
            pasirinko = comboBox1.SelectedItem.ToString();

            SQLiteConnection conn = new SQLiteConnection(source);
            conn.Open();
            string query = "select encrypted, publickey from uzsifruoti where id = '" + pasirinko + "'";
            SQLiteCommand cmd = new SQLiteCommand(query, conn);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                textBox1.Text = Convert.ToString(reader[0]);
                textBox6.Text = Convert.ToString(reader[1]);
            }
            reader.Close();

            panel1.BringToFront();


        }

        private void button8_Click(object sender, EventArgs e)
        {
            panel1.BringToFront();
        }

        public void Keys()
        {

            Random r = new Random();
            List<BigInteger> Possible_E = new();

            N = p * q;
            F = (p - 1) * (q - 1);

            int amount = 0;

            for (E = 2; E < F; E++)
            {
                if (BigInteger.GreatestCommonDivisor(F, E) == 1)
                {
                    amount++;
                    Possible_E.Add(E);
                }
                if (amount == 10)
                {
                    break;
                }
            }

            E = Possible_E[r.Next(0, Possible_E.Count)];

            amount = 0;

            List<BigInteger> Possible_D = new();

            for (D = p; D < F; D++)
            {


                if (E * D % F == 1)
                {
                    Possible_D.Add(D);


                    if (amount == 10)
                    {
                        break;
                    }
                }
            }

            D = Possible_D[r.Next(0, Possible_D.Count)];


            textBox6.Text = Convert.ToString(E);
            textBox7.Text = Convert.ToString(D);

        }

        public string Encrypt(string msg)
        {
            N = Convert.ToInt32(textBox5.Text) * Convert.ToInt32(textBox2.Text);
            E = Convert.ToInt32(textBox6.Text);
            string str = "";
            foreach (char c in msg)
            {
                str += (char)BigInteger.ModPow((int)c, E, N);
            }
            return str;
        }
        public void decryptnopq()
        {
            string n = "";
            SQLiteConnection conn = new SQLiteConnection(source);
            conn.Open();
            string query = "select encrypted, publickey, n from uzsifruoti where id = '" + comboBox1.SelectedItem.ToString() + "'";
            
            
            SQLiteCommand cmd = new SQLiteCommand(query, conn);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                textBox1.Text = Convert.ToString(reader[0]);
                textBox6.Text = Convert.ToString(reader[1]);
                n = Convert.ToString(reader[2]);
                E = Convert.ToInt32(reader[1]);
            }
            reader.Close();
            conn.Close();
            BigInteger a = (int)Convert.ToInt32(n);
            int factor = 0;
            int[] mas = new int[4];
            int i = 0;
            for (factor = 1; factor <= a; factor++)
            {
                if (a % factor == 0)
                {
                    mas[i] = factor;
                    i++;
                }
            }
            textBox5.Text = mas[1].ToString();
            textBox2.Text = mas[2].ToString();

            p = Convert.ToInt32(mas[1]);
            q = Convert.ToInt32(mas[2]);

            int amount = 0;

            Random r = new Random();

            F = (p - 1) * (q - 1);

            List<BigInteger> Possible_D = new();

            for (D = p; D < F; D++)
            {


                if (E * D % F == 1)
                {
                    Possible_D.Add(D);


                    if (amount == 10)
                    {
                        break;
                    }
                }
            }

            D = Possible_D[r.Next(0, Possible_D.Count)];

            textBox7.Text = Convert.ToString(D);
            textBox3.Text = Decrypt(textBox1.Text);
            textBox4.Text = Encrypt(textBox3.Text);




        }
        public string Decrypt(string msg)
        {

            N = Convert.ToInt32(textBox5.Text) * Convert.ToInt32(textBox2.Text);
            D = Convert.ToInt32(textBox7.Text);
            string rez = "";
            foreach (char c in msg)
            {
                rez += (char)BigInteger.ModPow((int)c, D, N);
            }
            return rez;
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox3.Text = Encrypt(textBox1.Text);
            textBox4.Text = Decrypt(textBox3.Text);
        }
    }
}