using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ImageMagick;
using Microsoft.Win32;

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

        private Settings Settings { get; set; }
        bool _isPreviewMode = false;

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
            moveTimer.Stop();

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

        private Queue<string> _imageFiles = new Queue<string>();
        private Queue<string> _seenImages = new Queue<string>();
        private float _zoomFactor = .1f;

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {
            Settings = new Settings();
            Settings.LoadFromReg();

            Cursor.Hide();
            TopMost = true;

            moveTimer.Interval = Settings.Speed * 1000;
            moveTimer.Tick += new EventHandler(moveTimer_Tick);

            var tempItems = Directory
                .GetFiles(Settings.Path, "*.jpg", SearchOption.AllDirectories).ToList();

            if (Settings.Shuffle)
                tempItems = tempItems.Randomize().ToList();
         

            foreach (var tempItem in tempItems)
            {
                _imageFiles.Enqueue(tempItem);
            }
            moveTimer.Start();
        }

        public bool IsLoadingNext = false;
        private void ShowNextImage()
        {
            if (IsLoadingNext)
                return;

            IsLoadingNext = true;

            moveTimer.Stop();
            try
            {
                if (_imageFiles.Count <= 0)
                {
                    foreach (var s in _seenImages.ToList().Randomize())
                    {
                        _imageFiles.Enqueue(s);
                    }

                    _seenImages.Clear();
                }

                var first = _imageFiles.Dequeue();
                _seenImages.Enqueue(first);
                // this.BackgroundImage = new Bitmap(first);

                this.BackgroundImage = MirrorUpconvertImage(first);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                GC.Collect();
                moveTimer.Start();
                IsLoadingNext = false;
            }
        }

        private void moveTimer_Tick(object sender, System.EventArgs e)
        {
            ShowNextImage();

            //change the image

            //Size newSize = new Size((int)(originalBitmap.Width * zoomFactor), (int)(originalBitmap.Height * zoomFactor));
            //pbImage.Image = new Bitmap(originalBitmap, newSize);
            //zoomFactor += .2f;
        }

        private void LoadSettings()
        {
            this.LoadSettings();
        }

        private Bitmap MirrorUpconvertImage(string i)
        {
            if (Settings.Fancytile)
            {
                using (MagickImage orig = new MagickImage(i))
                {
                    int origWidth = orig.Width;
                    int origHeight = orig.Height;

                    if (origHeight >= Bounds.Height && origWidth >= Bounds.Width)
                    {
                        Debug.WriteLine($"{new FileInfo(i).Name} - Original image is big enough");
                        return new Bitmap(i);
                    }

                    if (origHeight >= Bounds.Height && origWidth < Bounds.Height)
                    {
                        Debug.WriteLine($"{new FileInfo(i).Name} - Not wide enough");
                        var mirroredWImage = MirrorLeftAndRight(orig.ToByteArray(), origWidth, origHeight);
                        return new MagickImage(mirroredWImage).ToBitmap();
                    }

                    if (origWidth >= Bounds.Width && origHeight < Bounds.Height)
                    {
                        Debug.WriteLine($"{new FileInfo(i).Name} - Not tall enough");
                        var mirroredHImage = MirrorUpAndDown(orig.ToByteArray(), origWidth, Bounds.Height);
                        return new MagickImage(mirroredHImage).ToBitmap();
                    }

                    if (origHeight < Bounds.Height && origWidth < Bounds.Width)
                    {
                        Debug.WriteLine($"{new FileInfo(i).Name} - Not tall or wide enough");
                        var mirroredWImage = MirrorLeftAndRight(orig.ToByteArray(), origWidth, origHeight);

                        //pass the niw extra wide one to be mirrored top and bottom
                        var mirroredHImage = MirrorUpAndDown(mirroredWImage.ToArray(), Bounds.Width, origHeight);
                        return new MagickImage(mirroredHImage).ToBitmap();
                    }

                    return new Bitmap(i);
                }
            }
            else
            {
                return new Bitmap(i);
            }
        }

        private byte[] MirrorUpAndDown(byte[] orig, int origWidth, int origHeight)
        {
            using (MagickImage top = new MagickImage(orig.ToArray()))
            using (MagickImage bottom = new MagickImage(orig.ToArray()))
            {
                var hDif = (Bounds.Height - origHeight) / 2;

                var geom1 = new MagickGeometry(0, 0, origWidth, hDif);
                top.Crop(geom1);
                var geom2 = new MagickGeometry(0, origHeight - hDif, origWidth, hDif);
                bottom.Crop(geom2);

                using (var imageCol = new MagickImageCollection())
                {
                    top.Flip();
                    bottom.Flip();
                    imageCol.Add(top);
                    imageCol.Add(new MagickImage(orig));
                    imageCol.Add(bottom);

                    using (var result = imageCol.AppendVertically())
                    {
                        var size = new MagickGeometry(origWidth, Bounds.Height);
                        size.IgnoreAspectRatio = true;
                        result.Resize(size);

                        return result.ToByteArray();
                    }
                }
            }
        }

        private byte[] MirrorLeftAndRight(byte[] orig, int origWidth, int origHeight)
        {
            using (MagickImage left = new MagickImage(orig.ToArray()))
            using (MagickImage right = new MagickImage(orig.ToArray()))
            {
                var wDif = (Bounds.Width - origWidth) / 2;

                var geom1 = new MagickGeometry(origWidth - wDif, 0, wDif, origHeight);
                right.Crop(geom1);
                var geom2 = new MagickGeometry(0, 0, wDif, origHeight);
                left.Crop(geom2);

                using (var imageCol = new MagickImageCollection())
                {
                    left.Flop();
                    right.Flop();
                    imageCol.Add(left);
                    imageCol.Add(new MagickImage(orig));
                    imageCol.Add(right);

                    using (var result = imageCol.AppendHorizontally())
                    {
                        var size = new MagickGeometry(Bounds.Width, origHeight);
                        size.IgnoreAspectRatio = true;
                        result.Resize(size);

                        return result.ToByteArray();
                    }
                }
            }
        }


        #region User Input

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
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
