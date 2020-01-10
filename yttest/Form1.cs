using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VideoLibrary;

namespace YouYubeLinkTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private const string YoutubeLinkRegex = "(?:.+?)?(?:\\/v\\/|watch\\/|\\?v=|\\&v=|youtu\\.be\\/|\\/v=|^youtu\\.be\\/)([a-zA-Z0-9_-]{11})+"; // Annetun linkin validointi

        internal static string ValidateVideoLink(string input)
        {
            var regex = new Regex(YoutubeLinkRegex, RegexOptions.Compiled);
            foreach (Match match in regex.Matches(input))
            {
                //Console.WriteLine(match);
                foreach (var groupdata in match.Groups.Cast<Group>().Where(groupdata => !groupdata.ToString().StartsWith("http://") && !groupdata.ToString().StartsWith("https://") && !groupdata.ToString().StartsWith("youtu") && !groupdata.ToString().StartsWith("www.")))
                {
                    return groupdata.ToString();
                }
            }
            return string.Empty;
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) // Datan sorttaaminen datagridin headeria klikkaamalla
        {
            if (e.Button == MouseButtons.Left)
            {
                DataGridViewColumn newColumn =
                dataGridView1.Columns[e.ColumnIndex];

                SortOrder direction;
                if (dataGridView1.SortOrder == SortOrder.Ascending)
                {
                    dataGridView1.Sort(newColumn,
                    ListSortDirection.Ascending);
                    direction = SortOrder.Ascending;
                }
                else
                {
                    dataGridView1.Sort(newColumn,
                    ListSortDirection.Descending);
                    direction = SortOrder.Descending;
                }
                newColumn.HeaderCell.SortGlyphDirection = direction;
            }
        }

        private void btnClear_Click(object sender, EventArgs e) // DataGridin tyhjentäminen tapahtuu tässä
        {
            int numRows = dataGridView1.Rows.Count;
            for (int i = 0; i < numRows; i++)
            {
                try
                {
                    int max = dataGridView1.Rows.Count - 1;
                    dataGridView1.Rows.Remove(dataGridView1.Rows[max]);
                    pictureBoxYT.Visible = true;
                }
                catch (Exception exe)
                {
                    MessageBox.Show("You can´t delete A row " + exe, "WTF",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)  // Tehdään tietopyyntö linkistä ja käsitellään virheet
        {
            pictureBoxYT.Visible = false;
            try
            {
                var list = YouTube.Default.GetAllVideos(string.Format(UrlBox.Text));
                dataGridView1.DataSource = new System.Collections.ObjectModel.ObservableCollection<YouTubeVideo>(list).ToBindingList();
            }
            catch (Exception exe)
            {
                MessageBox.Show("There may be a problem with your URL " + exe, "WTF",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UrlBox_TextChanged(object sender, EventArgs e)  //Testataan että YouTube Linkki vaikuttaa oikealta
        {
            if (ValidateVideoLink(UrlBox.Text) != string.Empty)
            {
                btnSearch.Enabled = true;
                while(this.Height == 510)
                {
                    this.Height++;
                }
            }
            else
            {
                btnSearch.Enabled = false;
            }
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if ((Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Resolution"].Value) < 1) && Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["AudioBitrate"].Value) < 1)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightPink;
            }
            else if((Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Resolution"].Value) > 1) && Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["AudioBitrate"].Value) > 1)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
            }
            else if ((Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Resolution"].Value) > 1) && Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["AudioBitrate"].Value) == -1)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCyan;
            }
            else if ((Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Resolution"].Value) == -1) && Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["AudioBitrate"].Value) > 1)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Lime;
            }
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Copy" && dataGridView1.CurrentCell.Value != null)
            {
                Clipboard.SetDataObject(dataGridView1.CurrentCell.Value.ToString(), false);
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (/*e.Button == MouseButtons.Right &&*/ e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ContextMenuStrip = contextMenuStrip1;
                dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }
    }
}
