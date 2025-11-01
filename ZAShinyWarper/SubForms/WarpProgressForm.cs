namespace ZAShinyWarper
{
    public partial class WarpProgressForm : Form
    {
        public string ShownText => lblWarping.Text;

        public void SetText(string text)
        {
            lblWarping.Text = text;
        }

        public WarpProgressForm()
        {
            InitializeComponent();
        }
    }
}
