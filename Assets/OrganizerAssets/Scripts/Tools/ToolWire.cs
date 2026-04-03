using UnityEngine;

public class ToolWire : MonoBehaviour
{
    public Transform monitor;
    public Transform tool;

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!monitor || !tool) return;

        lr.SetPosition(0, monitor.position);
        lr.SetPosition(1, tool.position);
    }

    public void Activate()
    {
        tool = GameObject.FindGameObjectWithTag("ToolWire").transform;
    }
}
