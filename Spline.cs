using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NurbsDemo
{
    public struct point2D
    {
        public float x;
        public float y;
    }
    
    public class Spline
    {
        //the properties
        public ArrayList controlPoints = new ArrayList(); // Control Points
        public ArrayList curvePoints = new ArrayList(); // Final Curve
        public int Order;
        public int NrPoints; // number of control points
        public int CurvePointsUnit; // number of points on curve between two control points
        public int nodeNumberClicked;

        //constructor
        public Spline()
        { 
           
        }

        // set the values
        public void setValues(int nrCPoints, int Ord, int CPointsUnit, int PW, int PH)
        {
            NrPoints = nrCPoints;
            Order = Ord;
            CurvePointsUnit = CPointsUnit;

            Random Rnd = new Random();

            // clear the control points array

            controlPoints.Clear();

            // pick control points in random locations

            for (int i = 0; i < nrCPoints; i++)
            {
                point2D P = new point2D();
                P.x = (float)Rnd.Next(5, PW - 5);
                P.y = (float)Rnd.Next(5, PH - 5);
                controlPoints.Add(P);

            }
        }

        // calculate the spline

        public void calculateSpline()
        {
            ArrayList BasisArray = new ArrayList();
            ArrayList KnotVector = new ArrayList();
            //ArrayList NBasis = new ArrayList();
            // NPlusC = knot values
            int NPlusC, n, ptsCurve, icount;
            point2D thePoint, temp2D, thePointOnCurve;

            float f, t, step;

            NPlusC = NrPoints + Order;

            curvePoints.Clear();

            // Re-dimension the Basis Array
            BasisArray.Clear();

            for (int i = 0; i < NrPoints; i++)
            {
                f = 0;
                BasisArray.Add(f);

            }

            //Re-dimension the Knot Vector;
            KnotVector.Clear();
            
            for (int i = 0; i < NPlusC; i++)
            {
                n = 0;
                KnotVector.Add(n);
            }

            // generate the knot vector

            knot(KnotVector);

            // calculate the number of points on the rational B-spline curve

            t = 0;

            // number of points on the curve
            ptsCurve = 1 + (CurvePointsUnit * NrPoints);

            f = (int)KnotVector[NPlusC - 1];

            step = f / (ptsCurve - 1);

            for (int i = 0; i < ptsCurve; i++)
            {
                temp2D.x = 0;
                temp2D.y = 0;
                curvePoints.Add(temp2D);
            }

            icount = 0;

            for (int i = 0; i < ptsCurve; i++)
            {
                if ((int)KnotVector[NPlusC - 1] - t < 0.000005)
                    t = (int)KnotVector[NPlusC - 1];

                // generate the basis function for this value of t

                RBasis(t, KnotVector, BasisArray);

                // generate a point on the curve
                temp2D.x = 0;
                temp2D.y = 0;
                thePointOnCurve.x = 0;
                thePointOnCurve.y = 0;

                for (int j = 0; j < NrPoints; j++)
                {
                    thePoint = (point2D)controlPoints[j];
                    temp2D.x = (float)BasisArray[j] * thePoint.x;
                    temp2D.y = (float)BasisArray[j] * thePoint.y;

                    thePointOnCurve = (point2D)curvePoints[icount];

                    thePointOnCurve.x = thePointOnCurve.x + temp2D.x;
                    thePointOnCurve.y = thePointOnCurve.y + temp2D.y;

                    curvePoints[icount] = thePointOnCurve;
                }
                

                icount++;
                t = t + step;
            }
        }

        // calculate the knot vector
        /* I'm not ashamed to say that this method, as well as the RBasis method were
         wrecking my head trying to "translate" from C to C#, with regard to fixed-size arrays
         in C, to the ArrayList class in C#
         */
        
        // a non-uniform knot vector, with multiplicity equal to the order at the ends

        void knot(ArrayList x) 
        {
            // x is the array containing the knot vector, called from the
            // calculateSpline method
            int NplusC, Nplus2;

            NplusC = NrPoints + Order;
            Nplus2 = NrPoints + 2;

            x[0] = 0; // x(0)

            for (int i = 1; i < NplusC; i++)
            {
                if((i > Order - 1) && (i < Nplus2 - 1))
                {
                    x[i] = 1 + (int)x[i - 1];
                }
                else
                {
                    x[i] = x[i - 1];
                }
            }
        }

        // calculate the basis functions
 
        void RBasis(float u, ArrayList x, ArrayList r)
        {
            // u = time value 
            // x = knot vector
            // r = basis array
            // all called from the calculateSpline method
            int NPlusC;
            float f, an, ad, bn, bd, sum;
            ArrayList tempArray = new ArrayList();

            NPlusC = NrPoints + Order;

            f = 0;

            // set up the tempArray

            for (int i = 0; i < NPlusC; i++)
            {
                tempArray.Add(1.1);
            }

            // calculate the first order basis function

            for (int i = 0; i < NPlusC - 1; i++)
            {
                if ((u >= (float)((int)x[i])) && (u < (float)((int)x[i + 1])))
                    f = 1f;
                else
                    f = 0f;
                tempArray[i] = f;
            }

            // calculate higher order basis functions

            for (int k = 2; k <= Order; k++)
            {
                for (int i = 0; i < NPlusC - k; i++)
                {
                    if ((float)tempArray[i] != 0) // if zero, skip this calculation
                    {
                        an = (u - (float)((int)x[i]))*(float)tempArray[i];
                        ad = (float)((int)x[i + k - 1] - (int)x[i]);
                    }
                    else
                    {
                        an = 0f;
                        ad = 1f;
                    }

                    if ((float)tempArray[i + 1] != 0) // if zero, skip this calculation
                    {
                        bn = ((float)((int)x[i + k]) - u) * (float)tempArray[i + 1];
                        bd = (float)((int)x[i + k] - (int)x[i + 1]);
                    }
                    else
                    {
                        bn = 0f;
                        bd = 1f;
                    }

                    tempArray[i] = (an / ad) + (bn / bd);
                }
            }

            // Pick up the last point

            if (u == (float)((int)x[NPlusC - 1]))
            {
                tempArray[NrPoints - 1] = 1f;
            }

            // calculate the sum for the denominator of rational base functions

            sum = 0;

            for (int i = 0; i < NrPoints; i++)
            {
                sum = sum + (float)tempArray[i];
            }

            // form the rational basis functions and put in the r vector

            r[0] = 0;

            for (int i = 0; i < NrPoints; i++)
            {
                if (sum != 0)
                {
                    r[i] = ((float)tempArray[i]) / sum;
                }
                else
                {
                    r[i] = 0;
                }
            }
        }

        // plot the spline

        public void drawSpline(PictureBox pb, int GripSize)
        {
            int PW, PH, iXA, iXB, iYA, iYB;
            point2D PA, PB;

            PW = pb.Width;
            PH = pb.Height;

            //setup the Graphics object and the pens with different colours

            Graphics Gr = pb.CreateGraphics();
            Pen penGrey = new Pen(Color.FromArgb(127, 127, 127));
            Pen penRed = new Pen(Color.Red);
            Pen penBlue = new Pen(Color.Blue);
            Pen penGreen = new Pen(Color.Green);
            Pen penTurquoise = new Pen(Color.FromArgb(80, 160, 255));

            Gr.Clear(Color.White);

            //draw the straight lines between the control points
            for (int i = 0; i < controlPoints.Count - 1; i++)
            {
                PA = (point2D)controlPoints[i];
                PB = (point2D)controlPoints[i + 1];

                iXA = (int)PA.x;
                iYA = (int)PA.y;
                iXB = (int)PB.x;
                iYB = (int)PB.y;

                Gr.DrawLine(penGrey, iXA, iYA, iXB, iYB);
            }

            //draw the grips forming the control points

            for (int i = 0; i < controlPoints.Count; i++)
            {
                PA = (point2D)controlPoints[i];

                iXA = (int)PA.x;
                iYA = (int)PA.y;

                Gr.DrawLine(penBlue, iXA - GripSize, iYA - GripSize, iXA + GripSize, iYA - GripSize);
                Gr.DrawLine(penBlue, iXA + GripSize, iYA - GripSize, iXA + GripSize, iYA + GripSize);
                Gr.DrawLine(penBlue, iXA + GripSize, iYA + GripSize, iXA - GripSize, iYA + GripSize);
                Gr.DrawLine(penBlue, iXA - GripSize, iYA + GripSize, iXA - GripSize, iYA - GripSize);
            }

            //draw the curve

            calculateSpline();

            for (int i = 0; i < curvePoints.Count - 1; i++)
            {
                PA = (point2D)curvePoints[i];
                PB = (point2D)curvePoints[i + 1];

                iXA = (int)PA.x;
                iYA = (int)PA.y;
                iXB = (int)PB.x;
                iYB = (int)PB.y;

                Gr.DrawLine(penRed, iXA, iYA, iXB, iYB);
                Gr.DrawEllipse(penTurquoise, iXA - 3, iYA - 3, 6, 6);
            }

        }

        // when the LMB is pressed over the picture box, scan for click over the grips
        public int scanPressOnGrip(int X, int Y, int GripSize)
        {
            int nodeX, nodeY;
            point2D theNode;
            bool passL, passR, passT, passB;
            //string myStr;

            nodeNumberClicked = -1;

            for (int i = 0; i < NrPoints; i++)
            {
                theNode = (point2D)controlPoints[i];
                nodeX = (int)theNode.x;
                nodeY = (int)theNode.y;

                passL = (X > nodeX - GripSize);
                passR = (X < nodeX + GripSize);
                passT = (Y > nodeY - GripSize);
                passB = (Y < nodeY + GripSize);

                if (passL && passR && passT && passB)
                    nodeNumberClicked = i;
            }

            // I have written these few lines to "prove" some aspects of the program according as I am writing it
            // They are left in but commented out so that others can play around with it
            /*if (nodeNumber > -1)
            {
                myStr = string.Concat("Node ", nodeNumber.ToString(), " has been clicked");
                MessageBox.Show(myStr);
            }
            else
                MessageBox.Show("Nope. Can't find any nodes");*/

            return nodeNumberClicked;
        }

        // drag a node
        public void nodeMove(int nodeNo, int X, int Y, int gripSize, PictureBox pb)
        {
            point2D myPoint;

            myPoint.x = X;
            myPoint.y = Y;
            controlPoints[nodeNo] = myPoint;

            drawSpline(pb, gripSize);
        }
    }
}
