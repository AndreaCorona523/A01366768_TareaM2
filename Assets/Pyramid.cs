using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(MeshFilter))]
public class Pyramid : MonoBehaviour
{

    //Piramide con Mesh
    Mesh mesh;
    MeshFilter mf;

    //Vértices iniciales
    Vector3[] vertices = new Vector3[]{
        new Vector3 (-1.812f,-5.395f,5.247f),
        new Vector3 (-0.162f,-8.253f,5.247f),
        new Vector3 (-3.462f,-8.253f,5.247f),
        new Vector3 (-1.812f,-6.824f, 5.247f + (float)(Math.Sqrt(3.0f)/2.0f) * 3.3f)
    };

    //Triangulos para la piramide
    int[] triangles =  new int[]{
        1, 2, 3,
        0, 1, 2,
        3, 0, 1,
        0, 3, 2
    };

    //Vertices y piramides
    GameObject pyramidObj, vA, vB, vC, vD;

    //Vectores para las transformaciones
    Vector3 t1, t2, t;
    Vector3[] newVerticesT1, newVerticesRY, newVerticesT2;

    //Banderas de control para las transformaciones
    bool arrivedT1 = false;
    bool rotated = false;
    bool arrivedT2 = false;

    //Angulos de rotacion
    float degrees = -15.0f;
    float cosine, sine, rad;

    //Variable para tener tiempo de espera entre las transformaciones
    int times = 1000;

    void Start()
    {

        //Se crea la piramide
        mf = GetComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.vertices = vertices;
        mf.mesh.triangles = triangles;
  
        //Se encuentran los objetos
        pyramidObj = GameObject.Find("pyramid");
        vA = GameObject.Find("vA");
        vB = GameObject.Find("vB");
        vC = GameObject.Find("vC");
        vD = GameObject.Find("vD");

        //Se posicionan los vertices acorde a la piramide
        vA.transform.position = vertices[0];
        vB.transform.position = vertices[1];
        vC.transform.position = vertices[2];
        vD.transform.position = vertices[3];

        //Se imprimen las posiciones iniciales
        Debug.Log("iniciales");
        printVertexPositions();

        //Se calculan las transformaciones
        t1 = new Vector3(1.812f,7.18125f,-5.9615f);
        t2 = new Vector3(-1.812f,-7.18125f,5.9615f);

        newVerticesT1 = new Vector3[]{
            move(t1, vA.transform.position),
            move(t1, vB.transform.position),
            move(t1, vC.transform.position),
            move(t1, vD.transform.position)
        };

        newVerticesRY = new Vector3[]{
            rotate(newVerticesT1[0]),
            rotate(newVerticesT1[1]),
            rotate(newVerticesT1[2]),
            rotate(newVerticesT1[3])
        };

        newVerticesT2 = new Vector3[]{
            move(t2, newVerticesRY[0]),
            move(t2, newVerticesRY[1]),
            move(t2, newVerticesRY[2]),
            move(t2, newVerticesRY[3])
        };

    }


    void Update()
    {
        if (times >= 600 && times < 800){
            //Se traslada la piramide
            if (!arrivedT1){
                updatePositions(newVerticesT1);
                Debug.Log("traladar a T1");
                printVertexPositions();
                arrivedT1 = true;
                
            }

        } else if (times >= 400 && times < 600){
            //Se rota la piramide
            if (arrivedT1 && !rotated){

                updatePositions(newVerticesRY);
                Debug.Log("rotar");
                printVertexPositions();
                rotated = true;
            }

        }else if (times < 200){
            //Se regresa la piramide a la posición del pivote
            if (arrivedT1 && rotated && !arrivedT2){

                updatePositions(newVerticesT2);
                Debug.Log("regresar");
                printVertexPositions();
                arrivedT2 = true;
            }

        }

        if( times > 0){
            times--;
        }
        
        
        
    }

