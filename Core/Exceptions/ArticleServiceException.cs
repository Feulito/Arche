using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Exceptions
{
    public class ArticleServiceException : Exception
    {

        public ArticleServiceException(string errorMessage) : base(errorMessage) { }

    }
}
