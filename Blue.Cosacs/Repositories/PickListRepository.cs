// -----------------------------------------------------------------------
// <copyright file="PickListRepository.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Blue.Cosacs.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PickListRepository
    {
        private readonly IClock clock;

        public PickListRepository()
        {
            clock = StructureMap.ObjectFactory.Container.GetInstance<IClock>();
        }


    }
}
