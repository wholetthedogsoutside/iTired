﻿using iTired.HexEditor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace iTired
{
    public partial class Form_main : Form
    {
        private RootDirectory root;

        private readonly string size_error = "RD entry's size must be exactly " + Constants.RD_ENTRY_LENGTH + " bytes.";

        private const string
            name = "iTired v1.1",
            seper = " - ";

        public Form_main()
        {
            InitializeComponent();
            Text = name;

            byte[] data = new byte[Constants.RD_ENTRY_LENGTH];

            for (int i = 0; i < data.Length; i++)
                data[i] = 0;

            richTextBox_data.ByteProvider = new DynamicByteProvider(data);
            richTextBox_data.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, SystemFonts.MessageBoxFont.Size, SystemFonts.MessageBoxFont.Style);
            richTextBox_data_KeyDown();
        }


        public /*static*/ string RemoveWhitespace(/*this*/ string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Created by Nick Bruno.\n" +
                "The hex editor is from Be.HexEditor.\n" +
                "This utility reads root directory entries from the FAT file system.\n" +
                "GitHub: https://github.com/wholetthedogsoutside/");
        }

        private void clear()
        {
            groupBox_info.Enabled = false;
            checkBox_readonly.Checked =
            checkBox_hidden.Checked =
            checkBox_system.Checked =
            checkBox_volume_label.Checked =
            checkBox_folder.Checked =
            checkBox_archive.Checked = false;

            textBox_fileBody.Text =
            textBox_file_ext.Text =
            textBox_start_cluster_num.Text =
            textBox_file_byte_size.Text =
            textBox_created_date.Text =
            textBox_modified_date.Text =
            textBox_access_date.Text =
            textBox_created_time.Text =
            textBox_modified_time.Text = "";
        }

        private void richTextBox_data_KeyDown(object sender = null, KeyEventArgs e = null)
        {
            //43 4F 4D 4D 41 4E 44 53 54 58 54 20 18 53 DD 98 8C 49 5F 4B 00 00 93 A0 3B 4B 24 01 27 54 00 00
            try
            {
                byte[] data = (((DynamicByteProvider)richTextBox_data.ByteProvider)._bytes).ToArray();
                if (data.Length == Constants.RD_ENTRY_LENGTH)
                {
                    root = new RootDirectory(data);

                    textBox_fileBody.Text = root.FileName;
                    textBox_file_ext.Text = root.FileExtension;

                    checkBox_readonly.Checked = root.isReadonly;
                    checkBox_hidden.Checked = root.isHidden;
                    checkBox_system.Checked = root.isSystemFile;
                    checkBox_volume_label.Checked = root.isVolumeLabel;
                    checkBox_folder.Checked = root.isFolder;
                    checkBox_archive.Checked = root.isArchive;
                    textBox_start_cluster_num.Text = root.start_cluster.ToString();
                    textBox_file_byte_size.Text = root.byteSize.ToString();
                    textBox_created_date.Text = root.createdDate;
                    textBox_modified_date.Text = root.modifyDate;
                    textBox_access_date.Text = root.accessDate;

                    textBox_created_time.Text = root.createdTime;
                    textBox_modified_time.Text = root.modifyTime;

                    if (!groupBox_info.Enabled)
                        groupBox_info.Enabled = true;
                }
                else
                {
                    clear();
                    //MessageBox.Show(size_error);
                }

            }
            catch
            {
                clear();
            }
        }

        private void richTextBox_data_ByteProviderChanged(object sender, EventArgs e)
        { richTextBox_data_KeyDown();  }
        private void richTextBox_data_InsertActiveChanged(object sender, EventArgs e)
        { richTextBox_data_KeyDown(); }

        private void richTextBox_data_MouseUp(object sender, MouseEventArgs e)
        { richTextBox_data_KeyDown(); }

        private void richTextBox_data_KeyUp(object sender, KeyEventArgs e)
        { richTextBox_data_KeyDown(); }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog FD = new OpenFileDialog();

            if (FD.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(FD.FileName))
                {
                    byte[] data = File.ReadAllBytes(FD.FileName);

                    if (data.Length == Constants.RD_ENTRY_LENGTH)
                    {
                        richTextBox_data.ByteProvider = new DynamicByteProvider(data);
                    }
                    else MessageBox.Show(size_error);
                }
            }
        }
    }
}
