using System;

namespace Engine4
{
    /// <summary>
    /// 6x6 matrix to represet Tensor inertia of an object in 4D.
    /// </summary>
    /// <remarks>
    /// Because its immense computation size, this matrix is intended 
    /// only for holding inertia data for the internal physics engine.
    /// The elements is stored in row-major order.
    /// </remarks>
    [Serializable]
    public struct Tensor4
    {
        /// <summary> First row of the matrix </summary>
        public Euler4 ex;
        /// <summary> Second row of the matrix </summary>
        public Euler4 ey;
        /// <summary> Third row of the matrix </summary>
        public Euler4 ez;
        /// <summary> Fourth row of the matrix </summary>
        public Euler4 et;
        /// <summary> Fifth row of the matrix </summary>
        public Euler4 eu;
        /// <summary> Sixth row of the matrix </summary>
        public Euler4 ev;

        /// <summary>
        /// Create a diagonally uniform tensor matrix.
        /// </summary>
        public Tensor4(float scale)
        {
            ex = new Euler4(scale, 0, 0, 0, 0, 0);
            ey = new Euler4(0, scale, 0, 0, 0, 0);
            ez = new Euler4(0, 0, scale, 0, 0, 0);
            et = new Euler4(0, 0, 0, scale, 0, 0);
            eu = new Euler4(0, 0, 0, 0, scale, 0);
            ev = new Euler4(0, 0, 0, 0, 0, scale);
        }

        /// <summary>
        /// Create a diagonal tensor matrix.
        /// </summary>
        public Tensor4(Euler4 scale)
        {
            ex = new Euler4(scale.x, 0, 0, 0, 0, 0);
            ey = new Euler4(0, scale.y, 0, 0, 0, 0);
            ez = new Euler4(0, 0, scale.z, 0, 0, 0);
            et = new Euler4(0, 0, 0, scale.t, 0, 0);
            eu = new Euler4(0, 0, 0, 0, scale.u, 0);
            ev = new Euler4(0, 0, 0, 0, 0, scale.v);
        }

        /// <summary>
        /// Create and assign matrix values with rows.
        /// </summary>
        public Tensor4(Euler4 x, Euler4 y, Euler4 z, Euler4 t, Euler4 u, Euler4 v)
        {
            ex = x;
            ey = y;
            ez = z;
            et = t;
            eu = u;
            ev = v;
        }

        /// <summary>
        /// Get Nth-row of the matrix
        /// </summary>
        public Euler4 this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return ex;
                    case 1: return ey;
                    case 2: return ez;
                    case 3: return et;
                    case 4: return eu;
                    case 5: return ev;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: ex = value; break;
                    case 1: ey = value; break;
                    case 2: ez = value; break;
                    case 3: et = value; break;
                    case 4: eu = value; break;
                    case 5: ev = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Transform euler rotation by the tensor.
        /// </summary>
        static public Euler4 operator *(Tensor4 lhs, Euler4 rhs)
        {
            return new Euler4()
            {
                x = Euler4.Dot(lhs.ex, rhs),
                y = Euler4.Dot(lhs.ey, rhs),
                z = Euler4.Dot(lhs.ez, rhs),
                t = Euler4.Dot(lhs.et, rhs),
                u = Euler4.Dot(lhs.eu, rhs),
                v = Euler4.Dot(lhs.ev, rhs),
            };
        }

        /// <summary>
        /// Combine two tensor.
        /// </summary>
        static public Tensor4 operator *(Tensor4 lhs, Tensor4 rhs)
        {
            return new Tensor4()
            {
                Column0 = (lhs * rhs.Column0),
                Column1 = (lhs * rhs.Column1),
                Column2 = (lhs * rhs.Column2),
                Column3 = (lhs * rhs.Column3),
                Column4 = (lhs * rhs.Column4),
                Column5 = (lhs * rhs.Column5),
            };
        }

        /// <summary>
        /// Scale the tensor.
        /// </summary>
        static public Tensor4 operator *(Tensor4 lhs, float f)
        {
            return new Tensor4()
            {
                ex = (lhs.ex * f),
                ey = (lhs.ey * f),
                ez = (lhs.ez * f),
                et = (lhs.et * f),
                eu = (lhs.eu * f),
                ev = (lhs.ev * f),
            };
        }

        /// <summary>
        /// Scale the tensor.
        /// </summary>
        static public Tensor4 operator +(Tensor4 lhs, Tensor4 rhs)
        {
            return new Tensor4()
            {
                ex = (lhs.ex + rhs.ex),
                ey = (lhs.ey + rhs.ey),
                ez = (lhs.ez + rhs.ez),
                et = (lhs.et + rhs.et),
                eu = (lhs.eu + rhs.eu),
                ev = (lhs.ev + rhs.ev),
            };
        }

        /// <summary>
        /// Element-wisely subract two tensor.
        /// </summary>
        static public Tensor4 operator -(Tensor4 lhs, Tensor4 rhs)
        {
            return new Tensor4()
            {
                ex = (lhs.ex - rhs.ex),
                ey = (lhs.ey - rhs.ey),
                ez = (lhs.ez - rhs.ez),
                et = (lhs.et - rhs.et),
                eu = (lhs.eu - rhs.eu),
                ev = (lhs.ev - rhs.ev),
            };
        }

