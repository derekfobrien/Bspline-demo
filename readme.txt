Spline Demo
 
This is a (reasonably) simple program to demonstrate coding to draw and control a NURBS, or non-rational uniform B-spline (I think) in C#.
 
Here is what you can do with it:
-         determine the number of control points on the spline (will only accept a number between 3 and 20)
-         determine the order (will only accept a number between 2 and 10)
-         when the number of control points and order are entered, the button is clicked and the spline is drawn
-         I have programmed in default values for the number of control points and order, to 8 and 3 respectively, so that if these text boxes are left blank and the button is clicked, the spline will be drawn with these values
-         once a spline is drawn, you can click on one of the grips (with the left mouse button), i.e. the blue squares marking the control points, and dragging them on the window, just like in AutoCAD.
 
In the interest of, shall we say orderly object-oriented coding, I tried to restrict coding in the Form1 class to just what happens in the window, and the controls. I have also included a class, namely the Spline class, where I have put in the various properties and methods associated with the Spline class.
 
The methods in the Form1 class, for example, a function related to the MouseDown event in the Picture object, would only have a line to call the relevant method in the Spline class.

Here's an explanation for the files in the repository:

1. Form1.Designer.cs has details of the controls in the form, including all their properties etc.
2. Form1.cs has the code for the events for the various controls, as well as the class-wide variables.
3. We have a separate class, called Spline, and it is here that all the main work is done. The code for this is stored in Spline.cs.


