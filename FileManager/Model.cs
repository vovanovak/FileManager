using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;


namespace FileManager
{
    public enum InitListView
    {
        Details, Nondetails
    }
    public static class Model
    {
        public static String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        public static bool IsFilePath(string selectedItemPath)
        {
            if (!File.Exists(selectedItemPath))
            {
                return false; 
            }
            else
            {
                return true;
            }
        }

        public static void RenameFileOrFolder(string path, string newFileName)
        {
            if (IsFilePath(path))
            {
                FileInfo file = new FileInfo(path);
                DirectoryInfo dir = new DirectoryInfo(file.DirectoryName);
                file.MoveTo(newFileName);
            }
            else
            {
                DirectoryInfo dir1 = new DirectoryInfo(path);
                DirectoryInfo dir;
                if (dir1.Parent.FullName != null)
                {
                    dir = new DirectoryInfo(dir1.Parent.FullName);
                }
                else
                {
                    dir = new DirectoryInfo(dir1.Root.FullName);
                }
                dir1.MoveTo(newFileName);
            }
        }
        
        public static void OpenFile(string path)
        {
            if (!new FileInfo(path).Attributes.HasFlag(FileAttributes.Directory))
            {
                
            }
        }

        public static void GetLogicalDrives(ToolStrip toolStrip)
        {
            ToolStripComboBox comboBox = new ToolStripComboBox();
            foreach (var i in toolStrip.Items)
            {
                if (i.GetType() == typeof(ToolStripComboBox))
                {
                    comboBox = (ToolStripComboBox)i;
                    break;
                }
            }
            
            DriveInfo[] drives = DriveInfo.GetDrives();
            bool IsFirst = true;
            foreach (var i in drives)
            {
                comboBox.Items.Add(i.Name);
                toolStrip.Items.Add(i.Name, System.Drawing.Image.FromFile("../../Icons/1377209014_71860.jpg"));
                if (IsFirst)
                {
                    IsFirst = false;
                    ToolStripButton btn = (ToolStripButton)toolStrip.Items[toolStrip.Items.Count - 1];
                    btn.CheckState = CheckState.Checked;
                }
            }
            comboBox.SelectedItem = comboBox.Items[0];
        }

