using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acc.Api.Enum
{
    public class SQL
    {
        public SQL()
        {

        }

        public class Function
        {

            public enum Aggregate
            {
                Max,
                Min,
                Count,
                Distinct,
                Sum,
                Avg
            }

            public Aggregate Aggregrate
            {
                get;
                set;
            }

        }
        public class Method
        {

            public enum Aggregate
            {
                Save,
                Save2,
                Save3,
                Save4,
                Save5,
                Update,
                Update2,
                Update3,
                Update4,
                Update5,
                Delete,
                Delete2,
                Delete3,
                Delete4,
                Delete5,
                Select,
                GetById,
                GetById2,
                GetById3,
                GetById4,
                GetById5,
                LookUp,
                LookUpBy,
                LookUpList,
                List,
                Grid,
                Post,
                DataList,
                GetOutput
            }

            public Aggregate Aggregrate
            {
                get;
                set;
            }

        }

        public class Sort
        {
            public enum Aggregate
            {
                ASC,
                DESC
            }

            public Aggregate Aggregrate
            {
                get;
                set;
            }
        }

    }
}
