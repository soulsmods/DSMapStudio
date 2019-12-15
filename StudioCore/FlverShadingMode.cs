using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore
{
    public enum FlverShadingMode
    {
        TEXDEBUG_DIFFUSEMAP = 0,
        TEXDEBUG_SPECULARMAP = 1,
        TEXDEBUG_NORMALMAP = 2,
        TEXDEBUG_EMISSIVEMAP = 3,
        TEXDEBUG_BLENDMASKMAP = 4,
        TEXDEBUG_SHININESSMAP = 5,
        TEXDEBUG_NORMALMAP_BLUE = 6,

        MESHDEBUG_NORMALS = 100,
        MESHDEBUG_NORMALS_MESH_ONLY = 101,
        MESHDEBUG_VERTEX_COLOR_ALPHA = 102,

        LEGACY = 200,
        PBR_GLOSS_DS3 = 201,
        PBR_GLOSS_BB = 202,
        CLASSIC_DIFFUSE_PTDE = 203,
    }
}
