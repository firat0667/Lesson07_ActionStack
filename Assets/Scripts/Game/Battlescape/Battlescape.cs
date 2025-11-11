using Actions;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Game.Battlescape
{
    public class Battlescape : ActionStack.ActionBehavior, ISearchableGraph
    {
        public class Node : IPositionNode
        {
            public Vector3      m_vPosition;
            public List<Link>   m_links = new List<Link>();

            #region Properties

            public IEnumerable<ILink> Links => m_links;

            public Vector3 WorldPosition => m_vPosition;

            #endregion
        }

        private Node[,]             m_nodes;
        private Queue<Team>         m_teamQueue = new Queue<Team>();

        private static Battlescape  sm_instance;

        const int                   SIZE = 8;

        #region Properties

        public IEnumerable<INode> Nodes
        {
            get
            {
                for (int z = 0; z < SIZE; z++)
                {
                    for (int x = 0; x < SIZE; x++)
                    {
                        yield return m_nodes[x, z];
                    }
                }
            }
        }

        public static Battlescape Instance => sm_instance;

        #endregion

        void Start()
        {
            sm_instance = this;

            ActionStack.Main.PushAction(this);

            m_teamQueue = new Queue<Team>(GetComponentsInChildren<Team>());
        }

        public override void OnBegin(bool bFirstTime)
        {
            base.OnBegin(bFirstTime);

            if (bFirstTime)
            {
                GenerateBattlefieldMesh();

                // initialize all units
                foreach (Unit unit in GetComponentsInChildren<Unit>())
                {
                    unit.InitializeUnit();
                }
            }

            // do the team loop!
            Team team = m_teamQueue.Dequeue();
            m_teamQueue.Enqueue(team);
            ActionStack.Main.PushAction(team);
        }

        protected void GenerateBattlefieldMesh()
        {
            // generate a mesh
            List<Vector3> vertices = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<int> triangles = new List<int>();

            m_nodes = new Node[SIZE, SIZE];
            for (int z = 0; z < SIZE; z++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    int iStart = vertices.Count;
                    vertices.AddRange(new Vector3[]{
                        new Vector3(x - 0.5f, 0.0f, z - 0.5f),
                        new Vector3(x - 0.5f, 0.0f, z + 0.5f),
                        new Vector3(x + 0.5f, 0.0f, z + 0.5f),
                        new Vector3(x + 0.5f, 0.0f, z - 0.5f)
                    });

                    Color c = (z + x) % 2 == 0 ? Color.white : Color.black;
                    colors.AddRange(new Color[] { c, c, c, c });

                    triangles.AddRange(new int[]{
                        iStart + 0, iStart + 1, iStart + 2,
                        iStart + 0, iStart + 2, iStart + 3
                    });

                    m_nodes[x, z] = new Node { m_vPosition = new Vector3(x, 0.0f, z) };
                }
            }

            // create links
            for (int z = 0; z < SIZE; z++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    for (int z1 = -1; z1 <= 1; z1++)
                    {
                        for (int x1 = -1; x1 <= 1; x1++)
                        {
                            Vector2Int v = new Vector2Int(x + x1, z + z1);
                            if (v.x >= 0 && v.y >= 0 && v.x < SIZE && v.y < SIZE)
                            {
                                Node A = m_nodes[x, z];
                                Node B = m_nodes[v.x, v.y];
                                if (A != B)
                                {
                                    A.m_links.Add(new Link(A, B));
                                }
                            }
                        }
                    }
                }
            }

            // create mesh
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.colors = colors.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
        }

        public override bool IsDone()
        {
            return false;
        }

        public float Heuristic(INode start, INode goal)
        {
            if (start is IPositionNode A &&
                goal is IPositionNode B)
            {
                return Vector3.Distance(A.WorldPosition, B.WorldPosition);
            }

            return 1.0f;
        }
    }
}