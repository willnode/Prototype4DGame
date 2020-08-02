using Engine4.Internal;

namespace Engine4.Rendering
{
    // Partial code for 120 and 600 cell
    public partial class Primitive4 : Modeler4
    {
        
        // Building block for primitive dodecahedron
        // The order follows: 5 top, 5 middle up, 5 middle low, 5 bottom. CCW from bird view
        static void AddDedocahedron(Buffer4 buffer, int a, int b, int c, int d, int e, int f, int g, int h,
        int i, int j, int k, int l, int m, int n, int o, int p, int q, int r, int s, int t)
        {

            switch (buffer.simplex)
            {
                case SimplexMode.Point:

                    buffer.AddPoint(
                        a, b, c, d, e, f, g, h, i, j, 
                        k, l, m, n, o, p, q, r, s, t);
                    break;

                case SimplexMode.Line: 

                    buffer.AddSegment(
                        a, f, b, g, c, h, d, i, e, j,
                        p, k, q, l, r, m, s, n, t, o);

                    buffer.AddPolygon(a, b, c, d, e);
                    buffer.AddPolygon(p, q, r, s, t);
                    buffer.AddPolygon(f, k, g, l, h, m, i, n, j, o);
                    break;

                case SimplexMode.Triangle:

                    buffer.AddPolygon(a, b, c, d, e);
                    buffer.AddPolygon(t, s, r, q, p);

                    buffer.AddPolygon(f, k, g, b, a);
                    buffer.AddPolygon(g, l, h, c, b);
                    buffer.AddPolygon(h, m, i, d, c);
                    buffer.AddPolygon(i, n, j, e, d);
                    buffer.AddPolygon(j, o, f, a, e);
                     
                    buffer.AddPolygon(p, q, k, f, o);
                    buffer.AddPolygon(q, r, l, g, k);
                    buffer.AddPolygon(r, s, m, h, l);
                    buffer.AddPolygon(s, t, n, i, m);
                    buffer.AddPolygon(t, p, o, j, n);

                    break;

                case SimplexMode.Tetrahedron:

                    // These have been so carefully ordered that there are no volume 'gap' inside.
                    // Don't touch. It took 24 hours with drinking coffee writing these blocks.

                    buffer.AddTrimid(a, b, c, g);
                    buffer.AddTrimid(a, d, e, j);
                    buffer.AddTrimid(c, m, h, l);
                    buffer.AddTrimid(d, m, i, n);

                    buffer.AddTrimid(f, o, j, t);
                    buffer.AddTrimid(f, k, g, p);
                    buffer.AddTrimid(r, q, p, l);
                    buffer.AddTrimid(r, s, t, n);

                    buffer.AddTrimid(a, c, d, m);
                    buffer.AddTrimid(r, p, t, f);

                    buffer.AddPrism(a, f, j, m, r, n);
                    buffer.AddPrism(a, f, g, m, r, l);

                    buffer.AddPyramid(d, a, j, n, m);
                    buffer.AddPyramid(c, a, g, l, m);
                    buffer.AddPyramid(t, f, j, n, r);
                    buffer.AddPyramid(p, f, g, l, n);

                    buffer.AddTrimid(t, r, f, n);
                    buffer.AddTrimid(p, r, f, l);

                    buffer.AddTrimid(a, c, m, l);
                    buffer.AddTrimid(a, d, m, n);

                    break;
                default:
                    break;
            }
        }

        static Vector4[] _cell120Vertices = {
            new Vector4(2, 2, 0, 0),
            new Vector4(2, 0, 2, 0),
            new Vector4(2, 0, 0, 2),
            new Vector4(0, 2, 2, 0),
            new Vector4(0, 2, 0, 2),
            new Vector4(0, 0, 2, 2),
        };

        static float[][] _cell120Vertices2 = new float[][]{
                new float[]{_SQRT5, 1},
                new float[]{1 / (_GR * _GR), _GR},
                new float[]{_GR * _GR, 1 / _GR},
            };

        static Vector4[] _cell120Vertices3 = {
            new Vector4(_GR * _GR, 1, 1 / (_GR * _GR), 0),
            new Vector4(_GR * _GR, 1 / (_GR * _GR), 0, 1),
            new Vector4(_GR * _GR, 0, 1, 1 / (_GR * _GR)),
            new Vector4(1, _GR * _GR, 0, 1 / (_GR * _GR)),
            new Vector4(1, 1 / (_GR * _GR), _GR * _GR, 0),
            new Vector4(1, 0, 1 / (_GR * _GR), _GR * _GR),
            new Vector4(1 / (_GR * _GR), _GR * _GR, 1, 0),
            new Vector4(1 / (_GR * _GR), 1, 0, _GR * _GR),
            new Vector4(1 / (_GR * _GR), 0, _GR * _GR, 1),
            new Vector4(0, _GR * _GR, 1 / (_GR * _GR), 1),
            new Vector4(0, 1, _GR * _GR, 1 / (_GR * _GR)),
            new Vector4(0, 1 / (_GR * _GR), 1, _GR * _GR),
            new Vector4(_SQRT5, _GR, 1 / _GR, 0),
            new Vector4(_SQRT5, 1 / _GR, 0, _GR),
            new Vector4(_SQRT5, 0, _GR, 1 / _GR),
            new Vector4(_GR, _SQRT5, 0, 1 / _GR),
            new Vector4(_GR, 1 / _GR, _SQRT5, 0),
            new Vector4(_GR, 0, 1 / _GR, _SQRT5),
            new Vector4(1 / _GR, _SQRT5, _GR, 0),
            new Vector4(1 / _GR, _GR, 0, _SQRT5),
            new Vector4(1 / _GR, 0, _SQRT5, _GR),
            new Vector4(0, _SQRT5, 1 / _GR, _GR),
            new Vector4(0, _GR, _SQRT5, 1 / _GR),
            new Vector4(0, 1 / _GR, _GR, _SQRT5),
        };

        static Vector4[] _cell120Vertices4 = {
            new Vector4(2, _GR, 1, 1 / _GR),
            new Vector4(2, 1, 1 / _GR, _GR),
            new Vector4(2, 1 / _GR, _GR, 1),
            new Vector4(_GR, 2, 1 / _GR, 1),
            new Vector4(_GR, 1, 2, 1 / _GR),
            new Vector4(_GR, 1 / _GR, 1, 2),
            new Vector4(1, 2, _GR, 1 / _GR),
            new Vector4(1, _GR, 1 / _GR, 2),
            new Vector4(1, 1 / _GR, 2, _GR),
            new Vector4(1 / _GR, 2, 1, _GR),
            new Vector4(1 / _GR, _GR, 2, 1),
            new Vector4(1 / _GR, 1, _GR, 2),
        };
        
        static Vector4[] _hexacosidedroidVertices = {
            new Vector4(_GR, 1, 1 / _GR, 0),
            new Vector4(_GR, 1 / _GR, 0, 1),
            new Vector4(_GR, 0, 1, 1 / _GR),
            new Vector4(1, _GR ,0 , 1 / _GR),
            new Vector4(1, 1 / _GR, _GR, 0),
            new Vector4(1, 0, 1 / _GR, _GR),
            new Vector4(1 / _GR, _GR, 1, 0),
            new Vector4(1 / _GR, 1, 0, _GR),
            new Vector4(1 / _GR, 0, _GR, 1),
            new Vector4(0, _GR, 1 / _GR, 1),
            new Vector4(0, 1, _GR, 1 / _GR),
            new Vector4(0, 1 / _GR, 1, _GR),
        };



