using System;

namespace Nav2Parser
{
    struct Vector3Half
    {
        public Half x;
        public Half y;
        public Half z;

        public Vector3Half(Half x, Half y, Half z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        } //constructor

        public Half this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException();
                } //switch
            } //get
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                } //switch
            } //set
        } //indexer
    } //struct
}
