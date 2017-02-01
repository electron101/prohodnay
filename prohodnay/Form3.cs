using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace prohodnay
{
    public partial class Form3 : Form
    {
        public static string USER;
        //static public SqlConnection cn;
        DataSet ds3 = new DataSet();
        BindingSource bs_lg_get_name = new BindingSource();


        // для перемещиния формы ----------
        private Point mouseOffset;
        private bool isMouseDown = false;
        // --------------------------------

        public Form3()
        {
            InitializeComponent();
        }
        
        public struct utmp
        {
            public int          tmp_uid;            // id пользователя
            public string       tmp_name;           // имя пользователя
            public int          tmp_role;           // роль в системе {0 - root; 1 - админ; 2 - пользак}
            //public string       tmp_time_boot;      // время входа в систему
            //public string       tmp_time_exit;      // последний вход в систему


            /* возвращает заполненную структуру utmp при входе пользователя в систему */
            static public utmp fill_on_boot (int id_user_in)
            {
                utmp utmp_boot = new utmp();

                string SQL = " SELECT name, role FROM polzov WHERE id_polzov = @ID ";
                using (SqlCommand cm = new SqlCommand(SQL, Form1.cn))
                {
                    cm.Parameters.Add("@ID", SqlDbType.Int).Value = id_user_in;
                    try
                    {
                        using (SqlDataReader rd = cm.ExecuteReader())
                        {
                            if (rd.HasRows)
                            {
                                rd.Read();
                                utmp_boot.tmp_uid = id_user_in;
                                utmp_boot.tmp_name = rd["name"].ToString();
                                utmp_boot.tmp_role = Convert.ToInt32(rd["role"]);
                            }
                            rd.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                return utmp_boot;
            }
        }

        public static utmp utmp_USER;

        void login_get_name()
        {
            ds3.Tables["LG_GET_NAME"].Clear();
            string strSQL = " SELECT id_polzov, name FROM polzov ";
            Form1.SQLAdapter = new SqlDataAdapter(strSQL, Form1.cn);
            Form1.SQLAdapter.Fill(ds3, "LG_GET_NAME");
            bs_lg_get_name.DataSource = ds3.Tables["LG_GET_NAME"];            
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Program.center_form(Program.F3);

            SqlConnectionStringBuilder bdr = new SqlConnectionStringBuilder();
            bdr.DataSource = @".\SQLExpress";
            bdr.InitialCatalog = "prohodnay";
            bdr.IntegratedSecurity = true;

            Form1.cn = new SqlConnection(bdr.ConnectionString);
            try
            {
                Form1.cn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            ds3.Tables.Add("LG_GET_NAME");

            // справочник прользователей
            login_get_name();
            listBox1.DisplayMember = "name";
            listBox1.ValueMember = "id_polzov";
            listBox1.DataSource = bs_lg_get_name;  
        }
                
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Text == "")
                MessageBox.Show("Пользователь не выбран", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if (Form1.login_pw(Convert.ToInt32(listBox1.SelectedValue), textBox_vx_pas.Text))
                {
                    try
                    {
                        textBox_vx_pas.Text = "";     // очистка пароля
                        // заполняем структуру с информацие о пользователе
                        utmp_USER = utmp.fill_on_boot(Convert.ToInt32(listBox1.SelectedValue));

                        Program.F1 = new Form1();   // создаёт экземпляр формы АРМ
                        Program.F3.Hide();          // скрывает форму входа
                        Program.F1.Show();          // отображает форму АРМ
                        Program.F3.Close();         // закрывает форму входа
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    textBox_vx_pas.Text = "";     // очистка пароля
                    MessageBox.Show("Неверный пароль", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        
        private void textBox_vx_pas_KeyDown(object sender, KeyEventArgs e)
        {
            // Вход по нажатию клавиши Enter, без нажатия кнопки вход
            // После ввода пароля нажимаем Enter и не нужно нажимать кнопку вход
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);   // дублирует код для нажатия кнопки вход
            }
        }

        // для перемещения формы мышью ----------------------------------------------
        //
        private void Form3_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }
        private void Form3_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }
        private void Form3_MouseDown(object sender, MouseEventArgs e)
        {
            int xOffset;
            int yOffset;

            if (e.Button == MouseButtons.Left)
            {
                xOffset = -e.X - SystemInformation.FrameBorderSize.Width;
                yOffset = -e.Y - SystemInformation.CaptionHeight -
                    SystemInformation.FrameBorderSize.Height;
                mouseOffset = new Point(xOffset, yOffset);
                isMouseDown = true;
            }
        }
        private void Form3_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }
    }
}
