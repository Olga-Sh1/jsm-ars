using JSMBase.SpecialAlgorithm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine
{
    /// <summary>Информация о выделении групп по непустоте</summary>
    public sealed class GroupInfoBase
    {
        /// <summary>Группа может быть пустой</summary>
        public Boolean CanBeNull { get; private set; }
        /// <summary>Свойства</summary>
        public PropInfo[] Props { get; private set; }
        /// <summary>Участвует в исчерпываемости</summary>
        public Boolean IsCompleteness { get; private set; }
        public GroupInfoBase(PropInfo[] prs, bool isNull, bool isCompl)
        {
            Props = prs;
            CanBeNull = isNull;
            IsCompleteness = isCompl;
            //if (sign == Signs.None) throw new ArgumentException("Недоспустимое значение для знака");
        }
        /// <summary>Конструктор для фильтра</summary><param name="prs"></param><param name="sign"></param>
        public GroupInfoBase(PropInfo[] prs) : this(prs, false, false) { }
        /// <summary>Конструтор для группы структуры</summary><param name="prs"></param><param name="isNull"></param>
        public GroupInfoBase(PropInfo[] prs, bool isNull) : this(prs, isNull, true) { }
    }

    /// <summary>Список о группах</summary>
    public sealed class GroupInfoList : IDataErrorInfo
    {
        //Dictionary<Signs, IEnumerable<GroupInfoBase>> inner;
        public IEnumerable<GroupInfoBase> Inner { get; private set; }
        public GroupInfoList(IEnumerable<GroupInfoBase> data_str /*, IEnumerable<GroupInfoBase> data_pl, IEnumerable<GroupInfoBase> data_min*/)
        {
            Inner = data_str;
            /*
            inner = new Dictionary<Signs, IEnumerable<GroupInfoBase>>();
            inner.Add(Signs.None, data_str);
            inner.Add(Signs.Plus, data_pl);
            inner.Add(Signs.Minus, data_min);
             */
        }
        string IDataErrorInfo.Error
        {
            get 
            {
                if (Inner.Where(d => d.IsCompleteness).All(d => !d.CanBeNull))
                    return "Одна из групп по исчерпываемости должна быть непустой";
                return null;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get { return ((IDataErrorInfo)(this)).Error; }
        }
    }

    public class MedicineExecContext : IExecContext<JSMMedicine>
    {
        //Структура
        readonly IEnumerable<GroupInfoBase> _inner;
        //Фильтры
        readonly IEnumerable<GroupInfoBase> _inner_s;
        readonly JSMStructure infs;
        readonly uint[][] _innIndicesCompl;
        readonly uint[][] _innIndicesInCompl;
        readonly uint[] _innOth;
        readonly MedicineContext _ctxt;
        public MedicineExecContext(JSMStructure str, GroupInfoList gilist, MedicineContext ctxt)
        {
            infs = str;
            _ctxt = ctxt;
            _inner = gilist.Inner.Where(g => g.IsCompleteness).ToArray();
            _inner_s = gilist.Inner.Where(g => !g.IsCompleteness).ToArray();
            _innIndicesCompl = GetIndices(infs, _inner, true);
            _innIndicesInCompl = GetIndices(infs, _inner_s, false);
            _innOth = ctxt.IndicesUsed.Select(a => (uint)a)
                .Except(_innIndicesCompl.SelectMany(a => a))
                .Except(_innIndicesInCompl.SelectMany(a => a)).ToArray();
        }
        BitArrayBase[] IExecContext<JSMMedicine>.GetCompletenessGroups(BitArrayBase _base)
        {
            return GetArrays(_base, _innIndicesCompl);
        }

        BitArrayBase[] IExecContext<JSMMedicine>.GetInCompletenessGroups(BitArrayBase _base)
        {
            return GetArrays(_base, _innIndicesInCompl);
        }

        private static uint[][] GetIndices(JSMStructure str, IEnumerable<GroupInfoBase> data, bool compl)
        {
            return data.Where(d => d.IsCompleteness == compl).Select(p => p.Props.Select(pp => (uint)str.IndexOf(pp)).ToArray()).ToArray();
        }

        private BitArrayBase[] GetArrays(BitArrayBase _base, uint[][] _indices)
        {
            if (infs.Count != _base.Count) throw new ArgumentException("Количество свойств не совпадает при выделении групп");
            BitArrayBase[] res = new BitArrayBase[_indices.Length];
            for (int i = 0; i < _indices.Length; i++)
                res[i] = new BitArrayBase(_base, _indices[i], true);
            return res;
        }

        bool IExecContext<JSMMedicine>.CheckComplenessIsEmpty(BitArrayBase[] data)
        {
            for (int i = 0; i < data.Length; i++)
                if (!data[i].IsEmpty())
                    return false;
            return true;
        }

        BitArrayBase IExecContext<JSMMedicine>.GetOther(BitArrayBase _base)
        {
            return new BitArrayBase(_base, _innOth, true);
        }

        BitArrayBase IExecContext<JSMMedicine>.Create(BitArrayBase[] compl, BitArrayBase[] incompl, BitArrayBase oth)
        {
            return new BitArrayBase(GetPair(_innIndicesCompl, compl)
                .Union(GetPair(_innIndicesInCompl, incompl))
                .Union(new BitBaseIndicesPair[] { new BitBaseIndicesPair(oth, _innOth) })
                .ToArray()
                );
        }

        private static IEnumerable<BitBaseIndicesPair> GetPair(uint[][] inds, BitArrayBase[] _base)
        {
            return inds.Select((a, i) => new BitBaseIndicesPair(_base[i], a));
        }

        IContext IExecContext<JSMMedicine>.Context
        {
            get { return _ctxt; }
        }
    }
}