    //Función que realiza la multiplicación de matrices
    float[,]  matrixMultiplication(float[,] matrix1, float[,] matrix2){
 
        // cahing matrix lengths for better performance  
        var matrix1Rows = matrix1.GetLength(0);  
        var matrix1Cols = matrix1.GetLength(1);  
        var matrix2Rows = matrix2.GetLength(0);  
        var matrix2Cols = matrix2.GetLength(1);  
        
        // checking if product is defined  
        if (matrix1Cols != matrix2Rows) {
            return null;
            throw new InvalidOperationException  
            ("Product is undefined. n columns of first matrix must equal to n rows of second matrix"); 
            

        } 
             
        // creating the final product matrix  
        float[,] product = new float[matrix1Rows, matrix2Cols];  
        
        // looping through matrix 1 rows  
        for (int matrix1_row = 0; matrix1_row < matrix1Rows; matrix1_row++) {  
            // for each matrix 1 row, loop through matrix 2 columns  
            for (int matrix2_col = 0; matrix2_col < matrix2Cols; matrix2_col++) {  
                // loop through matrix 1 columns to calculate the dot product  
                for (int matrix1_col = 0; matrix1_col < matrix1Cols; matrix1_col++) {  
                    product[matrix1_row, matrix2_col] +=   
                    matrix1[matrix1_row, matrix1_col] *   
                    matrix2[matrix1_col, matrix2_col];  
                    
                }  
            }  
        }
        
        return product;  

    }

    //Funcion que genera la matriz de rotación
    float[,] generateRotationMatrix(){
        rad = (float)(Math.PI * degrees / 180.0);
        cosine = (float)Math.Cos(rad);
        sine = (float)Math.Sin(rad);
        float[,] rotationMatrix = new float[4, 4];

        rotationMatrix[0,0] = cosine;
        rotationMatrix[2,2] = cosine;
        rotationMatrix[0,2] = sine;
        rotationMatrix[2,0] = -sine;
        rotationMatrix[1,1] = 1;
        rotationMatrix[3,3] = 1;
         
        return rotationMatrix;

    }

    //Funcion que genera la matriz de traslación
    float[,] generateTraslateMatrix(Vector3 pos){
        float[,] rotationMatrix = new float[4, 4];

        rotationMatrix[0,0] = 1;
        rotationMatrix[1,1] = 1;
        rotationMatrix[2,2] = 1;
        rotationMatrix[3,3] = 1;
        
        rotationMatrix[0,3] = pos[0];
        rotationMatrix[1,3] = pos[1];
        rotationMatrix[2,3] = pos[2];
         
        return rotationMatrix;

    }

    //Funcion que rota un vector
    Vector3 rotate(Vector3 vertex){
        float[,] rotationMatrix = generateRotationMatrix();
        float[,] currPosition = obtainVertexPosition(vertex);
        float[,] newPosition = matrixMultiplication(rotationMatrix, currPosition);
        return generatePositionVector(newPosition);
    }

    //Funcion que traslada un vector
    Vector3 move(Vector3 t, Vector3 vertex){
        float[,] traslateMatrix1 = generateTraslateMatrix(t);
        float[,] currPosition = obtainVertexPosition(vertex);
        float[,] newPosition = matrixMultiplication(traslateMatrix1, currPosition);
        return generatePositionVector(newPosition);
    }

    //Función que imprime la posicion de los vértices
    void printVertexPositions(){
        Debug.Log(vA.transform.position);
        Debug.Log(vB.transform.position);
        Debug.Log(vC.transform.position);
        Debug.Log(vD.transform.position);
    }

    //Función que genera un vector a partir de una matriz
    Vector3 generatePositionVector(float[,] pos){
        return new Vector3(pos[0,0], pos[1,0], pos[2,0]);
    }

    //Funcion que genera una matriz de acuerdo a un vector
    float[,] obtainVertexPosition(Vector3 vertex){
        float[,] positions = new float[4, 1];
        positions[0, 0] = vertex[0];
        positions[1, 0] = vertex[1];
        positions[2, 0] = vertex[2];
        positions[3, 0] = 1;

        return positions;

    }

    //Funcion que actualiza las posiciones
    void updatePositions(Vector3[] newPositions){
        vA.transform.position = newPositions[0];
        vB.transform.position = newPositions[1];
        vC.transform.position = newPositions[2];
        vD.transform.position = newPositions[3];

        mf.mesh.vertices = newPositions;

    }

}
