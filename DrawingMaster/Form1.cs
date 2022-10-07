using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace DrawingMaster
{
    public partial class Form1 : Form
    {

        public Form3 form;
        public Form1()
        {
            InitializeComponent();
            form = new Form3();
            form.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        public Bitmap image;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files(*.BMP,*.PNG,*.JPG,*.JPEG)|*.BMP;*.PNG;*.JPG;*.JPEG"; // file types, that will be allowed to upload
            dialog.Multiselect = false; // allow/deny user to upload more than one file at a time
            if (dialog.ShowDialog() == DialogResult.OK) // if user clicked OK
            {
                image = new Bitmap(resizeImageFromFile(Image.FromFile(dialog.FileName), Convert.ToInt32(numericUpDown2.Value), Convert.ToInt32(numericUpDown1.Value), checkBox2.Checked, checkBox1.Checked));
                UpdatePictureBox(image);
                label3.Text = image.Size.ToString();
            }
        }

        private void UpdatePictureBox(Bitmap bitmap)
        {
            int coef_x = pictureBox1.Width / bitmap.Width;
            int coef_y = pictureBox1.Height / bitmap.Height;
            Bitmap newImage = new Bitmap(bitmap.Width * coef_x, bitmap.Height * coef_y); // 5 - во сколько раз увеличить
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighSpeed;
                gr.CompositingQuality = CompositingQuality.HighSpeed;
                gr.InterpolationMode = InterpolationMode.NearestNeighbor;
                gr.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width * coef_x, bitmap.Height * coef_y)); // 5 - во сколько раз увеличить
            }

            pictureBox1.Image = newImage;
        }

        public Image resizeImageFromFile(Image FullsizeImage, int heigth, int width, Boolean keepAspectRatio, Boolean getCenter)
        {
            int newheigth = heigth;

            // Prevent using images internal thumbnail
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (keepAspectRatio || getCenter)
            {
                int bmpY = 0;
                double resize = (double)FullsizeImage.Width / (double)width;//get the resize vector
                if (getCenter)
                {
                    bmpY = (int)((FullsizeImage.Height - (heigth * resize)) / 2);// gives the Y value of the part that will be cut off, to show only the part in the center
                    Rectangle section = new Rectangle(new Point(0, bmpY), new Size(FullsizeImage.Width, (int)(heigth * resize)));// create the section to cut of the original image
                                                                                                                                 //System.Console.WriteLine("the section that will be cut off: " + section.Size.ToString() + " the Y value is minimized by: " + bmpY);
                    Bitmap orImg = new Bitmap((Bitmap)FullsizeImage);//for the correct effect convert image to bitmap.
                    FullsizeImage.Dispose();//clear the original image
                    using (Bitmap tempImg = new Bitmap(section.Width, section.Height))
                    {
                        Graphics cutImg = Graphics.FromImage(tempImg);//              set the file to save the new image to.
                        cutImg.DrawImage(orImg, 0, 0, section, GraphicsUnit.Pixel);// cut the image and save it to tempImg
                        FullsizeImage = tempImg;//save the tempImg as FullsizeImage for resizing later
                        orImg.Dispose();
                        cutImg.Dispose();
                        return FullsizeImage.GetThumbnailImage(width, heigth, null, IntPtr.Zero);
                    }
                }
                else newheigth = (int)(FullsizeImage.Height / resize);//  set the new heigth of the current image
            }//return the image resized to the given heigth and width
            return FullsizeImage.GetThumbnailImage(width, newheigth, null, IntPtr.Zero);
        }

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
            IntPtr lParam);

        static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                EnumThreadWindows(thread.Id,
                    (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

            return handles;
        }

        private IEnumerable<IntPtr> GetAllWindows()
        {
            return EnumerateProcessWindowHandles(Process.GetProcessesByName("dreamseeker")[0].Id);
        }

        private bool TurnCommand(string activate_command, List<string> data)
        {
            SetForegroundWindow(Process.GetProcessesByName("dreamseeker")[0].MainWindowHandle);
            IntPtr targetWindow = IntPtr.Zero;
            SendKeys.Send(activate_command);
            Thread.Sleep(120);
            IEnumerable<IntPtr> after_windows = GetAllWindows();
            targetWindow = GetAllWindows().First<IntPtr>();
            try
            {
                //targetWindow = GetLastWindow();
                SetForegroundWindow(targetWindow);
                foreach(string command in data)
                {
                    SendKeys.Send(command);
                    Thread.Sleep(40);
                }
                Thread.Sleep(120);
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static void ClickSomePoint(int x, int y)
        {
            // Set the cursor position
            System.Windows.Forms.Cursor.Position = new Point(x, y);

            DoClickMouse(0x2); // Left mouse button down
            DoClickMouse(0x4); // Left mouse button up
        }

        static void DoClickMouse(int mouseButton)
        {
            var input = new INPUT()
            {
                dwType = 0, // Mouse input
                mi = new MOUSEINPUT() { dwFlags = mouseButton }
            };

            if (SendInput(1, input, Marshal.SizeOf(input)) == 0)
            {
                throw new Exception();
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            int dx;
            int dy;
            int mouseData;
            public int dwFlags;
            int time;
            IntPtr dwExtraInfo;
        }
        struct INPUT
        {
            public uint dwType;
            public MOUSEINPUT mi;
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint cInputs, INPUT input, int size);

        private void ClickFromCoordinate(int x, int y)
        {
            double coef_x = form.panel.Width / Convert.ToInt32(numericUpDown1.Value);
            double coef_y = form.panel.Height / Convert.ToInt32(numericUpDown2.Value);
            double x_loc = form.Location.X + (coef_x * x) + coef_x;
            double y_loc = form.Location.Y + SystemInformation.CaptionHeight + (coef_y * y) + coef_y;

            ClickSomePoint(Convert.ToInt32(x_loc), Convert.ToInt32(y_loc));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (image == null)
                return;

            for (int i = 0; i < image.Width; i++)
            {
                for (int o = 0; o < image.Height; o++)
                {
                    Color color = image.GetPixel(i, o);
                    List<string> commands = new List<string>()
                        {
                            "{TAB}", "{TAB}", "{TAB}", "{TAB}", "{TAB}", Convert.ToString(color.R), "{TAB}", Convert.ToString(color.G), "{TAB}", Convert.ToString(color.B), "{TAB}", "{ENTER}",
                        };
                    TurnCommand("z", commands);
                    ClickFromCoordinate(i, o);
                    ClickFromCoordinate(i, o);
                    Thread.Sleep(10);
                }
            }
        }
    }
}