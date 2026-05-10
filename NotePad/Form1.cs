using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NotePad.Models;
using NotePad.Services;

namespace NotePad
{
    public partial class Form1 : Form
    {
        private List<Note> _notes;
        private readonly NoteService _service;
        private int _selectedNoteIndex = -1;

        private ListBox _lstNotes;
        private TextBox _txtTitle;
        private RichTextBox _txtContent;
        private MenuStrip _menuStrip;

        public Form1()
        {
            _service = new NoteService();
            _notes = _service.LoadNotes();
            SetupInterface();
            RefreshList();
        }

        private void SetupInterface()
        {
            // Main Window Settings
            this.Text = "NotePad"; // Name simplified as requested
            this.Size = new Size(1000, 650);
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.MainMenuStrip = _menuStrip;

            // 1. MenuStrip (Top Navigation)
            _menuStrip = new MenuStrip { BackColor = Color.FromArgb(45, 45, 48), ForeColor = Color.Gainsboro };
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File") { ForeColor = Color.Gainsboro };

            var newMenu = new ToolStripMenuItem("New Note", null, (s, e) => ResetEditor());
            var saveMenu = new ToolStripMenuItem("Save Changes", null, BtnSave_Click);
            var deleteMenu = new ToolStripMenuItem("Delete Note", null, BtnDelete_Click) { ForeColor = Color.IndianRed };
            var exitMenu = new ToolStripMenuItem("Exit", null, (s, e) => Application.Exit());

            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { newMenu, saveMenu, deleteMenu, new ToolStripSeparator(), exitMenu });
            _menuStrip.Items.Add(fileMenu);

            // 2. Sidebar (Left List)
            _lstNotes = new ListBox
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.Gainsboro,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };
            _lstNotes.SelectedIndexChanged += LstNotes_SelectedIndexChanged;

            // 3. Main Editor Container (Crucial Fix for Squeezing)
            Panel mainContainer = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 30, 30) };

            Panel editorPadding = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            _txtTitle = new TextBox
            {
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                Height = 40
            };

            Panel spacer = new Panel { Dock = DockStyle.Top, Height = 15 };

            _txtContent = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.Gainsboro,
                Font = new Font("Consolas", 12),
                BorderStyle = BorderStyle.None
            };

            // Hierarchy Assembly
            editorPadding.Controls.Add(_txtContent);
            editorPadding.Controls.Add(spacer);
            editorPadding.Controls.Add(_txtTitle);

            mainContainer.Controls.Add(editorPadding);

            // Adding to Form (Order matters!)
            this.Controls.Add(mainContainer);
            this.Controls.Add(_lstNotes);
            this.Controls.Add(_menuStrip); // Added last to ensure it stays on top
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtTitle.Text)) return;

            if (_selectedNoteIndex == -1)
            {
                _notes.Add(new Note { Title = _txtTitle.Text, Content = _txtContent.Text });
            }
            else
            {
                _notes[_selectedNoteIndex].Title = _txtTitle.Text;
                _notes[_selectedNoteIndex].Content = _txtContent.Text;
            }

            _service.SaveNotes(_notes);
            RefreshList();
            MessageBox.Show("Saved!");
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedNoteIndex != -1)
            {
                var result = MessageBox.Show("Delete this note?", "Confirm", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    _notes.RemoveAt(_selectedNoteIndex);
                    _service.SaveNotes(_notes);
                    RefreshList();
                    ResetEditor();
                }
            }
        }

        private void RefreshList()
        {
            _lstNotes.Items.Clear();
            foreach (var n in _notes) _lstNotes.Items.Add(n.Title);
        }

        private void LstNotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedNoteIndex = _lstNotes.SelectedIndex;
            if (_selectedNoteIndex != -1)
            {
                var selected = _notes[_selectedNoteIndex];
                _txtTitle.Text = selected.Title;
                _txtContent.Text = selected.Content;
            }
        }

        private void ResetEditor()
        {
            _txtTitle.Clear();
            _txtContent.Clear();
            _selectedNoteIndex = -1;
            _lstNotes.SelectedIndex = -1;
        }
    }
}