        void MakeHecatonicosahedroid(Buffer4 buffer)
        {
            // 6 * 4 => 24
            for (int i = 0; i < _cell120Vertices.Length; i += 1)
                for (int x = -1; x <= 1; x += 2)
                    for (int y = -1; y <= 1; y += 2)
                    {
                        var v = _cell120Vertices[i];
                        v = Utility.GiveSign(v, x, y);
                        buffer.AddVertex(v);
                    }
            // 3 * 4 * 16 => 192 + 24 => 216
            for (int i = 0; i < _cell120Vertices2.Length; i++)
                for (int j = i == 1 ? 3 : 0; i == 1 ? j >= 0 : j < 4; j = i == 1 ? j - 1 : j + 1)
                    for (int x = -1; x <= 1; x += 2)
                        for (int y = -1; y <= 1; y += 2)
                            for (int z = -1; z <= 1; z += 2)
                                for (int w = -1; w <= 1; w += 2)
                                {
                                    var v = Vector4.one * _cell120Vertices2[i][1];
                                    v[j] = _cell120Vertices2[i][0];
                                    v = Utility.GiveSign(v, x, y, z, w);
                                    buffer.AddVertex(v);
                                }
            // 24 * 8 => 192 + 216 = 408
            for (int i = 0; i < _cell120Vertices3.Length; i++)
                for (int x = -1; x <= 1; x += 2)
                    for (int y = -1; y <= 1; y += 2)
                        for (int z = -1; z <= 1; z += 2)
                        {
                            var v = _cell120Vertices3[i];
                            v = Utility.GiveSign(v, x, y, z);
                            buffer.AddVertex(v);
                        }
            // 12 * 16 => 192 + 408 = 600
            for (int i = 0; i < _cell120Vertices4.Length; i++)
                for (int x = -1; x <= 1; x += 2)
                    for (int y = -1; y <= 1; y += 2)
                        for (int z = -1; z <= 1; z += 2)
                            for (int w = -1; w <= 1; w += 2)
                            {
                                var v = _cell120Vertices4[i];
                                v = Utility.GiveSign(v, x, y, z, w);
                                buffer.AddVertex(v);
                            }

            // Normalize
            var _R = radius / (_GR * _GR);
            for (int i = 0; i < buffer.m_VerticesCount; i++)
            {
                buffer.m_Vertices[i] *= _R;
            }

            // These list starts from one, hence shift back -1
            buffer.offset--;
            // Magic cell-table: http://web.archive.org/web/20091024133911/http://homepages.cwi.nl/~dik/english/mathematics/poly/db/5,3,3/c/s-1.html
            AddDedocahedron(buffer, 001, 313, 217, 218, 314, 337, 409, 153, 155, 411, 457, 025, 225, 027, 459, 105, 425, 321, 427, 107);
            AddDedocahedron(buffer, 001, 313, 217, 218, 314, 338, 410, 154, 156, 412, 458, 026, 226, 028, 460, 106, 426, 322, 428, 108);
            AddDedocahedron(buffer, 001, 313, 409, 457, 337, 338, 410, 089, 041, 241, 458, 090, 505, 169, 242, 042, 506, 361, 265, 170);
            AddDedocahedron(buffer, 001, 314, 411, 459, 337, 338, 412, 091, 043, 241, 460, 092, 507, 171, 242, 044, 508, 362, 266, 172);
            AddDedocahedron(buffer, 002, 315, 219, 220, 316, 339, 413, 157, 159, 415, 461, 029, 227, 031, 463, 109, 429, 323, 431, 111);
            AddDedocahedron(buffer, 002, 315, 219, 220, 316, 340, 414, 158, 160, 416, 462, 030, 228, 032, 464, 110, 430, 324, 432, 112);
            AddDedocahedron(buffer, 002, 315, 413, 461, 339, 340, 414, 093, 045, 243, 462, 094, 509, 173, 244, 046, 510, 363, 267, 174);
            AddDedocahedron(buffer, 002, 316, 415, 463, 339, 340, 416, 095, 047, 243, 464, 096, 511, 175, 244, 048, 512, 364, 268, 176);
            AddDedocahedron(buffer, 003, 317, 221, 222, 318, 341, 417, 161, 163, 419, 465, 033, 229, 035, 467, 113, 433, 325, 435, 115);
            AddDedocahedron(buffer, 003, 317, 221, 222, 318, 342, 418, 162, 164, 420, 466, 034, 230, 036, 468, 114, 434, 326, 436, 116);
            AddDedocahedron(buffer, 003, 317, 417, 465, 341, 342, 418, 097, 049, 245, 466, 098, 513, 177, 246, 050, 514, 365, 269, 178);
            AddDedocahedron(buffer, 003, 318, 419, 467, 341, 342, 420, 099, 051, 245, 468, 100, 515, 179, 246, 052, 516, 366, 270, 180);
            AddDedocahedron(buffer, 004, 319, 223, 224, 320, 343, 421, 165, 167, 423, 469, 037, 231, 039, 471, 117, 437, 327, 439, 119);
            AddDedocahedron(buffer, 004, 319, 223, 224, 320, 344, 422, 166, 168, 424, 470, 038, 232, 040, 472, 118, 438, 328, 440, 120);
            AddDedocahedron(buffer, 004, 319, 421, 469, 343, 344, 422, 101, 053, 247, 470, 102, 517, 181, 248, 054, 518, 367, 271, 182);
            AddDedocahedron(buffer, 004, 320, 423, 471, 343, 344, 424, 103, 055, 247, 472, 104, 519, 183, 248, 056, 520, 368, 272, 184);
            AddDedocahedron(buffer, 005, 329, 233, 234, 330, 345, 441, 153, 154, 442, 473, 025, 217, 026, 474, 089, 409, 313, 410, 090);
            AddDedocahedron(buffer, 005, 329, 233, 234, 330, 347, 445, 157, 158, 446, 477, 029, 219, 030, 478, 093, 413, 315, 414, 094);
            AddDedocahedron(buffer, 005, 329, 441, 473, 345, 347, 445, 121, 057, 249, 477, 125, 537, 185, 251, 061, 541, 377, 281, 189);
            AddDedocahedron(buffer, 005, 330, 442, 474, 345, 347, 446, 122, 058, 249, 478, 126, 538, 186, 251, 062, 542, 378, 282, 190);
            AddDedocahedron(buffer, 006, 331, 235, 236, 332, 346, 443, 155, 156, 444, 475, 027, 218, 028, 476, 091, 411, 314, 412, 092);
            AddDedocahedron(buffer, 006, 331, 235, 236, 332, 348, 447, 159, 160, 448, 479, 031, 220, 032, 480, 095, 415, 316, 416, 096);
            AddDedocahedron(buffer, 006, 331, 443, 475, 346, 348, 447, 123, 059, 250, 479, 127, 539, 187, 252, 063, 543, 379, 283, 191);
            AddDedocahedron(buffer, 006, 332, 444, 476, 346, 348, 448, 124, 060, 250, 480, 128, 540, 188, 252, 064, 544, 380, 284, 192);
            AddDedocahedron(buffer, 007, 333, 237, 238, 334, 349, 449, 161, 162, 450, 481, 033, 221, 034, 482, 097, 417, 317, 418, 098);
            AddDedocahedron(buffer, 007, 333, 237, 238, 334, 351, 453, 165, 166, 454, 485, 037, 223, 038, 486, 101, 421, 319, 422, 102);
            AddDedocahedron(buffer, 007, 333, 449, 481, 349, 351, 453, 129, 065, 253, 485, 133, 545, 193, 255, 069, 549, 381, 285, 197);
            AddDedocahedron(buffer, 007, 334, 450, 482, 349, 351, 454, 130, 066, 253, 486, 134, 546, 194, 255, 070, 550, 382, 286, 198);
            AddDedocahedron(buffer, 008, 335, 239, 240, 336, 350, 451, 163, 164, 452, 483, 035, 222, 036, 484, 099, 419, 318, 420, 100);
            AddDedocahedron(buffer, 008, 335, 239, 240, 336, 352, 455, 167, 168, 456, 487, 039, 224, 040, 488, 103, 423, 320, 424, 104);
            AddDedocahedron(buffer, 008, 335, 451, 483, 350, 352, 455, 131, 067, 254, 487, 135, 547, 195, 256, 071, 551, 383, 287, 199);
            AddDedocahedron(buffer, 008, 336, 452, 484, 350, 352, 456, 132, 068, 254, 488, 136, 548, 196, 256, 072, 552, 384, 288, 200);
            AddDedocahedron(buffer, 009, 321, 225, 227, 323, 353, 425, 153, 157, 429, 489, 025, 233, 029, 493, 121, 441, 329, 445, 125);
            AddDedocahedron(buffer, 009, 321, 225, 227, 323, 355, 427, 155, 159, 431, 491, 027, 235, 031, 495, 123, 443, 331, 447, 127);
            AddDedocahedron(buffer, 009, 321, 425, 489, 353, 355, 427, 105, 073, 257, 491, 107, 521, 201, 259, 075, 523, 369, 273, 203);
            AddDedocahedron(buffer, 009, 323, 429, 493, 353, 355, 431, 109, 077, 257, 495, 111, 525, 205, 259, 079, 527, 371, 275, 207);
            AddDedocahedron(buffer, 010, 322, 226, 228, 324, 354, 426, 154, 158, 430, 490, 026, 234, 030, 494, 122, 442, 330, 446, 126);
            AddDedocahedron(buffer, 010, 322, 226, 228, 324, 356, 428, 156, 160, 432, 492, 028, 236, 032, 496, 124, 444, 332, 448, 128);
            AddDedocahedron(buffer, 010, 322, 426, 490, 354, 356, 428, 106, 074, 258, 492, 108, 522, 202, 260, 076, 524, 370, 274, 204);
            AddDedocahedron(buffer, 010, 324, 430, 494, 354, 356, 432, 110, 078, 258, 496, 112, 526, 206, 260, 080, 528, 372, 276, 208);
            AddDedocahedron(buffer, 011, 325, 229, 231, 327, 357, 433, 161, 165, 437, 497, 033, 237, 037, 501, 129, 449, 333, 453, 133);
            AddDedocahedron(buffer, 011, 325, 229, 231, 327, 359, 435, 163, 167, 439, 499, 035, 239, 039, 503, 131, 451, 335, 455, 135);
            AddDedocahedron(buffer, 011, 325, 433, 497, 357, 359, 435, 113, 081, 261, 499, 115, 529, 209, 263, 083, 531, 373, 277, 211);
            AddDedocahedron(buffer, 011, 327, 437, 501, 357, 359, 439, 117, 085, 261, 503, 119, 533, 213, 263, 087, 535, 375, 279, 215);
            AddDedocahedron(buffer, 012, 326, 230, 232, 328, 358, 434, 162, 166, 438, 498, 034, 238, 038, 502, 130, 450, 334, 454, 134);
            AddDedocahedron(buffer, 012, 326, 230, 232, 328, 360, 436, 164, 168, 440, 500, 036, 240, 040, 504, 132, 452, 336, 456, 136);
            AddDedocahedron(buffer, 012, 326, 434, 498, 358, 360, 436, 114, 082, 262, 500, 116, 530, 210, 264, 084, 532, 374, 278, 212);
            AddDedocahedron(buffer, 012, 328, 438, 502, 358, 360, 440, 118, 086, 262, 504, 120, 534, 214, 264, 088, 536, 376, 280, 216);
            AddDedocahedron(buffer, 013, 361, 265, 269, 365, 393, 505, 169, 177, 513, 569, 041, 289, 049, 577, 137, 553, 385, 561, 145);
            AddDedocahedron(buffer, 013, 361, 265, 269, 365, 394, 506, 170, 178, 514, 570, 042, 290, 050, 578, 138, 554, 386, 562, 146);
            AddDedocahedron(buffer, 013, 361, 505, 569, 393, 394, 506, 089, 057, 297, 570, 090, 473, 185, 298, 058, 474, 345, 249, 186);
            AddDedocahedron(buffer, 013, 365, 513, 577, 393, 394, 514, 097, 065, 297, 578, 098, 481, 193, 298, 066, 482, 349, 253, 194);
            AddDedocahedron(buffer, 014, 362, 266, 270, 366, 395, 507, 171, 179, 515, 571, 043, 291, 051, 579, 139, 555, 387, 563, 147);
            AddDedocahedron(buffer, 014, 362, 266, 270, 366, 396, 508, 172, 180, 516, 572, 044, 292, 052, 580, 140, 556, 388, 564, 148);
            AddDedocahedron(buffer, 014, 362, 507, 571, 395, 396, 508, 091, 059, 299, 572, 092, 475, 187, 300, 060, 476, 346, 250, 188);
            AddDedocahedron(buffer, 014, 366, 515, 579, 395, 396, 516, 099, 067, 299, 580, 100, 483, 195, 300, 068, 484, 350, 254, 196);
            AddDedocahedron(buffer, 015, 363, 267, 271, 367, 397, 509, 173, 181, 517, 573, 045, 293, 053, 581, 141, 557, 389, 565, 149);
            AddDedocahedron(buffer, 015, 363, 267, 271, 367, 398, 510, 174, 182, 518, 574, 046, 294, 054, 582, 142, 558, 390, 566, 150);
            AddDedocahedron(buffer, 015, 363, 509, 573, 397, 398, 510, 093, 061, 301, 574, 094, 477, 189, 302, 062, 478, 347, 251, 190);
            AddDedocahedron(buffer, 015, 367, 517, 581, 397, 398, 518, 101, 069, 301, 582, 102, 485, 197, 302, 070, 486, 351, 255, 198);
            AddDedocahedron(buffer, 016, 364, 268, 272, 368, 399, 511, 175, 183, 519, 575, 047, 295, 055, 583, 143, 559, 391, 567, 151);
            AddDedocahedron(buffer, 016, 364, 268, 272, 368, 400, 512, 176, 184, 520, 576, 048, 296, 056, 584, 144, 560, 392, 568, 152);
            AddDedocahedron(buffer, 016, 364, 511, 575, 399, 400, 512, 095, 063, 303, 576, 096, 479, 191, 304, 064, 480, 348, 252, 192);
            AddDedocahedron(buffer, 016, 368, 519, 583, 399, 400, 520, 103, 071, 303, 584, 104, 487, 199, 304, 072, 488, 352, 256, 200);
            AddDedocahedron(buffer, 017, 369, 273, 277, 373, 385, 521, 201, 209, 529, 553, 073, 305, 081, 561, 137, 585, 401, 593, 145);
            AddDedocahedron(buffer, 017, 369, 273, 277, 373, 387, 523, 203, 211, 531, 555, 075, 307, 083, 563, 139, 587, 403, 595, 147);
            AddDedocahedron(buffer, 017, 369, 521, 553, 385, 387, 523, 105, 041, 289, 555, 107, 457, 169, 291, 043, 459, 337, 241, 171);
            AddDedocahedron(buffer, 017, 373, 529, 561, 385, 387, 531, 113, 049, 289, 563, 115, 465, 177, 291, 051, 467, 341, 245, 179);
            AddDedocahedron(buffer, 018, 370, 274, 278, 374, 386, 522, 202, 210, 530, 554, 074, 306, 082, 562, 138, 586, 402, 594, 146);
            AddDedocahedron(buffer, 018, 370, 274, 278, 374, 388, 524, 204, 212, 532, 556, 076, 308, 084, 564, 140, 588, 404, 596, 148);
            AddDedocahedron(buffer, 018, 370, 522, 554, 386, 388, 524, 106, 042, 290, 556, 108, 458, 170, 292, 044, 460, 338, 242, 172);
            AddDedocahedron(buffer, 018, 374, 530, 562, 386, 388, 532, 114, 050, 290, 564, 116, 466, 178, 292, 052, 468, 342, 246, 180);
            AddDedocahedron(buffer, 019, 371, 275, 279, 375, 389, 525, 205, 213, 533, 557, 077, 309, 085, 565, 141, 589, 405, 597, 149);
            AddDedocahedron(buffer, 019, 371, 275, 279, 375, 391, 527, 207, 215, 535, 559, 079, 311, 087, 567, 143, 591, 407, 599, 151);
            AddDedocahedron(buffer, 019, 371, 525, 557, 389, 391, 527, 109, 045, 293, 559, 111, 461, 173, 295, 047, 463, 339, 243, 175);
            AddDedocahedron(buffer, 019, 375, 533, 565, 389, 391, 535, 117, 053, 293, 567, 119, 469, 181, 295, 055, 471, 343, 247, 183);
            AddDedocahedron(buffer, 020, 372, 276, 280, 376, 390, 526, 206, 214, 534, 558, 078, 310, 086, 566, 142, 590, 406, 598, 150);
            AddDedocahedron(buffer, 020, 372, 276, 280, 376, 392, 528, 208, 216, 536, 560, 080, 312, 088, 568, 144, 592, 408, 600, 152);
            AddDedocahedron(buffer, 020, 372, 526, 558, 390, 392, 528, 110, 046, 294, 560, 112, 462, 174, 296, 048, 464, 340, 244, 176);
            AddDedocahedron(buffer, 020, 376, 534, 566, 390, 392, 536, 118, 054, 294, 568, 120, 470, 182, 296, 056, 472, 344, 248, 184);
            AddDedocahedron(buffer, 021, 377, 281, 285, 381, 401, 537, 185, 193, 545, 585, 057, 297, 065, 593, 137, 569, 393, 577, 145);
            AddDedocahedron(buffer, 021, 377, 281, 285, 381, 405, 541, 189, 197, 549, 589, 061, 301, 069, 597, 141, 573, 397, 581, 149);
            AddDedocahedron(buffer, 021, 377, 537, 585, 401, 405, 541, 121, 073, 305, 589, 125, 489, 201, 309, 077, 493, 353, 257, 205);
            AddDedocahedron(buffer, 021, 381, 545, 593, 401, 405, 549, 129, 081, 305, 597, 133, 497, 209, 309, 085, 501, 357, 261, 213);
            AddDedocahedron(buffer, 022, 378, 282, 286, 382, 402, 538, 186, 194, 546, 586, 058, 298, 066, 594, 138, 570, 394, 578, 146);
            AddDedocahedron(buffer, 022, 378, 282, 286, 382, 406, 542, 190, 198, 550, 590, 062, 302, 070, 598, 142, 574, 398, 582, 150);
            AddDedocahedron(buffer, 022, 378, 538, 586, 402, 406, 542, 122, 074, 306, 590, 126, 490, 202, 310, 078, 494, 354, 258, 206);
            AddDedocahedron(buffer, 022, 382, 546, 594, 402, 406, 550, 130, 082, 306, 598, 134, 498, 210, 310, 086, 502, 358, 262, 214);
            AddDedocahedron(buffer, 023, 379, 283, 287, 383, 403, 539, 187, 195, 547, 587, 059, 299, 067, 595, 139, 571, 395, 579, 147);
            AddDedocahedron(buffer, 023, 379, 283, 287, 383, 407, 543, 191, 199, 551, 591, 063, 303, 071, 599, 143, 575, 399, 583, 151);
            AddDedocahedron(buffer, 023, 379, 539, 587, 403, 407, 543, 123, 075, 307, 591, 127, 491, 203, 311, 079, 495, 355, 259, 207);
            AddDedocahedron(buffer, 023, 383, 547, 595, 403, 407, 551, 131, 083, 307, 599, 135, 499, 211, 311, 087, 503, 359, 263, 215);
            AddDedocahedron(buffer, 024, 380, 284, 288, 384, 404, 540, 188, 196, 548, 588, 060, 300, 068, 596, 140, 572, 396, 580, 148);
            AddDedocahedron(buffer, 024, 380, 284, 288, 384, 408, 544, 192, 200, 552, 592, 064, 304, 072, 600, 144, 576, 400, 584, 152);
            AddDedocahedron(buffer, 024, 380, 540, 588, 404, 408, 544, 124, 076, 308, 592, 128, 492, 204, 312, 080, 496, 356, 260, 208);
            AddDedocahedron(buffer, 024, 384, 548, 596, 404, 408, 552, 132, 084, 308, 600, 136, 500, 212, 312, 088, 504, 360, 264, 216);
            AddDedocahedron(buffer, 025, 409, 089, 473, 441, 425, 457, 505, 057, 121, 105, 041, 569, 537, 489, 521, 553, 137, 585, 073);
            AddDedocahedron(buffer, 026, 410, 090, 474, 442, 426, 458, 506, 058, 122, 106, 042, 570, 538, 490, 522, 554, 138, 586, 074);
            AddDedocahedron(buffer, 027, 411, 091, 475, 443, 427, 459, 507, 059, 123, 107, 043, 571, 539, 491, 523, 555, 139, 587, 075);
            AddDedocahedron(buffer, 028, 412, 092, 476, 444, 428, 460, 508, 060, 124, 108, 044, 572, 540, 492, 524, 556, 140, 588, 076);
            AddDedocahedron(buffer, 029, 413, 093, 477, 445, 429, 461, 509, 061, 125, 109, 045, 573, 541, 493, 525, 557, 141, 589, 077);
            AddDedocahedron(buffer, 030, 414, 094, 478, 446, 430, 462, 510, 062, 126, 110, 046, 574, 542, 494, 526, 558, 142, 590, 078);
            AddDedocahedron(buffer, 031, 415, 095, 479, 447, 431, 463, 511, 063, 127, 111, 047, 575, 543, 495, 527, 559, 143, 591, 079);
            AddDedocahedron(buffer, 032, 416, 096, 480, 448, 432, 464, 512, 064, 128, 112, 048, 576, 544, 496, 528, 560, 144, 592, 080);
            AddDedocahedron(buffer, 033, 417, 097, 481, 449, 433, 465, 513, 065, 129, 113, 049, 577, 545, 497, 529, 561, 145, 593, 081);
            AddDedocahedron(buffer, 034, 418, 098, 482, 450, 434, 466, 514, 066, 130, 114, 050, 578, 546, 498, 530, 562, 146, 594, 082);
            AddDedocahedron(buffer, 035, 419, 099, 483, 451, 435, 467, 515, 067, 131, 115, 051, 579, 547, 499, 531, 563, 147, 595, 083);
            AddDedocahedron(buffer, 036, 420, 100, 484, 452, 436, 468, 516, 068, 132, 116, 052, 580, 548, 500, 532, 564, 148, 596, 084);
            AddDedocahedron(buffer, 037, 421, 101, 485, 453, 437, 469, 517, 069, 133, 117, 053, 581, 549, 501, 533, 565, 149, 597, 085);
            AddDedocahedron(buffer, 038, 422, 102, 486, 454, 438, 470, 518, 070, 134, 118, 054, 582, 550, 502, 534, 566, 150, 598, 086);
            AddDedocahedron(buffer, 039, 423, 103, 487, 455, 439, 471, 519, 071, 135, 119, 055, 583, 551, 503, 535, 567, 151, 599, 087);
            AddDedocahedron(buffer, 040, 424, 104, 488, 456, 440, 472, 520, 072, 136, 120, 056, 584, 552, 504, 536, 568, 152, 600, 088);
            AddDedocahedron(buffer, 153, 217, 154, 234, 233, 225, 218, 226, 158, 157, 155, 156, 228, 219, 227, 235, 236, 160, 220, 159);
            AddDedocahedron(buffer, 161, 221, 162, 238, 237, 229, 222, 230, 166, 165, 163, 164, 232, 223, 231, 239, 240, 168, 224, 167);
            AddDedocahedron(buffer, 169, 241, 171, 291, 289, 265, 242, 266, 179, 177, 170, 172, 270, 245, 269, 290, 292, 180, 246, 178);
            AddDedocahedron(buffer, 173, 243, 175, 295, 293, 267, 244, 268, 183, 181, 174, 176, 272, 247, 271, 294, 296, 184, 248, 182);
            AddDedocahedron(buffer, 185, 249, 186, 298, 297, 281, 251, 282, 194, 193, 189, 190, 286, 253, 285, 301, 302, 198, 255, 197);
            AddDedocahedron(buffer, 187, 250, 188, 300, 299, 283, 252, 284, 196, 195, 191, 192, 288, 254, 287, 303, 304, 200, 256, 199);
            AddDedocahedron(buffer, 201, 257, 205, 309, 305, 273, 259, 275, 213, 209, 203, 207, 279, 261, 277, 307, 311, 215, 263, 211);
            AddDedocahedron(buffer, 202, 258, 206, 310, 306, 274, 260, 276, 214, 210, 204, 208, 280, 262, 278, 308, 312, 216, 264, 212);
            //     */
        }


