using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    public partial class MainForm : Form
    {
        public string[] pathArray;
        public int SelectedIndex { get; set; }
        public ListView[] listViewArray;
        public TreeView[] treeViewArray;
        public string buffer;
        public string selectedItemPath;
        public bool searchWithSubdirs;
        public string newFolder;

        public MainForm()
        {
            InitializeComponent();

            SelectedIndex = 0;
            listViewArray = new ListView[1];
            treeViewArray = new TreeView[1];
            pathArray = new string[1];

            Model.GetLogicalDrives(toolStripDrives);
           
            
            pathArray[0] = @"C:\";

            tabControl1.TabPages[0].Text = @"C:\";
            tabControl1.Dock = DockStyle.Fill;

            tabControl1.TabPages[0].Width = this.Width;

            InitTabPage();
           
            Model.InitTreeViewAndListView(pathArray[SelectedIndex], treeViewArray[SelectedIndex], listViewArray[SelectedIndex]);

            searchWithSubdirs = true;
        }

        private void InitTabPage()
        {
            if (SelectedIndex >= listViewArray.Length)
            {
                Array.Resize(ref listViewArray, listViewArray.Length + 1);
                Array.Resize(ref treeViewArray, treeViewArray.Length + 1);
                Array.Resize(ref pathArray, pathArray.Length + 1);
            }
            
            listViewArray[SelectedIndex] = new ListView();

            listViewArray[SelectedIndex].Size = new System.Drawing.Size(755, 495);
            listViewArray[SelectedIndex].Location = new Point(304, 0);

            treeViewArray[SelectedIndex] = new TreeView();

            treeViewArray[SelectedIndex].Size = new System.Drawing.Size(251, 495);
            treeViewArray[SelectedIndex].Location = new Point(0, 0);

            listViewArray[SelectedIndex].LargeImageList = new ImageList();
            listViewArray[SelectedIndex].SmallImageList = new ImageList();
            listViewArray[SelectedIndex].LargeImageList.ImageSize = new System.Drawing.Size(32, 32);
            listViewArray[SelectedIndex].SmallImageList.ImageSize = new System.Drawing.Size(16, 16);

            listViewArray[SelectedIndex].LargeImageList.Images.Add(new System.Drawing.Icon("../../Icons/Folder.ico"));
            listViewArray[SelectedIndex].SmallImageList.Images.Add(new System.Drawing.Icon("../../Icons/Folder.ico"));
            listViewArray[SelectedIndex].LargeImageList.Images.Add(new System.Drawing.Icon("../../Icons/File.ico"));
            listViewArray[SelectedIndex].SmallImageList.Images.Add(new System.Drawing.Icon("../../Icons/File.ico"));
            listViewArray[SelectedIndex].Scrollable = true;

            for (int i = 0; i < System.IO.Directory.GetFiles("../../Icons/ext").Length; i++)
            {
                listViewArray[SelectedIndex].SmallImageList.Images.Add(System.Drawing.Image.FromFile(System.IO.Directory.GetFiles("../../Icons/ext")[i]));
                listViewArray[SelectedIndex].SmallImageList.Images.Keys.Add(new System.IO.DirectoryInfo(System.IO.Directory.GetFiles("../../Icons/ext")[i]).Extension);
                listViewArray[SelectedIndex].LargeImageList.Images.Add(System.Drawing.Image.FromFile(System.IO.Directory.GetFiles("../../Icons/ext")[i]));
                listViewArray[SelectedIndex].LargeImageList.Images.Keys.Add(new System.IO.DirectoryInfo(System.IO.Directory.GetFiles("../../Icons/ext")[i]).Extension);
            }

            treeViewArray[SelectedIndex].ImageList = new ImageList();
            treeViewArray[SelectedIndex].ImageList.Images.Add(System.Drawing.Image.FromFile("../../Icons/OpenedFolder.ico"));
            treeViewArray[SelectedIndex].ImageList.Images.Add(System.Drawing.Image.FromFile("../../Icons/NotOpenedFolder.ico"));
            
            treeViewArray[SelectedIndex].ImageIndex = 0;

            treeViewArray[SelectedIndex].BeforeExpand += treeView_BeforeExpand;
            treeViewArray[SelectedIndex].AfterSelect += treeView_AfterSelect;

            listViewArray[SelectedIndex].SelectedIndexChanged += listView_SelectedIndexChanged;
            listViewArray[SelectedIndex].DoubleClick += listViewArray_DoubleClick;
            listViewArray[SelectedIndex].AfterLabelEdit += listView_AfterLabelEdit;
            listViewArray[SelectedIndex].Width = this.Width - listViewArray[SelectedIndex].Left;
            listViewArray[SelectedIndex].Height = this.Height - toolStripActions.Height - toolStripDrives.Height - menuStrip.Height - statusStrip.Height - 65;
            treeViewArray[SelectedIndex].Dock = DockStyle.Left;
            
            tabControl1.TabPages[SelectedIndex].Controls.Clear();
            tabControl1.TabPages[SelectedIndex].Controls.Add((Control)treeViewArray[SelectedIndex]);
            tabControl1.TabPages[SelectedIndex].Controls.Add((Control)listViewArray[SelectedIndex]);
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
        }

        void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolStripComboBoxDisks.SelectedItem = toolStripComboBoxDisks.Items.OfType<string>().Where(i => i == tabControl1.SelectedTab.Text).ElementAt(0);
        }

        void listView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (listViewArray[SelectedIndex].Items.ContainsKey(e.Label) || string.IsNullOrEmpty(e.Label))
            {
                e.CancelEdit = true;
            }
            else
            {
                Model.RenameFileOrFolder(listViewArray[SelectedIndex].Items[e.Item].Name, e.Label);

                if (listViewArray[SelectedIndex].FocusedItem.Focused)
                {
                    if (!(selectedItemPath.IndexOf(".", StringComparison.InvariantCulture) <= selectedItemPath.Length - 3 && selectedItemPath.IndexOf(".", StringComparison.InvariantCulture) >= selectedItemPath.Length - 6))
                    {
                        Model.InitTreeViewAndListView(pathArray[SelectedIndex], treeViewArray[SelectedIndex], listViewArray[SelectedIndex]);
                    }
                    else
                    {
                        Model.InitListView(listViewArray[SelectedIndex], pathArray[SelectedIndex]);
                    }
                }
                
                else
                {

                }
                
                
            }
        }

        void listViewArray_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = (sender as ListView).SelectedItems[0];

            if (Model.IsFilePath(selectedItemPath))
            {
                selectedItemPath = item.ToolTipText;
                System.Diagnostics.Process.Start(selectedItemPath);
            }
            else
            {
                pathArray[SelectedIndex] = item.ToolTipText;
                selectedItemPath = item.ToolTipText;
                pathTextBox.Text = pathArray[SelectedIndex];

                treeViewArray[SelectedIndex].SelectedNode = treeViewArray[SelectedIndex].Nodes.Find(item.ToolTipText, true)[0];
                Model.InitTreeViewAndListView(pathArray[SelectedIndex], treeViewArray[SelectedIndex], listViewArray[SelectedIndex]);
            }
        }

        void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewArray[SelectedIndex].SelectedItems.Count > 0)
            {
                selectedItemPath = listViewArray[SelectedIndex].SelectedItems[0].ToolTipText;
            }
        }

        private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            pathArray[SelectedIndex] = e.Node.ToolTipText;
            treeViewArray[SelectedIndex].SelectedNode = e.Node;
            Model.InitTreeViewAndListView(pathArray[SelectedIndex], treeViewArray[SelectedIndex], listViewArray[SelectedIndex]);
            e.Node.ImageIndex = 1;
        }

        private void toolStripComboBoxDisks_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox comboBox = sender as ToolStripComboBox;

            string driveName = comboBox.SelectedItem as string;
            int index = -1;
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                if (tabControl1.TabPages[i].Text == driveName)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                tabControl1.TabPages.Add(driveName);
                tabControl1.TabPages[tabControl1.TabPages.Count - 1].Focus();

                tabControl1.SelectedIndex = tabControl1.TabPages.Count - 1;
                if (SelectedIndex >= listViewArray.Length)
                {
                    Array.Resize(ref listViewArray, listViewArray.Length + 1);
                    Array.Resize(ref treeViewArray, treeViewArray.Length + 1);
                    Array.Resize(ref pathArray, pathArray.Length + 1);
                }
                pathArray[SelectedIndex] = driveName;
                pathTextBox.Text = pathArray[SelectedIndex];
                InitTabPage();
                ToolStripItemCollection items = toolStripDrives.Items;

                foreach (var i in items)
                {
                    if (i.GetType() == typeof(ToolStripButton))
                    {
                        ToolStripButton button = i as ToolStripButton;

                        if (tabControl1.TabPages[SelectedIndex].Text == button.Text)
                        {
                            button.CheckState = CheckState.Checked;
                        }
                    }
                }
                Model.InitTreeViewAndListView(pathArray[SelectedIndex], treeViewArray[SelectedIndex], listViewArray[SelectedIndex]);
            }
            else
            {
                SelectedIndex = index;
                tabControl1.SelectTab(index);
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                pathArray[SelectedIndex] = e.Node.ToolTipText;
                pathTextBox.Text = pathArray[SelectedIndex];
                selectedItemPath = e.Node.ToolTipText;
                treeViewArray[SelectedIndex].SelectedNode = e.Node;
                Model.InitTreeViewAndListView(pathArray[SelectedIndex], treeViewArray[SelectedIndex], listViewArray[SelectedIndex]);
                treeViewArray[SelectedIndex].SelectedNode.Expand();
            }
            catch(NullReferenceException e1)
            {
                pathArray[SelectedIndex] = newFolder;
                pathTextBox.Text = newFolder;
                treeViewArray[SelectedIndex].SelectedNode.Nodes.Add(new TreeNode() { Text = new System.IO.DirectoryInfo(newFolder).Name, ToolTipText = new System.IO.DirectoryInfo(newFolder).FullName });
                Model.InitTreeViewAndListView(pathArray[SelectedIndex], treeViewArray[SelectedIndex], listViewArray[SelectedIndex]);
                treeViewArray[SelectedIndex].SelectedNode.Expand();
            }
        }
        
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedIndex = tabControl1.SelectedIndex;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            Model.OpenFile(listViewArray[SelectedIndex].SelectedItems[0].ToolTipText);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem item = (sender as ListView).SelectedItems[0];

                if (Model.IsFilePath(selectedItemPath))
                {
                    selectedItemPath = item.ToolTipText;
                    System.Diagnostics.Process.Start(selectedItemPath);
                }
                else
                {
                    pathArray[SelectedIndex] = item.ToolTipText;
                    pathTextBox.Text = pathArray[SelectedIndex];
                    selectedItemPath = item.ToolTipText;

                    treeViewArray[SelectedIndex].SelectedNode = treeViewArray[SelectedIndex].Nodes.Find(item.ToolTipText, true)[0];
                    Model.InitTreeViewAndListView(pathArray[SelectedIndex], treeViewArray[SelectedIndex], listViewArray[SelectedIndex]);
                }
            }
            catch
            {

            }
        }
    
        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (treeViewArray[SelectedIndex].Focused)
            {
                buffer = treeViewArray[SelectedIndex].SelectedNode.ToolTipText + "%";
            }
            else
                if (listViewArray[SelectedIndex].Focused)
                {
                    buffer = listViewArray[SelectedIndex].SelectedItems[0].ToolTipText + "%";
                }
                else
                {

                }
        }

        private void CutButton_Click(object sender, EventArgs e)
        {
            if (listViewArray[SelectedIndex].Focused)
            {
                buffer = listViewArray[SelectedIndex].SelectedItems[0].ToolTipText + "$";
            }
            else
                if (treeViewArray[SelectedIndex].Focused)
                {
                    buffer = treeViewArray[SelectedIndex].SelectedNode.ToolTipText + "$";
                }
                else
                {

                }
        }

        private void PasteButton_Click(object sender, EventArgs e)
        {
            try
            {
                Model.PasteFolderAndFiles(buffer, pathArray[SelectedIndex]);
                Model.InitTreeViewAndListView(pathArray[SelectedIndex], treeViewArray[SelectedIndex], listViewArray[SelectedIndex]);
            }
            catch
            {

            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    Model.DeleteFileOrFolder(selectedItemPath);
                    try
                    {
                        listViewArray[SelectedIndex].SelectedItems[0].Remove();
                    }
                    catch
                    {

                    }
                }
                catch
                {

                }
                if (!(selectedItemPath.IndexOf(".", StringComparison.InvariantCulture) <= selectedItemPath.Length - 3 && selectedItemPath.IndexOf(".", StringComparison.InvariantCulture) >= selectedItemPath.Length - 6))
                {
                    treeViewArray[SelectedIndex].Nodes.Find(selectedItemPath, true)[0].Remove();
                    treeViewArray[SelectedIndex].Refresh();
                }
                else
                {

                }

                
            }
            catch
            {

            }
        }

        private void RenameButton_Click(object sender, EventArgs e)
        {
            NewFileOrFolder dlg = new NewFileOrFolder(TypeOfDialog.Rename);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    if (listViewArray[SelectedIndex].Focused)
                    {
                        Model.RenameFileOrFolder(listViewArray[SelectedIndex].SelectedItems[0].ToolTipText, listViewArray[SelectedIndex].SelectedItems[0].ToolTipText.Remove(listViewArray[SelectedIndex].SelectedItems[0].ToolTipText.LastIndexOf('\\')) + "\\" + dlg.nameOfNewFileOrFolder);
                        listViewArray[SelectedIndex].SelectedItems[0].ToolTipText = listViewArray[SelectedIndex].SelectedItems[0].ToolTipText.Remove(listViewArray[SelectedIndex].SelectedItems[0].ToolTipText.LastIndexOf('\\')) + "\\" + dlg.nameOfNewFileOrFolder;
                        listViewArray[SelectedIndex].SelectedItems[0].Text = dlg.nameOfNewFileOrFolder;
                    }
                    else
                    {
                        Model.RenameFileOrFolder(treeViewArray[SelectedIndex].SelectedNode.ToolTipText, treeViewArray[SelectedIndex].SelectedNode.ToolTipText.Remove(treeViewArray[SelectedIndex].SelectedNode.ToolTipText.LastIndexOf('\\')) + "\\" + dlg.nameOfNewFileOrFolder);
                        treeViewArray[SelectedIndex].SelectedNode.ToolTipText = treeViewArray[SelectedIndex].SelectedNode.ToolTipText.Remove(treeViewArray[SelectedIndex].SelectedNode.ToolTipText.LastIndexOf('\\')) + "\\" + dlg.nameOfNewFileOrFolder;
                        treeViewArray[SelectedIndex].SelectedNode.Text = dlg.nameOfNewFileOrFolder;
                        pathTextBox.Text = treeViewArray[SelectedIndex].SelectedNode.ToolTipText;
                    }
                    Model.InitTreeViewAndListView(pathArray[SelectedIndex], treeViewArray[SelectedIndex], listViewArray[SelectedIndex]);
                }
                catch
                {

                }
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            Model.InitListView(listViewArray[SelectedIndex], pathArray[SelectedIndex], searchTextBox.Text, searchWithSubdirs);
        }

        private void largeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewArray[SelectedIndex].View = View.LargeIcon;
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewArray[SelectedIndex].View = View.Tile;
        }

        private void smallIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewArray[SelectedIndex].View = View.SmallIcon;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewArray[SelectedIndex].View = View.List;
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewArray[SelectedIndex].View = View.Details;

            if (listViewArray[SelectedIndex].Columns.Count <= 0)
            {
                listViewArray[SelectedIndex].Columns.Add("Name");
                listViewArray[SelectedIndex].Columns[0].Width = 300;
                listViewArray[SelectedIndex].Columns.Add("Extension");
                listViewArray[SelectedIndex].Columns.Add("Size");
                listViewArray[SelectedIndex].Columns[2].Width = 100;
                listViewArray[SelectedIndex].Columns.Add("Last change date");
                listViewArray[SelectedIndex].Columns[3].Width = 120;
                listViewArray[SelectedIndex].Columns.Add("Creation date");
                listViewArray[SelectedIndex].Columns[4].Width = 120;
            }
        }

        private void FindIndexOfTreeViewItemByString(string str, TreeView view)
        {

        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            NewFileOrFolder dialog = new NewFileOrFolder(TypeOfDialog.Add);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string newName = dialog.nameOfNewFileOrFolder;

                try
                {
                    if (newName.Contains('.'))
                    {
                        System.IO.File.Create(selectedItemPath + "\\" + newName).Close();
                        
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(selectedItemPath + "\\" + newName);
                        newFolder = selectedItemPath + "\\" + newName;
                    }
                }
                catch
                {
                    
                }
               
                Model.InitTreeViewAndListView(pathArray[SelectedIndex], treeViewArray[SelectedIndex], listViewArray[SelectedIndex]);
                
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            listViewArray[SelectedIndex].Width = this.Width - treeViewArray[SelectedIndex].Width;
            listViewArray[SelectedIndex].Height = treeViewArray[SelectedIndex].Height;
        }

        private void newFileOrFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewButton_Click(null, null);
        }

        private void renameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RenameButton_Click(null, null);
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopyButton_Click(null, null);
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PasteButton_Click(null, null);
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CutButton_Click(null, null);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteButton_Click(null, null);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox box = new AboutBox();
            box.ShowDialog();
        }

        private void toolStripLabelWithSubdirectories_DoubleClick(object sender, EventArgs e)
        {
            if (toolStripLabelWithSubdirectories.Text == "With subdirectories")
            {
                searchWithSubdirs = false;
                toolStripLabelWithSubdirectories.Text = "Without subdirectories";
            }
            else
            {
                searchWithSubdirs = true;
                toolStripLabelWithSubdirectories.Text = "With subdirectories";
            }
        }
    }
}
