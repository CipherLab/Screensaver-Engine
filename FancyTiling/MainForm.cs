using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScreenSaverHelper;
using SharedKernel;
using Timer = System.Threading.Timer;

namespace FancyTiling
{
    public partial class MainForm : Form
    {

        #region Preview API's

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        #endregion

        private Timer Timer { get; set; }
        private Settings Settings { get; set; }
        bool _isPreviewMode = false;
        private ImageHelper ImageHelper { get; }
        #region Constructors

        public MainForm()
        {
            InitializeComponent();
        }

        //This constructor is passed the bounds this form is to show in
        //It is used when in normal mode
        public MainForm(Rectangle bounds)
        {
            InitializeComponent();
            this.Bounds = bounds;
            //hide the cursor
            Cursor.Hide();

            Settings = new Settings();
            Settings.LoadFromReg();
             
            Cursor.Hide();
            TopMost = true;

            _imageFiles = Directory
               .GetFiles(Settings.Path, "*.jpg", SearchOption.AllDirectories).ToList();
            if (_imageFiles.Count <= 0)
                return;

            if (Settings.Shuffle)
                _imageFiles = _imageFiles.Randomize().ToList();

            Timer = new Timer(ShowNextImage,
                null, 0, Settings.Speed * 1000);
            // this.BackgroundImage = new Bitmap(1,1);
            //  ShowImage(_imageFiles[0]);

            ImageHelper = new ImageHelper(bounds, Settings.Fancytile);

        }

        //This constructor is the handle to the select screensaver dialog preview window
        //It is used when in preview mode (/p)
        public MainForm(IntPtr previewHandle)
        {
            InitializeComponent();

            //set the preview window as the parent of this window
            SetParent(this.Handle, previewHandle);

            //make this a child window, so when the select screensaver dialog closes, this will also close
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));

            //set our window's size to the size of our window's new parent
            Rectangle parentRect;
            GetClientRect(previewHandle, out parentRect);
            this.Size = parentRect.Size;

            //set our location at (0, 0)
            this.Location = new Point(0, 0);

            _isPreviewMode = true;
        }

        #endregion


        //sets up the fake BSOD
        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!_isPreviewMode) //we don't want all those effects for just a preview
            {
                this.Refresh();
            }
        }

        private List<string> _imageFiles = new List<string>();
        private float _zoomFactor = .1f;

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
        }

        public bool IsLoadingNext = false;
        private async Task ShowImage(string f)
        {
            Timer.Change(Settings.Speed * 1000, Timeout.Infinite);

            IsLoadingNext = true;
            try
            {
                var result = await ImageHelper.MirrorUpconvertImage(f);
                using (var ms = new MemoryStream(result))
                {
                   this.BackgroundImage = new Bitmap(ms);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                GC.Collect();
                IsLoadingNext = false;
            }
        }

        private int _imageIdx = 0;
        private void ShowPrevImage()
        {

            if (IsLoadingNext)
                return;

            if (_imageIdx <= 0)
                return;

            ShowImage(_imageFiles[--_imageIdx]);
        }
        private void ShowNextImage(object state)
        {
            if (IsLoadingNext)
                return;

            if (_imageIdx >= _imageFiles.Count)
                _imageIdx = 0;

            ShowImage(_imageFiles[++_imageIdx]);
        }

        private void LoadSettings()
        {
            this.LoadSettings();
        }

    

        #region User Input

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                ShowNextImage(null);
                return;
            }
            if (e.KeyCode == Keys.Left)
            {
                ShowPrevImage();
                return;
            }
            if (!_isPreviewMode) //disable exit functions for preview
            {
                Application.Exit();
            }
        }

        private void MainForm_Click(object sender, EventArgs e)
        {
            if (!_isPreviewMode) //disable exit functions for preview
            {
                Application.Exit();
            }
        }

        //start off OriginalLoction with an X and Y of int.MaxValue, because
        //it is impossible for the cursor to be at that position. That way, we
        //know if this variable has been set yet.
        Point _originalLocation = new Point(int.MaxValue, int.MaxValue);

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isPreviewMode) //disable exit functions for preview
            {
                //see if originallocat5ion has been set
                if (_originalLocation.X == int.MaxValue & _originalLocation.Y == int.MaxValue)
                {
                    _originalLocation = e.Location;
                }
                //see if the mouse has moved more than 20 pixels in any direction. If it has, close the application.
                if (Math.Abs(e.X - _originalLocation.X) > 20 | Math.Abs(e.Y - _originalLocation.Y) > 20)
                {
                    Application.Exit();
                }
            }
        }

        #endregion
    }
}
