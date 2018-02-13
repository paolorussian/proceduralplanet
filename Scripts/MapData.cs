using System.Collections;
using System.Collections.Generic;

public class MapData  {


    public Dictionary<string, CellEdge> edgeDictionary = new Dictionary<string, CellEdge>();

    public Dictionary<string, CellEdge> edgeDictionaryXPlusNormalPatch = new Dictionary<string, CellEdge>();
    public Dictionary<string, CellEdge> edgeDictionaryXMinusNormalPatch = new Dictionary<string, CellEdge>();

    public Dictionary<string, CellEdge> edgeDictionaryYPlusNormalPatch = new Dictionary<string, CellEdge>();
    public Dictionary<string, CellEdge> edgeDictionaryYMinusNormalPatch = new Dictionary<string, CellEdge>();

    public Dictionary<string, CellEdge> edgeDictionaryZPlusNormalPatch = new Dictionary<string, CellEdge>();
    public Dictionary<string, CellEdge> edgeDictionaryZMinusNormalPatch = new Dictionary<string, CellEdge>();

   




}
