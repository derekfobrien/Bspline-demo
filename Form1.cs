using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NurbsDemo
{
    public partial class Form1 : Form
    {

        Spline mySpline = new Spline();
        bool mouseIsDown;
        int gripSize;

        public Form1()
        {
            InitializeComponent();
        }

        private void cmdGo_Click(object sender, EventArgs e)
        {
            // here we are setting the number of control points to 8
            // and order to 3

            int CPoints, Order, CPointsUnit;
            CPoints = 8;
            Order = 3;

            // Number of control points to be entered
            if (txtNrPoints.Text != "")
            {
                try
                {
                    CPoints = (Convert.ToInt32(txtNrPoints.Text));
                }
                catch (FormatException exc)
                {
                    MessageBox.Show(string.Concat("A non-numeric character has been entered here. The default value will be used. ", exc));
                }
            }

            // set limits on the number of control points we can have
            // at least 3, at most 20
            if (CPoints < 3) CPoints = 3;
            if (CPoints > 20) CPoints = 20;

            // Order
            if (txtOrder.Text != "")
            {
                try
                {
                    Order = (Convert.ToInt32(txtOrder.Text));
                }
                catch(FormatException exc)
                {
                    MessageBox.Show(string.Concat("A non-numeric character has been entered here. The default value will be used.", exc));
                }
            }

            // set limits on the order we can have
            // at least 2, at most 10
            if (Order < 2) Order = 2;
            if (Order > 10) Order = 10;

            // This is the number of points on the curve between two control points
            CPointsUnit = 10;

            mySpline.setValues(CPoints, Order, CPointsUnit, pictureBox1.Width, pictureBox1.Height);

            mySpline.drawSpline(pictureBox1, gripSize);
        }

        void resizeControls()
        {
            // This method resizes/ repositions the controls when the window is re-sized or maximised
            pictureBox1.Width = ClientSize.Width - ((3 * pictureBox1.Left) + txtNrPoints.Width);
            pictureBox1.Height = ClientSize.Height - (2 * pictureBox1.Top);

            txtNrPoints.Left = pictureBox1.Width + (2 * pictureBox1.Left);
            txtOrder.Left = txtNrPoints.Left;
            label1.Left = txtNrPoints.Left;
            label2.Left = txtNrPoints.Left;
            cmdGo.Left = txtNrPoints.Left;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            resizeControls();
            mouseIsDown = false;
            gripSize = 8;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            resizeControls();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            // this method is fired when the user lets go of left mouse button
            mouseIsDown = false;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // this method is fired when the user presses down on the left mouse button
            if (e.Button == MouseButtons.Left) //LMB
            {
                mySpline.nodeNumberClicked = mySpline.scanPressOnGrip(e.X, e.Y, gripSize);
                mouseIsDown = true;
            }
            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // this method is fired when the user moves the mouse on the PictureBox
            if (mouseIsDown && mySpline.nodeNumberClicked > -1)
            {
                mySpline.nodeMove(mySpline.nodeNumberClicked, e.X, e.Y, 6, pictureBox1);
            }
        }
    }
}
