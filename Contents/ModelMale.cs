using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Contents
{
    public class ModelMale
    {
      public static string Mesh { get => Properties.Resource.MaleMesh; }
      public static Bitmap DiffuseTexture { get => Properties.Resource.MaleDiffuseTexture; }
      public static Bitmap NormalTexture { get => Properties.Resource.MaleNormalTexture; }
   }
}
