using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Clustering.Datasets;

namespace Socona.Clustering.Constraints
{
    public class PairConstraint:IComparable<PairConstraint>
    {
        public Record First{get;set;}
        public Record Second{get;set;}

        public PairConstraint(Record r1, Record r2)
        {
            if (r1.Id.DValue > r2.Id.DValue)
            {
                First = r2;
                Second = r1;
            }
            else
            {
                First = r1;
                Second = r2;
            }
        }

        public int CompareTo(PairConstraint other)
        {
            if (First.Id.DValue > Second.Id.DValue)
            {
                return 1;
            }
            else
                if (First.Id.DValue < Second.Id.DValue)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }

        }
        public override string ToString()
        {
            return "< (" + First.ToString() + ") , (" + Second.ToString() + ") >";
        }
    }
}
