using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class _3DGridGenerator : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private Canvas _3DCanvas;
    [SerializeField]
    private GameObject bigLine;
    [SerializeField]
    private GameObject smallLine;
    [SerializeField]
    private GameObject timelinePrefab;
    [SerializeField]
    private TextMeshProUGUI txtBeatLinePrefab;

    private Rect _3DCanvasRect;
    private Rect smallLineRect;

    public readonly int startYPos = -500;
    public readonly int distance = 80;

    private bool hasGeneratedGrid = false;

    #endregion

    #region Properties

    public GameObject LastLine { get; private set; }

    public static _3DGridGenerator Instance { get; private set; }

    #endregion

    #region Events

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _3DCanvasRect = _3DCanvas.GetComponent<RectTransform>().rect;
        smallLineRect = smallLine.GetComponent<RectTransform>().rect;
    }

    private void Update()
    {
        if (MusicPlayer.Instance.IsLoaded && !hasGeneratedGrid)
        {
            int lastLineYPos;

            GenerateHorizontalLines(startYPos, out lastLineYPos);
            GenerateVerticalLines(startYPos, lastLineYPos + startYPos);
            InstantiateTimeline();

            hasGeneratedGrid = true;
        }
    }

    #endregion

    #region Methods

    public Vector2 GetCoordinatePosition(Vector2Int coordinate, GameObject cube)
    {
        Bounds cubeBounds = cube.GetComponent<MeshFilter>().mesh.bounds;
        float cubeHeight = cubeBounds.size.y * cube.transform.localScale.y;

        return new Vector2(smallLineRect.x + _3DCanvasRect.width / 4 * coordinate.x + _3DCanvasRect.width / 8,
            (coordinate.y) * (2 - cubeHeight) - 50 * (coordinate.y) - cubeHeight * 0.5f);
    }

    public double GetBeatPosition(double beat)
    {
        return startYPos + distance * beat * 4;
    }

    private void InstantiateTimeline()
    {
        GameObject timeline = Instantiate(timelinePrefab);
        timeline.transform.position = new Vector3(0, (float)GetBeatPosition(0), 0);
        timeline.transform.SetParent(_3DCanvas.transform, false);
    }

    private void GenerateHorizontalLines(int yPos, out int lastLineYPos)
    {
        int beat = 0;
        GameObject lineType = null;
        for (int i = 0; i < MapCreator._Map.GetAmountOfBeatsInSong() * 4; i++)
        {
            lineType = i % 4 == 0 ? bigLine : smallLine;
            InstantiateLine(lineType, new Vector2(0, yPos));

            if (lineType.Equals(bigLine))
            {
                InstantiateTextBeatLine(beat.ToString(), new Vector2(smallLineRect.width * 0.5f + 120, yPos));
                beat++;
            }

            yPos += distance;
        }

        LastLine = lineType;

        lastLineYPos = yPos;
    }

    private void GenerateVerticalLines(int yPos, float length)
    {
        float xPos = smallLineRect.x;
        for (int i = 0; i < 5; i++)
        {
            GameObject verticalLine;
            InstantiateVerticalLine(new Vector2(xPos, yPos), out verticalLine);
            verticalLine.GetComponent<RectTransform>().sizeDelta = new Vector2(length, smallLineRect.height);

            verticalLine.transform.position = new Vector3(verticalLine.transform.position.x, verticalLine.transform.position.y, verticalLine.transform.position.z + length * 0.5f);
            xPos += _3DCanvasRect.width / 4;
        }
    }

    private void InstantiateLine(GameObject lineType, Vector2 position)
    {
        GameObject line = Instantiate(lineType);
        line.transform.position = position;
        line.transform.SetParent(_3DCanvas.transform, false);
    }

    private void InstantiateVerticalLine(Vector2 position, out GameObject line)
    {
        line = Instantiate(smallLine);
        line.transform.Rotate(new Vector3(0, 0, 1), 90);
        line.transform.position = position;
        line.transform.SetParent(_3DCanvas.transform, false);
    }

    private void InstantiateTextBeatLine(string text, Vector2 position)
    {
        TextMeshProUGUI txt = Instantiate(txtBeatLinePrefab);
        txt.transform.position = position;
        txt.transform.SetParent(_3DCanvas.transform, false);
        txt.text = text;
    }

    #endregion
}