        public static void InitTreeViewAndListView(string path, TreeView treeView, ListView listView)
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            if (treeView.Nodes.Count == 0)
            {
                TreeNode[] children = new TreeNode[dirs.Length];

                for (int i = 0; i < dirs.Length; i++)
                {
                    DirectoryInfo currentDir = new DirectoryInfo(dirs[i]);
                    children[i] = new TreeNode(currentDir.Name);
                    children[i].ToolTipText = currentDir.FullName;
                    children[i].Name = currentDir.FullName;

                    try
                    {
                        if (Directory.GetFiles(currentDir.FullName).Length + Directory.GetDirectories(currentDir.FullName).Length > 0)
                        {
                            children[i].Nodes.Add("123");
                        }
                    }
                    catch(UnauthorizedAccessException e)
                    {

                    }
                }

                treeView.Nodes.Add(new TreeNode(path, children));
                treeView.Nodes[0].ToolTipText = path;
                treeView.Nodes[0].Name = path;

                treeView.SelectedNode = treeView.Nodes[0];
                treeView.SelectedNode.Expand();
                treeView.SelectedNode.ImageIndex = 0;

                try
                {
                    listView.Clear();
                    foreach (var i in dirs)
                    {
                        DirectoryInfo currentDir = new DirectoryInfo(i);
                        listView.Items.Add(new ListViewItem(new string[] { currentDir.Name, "Directory", "", currentDir.LastWriteTime.ToLongDateString(), currentDir.CreationTime.ToLongDateString() }));
                        listView.Items[listView.Items.Count - 1].ImageIndex = 0;
                        listView.Items[listView.Items.Count - 1].ToolTipText = currentDir.FullName;
                        listView.Items[listView.Items.Count - 1].Name = currentDir.FullName;
                    }
                    foreach (var i in files)
                    {
                        FileInfo currentFile = new FileInfo(i);
                        string extension = currentFile.Extension;
                        listView.Items.Add(new ListViewItem(new string[] { currentFile.Name, currentFile.Extension, currentFile.Length / 1000 + " KB", currentFile.LastWriteTime.ToLongDateString(), currentFile.CreationTime.ToLongDateString() }));


                        listView.Items[listView.Items.Count - 1].ImageIndex = 1;
                            
                            
                        listView.Items[listView.Items.Count - 1].ToolTipText = currentFile.FullName;
                        listView.Items[listView.Items.Count - 1].Name = currentFile.FullName;
                    }
                }
                catch
                {

                }
            }
            else
            {
                TreeNode[] children = new TreeNode[dirs.Length];

                treeView.SelectedNode.Nodes.Clear();

                for (int i = 0; i < dirs.Length; i++)
                {
                    DirectoryInfo currentDir = new DirectoryInfo(dirs[i]);
                    children[i] = new TreeNode(currentDir.Name);
                    children[i].ToolTipText = currentDir.FullName;
                    children[i].Name = currentDir.FullName;
                    try
                    {
                        if (Directory.GetFiles(currentDir.FullName).Length + Directory.GetDirectories(currentDir.FullName).Length > 0)
                        {
                            children[i].Nodes.Add("123");
                        }
                    }
                    catch(UnauthorizedAccessException e)
                    {

                    }
                }
                treeView.SelectedNode.Nodes.AddRange(children);

                listView.Items.Clear();
                try
                {
                    foreach (var i in dirs)
                    {
                        DirectoryInfo currentDir = new DirectoryInfo(i);
                        listView.Items.Add(new ListViewItem(new string[] { currentDir.Name, "Directory", "", currentDir.LastWriteTime.ToLongDateString(), currentDir.CreationTime.ToLongDateString() }));
                        listView.Items[listView.Items.Count - 1].ImageIndex = 0;
                        listView.Items[listView.Items.Count - 1].ToolTipText = currentDir.FullName;
                        listView.Items[listView.Items.Count - 1].Name = currentDir.FullName;
                    }
                    foreach (var i in files)
                    {
                        FileInfo currentFile = new FileInfo(i);
                        listView.Items.Add(new ListViewItem(new string[] { currentFile.Name, currentFile.Extension, currentFile.Length / 1000 + " KB", currentFile.LastWriteTime.ToLongDateString(), currentFile.CreationTime.ToLongDateString() }));

                        listView.Items[listView.Items.Count - 1].ImageIndex = 1;

                        listView.Items[listView.Items.Count - 1].ToolTipText = currentFile.FullName;
                        listView.Items[listView.Items.Count - 1].Name = currentFile.FullName;
                    }
                }
                catch
                {

                }
                    
            }
        }

        public static void InitListView(ListView listView, string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            listView.Clear();
            foreach (var i in dirs)
            {
                DirectoryInfo currentDir = new DirectoryInfo(i);
                listView.Items.Add(new ListViewItem(new string[] { currentDir.Name, "Directory", "", currentDir.LastWriteTime.ToLongDateString(), currentDir.CreationTime.ToLongDateString() }));
                listView.Items[listView.Items.Count - 1].ImageIndex = 0;
                listView.Items[listView.Items.Count - 1].ToolTipText = currentDir.FullName;
                listView.Items[listView.Items.Count - 1].Name = currentDir.FullName;
            }
            foreach (var i in files)
            {
                FileInfo currentFile = new FileInfo(i);
                string extension = currentFile.Extension;
                listView.Items.Add(new ListViewItem(new string[] { currentFile.Name, currentFile.Extension, BytesToString(currentFile.Length), currentFile.LastWriteTime.ToLongDateString(), currentFile.CreationTime.ToLongDateString() }));
                listView.Items[listView.Items.Count - 1].ImageIndex = 1;
                listView.Items[listView.Items.Count - 1].ToolTipText = currentFile.FullName;
                listView.Items[listView.Items.Count - 1].Name = currentFile.FullName;
            }
        }

