using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Data.Enumenators;

public class ShapeSelectCanvas : MonoBehaviour
{
    #region Inspector
    [SerializeField] private Transform _square;
    [SerializeField] private Transform _circle;
    [SerializeField] private Transform _triangle;
    #endregion
    
    private Vector3 firstPointPos3D;
    private ShapeType _currentShapeType;

    public void SetShapePoint(ShapeType shapeType, Vector3 ShapePoint, bool firstPoint) 
    {
        _currentShapeType = shapeType;
        switch (shapeType)
        {
            case ShapeType.Quad:
                SetQuadSize(ShapePoint, firstPoint);
                break;
            case ShapeType.Triangle:
                SetTriangleSize(ShapePoint, firstPoint);
                break;
            case ShapeType.Circle:
                SetCircleSize(ShapePoint, firstPoint);
                break;
            default:
                break;
        }
    }

    public Vector3[] Complete()
    {
        Vector3[] points = null;
        switch (_currentShapeType)
        {
            case ShapeType.Quad:
                points = GetQuadCorners();
                break;
            case ShapeType.Triangle:
                points = GetTrianglePoints();
                break;
            case ShapeType.Circle:
                points = GetCirclePoints();
                break;
        }
        return points;
    }

    private void SetCircleSize(Vector3 ShapePoint, bool firstPoint) 
    {
        Vector3 resultPoint = ShapePoint;
        if (firstPoint)
        {
            _circle.gameObject.SetActive(true);
            firstPointPos3D = resultPoint;
        }

        float areaWidth = Vector3.Dot(resultPoint - firstPointPos3D, _circle.transform.right);
        float areaHeight = Vector3.Dot(resultPoint - firstPointPos3D, _circle.transform.up);

        _circle.transform.localScale = new Vector3(areaWidth, areaHeight, 1);
        _circle.transform.position =(firstPointPos3D + 0.5f * (resultPoint - firstPointPos3D));
    }
    
    private void SetTriangleSize(Vector3 ShapePoint, bool firstPoint)
    {
        Vector3 resultPoint = ShapePoint;
        if (firstPoint)
        {
            _triangle.gameObject.SetActive(true);
            firstPointPos3D = resultPoint;
        }

        float areaWidth = Vector3.Dot(resultPoint - firstPointPos3D, _triangle.transform.right);
        float areaHeight = Vector3.Dot(resultPoint - firstPointPos3D, _triangle.transform.up);

        _triangle.localScale = new Vector3(areaWidth, areaHeight, 1);
        _triangle.position = firstPointPos3D + 0.5f * (resultPoint - firstPointPos3D);
    }

    private void SetQuadSize(Vector3 ShapePoint, bool firstPoint)
    {
        Vector3 resultPoint = ShapePoint;
        if (firstPoint)
        {
            _square.gameObject.SetActive(true);
            firstPointPos3D = resultPoint;
        }

        float areaWidth = Vector3.Dot(resultPoint - firstPointPos3D, _square.transform.right);
        float areaHeight = Vector3.Dot(resultPoint - firstPointPos3D, _square.transform.up);

        _square.localScale =  new Vector3(areaWidth, areaHeight, 1);
        _square.position = firstPointPos3D + 0.5f * (resultPoint - firstPointPos3D);
    }

    private Vector3[] GetQuadCorners() 
    {
        _square.gameObject.SetActive(false);

        Vector3[] shapeCornerPositions = new Vector3[5];

        float halfWidth = 0.5f * _square.lossyScale.x;
        float halfHeight = 0.5f * _square.lossyScale.y;

        Transform squareTransform = _square.transform;
        Vector3 squarePosition = squareTransform.position;

        shapeCornerPositions[0] = squarePosition + squareTransform.right * halfWidth + squareTransform.up * halfHeight;
        shapeCornerPositions[1] = squarePosition + squareTransform.right * halfWidth - squareTransform.up * halfHeight;
        shapeCornerPositions[2] = squarePosition - squareTransform.right * halfWidth - squareTransform.up * halfHeight;
        shapeCornerPositions[3] = squarePosition - squareTransform.right * halfWidth + squareTransform.up * halfHeight;
        shapeCornerPositions[4] = shapeCornerPositions[0];

        return shapeCornerPositions;
    }

    private Vector3[] GetTrianglePoints()
    {
        float halfWidth = (0.5f * _triangle.lossyScale.x);
        float halfHeight = (0.5f * _triangle.lossyScale.y);
        var points = new Vector3[4];

        points[0] = _triangle.position + _triangle.transform.up * halfHeight;
        points[1] = _triangle.position - _triangle.transform.up * halfHeight + _triangle.transform.right * halfWidth;
        points[2] = _triangle.position - _triangle.transform.up * halfHeight - _triangle.transform.right * halfWidth;
        points[3] = points[0];

        _triangle.gameObject.SetActive(false);
        return points;
    }

    private Vector3[] GetCirclePoints() 
    {
        float halfWidth = ( 0.5f * _circle.localScale.x);
        float halfHeight = (0.5f * _circle.localScale.y);

        int pointCount = 36;
        float angle = 20f;
        var points = new Vector3[pointCount+1];

        Transform circleTransform = _circle.transform;
        Vector3 circlePosition = circleTransform.position;

        for (int i = 0; i < (pointCount + 1); i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * halfWidth;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * halfHeight;
            points[i] = circlePosition + circleTransform.right * x + circleTransform.up * y;

            angle += (360f / pointCount);
        }

        _circle.gameObject.SetActive(false);
        return points;
    }

}
