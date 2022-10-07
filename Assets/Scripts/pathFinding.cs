using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathFinding : MonoBehaviour
{
    public const int MOVE_DIAGONAL_COST = 14;
    public const int MOVE_STRAIGHT_COST = 10;

    public bool carregou;

    public List<pathNode> openNodes;
    public List<pathNode> closeNodes;
    public List<pathNode> caminho;
    public pathNode inicio;
    public pathNode fim;

    public pathNode[,] listaNodes;

    public GameObject labirinto;
    public GameObject queijo;

    public int tamX;
    public int tamY;

    public float count;
    public int count2;

    // Start is called before the first frame update
    void Start()
    {
        carregou = false;
        count = 0f;
        count2 = 0;
        caminho = new List<pathNode>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!carregou)
        {
            carregar();
            caminho = AStar();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(count2 <= caminho.Count)
            {
                Debug.Log(caminho[count2].posX + "  " + caminho[count2].posY);
                transform.position = new Vector3(caminho[count2].posX, transform.position.y, caminho[count2].posY);
                count2++;
            }
            
        }

        

    }

    public void carregar()
    {
        Debug.Log("Entrou para carregar");

        tamX = labirinto.GetComponent<MazeGenerator>().mazeDepth;
        tamY = labirinto.GetComponent<MazeGenerator>().mazeWidth;

        listaNodes = labirinto.GetComponent<MazeGenerator>().nodes;

        //inicio = AcharInicio();
        //fim = AcharFim();

        inicio = listaNodes[0, 1];
        fim = listaNodes[18, 20];

        inicio.gCost = 0;
        inicio.hCost = CalculateDistanceCost(inicio, fim);
        inicio.CalculateFCost();

        openNodes = new List<pathNode> { inicio };
        closeNodes = new List<pathNode>();

        carregou = true;

        Debug.Log("Carregou");
    }

    List<pathNode> AStar()
    {
        while (openNodes.Count > 0)
        {
            count++;

            pathNode currentNode = GetLowestFCount(openNodes);

            Debug.Log("Indo " + currentNode.x + "  " + currentNode.y);
            Debug.Log(count);

            if (currentNode == fim)
            {

                caminho = CalcularCaminho(fim);
                return caminho;
            }
            
            openNodes.Remove(currentNode);
            closeNodes.Add(currentNode);
            
            foreach(pathNode vizinhosNode in PegarListaVizinhos(currentNode))
            {
                if(closeNodes.Contains(vizinhosNode)) continue;

                int tentativeGcost = currentNode.gCost + CalculateDistanceCost(currentNode, vizinhosNode);
                if(tentativeGcost < vizinhosNode.gCost)
                {
                    vizinhosNode.cameFromNode = currentNode;
                    vizinhosNode.gCost = tentativeGcost;
                    vizinhosNode.hCost = CalculateDistanceCost(vizinhosNode, fim);
                    vizinhosNode.CalculateFCost();

                    if(!openNodes.Contains(vizinhosNode))
                    {
                        openNodes.Add(vizinhosNode);
                    }
                }
            }
        }

        return null;
    }

    pathNode GetLowestFCount(List<pathNode> pathNodesList)
    {
        pathNode lowestFCount = pathNodesList[0];

        for (int i = 1; i < pathNodesList.Count; i++)
        {
            if (pathNodesList[i].fCost < lowestFCount.fCost)
            {
                lowestFCount = pathNodesList[i];
            }
        }

        return lowestFCount;
    }

    public List<pathNode> PegarListaVizinhos(pathNode nodo)
    {
        List<pathNode> listaVizinhos = new List<pathNode>();

        if(nodo.x -1 >= 0)
        {
            //Debug.Log(nodo.x - 1 + "  " + nodo.y);

            if (!listaNodes[nodo.x - 1, nodo.y].parede)
            {
                listaVizinhos.Add(listaNodes[nodo.x -1, nodo.y]);
            }

            if(nodo.y -1 >= 0)
            {
                if (!listaNodes[nodo.x - 1, nodo.y - 1].parede)
                {
                    listaVizinhos.Add(listaNodes[nodo.x - 1, nodo.y - 1]);
                }
                
            }
            if(nodo.y + 1 <= tamY)
            {
                if (!listaNodes[nodo.x - 1, nodo.y + 1].parede)
                {
                    listaVizinhos.Add(listaNodes[nodo.x - 1, nodo.y + 1]);
                }
                
            }
        }
        if (nodo.x + 1 <= tamX - 1 )
        {
            if(!listaNodes[nodo.x + 1, nodo.y].parede)
            {
                listaVizinhos.Add(listaNodes[nodo.x + 1, nodo.y]);
            }
            if (nodo.y - 1 >= 0)
            {
                if (!listaNodes[nodo.x + 1, nodo.y - 1].parede)
                {
                    listaVizinhos.Add(listaNodes[nodo.x + 1, nodo.y - 1]);
                }
            }
            if (nodo.y + 1 <= tamY)
            {
                //Debug.Log(nodo.x + 1 + "  " + nodo.y);

                if (!listaNodes[nodo.x + 1, nodo.y + 1].parede)
                {
                    listaVizinhos.Add(listaNodes[nodo.x + 1, nodo.y + 1]);
                }
               
            }
        }

        if(nodo.y -1 >= 0)
        {
            //Debug.Log(nodo.x + "  " + (nodo.y - 1));

            if (!listaNodes[nodo.x, nodo.y -1].parede)
            {
                listaVizinhos.Add(listaNodes[nodo.x, nodo.y - 1]);
            }
            
        }
        if(nodo.y + 1 <= tamY)
        {
            if (!listaNodes[nodo.x, nodo.y + 1].parede)
            {
                listaVizinhos.Add(listaNodes[nodo.x, nodo.y + 1]);
            }
        }

        return listaVizinhos;
    }

    public int CalculateDistanceCost(pathNode a, pathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);

        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    public List<pathNode> CalcularCaminho(pathNode fim)
    {
        List<pathNode> path = new List<pathNode>();

        path.Add(fim);

        pathNode currentNode = fim;

        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;

            Debug.Log("Voltando " + currentNode.x + " " + currentNode.y);
            count++;
            Debug.Log(count);

        }

        //path.Reverse();

        return path;
    }

    pathNode AcharInicio()
    {
        pathNode menorDistancia;

        menorDistancia = listaNodes[0, 0];

        for(int i = 1; i < tamX; i++)
        {
            for(int j = 0; j < tamY; j++)
            {
                if(Vector2.Distance(new Vector2(listaNodes[i, j].x, listaNodes[i, j].y), 
                    (Vector2)transform.position) < Vector2.Distance(new Vector2(menorDistancia.x, menorDistancia.y),
                    (Vector2)transform.position))
                {
                    menorDistancia = listaNodes[i, j];
                }
            }
        }

        Debug.Log("Inicio " + menorDistancia.x + "  " + menorDistancia.y);

        return menorDistancia;
    }

    pathNode AcharFim()
    {
        pathNode menorDistancia;

        menorDistancia = listaNodes[0, 0];

        for (int i = 1; i < tamX; i++)
        {
            for (int j = 0; j < tamY; j++)
            {
                if (Vector2.Distance(new Vector2(listaNodes[i, j].x, listaNodes[i, j].y),
                    (Vector2)queijo.transform.position) < Vector2.Distance(new Vector2(menorDistancia.x, menorDistancia.y),
                    (Vector2)queijo.transform.position))
                {
                    menorDistancia = listaNodes[i, j];
                }
            }
        }

        Debug.Log("Fim " + menorDistancia.x + "  " + menorDistancia.y);

        return menorDistancia;
    }
}
