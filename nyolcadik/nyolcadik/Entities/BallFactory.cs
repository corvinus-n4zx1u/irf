using nyolcadik.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nyolcadik.Entities
{
    public class BallFactory : IToyFactory
    {
        public Color BallColor { get; set; }

        public Toy CreateNew()
        {
            return new Ball(BallColor);
        }
    }

    internal class Ball : Toy
    {
        private Color ballColor;

        public Ball(Color ballColor)
        {
            this.ballColor = ballColor;
        }

        protected override void DrawImage(Graphics g)
        {
            throw new NotImplementedException();
        }
    }
}