        void MakeHexacosidedroid(Buffer4 buffer)
        {
            // 4 * 3 * 2 * 2 * 2 = 12 * 6 = 96
            for (int i = 0; i < 12; i += 1)
                for (int x = -1; x <= 1; x += 2)
                    for (int y = -1; y <= 1; y += 2)
                        for (int z = -1; z <= 1; z += 2)
                        {
                            var v = _hexacosidedroidVertices[i];
                            v = Utility.GiveSign(v, x, y, z);
                            buffer.AddVertex(v);
                        }

            // 104 + 8 = 112
            for (int a = 0; a < 4; a += 1)
                for (int b = -1; b <= 1; b += 2)
                {
                    var v = Vector4.zero;
                    v[a] = b * 2;
                    buffer.AddVertex(v);
                }
            // 96 + 8 = 104
            for (int x = -1; x <= 1; x += 2)
                for (int y = -1; y <= 1; y += 2)
                    for (int z = -1; z <= 1; z += 2)
                        for (int w = -1; w <= 1; w += 2)
                        {
                            var v = new Vector4(x, y, z, w);
                            buffer.AddVertex(v);
                        }

            // Normalize
            var _R = radius / 2;
            for (int i = 0; i < buffer.m_VerticesCount; i++)
            {
                buffer.m_Vertices[i] = buffer.m_Vertices[i] * _R;
            }

            // Magic cell-table: http://web.archive.org/web/20091024133911/http://homepages.cwi.nl/~dik/english/mathematics/poly/db/3,3,5/c/s-1.html
            buffer.offset--;
            buffer.AddTrimid(001, 002, 009, 025);
            buffer.AddTrimid(001, 002, 009, 097);
            buffer.AddTrimid(001, 002, 010, 026);
            buffer.AddTrimid(001, 002, 010, 097);
            buffer.AddTrimid(001, 002, 025, 026);
            buffer.AddTrimid(001, 009, 017, 097);
            buffer.AddTrimid(001, 009, 017, 105);
            buffer.AddTrimid(001, 009, 025, 105);
            buffer.AddTrimid(001, 010, 018, 097);
            buffer.AddTrimid(001, 010, 018, 106);
            buffer.AddTrimid(001, 010, 026, 106);
            buffer.AddTrimid(001, 017, 018, 033);
            buffer.AddTrimid(001, 017, 018, 097);
            buffer.AddTrimid(001, 017, 033, 105);
            buffer.AddTrimid(001, 018, 033, 106);
            buffer.AddTrimid(001, 025, 026, 049);
            buffer.AddTrimid(001, 025, 049, 105);
            buffer.AddTrimid(001, 026, 049, 106);
            buffer.AddTrimid(001, 033, 049, 105);
            buffer.AddTrimid(001, 033, 049, 106);
            buffer.AddTrimid(002, 009, 019, 097);
            buffer.AddTrimid(002, 009, 019, 107);
            buffer.AddTrimid(002, 009, 025, 107);
            buffer.AddTrimid(002, 010, 020, 097);
            buffer.AddTrimid(002, 010, 020, 108);
            buffer.AddTrimid(002, 010, 026, 108);
            buffer.AddTrimid(002, 019, 020, 034);
            buffer.AddTrimid(002, 019, 020, 097);
            buffer.AddTrimid(002, 019, 034, 107);
            buffer.AddTrimid(002, 020, 034, 108);
            buffer.AddTrimid(002, 025, 026, 050);
            buffer.AddTrimid(002, 025, 050, 107);
            buffer.AddTrimid(002, 026, 050, 108);
            buffer.AddTrimid(002, 034, 050, 107);
            buffer.AddTrimid(002, 034, 050, 108);
            buffer.AddTrimid(003, 004, 011, 027);
            buffer.AddTrimid(003, 004, 011, 097);
            buffer.AddTrimid(003, 004, 012, 028);
            buffer.AddTrimid(003, 004, 012, 097);
            buffer.AddTrimid(003, 004, 027, 028);
            buffer.AddTrimid(003, 011, 017, 097);
            buffer.AddTrimid(003, 011, 017, 109);
            buffer.AddTrimid(003, 011, 027, 109);
            buffer.AddTrimid(003, 012, 018, 097);
            buffer.AddTrimid(003, 012, 018, 110);
            buffer.AddTrimid(003, 012, 028, 110);
            buffer.AddTrimid(003, 017, 018, 035);
            buffer.AddTrimid(003, 017, 018, 097);
            buffer.AddTrimid(003, 017, 035, 109);
            buffer.AddTrimid(003, 018, 035, 110);
            buffer.AddTrimid(003, 027, 028, 051);
            buffer.AddTrimid(003, 027, 051, 109);
            buffer.AddTrimid(003, 028, 051, 110);
            buffer.AddTrimid(003, 035, 051, 109);
            buffer.AddTrimid(003, 035, 051, 110);
            buffer.AddTrimid(004, 011, 019, 097);
            buffer.AddTrimid(004, 011, 019, 111);
            buffer.AddTrimid(004, 011, 027, 111);
            buffer.AddTrimid(004, 012, 020, 097);
            buffer.AddTrimid(004, 012, 020, 112);
            buffer.AddTrimid(004, 012, 028, 112);
            buffer.AddTrimid(004, 019, 020, 036);
            buffer.AddTrimid(004, 019, 020, 097);
            buffer.AddTrimid(004, 019, 036, 111);
            buffer.AddTrimid(004, 020, 036, 112);
            buffer.AddTrimid(004, 027, 028, 052);
            buffer.AddTrimid(004, 027, 052, 111);
            buffer.AddTrimid(004, 028, 052, 112);
            buffer.AddTrimid(004, 036, 052, 111);
            buffer.AddTrimid(004, 036, 052, 112);
            buffer.AddTrimid(005, 006, 013, 029);
            buffer.AddTrimid(005, 006, 013, 098);
            buffer.AddTrimid(005, 006, 014, 030);
            buffer.AddTrimid(005, 006, 014, 098);
            buffer.AddTrimid(005, 006, 029, 030);
            buffer.AddTrimid(005, 013, 021, 098);
            buffer.AddTrimid(005, 013, 021, 113);
            buffer.AddTrimid(005, 013, 029, 113);
            buffer.AddTrimid(005, 014, 022, 098);
            buffer.AddTrimid(005, 014, 022, 114);
            buffer.AddTrimid(005, 014, 030, 114);
            buffer.AddTrimid(005, 021, 022, 037);
            buffer.AddTrimid(005, 021, 022, 098);
            buffer.AddTrimid(005, 021, 037, 113);
            buffer.AddTrimid(005, 022, 037, 114);
            buffer.AddTrimid(005, 029, 030, 053);
            buffer.AddTrimid(005, 029, 053, 113);
            buffer.AddTrimid(005, 030, 053, 114);
            buffer.AddTrimid(005, 037, 053, 113);
            buffer.AddTrimid(005, 037, 053, 114);
            buffer.AddTrimid(006, 013, 023, 098);
            buffer.AddTrimid(006, 013, 023, 115);
            buffer.AddTrimid(006, 013, 029, 115);
            buffer.AddTrimid(006, 014, 024, 098);
            buffer.AddTrimid(006, 014, 024, 116);
            buffer.AddTrimid(006, 014, 030, 116);
            buffer.AddTrimid(006, 023, 024, 038);
            buffer.AddTrimid(006, 023, 024, 098);
            buffer.AddTrimid(006, 023, 038, 115);
            buffer.AddTrimid(006, 024, 038, 116);
            buffer.AddTrimid(006, 029, 030, 054);
            buffer.AddTrimid(006, 029, 054, 115);
            buffer.AddTrimid(006, 030, 054, 116);
            buffer.AddTrimid(006, 038, 054, 115);
            buffer.AddTrimid(006, 038, 054, 116);
            buffer.AddTrimid(007, 008, 015, 031);
            buffer.AddTrimid(007, 008, 015, 098);
            buffer.AddTrimid(007, 008, 016, 032);
            buffer.AddTrimid(007, 008, 016, 098);
            buffer.AddTrimid(007, 008, 031, 032);
            buffer.AddTrimid(007, 015, 021, 098);
            buffer.AddTrimid(007, 015, 021, 117);
            buffer.AddTrimid(007, 015, 031, 117);
            buffer.AddTrimid(007, 016, 022, 098);
            buffer.AddTrimid(007, 016, 022, 118);
            buffer.AddTrimid(007, 016, 032, 118);
            buffer.AddTrimid(007, 021, 022, 039);
            buffer.AddTrimid(007, 021, 022, 098);
            buffer.AddTrimid(007, 021, 039, 117);
            buffer.AddTrimid(007, 022, 039, 118);
            buffer.AddTrimid(007, 031, 032, 055);
            buffer.AddTrimid(007, 031, 055, 117);
            buffer.AddTrimid(007, 032, 055, 118);
            buffer.AddTrimid(007, 039, 055, 117);
            buffer.AddTrimid(007, 039, 055, 118);
            buffer.AddTrimid(008, 015, 023, 098);
            buffer.AddTrimid(008, 015, 023, 119);
            buffer.AddTrimid(008, 015, 031, 119);
            buffer.AddTrimid(008, 016, 024, 098);
            buffer.AddTrimid(008, 016, 024, 120);
            buffer.AddTrimid(008, 016, 032, 120);
            buffer.AddTrimid(008, 023, 024, 040);
            buffer.AddTrimid(008, 023, 024, 098);
            buffer.AddTrimid(008, 023, 040, 119);
            buffer.AddTrimid(008, 024, 040, 120);
            buffer.AddTrimid(008, 031, 032, 056);
            buffer.AddTrimid(008, 031, 056, 119);
            buffer.AddTrimid(008, 032, 056, 120);
            buffer.AddTrimid(008, 040, 056, 119);
            buffer.AddTrimid(008, 040, 056, 120);
            buffer.AddTrimid(009, 011, 017, 041);
            buffer.AddTrimid(009, 011, 017, 097);
            buffer.AddTrimid(009, 011, 019, 043);
            buffer.AddTrimid(009, 011, 019, 097);
            buffer.AddTrimid(009, 011, 041, 043);
            buffer.AddTrimid(009, 017, 041, 105);
            buffer.AddTrimid(009, 019, 043, 107);
            buffer.AddTrimid(009, 025, 057, 105);
            buffer.AddTrimid(009, 025, 057, 107);
            buffer.AddTrimid(009, 041, 043, 057);
            buffer.AddTrimid(009, 041, 057, 105);
            buffer.AddTrimid(009, 043, 057, 107);
            buffer.AddTrimid(010, 012, 018, 042);
            buffer.AddTrimid(010, 012, 018, 097);
            buffer.AddTrimid(010, 012, 020, 044);
            buffer.AddTrimid(010, 012, 020, 097);
            buffer.AddTrimid(010, 012, 042, 044);
            buffer.AddTrimid(010, 018, 042, 106);
            buffer.AddTrimid(010, 020, 044, 108);
            buffer.AddTrimid(010, 026, 058, 106);
            buffer.AddTrimid(010, 026, 058, 108);
            buffer.AddTrimid(010, 042, 044, 058);
            buffer.AddTrimid(010, 042, 058, 106);
            buffer.AddTrimid(010, 044, 058, 108);
            buffer.AddTrimid(011, 017, 041, 109);
            buffer.AddTrimid(011, 019, 043, 111);
            buffer.AddTrimid(011, 027, 059, 109);
            buffer.AddTrimid(011, 027, 059, 111);
            buffer.AddTrimid(011, 041, 043, 059);
            buffer.AddTrimid(011, 041, 059, 109);
            buffer.AddTrimid(011, 043, 059, 111);
            buffer.AddTrimid(012, 018, 042, 110);
            buffer.AddTrimid(012, 020, 044, 112);
            buffer.AddTrimid(012, 028, 060, 110);
            buffer.AddTrimid(012, 028, 060, 112);
            buffer.AddTrimid(012, 042, 044, 060);
            buffer.AddTrimid(012, 042, 060, 110);
            buffer.AddTrimid(012, 044, 060, 112);
            buffer.AddTrimid(013, 015, 021, 045);
            buffer.AddTrimid(013, 015, 021, 098);
            buffer.AddTrimid(013, 015, 023, 047);
            buffer.AddTrimid(013, 015, 023, 098);
            buffer.AddTrimid(013, 015, 045, 047);
            buffer.AddTrimid(013, 021, 045, 113);
            buffer.AddTrimid(013, 023, 047, 115);
            buffer.AddTrimid(013, 029, 061, 113);
            buffer.AddTrimid(013, 029, 061, 115);
            buffer.AddTrimid(013, 045, 047, 061);
            buffer.AddTrimid(013, 045, 061, 113);
            buffer.AddTrimid(013, 047, 061, 115);
            buffer.AddTrimid(014, 016, 022, 046);
            buffer.AddTrimid(014, 016, 022, 098);
            buffer.AddTrimid(014, 016, 024, 048);
            buffer.AddTrimid(014, 016, 024, 098);
            buffer.AddTrimid(014, 016, 046, 048);
            buffer.AddTrimid(014, 022, 046, 114);
            buffer.AddTrimid(014, 024, 048, 116);
            buffer.AddTrimid(014, 030, 062, 114);
            buffer.AddTrimid(014, 030, 062, 116);
            buffer.AddTrimid(014, 046, 048, 062);
            buffer.AddTrimid(014, 046, 062, 114);
            buffer.AddTrimid(014, 048, 062, 116);
            buffer.AddTrimid(015, 021, 045, 117);
            buffer.AddTrimid(015, 023, 047, 119);
            buffer.AddTrimid(015, 031, 063, 117);
            buffer.AddTrimid(015, 031, 063, 119);
            buffer.AddTrimid(015, 045, 047, 063);
            buffer.AddTrimid(015, 045, 063, 117);
            buffer.AddTrimid(015, 047, 063, 119);
            buffer.AddTrimid(016, 022, 046, 118);
            buffer.AddTrimid(016, 024, 048, 120);
            buffer.AddTrimid(016, 032, 064, 118);
            buffer.AddTrimid(016, 032, 064, 120);
            buffer.AddTrimid(016, 046, 048, 064);
            buffer.AddTrimid(016, 046, 064, 118);
            buffer.AddTrimid(016, 048, 064, 120);
            buffer.AddTrimid(017, 018, 033, 035);
            buffer.AddTrimid(017, 033, 035, 065);
            buffer.AddTrimid(017, 033, 065, 105);
            buffer.AddTrimid(017, 035, 065, 109);
            buffer.AddTrimid(017, 041, 065, 105);
            buffer.AddTrimid(017, 041, 065, 109);
            buffer.AddTrimid(018, 033, 035, 066);
            buffer.AddTrimid(018, 033, 066, 106);
            buffer.AddTrimid(018, 035, 066, 110);
            buffer.AddTrimid(018, 042, 066, 106);
            buffer.AddTrimid(018, 042, 066, 110);
            buffer.AddTrimid(019, 020, 034, 036);
            buffer.AddTrimid(019, 034, 036, 067);
            buffer.AddTrimid(019, 034, 067, 107);
            buffer.AddTrimid(019, 036, 067, 111);
            buffer.AddTrimid(019, 043, 067, 107);
            buffer.AddTrimid(019, 043, 067, 111);
            buffer.AddTrimid(020, 034, 036, 068);
            buffer.AddTrimid(020, 034, 068, 108);
            buffer.AddTrimid(020, 036, 068, 112);
            buffer.AddTrimid(020, 044, 068, 108);
            buffer.AddTrimid(020, 044, 068, 112);
            buffer.AddTrimid(021, 022, 037, 039);
            buffer.AddTrimid(021, 037, 039, 069);
            buffer.AddTrimid(021, 037, 069, 113);
            buffer.AddTrimid(021, 039, 069, 117);
            buffer.AddTrimid(021, 045, 069, 113);
            buffer.AddTrimid(021, 045, 069, 117);
            buffer.AddTrimid(022, 037, 039, 070);
            buffer.AddTrimid(022, 037, 070, 114);
            buffer.AddTrimid(022, 039, 070, 118);
            buffer.AddTrimid(022, 046, 070, 114);
            buffer.AddTrimid(022, 046, 070, 118);
            buffer.AddTrimid(023, 024, 038, 040);
            buffer.AddTrimid(023, 038, 040, 071);
            buffer.AddTrimid(023, 038, 071, 115);
            buffer.AddTrimid(023, 040, 071, 119);
            buffer.AddTrimid(023, 047, 071, 115);
            buffer.AddTrimid(023, 047, 071, 119);
            buffer.AddTrimid(024, 038, 040, 072);
            buffer.AddTrimid(024, 038, 072, 116);
            buffer.AddTrimid(024, 040, 072, 120);
            buffer.AddTrimid(024, 048, 072, 116);
            buffer.AddTrimid(024, 048, 072, 120);
            buffer.AddTrimid(025, 026, 049, 099);
            buffer.AddTrimid(025, 026, 050, 099);
            buffer.AddTrimid(025, 049, 073, 099);
            buffer.AddTrimid(025, 049, 073, 105);
            buffer.AddTrimid(025, 050, 075, 099);
            buffer.AddTrimid(025, 050, 075, 107);
            buffer.AddTrimid(025, 057, 073, 075);
            buffer.AddTrimid(025, 057, 073, 105);
            buffer.AddTrimid(025, 057, 075, 107);
            buffer.AddTrimid(025, 073, 075, 099);
            buffer.AddTrimid(026, 049, 074, 099);
            buffer.AddTrimid(026, 049, 074, 106);
            buffer.AddTrimid(026, 050, 076, 099);
            buffer.AddTrimid(026, 050, 076, 108);
            buffer.AddTrimid(026, 058, 074, 076);
            buffer.AddTrimid(026, 058, 074, 106);
            buffer.AddTrimid(026, 058, 076, 108);
            buffer.AddTrimid(026, 074, 076, 099);
            buffer.AddTrimid(027, 028, 051, 100);
            buffer.AddTrimid(027, 028, 052, 100);
            buffer.AddTrimid(027, 051, 077, 100);
            buffer.AddTrimid(027, 051, 077, 109);
            buffer.AddTrimid(027, 052, 079, 100);
            buffer.AddTrimid(027, 052, 079, 111);
            buffer.AddTrimid(027, 059, 077, 079);
            buffer.AddTrimid(027, 059, 077, 109);
            buffer.AddTrimid(027, 059, 079, 111);
            buffer.AddTrimid(027, 077, 079, 100);
            buffer.AddTrimid(028, 051, 078, 100);
            buffer.AddTrimid(028, 051, 078, 110);
            buffer.AddTrimid(028, 052, 080, 100);
            buffer.AddTrimid(028, 052, 080, 112);
            buffer.AddTrimid(028, 060, 078, 080);
            buffer.AddTrimid(028, 060, 078, 110);
            buffer.AddTrimid(028, 060, 080, 112);
            buffer.AddTrimid(028, 078, 080, 100);
            buffer.AddTrimid(029, 030, 053, 099);
            buffer.AddTrimid(029, 030, 054, 099);
            buffer.AddTrimid(029, 053, 073, 099);
            buffer.AddTrimid(029, 053, 073, 113);
            buffer.AddTrimid(029, 054, 075, 099);
            buffer.AddTrimid(029, 054, 075, 115);
            buffer.AddTrimid(029, 061, 073, 075);
            buffer.AddTrimid(029, 061, 073, 113);
            buffer.AddTrimid(029, 061, 075, 115);
            buffer.AddTrimid(029, 073, 075, 099);
            buffer.AddTrimid(030, 053, 074, 099);
            buffer.AddTrimid(030, 053, 074, 114);
            buffer.AddTrimid(030, 054, 076, 099);
            buffer.AddTrimid(030, 054, 076, 116);
            buffer.AddTrimid(030, 062, 074, 076);
            buffer.AddTrimid(030, 062, 074, 114);
            buffer.AddTrimid(030, 062, 076, 116);
            buffer.AddTrimid(030, 074, 076, 099);
            buffer.AddTrimid(031, 032, 055, 100);
            buffer.AddTrimid(031, 032, 056, 100);
            buffer.AddTrimid(031, 055, 077, 100);
            buffer.AddTrimid(031, 055, 077, 117);
            buffer.AddTrimid(031, 056, 079, 100);
            buffer.AddTrimid(031, 056, 079, 119);
            buffer.AddTrimid(031, 063, 077, 079);
            buffer.AddTrimid(031, 063, 077, 117);
            buffer.AddTrimid(031, 063, 079, 119);
            buffer.AddTrimid(031, 077, 079, 100);
            buffer.AddTrimid(032, 055, 078, 100);
            buffer.AddTrimid(032, 055, 078, 118);
            buffer.AddTrimid(032, 056, 080, 100);
            buffer.AddTrimid(032, 056, 080, 120);
            buffer.AddTrimid(032, 064, 078, 080);
            buffer.AddTrimid(032, 064, 078, 118);
            buffer.AddTrimid(032, 064, 080, 120);
            buffer.AddTrimid(032, 078, 080, 100);
            buffer.AddTrimid(033, 035, 065, 101);
            buffer.AddTrimid(033, 035, 066, 101);
            buffer.AddTrimid(033, 049, 081, 082);
            buffer.AddTrimid(033, 049, 081, 105);
            buffer.AddTrimid(033, 049, 082, 106);
            buffer.AddTrimid(033, 065, 081, 101);
            buffer.AddTrimid(033, 065, 081, 105);
            buffer.AddTrimid(033, 066, 082, 101);
            buffer.AddTrimid(033, 066, 082, 106);
            buffer.AddTrimid(033, 081, 082, 101);
            buffer.AddTrimid(034, 036, 067, 102);
            buffer.AddTrimid(034, 036, 068, 102);
            buffer.AddTrimid(034, 050, 083, 084);
            buffer.AddTrimid(034, 050, 083, 107);
            buffer.AddTrimid(034, 050, 084, 108);
            buffer.AddTrimid(034, 067, 083, 102);
            buffer.AddTrimid(034, 067, 083, 107);
            buffer.AddTrimid(034, 068, 084, 102);
            buffer.AddTrimid(034, 068, 084, 108);
            buffer.AddTrimid(034, 083, 084, 102);
            buffer.AddTrimid(035, 051, 085, 086);
            buffer.AddTrimid(035, 051, 085, 109);
            buffer.AddTrimid(035, 051, 086, 110);
            buffer.AddTrimid(035, 065, 085, 101);
            buffer.AddTrimid(035, 065, 085, 109);
            buffer.AddTrimid(035, 066, 086, 101);
            buffer.AddTrimid(035, 066, 086, 110);
            buffer.AddTrimid(035, 085, 086, 101);
            buffer.AddTrimid(036, 052, 087, 088);
            buffer.AddTrimid(036, 052, 087, 111);
            buffer.AddTrimid(036, 052, 088, 112);
            buffer.AddTrimid(036, 067, 087, 102);
            buffer.AddTrimid(036, 067, 087, 111);
            buffer.AddTrimid(036, 068, 088, 102);
            buffer.AddTrimid(036, 068, 088, 112);
            buffer.AddTrimid(036, 087, 088, 102);
            buffer.AddTrimid(037, 039, 069, 101);
            buffer.AddTrimid(037, 039, 070, 101);
            buffer.AddTrimid(037, 053, 081, 082);
            buffer.AddTrimid(037, 053, 081, 113);
            buffer.AddTrimid(037, 053, 082, 114);
            buffer.AddTrimid(037, 069, 081, 101);
            buffer.AddTrimid(037, 069, 081, 113);
            buffer.AddTrimid(037, 070, 082, 101);
            buffer.AddTrimid(037, 070, 082, 114);
            buffer.AddTrimid(037, 081, 082, 101);
            buffer.AddTrimid(038, 040, 071, 102);
            buffer.AddTrimid(038, 040, 072, 102);
            buffer.AddTrimid(038, 054, 083, 084);
            buffer.AddTrimid(038, 054, 083, 115);
            buffer.AddTrimid(038, 054, 084, 116);
            buffer.AddTrimid(038, 071, 083, 102);
            buffer.AddTrimid(038, 071, 083, 115);
            buffer.AddTrimid(038, 072, 084, 102);
            buffer.AddTrimid(038, 072, 084, 116);
            buffer.AddTrimid(038, 083, 084, 102);
            buffer.AddTrimid(039, 055, 085, 086);
            buffer.AddTrimid(039, 055, 085, 117);
            buffer.AddTrimid(039, 055, 086, 118);
            buffer.AddTrimid(039, 069, 085, 101);
            buffer.AddTrimid(039, 069, 085, 117);
            buffer.AddTrimid(039, 070, 086, 101);
            buffer.AddTrimid(039, 070, 086, 118);
            buffer.AddTrimid(039, 085, 086, 101);
            buffer.AddTrimid(040, 056, 087, 088);
            buffer.AddTrimid(040, 056, 087, 119);
            buffer.AddTrimid(040, 056, 088, 120);
            buffer.AddTrimid(040, 071, 087, 102);
            buffer.AddTrimid(040, 071, 087, 119);
            buffer.AddTrimid(040, 072, 088, 102);
            buffer.AddTrimid(040, 072, 088, 120);
            buffer.AddTrimid(040, 087, 088, 102);
            buffer.AddTrimid(041, 043, 057, 103);
            buffer.AddTrimid(041, 043, 059, 103);
            buffer.AddTrimid(041, 057, 089, 103);
            buffer.AddTrimid(041, 057, 089, 105);
            buffer.AddTrimid(041, 059, 093, 103);
            buffer.AddTrimid(041, 059, 093, 109);
            buffer.AddTrimid(041, 065, 089, 093);
            buffer.AddTrimid(041, 065, 089, 105);
            buffer.AddTrimid(041, 065, 093, 109);
            buffer.AddTrimid(041, 089, 093, 103);
            buffer.AddTrimid(042, 044, 058, 104);
            buffer.AddTrimid(042, 044, 060, 104);
            buffer.AddTrimid(042, 058, 090, 104);
            buffer.AddTrimid(042, 058, 090, 106);
            buffer.AddTrimid(042, 060, 094, 104);
            buffer.AddTrimid(042, 060, 094, 110);
            buffer.AddTrimid(042, 066, 090, 094);
            buffer.AddTrimid(042, 066, 090, 106);
            buffer.AddTrimid(042, 066, 094, 110);
            buffer.AddTrimid(042, 090, 094, 104);
            buffer.AddTrimid(043, 057, 091, 103);
            buffer.AddTrimid(043, 057, 091, 107);
            buffer.AddTrimid(043, 059, 095, 103);
            buffer.AddTrimid(043, 059, 095, 111);
            buffer.AddTrimid(043, 067, 091, 095);
            buffer.AddTrimid(043, 067, 091, 107);
            buffer.AddTrimid(043, 067, 095, 111);
            buffer.AddTrimid(043, 091, 095, 103);
            buffer.AddTrimid(044, 058, 092, 104);
            buffer.AddTrimid(044, 058, 092, 108);
            buffer.AddTrimid(044, 060, 096, 104);
            buffer.AddTrimid(044, 060, 096, 112);
            buffer.AddTrimid(044, 068, 092, 096);
            buffer.AddTrimid(044, 068, 092, 108);
            buffer.AddTrimid(044, 068, 096, 112);
            buffer.AddTrimid(044, 092, 096, 104);
            buffer.AddTrimid(045, 047, 061, 103);
            buffer.AddTrimid(045, 047, 063, 103);
            buffer.AddTrimid(045, 061, 089, 103);
            buffer.AddTrimid(045, 061, 089, 113);
            buffer.AddTrimid(045, 063, 093, 103);
            buffer.AddTrimid(045, 063, 093, 117);
            buffer.AddTrimid(045, 069, 089, 093);
            buffer.AddTrimid(045, 069, 089, 113);
            buffer.AddTrimid(045, 069, 093, 117);
            buffer.AddTrimid(045, 089, 093, 103);
            buffer.AddTrimid(046, 048, 062, 104);
            buffer.AddTrimid(046, 048, 064, 104);
            buffer.AddTrimid(046, 062, 090, 104);
            buffer.AddTrimid(046, 062, 090, 114);
            buffer.AddTrimid(046, 064, 094, 104);
            buffer.AddTrimid(046, 064, 094, 118);
            buffer.AddTrimid(046, 070, 090, 094);
            buffer.AddTrimid(046, 070, 090, 114);
            buffer.AddTrimid(046, 070, 094, 118);
            buffer.AddTrimid(046, 090, 094, 104);
            buffer.AddTrimid(047, 061, 091, 103);
            buffer.AddTrimid(047, 061, 091, 115);
            buffer.AddTrimid(047, 063, 095, 103);
            buffer.AddTrimid(047, 063, 095, 119);
            buffer.AddTrimid(047, 071, 091, 095);
            buffer.AddTrimid(047, 071, 091, 115);
            buffer.AddTrimid(047, 071, 095, 119);
            buffer.AddTrimid(047, 091, 095, 103);
            buffer.AddTrimid(048, 062, 092, 104);
            buffer.AddTrimid(048, 062, 092, 116);
            buffer.AddTrimid(048, 064, 096, 104);
            buffer.AddTrimid(048, 064, 096, 120);
            buffer.AddTrimid(048, 072, 092, 096);
            buffer.AddTrimid(048, 072, 092, 116);
            buffer.AddTrimid(048, 072, 096, 120);
            buffer.AddTrimid(048, 092, 096, 104);
            buffer.AddTrimid(049, 053, 073, 081);
            buffer.AddTrimid(049, 053, 073, 099);
            buffer.AddTrimid(049, 053, 074, 082);
            buffer.AddTrimid(049, 053, 074, 099);
            buffer.AddTrimid(049, 053, 081, 082);
            buffer.AddTrimid(049, 073, 081, 105);
            buffer.AddTrimid(049, 074, 082, 106);
            buffer.AddTrimid(050, 054, 075, 083);
            buffer.AddTrimid(050, 054, 075, 099);
            buffer.AddTrimid(050, 054, 076, 084);
            buffer.AddTrimid(050, 054, 076, 099);
            buffer.AddTrimid(050, 054, 083, 084);
            buffer.AddTrimid(050, 075, 083, 107);
            buffer.AddTrimid(050, 076, 084, 108);
            buffer.AddTrimid(051, 055, 077, 085);
            buffer.AddTrimid(051, 055, 077, 100);
            buffer.AddTrimid(051, 055, 078, 086);
            buffer.AddTrimid(051, 055, 078, 100);
            buffer.AddTrimid(051, 055, 085, 086);
            buffer.AddTrimid(051, 077, 085, 109);
            buffer.AddTrimid(051, 078, 086, 110);
            buffer.AddTrimid(052, 056, 079, 087);
            buffer.AddTrimid(052, 056, 079, 100);
            buffer.AddTrimid(052, 056, 080, 088);
            buffer.AddTrimid(052, 056, 080, 100);
            buffer.AddTrimid(052, 056, 087, 088);
            buffer.AddTrimid(052, 079, 087, 111);
            buffer.AddTrimid(052, 080, 088, 112);
            buffer.AddTrimid(053, 073, 081, 113);
            buffer.AddTrimid(053, 074, 082, 114);
            buffer.AddTrimid(054, 075, 083, 115);
            buffer.AddTrimid(054, 076, 084, 116);
            buffer.AddTrimid(055, 077, 085, 117);
            buffer.AddTrimid(055, 078, 086, 118);
            buffer.AddTrimid(056, 079, 087, 119);
            buffer.AddTrimid(056, 080, 088, 120);
            buffer.AddTrimid(057, 061, 073, 075);
            buffer.AddTrimid(057, 061, 073, 089);
            buffer.AddTrimid(057, 061, 075, 091);
            buffer.AddTrimid(057, 061, 089, 103);
            buffer.AddTrimid(057, 061, 091, 103);
            buffer.AddTrimid(057, 073, 089, 105);
            buffer.AddTrimid(057, 075, 091, 107);
            buffer.AddTrimid(058, 062, 074, 076);
            buffer.AddTrimid(058, 062, 074, 090);
            buffer.AddTrimid(058, 062, 076, 092);
            buffer.AddTrimid(058, 062, 090, 104);
            buffer.AddTrimid(058, 062, 092, 104);
            buffer.AddTrimid(058, 074, 090, 106);
            buffer.AddTrimid(058, 076, 092, 108);
            buffer.AddTrimid(059, 063, 077, 079);
            buffer.AddTrimid(059, 063, 077, 093);
            buffer.AddTrimid(059, 063, 079, 095);
            buffer.AddTrimid(059, 063, 093, 103);
            buffer.AddTrimid(059, 063, 095, 103);
            buffer.AddTrimid(059, 077, 093, 109);
            buffer.AddTrimid(059, 079, 095, 111);
            buffer.AddTrimid(060, 064, 078, 080);
            buffer.AddTrimid(060, 064, 078, 094);
            buffer.AddTrimid(060, 064, 080, 096);
            buffer.AddTrimid(060, 064, 094, 104);
            buffer.AddTrimid(060, 064, 096, 104);
            buffer.AddTrimid(060, 078, 094, 110);
            buffer.AddTrimid(060, 080, 096, 112);
            buffer.AddTrimid(061, 073, 089, 113);
            buffer.AddTrimid(061, 075, 091, 115);
            buffer.AddTrimid(062, 074, 090, 114);
            buffer.AddTrimid(062, 076, 092, 116);
            buffer.AddTrimid(063, 077, 093, 117);
            buffer.AddTrimid(063, 079, 095, 119);
            buffer.AddTrimid(064, 078, 094, 118);
            buffer.AddTrimid(064, 080, 096, 120);
            buffer.AddTrimid(065, 069, 081, 089);
            buffer.AddTrimid(065, 069, 081, 101);
            buffer.AddTrimid(065, 069, 085, 093);
            buffer.AddTrimid(065, 069, 085, 101);
            buffer.AddTrimid(065, 069, 089, 093);
            buffer.AddTrimid(065, 081, 089, 105);
            buffer.AddTrimid(065, 085, 093, 109);
            buffer.AddTrimid(066, 070, 082, 090);
            buffer.AddTrimid(066, 070, 082, 101);
            buffer.AddTrimid(066, 070, 086, 094);
            buffer.AddTrimid(066, 070, 086, 101);
            buffer.AddTrimid(066, 070, 090, 094);
            buffer.AddTrimid(066, 082, 090, 106);
            buffer.AddTrimid(066, 086, 094, 110);
            buffer.AddTrimid(067, 071, 083, 091);
            buffer.AddTrimid(067, 071, 083, 102);
            buffer.AddTrimid(067, 071, 087, 095);
            buffer.AddTrimid(067, 071, 087, 102);
            buffer.AddTrimid(067, 071, 091, 095);
            buffer.AddTrimid(067, 083, 091, 107);
            buffer.AddTrimid(067, 087, 095, 111);
            buffer.AddTrimid(068, 072, 084, 092);
            buffer.AddTrimid(068, 072, 084, 102);
            buffer.AddTrimid(068, 072, 088, 096);
            buffer.AddTrimid(068, 072, 088, 102);
            buffer.AddTrimid(068, 072, 092, 096);
            buffer.AddTrimid(068, 084, 092, 108);
            buffer.AddTrimid(068, 088, 096, 112);
            buffer.AddTrimid(069, 081, 089, 113);
            buffer.AddTrimid(069, 085, 093, 117);
            buffer.AddTrimid(070, 082, 090, 114);
            buffer.AddTrimid(070, 086, 094, 118);
            buffer.AddTrimid(071, 083, 091, 115);
            buffer.AddTrimid(071, 087, 095, 119);
            buffer.AddTrimid(072, 084, 092, 116);
            buffer.AddTrimid(072, 088, 096, 120);
            buffer.AddTrimid(073, 081, 089, 105);
            buffer.AddTrimid(073, 081, 089, 113);
            buffer.AddTrimid(074, 082, 090, 106);
            buffer.AddTrimid(074, 082, 090, 114);
            buffer.AddTrimid(075, 083, 091, 107);
            buffer.AddTrimid(075, 083, 091, 115);
            buffer.AddTrimid(076, 084, 092, 108);
            buffer.AddTrimid(076, 084, 092, 116);
            buffer.AddTrimid(077, 085, 093, 109);
            buffer.AddTrimid(077, 085, 093, 117);
            buffer.AddTrimid(078, 086, 094, 110);
            buffer.AddTrimid(078, 086, 094, 118);
            buffer.AddTrimid(079, 087, 095, 111);
            buffer.AddTrimid(079, 087, 095, 119);
            buffer.AddTrimid(080, 088, 096, 112);
            buffer.AddTrimid(080, 088, 096, 120);
            buffer.offset = 0;
        }

    }
}