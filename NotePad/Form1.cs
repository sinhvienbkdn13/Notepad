using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotePad
{
    public partial class MiniNotepad : Form
    {
        
        #region declare
        int d;
        public static Boolean matchcase;
        public static string FindText;
        public static string ReplaceText;
        private bool isFileAlreadySaved;
        private bool isFileDirty;
        private string currOpenFileName;
        private FontDialog fontDialog = new FontDialog();
        #endregion
        public MiniNotepad()
        {
            InitializeComponent();
        }
        // Save Setting when exit
        private void SaveSetting()
        {
            Properties.Settings.Default.Location = this.Location;
            Properties.Settings.Default.Height = this.Height;
            Properties.Settings.Default.Width = this.Width;
            Properties.Settings.Default.Font = MainRichTextBox.Font;
            Properties.Settings.Default.WordWrap = MainRichTextBox.WordWrap;
            Properties.Settings.Default.Save();
        }
        //Load previuos setting
        private void LoadSetting()
        {
            this.Location = Properties.Settings.Default.Location;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;
            MainRichTextBox.Font = Properties.Settings.Default.Font;
            MainRichTextBox.WordWrap = Properties.Settings.Default.WordWrap;
            wordWrapToolStripMenuItem.Checked = MainRichTextBox.WordWrap;
            
        }

        private void MiniNotepad_Load(object sender, EventArgs e)
        {
            
            isFileAlreadySaved = false;
            isFileDirty = false;
            currOpenFileName = "";
            LoadSetting();
            //Load size chu
            FontandSize();
            //Load setting truoc do
        }

        private void FontandSize()
        {
            for (int i = 1; i < 120; i++)
            {
                comboSize.Items.Add(i);
            }
            InstalledFontCollection listFont = new InstalledFontCollection();
            foreach (FontFamily font in listFont.Families)
            {
                comboFont.Items.Add(font.Name);
            }
        }
        //Save Setting when close
        private void MiniNotepad_FormClosed(object sender, FormClosedEventArgs e)
        {
            ExitFile();
        }
        //Load Font 
        private void comboFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Xu ly text trong richbox khi font thay doi
            LoadFontandSize();
            
        }

        private void LoadFontandSize()
        {
            FontFamily fa = new FontFamily(comboFont.Text);
            Font f = new Font(fa, float.Parse(comboSize.Text));
            if (MainRichTextBox.SelectedText.Length > 0)
                MainRichTextBox.SelectionFont = f;
        }

        private void comboSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFontandSize();
        }
        //About
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About();
        }

        private static void About()
        {
            MessageBox.Show("Written By Tommy", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //Button About
        private void toolStripButton27_Click(object sender, EventArgs e)
        {
            About();
        }
        //Create New file
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void NewFile()
        {
            if(isFileDirty)
            {
                DialogResult result = MessageBox.Show("Do you want to save your change ?", "File Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                switch(result)
                {
                    case DialogResult.Yes:
                        SaveAsFile();
                        break;
                    case DialogResult.No:
                        break;
                }
            }
                ClearScreen();
                isFileAlreadySaved = false;
                currOpenFileName = "";
                EnableDisableUndoRedoFile(false);
        }
        //Save File
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void SaveFile()
        {
            if (isFileAlreadySaved)
            {
                if (Path.GetExtension(currOpenFileName) == ".rtf")
                    MainRichTextBox.SaveFile(currOpenFileName, RichTextBoxStreamType.RichText);
                if (Path.GetExtension(currOpenFileName) == ".txt")
                    MainRichTextBox.SaveFile(currOpenFileName, RichTextBoxStreamType.PlainText);
                isFileDirty = false;
            }
            else
            {
                if (isFileDirty)
                {
                    SaveAsFile();
                }
                else
                {
                    ClearScreen();
                }
            }
        }
        //Xoa man hinh va mac dinh thong so
        private void ClearScreen()
        {
            MainRichTextBox.Clear();
            this.Text = "Untitled - NotePad";
            isFileDirty = false;
        }
        // Save As File
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsFile();
        }

        private void SaveAsFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|Rich Text Format (*.rtf)|*.rtf";
            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (Path.GetExtension(saveFileDialog.FileName) == ".txt")
                    MainRichTextBox.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
                if (Path.GetExtension(saveFileDialog.FileName) == ".rtf")
                    MainRichTextBox.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.RichText);
                this.Text = Path.GetFileName(saveFileDialog.FileName) + " - Untitled NotePad";

                isFileAlreadySaved = true;
                isFileDirty = false;
                currOpenFileName = saveFileDialog.FileName;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void OpenFile()
        {
            //File da luu chua.. Neu chua thi luu lai
            if (isFileDirty)
            {
                DialogResult result1 = MessageBox.Show("Do you want to save your change ?", "File Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                switch (result1)
                {
                    case DialogResult.Yes:
                        SaveAsFile();
                        break;
                    case DialogResult.No:
                        break;
                }
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|Rich Text Format (*.rtf)|*.rtf";

            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (Path.GetExtension(openFileDialog.FileName) == ".txt")
                    MainRichTextBox.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.PlainText);
                if (Path.GetExtension(openFileDialog.FileName) == ".rtf")
                    MainRichTextBox.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.RichText);
            }
            this.Text = Path.GetFileName(openFileDialog.FileName) + " - Untitled NotePad";
            isFileAlreadySaved = true;
            isFileDirty = false;
            currOpenFileName = openFileDialog.FileName;

            EnableDisableUndoRedoFile(false);       
        }

        private void EnableDisableUndoRedoFile(bool enable)
        {
            undoToolStripMenuItem.Enabled = enable;
            redoToolStripMenuItem.Enabled = enable;
            undoToolStripMenuItem1.Enabled = enable;
            redoToolStripMenuItem1.Enabled = enable;
            UndoToolStripButton16.Enabled = enable;
            RedoToolStripButton17.Enabled = enable;
            
        }

        private void MainRichTextBox_TextChanged(object sender, EventArgs e)
        {
            isFileDirty = true;
            undoToolStripMenuItem.Enabled = true;
            UndoToolStripButton16.Enabled = true;
            undoToolStripMenuItem1.Enabled = true;
            if (MainRichTextBox.Text.Length > 0)
            {
                cutToolStripMenuItem.Enabled = true;
                cutToolStripMenuItem1.Enabled = true;
                CuttoolStripButton7.Enabled = true;
                copyToolStripMenuItem.Enabled = true;
                copyToolStripMenuItem1.Enabled = true;
                CopytoolStripButton8.Enabled = true;
                selectAllToolStripMenuItem.Enabled = true;
            }
            else
            {
                cutToolStripMenuItem.Enabled = false;
                cutToolStripMenuItem1.Enabled = false;
                CuttoolStripButton7.Enabled = false;
                copyToolStripMenuItem.Enabled = false;
                copyToolStripMenuItem1.Enabled = false;
                CopytoolStripButton8.Enabled = false;
                selectAllToolStripMenuItem.Enabled = false;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitFile();
        }

        private void ExitFile()
        {
            // File da luu chua. Neu chua thi luu lai, neu luu roi thi thoat
            if (isFileDirty)
            {
                DialogResult result1 = MessageBox.Show("Do you want to save your change ?", "File Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                switch (result1)
                {
                    case DialogResult.Yes:
                        SaveAsFile();
                        break;
                    case DialogResult.No:
                        break;
                }
            }
            SaveSetting();
            Application.Exit();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            NewFile();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
           
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SaveAsFile();
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintPreviewFile();

        }

        private void PrintPreviewFile()
        {
            DVPrintPreviewDialog.Document = DVPrintDocument;
            DVPrintPreviewDialog.ShowDialog();
        }

        private void DVPrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString( MainRichTextBox.Text, new Font("Arial", 45, FontStyle.Regular), Brushes.Black, new Point(0, 0));
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintFile();
        }

        private void PrintFile()
        {
            DVPrintDocument.Print();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            PrintPreviewFile();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            PrintFile();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainRichTextBox.Undo();
            redoToolStripMenuItem.Enabled = true;
            undoToolStripMenuItem.Enabled = false;
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainRichTextBox.Redo();
            redoToolStripMenuItem.Enabled = false;
            undoToolStripMenuItem.Enabled = true;
        }

        private void undoToolStripButton16_Click(object sender, EventArgs e)
        {
            MainRichTextBox.Undo();
            RedoToolStripButton17.Enabled = true;
            UndoToolStripButton16.Enabled = false;
        }

        private void RedoToolStripButton17_Click(object sender, EventArgs e)
        {
            MainRichTextBox.Redo();
            RedoToolStripButton17.Enabled = false;
            UndoToolStripButton16.Enabled = true;
        }

        private void undoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MainRichTextBox.Undo();
            redoToolStripMenuItem1.Enabled = true;
            undoToolStripMenuItem1.Enabled = false;
        }

        private void redoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MainRichTextBox.Redo();
            redoToolStripMenuItem1.Enabled = false;
            undoToolStripMenuItem1.Enabled = true;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainRichTextBox.SelectAll();
        }

        private void timeDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTimeMenu();
        }

        private void DateTimeMenu()
        {
            MainRichTextBox.SelectedText = DateTime.Now.ToString();
        }

        private void FormatText(FontStyle fontStyle)
        {
            MainRichTextBox.SelectionFont = new Font(MainRichTextBox.Font, fontStyle);
        }

        private void boldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormatText(FontStyle.Bold);
        }

        private void italicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormatText(FontStyle.Italic);
        }

        private void underlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormatText(FontStyle.Underline);
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormatText(FontStyle.Regular);
        }

        private void strikethroughToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormatText(FontStyle.Strikeout);
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontText();
        }

        private void FontText()
        {
            fontDialog.ShowColor = true;
            fontDialog.ShowApply = true;
            fontDialog.Apply += new System.EventHandler(fontDialog_Apply);
            fontDialog.Font = MainRichTextBox.Font;
            DialogResult result = fontDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                MainRichTextBox.Font = fontDialog.Font;
                if (MainRichTextBox.SelectionLength > 0)
                {
                    MainRichTextBox.SelectionFont = fontDialog.Font;
                    MainRichTextBox.SelectionColor = fontDialog.Color;
                }
            }
        }

        private void fontDialog_Apply(object sender , EventArgs e)
        {
            if (MainRichTextBox.SelectionLength > 0)
            {
                MainRichTextBox.SelectionFont = fontDialog.Font;
                MainRichTextBox.SelectionColor = fontDialog.Color;
            }
        }

        private void textColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextColor();
        }

        private void TextColor()
        {
            ColorDialog textcolor = new ColorDialog();
            textcolor.ShowDialog();
            MainRichTextBox.SelectionColor = textcolor.Color;
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutMenu();
        }

        private void CutMenu()
        {
            MainRichTextBox.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyMenu();
        }

        private void CopyMenu()
        {
            MainRichTextBox.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteMenu();
        }

        private void PasteMenu()
        {
            MainRichTextBox.Paste();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            CutMenu();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            CopyMenu();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            PasteMenu();
        }

        private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CutMenu();
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopyMenu();
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PasteMenu();
        }

        private void alignLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AlignLeft();
        }

        private void AlignLeft()
        {
            MainRichTextBox.SelectionAlignment = HorizontalAlignment.Left;
        }

        private void alignCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AlignCenter();
        }

        private void AlignCenter()
        {
            MainRichTextBox.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void alignRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AlignRight();
        }

        private void AlignRight()
        {
            MainRichTextBox.SelectionAlignment = HorizontalAlignment.Right;
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            AlignLeft();
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            AlignCenter();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            AlignRight();
        }

        private void alignLeftToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AlignLeft();
        }

        private void alignCenterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AlignCenter();
        }

        private void alignRightToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AlignRight();
        }

        private void fontColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontColor();
        }

        private void FontColor()
        {
            ColorDialog fontcolor = new ColorDialog();
            fontcolor.ShowDialog();
            MainRichTextBox.ForeColor = fontcolor.Color;
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundColor();
        }

        private void BackgroundColor()
        {
            ColorDialog backcolor = new ColorDialog();
            backcolor.ShowDialog();
            MainRichTextBox.BackColor = backcolor.Color;
        }

        private void backgroundTextColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundText();
        }

        private void BackgroundText()
        {
            ColorDialog backtext = new ColorDialog();
            backtext.ShowDialog();
            MainRichTextBox.SelectionBackColor = backtext.Color;
        }

        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            FontText();
        }

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(wordWrapToolStripMenuItem.Checked)
            {
                MainRichTextBox.WordWrap = true;
            }
            else
            {
                MainRichTextBox.WordWrap = false;
            }
        }

        private void toolStripButton22_Click(object sender, EventArgs e)
        {
            BackgroundColor();
        }

        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            FontColor();
        }

        private void toolStripButton20_Click(object sender, EventArgs e)
        {
            TextColor();
        }

        private void toolStripButton23_Click(object sender, EventArgs e)
        {
            BackgroundText();
        }

        private void toolStripButton26_Click(object sender, EventArgs e)
        {
            DateTimeMenu();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            FormatText(FontStyle.Bold);
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            FormatText(FontStyle.Italic);
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            FormatText(FontStyle.Underline);
        }

        private void toolStripButton21_Click(object sender, EventArgs e)
        {
            FormatText(FontStyle.Strikeout);
        }

        private void boldToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormatText(FontStyle.Bold);
        }

        private void italicToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormatText(FontStyle.Italic);
        }

        private void underlineToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormatText(FontStyle.Underline);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainRichTextBox.SelectedText = "";
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Find find = new Find();
            find.ShowDialog();
            if(FindText != "")
            {
                 d=MainRichTextBox.Find(FindText);
            }
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FindText != "")
            {
                if(matchcase)
                {
                    d = MainRichTextBox.Find(FindText, (d + 1), MainRichTextBox.Text.Length, RichTextBoxFinds.MatchCase) ;
                }
                else
                {
                    d = MainRichTextBox.Find(FindText, (d + 1), MainRichTextBox.Text.Length, RichTextBoxFinds.None);
                }
                
            }
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Replace replace = new Replace();
            replace.ShowDialog();
            d=MainRichTextBox.Find(FindText);
            MainRichTextBox.SelectedText = ReplaceText;
        }

        private void comboSize_Click(object sender, EventArgs e)
        {

        }

        private void picturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertImage();
        }

        private void insertImage()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = " JPEG File Interchange Format (*.jpg)|*.jpg|Portable Network Graphics(*.png)|*.png|Graphics Interchange Format(*.gif)|*.gif";
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                Image img = Image.FromFile(dlg.FileName);
                Clipboard.SetDataObject(img);
                DataFormats.Format df;
                df = DataFormats.GetFormat(DataFormats.Bitmap);

                if (this.MainRichTextBox.CanPaste(df))
                    MainRichTextBox.Paste(df);
            }
        }

        private void bulletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertbullet();
        }

        private void insertbullet()
        {
            if (MainRichTextBox.SelectedText.Length > 0)
                MainRichTextBox.SelectionIndent = 15;
            MainRichTextBox.BulletIndent = 5;
            MainRichTextBox.SelectionBullet = true;
        }

        private void comboFont_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton24_Click(object sender, EventArgs e)
        {
            insertImage();
        }

        private void toolStripButton25_Click(object sender, EventArgs e)
        {
            insertbullet();
        }
    }
}