        /// <summary>
        /// Create inertia from rotation matrix.
        /// </summary>
        public static Tensor4 Cross(Matrix4 t)
        {
            return new Tensor4(
                Euler4.Cross(t.ey, t.ez),
                Euler4.Cross(t.ez, t.ex),
                Euler4.Cross(t.ex, t.ey),
                Euler4.Cross(t.ex, t.ew),
                Euler4.Cross(t.ey, t.ew),
                Euler4.Cross(t.ez, t.ew)
                );
        }

        /// <summary>
        /// Rotate the tensor orientation by the matrix
        /// </summary>
        public static Tensor4 Transform(Tensor4 I, Matrix4 r)
        {
            var t = Cross(r);
            return t * I * Transpose(t);
        }

        /// <summary>
        /// Create a tensor translation.
        /// </summary>
        public static Tensor4 Transform(Vector4 p, float mass)
        {
            return new Tensor4(Vector4.LengthSq(p)) - OuterProduct(p, p) * mass;
        }

        /// <summary>
        /// Transform the inertia.
        /// </summary>
        public static Tensor4 Transform(Tensor4 I, Matrix4x5 T, float mass)
        {
            return Transform(I, T.rotation) + Transform(T.position, mass);
        }

        /// <summary>
        /// Transpose the tensor.
        /// </summary>
        public static Tensor4 Transpose(Tensor4 m)
        {
            return new Tensor4()
            {
                ex = m.Column(0),
                ey = m.Column(1),
                ez = m.Column(2),
                et = m.Column(3),
                eu = m.Column(4),
                ev = m.Column(5),
            };
        }

        /// <summary>
        /// Create a tensor with zero values.
        /// </summary>
        public static Tensor4 zero { get { return new Tensor4(); } }

        /// <summary>
        /// Create a tensor as an identity matrix.
        /// </summary>
        public static Tensor4 identity { get { return new Tensor4(1); } }

        /// <summary>
        /// ? (WIP)
        /// </summary>
        public static Tensor4 OuterProduct(Vector4 u, Vector4 v)
        {
            //Vector4 a = v * u.x;
            //Vector4 b = v * u.y;
            //Vector4 c = v * u.z;

            return new Tensor4();
        }

