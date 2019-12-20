namespace Navigator
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if(map!=null)
                map.Dispose();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_start_search = new System.Windows.Forms.Button();
            this.textBox_info = new System.Windows.Forms.TextBox();
            this.button_load_navdata = new System.Windows.Forms.Button();
            this.openFileDialog_map = new System.Windows.Forms.OpenFileDialog();
            this.pictureBox_navmap = new System.Windows.Forms.PictureBox();
            this.button_prev_map = new System.Windows.Forms.Button();
            this.button_next_map = new System.Windows.Forms.Button();
            this.button_first_point = new System.Windows.Forms.Button();
            this.button_last_point = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.button_stop_search = new System.Windows.Forms.Button();
            this.button_update_status_search = new System.Windows.Forms.Button();
            this.button_pts_add = new System.Windows.Forms.Button();
            this.button_save_pts = new System.Windows.Forms.Button();
            this.button_form_save_pts = new System.Windows.Forms.Button();
            this.button_create_ptsfile = new System.Windows.Forms.Button();
            this.saveFileDialog_PTS = new System.Windows.Forms.SaveFileDialog();
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            this.PTSSearch = new System.Windows.Forms.Button();
            this.PTSLoad = new System.Windows.Forms.Button();
            this.LandmarkFlag = new System.Windows.Forms.Button();
            this.Exit = new System.Windows.Forms.Button();
            this.button_pts_cost_set = new System.Windows.Forms.Button();
            this.checkBox_mode0 = new System.Windows.Forms.CheckBox();
            this.checkBox_mode1 = new System.Windows.Forms.CheckBox();
            this.checkBox_mode2 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_navmap)).BeginInit();
            this.SuspendLayout();
            // 
            // button_start_search
            // 
            this.button_start_search.Enabled = false;
            this.button_start_search.Location = new System.Drawing.Point(16, 160);
            this.button_start_search.Margin = new System.Windows.Forms.Padding(4);
            this.button_start_search.Name = "button_start_search";
            this.button_start_search.Size = new System.Drawing.Size(109, 71);
            this.button_start_search.TabIndex = 0;
            this.button_start_search.Text = "Искать путь";
            this.button_start_search.UseVisualStyleBackColor = true;
            this.button_start_search.Click += new System.EventHandler(this.StartSearch);
            // 
            // textBox_info
            // 
            this.textBox_info.Location = new System.Drawing.Point(13, 239);
            this.textBox_info.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_info.Multiline = true;
            this.textBox_info.Name = "textBox_info";
            this.textBox_info.ReadOnly = true;
            this.textBox_info.Size = new System.Drawing.Size(257, 114);
            this.textBox_info.TabIndex = 100;
            this.textBox_info.TabStop = false;
            this.textBox_info.MouseClick += new System.Windows.Forms.MouseEventHandler(this.StopSelectTextBoxes);
            // 
            // button_load_navdata
            // 
            this.button_load_navdata.Location = new System.Drawing.Point(16, 15);
            this.button_load_navdata.Margin = new System.Windows.Forms.Padding(4);
            this.button_load_navdata.Name = "button_load_navdata";
            this.button_load_navdata.Size = new System.Drawing.Size(255, 28);
            this.button_load_navdata.TabIndex = 1;
            this.button_load_navdata.Text = "Загрузка карты";
            this.button_load_navdata.UseVisualStyleBackColor = true;
            this.button_load_navdata.Click += new System.EventHandler(this.OpenNavData);
            // 
            // openFileDialog_map
            // 
            this.openFileDialog_map.FileName = "openFileDialog1";
            // 
            // pictureBox_navmap
            // 
            this.pictureBox_navmap.Enabled = false;
            this.pictureBox_navmap.Location = new System.Drawing.Point(280, 15);
            this.pictureBox_navmap.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox_navmap.Name = "pictureBox_navmap";
            this.pictureBox_navmap.Size = new System.Drawing.Size(993, 613);
            this.pictureBox_navmap.TabIndex = 6;
            this.pictureBox_navmap.TabStop = false;
            this.pictureBox_navmap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GetIndexFromMap);
            // 
            // button_prev_map
            // 
            this.button_prev_map.Enabled = false;
            this.button_prev_map.Location = new System.Drawing.Point(16, 51);
            this.button_prev_map.Margin = new System.Windows.Forms.Padding(4);
            this.button_prev_map.Name = "button_prev_map";
            this.button_prev_map.Size = new System.Drawing.Size(109, 66);
            this.button_prev_map.TabIndex = 7;
            this.button_prev_map.Text = "Предыдущий кусок";
            this.button_prev_map.UseVisualStyleBackColor = true;
            this.button_prev_map.Click += new System.EventHandler(this.PrevMap);
            // 
            // button_next_map
            // 
            this.button_next_map.Enabled = false;
            this.button_next_map.Location = new System.Drawing.Point(153, 50);
            this.button_next_map.Margin = new System.Windows.Forms.Padding(4);
            this.button_next_map.Name = "button_next_map";
            this.button_next_map.Size = new System.Drawing.Size(117, 66);
            this.button_next_map.TabIndex = 8;
            this.button_next_map.Text = "Следующий кусок";
            this.button_next_map.UseVisualStyleBackColor = true;
            this.button_next_map.Click += new System.EventHandler(this.NextMap);
            // 
            // button_first_point
            // 
            this.button_first_point.Enabled = false;
            this.button_first_point.Location = new System.Drawing.Point(16, 124);
            this.button_first_point.Margin = new System.Windows.Forms.Padding(4);
            this.button_first_point.Name = "button_first_point";
            this.button_first_point.Size = new System.Drawing.Size(109, 28);
            this.button_first_point.TabIndex = 12;
            this.button_first_point.Text = "Точка начала";
            this.button_first_point.UseVisualStyleBackColor = true;
            this.button_first_point.Click += new System.EventHandler(this.FirstPoint);
            // 
            // button_last_point
            // 
            this.button_last_point.Enabled = false;
            this.button_last_point.Location = new System.Drawing.Point(153, 124);
            this.button_last_point.Margin = new System.Windows.Forms.Padding(4);
            this.button_last_point.Name = "button_last_point";
            this.button_last_point.Size = new System.Drawing.Size(117, 28);
            this.button_last_point.TabIndex = 13;
            this.button_last_point.Text = "Точка конца";
            this.button_last_point.UseVisualStyleBackColor = true;
            this.button_last_point.Click += new System.EventHandler(this.LastPoint);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.OpenFileOrTwoWaySearchBackgroubdThread);
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.WorkerSupportsCancellation = true;
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.OneWaySearchBackgroundThread);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.OneWaySearchCompleted);
            // 
            // button_stop_search
            // 
            this.button_stop_search.Enabled = false;
            this.button_stop_search.Location = new System.Drawing.Point(153, 196);
            this.button_stop_search.Margin = new System.Windows.Forms.Padding(4);
            this.button_stop_search.Name = "button_stop_search";
            this.button_stop_search.Size = new System.Drawing.Size(117, 28);
            this.button_stop_search.TabIndex = 3;
            this.button_stop_search.Text = "Стоп";
            this.button_stop_search.UseVisualStyleBackColor = true;
            this.button_stop_search.Click += new System.EventHandler(this.StopSearch);
            // 
            // button_update_status_search
            // 
            this.button_update_status_search.Enabled = false;
            this.button_update_status_search.Location = new System.Drawing.Point(153, 160);
            this.button_update_status_search.Margin = new System.Windows.Forms.Padding(4);
            this.button_update_status_search.Name = "button_update_status_search";
            this.button_update_status_search.Size = new System.Drawing.Size(117, 28);
            this.button_update_status_search.TabIndex = 2;
            this.button_update_status_search.Text = "Обновить";
            this.button_update_status_search.UseVisualStyleBackColor = true;
            this.button_update_status_search.Click += new System.EventHandler(this.UpdateSearchStatus);
            // 
            // button_pts_add
            // 
            this.button_pts_add.Location = new System.Drawing.Point(16, 477);
            this.button_pts_add.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_pts_add.Name = "button_pts_add";
            this.button_pts_add.Size = new System.Drawing.Size(109, 23);
            this.button_pts_add.TabIndex = 101;
            this.button_pts_add.Text = "Add PTS";
            this.button_pts_add.UseVisualStyleBackColor = true;
            this.button_pts_add.Click += new System.EventHandler(this.button_pts_add_Click);
            // 
            // button_save_pts
            // 
            this.button_save_pts.Location = new System.Drawing.Point(16, 504);
            this.button_save_pts.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_save_pts.Name = "button_save_pts";
            this.button_save_pts.Size = new System.Drawing.Size(109, 53);
            this.button_save_pts.TabIndex = 102;
            this.button_save_pts.Text = "Finish adding";
            this.button_save_pts.UseVisualStyleBackColor = true;
            this.button_save_pts.Click += new System.EventHandler(this.button_save_pts_Click);
            // 
            // button_form_save_pts
            // 
            this.button_form_save_pts.Location = new System.Drawing.Point(153, 477);
            this.button_form_save_pts.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_form_save_pts.Name = "button_form_save_pts";
            this.button_form_save_pts.Size = new System.Drawing.Size(117, 23);
            this.button_form_save_pts.TabIndex = 103;
            this.button_form_save_pts.Text = "Save PTS";
            this.button_form_save_pts.UseVisualStyleBackColor = true;
            this.button_form_save_pts.Click += new System.EventHandler(this.button_form_save_pts_Click);
            // 
            // button_create_ptsfile
            // 
            this.button_create_ptsfile.Location = new System.Drawing.Point(153, 504);
            this.button_create_ptsfile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_create_ptsfile.Name = "button_create_ptsfile";
            this.button_create_ptsfile.Size = new System.Drawing.Size(117, 53);
            this.button_create_ptsfile.TabIndex = 104;
            this.button_create_ptsfile.Text = "Create PTS file";
            this.button_create_ptsfile.UseVisualStyleBackColor = true;
            this.button_create_ptsfile.Click += new System.EventHandler(this.button_create_ptsfile_Click);
            // 
            // saveFileDialog_PTS
            // 
            this.saveFileDialog_PTS.Filter = "PTSM files(*.xml.PTS)|*.xml.PTS";
            // 
            // backgroundWorker3
            // 
            this.backgroundWorker3.DoWork += new System.ComponentModel.DoWorkEventHandler(this.LoadALTPaths);
            this.backgroundWorker3.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.LoadALTComplete);
            // 
            // PTSSearch
            // 
            this.PTSSearch.Enabled = false;
            this.PTSSearch.Location = new System.Drawing.Point(153, 374);
            this.PTSSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PTSSearch.Name = "PTSSearch";
            this.PTSSearch.Size = new System.Drawing.Size(117, 50);
            this.PTSSearch.TabIndex = 107;
            this.PTSSearch.Text = "PTSSearh";
            this.PTSSearch.UseVisualStyleBackColor = true;
            this.PTSSearch.Click += new System.EventHandler(this.PTSSearch_Click);
            // 
            // PTSLoad
            // 
            this.PTSLoad.Location = new System.Drawing.Point(16, 374);
            this.PTSLoad.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PTSLoad.Name = "PTSLoad";
            this.PTSLoad.Size = new System.Drawing.Size(109, 50);
            this.PTSLoad.TabIndex = 108;
            this.PTSLoad.Text = "PTS Load";
            this.PTSLoad.UseVisualStyleBackColor = true;
            this.PTSLoad.Click += new System.EventHandler(this.PTSLoad_Click);
            // 
            // LandmarkFlag
            // 
            this.LandmarkFlag.Location = new System.Drawing.Point(113, 562);
            this.LandmarkFlag.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.LandmarkFlag.Name = "LandmarkFlag";
            this.LandmarkFlag.Size = new System.Drawing.Size(157, 26);
            this.LandmarkFlag.TabIndex = 109;
            this.LandmarkFlag.Text = "LandmarkFlag";
            this.LandmarkFlag.UseVisualStyleBackColor = true;
            this.LandmarkFlag.Click += new System.EventHandler(this.LandmarkFlag_Click);
            // 
            // Exit
            // 
            this.Exit.Location = new System.Drawing.Point(45, 605);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(198, 23);
            this.Exit.TabIndex = 110;
            this.Exit.Text = "Exit";
            this.Exit.UseVisualStyleBackColor = true;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // button_pts_cost_set
            // 
            this.button_pts_cost_set.Location = new System.Drawing.Point(16, 562);
            this.button_pts_cost_set.Name = "button_pts_cost_set";
            this.button_pts_cost_set.Size = new System.Drawing.Size(75, 23);
            this.button_pts_cost_set.TabIndex = 111;
            this.button_pts_cost_set.Text = "SetCost";
            this.button_pts_cost_set.UseVisualStyleBackColor = true;
            this.button_pts_cost_set.Click += new System.EventHandler(this.button_pts_cost_set_Click);
            // 
            // checkBox_mode0
            // 
            this.checkBox_mode0.AutoSize = true;
            this.checkBox_mode0.Checked = true;
            this.checkBox_mode0.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_mode0.Location = new System.Drawing.Point(93, 429);
            this.checkBox_mode0.Name = "checkBox_mode0";
            this.checkBox_mode0.Size = new System.Drawing.Size(79, 21);
            this.checkBox_mode0.TabIndex = 112;
            this.checkBox_mode0.Text = "I режим";
            this.checkBox_mode0.UseVisualStyleBackColor = true;
            this.checkBox_mode0.CheckedChanged += new System.EventHandler(this.checkBox_mode0_CheckedChanged);
            // 
            // checkBox_mode1
            // 
            this.checkBox_mode1.AutoSize = true;
            this.checkBox_mode1.Location = new System.Drawing.Point(27, 451);
            this.checkBox_mode1.Name = "checkBox_mode1";
            this.checkBox_mode1.Size = new System.Drawing.Size(82, 21);
            this.checkBox_mode1.TabIndex = 113;
            this.checkBox_mode1.Text = "II режим";
            this.checkBox_mode1.UseVisualStyleBackColor = true;
            this.checkBox_mode1.CheckedChanged += new System.EventHandler(this.checkBox_mode1_CheckedChanged);
            // 
            // checkBox_mode2
            // 
            this.checkBox_mode2.AutoSize = true;
            this.checkBox_mode2.Location = new System.Drawing.Point(153, 451);
            this.checkBox_mode2.Name = "checkBox_mode2";
            this.checkBox_mode2.Size = new System.Drawing.Size(85, 21);
            this.checkBox_mode2.TabIndex = 114;
            this.checkBox_mode2.Text = "III режим";
            this.checkBox_mode2.UseVisualStyleBackColor = true;
            this.checkBox_mode2.CheckedChanged += new System.EventHandler(this.checkBox_mode2_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1311, 641);
            this.Controls.Add(this.checkBox_mode2);
            this.Controls.Add(this.checkBox_mode1);
            this.Controls.Add(this.checkBox_mode0);
            this.Controls.Add(this.button_pts_cost_set);
            this.Controls.Add(this.Exit);
            this.Controls.Add(this.LandmarkFlag);
            this.Controls.Add(this.PTSLoad);
            this.Controls.Add(this.PTSSearch);
            this.Controls.Add(this.button_create_ptsfile);
            this.Controls.Add(this.button_form_save_pts);
            this.Controls.Add(this.button_save_pts);
            this.Controls.Add(this.button_pts_add);
            this.Controls.Add(this.button_update_status_search);
            this.Controls.Add(this.button_stop_search);
            this.Controls.Add(this.button_last_point);
            this.Controls.Add(this.button_first_point);
            this.Controls.Add(this.button_next_map);
            this.Controls.Add(this.button_prev_map);
            this.Controls.Add(this.pictureBox_navmap);
            this.Controls.Add(this.button_load_navdata);
            this.Controls.Add(this.textBox_info);
            this.Controls.Add(this.button_start_search);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Navigator";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_navmap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_start_search;
        private System.Windows.Forms.TextBox textBox_info;
        private System.Windows.Forms.Button button_load_navdata;
        private System.Windows.Forms.OpenFileDialog openFileDialog_map;
        private System.Windows.Forms.PictureBox pictureBox_navmap;
        private System.Windows.Forms.Button button_prev_map;
        private System.Windows.Forms.Button button_next_map;
        private System.Windows.Forms.Button button_first_point;
        private System.Windows.Forms.Button button_last_point;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.Button button_stop_search;
        private System.Windows.Forms.Button button_update_status_search;
        private System.Windows.Forms.Button button_pts_add;
        private System.Windows.Forms.Button button_save_pts;
        private System.Windows.Forms.Button button_form_save_pts;
        private System.Windows.Forms.Button button_create_ptsfile;
        private System.Windows.Forms.SaveFileDialog saveFileDialog_PTS;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        private System.Windows.Forms.Button PTSSearch;
        private System.Windows.Forms.Button PTSLoad;
        private System.Windows.Forms.Button LandmarkFlag;
        private System.Windows.Forms.Button Exit;
        private System.Windows.Forms.Button button_pts_cost_set;
        private System.Windows.Forms.CheckBox checkBox_mode0;
        private System.Windows.Forms.CheckBox checkBox_mode1;
        private System.Windows.Forms.CheckBox checkBox_mode2;
    }
}

