namespace prohodnay
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_vx_pas = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.visualStyler2 = new SkinSoft.VisualStyler.VisualStyler(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.visualStyler2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.textBox_vx_pas);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.listBox1);
            this.groupBox1.Location = new System.Drawing.Point(163, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(191, 225);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Вход";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(105, 193);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(77, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Выход";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(10, 193);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(77, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Вход";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox_vx_pas
            // 
            this.textBox_vx_pas.Location = new System.Drawing.Point(10, 167);
            this.textBox_vx_pas.Name = "textBox_vx_pas";
            this.textBox_vx_pas.PasswordChar = '*';
            this.textBox_vx_pas.Size = new System.Drawing.Size(172, 20);
            this.textBox_vx_pas.TabIndex = 3;
            this.textBox_vx_pas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_vx_pas_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Пароль";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Пользователи";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(10, 40);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(172, 108);
            this.listBox1.TabIndex = 0;
            // 
            // visualStyler2
            // 
            this.visualStyler2.License = ((SkinSoft.VisualStyler.Licensing.VisualStylerLicense)(resources.GetObject("visualStyler2.License")));
            this.visualStyler2.LoadVisualStyle(null, "OSX (Leopard).vssf");
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(366, 255);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form3";
            this.Text = " ";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form3_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form3_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form3_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form3_MouseUp);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.visualStyler2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_vx_pas;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private SkinSoft.VisualStyler.VisualStyler visualStyler1;
        private SkinSoft.VisualStyler.VisualStyler visualStyler2;
    }
}