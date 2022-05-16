using PairMatching.DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.DomainModel.DataAccessFactory
{
    public interface IDataAccessFactory
    {
        IUnitOfWork GetDataAccess();
    }
}
