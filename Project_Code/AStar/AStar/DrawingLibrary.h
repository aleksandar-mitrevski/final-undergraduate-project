#ifndef DRAWING_LIBRARY_H
#define DRAWING_LIBRARY_H

#include <windows.h>
#include <GL\GL.h>
#include <gl\glut.h>
#include "AStarVisualWorldMap.h"
#include "DrawingConstants.h"

//object storing the visual map and the shortest path
AStarVisualWorldMap WorldMap;

//<summary>
//Class used for initializing OpenGL parameters for drawing
//the world grid and the shortest path.
//</summary>
class DrawingLibrary
{
public:
	void StartDrawing();
private:
	void Initialize();
};

void Render();
void DrawMap();
void TrackKeyboardKeys(unsigned char key, int mousePositionX, int mousePositionY);
void DrawField(unsigned int i, unsigned int j);
void DrawShortestPath();
bool FieldExpanded(int row, int column);

//<summary>
//Initializes GLUT parameters and starts drawing;
//adapter from http://openglsamples.sourceforge.net/files/glut_triangle.cpp.
//</summary>
void DrawingLibrary::StartDrawing()
{
	glutInitDisplayMode(GLUT_RGB | GLUT_DOUBLE | GLUT_DEPTH );  // Display Mode
	glutInitWindowSize(WINDOW_WIDTH,WINDOW_HEIGHT);				// set window size
	glutCreateWindow("A* Visualization");					// create Window
	glutDisplayFunc(Render);									// register Display Function
	glutIdleFunc(Render);										// register Idle Function
    glutKeyboardFunc(TrackKeyboardKeys);						// register Keyboard Handler
	this->Initialize();
	glutMainLoop();												// run GLUT mainloop
	return;
}

//<summary>
//Inititalizes OpenGL parameters for drawing;
//used from http://openglsamples.sourceforge.net/files/glut_triangle.cpp.
//</summary>
void DrawingLibrary::Initialize()
{
    glMatrixMode(GL_PROJECTION);												// select projection matrix
    glViewport(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);									// set the viewport
    glMatrixMode(GL_PROJECTION);												// set matrix mode
    glLoadIdentity();															// reset projection matrix
    GLfloat aspect = (GLfloat) WINDOW_WIDTH / WINDOW_HEIGHT;
	gluPerspective(VIEW_ANGLE, aspect, Z_NEAR, Z_FAR);		// set up a perspective projection matrix
    glMatrixMode(GL_MODELVIEW);													// specify which matrix is the current matrix
    glShadeModel( GL_SMOOTH );
    glClearDepth( 1.0f );														// specify the clear value for the depth buffer
    glEnable( GL_DEPTH_TEST );
    glDepthFunc( GL_LEQUAL );
    glHint( GL_PERSPECTIVE_CORRECTION_HINT, GL_NICEST );						// specify implementation-specific hints
	glClearColor(0.0, 0.0, 0.0, 1.0);											// specify clear values for the color buffers								
}

//<summary>
//Prepares the window for drawing and draws the grid.
//</summary>
void Render()
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);		     // Clear Screen and Depth Buffer
	glLoadIdentity();
	glTranslatef(0.0f,0.0f,-10.0f);		//y: [-4,4], x: [-5.5,5.5]
	glLineWidth(2.5f);
	DrawMap();
	glutSwapBuffers();
}

//<summary>
//Draws the grid and the shortest path.
//</summary>
void DrawMap()
{
	unsigned int numberOfRows = WorldMap.Vertices.size();
	unsigned int numberOfColumns = WorldMap.Vertices[0].size();

	//we are drawing the grid
	for(unsigned int i=0; i<numberOfRows; i++)
	{
		for(unsigned int j=0; j<numberOfColumns; j++)
		{
			if(WorldMap.Vertices[i][j].IsObstacle)
			{
				glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
				glColor3ub(0,255,0);
			}
			else if(FieldExpanded((int)i,(int)j))
			{
				glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
				glColor3ub(255,255,255);
			}

			DrawField(i,j);
			glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
			glColor3ub(0,0,255);
			DrawField(i,j);
		}
	}

	DrawShortestPath();
}