        /// <summary>
        /// Inverse the tensor.
        /// </summary>
        /// <remarks>
        /// The operation is working, but unoptimized, yet expensive.
        /// </remarks>
        public static Tensor4 Inverse(Tensor4 m)
        {
            // Made by my awesome bot github.com/willnode/Matrix-N-Programmer
            // TODO: Make the bot smarter again, possibly use Cross(x) operation

            var A4545 = m.eu.u * m.ev.v - m.eu.v * m.ev.u;
            var A3545 = m.eu.t * m.ev.v - m.eu.v * m.ev.t;
            var A3445 = m.eu.t * m.ev.u - m.eu.u * m.ev.t;
            var A2545 = m.eu.z * m.ev.v - m.eu.v * m.ev.z;
            var A2445 = m.eu.z * m.ev.u - m.eu.u * m.ev.z;
            var A2345 = m.eu.z * m.ev.t - m.eu.t * m.ev.z;
            var A1545 = m.eu.y * m.ev.v - m.eu.v * m.ev.y;
            var A1445 = m.eu.y * m.ev.u - m.eu.u * m.ev.y;
            var A1345 = m.eu.y * m.ev.t - m.eu.t * m.ev.y;
            var A1245 = m.eu.y * m.ev.z - m.eu.z * m.ev.y;
            var A0545 = m.eu.x * m.ev.v - m.eu.v * m.ev.x;
            var A0445 = m.eu.x * m.ev.u - m.eu.u * m.ev.x;
            var A0345 = m.eu.x * m.ev.t - m.eu.t * m.ev.x;
            var A0245 = m.eu.x * m.ev.z - m.eu.z * m.ev.x;
            var A0145 = m.eu.x * m.ev.y - m.eu.y * m.ev.x;
            var A4535 = m.et.u * m.ev.v - m.et.v * m.ev.u;
            var A3535 = m.et.t * m.ev.v - m.et.v * m.ev.t;
            var A3435 = m.et.t * m.ev.u - m.et.u * m.ev.t;
            var A2535 = m.et.z * m.ev.v - m.et.v * m.ev.z;
            var A2435 = m.et.z * m.ev.u - m.et.u * m.ev.z;
            var A2335 = m.et.z * m.ev.t - m.et.t * m.ev.z;
            var A1535 = m.et.y * m.ev.v - m.et.v * m.ev.y;
            var A1435 = m.et.y * m.ev.u - m.et.u * m.ev.y;
            var A1335 = m.et.y * m.ev.t - m.et.t * m.ev.y;
            var A1235 = m.et.y * m.ev.z - m.et.z * m.ev.y;
            var A4534 = m.et.u * m.eu.v - m.et.v * m.eu.u;
            var A3534 = m.et.t * m.eu.v - m.et.v * m.eu.t;
            var A3434 = m.et.t * m.eu.u - m.et.u * m.eu.t;
            var A2534 = m.et.z * m.eu.v - m.et.v * m.eu.z;
            var A2434 = m.et.z * m.eu.u - m.et.u * m.eu.z;
            var A2334 = m.et.z * m.eu.t - m.et.t * m.eu.z;
            var A1534 = m.et.y * m.eu.v - m.et.v * m.eu.y;
            var A1434 = m.et.y * m.eu.u - m.et.u * m.eu.y;
            var A1334 = m.et.y * m.eu.t - m.et.t * m.eu.y;
            var A1234 = m.et.y * m.eu.z - m.et.z * m.eu.y;
            var A0535 = m.et.x * m.ev.v - m.et.v * m.ev.x;
            var A0435 = m.et.x * m.ev.u - m.et.u * m.ev.x;
            var A0335 = m.et.x * m.ev.t - m.et.t * m.ev.x;
            var A0235 = m.et.x * m.ev.z - m.et.z * m.ev.x;
            var A0534 = m.et.x * m.eu.v - m.et.v * m.eu.x;
            var A0434 = m.et.x * m.eu.u - m.et.u * m.eu.x;
            var A0334 = m.et.x * m.eu.t - m.et.t * m.eu.x;
            var A0234 = m.et.x * m.eu.z - m.et.z * m.eu.x;
            var A0135 = m.et.x * m.ev.y - m.et.y * m.ev.x;
            var A0134 = m.et.x * m.eu.y - m.et.y * m.eu.x;

            var B345345 = m.et.t * A4545 - m.et.u * A3545 + m.et.v * A3445;
            var B245345 = m.et.z * A4545 - m.et.u * A2545 + m.et.v * A2445;
            var B235345 = m.et.z * A3545 - m.et.t * A2545 + m.et.v * A2345;
            var B234345 = m.et.z * A3445 - m.et.t * A2445 + m.et.u * A2345;
            var B145345 = m.et.y * A4545 - m.et.u * A1545 + m.et.v * A1445;
            var B135345 = m.et.y * A3545 - m.et.t * A1545 + m.et.v * A1345;
            var B134345 = m.et.y * A3445 - m.et.t * A1445 + m.et.u * A1345;
            var B125345 = m.et.y * A2545 - m.et.z * A1545 + m.et.v * A1245;
            var B124345 = m.et.y * A2445 - m.et.z * A1445 + m.et.u * A1245;
            var B123345 = m.et.y * A2345 - m.et.z * A1345 + m.et.t * A1245;
            var B045345 = m.et.x * A4545 - m.et.u * A0545 + m.et.v * A0445;
            var B035345 = m.et.x * A3545 - m.et.t * A0545 + m.et.v * A0345;
            var B034345 = m.et.x * A3445 - m.et.t * A0445 + m.et.u * A0345;
            var B025345 = m.et.x * A2545 - m.et.z * A0545 + m.et.v * A0245;
            var B024345 = m.et.x * A2445 - m.et.z * A0445 + m.et.u * A0245;
            var B023345 = m.et.x * A2345 - m.et.z * A0345 + m.et.t * A0245;
            var B015345 = m.et.x * A1545 - m.et.y * A0545 + m.et.v * A0145;
            var B014345 = m.et.x * A1445 - m.et.y * A0445 + m.et.u * A0145;
            var B013345 = m.et.x * A1345 - m.et.y * A0345 + m.et.t * A0145;
            var B012345 = m.et.x * A1245 - m.et.y * A0245 + m.et.z * A0145;
            var B345245 = m.ez.t * A4545 - m.ez.u * A3545 + m.ez.v * A3445;
            var B245245 = m.ez.z * A4545 - m.ez.u * A2545 + m.ez.v * A2445;
            var B235245 = m.ez.z * A3545 - m.ez.t * A2545 + m.ez.v * A2345;
            var B234245 = m.ez.z * A3445 - m.ez.t * A2445 + m.ez.u * A2345;
            var B145245 = m.ez.y * A4545 - m.ez.u * A1545 + m.ez.v * A1445;
            var B135245 = m.ez.y * A3545 - m.ez.t * A1545 + m.ez.v * A1345;
            var B134245 = m.ez.y * A3445 - m.ez.t * A1445 + m.ez.u * A1345;
            var B125245 = m.ez.y * A2545 - m.ez.z * A1545 + m.ez.v * A1245;
            var B124245 = m.ez.y * A2445 - m.ez.z * A1445 + m.ez.u * A1245;
            var B123245 = m.ez.y * A2345 - m.ez.z * A1345 + m.ez.t * A1245;
            var B345235 = m.ez.t * A4535 - m.ez.u * A3535 + m.ez.v * A3435;
            var B245235 = m.ez.z * A4535 - m.ez.u * A2535 + m.ez.v * A2435;
            var B235235 = m.ez.z * A3535 - m.ez.t * A2535 + m.ez.v * A2335;
            var B234235 = m.ez.z * A3435 - m.ez.t * A2435 + m.ez.u * A2335;
            var B145235 = m.ez.y * A4535 - m.ez.u * A1535 + m.ez.v * A1435;
            var B135235 = m.ez.y * A3535 - m.ez.t * A1535 + m.ez.v * A1335;
            var B134235 = m.ez.y * A3435 - m.ez.t * A1435 + m.ez.u * A1335;
            var B125235 = m.ez.y * A2535 - m.ez.z * A1535 + m.ez.v * A1235;
            var B124235 = m.ez.y * A2435 - m.ez.z * A1435 + m.ez.u * A1235;
            var B123235 = m.ez.y * A2335 - m.ez.z * A1335 + m.ez.t * A1235;
            var B345234 = m.ez.t * A4534 - m.ez.u * A3534 + m.ez.v * A3434;
            var B245234 = m.ez.z * A4534 - m.ez.u * A2534 + m.ez.v * A2434;
            var B235234 = m.ez.z * A3534 - m.ez.t * A2534 + m.ez.v * A2334;
            var B234234 = m.ez.z * A3434 - m.ez.t * A2434 + m.ez.u * A2334;
            var B145234 = m.ez.y * A4534 - m.ez.u * A1534 + m.ez.v * A1434;
            var B135234 = m.ez.y * A3534 - m.ez.t * A1534 + m.ez.v * A1334;
            var B134234 = m.ez.y * A3434 - m.ez.t * A1434 + m.ez.u * A1334;
            var B125234 = m.ez.y * A2534 - m.ez.z * A1534 + m.ez.v * A1234;
            var B124234 = m.ez.y * A2434 - m.ez.z * A1434 + m.ez.u * A1234;
            var B123234 = m.ez.y * A2334 - m.ez.z * A1334 + m.ez.t * A1234;
            var B045245 = m.ez.x * A4545 - m.ez.u * A0545 + m.ez.v * A0445;
            var B035245 = m.ez.x * A3545 - m.ez.t * A0545 + m.ez.v * A0345;
            var B034245 = m.ez.x * A3445 - m.ez.t * A0445 + m.ez.u * A0345;
            var B025245 = m.ez.x * A2545 - m.ez.z * A0545 + m.ez.v * A0245;
            var B024245 = m.ez.x * A2445 - m.ez.z * A0445 + m.ez.u * A0245;
            var B023245 = m.ez.x * A2345 - m.ez.z * A0345 + m.ez.t * A0245;
            var B045235 = m.ez.x * A4535 - m.ez.u * A0535 + m.ez.v * A0435;
            var B035235 = m.ez.x * A3535 - m.ez.t * A0535 + m.ez.v * A0335;
            var B034235 = m.ez.x * A3435 - m.ez.t * A0435 + m.ez.u * A0335;
            var B025235 = m.ez.x * A2535 - m.ez.z * A0535 + m.ez.v * A0235;
            var B024235 = m.ez.x * A2435 - m.ez.z * A0435 + m.ez.u * A0235;
            var B023235 = m.ez.x * A2335 - m.ez.z * A0335 + m.ez.t * A0235;
            var B045234 = m.ez.x * A4534 - m.ez.u * A0534 + m.ez.v * A0434;
            var B035234 = m.ez.x * A3534 - m.ez.t * A0534 + m.ez.v * A0334;
            var B034234 = m.ez.x * A3434 - m.ez.t * A0434 + m.ez.u * A0334;
            var B025234 = m.ez.x * A2534 - m.ez.z * A0534 + m.ez.v * A0234;
            var B024234 = m.ez.x * A2434 - m.ez.z * A0434 + m.ez.u * A0234;
            var B023234 = m.ez.x * A2334 - m.ez.z * A0334 + m.ez.t * A0234;
            var B015245 = m.ez.x * A1545 - m.ez.y * A0545 + m.ez.v * A0145;
            var B014245 = m.ez.x * A1445 - m.ez.y * A0445 + m.ez.u * A0145;
            var B013245 = m.ez.x * A1345 - m.ez.y * A0345 + m.ez.t * A0145;
            var B015235 = m.ez.x * A1535 - m.ez.y * A0535 + m.ez.v * A0135;
            var B014235 = m.ez.x * A1435 - m.ez.y * A0435 + m.ez.u * A0135;
            var B013235 = m.ez.x * A1335 - m.ez.y * A0335 + m.ez.t * A0135;
            var B015234 = m.ez.x * A1534 - m.ez.y * A0534 + m.ez.v * A0134;
            var B014234 = m.ez.x * A1434 - m.ez.y * A0434 + m.ez.u * A0134;
            var B013234 = m.ez.x * A1334 - m.ez.y * A0334 + m.ez.t * A0134;
            var B012245 = m.ez.x * A1245 - m.ez.y * A0245 + m.ez.z * A0145;
            var B012235 = m.ez.x * A1235 - m.ez.y * A0235 + m.ez.z * A0135;
            var B012234 = m.ez.x * A1234 - m.ez.y * A0234 + m.ez.z * A0134;

            var C23452345 = m.ez.z * B345345 - m.ez.t * B245345 + m.ez.u * B235345 - m.ez.v * B234345;
            var C13452345 = m.ez.y * B345345 - m.ez.t * B145345 + m.ez.u * B135345 - m.ez.v * B134345;
            var C12452345 = m.ez.y * B245345 - m.ez.z * B145345 + m.ez.u * B125345 - m.ez.v * B124345;
            var C12352345 = m.ez.y * B235345 - m.ez.z * B135345 + m.ez.t * B125345 - m.ez.v * B123345;
            var C12342345 = m.ez.y * B234345 - m.ez.z * B134345 + m.ez.t * B124345 - m.ez.u * B123345;
            var C03452345 = m.ez.x * B345345 - m.ez.t * B045345 + m.ez.u * B035345 - m.ez.v * B034345;
            var C02452345 = m.ez.x * B245345 - m.ez.z * B045345 + m.ez.u * B025345 - m.ez.v * B024345;
            var C02352345 = m.ez.x * B235345 - m.ez.z * B035345 + m.ez.t * B025345 - m.ez.v * B023345;
            var C02342345 = m.ez.x * B234345 - m.ez.z * B034345 + m.ez.t * B024345 - m.ez.u * B023345;
            var C01452345 = m.ez.x * B145345 - m.ez.y * B045345 + m.ez.u * B015345 - m.ez.v * B014345;
            var C01352345 = m.ez.x * B135345 - m.ez.y * B035345 + m.ez.t * B015345 - m.ez.v * B013345;
            var C01342345 = m.ez.x * B134345 - m.ez.y * B034345 + m.ez.t * B014345 - m.ez.u * B013345;
            var C01252345 = m.ez.x * B125345 - m.ez.y * B025345 + m.ez.z * B015345 - m.ez.v * B012345;
            var C01242345 = m.ez.x * B124345 - m.ez.y * B024345 + m.ez.z * B014345 - m.ez.u * B012345;
            var C01232345 = m.ez.x * B123345 - m.ez.y * B023345 + m.ez.z * B013345 - m.ez.t * B012345;
            var C23451345 = m.ey.z * B345345 - m.ey.t * B245345 + m.ey.u * B235345 - m.ey.v * B234345;
            var C13451345 = m.ey.y * B345345 - m.ey.t * B145345 + m.ey.u * B135345 - m.ey.v * B134345;
            var C12451345 = m.ey.y * B245345 - m.ey.z * B145345 + m.ey.u * B125345 - m.ey.v * B124345;
            var C12351345 = m.ey.y * B235345 - m.ey.z * B135345 + m.ey.t * B125345 - m.ey.v * B123345;
            var C12341345 = m.ey.y * B234345 - m.ey.z * B134345 + m.ey.t * B124345 - m.ey.u * B123345;
            var C23451245 = m.ey.z * B345245 - m.ey.t * B245245 + m.ey.u * B235245 - m.ey.v * B234245;
            var C13451245 = m.ey.y * B345245 - m.ey.t * B145245 + m.ey.u * B135245 - m.ey.v * B134245;
            var C12451245 = m.ey.y * B245245 - m.ey.z * B145245 + m.ey.u * B125245 - m.ey.v * B124245;
            var C12351245 = m.ey.y * B235245 - m.ey.z * B135245 + m.ey.t * B125245 - m.ey.v * B123245;
            var C12341245 = m.ey.y * B234245 - m.ey.z * B134245 + m.ey.t * B124245 - m.ey.u * B123245;
            var C23451235 = m.ey.z * B345235 - m.ey.t * B245235 + m.ey.u * B235235 - m.ey.v * B234235;
            var C13451235 = m.ey.y * B345235 - m.ey.t * B145235 + m.ey.u * B135235 - m.ey.v * B134235;
            var C12451235 = m.ey.y * B245235 - m.ey.z * B145235 + m.ey.u * B125235 - m.ey.v * B124235;
            var C12351235 = m.ey.y * B235235 - m.ey.z * B135235 + m.ey.t * B125235 - m.ey.v * B123235;
            var C12341235 = m.ey.y * B234235 - m.ey.z * B134235 + m.ey.t * B124235 - m.ey.u * B123235;
            var C23451234 = m.ey.z * B345234 - m.ey.t * B245234 + m.ey.u * B235234 - m.ey.v * B234234;
            var C13451234 = m.ey.y * B345234 - m.ey.t * B145234 + m.ey.u * B135234 - m.ey.v * B134234;
            var C12451234 = m.ey.y * B245234 - m.ey.z * B145234 + m.ey.u * B125234 - m.ey.v * B124234;
            var C12351234 = m.ey.y * B235234 - m.ey.z * B135234 + m.ey.t * B125234 - m.ey.v * B123234;
            var C12341234 = m.ey.y * B234234 - m.ey.z * B134234 + m.ey.t * B124234 - m.ey.u * B123234;
            var C03451345 = m.ey.x * B345345 - m.ey.t * B045345 + m.ey.u * B035345 - m.ey.v * B034345;
            var C02451345 = m.ey.x * B245345 - m.ey.z * B045345 + m.ey.u * B025345 - m.ey.v * B024345;
            var C02351345 = m.ey.x * B235345 - m.ey.z * B035345 + m.ey.t * B025345 - m.ey.v * B023345;
            var C02341345 = m.ey.x * B234345 - m.ey.z * B034345 + m.ey.t * B024345 - m.ey.u * B023345;
            var C03451245 = m.ey.x * B345245 - m.ey.t * B045245 + m.ey.u * B035245 - m.ey.v * B034245;
            var C02451245 = m.ey.x * B245245 - m.ey.z * B045245 + m.ey.u * B025245 - m.ey.v * B024245;
            var C02351245 = m.ey.x * B235245 - m.ey.z * B035245 + m.ey.t * B025245 - m.ey.v * B023245;
            var C02341245 = m.ey.x * B234245 - m.ey.z * B034245 + m.ey.t * B024245 - m.ey.u * B023245;
            var C03451235 = m.ey.x * B345235 - m.ey.t * B045235 + m.ey.u * B035235 - m.ey.v * B034235;
            var C02451235 = m.ey.x * B245235 - m.ey.z * B045235 + m.ey.u * B025235 - m.ey.v * B024235;
            var C02351235 = m.ey.x * B235235 - m.ey.z * B035235 + m.ey.t * B025235 - m.ey.v * B023235;
            var C02341235 = m.ey.x * B234235 - m.ey.z * B034235 + m.ey.t * B024235 - m.ey.u * B023235;
            var C03451234 = m.ey.x * B345234 - m.ey.t * B045234 + m.ey.u * B035234 - m.ey.v * B034234;
            var C02451234 = m.ey.x * B245234 - m.ey.z * B045234 + m.ey.u * B025234 - m.ey.v * B024234;
            var C02351234 = m.ey.x * B235234 - m.ey.z * B035234 + m.ey.t * B025234 - m.ey.v * B023234;
            var C02341234 = m.ey.x * B234234 - m.ey.z * B034234 + m.ey.t * B024234 - m.ey.u * B023234;
            var C01451345 = m.ey.x * B145345 - m.ey.y * B045345 + m.ey.u * B015345 - m.ey.v * B014345;
            var C01351345 = m.ey.x * B135345 - m.ey.y * B035345 + m.ey.t * B015345 - m.ey.v * B013345;
            var C01341345 = m.ey.x * B134345 - m.ey.y * B034345 + m.ey.t * B014345 - m.ey.u * B013345;
            var C01451245 = m.ey.x * B145245 - m.ey.y * B045245 + m.ey.u * B015245 - m.ey.v * B014245;
            var C01351245 = m.ey.x * B135245 - m.ey.y * B035245 + m.ey.t * B015245 - m.ey.v * B013245;
            var C01341245 = m.ey.x * B134245 - m.ey.y * B034245 + m.ey.t * B014245 - m.ey.u * B013245;
            var C01451235 = m.ey.x * B145235 - m.ey.y * B045235 + m.ey.u * B015235 - m.ey.v * B014235;
            var C01351235 = m.ey.x * B135235 - m.ey.y * B035235 + m.ey.t * B015235 - m.ey.v * B013235;
            var C01341235 = m.ey.x * B134235 - m.ey.y * B034235 + m.ey.t * B014235 - m.ey.u * B013235;
            var C01451234 = m.ey.x * B145234 - m.ey.y * B045234 + m.ey.u * B015234 - m.ey.v * B014234;
            var C01351234 = m.ey.x * B135234 - m.ey.y * B035234 + m.ey.t * B015234 - m.ey.v * B013234;
            var C01341234 = m.ey.x * B134234 - m.ey.y * B034234 + m.ey.t * B014234 - m.ey.u * B013234;
            var C01251345 = m.ey.x * B125345 - m.ey.y * B025345 + m.ey.z * B015345 - m.ey.v * B012345;
            var C01241345 = m.ey.x * B124345 - m.ey.y * B024345 + m.ey.z * B014345 - m.ey.u * B012345;
            var C01251245 = m.ey.x * B125245 - m.ey.y * B025245 + m.ey.z * B015245 - m.ey.v * B012245;
            var C01241245 = m.ey.x * B124245 - m.ey.y * B024245 + m.ey.z * B014245 - m.ey.u * B012245;
            var C01251235 = m.ey.x * B125235 - m.ey.y * B025235 + m.ey.z * B015235 - m.ey.v * B012235;
            var C01241235 = m.ey.x * B124235 - m.ey.y * B024235 + m.ey.z * B014235 - m.ey.u * B012235;
            var C01251234 = m.ey.x * B125234 - m.ey.y * B025234 + m.ey.z * B015234 - m.ey.v * B012234;
            var C01241234 = m.ey.x * B124234 - m.ey.y * B024234 + m.ey.z * B014234 - m.ey.u * B012234;
            var C01231345 = m.ey.x * B123345 - m.ey.y * B023345 + m.ey.z * B013345 - m.ey.t * B012345;
            var C01231245 = m.ey.x * B123245 - m.ey.y * B023245 + m.ey.z * B013245 - m.ey.t * B012245;
            var C01231235 = m.ey.x * B123235 - m.ey.y * B023235 + m.ey.z * B013235 - m.ey.t * B012235;
            var C01231234 = m.ey.x * B123234 - m.ey.y * B023234 + m.ey.z * B013234 - m.ey.t * B012234;

            var det = m.ex.x * (m.ey.y * C23452345 - m.ey.z * C13452345 + m.ey.t * C12452345 - m.ey.u * C12352345 + m.ey.v * C12342345)
                    - m.ex.y * (m.ey.x * C23452345 - m.ey.z * C03452345 + m.ey.t * C02452345 - m.ey.u * C02352345 + m.ey.v * C02342345)
                    + m.ex.z * (m.ey.x * C13452345 - m.ey.y * C03452345 + m.ey.t * C01452345 - m.ey.u * C01352345 + m.ey.v * C01342345)
                    - m.ex.t * (m.ey.x * C12452345 - m.ey.y * C02452345 + m.ey.z * C01452345 - m.ey.u * C01252345 + m.ey.v * C01242345)
                    + m.ex.u * (m.ey.x * C12352345 - m.ey.y * C02352345 + m.ey.z * C01352345 - m.ey.t * C01252345 + m.ey.v * C01232345)
                    - m.ex.v * (m.ey.x * C12342345 - m.ey.y * C02342345 + m.ey.z * C01342345 - m.ey.t * C01242345 + m.ey.u * C01232345);

            det = 1 / det;

            var v = new Tensor4();
            v.ex.x = det * (m.ey.y * C23452345 - m.ey.z * C13452345 + m.ey.t * C12452345 - m.ey.u * C12352345 + m.ey.v * C12342345);
            v.ex.y = det * -(m.ex.y * C23452345 - m.ex.z * C13452345 + m.ex.t * C12452345 - m.ex.u * C12352345 + m.ex.v * C12342345);
            v.ex.z = det * (m.ex.y * C23451345 - m.ex.z * C13451345 + m.ex.t * C12451345 - m.ex.u * C12351345 + m.ex.v * C12341345);
            v.ex.t = det * -(m.ex.y * C23451245 - m.ex.z * C13451245 + m.ex.t * C12451245 - m.ex.u * C12351245 + m.ex.v * C12341245);
            v.ex.u = det * (m.ex.y * C23451235 - m.ex.z * C13451235 + m.ex.t * C12451235 - m.ex.u * C12351235 + m.ex.v * C12341235);
            v.ex.v = det * -(m.ex.y * C23451234 - m.ex.z * C13451234 + m.ex.t * C12451234 - m.ex.u * C12351234 + m.ex.v * C12341234);
            v.ey.x = det * -(m.ey.x * C23452345 - m.ey.z * C03452345 + m.ey.t * C02452345 - m.ey.u * C02352345 + m.ey.v * C02342345);
            v.ey.y = det * (m.ex.x * C23452345 - m.ex.z * C03452345 + m.ex.t * C02452345 - m.ex.u * C02352345 + m.ex.v * C02342345);
            v.ey.z = det * -(m.ex.x * C23451345 - m.ex.z * C03451345 + m.ex.t * C02451345 - m.ex.u * C02351345 + m.ex.v * C02341345);
            v.ey.t = det * (m.ex.x * C23451245 - m.ex.z * C03451245 + m.ex.t * C02451245 - m.ex.u * C02351245 + m.ex.v * C02341245);
            v.ey.u = det * -(m.ex.x * C23451235 - m.ex.z * C03451235 + m.ex.t * C02451235 - m.ex.u * C02351235 + m.ex.v * C02341235);
            v.ey.v = det * (m.ex.x * C23451234 - m.ex.z * C03451234 + m.ex.t * C02451234 - m.ex.u * C02351234 + m.ex.v * C02341234);
            v.ez.x = det * (m.ey.x * C13452345 - m.ey.y * C03452345 + m.ey.t * C01452345 - m.ey.u * C01352345 + m.ey.v * C01342345);
            v.ez.y = det * -(m.ex.x * C13452345 - m.ex.y * C03452345 + m.ex.t * C01452345 - m.ex.u * C01352345 + m.ex.v * C01342345);
            v.ez.z = det * (m.ex.x * C13451345 - m.ex.y * C03451345 + m.ex.t * C01451345 - m.ex.u * C01351345 + m.ex.v * C01341345);
            v.ez.t = det * -(m.ex.x * C13451245 - m.ex.y * C03451245 + m.ex.t * C01451245 - m.ex.u * C01351245 + m.ex.v * C01341245);
            v.ez.u = det * (m.ex.x * C13451235 - m.ex.y * C03451235 + m.ex.t * C01451235 - m.ex.u * C01351235 + m.ex.v * C01341235);
            v.ez.v = det * -(m.ex.x * C13451234 - m.ex.y * C03451234 + m.ex.t * C01451234 - m.ex.u * C01351234 + m.ex.v * C01341234);
            v.et.x = det * -(m.ey.x * C12452345 - m.ey.y * C02452345 + m.ey.z * C01452345 - m.ey.u * C01252345 + m.ey.v * C01242345);
            v.et.y = det * (m.ex.x * C12452345 - m.ex.y * C02452345 + m.ex.z * C01452345 - m.ex.u * C01252345 + m.ex.v * C01242345);
            v.et.z = det * -(m.ex.x * C12451345 - m.ex.y * C02451345 + m.ex.z * C01451345 - m.ex.u * C01251345 + m.ex.v * C01241345);
            v.et.t = det * (m.ex.x * C12451245 - m.ex.y * C02451245 + m.ex.z * C01451245 - m.ex.u * C01251245 + m.ex.v * C01241245);
            v.et.u = det * -(m.ex.x * C12451235 - m.ex.y * C02451235 + m.ex.z * C01451235 - m.ex.u * C01251235 + m.ex.v * C01241235);
            v.et.v = det * (m.ex.x * C12451234 - m.ex.y * C02451234 + m.ex.z * C01451234 - m.ex.u * C01251234 + m.ex.v * C01241234);
            v.eu.x = det * (m.ey.x * C12352345 - m.ey.y * C02352345 + m.ey.z * C01352345 - m.ey.t * C01252345 + m.ey.v * C01232345);
            v.eu.y = det * -(m.ex.x * C12352345 - m.ex.y * C02352345 + m.ex.z * C01352345 - m.ex.t * C01252345 + m.ex.v * C01232345);
            v.eu.z = det * (m.ex.x * C12351345 - m.ex.y * C02351345 + m.ex.z * C01351345 - m.ex.t * C01251345 + m.ex.v * C01231345);
            v.eu.t = det * -(m.ex.x * C12351245 - m.ex.y * C02351245 + m.ex.z * C01351245 - m.ex.t * C01251245 + m.ex.v * C01231245);
            v.eu.u = det * (m.ex.x * C12351235 - m.ex.y * C02351235 + m.ex.z * C01351235 - m.ex.t * C01251235 + m.ex.v * C01231235);
            v.eu.v = det * -(m.ex.x * C12351234 - m.ex.y * C02351234 + m.ex.z * C01351234 - m.ex.t * C01251234 + m.ex.v * C01231234);
            v.ev.x = det * -(m.ey.x * C12342345 - m.ey.y * C02342345 + m.ey.z * C01342345 - m.ey.t * C01242345 + m.ey.u * C01232345);
            v.ev.y = det * (m.ex.x * C12342345 - m.ex.y * C02342345 + m.ex.z * C01342345 - m.ex.t * C01242345 + m.ex.u * C01232345);
            v.ev.z = det * -(m.ex.x * C12341345 - m.ex.y * C02341345 + m.ex.z * C01341345 - m.ex.t * C01241345 + m.ex.u * C01231345);
            v.ev.t = det * (m.ex.x * C12341245 - m.ex.y * C02341245 + m.ex.z * C01341245 - m.ex.t * C01241245 + m.ex.u * C01231245);
            v.ev.u = det * -(m.ex.x * C12341235 - m.ex.y * C02341235 + m.ex.z * C01341235 - m.ex.t * C01241235 + m.ex.u * C01231235);
            v.ev.v = det * (m.ex.x * C12341234 - m.ex.y * C02341234 + m.ex.z * C01341234 - m.ex.t * C01241234 + m.ex.u * C01231234);

            return v;
        }

