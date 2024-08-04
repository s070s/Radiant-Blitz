using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public GameObject roadSegmentPrefab;
    public GameObject buildingPrefab; // Add a reference to the building prefab
    public Vector3 segmentScale = new Vector3(1, 0.5f, 10); // Updated scale for the road segment
    public Vector3 buildingScale = new Vector3(2, 10, 2); // Scale for the buildings
    public int iterations = 3;
    public string axiom = "F";
    public List<Instruction> instructions = new List<Instruction>();

    [System.Serializable]
    public struct Instruction
    {
        public char character;
        public string rule;
    }

    private Dictionary<char, string> rules;
    private Stack<TransformInfo> transformStack;
    private Vector3 currentPosition;
    private Quaternion currentRotation;
    private List<RoadPoint> roadPoints;

    [System.Serializable]
    public struct RoadPoint
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    void Start()
    {
        rules = new Dictionary<char, string>();
        foreach (var instruction in instructions)
        {
            rules[instruction.character] = instruction.rule;
        }

        transformStack = new Stack<TransformInfo>();
        currentPosition = Vector3.zero;
        currentRotation = Quaternion.identity;
        roadPoints = new List<RoadPoint>();

        GenerateRoad();
        PlaceBuildingsInGaps();
    }

    void GenerateRoad()
    {
        string path = GenerateLSystemPath();

        foreach (char instruction in path)
        {
            ExecuteInstruction(instruction);
        }
    }

    string GenerateLSystemPath()
    {
        string path = axiom;

        for (int i = 0; i < iterations; i++)
        {
            string newPath = "";

            foreach (char c in path)
            {
                newPath += rules.ContainsKey(c) ? rules[c] : c.ToString();
            }

            path = newPath;
        }

        return path;
    }

    void ExecuteInstruction(char instruction)
    {
        switch (instruction)
        {
            case 'F':
                MoveForward();
                break;
            case 'R':
                TurnAndMove(-90);
                break;
            case 'L':
                TurnAndMove(90);
                break;
            case '+':
                Turn(90);
                break;
            case '-':
                Turn(-90);
                break;
            case '[':
                SaveTransform();
                break;
            case ']':
                RestoreTransform();
                break;
            default:
                break;
        }
    }

    void MoveForward()
    {
        Vector3 startPosition = currentPosition;
        Vector3 offset = Vector3.zero;

        if (Mathf.Approximately(currentRotation.eulerAngles.y, 90))
        {
            offset = (-Vector3.forward - Vector3.right) * segmentScale.x;
        }
        else if (Mathf.Approximately(currentRotation.eulerAngles.y, -90) || Mathf.Approximately(currentRotation.eulerAngles.y, 270))
        {
            offset = (-Vector3.forward + Vector3.right) * segmentScale.x;
        }

        currentPosition += currentRotation * (Vector3.forward * segmentScale.z + offset);

        CreateSegment(startPosition, currentPosition, currentRotation);
    }

    void TurnAndMove(float angle)
    {
        Vector3 startPosition = currentPosition;
        Turn(angle);
        MoveForward();
    }

    void Turn(float angle)
    {
        currentRotation *= Quaternion.Euler(0, angle, 0);
    }

    void SaveTransform()
    {
        transformStack.Push(new TransformInfo
        {
            position = currentPosition,
            rotation = currentRotation
        });
    }

    void RestoreTransform()
    {
        if (transformStack.Count > 0)
        {
            TransformInfo t = transformStack.Pop();
            currentPosition = t.position;
            currentRotation = t.rotation;
        }
    }

    void CreateSegment(Vector3 start, Vector3 end, Quaternion rotation)
    {
        Vector3 position = (start + end) / 2;
        GameObject segment = Instantiate(roadSegmentPrefab, position, rotation);
        segment.transform.localScale = segmentScale;
        roadPoints.Add(new RoadPoint { position = position, rotation = rotation });
    }

    void PlaceBuildingsInGaps()
    {
        List<Vector3> gapPositions = IdentifyGaps();

        foreach (Vector3 gapPosition in gapPositions)
        {
            CreateBuilding(gapPosition);
        }
    }

    List<Vector3> IdentifyGaps()
    {
        List<Vector3> gapPositions = new List<Vector3>();

        // Calculate the bounding box around the road segments to find gaps
        foreach (var point in roadPoints)
        {
            // Add logic to identify gaps in the road based on your specific criteria
            // For example, you can use a grid-based approach to identify gaps
            // Here, we're using a simple distance-based approach for demonstration

            // Check distances between points to find gaps
            for (int i = 0; i < roadPoints.Count; i++)
            {
                if (i > 0)
                {
                    float distance = Vector3.Distance(roadPoints[i - 1].position, roadPoints[i].position);
                    if (distance > 2*segmentScale.z)
                    {
                        Vector3 gapPosition = (roadPoints[i - 1].position + roadPoints[i].position) / 2;
                        gapPositions.Add(gapPosition);
                    }
                }
            }
        }

        return gapPositions;
    }

    void CreateBuilding(Vector3 position)
    {
        GameObject building = Instantiate(buildingPrefab, position, Quaternion.identity);
        building.transform.localScale = buildingScale;
    }

    struct TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}
