using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Media3D;

namespace QuakeMapViewer {
   class Bsp {
      //public Entry entities; // List of Entities.
      //public Entry planes;   // Map Planes.
      //                       // numplanes = size/sizeof(plane_t)
      //public Entry miptex;   // Wall Textures.
      public Vector3D[] vertices; // Map Vertices.
                             // numvertices = size/sizeof(vertex_t)
      //public Entry visilist; // Leaves Visibility lists.
      //public Entry nodes;    // BSP Nodes.
      //                       // numnodes = size/sizeof(node_t)
      //public Entry texinfo;  // Texture Info for faces.
      //                       // numtexinfo = size/sizeof(texinfo_t)
      //public Entry faces;    // Faces of each surface.
      //                       // numfaces = size/sizeof(face_t)
      //public Entry lightmaps;// Wall Light Maps.
      //public Entry clipnodes;// clip nodes, for Models.
      //                       // numclips = size/sizeof(clipnode_t)
      //public Entry leaves;   // BSP Leaves.
      //                       // numlaves = size/sizeof(leaf_t)
      //public Entry lface;    // List of Faces.
      //public Entry edges;    // Edges of faces.
      //                       // numedges = Size/sizeof(edge_t)
      //public Entry ledges;   // List of Edges.
      //public Entry models;   // List of Models.
      //                       // nummodels = Size/sizeof(model_t)
   }

   class Entry {             // A Directory entry
      public int offset;     // Offset to entry, in bytes, from start of file
      public int size;       // Size of entry in file, in bytes
   }

   class Header {            // The BSP file header
      public int version;    // Model version, must be 0x17 (23).
      public Entry entities; // List of Entities.
      public Entry planes;   // Map Planes.
                             // numplanes = size/sizeof(plane_t)
      public Entry miptex;   // Wall Textures.
      public Entry vertices; // Map Vertices.
                             // numvertices = size/sizeof(vertex_t)
      public Entry visilist; // Leaves Visibility lists.
      public Entry nodes;    // BSP Nodes.
                             // numnodes = size/sizeof(node_t)
      public Entry texinfo;  // Texture Info for faces.
                             // numtexinfo = size/sizeof(texinfo_t)
      public Entry faces;    // Faces of each surface.
                             // numfaces = size/sizeof(face_t)
      public Entry lightmaps;// Wall Light Maps.
      public Entry clipnodes;// clip nodes, for Models.
                             // numclips = size/sizeof(clipnode_t)
      public Entry leaves;   // BSP Leaves.
                             // numlaves = size/sizeof(leaf_t)
      public Entry lface;    // List of Faces.
      public Entry edges;    // Edges of faces.
                             // numedges = Size/sizeof(edge_t)
      public Entry ledges;   // List of Edges.
      public Entry models;   // List of Models.
                             // nummodels = Size/sizeof(model_t)
   }

   static class BinaryReaderExtension {
      public static Bsp ReadBsp(this byte[] buf) {
         using (var ms = new MemoryStream(buf))
         using (var br = new BinaryReader(ms)) {
            var header = br.ReadHeader();
            
            Bsp bsp = new Bsp();

            // vertices
            br.BaseStream.Seek(header.vertices.offset, SeekOrigin.Begin);
            bsp.vertices = new Vector3D[header.vertices.size / 12];
            for (int i=0; i<bsp.vertices.Length; i++) {
               var x = br.ReadSingle();
               var y = br.ReadSingle();
               var z = br.ReadSingle();
               bsp.vertices[i] = new Vector3D(x, y, z);
            }

            return bsp;
         }
      }

      public static Entry ReadEntry(this BinaryReader br) {
         Entry entry = new Entry();
         entry.offset   = br.ReadInt32();
         entry.size     = br.ReadInt32();
         return entry;
      }

      public static Header ReadHeader(this BinaryReader br) {
         Header header    = new Header();
         header.version   = br.ReadInt32();
         header.entities  = br.ReadEntry();
         header.planes    = br.ReadEntry();
         header.miptex    = br.ReadEntry();
         header.vertices  = br.ReadEntry();
         header.visilist  = br.ReadEntry();
         header.nodes     = br.ReadEntry();
         header.texinfo   = br.ReadEntry();
         header.faces     = br.ReadEntry();
         header.lightmaps = br.ReadEntry();
         header.clipnodes = br.ReadEntry();
         header.leaves    = br.ReadEntry();
         header.lface     = br.ReadEntry();
         header.edges     = br.ReadEntry();
         header.ledges    = br.ReadEntry();
         header.models    = br.ReadEntry();
         return header;
      }
   }
}