        /// <summary>
        /// Get Nth-column of the matrix
        /// </summary>
        public Euler4 Column(int i)
        {
            return new Euler4(ex[i], ey[i], ez[i], et[i], eu[i], ev[i]);
        }

        /// <summary> Access first column of the matrix </summary>
        public Euler4 Column0
        {
            get { return new Euler4(ex.x, ey.x, ez.x, et.x, eu.x, ev.x); }   
            set { ex.x = value.x; ey.x = value.y; ez.x = value.z; et.x = value.t; eu.x = value.u; ev.x = value.v; }        
        }

        /// <summary> Access first column of the matrix </summary>
        public Euler4 Column1
        {
            get { return new Euler4(ex.y, ey.y, ez.y, et.y, eu.y, ev.y); }
            set { ex.y = value.x; ey.y = value.y; ez.y = value.z; et.y = value.t; eu.y = value.u; ev.y = value.v; }
        }

        /// <summary> Access first column of the matrix </summary>
        public Euler4 Column2
        {
            get { return new Euler4(ex.z, ey.z, ez.z, et.z, eu.z, ev.z); }
            set { ex.z = value.x; ey.z = value.y; ez.z = value.z; et.z = value.t; eu.z = value.u; ev.z = value.v; }
        }

        /// <summary> Access first column of the matrix </summary>
        public Euler4 Column3
        {
            get { return new Euler4(ex.t, ey.t, ez.t, et.t, eu.t, ev.t); }
            set { ex.t = value.x; ey.t = value.y; ez.t = value.z; et.t = value.t; eu.t = value.u; ev.t = value.v; }
        }

        /// <summary> Access first column of the matrix </summary>
        public Euler4 Column4
        {
            get { return new Euler4(ex.u, ey.u, ez.u, et.u, eu.u, ev.u); }
            set { ex.u = value.x; ey.u = value.y; ez.u = value.z; et.u = value.t; eu.u = value.u; ev.u = value.v; }
        }

        /// <summary> Access first column of the matrix </summary>
        public Euler4 Column5
        {
            get { return new Euler4(ex.v, ey.v, ez.v, et.v, eu.v, ev.v); }
            set { ex.v = value.x; ey.v = value.y; ez.v = value.z; et.v = value.t; eu.v = value.u; ev.v = value.v; }
        }

    }
}
