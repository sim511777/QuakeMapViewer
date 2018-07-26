using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace QuakeMapViewer {
   class Bsp {
      public static Color[] palette;
      public static int[] colorMap;
      public static Color ColorMapColor(int idx) {
         return Bsp.palette[Bsp.colorMap[idx]];
      }
      public static Color ColorMapColor(int palIdx, int brightness) {
         int darkness = 15-brightness;
         return Bsp.palette[Bsp.colorMap[darkness*256+palIdx]];
      }

      public static void LoadPalette() {
         var bytes = File.ReadAllBytes("palette.lmp");
         int palCnt = bytes.Length/3; 
         Bsp.palette = Enumerable.Range(0, palCnt).Select(palIdx=>Color.FromArgb(bytes[palIdx*3], bytes[palIdx*3+1], bytes[palIdx*3+2])).ToArray();
         Bsp.colorMap = File.ReadAllBytes("colormap.lmp").Take(64*256).Select(b=>(int)b).ToArray();
      }

      public string     entities;    // List of Entities.
      public Plane[]    planes;      // Map Planes.
      public Mipheader  mipheader;   // Wall Textures.
      public Miptex[]   miptexs;     // Wall TexturesData
      public Vertex[]   vertices;    // Map Vertices.
      public byte[]     visilist;    // Leaves Visibility lists.
      public Node[]     nodes;       // BSP Nodes.
      public Surface[]  texinfo;     // Texture Info for faces.
      public Face[]     faces;       // Faces of each surface.
      public byte[]     lightmaps;   // Wall Light Maps.
      public Clipnode[] clipnodes;   // clip nodes, for Models.
      public Leaf[]     leaves;      // BSP Leaves.
      public ushort[]   lface;       // List of Faces.
      public Edge[]     edges;       // Edges of faces.
      public ushort[]   ledges;      // List of Edges.
      public Model[]    models;      // List of Models.

      public static Bsp Read(byte[] buf) {
         using (var ms = new MemoryStream(buf))
         using (var br = new BinaryReader(ms)) {
            Header header = Header.Read(br);
            
            Bsp bsp = new Bsp();

            var entitiesBytes = ReadItems(br, header.entities, (b)=>b.ReadByte());
            bsp.entities   = Encoding.ASCII.GetString(entitiesBytes, 0, entitiesBytes.ToList().IndexOf((byte)0));
            
            bsp.planes     = ReadItems(br, header.planes,   (b)=>Plane.Read(b));

            br.BaseStream.Seek(header.miptex.offset, SeekOrigin.Begin);
            bsp.mipheader  = Mipheader.Read(br);
            bsp.miptexs    = new Miptex[bsp.mipheader.numtex];
            for (int i=0; i<bsp.miptexs.Length; i++) {
               br.BaseStream.Seek(header.miptex.offset+bsp.mipheader.offset[i], SeekOrigin.Begin);
               bsp.miptexs[i] = Miptex.Read(br);
            }
            
            bsp.vertices   = ReadItems(br, header.vertices, (b)=>Vertex.Read(b));
            bsp.visilist   = ReadItems(br, header.visilist, (b)=>b.ReadByte());
            bsp.nodes      = ReadItems(br, header.nodes,    (b)=>Node.Read(b));
            bsp.texinfo    = ReadItems(br, header.texinfo,  (b)=>Surface.Read(b));
            bsp.faces      = ReadItems(br, header.faces,    (b)=>Face.Read(b));
            bsp.lightmaps  = ReadItems(br, header.lightmaps,(b)=>b.ReadByte());
            bsp.clipnodes  = ReadItems(br, header.clipnodes,(b)=>Clipnode.Read(b));
            bsp.leaves     = ReadItems(br, header.leaves,   (b)=>Leaf.Read(b));
            bsp.lface      = ReadItems(br, header.lface,    (b)=>b.ReadUInt16());
            bsp.edges      = ReadItems(br, header.edges,    (b)=>Edge.Read(b));
            bsp.ledges     = ReadItems(br, header.ledges,   (b)=>b.ReadUInt16());
            bsp.models     = ReadItems(br, header.models,   (b)=>Model.Read(b));
            
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
         Entry entry    = new Entry();
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

   class Boundbox {
      Vertex min;
      Vertex max;
      public static Boundbox Read(BinaryReader br) {
         Boundbox box = new Boundbox();
         box.min = Vertex.Read(br);
         box.max = Vertex.Read(br);
         return box;
      }
   }

   class BBoxshort {
      short   minX;
      short   minY;
      short   minZ;
      short   maxX;
      short   maxY;
      short   maxZ;
      public static BBoxshort Read(BinaryReader br) {
         BBoxshort box = new BBoxshort();
         box.minX = br.ReadInt16();
         box.minY = br.ReadInt16();
         box.minZ = br.ReadInt16();
         box.maxX = br.ReadInt16();
         box.maxY = br.ReadInt16();
         box.maxZ = br.ReadInt16();
         return box;
      }
   }

   class Node {
      int       plane_id;
      ushort    front;
      ushort    back;
      BBoxshort box;
      ushort    face_id;
      ushort    face_num;
      public static Node Read(BinaryReader br) {
         Node node = new Node();
         node.plane_id  = br.ReadInt32();
         node.front     = br.ReadUInt16();
         node.back      = br.ReadUInt16();
         node.box       = BBoxshort.Read(br);
         node.face_id   = br.ReadUInt16();
         node.face_num  = br.ReadUInt16();
         return node;
      }
   }

   class Surface {
      Vertex   vectorS;
      float    distS;
      Vertex   vectorT;
      float    distT;
      uint     texture_id;
      uint     animated;
      public static Surface Read(BinaryReader br) {
         Surface surface      = new Surface();
         surface.vectorS      = Vertex.Read(br);
         surface.distS        = br.ReadSingle();
         surface.vectorT      = Vertex.Read(br);
         surface.distT        = br.ReadSingle();
         surface.texture_id   = br.ReadUInt32();
         surface.animated     = br.ReadUInt32();
         return surface;
      }
   }

   class Clipnode {
      uint  planenum;
      short front;
      short back;
      public static Clipnode Read(BinaryReader br) {
         Clipnode node = new Clipnode();
         node.planenum = br.ReadUInt32();
         node.front    = br.ReadInt16();
         node.back     = br.ReadInt16();
         return node;
      }
   }

   class Leaf {
      int         type;
      int         vislist;
      BBoxshort   bound;
      ushort      lface_id;
      ushort      lface_num;
      byte        sndwater;
      byte        sndsky;
      byte        sndslime;
      byte        sndlava;
      public static Leaf Read(BinaryReader br) {
         Leaf leaf = new Leaf();
         leaf.type      = br.ReadInt32();
         leaf.vislist   = br.ReadInt32();
         leaf.bound     = BBoxshort.Read(br);
         leaf.lface_id  = br.ReadUInt16();
         leaf.lface_num = br.ReadUInt16();
         leaf.sndwater  = br.ReadByte();
         leaf.sndsky    = br.ReadByte();
         leaf.sndslime  = br.ReadByte();
         leaf.sndlava   = br.ReadByte();
         return leaf;
      }
   }

   class Model {
      Boundbox bound;
      Vertex   origin;
      int      node_id0;
      int      node_id1;
      int      node_id2;
      int      node_id3;
      int      numleafs;
      int      face_id;
      int      face_num;
      public static Model Read(BinaryReader br) {
         Model model    = new Model();
         model.bound    = Boundbox.Read(br);
         model.origin   = Vertex.Read(br);
         model.node_id0 = br.ReadInt32();
         model.node_id1 = br.ReadInt32();
         model.node_id2 = br.ReadInt32();
         model.node_id3 = br.ReadInt32();
         model.numleafs = br.ReadInt32();
         model.face_id  = br.ReadInt32();
         model.face_num = br.ReadInt32();
         return model;
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
      public Vertex     normal;
      public float      dist;
      public PlaneType  type;
      public static Plane Read(BinaryReader br) {
         Plane plane    = new Plane();
         plane.normal   = Vertex.Read(br);
         plane.dist     = br.ReadSingle();
         plane.type     = (PlaneType)br.ReadInt32();
         return plane;
      }
   }

   class Mipheader {
      public int    numtex;
      public int[]  offset;
      public static Mipheader Read(BinaryReader br) {
         Mipheader mipheader    = new Mipheader();
         mipheader.numtex       = br.ReadInt32();
         mipheader.offset       = new Int32[mipheader.numtex];
         for (int i=0; i<mipheader.numtex; i++) {
            mipheader.offset[i] = br.ReadInt32();
         }
         return mipheader;
      }
   }

   class Miptex {
      public string name;  // 16
      public uint   width;
      public uint   height;
      public uint   offset1;
      public uint   offset2;
      public uint   offset4;
      public uint   offset8;
      public byte[] texture1; // full size
      public byte[] texture2; // 1/2  size
      public byte[] texture4; // 1/4  size
      public byte[] texture8; // 1/16 size
      public static Miptex Read(BinaryReader br) {
         Miptex miptex   = new Miptex();
         var bytes       = br.ReadBytes(16);
         miptex.name     = Encoding.ASCII.GetString(bytes, 0, bytes.ToList().IndexOf(0));
         miptex.width    = br.ReadUInt32();
         miptex.height   = br.ReadUInt32();
         miptex.offset1  = br.ReadUInt32();
         miptex.offset2  = br.ReadUInt32();
         miptex.offset4  = br.ReadUInt32();
         miptex.offset8  = br.ReadUInt32();
         miptex.texture1 = br.ReadBytes((int)(miptex.width*miptex.height));
         miptex.texture2 = br.ReadBytes((int)(miptex.width*miptex.height)/4);
         miptex.texture4 = br.ReadBytes((int)(miptex.width*miptex.height)/16);
         miptex.texture8 = br.ReadBytes((int)(miptex.width*miptex.height)/64);
         return miptex;
      }

      //public static void SaveBitmap(byte[] bytes, uint width, uint height, string name) {
      //   var bmp = new Bitmap((int)width, (int)height);
      //   for (int y=0; y<height; y++) {
      //      for (int x=0; x<width; x++) {
      //         bmp.SetPixel(x, y, Bsp.palette[bytes[y*width+x]]);
      //      }
      //   }
      //   bmp.Save(name.Replace("*", "#"));
      //   bmp.Dispose();
      //}
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
         Edge edge      = new Edge();
         edge.vertex0   = br.ReadUInt16();
         edge.vertex1   = br.ReadUInt16();
         return edge;
      }
   }
}
