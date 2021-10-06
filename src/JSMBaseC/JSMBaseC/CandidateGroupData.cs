using System;
using System.Collections.Generic;
using System.Text;

namespace JSMBaseC
{
    /// <summary>Данные по группе для сходства</summary>
    public sealed class CandidateGroupData
    {
        /// <summary>Название группы</summary>
        public String[] GroupsNames { get; private set; }
        /// <summary>Группа может быть пустой</summary>
        public Boolean CanBeNull { get; private set; }
        public CandidateGroupData(String[] groups, Boolean canBeNull = false)
        {
            GroupsNames = groups;
            CanBeNull = canBeNull;
        }

        public override string ToString()
        {
            return String.Join("+", GroupsNames);
        }
    }
}
