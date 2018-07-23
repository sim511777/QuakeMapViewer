using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace QuakeMapViewer {
   class Bsp {
      //public Entry entities; // List of Entities.
      public Plane[] planes;   // Map Planes.
      //public Entry miptex;   // Wall Textures.
      public Vertex[] vertices; // Map Vertices.
      //public Entry visilist; // Leaves Visibility lists.
      //public Entry nodes;    // BSP Nodes.
      //public Entry texinfo;  // Texture Info for faces.
      public Face[] faces;    // Faces of each surface.
      //public Entry lightmaps;// Wall Light Maps.
      //public Entry clipnodes;// clip nodes, for Models.
      //public Entry leaves;   // BSP Leaves.
      public ushort[] lface;    // List of Faces.
      public Edge[] edges;    // Edges of faces.
      public ushort[] ledges;   // List of Edges.
      //public Entry models;   // List of Models.

      public static Bsp Read(byte[] buf) {
         using (var ms = new MemoryStream(buf))
         using (var br = new BinaryReader(ms)) {
            Header header = Header.Read(br);
            
            Bsp bsp = new Bsp();
            bsp.planes     = ReadItems(br, header.planes,   (b)=>Plane.Read(b));
            bsp.vertices   = ReadItems(br, header.vertices, (b)=>Vertex.Read(b));
            bsp.faces      = ReadItems(br, header.faces,    (b)=>Face.Read(b));
            bsp.lface      = ReadItems(br, header.lface,    (b)=>b.ReadUInt16());
            bsp.edges      = ReadItems(br, header.edges,    (b)=>Edge.Read(b));
            bsp.ledges     = ReadItems(br, header.ledges,   (b)=>b.ReadUInt16());
            return bsp;
         }
      }

      public static T[] ReadItems<T>(BinaryReader br, Entry entry, Func<BinaryReader, T> itemReader) {
         var stream = br.BaseStream;
         var items = new List<T>();
         for (stream.Seek(entry.offset, SeekOrigin.Begin); stream.Position < entry.offset+entry.size;) {
            T item = itemReader(br);
            items.Add(item);
         }
         return items.ToArray();
      }
   }

   class Entry {
      public int offset;
      public int size;
      public static Entry Read(BinaryReader br) {
         Entry entry = new Entry();
         entry.offset   = br.ReadInt32();
         entry.size     = br.ReadInt32();
         return entry;
      }
   }

   class Header {
      public int version;
      public Entry entities;
      public Entry planes;
      public Entry miptex;
      public Entry vertices;
      public Entry visilist;
      public Entry nodes;
      public Entry texinfo;
      public Entry faces;
      public Entry lightmaps;
      public Entry clipnodes;
      public Entry leaves;
      public Entry lface;
      public Entry edges;
      public Entry ledges;
      public Entry models;
      public static Header Read(BinaryReader br) {
         Header header    = new Header();
         header.version   = br.ReadInt32();
         header.entities  = Entry.Read(br);
         header.planes    = Entry.Read(br);
         header.miptex    = Entry.Read(br);
         header.vertices  = Entry.Read(br);
         header.visilist  = Entry.Read(br);
         header.nodes     = Entry.Read(br);
         header.texinfo   = Entry.Read(br);
         header.faces     = Entry.Read(br);
         header.lightmaps = Entry.Read(br);
         header.clipnodes = Entry.Read(br);
         header.leaves    = Entry.Read(br);
         header.lface     = Entry.Read(br);
         header.edges     = Entry.Read(br);
         header.ledges    = Entry.Read(br);
         header.models    = Entry.Read(br);
         return header;
      }
   }

   class Vertex {
      public float x;
      public float y;
      public float z;
      public static Vertex Read(BinaryReader br) {
         Vertex vertex = new Vertex();
         vertex.x = br.ReadSingle();
         vertex.y = br.ReadSingle();
         vertex.z = br.ReadSingle();
         return vertex;
      }
   }

   enum PlaneType {
      AxialPlaneInX,
      AxialPlaneInY,
      AxialPlaneInZ,
      NonAxialPlaneTowardX,
      NonAxialPlaneTowardY,
      NonAxialPlaneTowardZ,
   }

   class Plane {
      public Vertex normal;
      public float dist;
      public PlaneType type;
      public static Plane Read(BinaryReader br) {
         Plane plane = new Plane();
         plane.normal = Vertex.Read(br);
         plane.dist = br.ReadSingle();
         plane.type = (PlaneType)br.ReadInt32();
         return plane;
      }
   }

   class Face {
      public ushort plane_id;
      public ushort side;
      public int    ledge_id;
      public ushort ledge_num;
      public ushort texinfo_id;
      public byte   typelight;
      public byte   baselight;
      public byte   light0;
      public byte   light1;
      public int    lightmap;
      public static Face Read(BinaryReader br) {
         Face face       = new Face();
         face.plane_id   = br.ReadUInt16();
         face.side       = br.ReadUInt16();
         face.ledge_id   = br.ReadInt32();
         face.ledge_num  = br.ReadUInt16();
         face.texinfo_id = br.ReadUInt16();
         face.typelight  = br.ReadByte();
         face.baselight  = br.ReadByte();
         face.light0     = br.ReadByte();
         face.light1     = br.ReadByte();
         face.lightmap   = br.ReadInt32();
         return face;
      }
   }

   class Edge {
      public ushort vertex0;
      public ushort vertex1;
      public static Edge Read(BinaryReader br) {
         Edge edge   = new Edge();
         edge.vertex0 = br.ReadUInt16();
         edge.vertex1 = br.ReadUInt16();
         return edge;
      }
   }
}