        public static void InitListView(ListView listView, string path, InitListView type)
        {
            if (type == FileManager.InitListView.Nondetails)
            {
                InitListView(listView, path);
            }
            else
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);

                listView.Clear();
                foreach (var i in dirs)
                {
                    DirectoryInfo currentDir = new DirectoryInfo(i);
                    listView.Items.Add(new ListViewItem(new string[] { currentDir.Name, "Directory", "", currentDir.LastWriteTime.ToLongDateString(), currentDir.CreationTime.ToLongDateString() }));
                    listView.Items[listView.Items.Count - 1].ImageIndex = 0;
                    listView.Items[listView.Items.Count - 1].ToolTipText = currentDir.FullName;
                    listView.Items[listView.Items.Count - 1].Name = currentDir.FullName;
                }
                foreach (var i in files)
                {
                    FileInfo currentFile = new FileInfo(i);
                    string extension = currentFile.Extension;
                    listView.Items.Add(new ListViewItem(new string[] { currentFile.Name, currentFile.Extension, currentFile.Length / 1000 + " KB", currentFile.LastWriteTime.ToLongDateString(), currentFile.CreationTime.ToLongDateString() }));
                    listView.Items[listView.Items.Count - 1].ImageIndex = 1;
                    listView.Items[listView.Items.Count - 1].ToolTipText = currentFile.FullName;
                    listView.Items[listView.Items.Count - 1].Name = currentFile.FullName;
                }
            }
        }

        public static void InitListView(ListView listView, string path, string mask, bool withSubDirs)
        {
            Regex exp = new Regex("^" + mask + "$");
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            listView.Clear();
            foreach (var i in dirs)
            {
                DirectoryInfo currentDir = new DirectoryInfo(i);
                if (exp.IsMatch(i))
                {
                    listView.Items.Add(new ListViewItem(new string[] { currentDir.Name, "Directory", "", currentDir.LastWriteTime.ToLongDateString(), currentDir.CreationTime.ToLongDateString() }));
                    listView.Items[listView.Items.Count - 1].ImageIndex = 0;
                    listView.Items[listView.Items.Count - 1].ToolTipText = currentDir.FullName;
                    listView.Items[listView.Items.Count - 1].Name = currentDir.FullName;   
                }
                if (withSubDirs)
                {
                    if (currentDir.GetFiles().Length + currentDir.GetDirectories().Length > 0)
                    {
                        string[] found = SearchFilesAndDirs(i, mask);
                        for (int j = 0; j < found.Length; j++)
                        {
                            if (IsFilePath(found[j]))
                            {
                                FileInfo currentFile = new FileInfo(found[j]);
                                string extension = currentFile.Extension;
                                listView.Items.Add(new ListViewItem(new string[] { currentFile.Name, currentFile.Extension, currentFile.Length / 1000 + " KB", currentFile.LastWriteTime.ToLongDateString(), currentFile.CreationTime.ToLongDateString() }));
                                listView.Items[listView.Items.Count - 1].ImageIndex = 1;
                                listView.Items[listView.Items.Count - 1].ToolTipText = currentFile.FullName;
                                listView.Items[listView.Items.Count - 1].Name = currentFile.FullName;
                            }
                            else
                            {
                                DirectoryInfo currentDir1 = new DirectoryInfo(found[j]);
                                listView.Items.Add(new ListViewItem(new string[] { currentDir1.Name, "Directory", "", currentDir1.LastWriteTime.ToLongDateString(), currentDir1.CreationTime.ToLongDateString() }));
                                listView.Items[listView.Items.Count - 1].ImageIndex = 0;
                                listView.Items[listView.Items.Count - 1].ToolTipText = currentDir1.FullName;
                                listView.Items[listView.Items.Count - 1].Name = currentDir1.FullName;
                            }
                        }
                    }
                }
            }
            foreach (var i in files)
            {
                if (exp.IsMatch(i))
                {
                    FileInfo currentFile = new FileInfo(i);
                    string extension = currentFile.Extension;
                    listView.Items.Add(new ListViewItem(new string[] { currentFile.Name, currentFile.Extension, currentFile.Length / 1000 + " KB", currentFile.LastWriteTime.ToLongDateString(), currentFile.CreationTime.ToLongDateString() }));
                    listView.Items[listView.Items.Count - 1].ImageIndex = 1;
                    listView.Items[listView.Items.Count - 1].ToolTipText = currentFile.FullName;
                    listView.Items[listView.Items.Count - 1].Name = currentFile.FullName;
                }
            }
        }

        public static string[] SearchFilesAndDirs(string path, string mask)
        {
            Regex exp = new Regex("^" + mask + "$");
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            List<string> found = new List<string>();

            foreach (var i in dirs)
            {
                DirectoryInfo currentDir = new DirectoryInfo(i);
                if (exp.IsMatch(i))
                {
                    found.Add(currentDir.FullName);
                }
                if (currentDir.GetFiles().Length + currentDir.GetDirectories().Length > 0)
                {
                    found.AddRange(SearchFilesAndDirs(i, mask));
                }
            }
            foreach (var i in files)
            {
                if (exp.IsMatch(i))
                {
                    FileInfo currentFile = new FileInfo(i);
                    found.Add(currentFile.FullName);
                }
            }

            return found.ToArray();
        }

        public static string MakeThePathStringSmaller(string path)
        {
            string str = path;

            int startIndex = 0, endIndex = 0;
            bool isFirst = true;


            for (int i = 0; i < str.Length; i++)
            {
                if (str.Length < 34)
                {
                    break;
                }
                if (str[i] == '\\')
                {
                    if (isFirst)
                    {
                        startIndex = i;
                        isFirst = false;
                    }
                    else
                    {
                        endIndex = i;
                        isFirst = true;

                        str = str.Remove(startIndex, endIndex - startIndex);
                        str = str.Insert(startIndex, "\\...");
                    }
                }
            }

            if (str.Length > 32)
            {
                str = str.Replace("\\...", "\\.");
            }

            if (str.Length > 32)
            {
                str = str.Remove((str.Length - (str.Length - 32)) / 2, str.Length - 32);
                str = str.Insert(15, "...");
            }
            return str;
        }

        public static long CountSizeOfFolder(string path)
        {
            long length = 0;

            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = new FileInfo(files[i]);
                try
                {
                    length += file.Length;
                }
                catch
                {

                }
            }

            for (int i = 0; i < dirs.Length; i++)
            {
                length += CountSizeOfFolder(dirs[i]);
            }

            return length;
        }

        public static void CopyFolderFilesAndFolders(string source, string dest)
        {
            string[] dirs = Directory.GetDirectories(source);
            string[] files = Directory.GetFiles(source);

            DirectoryInfo dirInfo = new DirectoryInfo(dest);
            dirInfo.Create();
            FileInfo fi;
            for (int i = 0; i < dirs.Length; i++)
            {
                DirectoryInfo dirInfoNew = new DirectoryInfo(dest + "\\" + new DirectoryInfo(dirs[i]).Name);
                dirInfoNew.Create();

                if (Directory.GetDirectories(source + "\\" + new DirectoryInfo(dirs[i]).Name).Length != 0 || Directory.GetFiles(source + "\\" + new DirectoryInfo(dirs[i]).Name).Length != 0)
                {
                    CopyFolderFilesAndFolders(source + "\\" + new DirectoryInfo(dirs[i]).Name, dest + "\\" + new DirectoryInfo(dirs[i]).Name);
                }
            }
            for (int i = 0; i < files.Length; i++)
            {
                fi = new FileInfo(files[i]);
                File.Copy(files[i], dest + "\\" + fi.Name, true);
            }
        }

        public static void MoveFolderFilesAndFolders(string source, string dest)
        {
            string[] dirs = Directory.GetDirectories(source);
            string[] files = Directory.GetFiles(source);

            DirectoryInfo dirInfo = new DirectoryInfo(dest);
            dirInfo.Create();
            FileInfo fi;
            for (int i = 0; i < dirs.Length; i++)
            {
                DirectoryInfo dirInfoNew = new DirectoryInfo(dest + "\\" + new DirectoryInfo(dirs[i]).Name);
                dirInfoNew.Create();

                if (Directory.GetDirectories(source + "\\" + new DirectoryInfo(dirs[i]).Name).Length != 0 || Directory.GetFiles(source + "\\" + new DirectoryInfo(dirs[i]).Name).Length != 0)
                {
                    MoveFolderFilesAndFolders(source + "\\" + new DirectoryInfo(dirs[i]).Name, dest + "\\" + new DirectoryInfo(dirs[i]).Name);
                }
            }
            for (int i = 0; i < files.Length; i++)
            {
                fi = new FileInfo(files[i]);
                File.Move(files[i], dest + "\\" + fi.Name);
            }
            new DirectoryInfo(source).Delete(true);
        }

        public static void PasteFolderAndFiles(string source, string dest)
        {
            try
            {
                if (source != "")
                {
                    if (source.Last() == '$')
                    {
                        source = source.Remove(source.Count() - 1);
                        if (IsFilePath(source))
                        {
                            //source = source.Remove(source.Length - 1, 1);
                            File.Move(source, dest + "\\" + new FileInfo(source).Name);
                        }
                        else
                        {
                            //source = source.Remove(source.Length - 1, 1);
                            Model.MoveFolderFilesAndFolders(source, dest + "\\" + new DirectoryInfo(source).Name);
                        }
                    }
                    if (source.Last() == '%')
                    {
                        source = source.Remove(source.Count() - 1);
                        if (IsFilePath(source))
                        {
                            //source = source.Remove(source.Length - 1, 1);
                            File.Copy(source, dest + "\\" + new FileInfo(source).Name);
                        }
                        else
                        {
                            //source = source.Remove(source.Length - 1, 1);
                            Model.CopyFolderFilesAndFolders(source, dest + "\\" + new DirectoryInfo(source).Name);
                        }


                    }
                }
            }
            catch
            {

            }
        }

        public static void DeleteFileOrFolder(string path)
        {
            if (IsFilePath(path))
            {
                File.Delete(path);
            }
            else
            {
                DirectoryInfo dir1 = new DirectoryInfo(path);
                if (dir1.GetDirectories() == null && dir1.GetFiles() == null)
                {
                    dir1.Delete();
                }
                else
                {
                    dir1.Delete(true);
                }

            }
        }

        public static void EncryptFileOrFolder(string path)
        {
            if (Model.IsFilePath(path))
            {
                File.Encrypt(path);
            }
            else
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);

                for (int i = 0; i < files.Length; i++)
                {
                    File.Encrypt(files[i]);
                }

                for (int i = 0; i < dirs.Length; i++)
                {
                    if (Directory.GetFiles(dirs[i]).Length + Directory.GetDirectories(dirs[i]).Length > 0)
                    {
                        EncryptFileOrFolder(dirs[i]);
                    }
                }
            }
        }

        public static void DecryptFileOrFolder(string path)
        {
            if (Model.IsFilePath(path))
            {
                File.Decrypt(path);
            }
            else
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);

                for (int i = 0; i < files.Length; i++)
                {
                    File.Decrypt(files[i]);
                }

                for (int i = 0; i < dirs.Length; i++)
                {
                    if (Directory.GetFiles(dirs[i]).Length + Directory.GetDirectories(dirs[i]).Length > 0)
                    {
                        DecryptFileOrFolder(dirs[i]);
                    }
                }
            }
        }
    }
}
