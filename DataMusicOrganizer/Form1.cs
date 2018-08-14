using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DataMusicOrganizer
{
    public partial class Form1 : Form
    {
        int i;
        int row_playing;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {            
            dataGridView1.AllowUserToAddRows = false;
            lblSongsFound.Text = "";
            lblSongPlaying.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;
                Cursor.Current = Cursors.WaitCursor;
                LoadMusic(selectedPath);
                Cursor.Current = Cursors.Default;
            }
        }

        public void LoadMusic(string dir)
        {
            dataGridView1.Rows.Clear();
            TagLib.File f2 = null;
            FileInfo f = null;
            FileSystemInfo f1 = null;
            bool path_B;
            string path;
            String[] s1 = Directory.GetFiles(dir, "*.mp3", SearchOption.AllDirectories);
            int j = 0;
            for (i = 0; i < s1.Length; i++)
            {
                f = new FileInfo(s1[i]);
                f1 = new FileInfo(s1[i]);
                path_B = f1.Exists;
                if (path_B && f.Length != 0)
                {                    
                    try
                    {
                        path = f1.FullName;
                        f2 = TagLib.File.Create(path);
                        if (!f2.PossiblyCorrupt)
                        {
                            dataGridView1.Rows.Add();
                            dataGridView1.Rows[j].Cells[0].Value = path;
                            dataGridView1.Rows[j].Cells[1].Value = f1.Name;
                            dataGridView1.Rows[j].Cells[2].Value = f2.Tag.Title;
                            dataGridView1.Rows[j].Cells[3].Value = f2.Tag.FirstPerformer;
                            dataGridView1.Rows[j].Cells[4].Value = f2.Tag.Album;
                            dataGridView1.Rows[j].Cells[5].Value = f2.Tag.JoinedGenres;
                            dataGridView1.Rows[j].Cells[6].Value = f2.Tag.Track.ToString();
                            j++;
                        }
                    }
                    catch (TagLib.CorruptFileException ex)
                    {
                        MessageBox.Show((f1.FullName + " " + ex.Message), "Hey!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }            
            f2.Dispose();
            if(j==1)
                lblSongsFound.Text = "You have found " + j.ToString() + " file.";
            else
                lblSongsFound.Text = "You have found " + j.ToString() + " files.";
        }



        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string path = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                TagLib.File f = TagLib.File.Create(path);
                switch (e.ColumnIndex)
                {
                    case 2:
                        if (dataGridView1.CurrentRow.Cells[2].Value == null)
                            f.Tag.Title = "";
                        else
                            f.Tag.Title = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                        break;
                    case 3:
                        if (dataGridView1.CurrentRow.Cells[3].Value == null)
                            f.Tag.Performers = new[] { "" };
                        else
                            f.Tag.Performers = new[] { dataGridView1.CurrentRow.Cells[3].Value.ToString() };
                        break;
                    case 4:
                        if (dataGridView1.CurrentRow.Cells[4].Value == null)
                            f.Tag.Album = "";
                        else
                            f.Tag.Album = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                        break;
                    case 5:
                        if (dataGridView1.CurrentRow.Cells[5].Value == null)
                            f.Tag.Genres = new[] { "" };
                        else
                            f.Tag.Genres = new[] { dataGridView1.CurrentRow.Cells[5].Value.ToString() };
                        break;
                    case 6:
                        if (dataGridView1.CurrentRow.Cells[6].Value == null)
                            f.Tag.Track = 0;
                        else
                            f.Tag.Track = Convert.ToUInt32(dataGridView1.CurrentRow.Cells[6].Value.ToString());
                        break;
                }
                f.Save();
            }
            catch (IOException ex)
            {
                SetCorrectValuesAfterError();
                MessageBox.Show(ex.Message, "Oops :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException un)
            {
                SetCorrectValuesAfterError();
                MessageBox.Show(un.Message, "Oops :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetCorrectValuesAfterError()
        {
            string path = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            TagLib.File f = TagLib.File.Create(path);
            if (!f.PossiblyCorrupt)
            {
                string title = f.Tag.Title;
                string artist = f.Tag.FirstPerformer;
                string album = f.Tag.Album;
                string genre = f.Tag.JoinedGenres;
                string trackn = f.Tag.Track.ToString();
                dataGridView1.CurrentRow.Cells[2].Value = title;
                dataGridView1.CurrentRow.Cells[3].Value = artist;
                dataGridView1.CurrentRow.Cells[4].Value = album;
                dataGridView1.CurrentRow.Cells[5].Value = genre;
                dataGridView1.CurrentRow.Cells[6].Value = trackn;
            }
        }

        private void SetCorrectValuesAfterMultipleErrors(int r)
        {
            string path = dataGridView1.Rows[r].Cells[0].Value.ToString();
            TagLib.File f = TagLib.File.Create(path);
            if (!f.PossiblyCorrupt)
            {
                string title = f.Tag.Title;
                string artist = f.Tag.FirstPerformer;
                string album = f.Tag.Album;
                string genre = f.Tag.JoinedGenres;
                string trackn = f.Tag.Track.ToString();
                dataGridView1.Rows[r].Cells[2].Value = title;
                dataGridView1.Rows[r].Cells[3].Value = artist;
                dataGridView1.Rows[r].Cells[4].Value = album;
                dataGridView1.Rows[r].Cells[5].Value = genre;
                dataGridView1.Rows[r].Cells[6].Value = trackn;
            }
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    DataGridViewCell cell = dataGridView1.Rows[dataGridView1.CurrentRow.Index - 1].Cells[dataGridView1.CurrentCell.ColumnIndex];
                    dataGridView1.CurrentCell = cell;
                    dataGridView1.BeginEdit(true);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                axWindowsMediaPlayer1.URL = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                if (dataGridView1.CurrentRow.Cells[2].Value != null && dataGridView1.CurrentRow.Cells[3].Value != null)
                    lblSongPlaying.Text = "Now playing: " + dataGridView1.CurrentRow.Cells[3].Value.ToString() + " - " + dataGridView1.CurrentRow.Cells[2].Value.ToString();
                else
                {
                    lblSongPlaying.Text = "Now playing: " + dataGridView1.CurrentRow.Cells[1].Value.ToString();
                }
            }
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {

            var dataGridView = (sender as DataGridView);

            if (e.ColumnIndex >= 0 && e.ColumnIndex == 1&&e.RowIndex>=0)
                dataGridView.Cursor = Cursors.Hand;
            else
                dataGridView.Cursor = Cursors.Default;
            
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == 3)
            {
                dataGridView1.Rows[row_playing].ReadOnly = false;
                row_playing = dataGridView1.CurrentRow.Index;
                dataGridView1.CurrentRow.ReadOnly = true;                
            }

            if (e.newState == 8 || e.newState == 1)
            {                
                axWindowsMediaPlayer1.URL = "";
                lblSongPlaying.Text = "";
                dataGridView1.Rows[row_playing].ReadOnly = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < 1)
                MessageBox.Show("Please write... Something.", "Hey,", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            foreach (DataGridViewCell r in dataGridView1.SelectedCells)
            {
                r.Value = textBox1.Text;
                try
                {
                    string path = dataGridView1.Rows[r.RowIndex].Cells[0].Value.ToString();
                    TagLib.File f = TagLib.File.Create(path);
                    switch (r.ColumnIndex)
                    {
                        case 2:
                            if (dataGridView1.Rows[r.RowIndex].Cells[2].Value == null)
                                f.Tag.Title = "";
                            else
                                f.Tag.Title = dataGridView1.Rows[r.RowIndex].Cells[2].Value.ToString();
                            break;
                        case 3:
                            if (dataGridView1.Rows[r.RowIndex].Cells[3].Value == null)
                                f.Tag.Performers = new[] { "" };
                            else
                                f.Tag.Performers = new[] { dataGridView1.Rows[r.RowIndex].Cells[3].Value.ToString() };
                            break;
                        case 4:
                            if (dataGridView1.Rows[r.RowIndex].Cells[4].Value == null)
                                f.Tag.Album = "";
                            else
                                f.Tag.Album = dataGridView1.Rows[r.RowIndex].Cells[4].Value.ToString();
                            break;
                        case 5:
                            if (dataGridView1.Rows[r.RowIndex].Cells[5].Value == null)
                                f.Tag.Genres = new[] { "" };
                            else
                                f.Tag.Genres = new[] { dataGridView1.Rows[r.RowIndex].Cells[5].Value.ToString() };
                            break;
                        case 6:
                            if (dataGridView1.Rows[r.RowIndex].Cells[6].Value == null)
                                f.Tag.Track = 0;
                            else
                                f.Tag.Track = Convert.ToUInt32(dataGridView1.Rows[r.RowIndex].Cells[6].Value.ToString());
                            break;
                    }
                    f.Save();
                }
                catch (IOException ex)
                {
                    SetCorrectValuesAfterMultipleErrors(r.RowIndex);
                    MessageBox.Show(ex.Message, "Oops :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (UnauthorizedAccessException un)
                {
                    SetCorrectValuesAfterMultipleErrors(r.RowIndex);
                    MessageBox.Show(un.Message, "Oops :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            axWindowsMediaPlayer1.URL = "";
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                }
            }
        }
    }
}