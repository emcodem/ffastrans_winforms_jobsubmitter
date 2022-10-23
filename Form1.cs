
using System.Reflection;
using FFAStrans_Concat_Submitter.Classes;
using Nett; //toml config file
using System.Text.Json;
using System.Text.Json.Nodes;
using System.ComponentModel;

namespace FFAStrans_Concat_Submitter
{

    public partial class Form1 : Form
    {
        private FFAStrans m_ffastrans;

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public Form1()
        {
            InitializeComponent();
            dataGridView1.ColumnCount = 1;
            dataGridView1.Columns[0].Name = "Filename";
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //m_config = Config.Build(AssemblyDirectory + "\\config.txt");
            try { parseConfig(); }catch(Exception ex)
            {
                MessageBox.Show("There was a problem parsing config.toml, please make sure you have valid toml structure", ex.Message);
            }

        }

        private void parseConfig() {
            //parse configuration file and 
            List<ComboBoxWorkflowItem> wf_list = new List<ComboBoxWorkflowItem>();
            TomlTable tomlConfig = Toml.ReadFile(AssemblyDirectory + "\\config.toml");
            Dictionary<string, object> allConfigEntries= tomlConfig.ToDictionary();
            //get ffastrans api root
            foreach (var _dict in (Dictionary<string, object>)allConfigEntries["ffastrans"])
            {
                if (_dict.Key == "api") {
                    m_ffastrans = new FFAStrans((string)_dict.Value);
                }
            }

            //populate workflow dropdown
            foreach (var _wfdict in (Dictionary<string, object>[]) allConfigEntries["workflow"]) {
                wf_list.Add(new ComboBoxWorkflowItem(_wfdict));
            }
            BindingSource bs = new BindingSource();
            bs.DataSource = wf_list;
            comboBoxWorkflows.DataSource = bs.DataSource;
            comboBoxWorkflows.DisplayMember = "Name";
            comboBoxWorkflows.ValueMember = "Name";

        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            /* File browser dialog */
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                var files = openFileDialog1.FileNames;
                addFileListToGrid(files);
            }
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            // Create one gridrow foreach dropped file
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                addFileListToGrid(files);
            }
        }

        private void addFileListToGrid(string[] files_folders) {
            try
            {
                foreach (string file in files_folders)
                {
                    string[] row = new string[] { file };
                    dataGridView1.Rows.Add(row);
                }
                this.dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            // If the data is a file, display the copy cursor.
                
                e.Effect = DragDropEffects.Copy;
                
        }

        private async void buttonStart_Click(object sender, EventArgs e)
        {
            /* Starts FFAStrans job */
            if (dataGridView1.RowCount == 0) {
                MessageBox.Show("Got no Files, please drag and drop sourcefiles to the List");
                return;
            }

            buttonStart.Enabled = false;

            //grab list of files, push to json array
            JsonArray json_filelist = new JsonArray();
            var selectedItem = comboBoxWorkflows.SelectedItem;
            foreach (DataGridViewRow row in dataGridView1.Rows) {
                var filepath = row.Cells[0].Value;
                json_filelist.Add(filepath);
            }
            try
            {
                //submit ffastrans job
                await m_ffastrans.startWorkflow((ComboBoxWorkflowItem)selectedItem, json_filelist);

            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                MessageBox.Show("Error communicating with FFAStrans (maybe server is offline?). Unexpected Exception Message:  \n\n" + ex.Message);
            }
            catch (Exception ex) {
                MessageBox.Show("Error? Unexpected Exception Message:  \n\n" + ex.Message);
            }
            buttonStart.Enabled = true;

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int count = dataGridView1.SelectedRows.Count;
            while (count != 0)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                count--;
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void comboBoxWorkflows_MouseClick(object sender, MouseEventArgs e)
        {
            comboBoxWorkflows.DroppedDown = true;
        }

        private void btnUp_Click(object sender, EventArgs e)
        { /* move row up/down is a little bit tedious - only works for single rows */
            DataGridView dgv = dataGridView1;
            try
            {
                int totalRows = dgv.Rows.Count;
                // get index of the row for the selected cell
                int rowIndex = dgv.SelectedCells[0].OwningRow.Index;
                if (rowIndex == 0)
                    return;
                // get index of the column for the selected cell
                int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[rowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(rowIndex - 1, selectedRow);
                dgv.ClearSelection();
                dgv.Rows[rowIndex - 1].Cells[colIndex].Selected = true;
            }
            catch { }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {/* move row up/down is a little bit tedious - only works for single rows */
            DataGridView dgv = dataGridView1;
            try
            {
                int totalRows = dgv.Rows.Count;
                // get index of the row for the selected cell
                int rowIndex = dgv.SelectedCells[0].OwningRow.Index;
                if (rowIndex == totalRows - 1)
                    return;
                // get index of the column for the selected cell
                int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[rowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(rowIndex + 1, selectedRow);
                dgv.ClearSelection();
                dgv.Rows[rowIndex + 1].Cells[colIndex].Selected = true;
            }
            catch { }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
           ChildForms.InfoForm f = new ChildForms.InfoForm();
            f.ShowDialog();
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //sort grid by filename

        }
    }

}