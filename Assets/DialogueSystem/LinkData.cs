using System;

[Serializable]
public class LinkData
{
    public string baseGuid;
    public string portName;
    public string targetGuid;

    public LinkData(string baseGuid, string portName, string targetGuid)
    {
        this.baseGuid = baseGuid;
        this.portName = portName;
        this.targetGuid = targetGuid;
    }
}