//<summary>
//Draws the field with grid coordinates 'i' and 'j'.
//</summary>
//<param name='i'>Row of the field.</param>
//<param name='j'>Column of the field.</param>
void DrawField(unsigned int i, unsigned int j)
{
	glBegin(GL_POLYGON);
		glVertex2f(WorldMap.Vertices[i][j].Vertices[0].X, WorldMap.Vertices[i][j].Vertices[0].Y);
		glVertex2f(WorldMap.Vertices[i][j].Vertices[1].X, WorldMap.Vertices[i][j].Vertices[1].Y);

		glVertex2f(WorldMap.Vertices[i][j].Vertices[1].X, WorldMap.Vertices[i][j].Vertices[1].Y);
		glVertex2f(WorldMap.Vertices[i][j].Vertices[2].X, WorldMap.Vertices[i][j].Vertices[2].Y);

		glVertex2f(WorldMap.Vertices[i][j].Vertices[2].X, WorldMap.Vertices[i][j].Vertices[2].Y);
		glVertex2f(WorldMap.Vertices[i][j].Vertices[3].X, WorldMap.Vertices[i][j].Vertices[3].Y);

		glVertex2f(WorldMap.Vertices[i][j].Vertices[3].X, WorldMap.Vertices[i][j].Vertices[3].Y);
		glVertex2f(WorldMap.Vertices[i][j].Vertices[0].X, WorldMap.Vertices[i][j].Vertices[0].Y);
	glEnd();
}

//<summary>
//Draws the shortest path as a red line that goes through all fiels that are part of the path.
//</summary>
void DrawShortestPath()
{
	glColor3ub(255,0,0);
	unsigned int numberOfVerticesInShortestPath = WorldMap.ShortestPathAndExpandedNodes.ShortestPath.size();

	for(unsigned int i=0; i<numberOfVerticesInShortestPath-1; i++)
	{
		int firstFieldRow = WorldMap.ShortestPathAndExpandedNodes.ShortestPath[i].X;
		int firstFieldColumn = WorldMap.ShortestPathAndExpandedNodes.ShortestPath[i].Y;

		int secondFieldRow = WorldMap.ShortestPathAndExpandedNodes.ShortestPath[i+1].X;
		int secondFieldColumn = WorldMap.ShortestPathAndExpandedNodes.ShortestPath[i+1].Y;

		glBegin(GL_LINES);
			glVertex2f(WorldMap.Vertices[firstFieldRow][firstFieldColumn].VertexCenter.X, WorldMap.Vertices[firstFieldRow][firstFieldColumn].VertexCenter.Y);
			glVertex2f(WorldMap.Vertices[secondFieldRow][secondFieldColumn].VertexCenter.X, WorldMap.Vertices[secondFieldRow][secondFieldColumn].VertexCenter.Y);
		glEnd();
	}
}

//<summary>
//Returns true if the field with grid coordinates 'row' and 'column'
//was expanded by the A* algorithm and false otherwise.
//</summary>
//<param name='row'>Row of the field.</param>
//<param name='column'>Column of the field.</param>
bool FieldExpanded(int row, int column)
{
	for(unsigned int i=0; i<WorldMap.ShortestPathAndExpandedNodes.ExpandedNodes.size(); i++)
	{
		int fieldRow = WorldMap.ShortestPathAndExpandedNodes.ExpandedNodes[i].X;
		int fieldColumn = WorldMap.ShortestPathAndExpandedNodes.ExpandedNodes[i].Y;

		if(row == fieldRow && column == fieldColumn)
			return true;
	}

	return false;
}

//<summary>
//Ends the program if the escape key is pressed; otherwise, does nothing.
//</summary>
void TrackKeyboardKeys(unsigned char key, int mousePositionX, int mousePositionY)
{
	switch (key) 
	{
		case KEY_ESCAPE:
			exit ( 0 );
			break;

		default:
			break;
	}
}

#endif