using Actions;
using Game.Manager;
using Graphs;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battlescape
{
    public class PlayerInputAction : ActionStack.Action
    {
        private Mesh m_mesh;
        private Unit m_unit;
        private HashSet<Battlescape.Node> m_reachableNodes;
        private bool m_bIsDone;

        private static Material sm_vertexColor;

        public PlayerInputAction(Unit unit)
        {
            m_unit = unit;
        }

        public override void OnBegin(bool bFirstTime)
        {
            base.OnBegin(bFirstTime);

            if (bFirstTime)
            {
                m_mesh = new Mesh();
                m_mesh.hideFlags = HideFlags.DontSave;
                m_mesh.MarkDynamic();

              
                m_reachableNodes = GraphAlgorithms.GetNodesInRange(m_unit.Node, 2);
                m_reachableNodes.Remove(m_unit.Node);

                
                var toRemove = new List<Battlescape.Node>();
                foreach (var node in m_reachableNodes)
                {
                    if (UnitManager.Instance.IsNodeOccupied(node))
                        toRemove.Add(node);
                }
                foreach (var node in toRemove)
                    m_reachableNodes.Remove(node);
            }

            if (sm_vertexColor == null)
            {
                sm_vertexColor = new Material(Shader.Find("Common/VertexColor"));
            }

            UpdateMesh();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

         
            Graphics.DrawMesh(m_mesh, Matrix4x4.identity, sm_vertexColor, LayerMask.NameToLayer("Default"));

            if (Input.GetMouseButtonDown(0))
            {
                Camera mainCam = Camera.main;
                Ray mr = mainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                
                if (Physics.Raycast(mr, out hit))
                {
                    Unit clickedUnit = hit.collider.GetComponent<Unit>();
                    if (clickedUnit != null && clickedUnit.Team != m_unit.Team)
                    {
                        float dist = Vector3.Distance(m_unit.transform.position, clickedUnit.transform.position);
                        if (dist <= 1.5f)
                        {
                         
                            UnitAttackAction attack = m_unit.gameObject.AddComponent<UnitAttackAction>();
                            attack.Init(m_unit, clickedUnit);
                            ActionStack.Main.PushAction(attack);
                            m_unit.RemainingActionPoints = 0;
                            m_bIsDone = true;
                            return;
                        }
                        else
                        {
                            Debug.Log("Düşman çok uzakta, saldırı mesafesi dışında!");
                        }
                    }
                }

               
                Plane ground = new Plane(Vector3.up, Vector3.zero);
                float fEnter;
                if (ground.Raycast(mr, out fEnter))
                {
                    Vector3 vHit = mr.GetPoint(fEnter);
                    Battlescape.Node node = GraphAlgorithms.GetClosestNode<Battlescape.Node>(Battlescape.Instance, vHit);

                    if (m_reachableNodes.Contains(node))
                    {
                        UnitMoveAction move = m_unit.gameObject.AddComponent<UnitMoveAction>();
                        move.Init(m_unit, node);
                        ActionStack.Main.PushAction(move);
                        m_unit.RemainingActionPoints = 0;
                        m_bIsDone = true;
                    }
                }
            }
        }

        public override bool IsDone() => m_bIsDone;

        public override void OnEnd()
        {
            base.OnEnd();
            Object.Destroy(m_mesh);
            m_mesh = null;
        }

        protected void UpdateMesh()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<int> triangles = new List<int>();
            float yOffset = 0.02f;

            foreach (Battlescape.Node node in m_reachableNodes)
            {
                int iStart = vertices.Count;
                Vector3 vPosition = node.WorldPosition + new Vector3(0f, yOffset, 0f);

                vertices.AddRange(new Vector3[] {
                    vPosition + new Vector3(-0.5f, 0.0f, -0.5f),
                    vPosition + new Vector3(-0.5f, 0.0f,  0.5f),
                    vPosition + new Vector3( 0.5f, 0.0f,  0.5f),
                    vPosition + new Vector3( 0.5f, 0.0f, -0.5f)
                });

                bool occupied = UnitManager.Instance.IsNodeOccupied(node);
                Color c = occupied ? Color.red : Color.green;
                colors.AddRange(new Color[] { c, c, c, c });

                triangles.AddRange(new int[] {
                    iStart + 0, iStart + 1, iStart + 2,
                    iStart + 0, iStart + 2, iStart + 3
                });
            }

            m_mesh.Clear();
            m_mesh.vertices = vertices.ToArray();
            m_mesh.colors = colors.ToArray();
            m_mesh.triangles = triangles.ToArray();
            m_mesh.RecalculateBounds();
            m_mesh.RecalculateNormals();
        }
    }
}
