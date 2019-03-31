using System;
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

    public Grid _3DGrid { get; private set; }

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
            _3DGrid = new Grid(10);
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

    public double GetBeatPosition(double beat, float? startPos = null)
    {
        return (startPos.HasValue ? startPos.Value : startYPos) + distance * beat * 4;
    }

    private void InstantiateTimeline()
    {
        GameObject timeline = Instantiate(timelinePrefab);
        timeline.transform.position = new Vector3(0, (float)GetBeatPosition(0), 0);
        timeline.transform.SetParent(_3DCanvas.transform, false);
    }

    private List<HorizontalLine> GenerateHorizontalLines(int beat, out float size)
    {
        var lines = new List<HorizontalLine>();
        GameObject lineType = null;
        float yPos = (float)GetBeatPosition(beat);
        float startYPost = yPos;
        float startBeat = beat;

        for (int i = 0; i < 5; i++)
        {
            lineType = i % 4 == 0 ? bigLine : smallLine;
            GameObject line = InstantiateLine(lineType, new Vector2(0, yPos));

            TextMeshProUGUI beatText = null;
            if (lineType.Equals(bigLine))
            {
                beatText = InstantiateTextBeatLine(beat.ToString(), new Vector2(smallLineRect.width * 0.5f + 120, yPos));
                beat++;
            }

            lines.Add(new HorizontalLine(line, beatText));

            yPos += distance;
        }

        size = (Math.Abs(startYPos - yPos) - distance) / (startBeat + 1);

        return lines;
    }

    private List<GameObject> GenerateVerticalLines(int beat)
    {
        var verticalLines = new List<GameObject>();

        float length = distance * 4;
        var size = new Vector2(length, smallLineRect.height);
        float xPos = smallLineRect.x + size.y * 0.5f;
        float yPos = (float)GetBeatPosition(beat);
        for (int i = 0; i < 5; i++)
        {
            GameObject verticalLine;
            InstantiateVerticalLine(new Vector2(xPos, yPos), out verticalLine);
            verticalLine.GetComponent<RectTransform>().sizeDelta = size;

            verticalLine.transform.position = new Vector3(verticalLine.transform.position.x, verticalLine.transform.position.y, verticalLine.transform.position.z + length * 0.5f);
            verticalLines.Add(verticalLine);
            xPos += _3DCanvasRect.width / 4 - size.y / 4;
        }

        return verticalLines;
    }

    private GameObject InstantiateLine(GameObject lineType, Vector2 position)
    {
        GameObject line = Instantiate(lineType);
        line.transform.position = position;
        line.transform.SetParent(_3DCanvas.transform, false);
        return line;
    }

    private void InstantiateVerticalLine(Vector2 position, out GameObject line)
    {
        line = Instantiate(smallLine);
        line.transform.Rotate(new Vector3(0, 0, 1), 90);
        line.transform.position = position;
        line.transform.SetParent(_3DCanvas.transform, false);
    }

    private TextMeshProUGUI InstantiateTextBeatLine(string text, Vector2 position)
    {
        TextMeshProUGUI txt = Instantiate(txtBeatLinePrefab);
        txt.transform.position = position;
        txt.transform.SetParent(_3DCanvas.transform, false);
        txt.text = text;

        return txt;
    }

    #endregion

    public class HorizontalLine
    {
        public GameObject Line { get; private set; }
        public TextMeshProUGUI BeatText { get; private set; }

        public HorizontalLine(GameObject line, TextMeshProUGUI beatText)
        {
            Line = line;
            BeatText = beatText;
        }
    }

    public class BeatGrid
    {
        public List<HorizontalLine> _HorizontalLines { get; private set; }
        public List<GameObject> VerticalLines { get; private set; }
        public float Size { get; private set; }
        public int Beat { get; private set; }

        public BeatGrid(int beat, List<HorizontalLine> horizontalLines, List<GameObject> verticalLines, float size)
        {
            Beat = beat;
            _HorizontalLines = horizontalLines;
            VerticalLines = verticalLines;
            Size = size;
        }

        public void Move(int beat)
        {
            int beatText = beat;
            foreach (var horizontalLine in _HorizontalLines)
            {
                horizontalLine.Line.transform.position += new Vector3(0, 0, Size * (beat - Beat));
                if (horizontalLine.BeatText != null)
                {
                    horizontalLine.BeatText.transform.position += new Vector3(0, 0, Size * (beat - Beat));
                    horizontalLine.BeatText.text = beatText.ToString();
                    beatText++;
                }
            }

            foreach (var verticalLine in VerticalLines)
                verticalLine.transform.position += new Vector3(0, 0, Size * (beat - Beat));

            Beat = beat;
        }
    }

    public class Grid
    {
        public SortedList<double, BeatGrid> BeatGrids { get; private set; }
        public BeatGrid FirstBeatGrid { get; private set; }
        public BeatGrid LastBeatGrid { get; private set; }
        public int RenderDistance { get; private set; }

        public Grid(int renderDistance)
        {
            BeatGrids = new SortedList<double, BeatGrid>();

            for (int i = 0; i < renderDistance; i++)
                BeatGrids.Add(i, GenerateBeatGrid(i));

            FirstBeatGrid = BeatGrids[0];
            LastBeatGrid = BeatGrids[renderDistance - 1];
        }

        public void Update()
        {
            MoveBeatGrid(FirstBeatGrid.Beat, LastBeatGrid.Beat);
        }

        private void MoveBeatGrid(int from, int to)
        {
            if (!BeatGrids.ContainsKey(from))
                return;

            BeatGrid beatToMove = BeatGrids[from];
            FirstBeatGrid = BeatGrids[beatToMove.Beat + 1];

            beatToMove.Move(to + 1);
            LastBeatGrid = beatToMove;

            BeatGrids.Remove(from);
            BeatGrids.Add(to + 1, beatToMove);
        }

        private BeatGrid GenerateBeatGrid(int beat)
        {
            float size;
            List<HorizontalLine> horizontalLines = Instance.GenerateHorizontalLines(beat, out size);
            return new BeatGrid(beat, horizontalLines, Instance.GenerateVerticalLines(beat), size);
        }

        public bool shouldUpdate(double currentBeat)
        {
            return Math.Abs(currentBeat - FirstBeatGrid.Beat) >= 2;
        }
    }
}
