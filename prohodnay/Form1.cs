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
using System.Security.Cryptography;
//using Microsoft.Office.Interop;

namespace prohodnay
{
    public partial class Form1 : Form
    {
        static public SqlConnection cn;
        static public DataSet ds = new DataSet();
        static public SqlDataAdapter SQLAdapter = new SqlDataAdapter(strSQL, cn);
        static public string strSQL;

        BindingSource bs_polzov = new BindingSource();
        BindingSource bs_os = new BindingSource();
        BindingSource bs_protocol = new BindingSource();
        BindingSource bs_service = new BindingSource();
        BindingSource bs_tip_attack = new BindingSource();
        BindingSource bs_signature = new BindingSource();

        public Form1()
        {
            InitializeComponent();
        }

        static public string hash_md5(string text)
        {
            byte[] bytes_text = Encoding.Unicode.GetBytes(text);    // переводит входную строку в массив байт
            MD5 CSP = new MD5CryptoServiceProvider();               // экземпляр класск md5
            byte[] byteHash = CSP.ComputeHash(bytes_text);          // хешируем массив байт
            string hash = string.Empty;
            foreach (byte b in byteHash)
                hash += string.Format("{0:x2}", b);                 // преобразует HEX, два знака всего будет 32 
            return hash;
        }

        static public bool login_pw(int id_user_in, string pas_in)
        {
            strSQL = "SELECT pas FROM polzov WHERE id_polzov = @ID";
            using (SqlCommand cm = new SqlCommand(strSQL, cn))
            {
                cm.Parameters.Add("@ID", SqlDbType.Int).Value = id_user_in;
                try
                {
                    using (SqlDataReader rd = cm.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            rd.Read();
                            if (string.Compare(rd["pas"].ToString(), hash_md5(pas_in)) == 0)    // если пароли совпали
                                return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            return false;           // если пароли не совпали
        }

        static public void smena_pass_pw(int id_user, string pas_new)
        {
            strSQL = "UPDATE polzov SET pas = @PAS WHERE id_polzov = @ID";
            using (SqlCommand cm1 = new SqlCommand(strSQL, cn))
            {
                cm1.Parameters.Add("@PAS", SqlDbType.VarChar).Value = hash_md5(pas_new);
                cm1.Parameters.Add("@ID", SqlDbType.VarChar).Value = id_user;
                try
                {
                    cm1.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }                        
        }

        public void load_polzov()
        {
            ds.Tables["POLZOV"].Clear();
            strSQL = " SELECT id_polzov AS '№_Пользователя', name AS 'ФИО', role AS 'Роль', " + 
                     " tel AS 'Телефон', adres AS 'Адрес' FROM polzov ";
            SQLAdapter = new SqlDataAdapter(strSQL, cn);

            SQLAdapter.Fill(ds, "POLZOV");

            bs_polzov.DataSource = ds.Tables["POLZOV"];
            dataGridView_polzov.DataSource = bs_polzov;
        }

        void load_protocol()
        {
            ds.Tables["PROTOCOL"].Clear();
            strSQL = " SELECT id_protocol AS '№_Протокола', name AS 'Протокол' FROM protocol ";
            SQLAdapter = new SqlDataAdapter(strSQL, cn);

            SQLAdapter.Fill(ds, "PROTOCOL");

            bs_protocol.DataSource = ds.Tables["PROTOCOL"];
            dataGridView4.DataSource = bs_protocol;
        }

        void load_os()                   // функция для отображения информации
        {
            ds.Tables["OS"].Clear();
            strSQL = " SELECT id_os AS '№_Операционной_системы', name AS 'Операционная_система' FROM os";
            SQLAdapter = new SqlDataAdapter(strSQL, cn);

            SQLAdapter.Fill(ds, "OS");

            bs_os.DataSource = ds.Tables["OS"];
            dataGridView2.DataSource = bs_os;
        }

        void load_signature()                   // функция для отображения информации
        {
            ds.Tables["SIGNATURE"].Clear();
            strSQL = " SELECT signature.id_signature AS '№_Сигнатуры', tip_attack.name AS 'Тип_атаки', " +
                     " protocol.name AS 'Протокол', service.name AS 'Сервис', os.name AS 'Операционная_система' " +
                     " FROM signature JOIN tip_attack ON signature.id_tip_attack = tip_attack.id_tip_attack JOIN protocol ON " + 
                     " signature.id_protocol = protocol.id_protocol JOIN service ON signature.id_service " + 
                     " = service.id_service JOIN os ON signature.id_os = os.id_os ";

            SQLAdapter = new SqlDataAdapter(strSQL, cn);
            
            SQLAdapter.Fill(ds, "SIGNATURE");

            bs_signature.DataSource = ds.Tables["SIGNATURE"];
            dataGridView3.DataSource = bs_signature;
            dataGridView5.DataSource = bs_signature;
        }

        void load_tip_attack()                   // функция для отображения информации
        {
            ds.Tables["TIP_ATTACK"].Clear();
            strSQL = " SELECT id_tip_attack AS '№_Типа_атаки', name AS 'Тип_атаки' FROM tip_attack";
            SQLAdapter = new SqlDataAdapter(strSQL, cn);

            SQLAdapter.Fill(ds, "TIP_ATTACK");

            bs_tip_attack.DataSource = ds.Tables["TIP_ATTACK"];
            dataGridView8.DataSource = bs_tip_attack;
        }
        
        void load_service()                   // функция для отображения информации
        {
            ds.Tables["SERVICE"].Clear();
            strSQL = " SELECT id_service AS '№_Сервиса', name AS 'Сервис' FROM service";
            SQLAdapter = new SqlDataAdapter(strSQL, cn);

            SQLAdapter.Fill(ds, "SERVICE");

            bs_service.DataSource = ds.Tables["SERVICE"];
            dataGridView1.DataSource = bs_service;
        }

        void filter()             // фильтрация сигнатур по всем параметрам
        {
            //&& ds.Tables["SIGNATURE"].Rows.Count > 0
            if (ds.Tables["SIGNATURE"] != null)
            {
                BindingSource bs = new BindingSource();
                bs.DataSource = bs_signature;
                bs.Filter = 
                string.Format(" CONVERT(" + ds.Tables["SIGNATURE"].Columns[1].ToString() + ", 'System.String') LIKE '{0}%' AND " +
                              " CONVERT(" + ds.Tables["SIGNATURE"].Columns[2].ToString() + ", 'System.String') LIKE '{1}%' AND " +
                              " CONVERT(" + ds.Tables["SIGNATURE"].Columns[3].ToString() + ", 'System.String') LIKE '{2}%' AND " +
                              " CONVERT(" + ds.Tables["SIGNATURE"].Columns[4].ToString() + ", 'System.String') LIKE '{3}%'"
                              , comboBox_signature_tip_attack_f.Text, comboBox_signature_protocol_f.Text
                              , comboBox_signature_service_f.Text, comboBox_signature_os_f.Text);
            }
        }

        void spravochnik_reload()
        {
            comboBox_signature_tip_attack.DataSource = bs_tip_attack;
            comboBox_signature_tip_attack.DisplayMember = "Тип_атаки";
            comboBox_signature_tip_attack.ValueMember = "№_Типа_атаки";

            comboBox_signature_protocol.DataSource = bs_protocol;
            comboBox_signature_protocol.DisplayMember = "Протокол";
            comboBox_signature_protocol.ValueMember = "№_Протокола";

            comboBox_signature_service.DataSource = bs_service;
            comboBox_signature_service.DisplayMember = "Сервис";
            comboBox_signature_service.ValueMember = "№_Сервиса";

            comboBox_signature_os.DataSource = bs_os;
            comboBox_signature_os.DisplayMember = "Операционная_система";
            comboBox_signature_os.ValueMember = "№_Операционной_системы";
        }

        void spravochnik_reload_f()
        {
            comboBox_signature_tip_attack_f.DataSource = bs_tip_attack;
            comboBox_signature_tip_attack_f.DisplayMember = "Тип_атаки";
            comboBox_signature_tip_attack_f.ValueMember = "№_Типа_атаки";

            comboBox_signature_protocol_f.DataSource = bs_protocol;
            comboBox_signature_protocol_f.DisplayMember = "Протокол";
            comboBox_signature_protocol_f.ValueMember = "№_Протокола";

            comboBox_signature_service_f.DataSource = bs_service;
            comboBox_signature_service_f.DisplayMember = "Сервис";
            comboBox_signature_service_f.ValueMember = "№_Сервиса";

            comboBox_signature_os_f.DataSource = bs_os;
            comboBox_signature_os_f.DisplayMember = "Операционная_система";
            comboBox_signature_os_f.ValueMember = "№_Операционной_системы";
        }
        
        void controls_start()             // вкладка выдачи выставляем полям нужные свойства
        {
            comboBox_signature_tip_attack_f.Text = "";
            comboBox_signature_protocol_f.Text = "";
            comboBox_signature_service_f.Text = "";
            comboBox_signature_os_f.Text = "";

            //comboBox_signature_tip_attack_f.Enabled = false;
            //comboBox_signature_protocol_f.Enabled = false;
            //comboBox_signature_service_f.Enabled = false;
            //comboBox_signature_os_f.Enabled = false;
        }

        void check_v()                // вкладка выдачи выставляем чекбоксам нужные свойства
        {
            checkBox_tip_attack.Checked = true;
            checkBox_protocol.Checked = true;
            checkBox_service.Checked = true;
            checkBox_os.Checked = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.center_form(Program.F1);

            // Разделение полномочий: если root то загружаются все вкладки
            // если admin то вкладка Настройки не загружается
            // если пользователь то вкладки Настройки и Спаравочники не загружаются
            if (Form3.utmp_USER.tmp_role == 1)          // если админ
                tabPage_nastroi.Parent = null;          // скрыть настройки

            else if ((Form3.utmp_USER.tmp_role != 1) && (Form3.utmp_USER.tmp_role != 0))        // если пользователь
            {
                tabPage_nastroi.Parent = null;          // скрыть настройки
                tabPage_sprav.Parent = null;            // скрыть администратора
            }

            Program.F1.Text += " - [" + Form3.utmp_USER.tmp_name + "]";


            // УСТАНОВКИ ПРИ ПУСКЕ
            //
            //радио кнопка нажата 
            //radioButton3.Checked = true;
            check_v();
            //
            // -------------------------------------------------------------------------------------

            //
            // --- [ ЗАГРУЗКА ] ---   ПОЛЬЗОВАТЕЛИ -------------------------------------------------
            ds.Tables.Add("POLZOV");
            load_polzov();
            dataGridView_polzov.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView_polzov.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            textBox_polzov_fio.DataBindings.Add(new Binding("Text", bs_polzov, "ФИО", false, DataSourceUpdateMode.Never));
            textBox_smena_fio.DataBindings.Add(new Binding("Text", bs_polzov, "ФИО", false, DataSourceUpdateMode.Never));
            textBox_polzov_tel.DataBindings.Add(new Binding("Text", bs_polzov, "Телефон", false, DataSourceUpdateMode.Never));
            textBox_polzov_adres.DataBindings.Add(new Binding("Text", bs_polzov, "Адрес", false, DataSourceUpdateMode.Never));
            // -------------------------------------------------------------------------------------            

            //
            // --- [ ЗАГРУЗКА ] ---   ПРОТОКОЛ -----------------------------------------------------
            ds.Tables.Add("PROTOCOL");
            load_protocol();
            dataGridView4.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            textBox_protocol_name.DataBindings.Add(new Binding("Text", bs_protocol, "Протокол", false, DataSourceUpdateMode.Never));
            // ------------------------------------------------------------------------------------- 
          
            //
            // --- [ ЗАГРУЗКА ] ---   ОПЕРАЦИОННАЯ СИСТЕМА -----------------------------------------
            ds.Tables.Add("OS");
            load_os();
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            textBox_os_name.DataBindings.Add(new Binding("Text", bs_os, "Операционная_система", false, DataSourceUpdateMode.Never));
            // --------------------------------------------------------------------------------------
            
            //
            // --- [ ЗАГРУЗКА ] ---   СИГНАТУРА -----------------------------------------------------
            ds.Tables.Add("SIGNATURE");
            load_signature();
            dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            comboBox_signature_tip_attack.DataBindings.Add(new Binding("Text", bs_signature, "Тип_атаки", false, DataSourceUpdateMode.Never));
            comboBox_signature_protocol.DataBindings.Add(new Binding("Text", bs_signature, "Протокол", false, DataSourceUpdateMode.Never));
            comboBox_signature_service.DataBindings.Add(new Binding("Text", bs_signature, "Сервис", false, DataSourceUpdateMode.Never));
            comboBox_signature_os.DataBindings.Add(new Binding("Text", bs_signature, "Операционная_система", false, DataSourceUpdateMode.Never));
            // --------------------------------------------------------------------------------------

            //
            // --- [ ЗАГРУЗКА ] ---   ФИЛЬТРАЦИЯ ----------------------------------------------------
            dataGridView5.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // --------------------------------------------------------------------------------------

            //
            // --- [ ЗАГРУЗКА ] ---   ТИП АТАКИ -----------------------------------------------------
            ds.Tables.Add("TIP_ATTACK");
            load_tip_attack();
            dataGridView8.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView8.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            textBox_tip_attack_name.DataBindings.Add(new Binding("Text", bs_tip_attack, "Тип_атаки", false, DataSourceUpdateMode.Never));
            // --------------------------------------------------------------------------------------
            
            //
            // --- [ ЗАГРУЗКА ] ---   СЕРВИС --------------------------------------------------------
            ds.Tables.Add("SERVICE");
            load_service();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            textBox_service_name.DataBindings.Add(new Binding("Text", bs_service, "Сервис", false, DataSourceUpdateMode.Never));
            // --------------------------------------------------------------------------------------
        }

        private void button15_Click(object sender, EventArgs e)
        {
            // --- [ ДОБАВЛЕНИЕ ] ---  ПОЛЬЗОВАТЕЛЬ
            
            if (textBox_polzov_fio.Text == "")
                MessageBox.Show("ФИО не может быть пустым",
                    "Добавление пользователя", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if (textBox_polzov_fio.Text == "root")
                    MessageBox.Show("Запрещённое имя!",
                        "Добавление пользователя", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else 
                {
                    if (textBox_polzov_pas1.Text == "" || textBox_polzov_pas2.Text == "")
                        MessageBox.Show("Введите пароль",
                            "Добавление пользователя", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                    {
                        if (textBox_polzov_pas1.Text != textBox_polzov_pas2.Text)
                            MessageBox.Show("Пароли не совпадают", "Добавление пользователя",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                        {
                            int ROL = -1;
                            // если пользователь то роль = 2, если админ то 1
                            if (radioButton1.Checked)
                                ROL = 2;
                            if (radioButton2.Checked)
                                ROL = 1;

                            strSQL = "INSERT INTO polzov VALUES (@PAS, @ROL, @NAME, @TEL, @AD)";

                            SQLAdapter.InsertCommand = new SqlCommand(strSQL, cn);
                            SQLAdapter.InsertCommand.Parameters.Add("@PAS", SqlDbType.VarChar).Value = hash_md5(textBox_polzov_pas1.Text);
                            SQLAdapter.InsertCommand.Parameters.Add("@ROL", SqlDbType.Int).Value = ROL;
                            SQLAdapter.InsertCommand.Parameters.Add("@NAME", SqlDbType.VarChar).Value = textBox_polzov_fio.Text;
                            SQLAdapter.InsertCommand.Parameters.Add("@TEL", SqlDbType.VarChar).Value = textBox_polzov_tel.Text;
                            SQLAdapter.InsertCommand.Parameters.Add("@AD", SqlDbType.VarChar).Value = textBox_polzov_adres.Text;

                            try
                            {
                                SQLAdapter.InsertCommand.ExecuteNonQuery();
                                load_polzov();           // обновим таблицу
                                MessageBox.Show("Пользователь успешно добавлен!", "Добавление пользователя",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            // --- [ УДАЛЕНИЕ ВЫБРАННЫХ ] ---   ПОЛЬЗОВАТЕЛЬ

            if (ds.Tables["POLZOV"].Rows.Count > 0)              // проверка на наличие строк в таблице
            {
                strSQL = " DELETE FROM polzov WHERE id_polzov = @ID_P ";

                SQLAdapter.DeleteCommand = new SqlCommand(strSQL, cn);
                // Если нажата кномка да, удаления не избежать.
                if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                    + dataGridView_polzov.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                {
                    try
                    {
                        foreach (DataGridViewRow drv in dataGridView_polzov.SelectedRows)
                        {
                            // если выбран root то пропустить запись супера нельзя удалить
                            if (ds.Tables["POLZOV"].Rows[drv.Index][1].ToString() == "root")
                            {
                                MessageBox.Show("Суперпользователь не может быть удалён!",
                                    "Удаление пользователя", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                // если выбран для удаления только root то сразу выйти из функции
                                if (dataGridView_polzov.SelectedRows.Count > 1)
                                    continue;
                                else
                                    return;
                            }                                
                            SQLAdapter.DeleteCommand.Parameters.Add("@ID_P", SqlDbType.Int).Value =
                                Convert.ToInt32(ds.Tables["POLZOV"].Rows[drv.Index][0]);

                            SQLAdapter.DeleteCommand.ExecuteNonQuery();
                            SQLAdapter.DeleteCommand.Parameters.Clear();
                        }
                        load_polzov();           // обновим таблицу
                        MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);            
        }

        private void button25_Click(object sender, EventArgs e)
        {
            // --- [ ОБНОВЛЕНИЕ ] ---   ПОЛЬЗОВАТЕЛЬ

            if (ds.Tables["POLZOV"].Rows.Count > 0)              // проверка на наличие строк в таблице
            {
                // еслли выбран root для обновления то сразу выйти, обновлять нельзя
                if (ds.Tables["POLZOV"].Rows[dataGridView_polzov.CurrentRow.Index][1].ToString() == "root")
                {
                    MessageBox.Show("Суперпользователь не может быть обновлён!",
                        "Удаление пользователя", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // если выбран root то сразу выйти из функции
                    return;
                }                                
                if (textBox_polzov_fio.Text == "")
                    MessageBox.Show("ФИО не может быть пустым",
                        "Обновление пользователя", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    if (textBox_polzov_fio.Text == "root")
                        MessageBox.Show("Запрещённое имя!",
                            "Обновление пользователя", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                    {
                        int ROL = -1;
                        // если пользователь то роль = 2, если админ то 1
                        if (radioButton1.Checked)
                            ROL = 2;
                        if (radioButton2.Checked)
                            ROL = 1;
                        // запрос на обновление
                        strSQL = " UPDATE polzov SET name = @NAME, role = @ROL, tel = @TEL, adres = @AD " +
                                 " WHERE id_polzov = @ID_P ";

                        SQLAdapter.UpdateCommand = new SqlCommand(strSQL, cn);  // команда для обноления создана
                        // зададим значения параметрам 
                        SQLAdapter.UpdateCommand.Parameters.Add("@ROL", SqlDbType.Int).Value = ROL;
                        SQLAdapter.UpdateCommand.Parameters.Add("@NAME", SqlDbType.VarChar).Value = textBox_polzov_fio.Text;
                        SQLAdapter.UpdateCommand.Parameters.Add("@TEL", SqlDbType.VarChar).Value = textBox_polzov_tel.Text;
                        SQLAdapter.UpdateCommand.Parameters.Add("@AD", SqlDbType.VarChar).Value = textBox_polzov_adres.Text;

                        SQLAdapter.UpdateCommand.Parameters.Add("@ID_P", SqlDbType.VarChar).Value =
                            Convert.ToInt32(ds.Tables["POLZOV"].Rows[dataGridView_polzov.CurrentRow.Index][0]);
                        try
                        {
                            SQLAdapter.UpdateCommand.ExecuteNonQuery(); // выполним запрос
                            // если удачно то...
                            load_polzov();           // обновим таблицу
                            MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            // если запрос выполнился не удачно то ошибка с инфой
                            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            else
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);               
        }

        private void button17_Click(object sender, EventArgs e)
        {
            // --- [ СМЕНА ПАРОЛЯ ] ---   ПОЛЬЗОВАТЕЛЬ

            if (textBox_smena_fio.Text == "")
                MessageBox.Show("Имя не может быть пустым",
                    "Смена пароля", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if (textBox_smena_oldpas.Text == "")
                    MessageBox.Show("Введите пароль",
                        "Смена пароля", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    if (!login_pw(Convert.ToInt32(ds.Tables["POLZOV"].Rows[dataGridView_polzov.CurrentRow.Index][0]),
                            textBox_smena_oldpas.Text))
                        MessageBox.Show("Пароль не верен",
                            "Смена пароля", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                    {
                        if (textBox_smena_pas1.Text == "" || textBox_smena_pas2.Text == "")
                            MessageBox.Show("Введите новый пароль",
                                "Смена пароля", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                        {
                            if (textBox_smena_pas1.Text != textBox_smena_pas2.Text)
                                MessageBox.Show("Новые пароли не совпадают", "Смена пароля",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            else
                            {
                                smena_pass_pw(Convert.ToInt32(ds.Tables["POLZOV"].Rows[dataGridView_polzov.CurrentRow.Index][0]), 
                                    textBox_smena_pas1.Text);

                                MessageBox.Show("Пароль успешно изменён!",
                                "Смена пароля", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            Application.Exit();     // завершение всего, полный выход из приложения
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Program.F1.Close();     // закрываем АРМ
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.F3 = new Form3();   // созадём новый экземпляр класса формы входа
            Program.F3.Show();          // и возвращаемся к окну входа

            // удаление таблиц по именам при закртытии формы
                       
            ds.Tables.Remove("POLZOV");

            //ds.Tables.Remove("PROTOCOL");
            //ds.Tables.Remove("OS");
            //ds.Tables.Remove("TIP_ATTACK");
            //ds.Tables.Remove("SERVICE");

            //ds.Tables.Remove("SIGNATURE");

            ds.Dispose();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // --- [ ДОБАВЛЕНИЕ ] ---  ПРОТОКОЛ

            // проверим все поля на заполненность
            if (textBox_protocol_name.Text == "")
            {
                MessageBox.Show("Заполните все поля", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // запрос на добавление
            strSQL = "INSERT INTO protocol VALUES (@NAME)";

            // работаем через адаптер и свойство добавления
            SQLAdapter.InsertCommand = new SqlCommand(strSQL, cn);  // новая команда создана
            // определим параметры и зададим им значения
            SQLAdapter.InsertCommand.Parameters.Add("@NAME", SqlDbType.VarChar).Value = textBox_protocol_name.Text;
            try
            {
                SQLAdapter.InsertCommand.ExecuteNonQuery(); // выполним запрос
                // если удачно то...
                load_protocol();           // обновим таблицу
                MessageBox.Show("Успешно добавлен!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // если не удачно обшика с инфой
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            // --- [ ОБНОВЛЕНИЕ ] ---   ПРОТОКОЛ

            if (ds.Tables["PROTOCOL"].Rows.Count > 0)              // проверка на наличие строк в таблице
            {
                // проверим все поля на заполненность
                if (textBox_protocol_name.Text == "")
                {
                    MessageBox.Show("Заполните все поля", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // запрос на обновление
                strSQL = " UPDATE protocol SET name = @NAME WHERE id_protocol = @ID_P ";

                SQLAdapter.UpdateCommand = new SqlCommand(strSQL, cn);  // команда для обноления создана
                // зададим значения параметрам 
                SQLAdapter.UpdateCommand.Parameters.Add("@NAME", SqlDbType.VarChar).Value = textBox_protocol_name.Text;
                SQLAdapter.UpdateCommand.Parameters.Add("@ID_P", SqlDbType.Int).Value =
                    Convert.ToInt32(ds.Tables["PROTOCOL"].Rows[dataGridView4.CurrentRow.Index][0]);
                try
                {
                    SQLAdapter.UpdateCommand.ExecuteNonQuery(); // выполним запрос
                    // если удачно то...
                    load_protocol();           // обновим таблицу
                    MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // если запрос выполнился не удачно то ошибка с инфой
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // --- [ УДАЛЕНИЕ ВЫБРАННЫХ ] ---   ПРОТОКОЛ

            if (ds.Tables["PROTOCOL"].Rows.Count > 0)              // проверка на наличие строк в таблице
            {
                strSQL = " DELETE FROM protocol WHERE id_protocol = @ID_P ";

                SQLAdapter.DeleteCommand = new SqlCommand(strSQL, cn);
                // Если нажата кномка да, удаления не избежать.
                if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                    + dataGridView4.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                {
                    try
                    {
                        foreach (DataGridViewRow drv in dataGridView4.SelectedRows)
                        {
                            SQLAdapter.DeleteCommand.Parameters.Add("@ID_P", SqlDbType.Int).Value =
                                Convert.ToInt32(ds.Tables["PROTOCOL"].Rows[drv.Index][0]);

                            SQLAdapter.DeleteCommand.ExecuteNonQuery();
                            SQLAdapter.DeleteCommand.Parameters.Clear();
                        }
                        load_protocol();           // обновим таблицу
                        MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
            // --- [ ДОБАВЛЕНИЕ ] ---  ОПЕРАЦИОННОЙ СИСТЕМЫ

            // проверим все поля на заполненность
            if (textBox_os_name.Text == "")
            {
                MessageBox.Show("Заполните все поля", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // запрос на добавление
            strSQL = "INSERT INTO os VALUES (@NAME)";

            // работаем через адаптер и свойство добавления
            SQLAdapter.InsertCommand = new SqlCommand(strSQL, cn);  // новая команда создана
            // определим параметры и зададим им значения
            SQLAdapter.InsertCommand.Parameters.Add("@NAME", SqlDbType.VarChar).Value = textBox_os_name.Text;
            try
            {
                SQLAdapter.InsertCommand.ExecuteNonQuery(); // выполним запрос
                // если удачно то...
                load_os();           // обновим таблицу
                MessageBox.Show("Успешно добавлен!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // если не удачно обшика с инфой
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // --- [ ОБНОВЛЕНИЕ ] ---   ОПЕРАЦИОННОЙ СИСТЕМЫ

            if (ds.Tables["OS"].Rows.Count > 0)              // проверка на наличие строк в таблице
            {
                // проверим все поля на заполненность
                if (textBox_os_name.Text == "")
                {
                    MessageBox.Show("Заполните все поля", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // запрос на обновление
                strSQL = " UPDATE os SET name = @NAME WHERE id_os = @ID_OS ";

                SQLAdapter.UpdateCommand = new SqlCommand(strSQL, cn);  // команда для обноления создана
                // зададим значения параметрам 
                SQLAdapter.UpdateCommand.Parameters.Add("@NAME", SqlDbType.VarChar).Value = textBox_os_name.Text;
                SQLAdapter.UpdateCommand.Parameters.Add("@ID_OS", SqlDbType.Int).Value =
                    Convert.ToInt32(ds.Tables["OS"].Rows[dataGridView2.CurrentRow.Index][0]);
                try
                {
                    SQLAdapter.UpdateCommand.ExecuteNonQuery(); // выполним запрос
                    // если удачно то...
                    load_os();           // обновим таблицу
                    MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // если запрос выполнился не удачно то ошибка с инфой
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // --- [ УДАЛЕНИЕ ВЫБРАННЫХ ] ---   ОПЕРАЦИОННОЙ СИСТЕМЫ

            if (ds.Tables["OS"].Rows.Count > 0)              // проверка на наличие строк в таблице
            {
                strSQL = " DELETE FROM os WHERE id_os = @ID_OS ";

                SQLAdapter.DeleteCommand = new SqlCommand(strSQL, cn);
                // Если нажата кномка да, удаления не избежать.
                if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                    + dataGridView2.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                {
                    try
                    {
                        foreach (DataGridViewRow drv in dataGridView2.SelectedRows)
                        {
                            SQLAdapter.DeleteCommand.Parameters.Add("@ID_OS", SqlDbType.Int).Value =
                                Convert.ToInt32(ds.Tables["OS"].Rows[drv.Index][0]);

                            SQLAdapter.DeleteCommand.ExecuteNonQuery();
                            SQLAdapter.DeleteCommand.Parameters.Clear();
                        }
                        load_os();           // обновим таблицу
                        MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
               
        private void button9_Click(object sender, EventArgs e)
        {
            // --- [ ДОБАВЛЕНИЕ ] ---  СИГНАТУРА 
          
            // проверим все поля на заполненность
            if (comboBox_signature_tip_attack.Text == "" || comboBox_signature_protocol.Text == "" ||
                comboBox_signature_service.Text == "" || comboBox_signature_os.Text == "" )
            {
                MessageBox.Show("Заполните все поля",
                    "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Сложить все имена в хеш.
            // Хеш создаётся путём склеивания всех параметров сигнатуры. 
            // В результате такой склейки получается уникальная последовательность
            // по которой и будет происходить проверка при добавлении
            string all_name = comboBox_signature_tip_attack.Text + comboBox_signature_protocol.Text +
                comboBox_signature_service.Text + comboBox_signature_os.Text;

            string hash_all_name = hash_md5(all_name);

            // Сравнить хеш на совпадение в базе
            int id_signature = 0;
            strSQL = "SELECT id_signature AS 'id' FROM signature WHERE hash = @HASH";
            using (SqlCommand cm = new SqlCommand(strSQL, cn))
            {
                cm.Parameters.Add("@HASH", SqlDbType.VarChar).Value = hash_all_name;
                try
                {
                    using (SqlDataReader rd = cm.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            rd.Read();
                            id_signature = Convert.ToInt32(rd["id"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            // Проверить наличие хеша на совпадение
            // если такой хеш был найден то сразу выйти
            if (id_signature != 0)
            {
                MessageBox.Show("Такая сигнатура уже есть в базе данных!", "Добавление", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // если такой сигнатуры ещё нет то добавим её
            if (id_signature == 0)
            { 
                // запрос на добавление
                strSQL = " INSERT INTO signature VALUES (@ID_T, @ID_PROT, @ID_SERV, @ID_OS, @HASH) ";

                // работаем через адаптер и свойство добавления
                SQLAdapter.InsertCommand = new SqlCommand(strSQL, cn);  // новая команда создана
                // определим параметры и зададим им значения
                SQLAdapter.InsertCommand.Parameters.Add("@ID_T", SqlDbType.Int).Value =
                    comboBox_signature_tip_attack.SelectedValue;
                SQLAdapter.InsertCommand.Parameters.Add("@ID_PROT", SqlDbType.Int).Value =
                    comboBox_signature_protocol.SelectedValue;
                SQLAdapter.InsertCommand.Parameters.Add("@ID_SERV", SqlDbType.Int).Value = 
                    comboBox_signature_service.SelectedValue;
                SQLAdapter.InsertCommand.Parameters.Add("@ID_OS", SqlDbType.Int).Value =
                    comboBox_signature_os.SelectedValue;
                SQLAdapter.InsertCommand.Parameters.Add("@HASH", SqlDbType.VarChar).Value = hash_all_name;

                try
                {
                    SQLAdapter.InsertCommand.ExecuteNonQuery(); // выполним запрос    
                    load_signature();                              // обновим таблицу
                    MessageBox.Show("Успешно добавлен!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // если не удачно обшика с инфой
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
                
        private void button7_Click(object sender, EventArgs e)
        {
            // --- [ УДАЛЕНИЕ ВЫБРАННЫХ ] ---   СИГНАТУРА

            if (ds.Tables["SIGNATURE"].Rows.Count > 0)              // проверка на наличие строк в таблице
            {
                strSQL = " DELETE FROM signature WHERE id_signature = @ID_S ";

                SQLAdapter.DeleteCommand = new SqlCommand(strSQL, cn);
                // Если нажата кномка да, удаления не избежать.
                if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                    + dataGridView3.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                {
                    try
                    {
                        foreach (DataGridViewRow drv in dataGridView3.SelectedRows)
                        {
                            SQLAdapter.DeleteCommand.Parameters.Add("@ID_S", SqlDbType.Int).Value =
                                Convert.ToInt32(ds.Tables["SIGNATURE"].Rows[drv.Index][0]);

                            SQLAdapter.DeleteCommand.ExecuteNonQuery();
                            SQLAdapter.DeleteCommand.Parameters.Clear();
                        }
                        load_signature();           // обновим таблицу
                        MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            // --- [ ДОБАВЛЕНИЕ ] ---  ТИП АТАКИ

            // проверим все поля на заполненность
            if (textBox_tip_attack_name.Text == "")
            {
                MessageBox.Show("Заполните все поля", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // запрос на добавление
            strSQL = "INSERT INTO tip_attack VALUES (@NAME)";

            // работаем через адаптер и свойство добавления
            SQLAdapter.InsertCommand = new SqlCommand(strSQL, cn);  // новая команда создана
            // определим параметры и зададим им значения
            SQLAdapter.InsertCommand.Parameters.Add("@NAME", SqlDbType.VarChar).Value = textBox_tip_attack_name.Text;
            try
            {
                SQLAdapter.InsertCommand.ExecuteNonQuery(); // выполним запрос
                // если удачно то...
                load_tip_attack();          // обновим таблицу
                MessageBox.Show("Успешно добавлен!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // если не удачно обшика с инфой
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            // --- [ ОБНОВЛЕНИЕ ] ---   ТИП АТАКИ

            if (ds.Tables["TIP_ATTACK"].Rows.Count > 0)              // проверка на наличие строк в таблице
            {
                // проверим все поля на заполненность
                if (textBox_tip_attack_name.Text == "")
                {
                    MessageBox.Show("Заполните все поля", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // запрос на обновление
                strSQL = " UPDATE tip_attack SET name = @NAME WHERE id_tip_attack = @ID_A ";

                SQLAdapter.UpdateCommand = new SqlCommand(strSQL, cn);  // команда для обноления создана
                // зададим значения параметрам 
                SQLAdapter.UpdateCommand.Parameters.Add("@NAME", SqlDbType.VarChar).Value = textBox_tip_attack_name.Text;
                SQLAdapter.UpdateCommand.Parameters.Add("@ID_A", SqlDbType.Int).Value =
                    Convert.ToInt32(ds.Tables["TIP_ATTACK"].Rows[dataGridView8.CurrentRow.Index][0]);
                try
                {
                    SQLAdapter.UpdateCommand.ExecuteNonQuery(); // выполним запрос
                    // если удачно то...
                    load_tip_attack();           // обновим таблицу
                    MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // если запрос выполнился не удачно то ошибка с инфой
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            // --- [ УДАЛЕНИЕ ВЫБРАННЫХ ] ---   ТИП АТАКИ

            if (ds.Tables["TIP_ATTACK"].Rows.Count > 0)              // проверка на наличие строк в таблице
            {
                strSQL = " DELETE FROM tip_attack WHERE id_tip_attack = @ID_A ";

                SQLAdapter.DeleteCommand = new SqlCommand(strSQL, cn);
                // Если нажата кномка да, удаления не избежать.
                if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                    + dataGridView8.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                {
                    try
                    {
                        foreach (DataGridViewRow drv in dataGridView8.SelectedRows)
                        {
                            SQLAdapter.DeleteCommand.Parameters.Add("@ID_A", SqlDbType.Int).Value =
                                Convert.ToInt32(ds.Tables["TIP_ATTACK"].Rows[drv.Index][0]);

                            SQLAdapter.DeleteCommand.ExecuteNonQuery();
                            SQLAdapter.DeleteCommand.Parameters.Clear();
                        }
                        load_tip_attack();           // обновим таблицу
                        MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // --- [ ДОБАВЛЕНИЕ ] ---  СЕРВИС

            // проверим все поля на заполненность
            if (textBox_service_name.Text == "")
            {
                MessageBox.Show("Заполните все поля", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // запрос на добавление
            strSQL = "INSERT INTO service VALUES (@NAME)";

            // работаем через адаптер и свойство добавления
            SQLAdapter.InsertCommand = new SqlCommand(strSQL, cn);  // новая команда создана
            // определим параметры и зададим им значения
            SQLAdapter.InsertCommand.Parameters.Add("@NAME", SqlDbType.VarChar).Value = textBox_service_name.Text;
            try
            {
                SQLAdapter.InsertCommand.ExecuteNonQuery(); // выполним запрос
                // если удачно то...
                load_service();           // обновим таблицу
                MessageBox.Show("Успешно добавлен!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // если не удачно обшика с инфой
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // --- [ ОБНОВЛЕНИЕ ] ---  СЕРВИС

            if (ds.Tables["SERVICE"].Rows.Count > 0)              // проверка на наличие строк в таблице
            {
                // проверим все поля на заполненность
                if (textBox_service_name.Text == "")
                {
                    MessageBox.Show("Заполните все поля", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // запрос на обновление
                strSQL = " UPDATE service SET name = @NAME WHERE id_service = @ID_S ";

                SQLAdapter.UpdateCommand = new SqlCommand(strSQL, cn);  // команда для обноления создана
                // зададим значения параметрам 
                SQLAdapter.UpdateCommand.Parameters.Add("@NAME", SqlDbType.VarChar).Value = textBox_service_name.Text;
                SQLAdapter.UpdateCommand.Parameters.Add("@ID_S", SqlDbType.Int).Value =
                    Convert.ToInt32(ds.Tables["SERVICE"].Rows[dataGridView1.CurrentRow.Index][0]);
                try
                {
                    SQLAdapter.UpdateCommand.ExecuteNonQuery(); // выполним запрос
                    // если удачно то...
                    load_service();           // обновим таблицу
                    MessageBox.Show("Запись успешно обновлена!", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // если запрос выполнился не удачно то ошибка с инфой
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
                MessageBox.Show("Таблица пуста", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // --- [ УДАЛЕНИЕ ВЫБРАННЫХ ] ---   СЕРВИС

            if (ds.Tables["SERVICE"].Rows.Count > 0)              // проверка на наличие строк в таблице
            {
                strSQL = " DELETE FROM service WHERE id_service = @ID_S ";

                SQLAdapter.DeleteCommand = new SqlCommand(strSQL, cn);
                // Если нажата кномка да, удаления не избежать.
                if (DialogResult.Yes == MessageBox.Show("Вы уверены в удалении? \nЗаписей:  "
                    + dataGridView1.SelectedRows.Count.ToString(), "Удаление", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                {
                    try
                    {
                        foreach (DataGridViewRow drv in dataGridView1.SelectedRows)
                        {
                            SQLAdapter.DeleteCommand.Parameters.Add("@ID_S", SqlDbType.Int).Value =
                                Convert.ToInt32(ds.Tables["SERVICE"].Rows[drv.Index][0]);

                            SQLAdapter.DeleteCommand.ExecuteNonQuery();
                            SQLAdapter.DeleteCommand.Parameters.Clear();
                        }
                        load_service();           // обновим таблицу
                        MessageBox.Show("Успешно удалено!", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
                MessageBox.Show("Таблица пуста", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checkBox_tip_attack_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_tip_attack.Checked == false)
            {
                comboBox_signature_tip_attack_f.Enabled = false;
                comboBox_signature_tip_attack_f.Text = "";
            }
            else
                comboBox_signature_tip_attack_f.Enabled = true;
            filter();
        }

        private void checkBox_protocol_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_protocol.Checked == false)
            {
                comboBox_signature_protocol_f.Enabled = false;
                comboBox_signature_protocol_f.Text = "";
            }
            else
                comboBox_signature_protocol_f.Enabled = true;
            filter();
        }

        private void checkBox_service_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_service.Checked == false)
            {
                comboBox_signature_service_f.Enabled = false;
                comboBox_signature_service_f.Text = "";
            }
            else
                comboBox_signature_service_f.Enabled = true;
            filter();
        }

        private void checkBox_os_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_os.Checked == false)
            {
                comboBox_signature_os_f.Enabled = false;
                comboBox_signature_os_f.Text = "";
            }
            else
                comboBox_signature_os_f.Enabled = true;
            filter();
        }

        private void comboBox_signature_tip_attack_f_SelectedIndexChanged(object sender, EventArgs e)
        {
            filter();
        }

        private void comboBox_signature_protocol_f_SelectedIndexChanged(object sender, EventArgs e)
        {
            filter();
        }

        private void comboBox_signature_service_f_SelectedIndexChanged(object sender, EventArgs e)
        {
            filter();
        }

        private void comboBox_signature_os_f_SelectedIndexChanged(object sender, EventArgs e)
        {
            filter();
        }

        private void tabPage5_Enter(object sender, EventArgs e)
        {
            // ВКЛАДКА ФИЛЬТРАЦИЯ
            // для корректной работы вкладок сигнатуры и фльтрации
            // они используют одну и ту же таблицу SIGNATURE 

            dataGridView3.DataSource = null;
            dataGridView5.DataSource = bs_signature;

            comboBox_signature_tip_attack.DataSource = null;
            comboBox_signature_protocol.DataSource = null;
            comboBox_signature_service.DataSource = null;
            comboBox_signature_os.DataSource = null;

            spravochnik_reload_f();

            check_v();
            controls_start();
            filter();
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            // ВКЛАДКА СИГНАТУРА
            // для корректной работы вкладок сигнатуры и фльтрации
            // они используют одну и ту же таблицу SIGNATURE 

            BindingSource bs = new BindingSource();
            bs.DataSource = ds.Tables["SIGNATURE"];
            bs.RemoveFilter();

            dataGridView3.DataSource = bs_signature;
            dataGridView5.DataSource = null;

            comboBox_signature_tip_attack_f.DataSource = null;
            comboBox_signature_protocol_f.DataSource = null;
            comboBox_signature_service_f.DataSource = null;
            comboBox_signature_os_f.DataSource = null;

            spravochnik_reload();

        }
    }
}