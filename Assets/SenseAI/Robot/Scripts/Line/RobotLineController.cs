using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


namespace SenseAI.Robot.UI
{
    public class RobotLineController : MonoBehaviour
    {

        [SerializeField] LineRenderer _line;
        [SerializeField] List<Vector3> _points;

        public event Action<Vector3[]> PathCalculated;

        void Awake()
        {
            _line = GetComponentInParent<LineRenderer>();
        }

        public void SetPath(Vector3[] points)
        {
            _points = points.ToList();
            _line.positionCount = points.Length;
            _line.SetPositions(points);
            _line.Simplify(0.1f);
        }

        public void SetPathByAgent(NavMeshAgent agent)
        {
            StartCoroutine(DelayedPathRender(agent));
        }

        public void ClearPath()
        {
            _line.positionCount = 0;
            _points.Clear();
        }
        
        IEnumerator DelayedPathRender(NavMeshAgent agent)
        {
            ClearPath();

            Debug.Log("Робот вычилсятет тракеткорию");
            yield return new WaitUntil(() => agent.pathPending);
            Debug.Log("Робот вычилил");
            yield return new WaitUntil(() => agent.path.corners.Length > 0);

            Vector3[] path = agent.path.corners;
            SetPath(path);

            PathCalculated?.Invoke(path);
        }
    }
}